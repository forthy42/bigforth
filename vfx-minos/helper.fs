\ helper words for VFX Forth

: | ; \ headerless becomes a noop

Vocabulary editor \ empty vocabulary

\ memory words

vocabulary memory

also memory definitions
: NewPtr ( len -- addr )  allocate throw ;
: NewHandle ( len -- addr )  NewPtr 1 cells NewPtr tuck ! ;
: DisposPtr ( addr -- )  free throw ;
: DisposHandle ( addr -- )  dup @ DisposPtr DisposPtr ;
: Handle! ( len addr -- )  >r NewPtr r> ! ;
: HandleOff ( addr -- )  dup @ free throw off ;
: SetHandleSize ( addr -- ) >r
    r@ @ swap resize throw r> ! ;
: DelFix ( addr root -- ) dup @ 2 pick ! ! ;
: NewFix  ( root len # -- addr )
  BEGIN  2 pick @ ?dup  0= WHILE  2dup * NewPtr
         over 0 ?DO  dup 4 pick DelFix 2 pick +  LOOP  drop
  REPEAT  >r drop r@ @ rot ! r@ swap erase r> ;

previous definitions

synonym AVariable Variable
synonym A, ,
synonym AConstant Constant
synonym ALiteral Literal
synonym Patch Defer
synonym << lshift
synonym >> arshift
synonym toss previous
synonym extend s>d
synonym #! \

: cont >r ;
: push r> swap dup @ >r >r cont r> r> swap ! ;

: & ' >body state @ IF postpone Literal THEN ; immediate

: 0>= 0< 0= ;
: 0<= 0> 0= ;
: u>= u< 0= ;
: u<= u> 0= ;
: @+ dup @ swap cell+ ;
: rdrop postpone r> postpone drop ; immediate

: list> ( thread -- element )
  BEGIN  @ dup  WHILE  dup r@ execute
  REPEAT  drop rdrop ; \ restrict

-1 cells Constant -cell
: over2  2 pick ;

: tolower ( char -- )  dup 'A' 'Z' 1+ within IF  bl +  THEN ;
\ Structs for interpreter                              28nov92py

Variable (i)

: [DO]  ( n-limit n-index -- ) \ gforth bracket-do
  >in @ -rot
  DO   I (i) ! dup >r >in ! interpret r> swap +LOOP  drop ;
                                                      immediate

: [?DO] ( n-limit n-index -- ) \ gforth bracket-question-do
  2dup = IF 2drop postpone [ELSE] ELSE postpone [DO] THEN ;
                                                      immediate

: [+LOOP] ( n -- ) \ gforth bracket-question-plus-loop
  rdrop ;                                             immediate

: [LOOP] ( -- ) \ gforth bracket-loop
  1 rdrop ;                                           immediate

: [FOR] ( n -- ) \ gforth bracket-for
  0 swap postpone [DO] ;                              immediate

: [NEXT] ( n -- ) \ gforth bracket-next
  -1 rdrop ;                                          immediate

: [I]  (i) @ state @ IF  postpone Literal  THEN ;     immediate

[defined] $linux [IF]
    0 Constant unix
[THEN]

: \needs postpone [defined] IF postpone \ THEN ;

: ,0"   ( -- )  '"' parse  here swap dup allot move 0 c, ;

: onlyforth  only forth ;

: perform @ execute ;

: macro ; \ indicates macro

: Module  >in @ Vocabulary >in !
    get-order get-current swap 1+ set-order
    also ' execute also definitions ;

: Module;  previous previous definitions previous ;

: c@+ count swap ;
: w@+ dup w@ swap 2+ ;
: @+ dup @ swap cell+ ;

: c!+  tuck c! char+ ;
: w!+  tuck w! 2+ ;
: !+  tuck ! cell+ ;

: wextend dup $8000 and negate or ;
: cextend dup $80 and negate or ;

: cx@ c@ cextend ;
: wx@ w@ wextend ;
: wx@+ dup wx@ swap 2+ ;

: v! ! ;

\ bit words

: +bit ( addr n -- ) 8 /mod swap >r + 1 r> lshift over c@ or swap c! ;
: -bit ( addr n -- ) 8 /mod swap >r + 1 r> lshift invert over c@ and swap c! ;
: bit@ ( addr n -- ) 8 /mod swap >r + 1 r> lshift swap c@ and 0<> ;

\ Address marker

synonym AValue Value

Vocabulary DOS

also DOS definitions

extern: char * setlocale ( int , char * ); ( locale addr -- addr )
extern: void gettimeofday ( int * , int * ); ( timeval timezone -- )

synonym env$ readenv

Create timeval   0 , 0 ,
Create timezone  0 , 0 ,

previous definitions

\ zero string

synonym 0" z"

: >len ( addr -- addr u ) dup zstrlen ;

: 0place ( addr u addr -- )
  swap 2dup + >r move 0 r> c! ;

\ special characters

$08 Constant #bs         $0D Constant #cr
$0A Constant #lf         $1B Constant #esc
$09 Constant #tab        $07 Constant #bell

\ division - make sure everything is floored

: /mod ( n1 n2 -- rem quot ) >r s>d r> fm/mod ;
: / /mod nip ;
: mod /mod drop ;

: ud/mod ( ud1 u2 -- urem udquot )  >r 0 r@ um/mod r> swap >r
                                    um/mod r> ;
Synonym m/mod fm/mod
: d*     ( ud1 ud2 -- udprod )      >r swap >r 2dup um*
                                    2swap r> * swap r> * + + ;

\ timer handling

1 cells +user time

also DOS
: timer@  timeval timezone gettimeofday
  timeval 2@ swap $CB9CB68 um* nip swap
  $2000000 um* #675 ud/mod drop nip + ;
previous
: !time timer@ time ! ;
: ms>time ( ms -- time )
  $C6D750EB um* $3FFFFFF, d+ 6 lshift swap $1A rshift or ;
: >us ( time -- dus )  #86400000 um*
  #1000 um* >r >r #1000 um* r> + r> rot 0< s>d d- ;
: after ( ms -- time ) ms>time timer@ + ;
: till ( time -- )
    BEGIN dup timer@ - 0> WHILE  pause  REPEAT drop ;
: wait  ( ms -- )    after till ;
\ synonym ms wait
: timeout? ( time -- time f )  pause dup timer@ - 0> 0= ;
Defer idle ' 2drop IS idle

\ clearstack

: clearstack depth ndrop ;

\ constant adders

: 6+ 6 + ;
: 3+ 3 + ;

\ multitasker is needed

include /usr/share/doc/VfxForth/Lib/Lin32/MultiLin32.fth

synonym wake restart
synonym sleep halt

\ keyboard state

Variable kbshift

\ compile only

: restrict ;

: BUT  swap ; immediate

\ loops

: FOR  0 postpone Literal postpone swap postpone DO ; immediate
: NEXT  -1 postpone Literal postpone +LOOP ; immediate

\ digit?

: toupper ( char -- char' )
    dup [char] a - [ char z char a - 1 + ] Literal u<  bl and - ;

: ?lit, ( n -- ) state @ IF postpone Literal THEN ;

: ctrl    ( -- n )  char toupper $40 xor ?lit, ;
                                            immediate

: digit?   ( char -- digit true/ false ) \ gforth
  toupper [char] 0 - dup 9 u> IF
    [ char A char 9 1 + -  ] literal -
    dup 9 u<= IF
      drop false EXIT
    THEN
  THEN
  dup base @ u>= IF
    drop false EXIT
  THEN
  true ;

Create bases  #10 c,  $10 c, %10 c, #10 c, 0 c,
\             10      16     2      10     char
: getbase  ( addr u -- addr' u' )  over c@ '#' - dup 5 u<
  IF  bases + c@ base ! 1 /string  ELSE  drop  THEN ;
: getsign  over c@ '-' = dup >r negate /string r> ;
Defer char@ ' count IS char@
: s>number  ( addr len -- d )  base push
  getsign >r  getbase  base @ 0=
  IF  over + swap char@ >r swap over - dup 0= >r 1 = >r
      c@ ''' = r> and r> or dpl !  r> 0 rdrop  EXIT  THEN
  dpl on  getsign r> xor >r  0 0 2swap
  BEGIN  dup >r >number  dup r> =
         IF  rdrop 2drop dpl off  EXIT  THEN
         dup  WHILE  dup dpl ! over c@ -3 and ',' = 0=
         IF  rdrop 2drop dpl off  EXIT  THEN  1 /string
     dup 0= UNTIL  THEN  2drop r> IF  dnegate  THEN ;

\ case?

: case? ( n1 n2 -- n1 false / true )
    over = dup IF nip THEN ;

0 value script?
