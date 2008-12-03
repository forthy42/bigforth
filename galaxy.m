#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class galaxy
public:
  glcanvas ptr GLgalaxy
  canvas ptr mass#
  canvas ptr a#
  hscaler ptr bulge#
  hscaler ptr spread#
  hscaler ptr spiral#
  info-menu ptr meas-string
  textfield ptr iterations
  button ptr iterate
  button ptr iterates
  button ptr iterates
 ( [varstart] ) cell var alphax
cell var alphay
cell var alphaz
cell var alphapitch
cell var alphabend
cell var alpharoll
cell var zoom
cell var disc#
cell var startex
cell var bulgep
cell var spreadp
cell var spiralp
cell var star-path
cell var filename
cell var do-mass
cell var don't
3 cells var galaxy-lock
method redraw-galaxy ( [varend] ) 
how:
  : params   DF[ iterate self ]DF s" Galaxy" ;
class;

component class about
public:
  button ptr ok-button
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ ok-button self ]DF s" About" ;
class;

component class measure-menu
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Measure" ;
class;

measure-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[ ^ galaxy with
    0 disc# ! redraw-galaxy s" None" meas-string assign
endwith ]S ( MINOS ) X" None" menu-entry new 
        ^^ S[ ^ galaxy with
    1 disc# ! redraw-galaxy s" New/Old" meas-string assign
endwith ]S ( MINOS ) X" New/Old" menu-entry new 
        ^^ S[ ^ galaxy with
    2 disc# ! redraw-galaxy s" Old" meas-string assign
endwith ]S ( MINOS ) X" Old" menu-entry new 
        ^^ S[ ^ galaxy with
    3 disc# ! redraw-galaxy s" New" meas-string assign
endwith ]S ( MINOS ) X" New" menu-entry new 
      #4 vabox new #2 borderbox
    ( [dumpend] ) ;
class;

about implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          X" This program simulates 'dark masses' in a galaxy." text-label new 
          X" However, it doesn't assume real dark masses, but" text-label new 
          X" a generalization of Mach's principle." text-label new 
          X" The assumed formula for an inertial system is: sum(a*m/r²)=0." text-label new 
          X" Since the background mass is small compared to the galaxie's" text-label new 
          X" mass, most of the additional accelleration comes from the galaxy" text-label new 
          X" itself." text-label new 
            $10 $1 *hfil $10 $1 *vfil glue new 
             icon" icons/starsln" icon new 
            $10 $1 *hfil $10 $1 *vfil glue new 
          #3 habox new #1 vskips
        #8 vabox new
          $10 $1 *hfill $10 $1 *vfil rule new 
          ^^ S[ close ]S ( MINOS ) X" OK" button new  ^^bind ok-button
          $10 $1 *hfill $10 $1 *vfil glue new 
        #3 habox new
      #2 vabox new panel
    ( [dumpend] ) ;
class;

include galaxy.fs
galaxy implements
 ( [methodstart] ) : redraw-galaxy  GLgalaxy render  GLgalaxy draw ;
