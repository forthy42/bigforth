\ viewport                                             02jan05py
$20 Value /mslide

backing class viewport
public: 0 var org
private: cell var orgy          cell var orgx
        2 cells var steps
public: cell var sw             cell var sh
        cell var hstep          cell var vstep
        cell var minw           cell var minh
        2 cells var cliprec
        method 'hslide          method 'vslide
        method slided
        hslider ptr hspos       vslider ptr vspos
        early hslide            early vslide
        method xpos!            method ypos!

\ viewport                                             28mar99py
how:    : init  ( sx sy -- )  noback on  super init
          2dup steps 2! vstep ! hstep ! ;
        : trans ( x y -- x' y' )
          orgx @ hstep @ * x @ -
          orgy @ vstep @ * y @ - p+ ;
        : trans' ( x y -- x' y' )
          orgx @ hstep @ * x @ -
          orgy @ vstep @ * y @ - p- ;
        : <max  ( x y -- xmy y )  tuck max swap ;
        : hslide  w @ hstep @ /
          sw @ hstep @ / <max  orgx @ ;
        : vslide  h @ vstep @ /
          sh @ vstep @ / <max  orgy @ ;
        : screenpos ( -- x y )  dpy screenpos trans' ;
        gadget :: delete

\ viewport                                             11aug99py
[defined] x11 [IF]
        : create-pixmap ( -- )
          xwin @ IF  0 0 0 sp@ >r
	      xrc dpy @ xwin @  dummy dup dup
	      r> dup cell+  dummy  over cell+  XGetGeometry drop
	      * * 3 >> maxpixmap + TO maxpixmap
	      xrc dpy @ xwin @ XFreePixmap  THEN
          xwin off  noback @ ?EXIT
          xrc depth @
          dup h @ w @ * * 3 >> dup
          maxpixmap > IF  2drop EXIT  THEN
          maxpixmap swap - TO maxpixmap
          h @ 1 max w @ 1 max
          screen xwin @ xrc dpy @
          XCreatePixmap xwin ! ;                [THEN]

\ viewport                                             21oct99py
        : ?incbase ( b i -- )  dup 1 = IF  2drop 0 1  THEN ;
        : !steps
          child xinc ?incbase steps cell+ @ * hstep ! minw !
          child yinc ?incbase steps       @ * vstep ! minh ! ;
        : resized ( -- )  get-glues
          hglues cell+ @ sw @ max w !
          vglues cell+ @ sh @ max h !   !steps
          parent resized ( dpy resized ) ;
        : resize ( x y w h -- )  sh ! sw ! y ! x !
          hglues cell+ @ sw @ max w !
          vglues cell+ @ sh @ max h !
          & glue @ child class? 0=
          IF  0 0 w @ h @ child resize create-pixmap  xwin @
              IF    flags #draw 2dup bit@ >r -bit  child draw
	            r> IF  flags #draw +bit  THEN  THEN
          THEN  hslide -rot - min  vslide -rot - min org 2! ;

\ viewport                                             28mar99py

        : hglue  ( -- g )  hstep @ hglues 2@ + over -
          over minw @ - 0min + swap minw @ max swap ;
        : vglue  ( -- g )  vstep @ vglues 2@ + over -
          over minh @ - 0min + swap minh @ max swap ;

        : dpy!  bind dpy  xrc self IF  xrc dispose  THEN
          dpy xrc clone bind xrc  create-pixmap
[defined] x11 [IF]     get-win xrc get-gc  [THEN]
          0 clip-rect  flags #draw +bit
          self child dpy!
          child hglue 2dup hglues 2! drop
          child vglue 2dup vglues 2! drop  !steps
          2dup h ! w !  0 0 2swap child resize ;

