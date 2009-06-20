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
 ( [varstart] ) midi-player ptr player
cell var midi-path ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" Midi Player" ;
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
        T" " ^^ ST[  ]ST ( MINOS ) X" Datei:" infotextfield new  ^^bind filename
          ^^ S[ ?player player start ]S ( MINOS ) X" Play" button new  ^^bind play-it
          ^^ S[ ?player player stop ]S ( MINOS ) X" Stop" button new  ^^bind stop-it
          ^^ S[ s" MIDI" s" " midi-path @
IF  midi-path $@  ELSE  S" *.mid"  THEN
^ S[ 2swap midi-path $! filename assign ?player ]S
fsel-action ]S ( MINOS ) X" Load" button new  ^^bind load-it
          ^^ S[ close ]S ( MINOS ) X" Close" button new  ^^bind close-it
        #4 hatbox new #1 hskips
      #2 vabox new panel
    ( [dumpend] ) ;
class;

: main
  midi open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
