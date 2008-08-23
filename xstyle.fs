\ boxchar                                              23aug97py

boxchar implements
        : draw  ( -- )   shadedbox
          color 2+ 1 0  push? 1 and  textcenter ;
class;

togglechar implements
        boxchar :: draw
class;

\ triangle button                                      23aug97py

tributton implements
        : draw ( -- )  xywh defocuscol @ @ dpy box
          xywh min 1- shadow push? IF swap THEN
          color @ $E >> $C and tritable + perform ;
        : hglue  xM 1 *fil ;
        : vglue  xM 1 *fil ;
class;

slidetri implements
        tributton :: draw
        : hglue tributton :: hglue drop 0 ;
        : vglue tributton :: vglue drop 0 ;
class;

\ button                                               23aug97py

button implements
        : draw  ( -- )   shadedbox
          text $@ 0 push? 1 and  textcenter ;
class;

menu-entry implements
        : draw ( -- )  push? >r
          xywh color @ dpy box
          r@ 1 and  IF  shadow xS xywh drawshadow  THEN
          text $@ menu-sep scan nip
          IF  xM r> 2 = 1 and -1 text menu-sep
              :[ rot >r 2over
                 r@ parent with combined tab@ endwith drop 0 p+
                 textleft r> 1+ ]: $iter  drop 2drop  EXIT  THEN
          text $@ xM r> 2 = 1 and textleft ;
class;

menu-title implements
        : draw  ( -- )  xywh color @ dpy box
          color 2+ c@ IF  shadow xS xywh drawshadow  THEN
          text $@ 0 0 textcenter ;
        button :: hglue
class;

sub-menu implements
        : draw  ( -- )  menu-entry :: draw
          color 2+ c@ IF  shadow xS xywh drawshadow  THEN
          xM >r x @ w @ + r@ - xS - 1- y @ h @ r@ - 2/ + r>
          shadow color 2+ c@ IF swap  THEN
[defined] VFXFORTH [ 0= ] [IF] \ !!!FIXME: confuses VFXFORTH
	  tributton tritable 2 cells + perform [THEN] ;
        : hglue ( -- glue )  menu-entry :: hglue
          xM 1+ 0 p+ ;
class;

lbutton implements
        : draw  ( -- ) shadedbox
          text $@ xS 1+ push? 1 and textleft ;
class;

text-label implements
        : draw  ( -- )  xywh color @ dpy box
          text $@ 0 0 textleft ;
        : hglue  textwh @       1 *fil ;
        : vglue  textwh cell+ @ 1 *fil ;
class;

text-word implements
        text-label :: draw  ( -- )
        : hglue  textwh @       0 ;
        : vglue  textwh cell+ @ 0 ;
class;

menu-label implements
        text-label :: draw
        text-label :: hglue
        text-label :: vglue
class;

alertbutton implements
        button :: draw
class;

\ toggle buttons                                       09sep97py

tbutton implements
        : draw ( -- )  halfshade
          xM  xN xS +  callback fetch
          { m n s }  shadow s IF  swap  THEN  xS
            x @ m 2/ + y @ h @ n - 1+ 2/ +  n dup
            s IF  2over 2over color @ 8 >> dpy box  THEN
          drawshadow
          text $@ m n + 0 textleft ;
        : hglue  textwh @ xN + xS + xN 2* + 3+ 1 *fil ;
        button :: vglue
class;

rbutton implements
        : draw ( -- ) halfshade
          xN   xM over + 2/ xS + swap
          #169 #239 */  dup xS + 1+   callback fetch
          { m n np s }  shadow
            s 0= IF  swap  THEN  >r >r
            x @ m + np -  y @ h @ 2/ +  over 1- over 1+ 2swap
            <poly np 1- np poly,  np 1- np negate poly, poly>
            r> dpy fill
            <poly np np 1+ negate poly,  np np 1+ poly, poly>
            r> dpy fill
            x @ m + n -  y @ h @ 2/ +  over 1- over 1+
            <poly n n 1+ negate poly, n n 1+ poly, poly>
            color @ s IF  8 >>  THEN  dpy fill
            <poly n 1- n poly, n 1- n negate poly, poly>
            color @ s IF  8 >>  THEN  dpy fill
            text $@ m 2* 0 textleft ;
        : hglue   super  hglue  2 0 p+ ;
        button :: vglue
