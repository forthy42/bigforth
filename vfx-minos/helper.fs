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

: & ' >body state @ IF postpone Literal THEN ; immediate

: u>= u< 0= ;
: @+ dup @ swap cell+ ;
: rdrop r> r> drop >r ;

: list> ( thread -- element )
  BEGIN  @ dup  WHILE  dup r@ execute
  REPEAT  drop rdrop ; \ restrict

-1 cells Constant -cell
: over2  2 pick ;
