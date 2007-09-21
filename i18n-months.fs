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

\ by hand

Locale de

de

s" Januar" l" January" locale!
s" Februar" l" February" locale!
s" März" l" March" locale!
s" Mai" l" May" locale!
s" Juni" l" June" locale!
s" Juli" l" July" locale!
s" Oktober" l" October" locale!
s" Dezember" l" December" locale!

& de Country de_AT

de_AT

s" Jänner" l" January" locale!

\ by line

: <<months ( -- )
  12 0 DO  refill drop source months I cells + @ locale!  LOOP   refill drop ;

Locale fr
fr

s" «»" delimiters locale!

<<months
janvier
février
mars
avril
mai
juin
juillet
août
septembre
octobre
novembre
décembre

Locale it
it

<<months
gennaio
febbraio
marzo
aprile
maggio
giugno
luglio
agosto
settembre
ottobre
novembre
dicembre

Locale zh
zh

s" “”" delimiters locale!

<<months
一月
二月
三月
四月
五月
六月
七月
八月
九月
十月
十一月
十二月

Locale ru
ru

<<months
Январь
Февраль
Март
Апрель
Май
Июнь
Июль
Август
Сентябрь
Октябрь
Ноябрь
Декабрь

