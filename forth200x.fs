\ forth 200x stuff

: Synonym  Header  -2 allot bl word find dup 0= IF no.extensions THEN
    dup 0> IF  immediate  THEN
    1 and 0= IF  restrict  THEN  A,
    $20 last @ dup >r c@ or r> c!  reveal ;

\ defer@/defer!/action-of

: defer@ ( xt -- xt )  >body @ ;
: defer! ( xt1 xt2 -- )  >body ! ;
Synonym action-of what's

