#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

include midi-classes.fs
component class midi
public:
  infotextfield ptr filename
  button ptr play-it
  button ptr stop-it
  button ptr load-it
  button ptr close-it
  canvas ptr channel0
  canvas ptr channel1
  canvas ptr channel2
  canvas ptr channel3
  canvas ptr channel4
  canvas ptr channel5
  canvas ptr channel6
  canvas ptr channel7
  canvas ptr channel8
  canvas ptr channel9
  canvas ptr channelA
  canvas ptr channelB
  canvas ptr channelC
  canvas ptr channelD
  canvas ptr channelE
  canvas ptr channelF
 ( [varstart] ) midi-player ptr player
cell var midi-path
cell var scheduled ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Midi Player" ;
class;

include midi.fs
midi implements
 ( [methodstart] ) : ?player ( -- )
  player self 0= IF
     midi-player new bind player THEN 
  midi-path $@ filename get path+file player file ;
: dispose ( -- )
  midi-path @ IF  midi-path [ also memory ] HandleOff [ previous ] THEN
  super dispose ;
: do-draw
  canvas outer with scheduled @ scheduled on endwith
  0> IF  canvas outer with scheduled off endwith
  ELSE  parent draw dpy sync  THEN ;
: draw-sound ( i canvas -- )
  canvas with
     128 128 steps  0 0 0 rgb> backcolor clear
     7 lshift outer with player self
         IF  player patch-buf  ELSE  drop  0  THEN  endwith
     dup
     IF  127 0 DO  i $80 home!  dup I + c@ dup
                   IF  $00 $FF $80 rgb> drawcolor
                       >r path 0 r> to stroke
                   ELSE  drop  THEN
         LOOP  THEN  drop
     outer with
        scheduled @ 0< IF  ['] do-draw channel0 self
                           #40 after screen schedule  THEN
        scheduled off
     endwith
  endwith pause ; ( [methodend] ) 
  : widget  ( [dumpstart] )
          T" " ^^ ST[  ]ST ( MINOS ) X" Datei:" infotextfield new  ^^bind filename
            ^^ S[ ?player player start scheduled on draw ]S ( MINOS ) X" Play" button new  ^^bind play-it
            ^^ S[ ?player player stop 1 scheduled ! ]S ( MINOS ) X" Stop" button new  ^^bind stop-it
            ^^ S[ 1 scheduled !
s" MIDI" s" " midi-path @
IF  midi-path $@  ELSE  S" *.mid"  THEN
^ S[ 2swap midi-path $! filename assign ?player ]S fsel-action ]S ( MINOS ) X" Load" button new  ^^bind load-it
            ^^ S[ close ]S ( MINOS ) X" Close" button new  ^^bind close-it
            ^^  0 T[ player self IF  player precision on  THEN ][ ( MINOS ) player self IF  player precision off  THEN ]T ( MINOS )  TT" Timing Precission" X" time" tbutton new 
          #5 hatbox new #1 hskips
        #2 vabox new vfixbox  panel
          CV[ 0 ^ draw-sound ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $80 $1 *hfil $8 $1 *vfil canvas new  ^^bind channel0
          CV[ 1 ^ draw-sound ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $80 $1 *hfil $8 $1 *vfil canvas new  ^^bind channel1
          CV[ 2 ^ draw-sound ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $80 $1 *hfil $8 $1 *vfil canvas new  ^^bind channel2
          CV[ 3 ^ draw-sound ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $80 $1 *hfil $8 $1 *vfil canvas new  ^^bind channel3
          CV[ 4 ^ draw-sound ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $80 $1 *hfil $8 $1 *vfil canvas new  ^^bind channel4
          CV[ 5 ^ draw-sound ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $80 $1 *hfil $8 $1 *vfil canvas new  ^^bind channel5
          CV[ 6 ^ draw-sound ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $80 $1 *hfil $8 $1 *vfil canvas new  ^^bind channel6
          CV[ 7 ^ draw-sound ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $80 $1 *hfil $8 $1 *vfil canvas new  ^^bind channel7
          CV[ 8 ^ draw-sound ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $80 $1 *hfil $8 $1 *vfil canvas new  ^^bind channel8
          CV[ 9 ^ draw-sound ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $80 $1 *hfil $8 $1 *vfil canvas new  ^^bind channel9
          CV[ 10 ^ draw-sound ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $80 $1 *hfil $8 $1 *vfil canvas new  ^^bind channelA
          CV[ 11 ^ draw-sound ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $80 $1 *hfil $8 $1 *vfil canvas new  ^^bind channelB
          CV[ 12 ^ draw-sound ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $80 $1 *hfil $8 $1 *vfil canvas new  ^^bind channelC
          CV[ 13 ^ draw-sound ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $80 $1 *hfil $8 $1 *vfil canvas new  ^^bind channelD
          CV[ 14 ^ draw-sound ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $80 $1 *hfil $8 $1 *vfil canvas new  ^^bind channelE
          CV[ 15 ^ draw-sound ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $80 $1 *hfil $8 $1 *vfil canvas new  ^^bind channelF
        #16 vabox new #-2 borderbox
      #2 vabox new
    ( [dumpend] ) ;
class;

: main
  midi open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
