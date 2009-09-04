\ does compiler

\ compiles DOES> in a Gforth-like way to avoid code/data problems

: does? ( xt -- xt flag )
    dup c@ $E8 = IF
	dup cfa@ 2@ $8B50F48704C68306. d=
    ELSE  false  THEN ;

: does, ( xt -- xt / early exit )
    does? IF
	$68 c, here relon here 14 + ,
	$68 c, here relon dup >body , cfa@ compile, here 5 - dup c@ 1+ swap c!
	rdrop drop EXIT  THEN ;

' does, is 'cfa,
