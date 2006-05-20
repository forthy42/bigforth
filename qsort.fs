\ quicksort                                            30apr97py

: pivot@ ( addr u -- addr u pivot ) 2dup 2/ cells + @ ; macro
Defer lex       ' <= IS lex

: split< ( addr u pivot -- addr' u' addr" u" )
  >r 2dup cells bounds r> -rot
  ?DO  dup i @ lex
       IF  BEGIN  -cell +i' ?LEAVE  i' @ over lex  UNTIL
           i @ i' @ i ! i' !  THEN
       cell +ITERATE  DONE  i' nip  UNLOOP  >r
  r@ 2 pick - cell/ under - r> swap ;
: sort ( addr u -- )
  BEGIN  dup 1 >  WHILE  pivot@ split< recurse  REPEAT  2drop ;
