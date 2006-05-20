\               *** Search order word set ***          29nov92py

: WORDLIST        ( -- wid )  align here 0 voc, ;

: GET-ORDER       ( -- wid1 .. widn n )  vp dup @ cell/ dup >r
  FOR  cell+ dup >r @ r>  NEXT  drop r> 1+ ;
: SET-ORDER       ( wid1 .. widn n -- )
  -1 case? IF  Only exit  THEN  1- dup cells vp !  >r context r>
  1+ 0 ?DO  dup >r ! r> cell-  LOOP  drop ;

: GET-CURRENT     ( -- wid )  current @ ;
: SET-CURRENT     ( wid -- )  current ! ;

: FORTH-WORDLIST  ( -- wid ) also forth context @ toss ;
: SEARCH-WORDLIST ( addr u wid -- cfa state / f )
  >r 'findpad place 'findpad 'prehash  r> (find
  IF found ELSE drop 0 THEN ;