\ viewport                                             09nov97py

        : clipx ( -- x w )   xwin @ 0=
          IF    orgx @ hstep @ * sw @
                cliprec 2@ d0= 0=
                IF  0 and cliprec    w@+ 2+ w@ p+ THEN
          ELSE  0 w @  THEN ;
        : clipy ( -- y h )  xwin @ 0=
          IF    orgy @ vstep @ * sh @
                cliprec 2@ d0= 0=
                IF  0 and cliprec 2+ w@+ 2+ w@ p+ THEN
          ELSE  0 h @  THEN ;
        : clipxywh ( -- x y w h )  cliprec 2@ d0=
          IF    xywh
          ELSE  cliprec w@+ w@+ w@+ w@
                2swap x @ y @ p+ 2swap  THEN ;

\ viewport                                             27mar99py

        : clipxy ( x y d -- x' y' )  clipxywh
          { d x y w h } x y p- swap
	    w d + min d max swap
	    h d + min d max  x y p+ ;
        : clip  ( x y w h -- x y w h )
          >xyxy 2swap 0 clipxy 2swap -1 clipxy >xywh ;

        : !resized  super !resized  !steps ;

        : inclip? ( x y -- flag )  trans'
          $-8000 $8000 within swap
          $-8000 $8000 within and ;

\ viewport                                             17dec00py

        : draw ( -- )   clip-should @
          IF  xrect w@+ w@+ >r trans' r> w@+ w@ clip
              2swap 2drop 0= swap 0= or ?EXIT  THEN
          xwin @  IF    orgx @ hstep @ *    orgy @ vstep @ *
                        xywh 2swap
                        xwin @ dpy image
                  ELSE  child draw  THEN ;
        : assign ( widget -- )  set-child
          self child dpy!
          child hglue 2dup hglues 2! drop
          child vglue 2dup vglues 2! drop  !steps
          2dup h ! w !  0 0 2swap child resize  parent resized ;

\ viewport                                             24jan98py

        : <clip ( -- )   !txy
          clip-should @ dup 0=  IF  self nip  THEN
          dup dpy clip-should !  dpy clip-is @ =
          IF    xwin @ 0= ?EXIT  THEN   clip-should @
          IF    xrect w@+ w@+ >r trans' r> w@+ w@ clip
          ELSE  clipxywh  THEN  swap 2swap
          dpy with  clip-should @ dpy clip-is @ =
                    IF  xwin @ 0=  IF transback THEN  THEN
          endwith
          swap dpy xrect w!+ w!+ w!+ w!
          dpy xrect dpy clip-rect  clip-should @ clip-is !
          dpy clip-should @ dpy clip-is ! ;

\ viewport                                             25jul98py

        : clip> ( -- )
          dpy clip-should off  0 0 dpy txy!  xwin @ 0= ?EXIT
          0 clip-rect  clip-is off  clip-should off
          dpy clip-is off ;

        : xinc    0 hstep @ ;
        : yinc    0 vstep @ ;
        : >exposed xwin @ 0= IF  dpy >exposed  THEN ;
        : xywh ( -- x y w h )  x @ y @ sw @ sh @ ;

        : clipback ( x y w h -- x' y' w' h' )
          dpy with  xwin @ 0=
          IF  2swap trans' 2swap clip recurse
              2swap trans  2swap  THEN  endwith ;

