\ substitute stuff

Vocabulary macros

L" <>" AConstant delimiters

\ also macros definitions
: macro: ( addr u -- ) Create here 0 , $! DOES> $@ ;
\ previous Forth definitions

: replaces ( addr1 len1 addr2 len2 -- )
    2dup & macros search-wordlist IF  nip nip >body $!
    ELSE
	get-current >r & macros set-current
	['] macro: execute-parsing
	r> set-current
    THEN ;

Variable macro$

2Variable 'delims
Create ldel-buf 8 allot

: set-delimiter ( lead end -- )  over swap 'delims 2!
    ldel-buf xc!+ drop ;
: get-delimiter ( -- lead end )  'delims 2@ ;

: default-delimiter ( -- ) '%' dup set-delimiter ;
default-delimiter

: i18n-delimiter ( -- )
    delimiters locale@ drop xc@+ swap xc@+ nip set-delimiter ;

: xscan ( addr u xc -- addr' u' ) >r bounds
    BEGIN  xc@+ r@ <> WHILE  2dup u<=  UNTIL  ELSE  xchar-  THEN
    swap over - rdrop ;

: $xsplit ( addr u xc -- addr1 u1 addr2 u2 )
  >r 2dup r> xscan dup >r +x/string 2swap r> - 2swap ;

: $substitute ( addr1 len1 -- addr2 len2 n )
    s" " macro$ $! 0 >r
    BEGIN  dup  WHILE  'delims cell+ @ $split
	    2swap macro$ $+! dup IF
		over xc@+ nip 'delims cell+ @ = IF
		    over dup xchar+ over - macro$ $+! +x/string
		ELSE
		    'delims @ $xsplit 2swap dup 0= IF
			2drop ldel-buf dup 8 x-size macro$ $+! r> 1+ >r
		    ELSE
			2dup & macros search-wordlist  IF
			    execute 2swap 2drop r> 1+ >r macro$ $+!
			ELSE  2drop  THEN
		    THEN
		THEN
	    THEN
    REPEAT  2drop macro$ $@ r> ;

: substitute ( addr1 len1 addr2 len2 -- addr2 len3 n )
    2swap $substitute >r
    2swap rot umin 2dup >r >r move r> r> r> ;

: unescapes ( addr1 u1 dest -- dest u2 )  dp push dup >r dp !
    bounds ?DO
	I c@ dup '%' = IF  dup c,  THEN  c,
    LOOP  r> here over - ;