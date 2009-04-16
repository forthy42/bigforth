\ backing                                              15aug99py

displays class backing
public: gadget ptr child        method create-pixmap
        cell var noback         cell var closing
        2 cells var hglues      2 cells var vglues
[defined] win32 [IF]                   cell var oldbm      [THEN]

\ backing                                              28aug99py

how:    : init ( -- ) ;
        : schedule ( xt o time -- )  dpy schedule ;
        : invoke ( -- time )  dpy invoke ;
        : cleanup ( o -- )  dpy cleanup ;
        : sync ( -- )   dpy sync ;
        : get-event ( mask -- )  dpy get-event ;
        : !txy ( -- ) tx @ ty @ trans' dpy txy! ;
        : !t00 ( -- ) 0 0 dpy txy! ;
        gadget :: delete

\ backing                                              28mar99py

        : get-glues ( -- )
          child hglue hglues 2!
          child vglue vglues 2! ;
        : dpy! ( dpy -- )
          bind dpy  xrc self IF  xrc dispose  THEN
          dpy xrc clone bind xrc  create-pixmap
[defined] x11 [IF]     get-win xrc get-gc  [THEN]
          0 clip-rect  flags #draw +bit  child self 0= ?EXIT
          self child dpy!  !resized
          child xywh  resize ;

\ backing store                                        11nov06py
[defined] x11 [IF]
        : free-pixmap ( -- )
          xwin @  IF  xrc dpy @ xwin @ XFreePixmap xwin off  THEN ;
        : create-pixmap ( -- )  free-pixmap
          xpict @ IF  screen xrc dpy @ xpict @
                      XRenderFreePicture xpict off  THEN
          noback @ ?EXIT   xrc dpy @
	  dpy get-win  w @ 1 max  h @ 1 max  xrc depth @
          XCreatePixmap xwin ! ;
        : dispose ( -- )  child self  IF  child dispose  THEN
          free-pixmap super dispose ;
        : ?xpict ( -- )  xpict @ ?EXIT  xrc dpy @ xwin @
          over PictStandardRGB24 XRenderFindStandardFormat
          $800 pict_attrib XRenderCreatePicture xpict ! ;
[THEN]

\ backing store                                        11nov06py

[defined] win32 [IF]
        : create-pixmap ( -- )
          xwin @  IF  xwin @ DeleteObject drop xwin off  THEN
          noback @ ?EXIT
          h @ 1 max w @ 1 max
          screen xrc dc @ CreateCompatibleBitmap xwin !
          screen xrc dc @ CreateCompatibleDC xrc dc !
          xwin @ xrc dc @ SelectObject oldbm !  xrc init-dc ;
        : dispose ( -- )  child dispose
          xwin @  IF  xwin @ DeleteObject ?err
                      xrc dc @ DeleteDC ?err  THEN
          super dispose ;
[THEN]

\ backing store                                        06jul02py

        : child-dispose  child self IF child dispose THEN ;
        : set-child ( widget -- )  0 bind pointed
