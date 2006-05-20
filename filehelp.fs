\ tools for easy file manipulation

0 Value slurp-mem
0 Value lines-mem
: slurp-fid ( fid -- addr u )
    dup >r file-size throw drop dup allocate throw dup to slurp-mem
    swap 0. r@ reposition-file throw
    2dup r@ read-file throw nip
    r> close-file throw ;
: burp ( -- )
    slurp-mem IF  slurp-mem free throw  0 to slurp-mem  THEN
    lines-mem IF  lines-mem free throw  0 to lines-mem  THEN ;

: >lines ( addr u -- addr' u' )
    $2000 cells allocate throw dup to lines-mem dup >r >r
    BEGIN  r> $1000 0 DO  >r
	    BEGIN  dup  WHILE
		    2dup #lf scan dup >r 1 /string 2swap r> -
		    r@ 2! r> 2 cells + >r
	    REPEAT  r>  LOOP >r
	dup WHILE
	r> r> tuck - tuck $2000 cells + resize throw dup >r + >r
    REPEAT  2drop r> r> tuck - tuck resize throw swap ;

