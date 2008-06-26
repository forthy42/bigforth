\ widget                                               13may99py

gadget class widget             early >callback
public: displays ptr dpy        actor ptr callback
        early dopress           early whilepress
        early shadow            early xS
        early drawshadow        early textsize
        early xN                early xM
        early hM
        method +push            method -push

\ widget                                               27jun02py

how:    : >callback ( cb -- )
          callback self IF  callback dispose  THEN
          bind callback  self callback bind caller
          callback called self ?EXIT
          self callback set-called ;
        : dispose callback self  IF  callback dispose  THEN
          super dispose ;
        : dpy!  bind dpy !resized ;
        : textsize ( addr u n -- w h )
          dpy xrc font@ font with  size  endwith ;
        : close  dpy close ;

\ widget                                               28aug99py

        : DOPRESS  ( dx dy -- dx dy x y )
          BEGIN  BEGIN  dpy click? 0=  WHILE  dpy moved?
                        IF    2dup dpy mouse drop r@ execute
                              dpy sync
                        THEN  dpy invoke dpy do-idle
                 REPEAT dpy click nip 1 and  WHILE
          2drop  REPEAT  dpy moved! ;

\ widget                                               01mar98py

        : >timeout ( time -- )  dpy sync
          BEGIN  timeout? 0=  WHILE  dpy invoke 0=  UNTIL  THEN
          till ;
        : WHILEPRESS ( x y b n -- ) \ 2over moved
          nip nip nip 1 and
          IF    BEGIN 0 after
                    BEGIN  r@ swap >r execute
                           r> /step @ ms>time +
                           dup >timeout  dpy click?  UNTIL  drop
                    dpy click nip nip nip 1 and 0=  UNTIL
                rdrop dpy moved!
          ELSE  &50 after >timeout  THEN ;

\ draw shadow                                          25mar99py

        : draw-edge ( x y n col -- )  >r 1-
          dup 1 <  IF  2drop drop rdrop  EXIT  THEN
          dup 1 =  IF  dup r> dpy box    EXIT  THEN  >r
          <poly r@ 0 poly, 0 r@ - r> poly, poly> r> dpy fill ;
        : (drawshadow ( lightcol shadowcol n x y w h -- )
          { n x y w h | n 0< IF  swap  THEN  n abs  { lc sc n |
             x         y         w n - n      lc dpy box
             x         y n +     n     h n -  lc dpy box
             x         y h + n - w     n      sc dpy box
             x w + n - y         n     h n -  sc dpy box
             x w + n - y         n            lc draw-edge
             x         y h + n - n            lc draw-edge } } ;

\ draw shadow                                          27jan07py
         : 2+?  dup $10 < IF  2+  THEN ;
         : drawshadow ( lc sc n x y w h -- )
           4 pick abs twoborders u<= IF  (drawshadow
           ELSE { lc sc n x y w h |
                n IF  lc    sc n         x y w h (drawshadow
                      lc 2+? sc 2+? n 0< 2* 1+
                                         x y w h (drawshadow
                THEN }
           THEN ;
[IFDEF] x11 : focus  dpy self 0= ?EXIT
          dpy xrc self 0= ?EXIT dpy xrc ic @ 0= ?EXIT
          xywh nip + dpy trans' swap  spot w!+ w!
          0 XNSpotLocation spot 0 XVaCreateNestedList_1 >r
          dpy xrc ic @ XNPreeditAttributes r@ 0
          XSetICValues_1 drop r> XFree drop ;  [THEN]

\ widget                                               19oct99py
        : shadow ( -- lc sc )
          shadowcol @ @ dup 8 >> swap $FF and ;
        : show-you ( -- ) xywh p2/ p+ dpy show-me ;
        : >released ( click -- )  WHILEPRESS ;
        : clicked  >released ;
        : +push ;
        : -push ;
        : xN ( -- n )  dpy xrc xN @ ;
        : xM ( -- n )  dpy xrc xM @ ;
        : hM ( -- n )  dpy xrc hM @ ;
        : xS ( -- n )  dpy xS ;
        : moved ( x y -- )  2drop
          mouse_cursor dpy set-cursor ^ dpy set-rect ;
        : leave ( -- ) ;
class;

\ repeated press and moved press                       23aug97py

simple class rep
how:    : >press  widget WHILEPRESS  widget callback toggle ;
        : click  caller with widget +push >press
                             widget -push endwith ;
class;

simple class drag
how:    : click  toggle ;
class;

' :[ alias R[  immediate restrict
' :[ alias M[  immediate restrict
: ]R  postpone ]: rep    postpone new ;      immediate restrict
: ]M  postpone ]: drag   postpone new ;      immediate restrict

