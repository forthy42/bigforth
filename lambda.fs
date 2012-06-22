\ anonymous definitions in a definition

: [: ( compile-time: -- orig colon-sys )
     state @ IF  last @  postpone SCOPE POSTPONE AHEAD  true  ELSE  false  THEN
     :noname ; immediate
     
: ;] ( compile-time: orig colon-sys -- ; run-time: -- xt )
    POSTPONE ; >r IF ]  POSTPONE THEN  r> POSTPONE ALiteral  postpone ENDSCOPE  last !
    ELSE  r>  THEN ( xt ) ; immediate
\\\
: if-else ( ... f xt1 xt2 -- ... )
\ Postscript-style if-else
    rot IF
       drop
    ELSE
       nip
    THEN
    execute ;

: test ( f -- )
    [: ." true" ;]
    [: ." false" ;]
    if-else ;
   
1 test cr \ writes "true"
0 test cr \ writes "false"
