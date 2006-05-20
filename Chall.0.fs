#! ./bigforth float.fb
\ Challenge 1                                       17-8-2001jps
float vocabulary chall also chall definitions

32 constant symb-sz     create symb-tb symb-sz 81 * allot
: new-symb symb-sz symb-tb +!  source >in @ tuck - >r +
  symb-tb dup @ + r@ over c! 1+ r> move ;
18 cells constant gv-sz create gv-tb gv-sz allot

: gv: dup constant cell+ ;

gv-tb  gv: notme gv: score  gv:  drop

: myturn false notme ! ;  : theirturn true notme ! ;

\ Challenge 2                                       17-8-2001jps

: mk-mv ( s# ) drop ;
: best.0 ( - s# c" ) symb-tb dup @ 32 / random 1+ 32 * tuck + ;
: @score ( <f> ) bl parse >float f>s score +! ;
: reset 0 symb-tb !  gv-tb gv-sz erase theirturn ;


\ Challenge Final                                   17-8-2001jps
: getstr ( a n - m ) over + >r dup
  begin key dup 10 <> over 13 <> and
  while over c! 1+ dup r@ > until r@ then r> 2drop swap - ;
vocabulary commands vocabulary symbols
: @command commands definitions ; : @input ;
: >ps       symbols definitions ; create my-ibuff 255 allot
: str1 s" marker clean : new clean str1 evaluate reset ; >ps" ;
: prog str1 evaluate reset ." @info name MyPlayer" cr
  begin my-ibuff dup 255 accept dup
  if over c@ 35 <> if evaluate then else 2drop then again ;
also commands definitions
: exit   ." @info exit" cr bye ;
: symbol >ps new-symb create symb-tb @ , does> @ mk-mv ;
: play   >ps myturn best.0 count type cr mk-mv theirturn ;
 prog
