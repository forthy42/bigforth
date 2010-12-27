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
: -` ( -- )
    ]] count [[  char ]] Literal = ?LEAVE [[ ;  immediate

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
Variable greed-counts  9 cells allot \ no more than 9 nested greedy loops
: greed' ( -- addr )  greed-counts dup @ + ;

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

: (( ( addr u -- )
    vars off varsmax off loops off greed-counts off
    ]] FORK  AHEAD BUT JOIN !end [[ BEGIN, ; immediate
: )) ( -- addr f )
    ]] ?end drop true UNNEST [[
    DONE, ]] drop false UNNEST THEN [[ ; immediate

\ greedy loops

\ Idea: scan as many characters as possible, try the rest of the pattern
\ and then back off one pattern at a time

: drops ( n -- ) 1+ cells sp@ + sp! ;

: {** ( addr -- addr addr )
    cell greed-counts +!
    greed' ]] Literal off BEGIN dup [[ BEGIN, ; immediate
' {** Alias {++ ( addr -- addr addr ) immediate
: n*} ( sys n -- )
    >r greed' ]] 1 Literal +! $? 0= UNTIL dup [[ DONE, ]] drop [[
    r@ greed' ]] Literal @ 1+ Literal U+DO FORK BUT [[
    ]] IF  I' I - [[ r@ 1- ]] Literal + drops true UNLOOP UNNEST  THEN  LOOP [[
    r@ IF  r@ ]] Literal drops [[ THEN
    rdrop ]]  dup LEAVE JOIN [[
    -cell greed-counts +! ; immediate
: **}  0 postpone n*} ; immediate
: ++}  1 postpone n*} ; immediate

\ non-greedy loops

\ Idea: Try to match rest of the regexp, and if that fails, try match
\ first expr and then try again rest of regexp.

: {+ ( addr -- addr addr )
    ]] BEGIN  [[ BEGIN, ; immediate
: {* ( addr -- addr addr )
    ]] {+ dup FORK BUT  IF  drop true  UNNEST THEN [[ ; immediate
: *} ( addr addr' -- addr' )
    ]] dup end$ u>  UNTIL [[
    DONE, ]] drop false  UNNEST  JOIN [[ ; immediate
: +} ( addr addr' -- addr' )
    ]] dup FORK BUT  IF  drop true  UNNEST [[
    DONE, ]] drop dup  LEAVE [[ BEGIN, ]] THEN *} [[ ; immediate

: // ( -- ) ]] {* 1+ *} [[ ; immediate

\ alternatives

\ idea: try to match one alternative and then the rest of regexp.
\ if that fails, jump back to second alternative

: THENs ( sys -- )  BEGIN  dup  WHILE  ]] THEN [[  REPEAT  drop ;

: {{ ( addr -- addr addr ) \ regexp-pattern
    0 ]] dup dup FORK  IF  2drop true UNNEST  BUT  JOIN [[ vars @ ; immediate
: || ( addr addr -- addr addr ) \ regexp-pattern
    vars @ varsmax @ max varsmax !  vars !
    ]] AHEAD  BUT  THEN  drop [[
    ]] dup dup FORK  IF  2drop true UNNEST  BUT  JOIN [[ vars @ ; immediate
: }} ( addr addr -- addr ) \ regexp-pattern
    vars @ varsmax @ max vars !  drop
    ]] AHEAD  BUT  THEN  drop LEAVE [[  THENs ; immediate

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
: s>>  ( addr -- addr )  dup to >>ptr ;
: << ( run-addr addr u -- run-addr )
    <<ptr >>ptr over - >>string $+!
    >>string $+! dup to <<ptr ;
: <<"  '" parse postpone SLiteral postpone << ; immediate
: >>string@ ( -- addr u )
    >>string $@ ;
: >>string0 ( addr u -- addr u )  s" " >>string $!
    0 to >>ptr  over to <<ptr ;
: >>next ( -- addr u ) <<ptr end$ over - ;
: >>rest ( -- ) >>next >>string $+! ;
: >> ( addr -- addr ) ]] <<ptr >>ptr u> ?LEAVE ?end [[ ; immediate
: s// ( addr u -- ptr ) ]] >>string0 (( // s>> [[ ; immediate
: //s ( ptr -- ) ]] )) drop >>rest >>string@ [[ ; immediate
: //o ( ptr addr u -- addr' u' ) ]] << //s [[ ; immediate
: //g ( ptr addr u -- addr' u' ) ]] << LEAVE //s [[ ; immediate
