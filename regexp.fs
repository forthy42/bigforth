\ Regexp compile

\ The idea of the parser is the following:
\ As long as there's a match, continue
\ On a mismatch, LEAVE.
\ Insert appropriate control structures on alternative branches
\ Keep the old pointer (backtracking) on the stack
\ I try to keep the syntax as close to a real regexp system as possible
\ All regexp stuff is compiled into one function as forward branching
\ state machine

\ bulk-postponing

: [[ ; \ token to end bulk-postponing
: ]] BEGIN  >in @ ' ['] [[ <> WHILE  >in ! postpone postpone  REPEAT
    drop ; immediate

\ Charclasses

0 Value cur-class
: charclass ( -- )  Create here dup to cur-class $20 dup allot erase ;
: +char ( char -- )  cur-class swap +bit ;
: -char ( char -- )  cur-class swap -bit ;
: ..char ( start end -- )  1+ swap ?DO  I +char  LOOP ;
: or! ( n addr -- )  dup @ rot or swap ! ;
: and! ( n addr -- )  dup @ rot and swap ! ;
: +class ( class -- )  $20 0 ?DO  @+ swap
        cur-class I + or!  cell +LOOP  drop ;
: -class ( class -- )  $20 0 ?DO  @+ swap invert
        cur-class I + and!  cell +LOOP  drop ;

Code char? ( addr class -- addr' flag )  DX pop
    .b DX ) CX movsx  DX inc  DX push
    CX AX ) bt  b makeflag  Next end-code macro :dx :f T&P

\ Charclass tests

: c? ( addr class -- )   ]] char? 0= ?LEAVE [[ ; immediate
: -c? ( addr class -- )  ]] char?    ?LEAVE [[ ; immediate

charclass digit  '0 '9 ..char
charclass blanks 0 bl ..char
\ bl +char #tab +char #cr +char #lf +char ctrl L +char
charclass letter 'a 'z ..char 'A 'Z ..char
charclass any    0 $FF ..char #lf -char

: \d ( addr -- addr' )   ]] digit c?        [[ ; immediate
: \s ( addr -- addr' )   ]] blanks c?       [[ ; immediate
: .? ( addr -- addr' )   ]] any c?          [[ ; immediate
: -\d ( addr -- addr' )  ]] digit -c?       [[ ; immediate
: -\s ( addr -- addr' )  ]] blanks -c?      [[ ; immediate
: ` ( -- )
    ]] count [[  char ]] Literal <> ?LEAVE [[ ;  immediate

\ A word for string comparison

\ : $= ( addr1 addr2 u -- f )  tuck compare ;
\ : ,=" ( addr u -- ) tuck ]] dup SLiteral $= ?LEAVE Literal + noop [[ ;
\ : =" ( <string>" -- )  '" parse ,=" ; immediate
: =" '" parse bounds ?DO
	]] count [[ I c@ ]] Literal <> ?LEAVE [[
    LOOP ; immediate

\ loop stack

Variable loops  $40 cells allot
: loops> ( -- addr ) -1 loops +!  loops @+ swap cells + @ ;
: >loops ( addr -- ) loops @+ swap cells + ! 1 loops +! ;
: BEGIN, ( -- )  ]] BEGIN [[ >loops ;
: DONE, ( -- )  loops @ IF  loops> ]] DONE [[ THEN ]] noop [[ ;

\ variables

Variable vars   &18 cells allot
Variable varstack 9 cells allot
Variable varsmax
: >var ( -- addr ) vars @+ swap 2* cells +
    vars @ varstack @+ swap cells + !
    1 vars +! 1 varstack +! ;
: var> ( -- addr ) -1 varstack +!
    varstack @+ swap cells + @
    1+ 2* cells vars + ;

\ start end

0 Value end$
0 Value start$
: !end ( addr u -- addr )  over + to end$ dup to start$ ;
: $? ( addr -- addr flag ) dup end$ u< ; macro
: ^? ( addr -- addr flag ) dup start$ u> ; macro
: ?end ( addr -- addr ) ]] dup end$ u> ?LEAVE [[ ; immediate

\ start and end

: \^ ( addr -- addr )
    ]] ^? ?LEAVE [[ ; immediate
: \$ ( addr -- addr )
    ]] $? ?LEAVE [[ ; immediate

\ regexp block

\ FORK/JOIN are like AHEAD THEN, but producing a call on AHEAD
\ instead of a jump.

: (( ( addr u -- )  vars off varsmax off loops off
    ]] FORK  AHEAD BUT JOIN !end [[ BEGIN, ; immediate
: )) ( -- addr f )
    ]] ?end drop true EXIT [[
    DONE, ]] drop false EXIT THEN [[ ; immediate

\ greedy loops

\ Idea: scan as many characters as possible, try the rest of the pattern
\ and then back off one pattern at a time

: drops ( n -- ) 1+ cells sp@ + sp! ;

: {** ( addr -- addr addr )
    0 ]] Literal >r BEGIN dup [[ BEGIN, ; immediate
' {** Alias {++ ( addr -- addr addr ) immediate
: n*} ( sys n -- )  >r ]] r> 1+ >r $? 0= UNTIL dup [[ DONE, ]] drop [[
    r@ IF r@ ]] r@ Literal u< IF  r> 1+ drops false  EXIT  THEN [[ THEN
    r@ ]] r> 1+ Literal U+DO FORK BUT [[
    ]] IF  I' I - [[ r@ 1- ]] Literal + drops true UNLOOP EXIT  THEN  LOOP [[
    r@ IF  r@ ]] Literal drops [[ THEN
    rdrop ]] false  EXIT  JOIN [[ ; immediate
: **}  0 postpone n*} ; immediate
: ++}  1 postpone n*} ; immediate

\ non-greedy loops

\ Idea: Try to match rest of the regexp, and if that fails, try match
\ first expr and then try again rest of regexp.

: {+ ( addr -- addr addr )
    ]] BEGIN  [[ BEGIN, ; immediate
: {* ( addr -- addr addr )
    ]] {+ dup FORK BUT  IF  drop true  EXIT THEN [[ ; immediate
: *} ( addr addr' -- addr' )
    ]] dup end$ u>  UNTIL [[
    DONE, ]] drop false  EXIT  JOIN [[ ; immediate
: +} ( addr addr' -- addr' )
    ]] dup FORK BUT  IF  drop true  EXIT [[
    DONE, ]] drop false  EXIT  THEN *} [[ ; immediate

: // ( -- ) ]] {* 1+ *} [[ ; immediate

\ alternatives

\ idea: try to match one alternative and then the rest of regexp.
\ if that fails, jump back to second alternative

: THENs ( sys -- )  BEGIN  dup  WHILE  ]] THEN [[  REPEAT  drop ;

: {{ ( addr -- addr addr )  0 ]] dup BEGIN [[  vars @ ; immediate
: || ( addr addr -- addr addr ) vars @ varsmax @ max varsmax !
    ]] nip AHEAD [[ >r vars !
    ]] DONE drop dup [[ r> ]] BEGIN [[ vars @ ; immediate
: }} ( addr addr -- addr addr ) vars @ varsmax @ max vars !
    ]] nip AHEAD [[ >r drop
    ]] DONE drop LEAVE [[ r> THENs ; immediate

\ match variables

: \( ( addr -- addr )  ]] dup [[
    >var ]] ALiteral ! [[ ; immediate
: \) ( addr -- addr )  ]] dup [[
    var> ]] ALiteral ! [[ ; immediate
: \0 ( -- addr u )  start$ end$ over - ;
: \: ( i -- )
    Create 2* 1+ cells vars + ,
  DOES> ( -- addr u ) @ 2@ tuck - ;
: \:s ( n -- ) 0 ?DO  I \:  LOOP ;
9 \:s \1 \2 \3 \4 \5 \6 \7 \8 \9

\ replacements

0 Value >>ptr
0 Value <<ptr
Variable >>string
: >>  ( addr -- addr )  dup to >>ptr ;
: << ( run-addr addr u -- run-addr )
    <<ptr 0= IF  start$ to <<ptr  THEN
    >>string @ 0= IF  s" " >>string $!  THEN
    <<ptr >>ptr over - >>string $+!
    >>string $+! dup to <<ptr ;
: <<"  '" parse postpone SLiteral postpone << ; immediate
: >>string@ ( -- addr u )
    >>string $@len dup allocate throw
    swap 2dup >>string $@ drop -rot move
    >>string $off  0 to >>ptr  0 to <<ptr ;
: >>next ( -- addr u ) <<ptr end$ over - ;
: >>rest ( -- ) >>next >>string $+! ;
: s// ( addr u -- ptr ) ]] (( // >> [[ ; immediate
: //o ( ptr addr u -- addr' u' ) ]] << )) drop >>rest >>string@ [[ ; immediate
: //g ( ptr addr u -- addr' u' ) ]] << LEAVE )) drop >>string@ [[ ; immediate
