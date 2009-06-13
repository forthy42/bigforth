#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class dragon
public:
  glcanvas ptr GLdragon
  infotextfield ptr fps
 ( [varstart] ) cell var alphax
cell var alphay
cell var alphaz
cell var alphapitch
cell var alphabend
cell var alpharoll
cell var zoom
cell var speed
cell var last-time
cell var tail-time
cell var dragon-task
cell var scale-x
cell var scale-y
cell var scale-z
cell var shade
cell var shade1
cell var texture
cell var back-texture
cell var head-texture
cell var wing-texture
cell var claw-texture ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Swap Dragon" ;
class;

component class dragon-help
public:
  button ptr help-close
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Dragon Help" ;
class;

dragon-help implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          X" Type: Swap-Dragon (twoheaded)" text-label new 
          X" Sex: none" text-label new 
          X" Humor: none" text-label new 
          X" Navel: none, dragons are summoned, not born" text-label new 
          X" Myth: Maiden marry men and become dragons" text-label new 
          X" Maiden: dragon's don't rob them, they are brought to them" text-label new 
          X" Live expectation: dragons can only be killed by heros." text-label new 
          X" Duel: hero meets dragon, dragon dead; hero must marry princess." text-label new 
          X" Otherwise dragons are invulnerable." text-label new 
          X" Typical behaviour: sleeps much, especially on gold and jewels." text-label new 
        #10 vabox new vfixbox 
          $10 $1 *hfill $10 $1 *vfil glue new 
          ^^ S[ close ]S ( MINOS ) X"   Ok  " button new  ^^bind help-close
          $10 $1 *hfill $10 $1 *vfil glue new 
        #3 habox new vfixbox 
      #2 vabox new panel
    ( [dumpend] ) ;
class;