: make-galaxy  don't @ ?EXIT
  bulgep @ spreadp @ spiralp @ set-masses glgalaxy draw
  ( mass# draw a# draw ) redraw-galaxy 1 do-mass ! ;
: draw-galaxy
  startex @ 0= IF
      1 3d-turtle textures dup startex ! 
      3d-turtle set-texture
      S" icons/starsln" 3d-turtle load-texture
  THEN
  GLgalaxy self
  alphax @ alphay @ alphaz @
  alphapitch @ alphabend @ alpharoll @ zoom @ disc# @
  galaxy-draw ;
: iterate-step  ^ 1 $10000 dup NewTask pass op!
  galaxy-lock lock
  do-mass @ 1 = IF  set-msum mass# draw set-a a# draw  THEN
  do-mass off set-a+ a# draw redraw-galaxy dpy sync
  galaxy-lock unlock ;
: iterate-disc  ^ 1 $10000 dup NewTask pass op!
  galaxy-lock lock
  disc-msum disc-a disc-a+ redraw-galaxy dpy sync
  galaxy-lock unlock ; ( [methodend] ) 
  : widget  ( [dumpstart] )
              GL[ outer with draw-galaxy endwith ]GL ( MINOS ) ^^ CK[ 2drop 2drop ]CK ( MINOS ) $200 $1 *hfil $200 $1 *vfil glcanvas new  ^^bind GLgalaxy
            #1 habox new
                    CV[ clear visualize-mass ]CV ( MINOS ) ^^ CK[ 2drop 2drop ]CK ( MINOS ) $20 $1 *hfil $80 $1 *vfil canvas new  ^^bind mass#
                    ^^ #0 #360 SC[ &360 mod alphax ! redraw-galaxy ]SC ( MINOS )  TT" Rotate around X axis" hscaler new  #-180 SC# 
                    ^^ #0 #360 SC[ &360 mod alphay ! redraw-galaxy ]SC ( MINOS )  TT" Rotate around Y axis" hscaler new  #-180 SC# 
                    ^^ #0 #360 SC[ &360 mod alphaz ! redraw-galaxy ]SC ( MINOS )  TT" Rotate around Z axis" hscaler new  #-180 SC# 
                  #4 vabox new vfixbox  #2 borderbox
                    CV[ a-pos @ >r
visualize-a  r@ vis@ 
visualize-a+ r@ vis@ swap
dup 0= IF  nip  ELSE  &1000 swap */  THEN decimal
0 <# # # # '. hold #s #>
r@ 1+ 0 home! r> 2* vismax > IF  2 ELSE  0  THEN  0 textpos
text  path 0 vismax -$100 * to stroke ]CV ( MINOS ) ^^ CK[ nip 1 and IF  DOPRESS  2swap 2drop  THEN
drop
a# xywh drop >r drop - r> vismax swap */
0 max vismax 1- min a-pos !
a# draw ]CK ( MINOS ) $20 $1 *hfil $80 $1 *vfil canvas new  ^^bind a#
                    ^^ #0 #360 SC[ &360 mod alphapitch ! redraw-galaxy ]SC ( MINOS )  TT" Pitch" hscaler new  #-180 SC# 
                    ^^ #0 #360 SC[ &360 mod alphabend ! redraw-galaxy ]SC ( MINOS )  TT" Bend" hscaler new  #-180 SC# 
                    ^^ #0 #360 SC[ &360 mod alpharoll ! redraw-galaxy ]SC ( MINOS )  TT" Roll" hscaler new  #-180 SC# 
                  #4 vabox new vfixbox  #2 borderbox
                    ^^ #1000 #1000 SC[ ( pos -- ) zoom ! redraw-galaxy ]SC ( MINOS )  TT" Zoom factor in %" vscaler new 
                    ^^ #5 #10 SC[ ( pos -- ) !.1 fm* f>fs .black2 3 cells + !
redraw-galaxy ]SC ( MINOS )  TT" Background intensity" hscaler new 
                  #2 vabox new #2 borderbox
                #3 habox new
                      ^^ #60 #500 SC[ !.01 fm* !&3.8 f* f**2 1/f msum+ f! ]SC ( MINOS )  TT" Background radius relative to galaxy radius in percent" hscaler new 
                      ^^ #33 #100 SC[ bulgep ! make-galaxy ]SC ( MINOS )  TT" Bulge in percent" hscaler new  ^^bind bulge#
                    #2 vabox new
                      ^^ #25 #100 SC[ spreadp ! make-galaxy ]SC ( MINOS )  TT" Spread of the arms" hscaler new  ^^bind spread#
                      ^^ #100 #500 SC[ spiralp ! make-galaxy ]SC ( MINOS )  TT" Spiral factor" hscaler new  ^^bind spiral#
                    #2 vabox new
                  #2 habox new
                        ^^ -1 T[ spiral-dist on make-galaxy ][ ( MINOS ) spiral-dist off make-galaxy ]T ( MINOS ) X" dist" tbutton new 
                        ^^ -1 T[ dirsens on a# draw redraw-galaxy ][ ( MINOS ) dirsens off a# draw redraw-galaxy ]T ( MINOS )  TT" On: Show a only in direction to center" X" ->r" tbutton new 
                        M: measure-menu menu X" " info-menu new  ^^bind meas-string
                      #3 habox new hfixbox  vfixbox  #2 borderbox
                      $0 $1 *hfil $0 $1 *vfil glue new 
                        ^^ #3 #7 SC[ $100 swap << to star# make-galaxy ]SC ( MINOS )  TT" 2^(9+n) stars" hscaler new 
                          $0 $1 *hfil $0 $1 *vfil glue new 
                          #10, ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) textfield new  ^^bind iterations
                          $0 $1 *hfil $0 $1 *vfil glue new 
                        #3 vabox new hfixbox  #1 hskips
                      #2 habox new vfixbox 
                    #3 vabox new hfixbox 
                  #1 vabox new hfixbox  #2 vskips
                #2 habox new #2 borderbox
              #2 vabox new vfixbox 
            #1 habox new #-2 borderbox
                ^^ S[ s" Load Stars" s" " star-path @
IF  star-path $@  ELSE  s" *.star"  THEN
^ S[ 2swap star-path $! filename $!
     star-path $@ filename $@ path+file r/o open-file throw >r
     & star# cell r@ read-file throw drop init-stars
     bulgep 2 cells r@ read-file throw drop  don't on
     bulgep @ 0 bulge# assign
     spreadp @ 0 spread# assign  don't off
     stars $@ r@ read-file throw drop r> close-file throw
     a# draw redraw-galaxy ]S
fsel-action ]S ( MINOS )  icon" icons/load" icon-but new 
                ^^ S[ s" Save Stars" s" " star-path @
IF  star-path $@  ELSE  s" *.star"  THEN
^ S[ 2swap star-path $! filename $!
     star-path $@ filename $@ path+file r/w create-file throw
     >r & star# cell r@ write-file throw
     bulgep 2 cells r@ write-file throw
     stars $@ r@ write-file throw r> close-file throw ]S
fsel-action ]S ( MINOS )  icon" icons/save" icon-but new 
              #2 hatbox new vfixbox 
              ^^ S[ iterate-step ]S ( MINOS ) X" Run" button new  ^^bind iterate
              ^^ S[ iterations get drop 0 DO iterate-step LOOP ]S ( MINOS ) X" Runs" button new  ^^bind iterates
              ^^ S[ iterate-disc ]S ( MINOS ) X" Disc" button new  ^^bind iterates
              ^^ S[ close ]S ( MINOS ) X" Close" button new 
              ^^ S[ about open ]S ( MINOS ) X" About" button new 
            #6 hatbox new vfixbox  panel #-2 borderbox
          #3 vabox new
        #1 vabox new
      #1 vabox new
    ( [dumpend] ) ;
class;

: main
  galaxy open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
