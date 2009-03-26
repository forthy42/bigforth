\ anonymous definitions in a definition

\ *** Stephen: check if this is complete ***

: :[ ( compile-time: -- orig colon-sys )
    state @ IF  <headerless> @ last @
	POSTPONE AHEAD  true  ELSE  false  THEN
     postpone [ :noname ; immediate
     
: ]: ( compile-time: orig colon-sys -- ; run-time: -- xt )
    discard-sinline  POSTPONE ; >r
    IF ]  POSTPONE THEN  r> POSTPONE Literal
	last ! <headerless> !  discard-sinline
    ELSE  r>  THEN ( xt ) ; immediate

0 [IF]
: if-else ( ... f xt1 xt2 -- ... )
\ Postscript-style if-else
    rot IF
       drop
    ELSE
       nip
    THEN
    execute ;

: test ( f -- )
    :[ ." true" ]:
    :[ ." false" ]:
    if-else ;
   
1 test cr \ writes "true"
0 test cr \ writes "false"
[THEN]