\ Icon                                                 21mar04py
include pixmap.fs
widget class icon-pixmap
public: cell var shape          cell var image
        method draw-at
how:    : init ( file len -- ) super init  assign ;
        : dispose-image ( -- )
[IFDEF] x11
          shape @ ?dup IF screen xrc dpy @ swap XFreePixmap  THEN
          image @ ?dup IF screen xrc dpy @ swap XFreePixmap  THEN
 [THEN]   shape off image off ;
        : assign ( file len -- )  dispose-image
          read-icon  h ! w !
[IFDEF] win32  2 0 DO  swap  screen xrc dc @
          CreateCompatibleDC tuck SelectObject drop  LOOP
[THEN]    shape ! image ! ;

\ Icon                                                 27jun02py
        : dispose ( -- )  dispose-image super dispose ;
        : draw-at ( x y -- x y w h x y w1 w2 ) >r >r
          0 0 xywh 2swap 2drop r> r> shape @ image @ ;
        : draw  xywh 2drop draw-at dpy mask ;
class;
[IFDEF] x11
xresource implements
        : set-tile ( x y pixmap -- )  \ -1 cur-color !
          icon-pixmap with image @ endwith
                    [ xgc XGCValues tile         ] ALiteral !
          swap      [ xgc XGCValues ts_x_origin  ] ALiteral 2!
          FillTiled [ xgc XGCValues fill_style   ] ALiteral !
	  dpy @ gc @
          [ GCTile GCTileStipXOrigin or
            GCTileStipYOrigin or GCFillStyle or ] Literal xgc
	  XChangeGC drop ;   class;  [THEN]

\ Icon                                                 26oct07py
widget class icon
public: icon-pixmap ptr picture
how:    : init ( ficon -- )  super init assign ;
        : !resized  picture xywh  h ! w ! 2drop ;
        : assign ( ficon -- )  bind picture !resized ;
        : draw   xywh defocuscol @ @ dpy box
          xywh 2drop picture draw-at dpy mask ;
class;
[IFDEF] x11
icon-pixmap class memory-pixmap
how:    : assign ( data w h -- )  dispose-image
          2dup * pixels -rot
          create-pixmap h ! w ! image ! ;
class;
[THEN]

\ glue                                                 10aug05py

widget class glue
public: cell var w+             cell var h+
        cell var wmin           cell var hmin
how:    : init ( w w+ h h+ -- )  super init
          h+ ! dup h ! hmin ! w+ ! dup w ! wmin ! ;
        : hglue  wmin @ w+ @ ;
        : vglue  hmin @ h+ @ ;
        : draw ( -- )  xywh defocuscol @ @ dpy box ;
        widget :: handle-key? ( -- flag )
class;

\ nil                                                  27jun02py

