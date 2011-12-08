\ Enlightenment style                                  07feb98py

\needs get-imdata  include Estyle.fs

\ E icons                                              14feb98py

widget class Eicon-pixmap
public:
    cell var Eimage
    cell var pixmap
    cell var shape
    method draw-at
how:
    : init ( file len dpy -- )  bind dpy super init
        (read-imicon Eimage ! ;
    : draw-at ( x y w h -- 0 0 w h x y w1 w2 )
        over w @ <> over h @ <> or >r h ! w !
        r> IF
            pixmap @ ?dup IF
                dpy xrc imdata @ ImlibFreePixmap drop
            THEN
            h @ w @ Eimage @ dpy xrc imdata @
            ImlibRender drop
            Eimage @ dpy xrc imdata @ ImlibMoveMaskToPixmap shape !
            Eimage @ dpy xrc imdata @ ImlibMoveImageToPixmap pixmap !
	    
	    4 dpy xrc set-function  0 dpy xrc set-color
	    dpy drawable nip shape @ pixmap @ rot
	    0 0 w @ h @ 0 0 1 XCopyPlane
	    3 dpy xrc set-function
        THEN
        >r >r 0 0 w @ h @ r> r>  shape @ pixmap @ ;
    : draw  xywh draw-at dpy mask ;
class;

: Eicon: ( "name" "file"<"> -- )
  Create 0 , ,"  DOES> ( -- icon )
  dup @ dup IF  nip  EXIT  THEN  drop
  dup cell+ count screen self Eicon-pixmap new tuck swap ! ;

\ draw button                                          14feb98py

Eicon: button-fl Estyle/wood/button-fl.png"
Eicon: button-fms Estyle/wood/button-fms.png"
Eicon: button-fr Estyle/wood/button-fr.png"
Eicon: button-dl Estyle/wood/button-dl.png"
Eicon: button-dms Estyle/wood/button-dms.png"
Eicon: button-dr Estyle/wood/button-dr.png"
Eicon: button-pl Estyle/wood/button-pl.png"
Eicon: button-pms Estyle/wood/button-pms.png"
Eicon: button-pr Estyle/wood/button-pr.png"
Eicon: button-sl Estyle/wood/button-sl.png"
Eicon: button-sms Estyle/wood/button-sms.png"
Eicon: button-sr Estyle/wood/button-sr.png"

button with
  F : e-draw ( x y w h icon -- )
        Eicon-pixmap with draw-at endwith dpy mask ;
  F : e-button ( l ms r -- )
      { l ms r |
        x @ y @ h @ 2/ w @ 2/ min h @ l e-draw
        x @ h @ 2/ w @ 2/ min + y @ w @ h @ -2 and w @ -2 and min - h @ ms e-draw
        x @ w @ h @ 2/ w @ 2/ min - + y @ h @ 2/ w @ 2/ min h @ r e-draw } ;
  F : e-choise ( -- l ms r )
        color @ $FF and 1 =
        IF    push?
            IF    button-pl button-pms button-pr
            ELSE  button-fl button-fms button-fr  THEN
        ELSE  push?
            IF    button-sl button-sms button-sr
            ELSE  button-dl button-dms button-dr  THEN
	THEN  ;
endwith

button implements
    : draw ( -- )
      xywh defocuscol @ @ dpy box
      e-choise e-button
      text $@ 0 push? 1 and textcenter ;
    : hglue  textwh @ texth @ + 1+ 1 *fil ;
class;

topindex implements
    : e-draw-half ( x y w h icon -- )
        Eicon-pixmap with 2* draw-at 4 pick 2/ 4 pin endwith
        dpy mask ;
    : e-button-half ( l ms r y h -- )
      { l ms r y h |
        x @ y h 2/ w @ 2/ min h l e-draw-half
        x @ h 2/ w @ 2/ min + y w @ h -2 and w @ -2 and min - h ms e-draw-half
        x @ w @ h 2/ w @ 2/ min - + y h 2/ w @ 2/ min h r e-draw-half } ;
    : draw ( -- )
        callback fetch color @ $18 >> negate
        { state o |
          xywh state IF  xS +  THEN  defocuscol @ @ dpy box
          state 0= IF  xywh rot + swap xS shadow drop dpy box  THEN
          e-choise y @ h @ xS +
          state 0= IF  xS -2 xS * p+  THEN  e-button-half
          text $@ state IF  0 o  ELSE  xS negate o xS + THEN
          textcenter } ;
     : hglue  textwh @ texth @ + 1+ 1 *fil ;
class;

menu-entry implements
    button :: draw
    button :: hglue
    button :: vglue
class;

menu-title implements
    : draw ( -- )
        xywh color @ dpy box
        color 2+ c@ IF  e-choise e-button  THEN
        text $@ 0 0 textcenter ;
class;

boxchar implements
    : draw ( -- )
      xywh defocuscol @ @ dpy box
      e-choise e-button
      color 2+ 1 0 push? 1 and textcenter ;
    button :: hglue
class;

togglechar implements
    boxchar :: draw
    boxchar :: hglue
class;

lbutton implements
    : draw ( -- )
      xywh defocuscol @ @ dpy box
      e-choise e-button
      text $@ h @ 2/ w @ 2/ min push? 1 and textleft ;
    button :: hglue
class;

\ toggle button

Eicon: tbutton-fl+ Estyle/wood/tbutton-fl+.png"
Eicon: tbutton-fl- Estyle/wood/tbutton-fl-.png"
Eicon: tbutton-fms Estyle/wood/tbutton-fms.png"
Eicon: tbutton-fr Estyle/wood/tbutton-fr.png"
Eicon: tbutton-dl+ Estyle/wood/tbutton-dl+.png"
Eicon: tbutton-dl- Estyle/wood/tbutton-dl-.png"
Eicon: tbutton-dms Estyle/wood/tbutton-dms.png"
Eicon: tbutton-dr Estyle/wood/tbutton-dr.png"
Eicon: tbutton-pl+ Estyle/wood/tbutton-pl+.png"
Eicon: tbutton-pl- Estyle/wood/tbutton-pl-.png"
Eicon: tbutton-pms Estyle/wood/tbutton-pms.png"
Eicon: tbutton-pr Estyle/wood/tbutton-pr.png"

tbutton with
  F : e-tchoise ( -- l+ l- ms r )
        color @ $FF and 1 =
        IF    push?
            IF    tbutton-pl+ tbutton-pl- tbutton-pms tbutton-pr
            ELSE  tbutton-fl+ tbutton-fl- tbutton-fms tbutton-fr  THEN
        ELSE  push?
            IF    tbutton-dl+ tbutton-dl- tbutton-dms tbutton-dr
            ELSE  tbutton-dl+ tbutton-dl- tbutton-dms tbutton-dr  THEN
        THEN  ;
  F : e-tbutton ( l+ l- ms r -- )
      { l+ l- ms r |
        x @ y @ h @ w @ 2/ min h @
        callback fetch IF  l+  ELSE  l-  THEN e-draw
        x @ h @ w @ 2/ min + y @ w @ h @ h @ 2/ + w @ h @ 2/ + min - h @ ms e-draw
        x @ w @ h @ 2/ w @ 2/ min - + y @ h @ 2/ w @ 2/ min h @ r e-draw } ;
endwith

tbutton implements
    : draw ( -- )
        xywh defocuscol @ @ dpy box
        e-tchoise e-tbutton
        text $@ h @ w @ 2/ min 0 textleft ;
class;

Eicon: rbutton-fl+ Estyle/wood/rbutton-fl+.png"
Eicon: rbutton-fl- Estyle/wood/rbutton-fl-.png"
Eicon: rbutton-fms Estyle/wood/rbutton-fms.png"
Eicon: rbutton-fr Estyle/wood/rbutton-fr.png"
Eicon: rbutton-dl+ Estyle/wood/rbutton-dl+.png"
Eicon: rbutton-dl- Estyle/wood/rbutton-dl-.png"
Eicon: rbutton-dms Estyle/wood/rbutton-dms.png"
Eicon: rbutton-dr Estyle/wood/rbutton-dr.png"
Eicon: rbutton-pl+ Estyle/wood/rbutton-pl+.png"
Eicon: rbutton-pl- Estyle/wood/rbutton-pl-.png"
Eicon: rbutton-pms Estyle/wood/rbutton-pms.png"
Eicon: rbutton-pr Estyle/wood/rbutton-pr.png"

rbutton implements
    : e-rchoise ( -- l+ l- ms r )
        color @ $FF and 1 =
        IF    push?
            IF    rbutton-pl+ rbutton-pl- rbutton-pms rbutton-pr
            ELSE  rbutton-fl+ rbutton-fl- rbutton-fms rbutton-fr  THEN
        ELSE  push?
            IF    rbutton-dl+ rbutton-dl- rbutton-dms rbutton-dr
            ELSE  rbutton-dl+ rbutton-dl- rbutton-dms rbutton-dr  THEN
        THEN  ;
    : e-rbutton ( l+ l- ms r -- )
      { l+ l- ms r |
        x @ y @ h @ w @ 2/ min h @
        callback fetch IF  l+  ELSE  l-  THEN e-draw
        x @ h @ w @ 2/ min + y @ w @ h @ h @ 2/ + w @ h @ 2/ + min - h @ ms e-draw
        x @ w @ h @ 2/ w @ 2/ min - + y @ h @ 2/ w @ 2/ min h @ r e-draw } ;
    : draw ( -- )
        xywh defocuscol @ @ dpy box
        e-rchoise e-rbutton
        text $@ h @ w @ 2/ min 0 textleft ;
class;

\ text label

text-label implements
    : draw ( -- )
      xywh defocuscol @ @ dpy box
      button-dl button-dms button-dr e-button
      text $@ h @ 2/ w @ textwh @ - 2/ min 0 textleft ;
    button :: hglue
class;

icon-button implements
    : draw  ( -- )
        xywh defocuscol @ @ dpy box
        e-choise e-button
        x @ xS + y @ h @ icon h @ - 2/ + push? dup p-
        icon draw-at dpy mask
        text $@
        xS 1+ icon w @ + xN + push? 1 and textleft ;
    : hglue   super hglue  swap icon w @ + xN + swap ;
    : vglue   super vglue  swap icon h @ xS 2* + 1+ max swap ;
class;

icon-but implements
    : draw ( -- ) push? 1 and >r
        xywh defocuscol @ @ dpy box
        e-choise e-button
        x @ w @ icon w @ - 2/ + r@ +
        y @ h @ icon h @ - 2/ + r> +
        icon draw-at dpy mask ;
    : hglue   icon w @ xS 2* + 1 *fil ;
    : vglue   icon h @ xS 2* + 1+ 1 *fil ;
class;

toggleicon implements
    : draw  ( -- )
        callback fetch  push?  { s of |
            xywh defocuscol @ @ dpy box
            e-choise e-button
            s IF    icon+ w @ icon+ h @
            ELSE  icon- w @ icon- h @  THEN  >r >r
            x @ w @ r> - 2/ +  y @ h @ r> - 2/ +  of dup p+
            s IF icon+ draw-at ELSE icon- draw-at THEN dpy mask } ;
    : hglue  icon+ w @ icon- w @ max  xS 2* + 1+ 1 *fil ;
    : vglue  icon+ h @ icon- h @ max  xS 2* + 1+ 1 *fil ;
class;

flipicon implements
    : draw  ( -- )  color push  color @ $FFFFFF and
        callback fetch  IF -3 ELSE 2 THEN  $18 << or color !
        xywh defocuscol @ @ dpy box
        e-choise e-button
        callback fetch 1 and >r
        x @ w @ icon w @ - 2/ + r@ +
        y @ h @ icon h @ - 2/ + r> +
        icon draw-at dpy mask ;
    : hglue  icon w @ xS 2* + 1+ 1 *fil ;
    : vglue  icon h @ xS 2* + 1+ 1 *fil ;
class;

Eicon: arrow-pt Estyle/wood/arrow-pt.png"
Eicon: arrow-ft Estyle/wood/arrow-ft.png"
Eicon: arrow-dt Estyle/wood/arrow-dt.png"
Eicon: arrow-pr Estyle/wood/arrow-pr.png"
Eicon: arrow-fr Estyle/wood/arrow-fr.png"
Eicon: arrow-dr Estyle/wood/arrow-dr.png"
Eicon: arrow-pb Estyle/wood/arrow-pb.png"
Eicon: arrow-fb Estyle/wood/arrow-fb.png"
Eicon: arrow-db Estyle/wood/arrow-db.png"
Eicon: arrow-pl Estyle/wood/arrow-pl.png"
Eicon: arrow-fl Estyle/wood/arrow-fl.png"
Eicon: arrow-dl Estyle/wood/arrow-dl.png"

tributton with
    | Create p-table  T] arrow-pl arrow-pt arrow-pr arrow-pb [
    | Create f-table  T] arrow-fl arrow-ft arrow-fr arrow-fb [
    | Create d-table  T] arrow-dl arrow-dt arrow-dr arrow-db [
endwith

tributton implements
    : draw ( -- )  xywh defocuscol @ @ dpy box
        xywh 1 1 p- -1 xywh-
        color @ $FF and 1 =
        IF  push?  IF  p-table  ELSE  f-table  THEN
        ELSE  d-table  THEN
        color @ $E >> $C and + perform e-draw ;
    : hglue  xM xS + 1 *fil ;
    : vglue  xM xS + 1 *fil ;
class;

slidetri implements
    | Create o-table  3 0 , ,  0 3 , ,  -3 0 , ,  0 -3 , ,
    : draw ( -- )  xywh defocuscol @ @ dpy box
        xywh 1 1 p- -1 xywh-
        2swap  color @ $D >> $18 and o-table + 2@
        xS * 2/ swap xS * 2/ swap p+ 2swap
        color @ $FF and 1 =
        IF  push?  IF  p-table  ELSE  f-table  THEN
        ELSE  d-table  THEN
        color @ $E >> $C and + perform e-draw ;
    : hglue tributton :: hglue drop 0 ;
    : vglue tributton :: vglue drop 0 ;
class;

\ slider

Eicon: hslider-dl Estyle/wood/hslider-dl.png"
Eicon: hslider-dls Estyle/wood/hslider-dls.png"
Eicon: hslider-dm Estyle/wood/hslider-dm.png"
Eicon: hslider-dr Estyle/wood/hslider-dr.png"
Eicon: hslider-drs Estyle/wood/hslider-drs.png"
Eicon: hslider-fl Estyle/wood/hslider-fl.png"
Eicon: hslider-fls Estyle/wood/hslider-fls.png"
Eicon: hslider-fm Estyle/wood/hslider-fm.png"
Eicon: hslider-fr Estyle/wood/hslider-fr.png"
Eicon: hslider-frs Estyle/wood/hslider-frs.png"
Eicon: hslider-pl Estyle/wood/hslider-pl.png"
Eicon: hslider-pms Estyle/wood/hslider-pms.png"
Eicon: hslider-pr Estyle/wood/hslider-pr.png"

Eicon: vslider-dt Estyle/wood/vslider-dt.png"
Eicon: vslider-dts Estyle/wood/vslider-dts.png"
Eicon: vslider-dm Estyle/wood/vslider-dm.png"
Eicon: vslider-db Estyle/wood/vslider-db.png"
Eicon: vslider-dbs Estyle/wood/vslider-dbs.png"
Eicon: vslider-ft Estyle/wood/vslider-ft.png"
Eicon: vslider-fts Estyle/wood/vslider-fts.png"
Eicon: vslider-fm Estyle/wood/vslider-fm.png"
Eicon: vslider-fb Estyle/wood/vslider-fb.png"
Eicon: vslider-fbs Estyle/wood/vslider-fbs.png"
Eicon: vslider-pt Estyle/wood/vslider-pt.png"
Eicon: vslider-pms Estyle/wood/vslider-pms.png"
Eicon: vslider-pb Estyle/wood/vslider-pb.png"

arule class Erule
public: 2 cells var Estretch
how:
    : init  ( actor hxt vxt iconf icond -- )
        Estretch 2!  super init ;
    : draw  ( -- )
        xywh Estretch 2@
        color @ $FF and 1 <>  IF  swap  THEN  drop
        Eicon-pixmap with draw-at endwith dpy mask ;
class;

arule class Eside
public: 4 cells var Eicons
how:
    : focus    1 color c! draw ;
    : defocus  0 color c! draw ;
    : init ( actor hxt vxt iconl icons iconrf iconrd -- )
        swap 2swap swap Eicons !+ !+ !+ ! super init ;
    : e-draw ( x y w h icon -- )
        Eicon-pixmap with draw-at endwith dpy mask ;
    : ep-draw ( x y w h icon -- ) >r parent xywh
        r> Eicon-pixmap with draw-at endwith
        >r >r 2drop 2drop 2drop { x y w h |
        x y parent xywh 2drop p- w h x y } r> r>
        dpy mask ;
class;

Eside class Eleft
how:
    : draw ( -- )
        xywh 2dup 2/ min { x y w h d | x d + y w d - h }
        Eicons cell+ @ ep-draw
        xywh tuck 2/ min swap
        Eicons @ e-draw
        xywh 2dup 2/ min { x y w h d | x w + d - y d h }
        Eicons 3 color @ $FF and 1 = + cells + @ e-draw ;
class;

Eside class Eright
how:
    : draw ( -- )
        xywh 2dup 2/ min rot swap - swap
        Eicons cell+ @ ep-draw
        xywh 2dup 2/ min { x y w h d | x w + d - y d h }
        Eicons @ e-draw
        xywh tuck 2/ min swap
        Eicons 3 color @ $FF and 1 = + cells + @ e-draw
    ;
class;

Eside class Etop
how:
    : draw ( -- )
        xywh over 2/ over min { x y w h d | x y d + w h d - }
        Eicons cell+ @ ep-draw
        xywh swap tuck 2/ min
        Eicons @ e-draw
        xywh over 2/ over min { x y w h d | x y h + d - w d }
        Eicons 3 color @ $FF and 1 = + cells + @ e-draw ;
class;

Eside class Ebot
how:
    : draw ( -- )
        xywh over 2/ over min -
        Eicons cell+ @ ep-draw
        xywh over 2/ over min { x y w h d | x y h + d - w d }
        Eicons @ e-draw
        xywh swap tuck 2/ min
        Eicons 3 color @ $FF and 1 = + cells + @ e-draw
    ;
class;

hslider implements
    : draw  xywh defocuscol @ @ dpy box  :: draw ;
    : part2'  part2 swap xM - swap 2/ ;
    : part0b  xN 0 ;
    : init ( callback -- )  >callback
        ^ R[ lstep ]R 0 slidetri new \ 1 ^ habox new fixbox
        ^ R[ lpage ]R [: part1 part0 drop 2/ 0 p+ ;] ['] part0
          hslider-pl hslider-pms hslider-fl hslider-dl Eleft new
        ^ M[ slide ]M ['] part2' ['] part0 hslider-fls hslider-dls Erule new
          arule with $02000003 assign ^ endwith
        ^ M[ slide ]M ['] part0b ['] part0 hslider-fm hslider-dm Erule new
          arule with $01000003 assign ^ endwith
        ^ M[ slide ]M ['] part2' ['] part0 hslider-frs hslider-dls Erule new
          arule with $02000003 assign ^ endwith
        ^ R[ rpage ]R [: part3 part0 drop 2/ 0 p+ ;] ['] part0
          hslider-pr hslider-pms hslider-fr hslider-dr Eright new
        ^ R[ rstep ]R 2 slidetri new \ 1 ^ habox new fixbox
        7 super init ;
class;

hslider0 implements
    : draw  xywh defocuscol @ @ dpy box  :: draw ;
    hslider :: init
class;

hscaler implements
    : draw  xywh defocuscol @ @ dpy box  :: draw ;
    : part0c ( -- glue )  xM xS + 0 ;
    : part4' ( -- glue )  part4 swap xM xS 2* - 2/ - 1 max swap ;
    : init ( callback -- )  >callback
        ^ M[ slide ]M ['] part0a ['] part5 arule new
        ^ R[ lpage ]R [: part1 part0c drop 2/ 0 p+ ;] ['] part0c
        hslider-pl hslider-pms hslider-fl hslider-dl Eleft new
        arule with $01000003 assign ^ endwith
          ^ M[ slide ]M ['] part4' ['] part0c hslider-fls hslider-dls Erule new
          arule with $01000003 assign ^ endwith
          ^ M[ slide ]M ['] part0b ['] part0c hslider-fm hslider-dm Erule new
          arule with $01000003 assign ^ endwith
          ^ M[ slide ]M ['] part4' ['] part0c hslider-frs hslider-drs Erule new
          arule with $01000003 assign ^ endwith
        3 hbox new
        ^ R[ rpage ]R [: part3 part0c drop 2/ 0 p+ ;] ['] part0c
        hslider-pr hslider-pms hslider-fr hslider-dr Eright new
        arule with $01000003 assign ^ endwith
        3 hbox new
        2 vbox new 1 super super init ;
class;

vscaler implements
    : draw  xywh defocuscol @ @ dpy box  :: draw ;
    : part0cv ( -- glue )  xM xS + 0 ;
    : part4'v ( -- glue )  part4 swap xM xS 2* - 2/ - 1 max swap ;
    : init ( callback -- )  >callback
        ^ M[ slide ]M ['] part5 ['] part0a arule new
        ^ R[ rpage ]R ['] part0cv [: part3 part0cv drop 2/ 0 p+ ;]
        vslider-pt vslider-pms vslider-ft vslider-dt Etop new
        arule with $01000003 assign ^ endwith
          ^ M[ slide ]M ['] part0cv ['] part4'v vslider-fts vslider-dts Erule new
          arule with $01000003 assign ^ endwith
          ^ M[ slide ]M ['] part0cv ['] part0b vslider-fm vslider-dm Erule new
          arule with $01000003 assign ^ endwith
          ^ M[ slide ]M ['] part0cv ['] part4'v vslider-fbs vslider-dbs Erule new
          arule with $01000003 assign ^ endwith
        3 vbox new
        ^ R[ lpage ]R ['] part0cv [: part1 part0cv drop 2/ 0 p+ ;]
        vslider-pb vslider-pms vslider-fb vslider-db Ebot new
        arule with $01000003 assign ^ endwith
        3 vbox new
        2 hbox new 1 super super init ;
class;

vslider implements
    : draw  xywh defocuscol @ @ dpy box  :: draw ;
    : part2'v  part2 swap xM - swap 2/ ;
    : part0bv  xN 0 ;
    : init ( callback -- )  >callback
        ^ R[ lstep ]R 1 slidetri new \ 1 ^ habox new fixbox
        ^ R[ lpage ]R ['] part0 [: part1 part0 drop 2/ 0 p+ ;]
          vslider-pt vslider-pms vslider-ft vslider-dt Etop new
        ^ M[ slide ]M ['] part0 ['] part2'v vslider-fts vslider-dts Erule new
          arule with $02000003 assign ^ endwith
        ^ M[ slide ]M ['] part0 ['] part0bv vslider-fm vslider-dm Erule new
          arule with $02000003 assign ^ endwith
        ^ M[ slide ]M ['] part0 ['] part2'v vslider-fbs vslider-dbs Erule new
          arule with $02000003 assign ^ endwith
        ^ R[ rpage ]R ['] part0 [: part3 part0 drop 2/ 0 p+ ;]
          vslider-pb vslider-pms vslider-fb vslider-db Ebot new
        ^ R[ rstep ]R 3 slidetri new \ 1 ^ habox new fixbox
        7 super init ;
class;

vslider0 implements
    : draw  xywh defocuscol @ @ dpy box  :: draw ;
    vslider :: init
class;

0 [IF]
hrtsizer implements
    : draw ( -- )  xywh defocuscol @ @ dpy box
      e-choise e-button ; 
class;

hsizer implements
    hrtsizer :: draw
class;

hxrtsizer implements
    hrtsizer :: draw
class;
[THEN]

vrtsizer implements
    : draw ( -- )  xywh defocuscol @ @ dpy box
      e-choise e-button ; 
class;

vsizer implements
    vrtsizer :: draw
class;

vxrtsizer implements
    vrtsizer :: draw
class;

previous previous previous previous previous previous Forth
