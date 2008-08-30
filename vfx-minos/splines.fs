\ Bezierkurven (Einhüllung)                            01jul98py
: >sc >r r@ 2* 0 ?DO  i pick $10 <<      i pin  LOOP r> ;
: sc> >r r@ 2* 0 ?DO  i pick $F >> 1+ 2/ i pin  LOOP r> ;
: 2-rot 2swap 2>r 2swap 2r> ;
: z1/2  rot + 1+ 2/ >r + 1+ 2/ r> ;
: >bezier ( z1 .. zn n d -- z1 .. zc c ) ?dup 0= ?exit  over
  BEGIN 1- dup WHILE  >r 2over r> -rot 2>r -rot 2>r
     dup BEGIN 2>r 2over z1/2 2r> 2swap 2>r 1- dup 0= UNTIL drop
     dup BEGIN 2r> 2swap 1- dup 0< UNTIL drop
  REPEAT drop
  2dup 2>r 1- recurse nip nip 2r> over 1-
  BEGIN  2r> 2-rot 1- dup 0= UNTIL  drop
  rot >r 1- recurse r> + 2- ;
