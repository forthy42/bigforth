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
        (read-imicon Eimage !
        3 3 3 3 Eimage @ Image border !+ !+ !+ !
        $100 $100 $200 sp@ Eimage @ dpy xrc imdata @
        ImlibSetImageModifier 2drop 2drop ;
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

Eicon: button-f Estyle/ShinyMetal/bar_horizontal_2.png"
Eicon: button-d Estyle/ShinyMetal/bar_horizontal_1.png"
Eicon: button-p Estyle/ShinyMetal/bar_horizontal_3.png"

button with
  F : e-choise ( -- m )
        color @ $FF and 1 =
        IF    push?
            IF    button-p
            ELSE  button-f  THEN
        ELSE  push?
            IF    button-p
            ELSE  button-d  THEN
        THEN  ;
  F : e-draw ( x y w h icon -- )
        Eicon-pixmap with draw-at endwith dpy mask ;
  F : e-button ( m -- ) >r xywh r> e-draw ;
endwith

button implements
    : draw ( -- )
      xywh defocuscol @ @ dpy box
      e-choise e-button
      text $@ 0 push? 1 and textcenter ;
class;

togglebutton implements
    : draw ( -- )
      xywh defocuscol @ @ dpy box
      e-choise e-button
      callback fetch
      0= IF    text $@ 0 push? 1 and textcenter
      ELSE  text1 $@  2dup 0 textsize >r >r
          xywh r> r> p- p2/ p+
          push? dup p- color @ 8 >> dpy text THEN ;
class;

flipbutton implements
     : draw ( -- )      xywh defocuscol @ @ dpy box
	 callback fetch push? or  color @ $FF and 1 =
	 IF    IF  button-p  ELSE  button-f  THEN
	 ELSE  IF  button-p  ELSE  button-d  THEN  THEN
	 e-button
	 text $@ 0 push? 1 and textcenter ;
class;

topindex implements
    : e-draw-half ( x y w h icon -- )
        Eicon-pixmap with 2* draw-at 4 pick 2/ 4 pin endwith
        dpy mask ;
    : e-button-half ( m y h -- )
      { m y h |
        x @ y w @ h m e-draw-half } ;
    : draw ( -- )
        callback fetch color @ $18 >> negate
        { state o |
          xywh state IF  xS +  THEN  defocuscol @ @ dpy box
          state 0= IF  xywh rot + swap xS shadow drop dpy box  THEN
          e-choise y @ h @ xS +
          state 0= IF  xS -2 xS * p+  THEN  e-button-half
          text $@ state IF  0 o  ELSE  xS negate o xS + THEN
          textcenter } ;
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
class;

\ toggle button

Eicon: tbutton-fl+ Estyle/ShinyMetal/button_kill_2.png"
Eicon: tbutton-fl- Estyle/ShinyMetal/button_off_2.png"
Eicon: tbutton-dl+ Estyle/ShinyMetal/button_kill_1.png"
Eicon: tbutton-dl- Estyle/ShinyMetal/button_off_1.png"
Eicon: tbutton-pl+ Estyle/ShinyMetal/button_kill_3.png"
Eicon: tbutton-pl- Estyle/ShinyMetal/button_off_3.png"

tbutton with
  F : e-tchoise ( -- l+ l- ms r )
        color @ $FF and 1 =
        IF    push?
            IF    tbutton-pl+ tbutton-pl- button-p
            ELSE  tbutton-fl+ tbutton-fl- button-f  THEN
        ELSE  push?
            IF    tbutton-dl+ tbutton-dl- button-d
            ELSE  tbutton-dl+ tbutton-dl- button-d  THEN
        THEN  ;
  F : e-tbutton ( l+ l- ms -- )
      { l+ l- ms |
        xywh ms e-draw
        x @ y @ h @ h @ xS xywh-
        callback fetch IF  l+  ELSE  l-  THEN e-draw } ;
endwith

tbutton implements
    : draw ( -- )
\       xywh defocuscol @ @ dpy box
        e-tchoise e-tbutton
        text $@ h @ w @ 2/ min 0 textleft ;
class;

ticonbutton implements
    : draw ( -- )
        xywh e-tchoise nip nip e-draw
        x @ 1+ y @ 1+ h @ 2- callback fetch
        IF    icon+ h @ - 2/ + icon+ draw-at
        ELSE  icon- h @ - 2/ + icon- draw-at
        THEN  dpy mask
        text $@
        xN icon+ w @ icon- w @ max + 0 textleft ;
class;

Eicon: rbutton-fl+ Estyle/ShinyMetal/button_iconify_2.png"
Eicon: rbutton-fl- Estyle/ShinyMetal/button_off_2.png"
Eicon: rbutton-dl+ Estyle/ShinyMetal/button_iconify_1.png"
Eicon: rbutton-dl- Estyle/ShinyMetal/button_off_1.png"
Eicon: rbutton-pl+ Estyle/ShinyMetal/button_iconify_3.png"
Eicon: rbutton-pl- Estyle/ShinyMetal/button_off_3.png"

rbutton implements
    : e-rchoise ( -- l+ l- ms r )
        color @ $FF and 1 =
        IF    push?
            IF    rbutton-pl+ rbutton-pl- button-p
            ELSE  rbutton-fl+ rbutton-fl- button-f  THEN
        ELSE  push?
            IF    rbutton-dl+ rbutton-dl- button-d
            ELSE  rbutton-dl+ rbutton-dl- button-d  THEN
        THEN  ;
    : e-rbutton ( l+ l- ms -- )
      { l+ l- ms |
        xywh ms e-draw
        x @ y @ h @ h @ xS xywh-
        callback fetch IF  l+  ELSE  l-  THEN e-draw } ;
    : draw ( -- )
\       xywh defocuscol @ @ dpy box
        e-rchoise e-rbutton
        text $@ h @ w @ 2/ min 0 textleft ;
class;

\ text label

text-label implements
    : draw ( -- )
      xywh defocuscol @ @ dpy box
      button-d e-button
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
            x @ w @ r> - 2/ +  y @ h @ r> - 2/ +  of dup p-
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

Eicon: arrow-pt Estyle/ShinyMetal/button_arrow_up_3.png"
Eicon: arrow-ft Estyle/ShinyMetal/button_arrow_up_2.png"
Eicon: arrow-dt Estyle/ShinyMetal/button_arrow_up_1.png"
Eicon: arrow-pr Estyle/ShinyMetal/button_arrow_right_3.png"
Eicon: arrow-fr Estyle/ShinyMetal/button_arrow_right_2.png"
Eicon: arrow-dr Estyle/ShinyMetal/button_arrow_right_1.png"
Eicon: arrow-pb Estyle/ShinyMetal/button_arrow_down_3.png"
Eicon: arrow-fb Estyle/ShinyMetal/button_arrow_down_2.png"
Eicon: arrow-db Estyle/ShinyMetal/button_arrow_down_1.png"
Eicon: arrow-pl Estyle/ShinyMetal/button_arrow_left_3.png"
Eicon: arrow-fl Estyle/ShinyMetal/button_arrow_left_2.png"
Eicon: arrow-dl Estyle/ShinyMetal/button_arrow_left_1.png"

| Create tri-p-table  T] arrow-pl arrow-pt arrow-pr arrow-pb [
| Create tri-f-table  T] arrow-fl arrow-ft arrow-fr arrow-fb [
| Create tri-d-table  T] arrow-dl arrow-dt arrow-dr arrow-db [

tributton implements
    : draw ( -- )  xywh defocuscol @ @ dpy box
        xywh  color @ $FF and 1 =
        IF  push?  IF  tri-p-table  ELSE  tri-f-table  THEN
        ELSE  tri-d-table  THEN
        color @ $E >> $C and + perform e-draw ;
    : hglue  xM xS 2* + 1 *fil ;
    : vglue  xM xS 2* + 1 *fil ;
class;

slidetri implements
    | Create o-table  3 0 , ,  0 3 , ,  -3 0 , ,  0 -3 , ,
    | Create p-table  T] arrow-pl arrow-pt arrow-pr arrow-pb [
    | Create f-table  T] arrow-fl arrow-ft arrow-fr arrow-fb [
    | Create d-table  T] arrow-dl arrow-dt arrow-dr arrow-db [
    : draw ( -- )  xywh defocuscol @ @ dpy box
        xywh
        color @ $FF and 1 =
        IF  push?  IF  p-table  ELSE  f-table  THEN
        ELSE  d-table  THEN
        color @ $E >> $C and + perform e-draw ;
    : hglue tributton :: hglue drop 0 ;
    : vglue tributton :: vglue drop 0 ;
class;

\ slider

Eicon: hslider-d Estyle/ShinyMetal/bar_amber_horizontal_1.png"
Eicon: hslider-f Estyle/ShinyMetal/bar_amber_horizontal_2.png"
Eicon: hslider-p Estyle/ShinyMetal/bar_horizontal_3.png"

Eicon: vslider-d Estyle/ShinyMetal/bar_amber_vertical_1.png"
Eicon: vslider-f Estyle/ShinyMetal/bar_amber_vertical_2.png"
Eicon: vslider-p Estyle/ShinyMetal/bar_vertical_3.png"

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
public: cell var Eicons
how:
    : focus    1 color c! draw ;
    : defocus  0 color c! draw ;
    : init ( actor hxt vxt icon -- )
        Eicons ! super init ;
    : e-draw ( x y w h icon -- )
        Eicon-pixmap with draw-at endwith dpy mask ;
    : ep-draw ( x y w h icon -- ) >r parent xywh
        r> Eicon-pixmap with draw-at endwith
        >r >r 2drop 2drop 2drop { x y w h |
        x y parent xywh 2drop p- w h x y } r> r>
        dpy mask ;
    : draw ( -- )
        xywh Eicons @ ep-draw ;
class;

hslider implements
    : subbox ( -- ) 
        ^ R[ lstep ]R 0 slidetri new
        ^ R[ lpage ]R ['] part1 ['] part0 hslider-p Eside new
        ^ M[ slide ]M ['] part2 ['] part0 hslider-f hslider-d Erule new
          arule with $01000003 assign ^ endwith
        ^ R[ rpage ]R ['] part3 ['] part0 hslider-p Eside new
        ^ R[ rstep ]R 2 slidetri new
        5 ;
class;

hslider0 implements
    hslider :: subbox
class;

hscaler implements
    : subbox ( -- )
        ^ M[ slide ]M ['] part0a ['] part5 arule new
        ^ R[ lpage ]R ['] part1 ['] part0b hslider-p Eside new
        ^ M[ slide ]M :[ part4 swap 2* xS + swap ]:
                      :[ part0a swap xS 2* + swap ]:
                      hslider-f hslider-d Erule new
        arule with $01000003 assign ^ endwith
        ^ R[ rpage ]R ['] part3 ['] part0b hslider-p Eside new
        3 hbox new
        2 vbox new 1 ;
class;

vscaler implements
    : subbox ( -- )
        ^ M[ slide ]M ['] part5 ['] part0a arule new
        ^ R[ rpage ]R ['] part0b ['] part3 vslider-p Eside new
        ^ M[ slide ]M :[ part0a swap xS 2* + swap ]:
                      :[ part4 swap 2* xS + swap ]:
                      vslider-f vslider-d Erule new
        arule with $01000003 assign ^ endwith
        ^ R[ lpage ]R ['] part0b ['] part1 vslider-p Eside new
        3 vbox new
        2 hbox new 1 ;
class;

vslider implements
    : subbox ( -- )
        ^ R[ lstep ]R 1 slidetri new \ 1 ^ habox new fixbox
        ^ R[ lpage ]R ['] part0 ['] part1 vslider-p Eside new
        ^ M[ slide ]M ['] part0 ['] part2 vslider-f vslider-d Erule new
          arule with $02000003 assign ^ endwith
        ^ R[ rpage ]R ['] part0 ['] part3 vslider-p Eside new
        ^ R[ rstep ]R 3 slidetri new \ 1 ^ habox new fixbox
        5 ;
class;

vslider0 implements
    vslider :: subbox
class;

Eicon: vbutton-f Estyle/ShinyMetal/bar_vertical_2.png"
Eicon: vbutton-d Estyle/ShinyMetal/bar_vertical_1.png"
Eicon: vbutton-p Estyle/ShinyMetal/bar_vertical_3.png"

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

hrtsizer implements
    : he-choise ( -- m )
        color @ $FF and 1 =
        IF    push?
            IF    vbutton-p
            ELSE  vbutton-f  THEN
        ELSE  push?
            IF    vbutton-p
            ELSE  vbutton-d  THEN
        THEN  ;
    : draw ( -- )  xywh defocuscol @ @ dpy box
      he-choise e-button ; 
class;

hsizer implements
    hrtsizer :: draw
class;

hxrtsizer implements
    hrtsizer :: draw
class;

menu-entry implements
    : draw ( -- ) push? >r
        r@ 1 and
        IF    button-f e-button
        ELSE  xywh color @ dpy box  THEN
        text $@ xM r> 2 = 1 and textleft ;
class;

menu-title implements
    : draw ( -- )
        color 2+ c@
        IF    button-f e-button
        ELSE  xywh color @ dpy box  THEN
        text $@ 0 0 textcenter ;
class;

sub-menu implements
    : draw  ( -- ) push? >r
        r@ 1 and color 2+ c@ or
        IF    button-f e-button
        ELSE  xywh color @ dpy box  THEN
        text $@ xM r> 2 = 1 and textleft
        xM xS 2* + >r
        x @ w @ + r@ - y @ h @ r@ - 2/ + r> dup
        color $FF and 1 =
        IF   push? IF  tri-p-table  ELSE  tri-f-table  THEN
        ELSE  tri-d-table  THEN  2 cells + perform e-draw ;
class;

previous previous previous previous previous previous previous Forth