class;

flipbutton implements
        : draw  ( -- ) color push  color @ $FFFFFF and
          callback fetch  IF -3 ELSE 2 THEN  $18 << or color !
          shadedbox  text $@ 0 push? 1 and textcenter ;
        : hglue  textwh @       xS 2* + 1+ 1 *fil ;
        : vglue  textwh cell+ @ xS 2* + 1+ 1 *fil ;
class;

togglebutton implements
        : draw  ( -- )  shadedbox
          callback fetch
          0= IF    text $@ 0 push? 1 and textcenter
             ELSE  text1 $@  2dup 0 textsize >r >r
                   xywh r> r> p- p2/ p+
                   push? dup p- color @ 8 >> .text  THEN  ;
        : hglue  textwh @ textwh1 @ max xS 2* + 1 *fil ;
        tbutton :: vglue
class;

topindex implements
        : draw-state { state o } ( --> state )
          state 0= IF  xywh drop xS defocuscol @ @ dpy box THEN
          shadow o IF swap THEN xS xywh
          state IF  xS +  ELSE  2swap xS + 2swap THEN drawshadow
          xywh 2swap xS dup p+ state 0= IF  xS +  THEN
          2swap xS dup 2* swap p- color @ dpy box
          x @ xS + y @ h @ + w @ xS 2* - xS
          state IF    defocuscol @ @
                ELSE  shadow drop  THEN dpy box
          text $@ state IF  0 o  ELSE  xS negate o xS + THEN
          textcenter
          x @ y @ h @ + xS dup shadow drop dpy box
          twoborders 0> IF
              x @ y @ h @ + twoborders dup shadow drop 2+ dpy box
              x @ xS + y @ h @ + w @ xS 2* - twoborders
              shadow drop 2+ dpy box
          THEN
          x @ w @ + xS - y @ h @ + xS dup shadow
          widgets self 'nil =
          IF  nip  ELSE  drop  THEN  dpy box
          state ;
        : draw  ( -- )
          callback fetch color @ $18 >> negate
          draw-state widgets self 'nil = = ?EXIT
          x @ w @ + xS - y @ h @ + widgets self 'nil <>
          xS + shadow   callback fetch
	    IF nip ELSE drop THEN  swap
	    [defined] VFXFORTH [IF] dup { n }
		0 ?DO  >r 2dup n I - 1 r@ dpy box 1+ r>  LOOP
	    [ELSE]
		0 ?DO  >r 2dup I' I - 1 r@ dpy box 1+ r>  LOOP
	    [THEN]
          drop 2drop ;
        button :: hglue
        button :: vglue
class;

icon-button implements
        : draw  ( -- )  shadedbox
          x @ xS + y @ h @ icon h @ - 2/ + push? dup p-
          icon draw-at dpy mask
          text $@
          xS 1+ icon w @ + xN + push? 1 and textleft ;
        : hglue   super hglue  swap icon w @ + xN + swap ;
        : vglue   super vglue  swap icon h @ xS 2* + 1+ max
          swap ;
class;

icon-but implements
        : draw ( -- )  push? 1 and >r shadedbox
          x @ w @ icon w @ - 2/ + r@ +
          y @ h @ icon h @ - 2/ + r> +
          icon draw-at dpy mask ;
        : hglue   icon w @ xS 2* + 1 *fil ;
        : vglue   icon h @ xS 2* + 1+ 1 *fil ;
class;

toggleicon implements
        : draw  ( -- )
          callback fetch  push?  { s of }
          shadedbox
          s IF    icon+ w @ icon+ h @
            ELSE  icon- w @ icon- h @  THEN  >r >r
          x @ w @ r> - 2/ +  y @ h @ r> - 2/ +  of dup p+
         s IF icon+ draw-at ELSE icon- draw-at THEN dpy mask ;
        : hglue  icon+ w @ icon- w @ max  xS 2* + 1+ 1 *fil ;
        : vglue  icon+ h @ icon- h @ max  xS 2* + 1+ 1 *fil ;
class;

