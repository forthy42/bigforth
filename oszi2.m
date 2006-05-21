#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class osci
public:
  early widget
  early open
  early open-app
  button ptr ende
 ( [varstart] ) cell var samples ( [varend] ) 
how:
  : open     new DF[ ende self ]DF s" Saw Tooth" open-component ;
  : open-app new DF[ ende self ]DF s" Saw Tooth" open-application ;
class;

osci implements
 ( [methodstart] ) 600 Constant width
400 Constant height
: ?samples  canvas outer with
  samples @ 0= IF width 1+ cells samples
                  [ also memory ] Handle! [ previous ]
                  width 0 DO  I height width */
                              samples @ I cells + ! LOOP
  THEN  endwith ;
: draw-oszi canvas with  ?samples
  width height steps 0 0 0 rgb> backcolor clear
  0 $FF 0 rgb> drawcolor
  0 height outer with samples @ @ endwith - home!
  path  outer with samples @ cell+ endwith width cells bounds
  DO  0 I @ I cell- @ - to  1 0 to
      (poly# @ $400 >= IF  stroke path  THEN
      cell +LOOP  stroke
  outer with samples @ endwith width 1+ cells bounds
  DO  I @ 1+ height mod I !  cell +LOOP
  ['] draw ^ &40 after dpy schedule
  endwith ; ( [methodend] ) 
  : widget  ( [dumpstart] )
        doublebuffer new  D[ 
          CV[ ^ draw-oszi dpy sync ['] draw ^ &40 after screen schedule ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $258 $1 *hfil $190 $1 *vfil canvas new 
        &1 habox new ]D ( MINOS ) 
        ^^ S[ close ]S ( MINOS ) S" Ende" button new  ^^bind ende
      &2 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  osci open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
