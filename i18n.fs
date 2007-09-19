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

: locale! ( addr u lsid -- ) >r
    2dup r@ locale@ str= IF  rdrop 2drop  EXIT  THEN
    r> id#@ here locale' dup @ A, ! , s, ;

\ easy use

: x" state @ IF  postpone l" postpone locale@
    ELSE  ['] l" execute locale@  THEN ;

\ Examples

Create months 12 cells allot
l" January"   months 0 cells + !
l" February"  months 1 cells + !
l" March"     months 2 cells + !
l" April"     months 3 cells + !
l" May"       months 4 cells + !
l" June"      months 5 cells + !
l" July"      months 6 cells + !
l" August"    months 7 cells + !
l" September" months 8 cells + !
l" October"   months 9 cells + !
l" November"  months 10 cells + !
l" December"  months 11 cells + !

: .month ( n -- ) 1- cells months + @ locale@ type ;

' lsids Alias en

Locale de

de >locale

s" Januar" l" January" locale!
s" Februar" l" February" locale!
s" März" l" March" locale!
s" Mai" l" May" locale!
s" Juni" l" June" locale!
s" Juli" l" July" locale!
s" Oktober" l" October" locale!
s" Dezember" l" December" locale!

Locale de_AT

de_AT >locale

s" Jänner" l" January" locale!

locale-stack off

Locale fr
fr >locale

s" Janvier" l" January" locale!
s" Février" l" February" locale!
s" Mars" l" March" locale!
s" Avril" l" April" locale!
s" Mai" l" May" locale!
s" Juin" l" June" locale!
s" Juillet" l" July" locale!
s" Août" l" August" locale!
s" Septembre" l" September" locale!
s" Octobre" l" October" locale!
s" Novembre" l" November" locale!
s" Décembre" l" December" locale!

locale-stack off

Locale zh
zh >locale

s" 一月" l" January" locale!
s" 二月" l" February" locale!
s" 三月" l" March" locale!
s" 四月" l" April" locale!
s" 五月" l" May" locale!
s" 六月" l" June" locale!
s" 七月" l" July" locale!
s" 八月" l" August" locale!
s" 九月" l" September" locale!
s" 十月" l" October" locale!
s" 十一月" l" November" locale!
s" 十二月" l" December" locale!
