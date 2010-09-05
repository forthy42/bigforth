\ dynamic string handling                              10aug99py

| Code 5*4/ ( n -- n*9/8+padding )
    &12 cells AX AX *4 DI) AX lea
    2 # AX shr  -4 cells # AX and
    Next end-code macro
| Code 3*4/ ( n -- n*7/8+padding )
    &12 cells AX AX *2 DI) AX lea
    2 # AX shr  -4 cells # AX and
    Next end-code macro

\ this guarantees that no more than 1/4 of the memory is wasted

also Memory
: $padding ( n -- n' )
  [ $6 cells ] Literal + [ -$4 cells ] Literal and ; macro
: $! ( addr1 u addr2 -- )
  dup @ IF  dup HandleOff  THEN
  over $padding over Handle! @
  over >r  rot over cell+  r> move 2dup ! + cell+ bl swap c! ;
: $@len ( addr -- u )  @ @ ;                    macro 0 :@ T&P
: $@ ( addr1 -- addr2 u )  @ @+ swap ;          \ macro
: $!len ( u addr -- )
    >r dup $padding
    BEGIN  r@ GetHandleSize
	2dup >  WHILE  5*4/ r@ swap SetHandleSize  REPEAT
    2dup 3*4/ <= IF   r@ over2 SetHandleSize  THEN
    2drop r> @ ! ;
: $del ( addr off u -- )   >r >r dup $@ r> /string r@ delete
  dup $@len r> - swap $!len ;
: $ins ( addr1 u addr2 off -- ) >r
  2dup dup $@len rot + swap $!len  $@ 1+ r> /string insert ;
: $+! ( addr1 u addr2 -- )
  >r dup r@ $@len + r@ $!len r> $@ + over - swap move ;
: $off ( addr -- ) dup @ IF  HandleOff  ELSE  drop  THEN ;
toss

\ dynamic string handling                              12dec99py

: $split ( addr u char -- addr1 u1 addr2 u2 )
  >r 2dup r> scan dup >r 1 /string 2swap r> - 2swap ;

: $iter ( .. $addr char xt -- .. ) { char xt }
  $@ BEGIN  dup  WHILE  char $split >r >r xt execute r> r>
     REPEAT  2drop ;
