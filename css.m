#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

include css.fs
component class dvd-play
public:
  early widget
  early open
  early dialog
  early open-app
  icon-but ptr play-it
 ( [varstart] ) cell var playing ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" DVD Player" open-component ;
  : dialog   new DF[ 0 ]DF s" DVD Player" open-dialog ;
  : open-app new DF[ 0 ]DF s" DVD Player" open-application ;
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
          ^^ S[ >disk_key ]S ( MINOS ) S" Auth Disk" button new 
        &2 vatbox new &1 vskips
          ^^ TV[ do-audio ]T[ ( MINOS )  ]TV ( MINOS ) S" Audio" tbutton new 
          ^^ TV[ do-video ]T[ ( MINOS )  ]TV ( MINOS ) S" Video" tbutton new 
        &2 vabox new &1 vskips
          ^^ TN[ 0 track ]T[ ( MINOS )  ]TN ( MINOS ) S" English" rbutton new 
          ^^ TN[ 1 track ]T[ ( MINOS )  ]TN ( MINOS ) S" German" rbutton new 
          ^^ TN[ 2 track ]T[ ( MINOS )  ]TN ( MINOS ) S" French" rbutton new 
        &3 varbox new
      &3 hatbox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  dvd-play open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
