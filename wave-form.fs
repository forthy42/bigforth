\ a waveform widget                                    03sep97py

\ generate pattern with:
\ bigforth genwave.fs -e '$2000 genwave bye' >wave.trc
\ load with "wave wave.trc"

\ module wave-form

vocabulary scan-it
also scan-it definitions
: ?        $F        ;
: ??       $FF       ;
: ???      $FFF      ;
: ????     $FFFF     ;
: ?????    $FFFFF    ;
: ??????   $FFFFFF   ;
: ???????  $FFFFFFF  ;
: ???????? $FFFFFFFF ;

$0000FFFF Constant XXXXXXXXXXXXXXXX

\ standard ulogic vectors

2 Constant Z
3 Constant X
4 Constant L
5 Constant H
6 Constant W
7 Constant U
$D Constant D

previous definitions

-$80000000 Constant min#
 $7FFFFFFF Constant max#

also memory also dos also

canvas class waveform
public:
    cell var addr
    cell var comment
    cell var vmax
    cell var vmin
    cell var vbase
    early add-wave
how:
    : next-by-2 ( n -- n' )  1- $FFF or 1+ ;
    : add-wave ( n -- )
      dup vmax @ max vmax !
      dup vmin @ min vmin !
      addr $@len 2 cells + next-by-2 addr swap SetHandleSize
      addr $@ + !  cell addr @ +! ;
    : step-x  outer self wave-form with step-x endwith ;
    : wave-x  outer self wave-form with wave-x endwith ;
    : wave-y  outer self wave-form with wave-y endwith ;
    : hsteps  addr $@len cell/ 2+ ;
    : >coord  hsteps w @ */f 1- 0max ;
    : info-line ( n -- )  >r
      hsteps h @ steps r@ 1+ h @ home!
      path  0 h @ to stroke
      r@
      0 r> hsteps 2/ > IF  2  ELSE  0  THEN
      2dup 2 xor swap textpos
      2 dpy xrc font@ font comment $@ text
      0 dpy xrc font@ font
      swap textpos
      cells addr $@ cell- rot min + @
      base @ 8 =
      IF    8 umin S" 01ZXLHWU-----D" rot /string 1 min
      ELSE  base @ 2 = IF  0 tuck  ELSE  extend tuck dabs  THEN
          <# bl hold #S rot sign bl hold #>  THEN text ;
    : draw-normal ( path xi addr' addr -- )
      ?DO  0. rot I @ extend rot extend d- dto
          1 0 to
          (poly# @ $400 >= IF  stroke path  THEN  I @
          cell +LOOP ;
    : up-down ( n1 n2 -- n2 )
      tuck extend rot extend d- 0. 2swap dto ;
    : draw-magstep ( path xi addr' addr -- )
      1 1 step-x @ - << { xsteps |
      ?DO min# max#
          I xsteps cells bounds swap I' umin dup >r swap
          ?DO  I @ tuck min >r max r>  cell  +LOOP
          >r up-down r> up-down r> cell- @ up-down >r
          xsteps 0 to
          (poly# @ $400 >= IF  stroke path  THEN
          r>  xsteps cells +LOOP } ;
    : draw-wave ( -- )
      0 drawcolor
      hsteps vmax @ vmin @ - 1 umax steps
      dpy clipx -$10 $40 p+ >coord >r >coord
      dup addr $@ rot cells /string r> cells min
      2dup >r >r  IF  @  ELSE  drop 0  THEN
      >r 1+ 0 vmax @ extend r@ extend d- dhome!  path
      r> r> r> bounds
      step-x @ 0>  IF  draw-normal  ELSE  draw-magstep  THEN
      drop stroke
      ( 1 font ) base @ >r vbase @ base !

      $C0 $00 $00 rgb> drawcolor
      wave-x @ info-line
      $00 $00 $C0 rgb> drawcolor
      wave-y @ info-line

      r> base ! ( 0 font ) 0 drawcolor ;
    : move-bar ( x y n addr -- ) >r
      1 and IF  2drop 0 r> dpy moved!
                DOPRESS  2swap >r drop  THEN
      2dup dpy scroll drop 2* step-x @
      dup 0> IF /f ELSE 2 swap negate << * THEN
      1- 2/ 0max addr $@ nip cell/ min
      r@ @ case? IF  rdrop  EXIT  THEN  r> !
      outer self wave-form with set-dist endwith
      dpy draw ;
    : text! ( addr u -- ) comment $! ;
    : init ( addr u -- )  text! s" " addr $!
      CV[ draw-wave ]CV
      ^ CK[ over 1 =  IF  nip wave-x move-bar  EXIT  THEN
            over 4 =  IF  nip wave-y move-bar  EXIT  THEN
            >released vbase @ $10 =
            IF  &10  ELSE
                vbase @ &10 =
                IF  2  ELSE
                    vbase @ 2 = IF  8  ELSE  $10  THEN  THEN
            THEN  vbase !
            draw ]CK
      hsteps 0 $10 $1 *fil
      super init $10 vbase ! ;
    : dispose  comment HandleOff  addr HandleOff
      super dispose ;
    : hglue@  hsteps step-x @ dup 0>
      IF  *  ELSE  negate 1+ 1 swap << /f THEN 0 ;
    : xinc  0 step-x @ 1 max ;
class;

previous previous previous

Variable old-file
Variable old-path

\ Module;