\ viewport                                             02jan05py

        : (xpos! { o x0 y0 x y w h }
	    x o 0max + y  dpy transback
	    w o abs - h x o 0min - y
	    dpy get-win  dpy image \ dpy >exposed
	    h o abs 0  y y0 - 0max
	    o 0> IF  w + o -  THEN  x x0 - 0max +
	    cliprec w!+ w!+ w!+ w! ;
        : xpos! ( p -- )  orgx @ case? 0=
          IF  orgx @ over  orgx ! -  hstep @ *
              dup abs sw @ <
              IF  clipxywh 2over 2swap clipback (xpos!
              ELSE  drop  THEN  draw  0, cliprec 2!  THEN ;

\ viewport                                             02jan05py

        : (ypos! { o x0 y0 x y w h }
	    x y o 0max +  dpy transback
	    w h o abs - x y o 0min -
	    dpy get-win  dpy image \ dpy >exposed
	    o abs w 0    o 0> IF  h + o -  THEN
	    y y0 - 0max + x x0 - 0max
	    cliprec w!+ w!+ w!+ w! ;
        : ypos! ( p -- )  orgy @ case? 0=
          IF  orgy @ over  orgy ! -  vstep @ *
              dup abs sh @ <
              IF  clipxywh 2over 2swap clipback (ypos!
              ELSE  drop  THEN  draw  0, cliprec 2!  THEN ;
        : 'hslide self ['] xpos! ['] hslide toggle-state new ;
        : 'vslide self ['] ypos! ['] vslide toggle-state new ;

\ viewport                                             20oct99py
        : line ( x y x y color -- )  >r  2dup inclip?
          IF  xwin @  IF  2over 2over r@ super super line  THEN
              flags #draw bit@ IF  2over trans' 2over trans' r@
                          <clip dpy line clip>  THEN  THEN
          rdrop 2drop 2drop ;
        : text ( addr u x y color -- )  >r 2dup inclip?
          IF  xwin @  IF  2over 2over r@ super super text  THEN
              flags #draw bit@ IF  2over 2over trans' r@
                          <clip dpy text clip>  THEN  THEN
          rdrop 2drop 2drop ;
        : box ( x y w h color -- ) >r
          xwin @  IF  2over 2over r@ super super box  THEN
          flags #draw bit@ IF  2over trans' 2over clip
                      r@ <clip  dpy box  clip>  THEN
          rdrop 2drop 2drop ;

\ viewport                                             28jun98py
        : image ( x y w h x y win -- ) >r 2dup inclip?
          IF  xwin @  IF  [ 5 ] [FOR] 5 pick [NEXT] r@
                          super super image  THEN
              flags #draw bit@ IF  r@ xwin @ =
                          IF    draw
                        ELSE [ 5 ] [FOR] 5 pick [NEXT] trans' r@
                               <clip dpy image clip>  THEN THEN
          THEN  rdrop 2drop 2drop 2drop ;
        : mask ( x y w h x y win1 win2 -- ) 2over inclip?
          IF  xwin @ IF  [ 7 ] [FOR] 7 pick [NEXT]
                         super super mask  THEN
              flags #draw bit@
              IF  [ 5 ] [FOR] 7 pick [NEXT] trans' 7 pick 7 pick
                  <clip dpy mask clip>  THEN
          THEN  2drop 2drop 2drop 2drop ;

\ viewport                                             28jun98py
        : fill ( x y addr n color -- )  >r  2over inclip?
          IF  xwin @  IF  2over 2over r@ super super fill  THEN
              flags #draw bit@ IF  2over trans' 2over r@
                          <clip dpy fill clip>  THEN
          THEN  rdrop 2drop 2drop ;
        : stroke ( x y addr n color -- )  >r  2over inclip?
          IF xwin @  IF  2over 2over r@ super super stroke  THEN
             flags #draw bit@ IF  2over trans' 2over r@
                         <clip dpy stroke clip>  THEN
          THEN  rdrop 2drop 2drop ;
        : drawer ( x y o xt -- )
          xwin @  IF  2over 2over super super drawer  THEN
          flags #draw bit@ IF  2over trans' 2over
                      <clip dpy drawer clip>  THEN
          2drop 2drop ;

\ viewport                                             09nov97py
        : inside?  ( x y -- )
          y @ - sh @ u< swap x @ - sw @ u< and ;
        : xlegal ( x -- x' )
          w @ sw @ - hstep @ / min 0max ;
        : ylegal ( y -- y' )
          h @ sh @ - vstep @ / min 0max ;
        : slided ( -- )
          hspos self  IF  hspos draw  THEN
          vspos self  IF  vspos draw  THEN ;
        : show-me  ( x y -- ) sw @ sh @
          { x y w h }  y orgy @ vstep @ * - h u>= dup
            IF  y h 2/ - vstep @ / ylegal ypos!  THEN
            x orgx @ hstep @ * - w u>= dup
            IF  x w 2/ - hstep @ / xlegal xpos!  THEN
            or IF  slided  THEN  x y trans' dpy show-me ;

\ viewport                                             02jan05py

        : scroll  ( x y -- )  sw @ 4- sh @ 4- { x y w h }
          y orgy @ vstep @ * - dup
          0<    IF  drop y  ELSE
          h >=  IF  y h - orgy @ 1+ vstep @ * max BUT  THEN
                    vstep @ / ylegal ypos! true  ELSE false THEN
          x orgx @ hstep @ * - dup
          0<    IF  drop x  ELSE
          w >=  IF  x w - orgx @ 1+ hstep @ * max BUT  THEN
                    hstep @ / xlegal xpos! true  ELSE false THEN
          or IF  slided moved!  THEN
          x y trans' dpy scroll ;
        : focus    child focus   ;
        : defocus  child defocus ;

\ viewport                                             22jan05py

        : ud ( -- n )  /mslide vstep @ / 1+ ;
        : clicked ( x y b n -- )
          over $18 and over 1 and 0= and
          sh @ h @ < and IF  \ scroll
             over $10 and  IF  orgy @ ud + ylegal ypos!  THEN
             over $08 and  IF  orgy @ ud - ylegal ypos!  THEN
             over $18 and  IF  slided  THEN
             2drop 2drop
          ELSE  super clicked  THEN ;

\ viewport                                             22jan05py

        : /vpage ( -- n )  sh @ vstep @ / 1- 1 max ;
        : /hpage ( -- n )  sw @ hstep @ / 1- 1 max ;
        : keyed ( key sh -- )  dup 1 and IF
          over $FF55 =  IF  orgy @ /vpage - ylegal ypos!
                            slided  2drop  EXIT  THEN
          over $FF56 =  IF  orgy @ /vpage + ylegal ypos!
                            slided  2drop  EXIT  THEN
          THEN  super keyed ;
class;

\ viewport variants                                    12nov06py

viewport class hviewport
        2 cells var yincs
how:    : vglue  ( -- g )  vglues 2@ ;
        : resized  child vglue nip 0=
          IF 0 1 ELSE child yinc THEN yincs 2! super resized ;
        : yinc   yincs 2@ ;
        : clicked ( x y b n -- )
          over $18 and over 1 and 0= and
          sw @ w @ < and IF  \ scroll
             over $10 and  IF  orgx @ ud + xlegal xpos!  THEN
             over $08 and  IF  orgx @ ud - xlegal xpos!  THEN
             over $18 and  IF  slided  THEN
             2drop 2drop
          ELSE  super super clicked  THEN ;

\ viewport variants                                    22jan05py

        : keyed ( key sh -- )  dup 1 and  IF
          over $FF55 =  IF  orgx @ /hpage - xlegal xpos!
                            slided  2drop  EXIT  THEN
          over $FF56 =  IF  orgx @ /hpage + xlegal xpos!
                            slided  2drop  EXIT  THEN
          THEN  super super keyed ;
class;
viewport class vviewport
        2 cells var xincs
how:    : hglue  ( -- g )  hglues 2@ ;
        : resized  child hglue nip 0=
          IF 0 1 ELSE child xinc THEN xincs 2! super resized ;
        : xinc   xincs 2@ ;
class;

\ Sskip                                                11nov06py
vviewport class clipper
how:    : vglue  ( -- g )  vglues 2@ ;
        : resized  super super resized ;
        : init  1 1 super init ;
        backing :: xinc
        backing :: yinc
class;

Mskip class Sskip
public: widget ptr vsize        widget ptr hsize
how:    : init ( ho vo -- )  super init
          bind vsize bind hsize ;
        : hglue  hsize hglue drop 0 ;
        : vglue  vsize vglue drop 0 ;
class;

\ Container object                                     03jan98py

vbox class sliderview           cell var glues
public: gadget ptr inner        & inner viewport asptr viewp
        static border-at
how:    0 border-at v!

\ Container object                                     08apr00py

        : init ( viewport -- ) dup bind inner
          1 hbox new rzbox
          -2  border-at @ IF  borderw c!  ELSE  borderbox  THEN
          & viewport @ inner class?
          IF  & hviewport @ viewp class?  0=
              IF  viewp 'vslide vslider0 new
              dup viewp bind vspos  2 hbox new rzbox THEN
              & vviewport @ viewp class?  0=
              IF  viewp 'hslide hslider0 new
                  dup viewp bind hspos
                  & hviewport @ viewp class?
                  0= IF  viewp vspos self  viewp hspos self
                  Sskip new  2 hbox new rzbox THEN
         2 ELSE 1 THEN ELSE 1 THEN super init ;

\ Container object                                     14sep97py

        : glue-off  0, hglues 2!  0, vglues 2!
          & viewport @ inner class?
          IF  & hviewport @ viewp class?  0=
              IF  childs hglue 2drop  childs vglue 2drop  THEN
              & vviewport @ viewp class?  0=
              IF  childs widgets hglue 2drop
                  childs widgets vglue 2drop  THEN
          THEN ;
        : glues? ( -- n )
          & vviewport @ viewp class? 0=
          IF  viewp hspos vglue drop 0= 1 and  ELSE  0  THEN
          & hviewport @ viewp class? 0=
          IF  viewp vspos hglue drop 0= 2 and  ELSE  0  THEN
          or ( dup . ) dup glues ! ;

\ Container object                                     28mar99py

        : ?portwin
          viewp hslide -rot - min  viewp vslide -rot - min
          viewp org 2!
          viewp xwin @
          IF    viewp noback @ IF  viewp draw  THEN
          ELSE  viewp noback @ 0= IF
                viewp create-pixmap
                viewp flags #draw dup push off
                viewp child draw  THEN  THEN ;
        : sresize ( x y w h -- x y w h )
          viewp org 2@ >r >r
          2over 2over  super resize glue-off
          r> r> viewp org 2! ;

\ Container object                                     03oct98py

        : >parent
          dpy counter @ 4 <
          IF
              1 dpy counter +!  parent resized -1 dpy counter +!
          THEN ;

\ Container object                                     28mar99py

\        : >parent  parent resized ;
        : resize ( x y w h -- )
          & viewport @ inner class?
          IF  viewp noback dup @ >r on viewp flags #draw -bit
              glues @ >r  viewp org 2@ >r >r
              2over 2over xS xywh- viewp resize glue-off
              r> r> viewp org 2!  glues? dup r@ or
              IF  >r sresize glues? r> <> IF  sresize  THEN
                  glues? r> <>  r> viewp noback ! viewp flags #draw +bit
                  IF    >parent parent draw
                  THEN  2drop 2drop  ?portwin EXIT
              THEN  drop rdrop  r> viewp noback ! viewp flags #draw +bit
          THEN  super resize ;
class;

\ Container object                                     03jan98py

sliderview class asliderview
how:    vabox :: (clicked       vabox :: show-you
        vabox :: focus          vabox :: defocus
        vabox :: keyed          vabox :: handle-key?
        0 border-at v!

\ Container object                                     08apr00py
        : init ( viewport -- ) dup bind inner
          1 habox new rzbox
          -2  border-at @ IF  borderw c!  ELSE  borderbox  THEN
          & viewport @ inner class?
          IF  & hviewport @ viewp class?  0=
              IF  viewp 'vslide      vslider0 new
              dup viewp bind vspos 2 habox new rzbox THEN
              & vviewport @ viewp class?  0=
              IF  viewp 'hslide      hslider0 new
                  dup viewp bind hspos
                  & hviewport @ viewp class?
                  0= IF  viewp vspos self  viewp hspos self
                 Sskip new  2 habox new rzbox THEN
                  2  ELSE  1  THEN
          ELSE  1  THEN super super init ; class;

\ display simplicifacionts                             06feb00py

: D[   postpone >r ;                         immediate restrict
| : (]D   r> r> swap >r displays with  assign self  endwith ;
                                                       restrict

: DS[  postpone >r ;                         immediate restrict
| : (]DS  r> r> swap >r viewport with  assign self  endwith
    asliderview new ;                                  restrict
[defined] loffset [IF]
: ]D  -cell loffset +!  postpone (]D ;       immediate restrict
: ]DS  -cell loffset +!  postpone (]DS ;     immediate restrict
[ELSE]
: ]D   postpone (]D ;       immediate restrict
: ]DS  postpone (]DS ;     immediate restrict
[THEN]

\ Container object                                     03aug98py

vabox class vresize
how:    : yinc  0 childs vglue drop ;
class;

habox class hresize
how:    : xinc  0 childs hglue drop ;
class;

\ Container object                                     21oct99py
: /glue ( glue addr -- glue' )
  >r  over swap + r@ @ min max dup r> ! 0 ;
Variable do-size
vabox class vasbox
public: cell var hsize          cell var vsize
        method oglue
how:    : vglue  ( -- glue )  super vglue  vsize /glue ;
        : vglue@ ( -- glue )  super vglue@ vsize /glue ;
        : oglue  ( -- glue )  super vglue ;
        : yinc   ( -- inc )   0 1 ;
        : resized  do-size @ IF  dpy counter @ 2 > ?EXIT  THEN
          super resized vglue 2drop ;
        : init  super init ;
class;

\ Container object                                     21oct99py



habox class hasbox
public: cell var hsize          cell var vsize
        method oglue
how:    : hglue  ( -- glue )  super hglue  hsize /glue ;
        : hglue@ ( -- glue )  super hglue@ hsize /glue ;
        : oglue  ( -- glue )  super hglue ;
        : xinc   ( -- inc )   0 1 ;
        : resized  do-size @ IF  dpy counter @ 2 > ?EXIT  THEN
          super resized hglue 2drop ;
        : init  super init ;
class;

\ vsizer                                               28mar99py
boxchar class vrtsizer
public: method drawxorline      & parent vasbox asptr vsized
        early relation?         method <size>
how:    : init ( -- )
          noop-act 0 ( rot) super init ;
        Variable vsize'
        : relation? ( -- flag )  widgets self 'nil = ;
        : hglue ( -- glue )  xN 1 *filll ;
        : vglue ( -- glue )  xN 0 ;
        : draw ( -- )  shadedbox ;
        : moved ( x y -- )  2drop
[defined] x11 [IF]
          relation?  IF  XC_bottom_side  ELSE  XC_top_side  THEN
 [THEN] [defined] win32 [IF]  IDC_SIZENS  [THEN]
          dpy set-cursor ^ dpy set-rect ;

\ vsizer                                               27mar99py

        : maxsize ( -- m )
          vsized parent with h @ vglue drop - endwith ;
        : <size> ( h -- h' )
          vsized oglue over >r + min 0max maxsize
          vsized vsize @ +  min r> max 0max ;
        : size! ( h -- ) <size> dup vsized vsize
          dup @ -rot ! = ?EXIT
          do-size on  vsized resized  do-size off ;
        : drawxorline  vsized vsize @ vsize' @ <>
          IF  vsize' @ size!  THEN ;
        : >released ( x y -- ) vsized vsize @
          relation? IF  -  ELSE  +  THEN
          DOPRESS  drawxorline  nip relation? IF  swap  THEN -
                   <size> vsize' ! drop drawxorline ;

\ vsizer                                               27mar99py
        : keyed ( key sh -- )   drop
          relation? IF  $FF57  ELSE  $FF50  THEN  case?
          IF  maxsize vsized vsize @ +
                                size!  EXIT  THEN
          relation? IF  $FF50  ELSE  $FF57  THEN  case?
          IF  0                 size!  EXIT  THEN
          relation? IF  $FF54  ELSE  $FF52  THEN  case?
          IF  vsized vsize @ vsized yinc nip + size!  EXIT  THEN
          relation? IF  $FF52  ELSE  $FF54  THEN  case?
          IF  vsized vsize @ vsized yinc nip - size!  EXIT  THEN
          drop ;
        : clicked ( x y b n -- )  2over moved  nip 1 and 0=
          IF  2drop  EXIT  THEN  vsized vsize @ vsize' !
          drawxorline  >released ( cc )
          drawxorline   vsize' @ size! ;        class;

\ hsizer                                               28mar99py
boxchar class hrtsizer
public: method drawxorline      & parent hasbox asptr hsized
        early relation?         method <size>
how:    : init ( -- )
          noop-act 0 ( rot) super init ;
        Variable hsize'
        : relation? ( -- flag )  widgets self 'nil = ;
        vrtsizer :: draw
        : hglue ( -- glue )  xN 0 ;
        : vglue ( -- glue )  xN 1 *filll ;
        : moved ( x y -- )  2drop
[defined] x11 [IF]
          relation?  IF  XC_right_side  ELSE  XC_left_side  THEN
 [THEN] [defined] win32 [IF]  IDC_SIZEWE  [THEN]
          dpy set-cursor  ^ dpy set-rect ;

\ hsizer                                               27mar99py

        : maxsize ( -- m )
          hsized parent with w @ hglue drop - endwith ;
        : <size> ( w -- w' )
          hsized oglue over >r + min 0max  maxsize
          hsized hsize @ + min r> max 0max ;
        : size! ( w -- ) <size> dup hsized hsize
          dup @ -rot ! = ?EXIT
          do-size on  hsized resized  do-size off ;
        : drawxorline  hsized hsize @ hsize' @ <>
          IF  hsize' @ size!  THEN ;
        : >released ( x y -- ) swap  hsized hsize @
          relation? IF  -  ELSE  +  THEN
          DOPRESS  drawxorline  drop relation? IF  swap  THEN -
                   <size> hsize' ! drop drawxorline ;

\ hsizer                                               27mar99py
        : keyed ( key sh -- )   drop
          relation? IF  $FF57  ELSE  $FF50  THEN  case?
          IF  maxsize hsized hsize @ +
                                size!  EXIT  THEN
          relation? IF  $FF50  ELSE  $FF57  THEN  case?
          IF  0                 size!  EXIT  THEN
          relation? IF  $FF53  ELSE  $FF51  THEN  case?
          IF  hsized hsize @ hsized yinc nip + size!  EXIT  THEN
          relation? IF  $FF51  ELSE  $FF53  THEN  case?
          IF  hsized hsize @ hsized yinc nip - size!  EXIT  THEN
          drop ;
        : clicked ( x y b n -- )  nip 1 and 0=
          IF  2drop  EXIT  THEN  hsized hsize @ hsize' !
          drawxorline  >released ( cc )
          drawxorline   hsize' @ size! ;        class;

\ vsizer                                               14nov98py

vrtsizer class vsizer
how:    : drawxorline
[defined] x11 [IF]    dpy drawable nip 9 XSetFunction drop [THEN]
          x @ y @ vsized vsize @ vsize' @
          relation? IF  -  ELSE  swap -  THEN  -
          h @ 2/ + w @ 1 color @ 8 >> dpy box
[defined] x11 [IF]    dpy drawable nip 3 XSetFunction drop [THEN] ;
class;

vrtsizer class vxrtsizer
how:    : <size> ( h -- )  vsized oglue + min 0max ;
class;

\ hsizer                                               14nov98py

hrtsizer class hsizer
how:    : drawxorline
[defined] x11 [IF]    dpy drawable nip 9 XSetFunction drop [THEN]
          x @ hsized hsize @ hsize' @
          relation? IF  -  ELSE  swap -  THEN  -
          w @ 2/ + y @ 1 h @ color @ 8 >> dpy box
[defined] x11 [IF]    dpy drawable nip 9 XSetFunction drop [THEN] ;
class;

hrtsizer class hxrtsizer
how:    : <size> ( w -- )  hsized oglue + min 0max ;
class;