[defined] .except [IF]
          ['] child-dispose catch IF .except THEN
[ELSE]    child-dispose  [THEN]
          bind child  self child bind parent
          dpy self 0= IF  rdrop  THEN ;
        : rest-child ( -- )  get-glues  child xywh  resize ;

\ backing store                                        17dec00py
        : assign ( widget -- )  set-child
          self child dpy!  rest-child ;
        : resized
          get-glues child xywh resize parent resized ;
        : trans ( x y -- x' y' )  x @ y @ p- ;
        : trans' ( x y -- x' y' )  x @ y @ p+ ;
        : screenpos  dpy screenpos x @ y @ p+ ;
        : next-active   child next-active ;
        : prev-active   child prev-active ;
        : first-active  child first-active ;
        : clicked ( click -- ) 2swap trans 2swap child clicked ;
        : click? ( -- flag )  dpy click?  ;
        : click ( -- x y b n )  dpy click 2swap trans 2swap ;

\ backing store                                        11nov06py
        : resize ( x y w h -- )   w @ h @ >r >r
          2swap 2over super resize 0 0 2swap child resize
          r> r> w @ h @ d= 0=  xwin @ 0= noback @ 0= and or
	  IF  create-pixmap flags #draw 2dup bit@ >r -bit
	      child draw r> IF flags #draw +bit THEN THEN ;
        : draw ( -- )  flags #hidden bit@ ?EXIT
	  xwin @ noback @ 0= and redraw-all @ 0= and
          IF    0 0 w @ h @ x @ y @
                [defined] win32 [IF]  xrc dc @ dpy image
                [ELSE]  xpict @  IF  -1 xpict @ dpy mask
                        ELSE  xwin @ dpy image  THEN  [THEN]
          ELSE  child draw  THEN ;
        : moved? ( -- flag )  dpy moved?  ;
        : moved! ( -- )  dpy moved!  ;
        : mouse ( -- x y b ) dpy mouse >r trans r>  ;
        : keyed ( key -- )  child keyed  ;
        : handle-key?  child handle-key? ;

\ backing store                                        20oct99py
        : line ( x y x y color -- )  flags #draw bit@
          IF  !txy >r 2over trans' 2over trans' r@
              dpy line r> !t00  THEN
          xwin @ 0= IF  drop 2drop 2drop  EXIT  THEN
          super line ;
        : text ( addr u x y color -- )  flags #draw bit@
          IF  !txy >r  2over 2over trans' r@ dpy text
              r> !t00 THEN
          xwin @ 0= IF  drop 2drop 2drop  EXIT  THEN
          super text ;
        : box ( x y w h color -- )  flags #draw bit@ IF
          !txy >r 2over trans' 2over r@ dpy box r> !t00 THEN
          xwin @ 0= IF  drop 2drop 2drop  EXIT  THEN
          super box ;

\ backing store                                        01mar98py
        : image ( x y w h x y win -- )  flags #draw bit@
          IF  6 pick 6 pick 6 pick 6 pick 6 pick 6 pick trans' 6 pick
              dpy image  THEN
          xwin @ 0= IF  drop 2drop 2drop 2drop  EXIT  THEN
          super image ;
\        : ximage ( x y w h x y win -- )  flags #draw bit@
\          IF  [ 5 ] [FOR] 6 pick [NEXT] trans' 6 pick
\              dpy ximage  THEN
\          xwin @ 0= IF  drop 2drop 2drop 2drop  EXIT  THEN
\          super ximage ;
        : mask ( x y w h x y win1 win2 -- )  flags #draw bit@
          IF  7 pick 7 pick 7 pick 7 pick 7 pick 7 pick trans' 7 pick 7 pick
              dpy mask  THEN
          xwin @ 0= IF  2drop 2drop 2drop 2drop  EXIT  THEN
          super mask ;

\ backing store                                        11nov06py

        : fill ( x y addr n color -- )  flags #draw bit@ IF
          !txy >r 2over trans' 2over r@ dpy fill r> !t00 THEN
          xwin @ 0= IF  drop 2drop 2drop  EXIT  THEN
          super fill ;
        : stroke ( x y addr n color -- )  flags #draw bit@ IF
          !txy >r 2over trans' 2over r@ dpy stroke r> !t00 THEN
          xwin @ 0= IF  drop 2drop 2drop  EXIT  THEN
          super stroke ;

        : drawer ( x y o xt -- )  flags #draw bit@
          IF   !txy 2over trans' 2over dpy drawer !t00  THEN
          xwin @ 0= IF  2drop 2drop  EXIT  THEN  super drawer ;
        : set-linewidth  dup dpy set-linewidth
          super set-linewidth ;

\ backing store                                        20oct99py
        : moved ( x y -- )  ^ dpy set-rect trans pointed self
          IF  2dup pointed xywh >r >r
              p- r> r> rot swap u< -rot u< and
              IF  backing self pointed class?
                  IF  pointed moved  ELSE  2drop THEN  EXIT THEN
              pointed leave  0 bind pointed  THEN
          child moved ;
        : leave  pointed self
          IF  pointed leave 0 bind pointed  THEN ;
        : set-cursor ( n -- )  dpy set-cursor ;
        : set-font ( font -- ) dup dpy set-font super set-font ;
        : show  flags #hidden -bit  child show ;
        : hide  flags #hidden +bit  child hide ;
        : focus    child focus     ;
        : defocus  child defocus   ;

\ backing store                                        15aug99py
        gadget :: append
        : xinc   child xinc ;
        : yinc   child yinc ;
        : txy! ( x y -- )  ty ! tx ! ;
        : get-dpy  dpy get-dpy ;
        : show-you  child show-you ;
        : hglue  ( -- g )  hglues 2@ ;
        : vglue  ( -- g )  vglues 2@ ;
        : get-win xwin @ ?dup 0= IF  dpy get-win  THEN ;
        : !resized ( -- ) xrc !font
          0 set-font  child !resized  get-glues ;
        : transback  xwin @ 0= IF trans' dpy transback  THEN ;
        : close  closing push closing @ closing on
          IF  dpy close  ELSE  child close  THEN ;
        : set-hints  dpy set-hints ;            class;

\ doublebuffer                                         28mar99py
backing class doublebuffer
how:    displays :: line ( x y x y color -- )
        displays :: text ( addr u x y color -- )
        displays :: box ( x y w h color -- )
        displays :: image ( x y w h x y win -- )
\        displays :: ximage ( x y w h x y win -- )
        displays :: mask ( x y w h x y win1 win2 -- )
        displays :: fill ( x y addr n color -- )
        displays :: stroke ( x y addr n color -- )
        displays :: drawer ( x y o xt -- )
        : sync  draw dpy sync ;
        : keyed  super keyed draw ;
        : clicked  super clicked draw ;
        : resize  super resize
          flags #draw bit@ IF  child draw draw  THEN ;   class;

\ pixmap                                               28oct06py

doublebuffer class pixmap
public: method map@
how:    : init ( depth w h dpy -- )
          screen self dpy! xrc clone bind xrc ;
        : draw ( -- ) ;
[defined] x11 [IF]
        : create-pixmap ( depth w h -- ) over2 xrc depth !
          2dup h ! w ! free-pixmap
          2>r >r xrc dpy @ dpy get-win r> 2r> rot
          XCreatePixmap xwin ! ;
        : get ( -- addr w h )
          xrc dpy @ xwin @ 0 0 w @ h @ -1 ZPixmap XGetImage w @ h @ ;
[THEN]

\ pixmap                                               28oct06py

[defined] win32 [IF]
        : create-pixmap ( depth w h -- )  h ! w ! drop
          super create-pixmap ;
\ !!!FIXME!!! This doesn't work!
        : get ( -- addr w h )  pad w @ h @ ;
[THEN]

class;

\ beamer                                               24jan98py
backing class beamer            cell var resize!
public: ptr nextb               ptr firstb
        cell var enable         method clone
        early all-on            early all-off
        early resize-all        \ early all-wh
        early delete-me         early set-first
how:    : drops  cells sp@ + cell+ sp! ;
        : line ( x y x y color -- ) >r
          enable @ IF  2over 2over r@ super line  THEN
          nextb self IF  r> nextb goto line
          ELSE  rdrop 2drop 2drop  THEN ;
        : text ( addr u x y color -- ) >r
          enable @ IF  2over 2over r@ super text  THEN
          nextb self IF  r> nextb goto text
          ELSE  rdrop 2drop 2drop  THEN ;

\ beamer                                               20oct99py

        : box ( x y w h color -- )  >r
          enable @  IF  2over 2over r@ super box  THEN
          nextb self IF  r> nextb goto box
          ELSE  rdrop 2drop 2drop  THEN ;
        : image ( x y w h x y win -- )
          enable @ IF  6 pick 6 pick 6 pick 6 pick 6 pick 6 pick 6 pick super image THEN
          nextb self IF nextb goto image  ELSE  7 drops  THEN ;
\        : ximage ( x y w h x y win -- )
\        enable @ IF [ 6 ] [FOR] 6 pick [NEXT] super ximage THEN
\         nextb self IF nextb goto ximage  ELSE  7 drops  THEN ;
        : mask ( x y w h x y win1 win2 -- )
          enable @ IF 7 pick 7 pick 7 pick 7 pick 7 pick 7 pick 7 pick 7 pick super mask  THEN
          nextb self IF nextb goto mask  ELSE   8 drops  THEN ;

\ beamer                                               11nov06py
        : fill ( x y addr n color -- ) >r
          enable @  IF  2over 2over r@ super fill  THEN
          nextb self IF  r> nextb goto fill
          ELSE  rdrop 2drop 2drop  THEN ;
        : stroke ( x y addr n color -- ) >r
          enable @  IF  2over 2over r@ super stroke  THEN
          nextb self IF  r> nextb goto stroke
          ELSE  rdrop 2drop 2drop  THEN ;
        : drawer ( x y o xt -- )
          enable @  IF 2over 2over super drawer THEN
          nextb self IF nextb goto drawer
          ELSE  2drop 2drop  THEN ;
        : set-linewidth ( n -- )
          enable @  IF dup super set-linewidth THEN nextb self
          IF nextb goto set-linewidth ELSE drop THEN ;

\ beamer                                               28mar99py

        : init ( first next -- )  noback on
          super init  bind nextb  bind firstb
          firstb self 0= IF  ^ bind firstb  THEN
          enable on ;
        : clone ( -- beamer )
          firstb self nextb self ( rot ) new bind nextb
          child self IF  child self nextb assign  THEN
          nextb self ;
        : all-on  BEGIN  enable on
          nextb self WHILE  nextb self op!  REPEAT ;
        : all-off  BEGIN  enable off
          nextb self WHILE  nextb self op!  REPEAT ;

\ beamer                                               28mar99py

        : first?  ^ firstb self = ;
        : hglue  first? IF  super hglue
          ELSE  firstb w @ 0  THEN ;
        : vglue  first? IF  super vglue
          ELSE  firstb h @ 0  THEN ;
        : resize-all ( -- )
          BEGIN  xywh 2drop  firstb xywh 2swap 2drop  super resize
	      parent resized
	      nextb self  WHILE  nextb self op!  REPEAT ;
        : resize ( x y w h -- )
          first? IF  super resize resize! on
          ELSE  gadget :: resize THEN ;
        : draw  redraw-all dup push off
	  super draw  resize! @ 0= ?EXIT  resize! off
          nextb self IF  nextb resize-all  THEN ;

\ beamer                                               28mar99py

        : delete-me ( beam -- )
	  BEGIN  dup nextb self =
	      0= WHILE  nextb self  WHILE  nextb self op!  REPEAT
	  ELSE  nextb nextb self bind nextb  THEN ;
        : set-first ( beam -- )
	  BEGIN  dup bind firstb
	      nextb self WHILE  nextb self op!  REPEAT ;
        : dispose  first? nextb self and
          IF  nextb self nextb set-first drop  THEN
          self firstb delete-me drop
          first? 0= nextb self or IF  0 bind child  THEN
          super dispose ;

\ beamer                                               17dec00py

        : dpy! ( dpy -- )  bind dpy
          xrc self IF  xrc dispose  THEN
          dpy xrc clone bind xrc  create-pixmap
[defined] x11 [IF]     get-win xrc get-gc  [THEN]
          0 clip-rect  flags #draw +bit
          first?  IF  self child dpy!  THEN
          !resized  child xywh  resize ;
        : assign ( widget -- )  set-child
          first?  IF  self child dpy!  THEN
	  rest-child ;
        : hide  flags #hidden +bit ;
        : close  dpy close ;
class;
: :beamer  0 0 ;

