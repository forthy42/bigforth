\ helper words for VFX Forth

: | ; \ headerless becomes a noop

\ memory words

vocabulary memory

also memory definitions
: NewPtr ( len -- addr )  allocate throw ;
: NewHandle ( len -- addr )  NewPtr 1 cells NewPtr tuck ! ;
: DisposPtr ( addr -- )  free throw ;
: DisposHandle ( addr -- )  dup @ DisposPtr DisposPtr ;
: Handle! ( len addr -- )  >r NewPtr r> ! ;
: HandleOff ( addr -- )  dup @ free throw off ;
previous definitions

synonym AVariable Variable
synonym A, ,
synonym AConstant Constant
synonym Patch Defer
synonym << lshift
synonym >> arshift

: cont >r ;
: push r> swap dup @ >r >r cont r> r> swap ! ;

: & ' >body state @ IF postpone Literal THEN ; immediate

: u>= u< 0= ;
: @+ dup @ swap cell+ ;
: rdrop r> r> drop >r ;

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

: wx@ w@ wextend ;
: wx@+ dup wx@ swap 2+ ;

: v! ! ;

\ bit words

: +bit ( addr n -- ) 8 /mod swap >r + 1 r> lshift over c@ or swap c! ;
: -bit ( addr n -- ) 8 /mod swap >r + 1 r> lshift invert over c@ and swap c! ;
: bit@ ( addr n -- ) 8 /mod swap >r + 1 r> lshift swap c@ and 0<> ;

\ Address marker

synonym AValue Value