include dragon.fs
dragon implements
 ( [methodstart] ) : make-dragon-task recursive
  1 dragon-task !  !time #20 after >r
  gldragon render #100 0 DO  pause  LOOP
  gldragon draw   #100 0 DO  pause  LOOP
  1000000. timer@ time @ - >us drop ud/mod fps assign drop
  ['] make-dragon-task self r> screen schedule ;
: draw-dragons
  dragon-task @ 0= IF
      foo off
      gldragon with
          5 3d-turtle textures  5 0 DO 4 pick  LOOP
          3d-turtle set-texture
          S" pattern/dragon"       3d-turtle load-texture
          3d-turtle set-texture
          S" pattern/dragon-back"  3d-turtle load-texture
          3d-turtle set-texture
          S" pattern/dragon-head"  3d-turtle load-texture
          3d-turtle set-texture
          S" pattern/dragon-wing"  3d-turtle load-texture
          3d-turtle set-texture
          S" pattern/dragon-claw"  3d-turtle load-texture
      endwith
      texture ! back-texture ! head-texture ! wing-texture ! claw-texture !
      make-dragon-task  THEN
  timer@ dup last-time @ -
  speed @ #20 */ tail-time +! last-time !
  gldragon self
  alphax @ alphay @ alphaz @
  alphapitch @ alphabend @ alpharoll @
  zoom @ tail-time @ shade @ 0 max shade1 @ or
  scale-x @ scale-y @ scale-z @
  texture @ back-texture @ head-texture @ wing-texture @ claw-texture @
  draw-dragon ;
: dispose
  dragon-task @  IF  self dpy cleanup pause dragon-task off  THEN
  super dispose ; ( [methodend] ) 
  : widget  ( [dumpstart] )
        GL[ outer with draw-dragons endwith ]GL ( MINOS ) ^^ CK[ 2drop 2drop ]CK ( MINOS ) $190 $1 *hfil $C8 $1 *vfil glcanvas new  ^^bind GLdragon
              ^^ #0 #360 SC[ #360 mod alphax ! ]SC ( MINOS )  TT" Rotate around X axis" hscaler new 
              ^^ #226 #360 SC[ #360 mod alphay ! ]SC ( MINOS )  TT" Rotate around Y axis" hscaler new 
              ^^ #0 #360 SC[ #360 mod alphaz ! ]SC ( MINOS )  TT" Rotate around Z axis" hscaler new 
            #3 vabox new #2 borderbox
              ^^ #0 #360 SC[ #360 mod alphapitch ! ]SC ( MINOS )  TT" Pitch" hscaler new 
              ^^ #0 #360 SC[ #360 mod alphabend ! ]SC ( MINOS )  TT" Bend" hscaler new 
              ^^ #0 #360 SC[ #360 mod alpharoll ! ]SC ( MINOS )  TT" Roll" hscaler new 
            #3 vabox new #2 borderbox
          #2 habox new
              ^^ #20 #40 SC[ speed ! ]SC ( MINOS )  TT" Speed" hscaler new 
              ^^ #60 #100 SC[ zoom ! ]SC ( MINOS )  TT" Zoom" hscaler new 
              $0 $1 *hfill $0 $1 *vfill rule new 
              #0. ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) X" Frames per second:" infotextfield new  ^^bind fps
              $0 $1 *hfill $0 $1 *vfill rule new 
            #5 vabox new #2 borderbox
              ^^ #100 #100 SC[ scale-x ! ]SC ( MINOS )  TT" Scale X" hscaler new 
              ^^ #100 #100 SC[ scale-y ! ]SC ( MINOS )  TT" Scale Y" hscaler new 
              ^^ #100 #100 SC[ scale-z ! ]SC ( MINOS )  TT" Scale Z" hscaler new 
            #3 vabox new #2 borderbox
          #2 hatbox new
              ^^ TN[ 0 shade ]T[ ( MINOS )  ]TN ( MINOS ) X" Text" rbutton new 
              ^^ TN[ 1 shade ]T[ ( MINOS )  ]TN ( MINOS ) X" Solid" rbutton new 
              ^^ TN[ 4 shade ]T[ ( MINOS )  ]TN ( MINOS ) X" Flat" rbutton new 
              ^^ TN[ 2 shade ]T[ ( MINOS )  ]TN ( MINOS ) X" Lines" rbutton new 
              ^^ TN[ 3 shade ]T[ ( MINOS )  ]TN ( MINOS ) X" Points" rbutton new 
              ^^ TN[ 5 shade ]T[ ( MINOS )  ]TN ( MINOS ) X" PolyT" rbutton new 
              ^^ TN[ 6 shade ]T[ ( MINOS )  ]TN ( MINOS ) X" Poly" rbutton new 
            #7 hartbox new #2 borderbox
              ^^  0 T[ shade1 @ 8 or shade1 ! ][ ( MINOS ) shade1 @ -9 and shade1 ! ]T ( MINOS ) X" List" tbutton new 
            #1 habox new #2 borderbox
          #2 habox new vfixbox 
              ^^ #5000 #5000 SC[ maxpoints ! ]SC ( MINOS )  TT" Maximal points to display" hscaler new 
              $0 $2 *hfil $0 $0 *vfil rule new 
            #2 vabox new vfixbox 
              ^^ #0 #10 SC[ $10 * shade1 @ $FFFFFF0F and or shade1 ! ]SC ( MINOS )  TT" Fog" hscaler new 
              $40 $1 *hfil $0 $0 *vfil rule new 
            #2 vabox new hfixbox  vfixbox 
              ^^ #12 #40 SC[ $100 * shade1 @ $FF and or shade1 ! ]SC ( MINOS )  TT" Steps" hscaler new  #12 SC# 
              $40 $0 *hfil $0 $0 *vfil rule new 
            #2 vabox new vfixbox 
          #3 habox new vfixbox  #2 borderbox
            ^^ S[ dragon open ]S ( MINOS ) X" One more" button new 
            ^^ S[ close ]S ( MINOS ) X" Close" button new 
            ^^ S[ dragon-help open ]S ( MINOS ) X" Help" button new 
          #3 hatbox new vfixbox  panel #-2 borderbox
        #5 vabox new vfixbox 
      #2 vabox new
    ( [dumpend] ) ;
class;

: main
  dragon open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