widget class (nil
how:    : delete ( addr addr' -- )  2drop ;
        : hglue  0 0 ;
        : vglue  0 0 ;
        : init ;
        : dispose ;
        : show-you ( -- ) ;
class;

(nil : nil
nil self nil bind widgets
nil self to 'nil

\ (nilscreen                                           26jul98py

displays class (nilscreen
how:    (nil :: delete
        (nil :: hglue
        (nil :: vglue
        (nil :: init
        (nil :: dispose
        : size-event ( -- ) ;
        : get-event ( mask -- ) drop ;
        : schedule ( xt o time -- )  dpy schedule ;
        : invoke ( -- flag )  dpy invoke ;
        : cleanup ( o -- )  dpy cleanup ;

\ (nilscreen                                           03aug98py

        : schedule-event ( -- ) ;
        : handle-event ( -- )
\          base push hex cr ." Event "
\          event XAnyEvent type ?
\          event XAnyEvent window ?
\          event sizeof XEvent dump
        ;
class;
(nilscreen : nilscreen
nilscreen self nilscreen bind nextwin
nilscreen self to 'nilscreen

\ fills                                                28mar99py

: *fil    ( n -- glue )   $C << ;
: *fill   ( n -- glue )  $14 << ;
: *filll  ( n -- glue )  $1C << ;
: *hpix   ( n -- n )            ; immediate
: *hfil   ( n -- glue )   $C << ;
: *hfill  ( n -- glue )  $14 << ;
: *hfilll ( n -- glue )  $1C << ;
: *vpix   ( n -- n )            ; immediate
: *vfil   ( n -- glue )   $C << ;
: *vfill  ( n -- glue )  $14 << ;
: *vfilll ( n -- glue )  $1C << ;
: +fil   ( -- ) 0 1 *fil   2dup glue new ;
: +fill  ( -- ) 0 1 *fill  2dup glue new ;
: +filll ( -- ) 0 1 *filll 2dup glue new ;

\ hglue vglue                                          10aug05py
: chcol ( addr col-addr -- )
  @ @ over @ $FFFF0000 and or swap ! ;
: tocol ( addr col-addr -- )
  @ @ over @ $FF000000 and or swap ! ;

glue class *hglue
how:    : init ( w w+ -- )  1 1 *hfil       super init ;
class;

glue class *vglue
how:    : init ( h h+ -- )  1 1 *hfil 2swap super init ;
class;

\ hrule vrule                                          05jan07py
glue class rule                 cell var color
how:    : init ( w w+ h h+ -- ) \ dup dup drop drop
          super init defocuscol @ @ assign ;
        : assign ( color -- )  color ! ;
        : draw ( -- )  xywh color @ dpy box
          shadow swap color @ $18 >> xywh drawshadow  ;
        : defocus  color defocuscol tocol draw ;
        : focus    color focuscol   tocol draw ;
class;
rule class hrule
how:    : init ( w w+ -- )  2 0 ( rot) super init ;
class;
rule class vrule
how: : init ( h h+ -- ) ( >r) 2 0 2swap ( r>)super init ;
class;

\ lines                                                28mar99py
: linepar  $1000000 0 colors @ or 0 1 *fill ;
: vline linepar vrule new widget with  assign ^  endwith ;
: hline linepar hrule new widget with  assign ^  endwith ;
rule class Mskip
how:    : init  ( -- )
          0 0 0 1 *fill super init ;
        : hglue ( -- min glue )  xM w+ @ ;
class;
rule class Mfill
how:    : init ( -- )
          0 1 *fill 0 1 *fill super init ;
        : hglue ( -- min glue )  xM w+ @ ;
class;
: 2skip   Mskip new ;
: 2fill   Mfill new ;

\ canvas                                               16jun02py

rule class canvas
public: defer drawer            defer pixel
        cell var angle          cell var coord
        cell var color
        cell var dx             cell var dy
        2 cells var sw          2 cells var sh
        cell var xp             cell var yp
        1 var textpx            1 var textpy
        font ptr fnt            widget ptr outer
        2 cells var startp

\ canvas                                               08aug99py

        method fd               method rt
        early bk                early lt
        method path             method to
        method stroke           method fill
        method clear            method steps
        method up               method down
        method home!            method linewidth
        method drawcolor        method fillcolor
        method backcolor
        method text             method font
        method textpos          method icon
        method dto              method dhome!

\ canvas                                               22jun02py
how:    : init ( xt ac w w+ h h+ -- )  super init ^^ bind outer
         >callback IS drawer down &360 coord ! $0D030C color ! ;
       : pixel, xp 2@ p+ 2dup xp 2! wextend swap wextend pixel ;
        : dx+ ( d -- n )  dx @ extend d+ swap dup dx ! 0< - ;
        : dy+ ( d -- n )  dy @ extend d+ swap dup dy ! 0< - ;
        : draw  flags #shown bit@ 0= ?EXIT  clear  ^ drawer ;
        : fd ( n -- ) >r angle @ sincos
          r@ negate m* sh 2@ d* $10 d>> dy+
          swap   r> m* sw 2@ d* $10 d>> dx+ pixel, ;
        : dto ( x y -- )  dnegate sh 2@ d* dy+
                             -rot sw 2@ d* dx+ pixel, ;
        : to ( x y -- )  swap extend rot extend dto ;
        : rt ( n -- ) 2pi 2* coord @ */ 1+ 2/ angle +! ;
        : bk ( n -- ) negate fd ;
        : lt ( n -- ) negate rt ;

\ canvas                                               25apr07py
        : clear ( -- )  xywh color 2+ c@ dpy box ;
        : stroke ( -- ) startp 2@ poly> color c@ dpy stroke ;
        : fill ( -- ) startp 2@ poly> color 1+ c@ dpy fill ;
        : path ( -- )
          xp 2@ wextend swap wextend <poly startp 2! ;
        : dhome! ( dx dy -- ) dx off  dy off  angle off
          sh 2@ d* dy+ -rot sw 2@ d* dx+ y @ x @ p+ xp 2! ;
        : home! ( p -- )  0 tuck dhome! ;
        : steps ( w h -- )
          $00000000 h @ 1- rot ud/mod sh 2! drop
          $00000000 w @ 1- rot ud/mod sw 2! drop ;
        : up    ['] 2drop IS pixel ;
        : down  ['] poly# IS pixel ;

\ canvas                                               11nov06py
        : linewidth ( n -- ) dpy set-linewidth ;
        : drawcolor ( n -- ) color c! ;
        : fillcolor ( n -- ) color 1+ c! ;
        : backcolor ( n -- ) color 2+ c! ;
        : >textxy ( w h -- x y )  textpy c@ * 2/ negate
          swap textpx c@ * 2/ negate  xp 2@ p+ swap ;
        : font ( fnt -- )  bind fnt ;
        : dpy! ( dpy -- )  super dpy!
          fnt self 0= IF  0 dpy xrc font@ font  THEN ;

\ canvas                                               21mar04py
        : icon ( icon-pixmap -- )  >r
          r@ icon-pixmap with xywh endwith 2swap 2drop >textxy
          r> icon-pixmap with draw-at endwith dpy mask ;
        : .text ( addr u x y c -- )  >r 2swap r>
          fnt select  fnt self fnt ' display dpy drawer ;
        : text ( addr u -- )
          2dup fnt size >textxy color c@ .text ;
        : textpos ( x y -- )  textpy c! textpx c! ;
        : clicked ( x y b n -- ) callback click ;
        : keyed ( key sh -- )  callback key ;
        : moved ( x y -- )  super moved  callback enter ;
        : leave ( -- )  callback leave ;
        : >released  WHILEPRESS ;
class;

\ Canvas helper words                                  04jun08py

[IFDEF] x11
    : col' ( -- addr )  screen xrc colarray @ @ ;
    | Variable rgb'
    
    : c+mask ( color mask -- rgb ) >r
        dup 8 << or dup $10 << or r@ um* swap 0< - r> and ;
    : rgb# ( r g b -- rgb )
	over2 over2 over2 8 << or 8 << or rgb' !
        blue## c+mask >r green## c+mask >r red## c+mask r> r> or or ;
    : color! ( rgb n -- )  >r
	rgb' @  Colortable r@ cells + !  col' r@ cells + ! r> ;
[THEN]
[IFDEF] win32
    : brushs'  screen xrc colarray @ @ ;
    : pens'    screen xrc penarray @ @ ;
    : rgbs'    screen xrc rgbarray @ @ ;

    : ?del ( addr -- addr )
	dup @ ?dup IF  DeleteObject drop  THEN ;
    
    : rgb# ( r g b -- rgb )  8 << or 8 << or ;
    : color! ( rgb n -- n ) >r Colortable r@ cells + !
        rgbs'   r@ cells +      r@ 1 bounds get-rgbs
        pens'   r@ cells + ?del r@ 1 bounds get-pens
        brushs' r@ cells + ?del r@ 1 bounds get-brushs r> ;
[THEN]
: rgb>pen ( r g b -- penc )
    rgb# $FF color! ;
: rgb>fill ( r g b -- fillc )
    rgb# $FE color! ;
: rgb>back ( r g b -- backc )
    rgb# $FD color! ;
