#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class gears
public:
  glcanvas ptr GLgear
 ( [varstart] ) cell var alphax
cell var alphay
cell var alphaz
cell var alphapitch
cell var alphabend
cell var alpharoll
cell var zoom
cell var speed
cell var last-time
cell var gear-time
cell var gear-task
cell var shade
cell var texting
cell var scale-x
cell var scale-y
cell var scale-z
3 cells var textures ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" Gears" ;
class;

include gears.fs
gears implements
 ( [methodstart] ) : make-gear-task recursive
  1 gear-task !  #1000 speed @ 1 max / #100 min after >r
  glgear render #100 0 DO  pause  LOOP
  glgear draw   #100 0 DO  pause  LOOP
  ['] make-gear-task self r> screen schedule ;
: init-texture ( -- t1 t2 t3 )
    glgear with
        5e 60e w @ h @ 3d-turtle new  3d-turtle with
            3 textures dup 2over swap
            set-texture S" pattern/normal-w1" load-texture
            set-texture S" pattern/back"      load-texture
            set-texture S" pattern/focus"     load-texture
        dispose endwith
    endwith ;
: draw-gears
  gear-task @ 0= IF  init-texture  textures !+ !+ !  make-gear-task  THEN
  timer@ dup last-time @ -
  speed @ #20 */ gear-time +! last-time !
  glgear self
  alphax @ alphay @ alphaz @
  alphapitch @ alphabend @ alpharoll @
  zoom @ gear-time @ shade @ 0 max texting @ 0 max
  scale-x @ scale-y @ scale-z @
  textures @+ @+ @ draw-gear ;
: dispose  gear-task @  IF  self dpy cleanup pause gear-task off  THEN
  super dispose ; ( [methodend] ) 
  : widget  ( [dumpstart] )
            GL[ outer with draw-gears endwith ]GL ( MINOS ) ^^ S[ 2drop 2drop ]S ( MINOS ) $100 $1 *hfil $100 $1 *vfil glcanvas new  ^^bind GLgear
            ^^ #0 #360 SC[ #360 mod alphax ! ]SC ( MINOS )  TT" Rotate around X axis" hscaler new 
            ^^ #0 #360 SC[ #360 mod alphay ! ]SC ( MINOS )  TT" Rotate around Y axis" hscaler new 
            ^^ #0 #360 SC[ #360 mod alphaz ! ]SC ( MINOS )  TT" Rotate around Z axis" hscaler new 
            ^^ #35 #360 SC[ #360 mod alphapitch ! ]SC ( MINOS )  TT" Pitch" hscaler new 
            ^^ #331 #360 SC[ #360 mod alphabend ! ]SC ( MINOS )  TT" Bend" hscaler new 
            ^^ #350 #360 SC[ #360 mod alpharoll ! ]SC ( MINOS )  TT" Roll" hscaler new 
            ^^ #0 #100 SC[ zoom ! ]SC ( MINOS )  TT" Zoom" hscaler new 
            ^^ #20 #40 SC[ speed ! ]SC ( MINOS )  TT" Speed" hscaler new 
            ^^ #100 #100 SC[ scale-x ! ]SC ( MINOS )  TT" Scale X" hscaler new 
            ^^ #100 #100 SC[ scale-y ! ]SC ( MINOS )  TT" Scale Y" hscaler new 
            ^^ #100 #100 SC[ scale-z ! ]SC ( MINOS )  TT" Scale Z" hscaler new 
              ^^ TN[ 1 shade ]T[ ( MINOS )  ]TN ( MINOS ) X" Texture" rbutton new 
              ^^ TN[ 0 shade ]T[ ( MINOS )  ]TN ( MINOS ) X" Solid" rbutton new 
              ^^ TN[ 2 shade ]T[ ( MINOS )  ]TN ( MINOS ) X" Lines" rbutton new 
            #3 hartbox new vfixbox 
              ^^ TN[ 0 texting ]T[ ( MINOS )  ]TN ( MINOS ) X" xy" rbutton new 
              ^^ TN[ 1 texting ]T[ ( MINOS )  ]TN ( MINOS ) X" rphi" rbutton new 
              ^^ TN[ 2 texting ]T[ ( MINOS )  ]TN ( MINOS ) X" zphi" rbutton new 
            #3 hartbox new vfixbox 
              ^^ S[ gears open ]S ( MINOS ) X" One more" button new 
              ^^ S[ close ]S ( MINOS ) X" Close" button new 
            #2 hatbox new vfixbox  panel
          #15 vabox new
        #1 vabox new
      #1 vabox new
    ( [dumpend] ) ;
class;

: main
  gears open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
