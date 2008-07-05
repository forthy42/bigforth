\ helper words for VFX Forth

: | ; \ headerless becomes a noop

\ memory words

vocabulary memory

also memory definitions
: NewPtr ( len -- addr )  allocate throw ;
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
