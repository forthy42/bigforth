\ windows 95 style corrections

minos also

1 to twoborders

[IFDEF] x11
: (win95-sys-colors ( -- )
    defers get-sys-colors
    $FFFFFF Colortable 4 cells + !
    $800000 Colortable 5 cells + ! ;

' (win95-sys-colors IS get-sys-colors

gray-colors
[THEN]

\ triangle button                                      23aug97py

tributton implements
    : trixy95 ( x y h lc sc -- d x0 y0 c )
        push? IF  swap  THEN  drop >r
        dup xS 2* - 2/ 1- >r 2/ push? - dup p+ r> -rot r> ;
    :noname ( x y h lc sc -- )  trixy95 { d x0 y0 c |
        x0 d - y0 xS - <poly d d poly, d d negate
        poly, poly> c dpy fill } ;
    :noname ( x y h lc sc -- )  trixy95 { d x0 y0 c |
        x0 xS - y0 d - <poly d d poly, d negate d
        poly, poly> c dpy fill } ;
    :noname ( x y h lc sc -- )  trixy95 { d x0 y0 c |
        x0 d - y0 xS + <poly d d negate poly, d d
        poly, poly> c dpy fill } ;
    :noname ( x y h lc sc -- )  trixy95 { d x0 y0 c |
        x0 xS + y0 d - <poly d negate d poly, d d
        poly, poly> c dpy fill } ;
    tritable !+ !+ !+ !
    : draw ( -- )  shadedbox
        xywh min 1-  2 2
        color @ $E >> $C and tritable + perform ;
    : hglue  xM xS 2* + 1 *fil ;
    : vglue  xM xS 2* + 1 *fil ;
class;

slidetri implements
    tributton :: draw
    : hglue tributton :: hglue drop 0 ;
    : vglue tributton :: vglue drop 0 ;
class;

\ toggle buttons                                       09sep97py

tbutton implements
    : draw ( -- )  halfshade
        xM  xM xS +  callback fetch
        { m n s |  shadow swap xS
        x @ m 2/ + y @ h @ n - 1+ 2/ +  n dup
        2over 2over $D dpy box
        s IF  2over 2over xS 1+ xywh- { w h |
            0 h 3 /f p+
            <poly
            w 3 /f h 3 /f poly,
            w w 3 /f - h h 3 /f - negate poly,
            0 h 3 /f poly,
            w w 3 /f - negate h h 3 /f - negate negate poly,
            w 3 /f negate h 3 /f negate poly,
            poly> $C dpy fill }
        THEN
        drawshadow
        text $@ m n + 0 textleft } ;
    : hglue  textwh @ xM + xS + xN 2* + 3+ 1 *fil ;
    button :: vglue
class;

rbutton implements
    : draw ( -- )  halfshade
        xM  xM xS + 2/ dup xS 2* -
        callback fetch
        { m n n' s |
        x @ m 2/ + y @ h @ n 3* - 1+ 2/ + n 2* +
        2dup <poly
        0 n negate poly,
        n' n' negate poly,
        n 0        poly,
        n' n'        poly,
        poly> shadow nip dpy fill
        2dup <poly
        n' n'        poly,
        n 0        poly,
        n' n' negate poly,
        0 n negate poly,
        poly> shadow drop dpy fill
        n' xS - 1+ to n' n 2- to n
        over xS + over xS 2/ - <poly
        0  n negate poly,
        n' n' negate poly,
        n 0         poly,
        n' n'        poly,
        0  n        poly,
        n' negate n' poly,
        n negate 0  poly,
        poly> $D dpy fill
        s IF
            over xS 3* 2/ + over xS 2/ dup 2/ + -
            n' 1- to n'
            <poly
            0  n negate poly,
            n' n' negate poly,
            n 0         poly,
            n' n'        poly,
            0  n        poly,
            n' negate n' poly,
            n negate 0  poly,
            poly> $C dpy fill
        THEN
        2drop
        text $@ m n 3* + 0 textleft } ;
    : hglue  textwh @ xM + xS + xN 2* + 3+ 1 *fil ;
    button :: vglue
class;


\ sliders                                              28dec97py

hslider implements
    : init ( callback -- )  >callback
        ^ R[ lstep ]R 0 slidetri new \ 1 ^ habox new fixbox
        ^ R[ lpage ]R ['] part1 ['] part0 arule new
        arule with $00000001 assign ^ endwith
        ^ M[ slide ]M ['] part2 ['] part0 arule new
        arule with $02000003 assign ^ endwith
        ^ R[ rpage ]R ['] part3 ['] part0 arule new
        arule with $00000001 assign ^ endwith
        ^ R[ rstep ]R 2 slidetri new
        5 super init ;
class;

hslider0 implements
    hslider :: init
class;

vslider implements
    : init ( callback dpy -- )  >callback
        ^ R[ lstep ]R 1 slidetri new
        ^ R[ lpage ]R ['] part0 ['] part1 arule new
        arule with $00000001 assign ^ endwith
        ^ M[ slide ]M ['] part0 ['] part2 arule new
        arule with $02000003 assign ^ endwith
        ^ R[ rpage ]R ['] part0 ['] part3 arule new
        arule with $00000001 assign ^ endwith
        ^ R[ rstep ]R 3 slidetri new
        5 super init ;
class;

vslider0 implements
        vslider :: init
class;

sliderview implements
        true border-at v!
class;

asliderview implements
        true border-at v!
class;

menu-title implements
    : draw  ( -- )  xywh color @ dpy box
        color 2+ c@ IF  shadow swap xS 2/ xywh xS 2/ xywh- drawshadow  THEN
        text $@ 0 0 textcenter ;
    : moved ( x y -- ) :: moved draw
      shadow xS 2/ xywh xS 2/ xywh- drawshadow ;
    : leave :: leave draw ;
class;

menu-entry implements
    : draw ( -- )  push? >r
        xywh color @ dpy box
        text $@ menu-sep scan nip
        IF  xM r> 2 = 1 and -1 text menu-sep
            :[ rot >r 2over
               r@ parent with combined tab@ endwith drop 0 p+
               textleft r> 1+ ]: $iter  drop 2drop  EXIT  THEN
        text $@ xM r> 2 = 1 and textleft ;
    : vglue  super vglue swap xS - swap ;
class;

[IFDEF] editor
also editor

edimenu-entry implements
    menu-entry :: draw
    menu-entry :: vglue
class;

previous
[THEN]

sub-menu implements
    2 colors focuscol !     3 colors defocuscol !
    : draw  ( -- )  menu-entry :: draw
        xM >r x @ w @ + r@ - xS - 1- y @ h @ r@ - xS - 2/ +
        push? dup p+ r> xS +
        6 4 tributton tritable 2 cells + perform ;
class;

info-menu implements
    : init  ( widget addr len dpy -- )
        text-label new bind info  bind callw
        callw self combined with childs get endwith 0 ST[ ]ST
        textfield new dup bind text 0 borderbox
        0 text edit ds !
        ^ M[ clicked ]M 3 tributton new bind tri
        info self 1 habox new hfixbox  text self
        tri self
        1 vbox new hfixbox 2 hbox new  -2 borderbox
        1 vbox new
        ^ S[ ]S :[ callw hglue ]: :[ 0 0 ]: arule new
        2 vbox new  +fill 3 super init drop ;
class;

previous forth