flipicon implements
        : draw  ( -- )  color push  color @ $FFFFFF and
          callback fetch  IF -3 ELSE 2 THEN  $18 << or color !
          shadedbox  callback fetch 1 and >r
          x @ w @ icon w @ - 2/ + r@ +
          y @ h @ icon h @ - 2/ + r> +
          icon draw-at dpy mask ;
        : hglue  icon w @ xS 2* + 1+ 1 *fil ;
        : vglue  icon h @ xS 2* + 1+ 1 *fil ;
class;

ticonbutton implements
        : draw ( -- )  halfshade
          x @ 1+ y @ 1+ h @ 2- callback fetch
          IF    icon+ h @ - 2/ + icon+ draw-at
          ELSE  icon- h @ - 2/ + icon- draw-at
          THEN  dpy mask
          text $@
          xN icon+ w @ icon- w @ max + 0 textleft ;
        : hglue ( -- glue )  super hglue swap xM -
          icon+ w @ icon- w @ max + swap ;
        : vglue ( -- glue )  super vglue bounds
          icon+ h @ icon- h @ max xS + max tuck - ;
class;

big-icon implements
        : draw ( -- )  push? >r
          r@ 0= IF  xywh defocuscol @ @ dpy box  THEN
          x @ w @ icon w @ - 2/ + y @ icon draw-at dpy mask
          text $@
          xywh textwh @+ @ p- drop 2/ icon h @ p+
          2dup 2swap textwh @+ @
          color @  r@     IF  8 >>  THEN   dpy box
          color @  r> 0= IF  $8 >>  THEN   .text ;
        : hglue   super super hglue swap icon w @ max swap ;
        : vglue   textwh cell+ @ icon h @ + 1 *fil ;
        : dpy! ( dpy -- )  widget :: dpy!
          fnt self 0= IF
              2 dpy xrc font@ bind fnt !resized  THEN ;
class;

\ topglue, arule                                       28dec97py

topglue implements
        : draw  super draw
          x @ y @ h @ + w @ xS - xS shadow drop dpy box
          xywh p+ xS 0 p- xS shadow drop draw-edge ;
        : hglue super hglue xS 0 p+ ;
class;

arule implements
        : draw ( -- )  Xshadow dup >r
          IF  x @ y @ dpy txy!  THEN
           xywh r@ abs xywh- color @ dpy box
          shadow r@ xywh drawshadow
          r> IF  0 0 dpy txy!  THEN ;
class;

\ text input field                                     28dec97py

