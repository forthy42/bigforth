#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class color
public:
  early widget
  early open
  early open-app
  button ptr color-ok
 ( [varstart] ) cell var color ( [varend] ) 
how:
  : open     new DF[ color-ok self ]DF s" Color" open-component ;
  : open-app new DF[ color-ok self ]DF s" Color" open-application ;
class;

component class result
public:
  early widget
  early open
  early open-app
  infotextfield ptr choice
  button ptr choice-ok
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ choice-ok self ]DF s" Your Choice" open-component ;
  : open-app new DF[ choice-ok self ]DF s" Your Choice" open-application ;
class;

result implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        T" " ^^ ST[  ]ST ( MINOS ) S" Choice" infotextfield new  ^^bind choice
          ^^ S[ bye ]S ( MINOS ) S" OK" button new  ^^bind choice-ok
          ^^ S[ close ]S ( MINOS ) S" Cancel" button new 
        &2 hatbox new &1 hskips
      &2 hatbox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

color implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          ^^ TN[ 0 color ]T[ ( MINOS )  ]TN ( MINOS ) S" Red" rbutton new 
          ^^ TN[ 1 color ]T[ ( MINOS )  ]TN ( MINOS ) S" Green" rbutton new 
          ^^ TN[ 2 color ]T[ ( MINOS )  ]TN ( MINOS ) S" Blue" rbutton new 
          ^^ TN[ 3 color ]T[ ( MINOS )  ]TN ( MINOS ) S" Alpha" rbutton new 
        &4 varbox new
          $10 $1 *hfill $0 $1 *vfill glue new 
          ^^ S[ color @ 0 = IF s" Red"   THEN
color @ 1 = IF s" Green" THEN
color @ 2 = IF s" Blue"  THEN
color @ 3 u>= IF s" Alpha" THEN
result new
result with choice assign ^
  choice-ok self S" Your Chouse" open-component endwith ]S ( MINOS ) S" OK" button new  ^^bind color-ok
          ^^ S[ close ]S ( MINOS ) S" Cancel" button new 
          $10 $1 *hfill $0 $1 *vfill glue new 
        &4 vabox new &1 vskips
      &2 habox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  color open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
