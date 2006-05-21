#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class gears-text
public:
  early widget
  early open
  early open-app
  glcanvas ptr GLgear
 ( [varstart] ) cell var alphax
cell var alphay
cell var alphaz
cell var alphapitch
cell var alphabend
cell var alpharoll
cell var zoom
cell var texture
cell var gear0
cell var gear1
cell var gear2
cell var gear-task ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Gears" open-component ;
  : open-app new DF[ 0 ]DF s" Gears" open-application ;
class;

include gears-text.fs
gears-text implements
 ( [methodstart] ) : make-gear-task recursive
  1 gear-task !  &60 after >r
  glgear render &100 0 DO  pause  LOOP
  glgear draw   &100 0 DO  pause  LOOP
  ['] make-gear-task self r> screen schedule ;
: draw-gears  recursive
  gear0 @ 0= IF  create-gears gear2 ! gear1 ! gear0 !  THEN
  gear-task @ 0= IF  make-gear-task  THEN
  glgear self
  gear0 @ gear1 @ gear2 @
  alphax @ alphay @ alphaz @
  alphapitch @ alphabend @ alpharoll @
  zoom @ texture @ draw-gear ;
: dispose
  gear-task @  IF  self dpy cleanup pause gear-task off  THEN
  super dispose ; ( [methodend] ) 
  : widget  ( [dumpstart] )
        GL[ outer with draw-gears endwith ]GL ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $100 $1 *hfil $100 $1 *vfil glcanvas new  ^^bind GLgear
          ^^ &325 &360 SC[ &360 mod alphax ! ]SC ( MINOS )  TT" Rotate around X axis" hscaler new 
          ^^ &45 &360 SC[ &360 mod alphay ! ]SC ( MINOS )  TT" Rotate around Y axis" hscaler new 
          ^^ &43 &360 SC[ &360 mod alphaz ! ]SC ( MINOS )  TT" Rotate around Z axis" hscaler new 
          ^^ &0 &360 SC[ &360 mod alphapitch ! ]SC ( MINOS )  TT" Pitch" hscaler new 
          ^^ &0 &360 SC[ &360 mod alphabend ! ]SC ( MINOS )  TT" Bend" hscaler new 
          ^^ &0 &360 SC[ &360 mod alpharoll ! ]SC ( MINOS )  TT" Roll" hscaler new 
          ^^ &0 &100 SC[ zoom ! ]SC ( MINOS )  TT" Zoom" hscaler new 
          ^^ TV[ texture ]T[ ( MINOS )  ]TV ( MINOS )  TT" Switches texture on/off" S" Texture" tbutton new 
            ^^ S[ gears-text open ]S ( MINOS ) S" One more" button new 
            ^^ S[ close ]S ( MINOS ) S" Close" button new 
          &2 hatbox new panel
        &9 vabox new vfixbox 
      &2 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  gears-text open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
