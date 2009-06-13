#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class tree
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
cell var branches
cell var rand
cell var gear-task
cell var shade
3 cells var textures ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Tree" ;
class;

include tree.fs
tree implements
 ( [methodstart] ) : make-gear-task recursive
  1 gear-task !
  glgear render #100 0 DO  pause  LOOP
  glgear draw   #100 0 DO  pause  LOOP ;
: draw-gears
  gear-task @ 0= IF  glgear with init-texture endwith
                     textures !  make-gear-task  EXIT  THEN
  glgear self
  alphax @ alphay @ alphaz @
  alphapitch @ alphabend @ alpharoll @
  zoom @ speed @ branches @ rand @ shade @ 0 max
  textures @ draw-gear ;
: dispose  gear-task @  IF  self dpy cleanup pause gear-task off  THEN
  super dispose ; ( [methodend] ) 
  : widget  ( [dumpstart] )
            GL[ outer with draw-gears endwith ]GL ( MINOS ) ^^ CK[ 2drop 2drop ]CK ( MINOS ) $100 $1 *hfil $100 $1 *vfil glcanvas new  ^^bind GLgear
                    ^^ #0 #360 SC[ #360 mod alphax ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Rotate around X axis" hscaler new 
                    ^^ #0 #360 SC[ #360 mod alphay ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Rotate around Y axis" hscaler new 
                    ^^ #0 #360 SC[ #360 mod alphaz ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Rotate around Z axis" hscaler new 
                  #3 vabox new #2 borderbox
                    ^^ #0 #360 SC[ #360 mod alphapitch ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Pitch" hscaler new 
                    ^^ #330 #360 SC[ #360 mod alphabend ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Bend" hscaler new 
                    ^^ #50 #360 SC[ #360 mod alpharoll ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Roll" hscaler new 
                  #3 vabox new #2 borderbox
                #2 habox new
                    ^^ #6 #8 SC[ 2+ speed ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Height" hscaler new 
                  #1 habox new #2 borderbox
                    ^^ #3 #2 SC[ branches ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Branches" hscaler new  #2 SC# 
                  #1 habox new #2 borderbox
                #2 habox new
              #2 vabox new
                ^^ #100 #100 SC[ zoom ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Zoom" vscaler new 
              #1 habox new #2 borderbox
            #2 habox new #-2 borderbox
            ^^ #558 #1000 SC[ rand ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Random branch drop" hscaler new 
              ^^ TN[ 1 shade ]T[ ( MINOS ) shade @ 0>= IF make-gear-task THEN ]TN ( MINOS ) X" Texture" rbutton new 
              ^^ TN[ 0 shade ]T[ ( MINOS ) shade @ 0>= IF make-gear-task THEN ]TN ( MINOS ) X" Solid" rbutton new 
              ^^ TN[ 2 shade ]T[ ( MINOS ) shade @ 0>= IF make-gear-task THEN ]TN ( MINOS ) X" Lines" rbutton new 
            #3 hartbox new vfixbox 
              ^^ S[ tree open ]S ( MINOS ) X" One more" button new 
              ^^ S[ close ]S ( MINOS ) X" Close" button new 
            #2 hatbox new vfixbox  panel
          #5 vabox new
        #1 vabox new
      #1 vabox new
    ( [dumpend] ) ;
class;

: main
  tree open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
