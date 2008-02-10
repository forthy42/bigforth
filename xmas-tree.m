#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class xmas-tree
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
cell var speed
cell var branches
cell var rand
cell var gear-task
cell var shade ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Xmas-Tree" open-component ;
  : open-app new DF[ 0 ]DF s" Xmas-Tree" open-application ;
class;

include xmas-tree.fs
xmas-tree implements
 ( [methodstart] ) : make-gear-task recursive
  gear-task @ 1 gear-task !
  0= IF
[IFDEF] xft
      screen xrc with
          s" -*-helvetica-bold-r-normal--32-*-*-*-p-*-iso10646-1" 4 font!
      endwith
      s" Christmas" 4 screen xrc font@ 3d-turtle text-texture F bind xmas-text
      s" Merry"     4 screen xrc font@ 3d-turtle text-texture F bind merry-text
[THEN]
  THEN
  glgear render &100 0 DO  pause  LOOP
  glgear draw   &100 0 DO  pause  LOOP ;
: draw-xmas
  gear-task @ 0= IF  make-gear-task  EXIT  THEN
  glgear self
  alphax @ alphay @ alphaz @
  alphapitch @ alphabend @ alpharoll @
  zoom @ 1 umax speed @ branches @ rand @ shade @ 0 max
  draw-xmas-tree ;
: dispose  gear-task @  IF  self dpy cleanup pause gear-task off  THEN
  super dispose ; ( [methodend] ) 
  : widget  ( [dumpstart] )
            GL[ outer with draw-xmas endwith ]GL ( MINOS ) ^^ CK[ 2drop 2drop ]CK ( MINOS ) $100 $1 *hfil $100 $1 *vfil glcanvas new  ^^bind GLgear
                    ^^ &0 &360 SC[ &360 mod alphax ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Rotate around X axis" hscaler new 
                    ^^ &0 &360 SC[ &360 mod alphay ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Rotate around Y axis" hscaler new 
                    ^^ &0 &360 SC[ &360 mod alphaz ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Rotate around Z axis" hscaler new 
                  &3 vabox new &2 borderbox
                    ^^ &0 &360 SC[ &360 mod alphapitch ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Pitch" hscaler new 
                    ^^ &25 &360 SC[ &360 mod alphabend ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Bend" hscaler new 
                    ^^ &0 &360 SC[ &360 mod alpharoll ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Roll" hscaler new 
                  &3 vabox new &2 borderbox
                &2 habox new
                    ^^ &4 &8 SC[ 2+ speed ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Height" hscaler new 
                  &1 habox new &2 borderbox
                    ^^ &4 &3 SC[ branches ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Branches" hscaler new  &2 SC# 
                  &1 habox new &2 borderbox
                &2 habox new
              &2 vabox new
                ^^ &90 &100 SC[ zoom ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Zoom" vscaler new 
              &1 habox new &2 borderbox
            &2 habox new -&2 borderbox
            ^^ &916 &300 SC[ rand ! gear-task @ IF make-gear-task THEN ]SC ( MINOS )  TT" Random branch drop" hscaler new  &700 SC# 
              ^^ TN[ 0 shade ]T[ ( MINOS ) shade @ 0>= IF make-gear-task THEN ]TN ( MINOS ) S" Solid" rbutton new 
              ^^ TN[ 1 shade ]T[ ( MINOS ) shade @ 0>= IF make-gear-task THEN ]TN ( MINOS ) S" Flat" rbutton new 
              ^^ TN[ 2 shade ]T[ ( MINOS ) shade @ 0>= IF make-gear-task THEN ]TN ( MINOS ) S" Lines" rbutton new 
            &3 hartbox new vfixbox 
              ^^ S[ xmas-tree open ]S ( MINOS ) S" One more" button new 
              ^^ S[ close ]S ( MINOS ) S" Close" button new 
            &2 hatbox new vfixbox  panel
          &5 vabox new
        &1 vabox new
      &1 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  xmas-tree open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