(textfield implements
        : draw ( -- )  xywh $D dpy box
          x @ curx @ + y @ curw @ xS + h @ { x y w h }
          color @ $FF and 1 =
          IF  x y w h color @ dpy box
              shadow xS 2/ x y w h drawshadow  THEN
          text $@ xS 2/ 0 textleft ;
        : dpy! ( dpy -- )  widget :: dpy!
          fnt self 0= IF
              3 dpy xrc font@ bind fnt !resized  THEN ;
class;

\ modal dialogs                                        28dec97py

modal implements
        : drawborder ( x y w h -- )
          xN 1+ 2/ negate xywh- drawshadow ;
        : draw ( -- )  attribs c@ :flip and ?EXIT
          super draw   default self 0= ?EXIT
          shadow swap xS 2/
          default with xywh drawborder endwith ;
class;

\ sliders                                              28dec97py

hslider implements
        : subbox ( -- o1 .. on n )  -2 borderw c! 
          ^ R[ lstep ]R 0 slidetri new
          ^ R[ lpage ]R ['] part1 ['] part0 arule new
          ^ M[ slide ]M ['] part2 ['] part0 arule new
             arule with $02000003 assign ^ endwith
          ^ R[ rpage ]R ['] part3 ['] part0 arule new
          ^ R[ rstep ]R 2 slidetri new 5 ;
class;

hslider0 implements
[defined] VFXFORTH [IF] \ !! FIXME: confuses VFXFORTH
        : subbox ( -- o1 .. on n )  -2 borderw c! 
          ^ R[ lstep ]R 0 slidetri new
          ^ R[ lpage ]R ['] part1 ['] part0 arule new
          ^ M[ slide ]M ['] part2 ['] part0 arule new
             arule with $02000003 assign ^ endwith
          ^ R[ rpage ]R ['] part3 ['] part0 arule new
          ^ R[ rstep ]R 2 slidetri new 5 ;
[ELSE]
        hslider :: subbox
[THEN]
class;

hscaler implements
        6 colors focuscol !  7 colors defocuscol !
        : subbox ( -- o1 .. on n )
          ^ M[ slide1 ]M ['] part0a ['] part5 arule new
          ^ R[ lpage ]R ['] part1 ['] part0b arule new
            ^ M[ slide ]M ['] part4 ['] part0a arule new
               arule with $01000003 assign ^ endwith
            ^ M[ slide ]M ['] part4 ['] part0a arule new
               arule with $01000003 assign ^ endwith
          2 hbox new 1 borderbox
          ^ R[ rpage ]R ['] part3 ['] part0b arule new
          3 hbox new -2 borderbox
          2 vbox new 1 ;
        : draw super draw  get nip nip #>text
          2dup fnt size drop textwh @ swap - 2/
          childs self vbox with childs widgets self endwith
          hbox with childs xywh endwith drop nip + + xS 2* +
          y @ color @ 8 >> .text ;
class;

vslider implements
        : subbox ( -- o1 .. on n )  -2 borderw c! 
          ^ R[ lstep ]R 1 slidetri new 
          ^ R[ lpage ]R ['] part0 ['] part1 arule new
           ^ M[ slide ]M ['] part0 ['] part2 arule new
             arule with $02000003 assign ^ endwith
          ^ R[ rpage ]R ['] part0 ['] part3 arule new
          ^ R[ rstep ]R 3 slidetri new 5 ;
class;

vslider0 implements
[defined] VFXFORTH [IF] \ !! FIXME: confuses VFXFORTH
        : subbox ( -- o1 .. on n )  -2 borderw c! 
          ^ R[ lstep ]R 1 slidetri new 
          ^ R[ lpage ]R ['] part0 ['] part1 arule new
           ^ M[ slide ]M ['] part0 ['] part2 arule new
             arule with $02000003 assign ^ endwith
          ^ R[ rpage ]R ['] part0 ['] part3 arule new
          ^ R[ rstep ]R 3 slidetri new 5 ;
[ELSE]
        vslider :: subbox
[THEN]
class;

vscaler implements
        6 colors focuscol !  7 colors defocuscol !
        : subbox ( -- o1 .. on n )
          ^ M[ slide1 ]M ['] part5 ['] part0a arule new
          ^ R[ rpage ]R ['] part0b ['] part3 arule new
            ^ M[ slide ]M ['] part0a ['] part4 arule new
               arule with $01000003 assign ^ endwith
            ^ M[ slide ]M ['] part0a ['] part4 arule new
               arule with $01000003 assign ^ endwith
          2 vbox new 1 borderbox
          ^ R[ lpage ]R ['] part0b ['] part1 arule new
          3 vbox new -2 borderbox
          2 hbox new 1 ;
        : draw super draw  get nip nip #>text
          2dup fnt size drop textwh @ swap - x @ +
          childs self vbox with childs widgets self endwith
          hbox with childs xywh endwith nip + nip xS 3* 2/ +
          color @ 8 >> .text ;
class;

\ Motif sizer

0 [IF]
hrtsizer implements
        : draw ( -- )  xywh defocuscol @ @ dpy box
          shadow swap xS 2/ x @ w @ 2/ + 1- y @ xS h @
                drawshadow
          x @ 0 dpy txy!  xM  xN xS +
          { m n }  shadow xS
            x @ w @ n - 2/ +    y @ h @ + m 2/ -
            n w @ n - 1 and + dup >r - r> dup 2over 2over
            color @ dpy box drawshadow 0 0 dpy txy! ;
        : hglue ( -- glue )  xN xS 2* 1+ + -2 and  0 ;
        : vglue ( -- glue )  xM  xN xS + + 1 *filll ;
class;

vrtsizer implements
        : draw ( -- )  xywh defocuscol @ @ dpy box
          shadow swap xS 2/ x @ y @ h @ 2/ + 1- w @ xS
               drawshadow
          0 y @ dpy txy!  xM  xN xS +
          { m n }  shadow xS
            x @ w @ + m 2/ -  n h @ n - 1 and + dup >r -
            y @ h @ n - 2/ +  r> dup  2over 2over
            color @ dpy box drawshadow 0 0 dpy txy! ;
        : hglue ( -- glue )  xM  xN xS + + 1 *filll ;
        : vglue ( -- glue )  xN xS 2* 1+ + -2 and  0 ;
class;
[THEN]
