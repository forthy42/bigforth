\ adjust pathes                                        22jun98py

include fileop.fb

DOS also fileop also minos also

: adjust-path-id ( addr-id u1 addr-path u2 addr-file u3 -- )
  2dup r/w open-file throw >r
  s" tmp.cnf" r/w output-file
  BEGIN  scratch $100 r@ read-line throw  WHILE
         scratch over 5 min s" Path " compare 0=
         IF    drop .' Path "'
               pathsep emit 2over type  '" emit
         ELSE  scratch over dup 7 - /string s" date-id" compare
               0= IF  drop >r >r
                      .'   s" ' 2over type .' " date-id'
                      r> r>
               ELSE   scratch swap type  THEN
         THEN  cr
  REPEAT  drop eot  r> close-file throw
  s" tmp.cnf" 2swap cp 0" tmp.cnf" fdelete drop 2drop 2drop ;
