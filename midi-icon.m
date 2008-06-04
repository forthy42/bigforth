#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

include midi-classes.fs
component class midi
public:
  early widget
  early open
  early dialog
  early open-app
  infotextfield ptr filename
  icon-but ptr play-it
  icon-but ptr stop-it
  icon-but ptr load-it
  icon-but ptr close-it
 ( [varstart] ) midi-player ptr player
cell var midi-path ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Midi Player" open-component ;
  : dialog   new DF[ 0 ]DF s" Midi Player" open-dialog ;
  : open-app new DF[ 0 ]DF s" Midi Player" open-application ;
class;

include midi.fs
midi implements
 ( [methodstart] ) : ?player ( -- )
  player self 0= IF
     midi-player new bind player THEN 
  midi-path $@ filename get path+file player file  ;
: dispose ( -- )
  midi-path @ IF  midi-path [ also memory ] HandleOff [ previous ] THEN
  super dispose ; ( [methodend] ) 
  : widget  ( [dumpstart] )
        T" " ^^ ST[  ]ST ( MINOS ) S" Datei:" infotextfield new  ^^bind filename
          ^^ S[ ?player player start ]S ( MINOS )  TT" Play"  icon" icons/play" icon-but new  ^^bind play-it
          ^^ S[ ?player player stop ]S ( MINOS )  TT" Stop"  icon" icons/stop" icon-but new  ^^bind stop-it
          ^^ S[ s" MIDI" s" " midi-path @
IF  midi-path $@  ELSE  S" *.mid"  THEN
^ S[ 2swap midi-path $! filename assign ?player ]S
fsel-action ]S ( MINOS )  TT" Load"  icon" icons/load" icon-but new  ^^bind load-it
          ^^ S[ close ]S ( MINOS )  TT" Close"  icon" icons/mini-cross" icon-but new  ^^bind close-it
        &4 hatbox new &1 hskips
      &2 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  midi open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
