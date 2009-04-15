\ Internationalization and localization

\ This implementation keeps everything in memory, LSIDs are linked
\ together in lists. Each LSID has also a number, which is used to go
\ from native to local LSID.

\ LSIDs

AVariable lsids
0 Value lsid#

: native@ ( lsid -- addr u )  cell+ cell+ @+ swap ;
: id#@ ( lsid -- n )  cell+ @ ;

: search-lsid ( addr u -- lsid )  lsids
    BEGIN  @ dup  WHILE  >r 2dup r@ native@ str= r> swap  UNTIL  THEN
    nip nip ;

: (l")  r> dup cell+ cell+ @+ + aligned >r ;
: (l2)  r> @+ >r ; 

: append-list ( addr list -- )
    BEGIN  dup @  WHILE  @  REPEAT  ! ;

: s, ( addr u -- )  dup , here swap dup allot move align ;
: l, ( addr u -- )
    here lsids append-list 0 A, lsid# dup , 1+ to lsid# s, ;

: LLiteral  2dup search-lsid dup  IF
        nip nip postpone (l2) A,
    ELSE  drop postpone (l") l,  THEN ; immediate

: L" ( "lsid<">" -- lsid ) '"' parse
    state @ IF  postpone LLiteral
    ELSE  2dup search-lsid dup  IF
            nip nip
        ELSE  drop align here >r l, r>  THEN
    THEN  ; immediate

\ deliberately unique string
: LU" ( "lsid<">" -- lsid ) '"' parse
    state @ IF  postpone (l") l,
    ELSE  align here >r l, r>
    THEN  ; immediate

: .lsids ( lsids -- ) list> native@ type cr ;

\ locale@ stuff

$3 Constant locale-depth \ lang country variances
Variable locale-stack  locale-depth cells allot

: >locale ( lsids -- )
    locale-stack @+ swap cells + !  1 locale-stack +!
    locale-stack @ locale-depth u>= abort" locale stack full" ;
: locale-drop ( -- )  -1 locale-stack +!
    locale-stack @ locale-depth u>= abort" locale stack empty" ;
: locale' ( -- addr )  locale-stack @+ swap 1- cells + @ ;

: Locale  Create 0 , DOES>  locale-stack off >locale ;
: Country  Create 0 , , DOES>  locale-stack off dup cell+ @ >locale >locale ;

: set-language ( lang -- ior )  locale-stack off >locale 0 ;
: set-country ( country -- ior )
    dup cell+ @ set-language >locale 0 ;

: search-lsid# ( id# lsids -- lsid )
    BEGIN  @ dup  WHILE  >r dup r@ cell+ @ = r> swap  UNTIL  THEN
    nip ;

Variable last-namespace

: locale@ ( lsid -- addr u )  last-namespace off
    locale-stack @ IF
        dup >r id#@
        locale-stack @+ swap cells bounds swap cell- DO
	    dup I @ search-lsid# dup IF
		I last-namespace !
		nip native@ unloop rdrop EXIT  THEN
            drop
        -cell +LOOP  drop r>
    THEN  native@ ;

: lsid@ ( lsid -- addr u )  last-namespace @  IF
	dup >r id#@
	last-namespace @ locale-stack cell+  DO
	    dup I @ search-lsid# dup IF
		nip native@ unloop rdrop EXIT  THEN
            drop
	-cell +LOOP  drop r>
    THEN  native@ ;

: locale! ( addr u lsid -- ) >r
    2dup r@ locale@ str= IF  rdrop 2drop  EXIT  THEN
    r> id#@ here locale' append-list 0 A, , s, ;

: native-file ( fid -- ) >r
    BEGIN  pad $1000 r@ read-line throw  WHILE
	    pad swap l,  REPEAT
    drop r> close-file throw ;

: locale-file ( fid -- ) >r  lsids
    BEGIN  @ dup  WHILE  pad $1000 r@ read-line throw
	    IF  pad swap over2 locale!  ELSE  drop  THEN  REPEAT
    drop r> close-file throw ;

: included-locale ( addr u -- )  r/o open-file throw
    locale-file ;

: included-native ( addr u -- )  r/o open-file throw
    native-file ;

[defined] getpathspec 0= [IF]
    : getpathspec ( -- fd )  use isfile@ ;
[THEN]

: include-locale ( -- )  getpathspec locale-file ;
: include-native ( -- )  getpathspec native-file ;

\ easy use

: x" state @ IF  postpone l" postpone locale@
    ELSE  ['] l" execute locale@  THEN ; immediate

\ substitute stuff

Vocabulary macros

L" <>" AConstant delimiters

also macros definitions
: macro: ( addr u -- ) Create here 0 , $! DOES> $@ ;
previous Forth definitions

[defined] execute-parsing [IF]
: replaces ( addr1 len1 addr2 len2 -- )
    2dup & macros search-wordlist IF  nip nip >body $!
    ELSE
	get-current >r & macros set-current
	s" macro:" execute-parsing
	r> set-current
    THEN ;
[THEN]

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