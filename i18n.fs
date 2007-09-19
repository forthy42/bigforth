\ Internationalization and localization

\ LSIDs

Variable lsids
0 Value lsid#

: native@ ( lsid -- addr u )  cell+ cell+ @+ swap ;
: id#@ ( lsid -- n )  cell+ @ ;

: search-lsid ( addr u -- lsid )  lsids
    BEGIN  @ dup  WHILE  >r 2dup r@ native@ str= r> swap  UNTIL  THEN
    nip nip ;

: (l")  r> dup cell+ cell+ @+ + aligned >r ;
: (l2)  r> dup cell+ >r @ ; 


: s, ( addr u -- )  dup , here swap dup allot move align ;
: l, ( addr u -- )
    here lsids @ A, lsids ! lsid# dup , 1+ to lsid# s, ;

: LLiteral  2dup search-lsid dup  IF
	nip nip postpone (l2) A,
    ELSE  drop postpone (l") l,  THEN ; immediate

: L" ( "lsid<">" -- lsid ) '" parse
    state @ IF  postpone LLiteral
    ELSE  2dup search-lsid dup  IF
	    nip nip
	ELSE  drop align here >r l, r>  THEN
    THEN  ; immediate

\ deliberately unique string
: LU" ( "lsid<">" -- lsid ) '" parse
    state @ IF  postpone (l") l,
    ELSE  align here >r l, r>
    THEN  ; immediate

: .lsids ( lsids -- ) list> native@ type cr ;

\ locale@ stuff

$3 Constant locale-depth \ lang country variances
Variable locale-stack  locale-depth cells allot

' Variable alias Locale

: >locale ( lsids -- )
    locale-stack @+ swap cells + !  1 locale-stack +!
    locale-stack @ locale-depth u>= abort" locale stack full" ;
: locale-drop ( -- )  -1 locale-stack +!
    locale-stack @ locale-depth u>= abort" locale stack empty" ;
: locale' ( -- addr )  locale-stack @+ swap 1- cells + @ ;

: search-lsid# ( id# lsids -- lsid# )
    BEGIN  @ dup  WHILE  >r dup r@ cell+ @ = r> swap  UNTIL  THEN
    nip ;

: locale@ ( lsid -- addr u )  locale-stack @ IF
	dup >r id#@
	locale-stack @+ swap cells bounds swap cell- DO
	    dup I @ search-lsid# dup IF  nip native@ unloop rdrop EXIT  THEN
	    drop
	-cell +LOOP  drop r>
    THEN  native@ ;

: locale! ( addr u lsid -- )  id#@
    here locale' dup @ A, ! , s, ;

\ Examples

' lsids Alias en

Locale de

de >locale

s" Januar" l" January" locale!
s" Februar" l" February" locale!
s" März" l" March" locale!
l" April" drop
s" Mai" l" May" locale!
s" Juni" l" June" locale!
s" Juli" l" July" locale!
l" August" drop
l" September" drop
s" Oktober" l" October" locale!
l" November" drop
s" Dezember" l" December" locale!

Locale de_AT

de_AT >locale

s" Jänner" l" January" locale!

locale-stack off