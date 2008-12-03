#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

include css.fs
component class dvd-play
public:
  icon-but ptr play-it
 ( [varstart] ) cell var playing ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" DVD Player" ;
class;

dvd-play implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          ^^ S[ playing @ 0=
IF  do-audio @
    IF  0" ac3dec -o oss /tmp/audio &" system drop  THEN
    do-video @
    IF  0" mpeg2dec -o x11 /tmp/video &" system drop  THEN
    $2000 dup NewTask activate css-cat EXIT
THEN  playing on ]S ( MINOS )  TT" Play"  icon" icons/play" icon-but new  ^^bind play-it
          ^^ S[ >disk_key ]S ( MINOS ) X" Auth Disk" button new 
        #2 vatbox new #1 vskips
          ^^ TV[ do-audio ]T[ ( MINOS )  ]TV ( MINOS ) X" Audio" tbutton new 
          ^^ TV[ do-video ]T[ ( MINOS )  ]TV ( MINOS ) X" Video" tbutton new 
        #2 vabox new #1 vskips
          ^^ TN[ 0 track ]T[ ( MINOS )  ]TN ( MINOS ) X" English" rbutton new 
          ^^ TN[ 1 track ]T[ ( MINOS )  ]TN ( MINOS ) X" German" rbutton new 
          ^^ TN[ 2 track ]T[ ( MINOS )  ]TN ( MINOS ) X" French" rbutton new 
        #3 varbox new
      #3 hatbox new panel
    ( [dumpend] ) ;
class;

: main
  dvd-play open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
