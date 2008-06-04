#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class calc
public:
  early widget
  early open
  early open-app
  infotextfield ptr a#
  infotextfield ptr b#
  infotextfield ptr r#
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" No Title" open-component ;
  : open-app new DF[ 0 ]DF s" No Title" open-application ;
class;

component class thermometer
public:
  early widget
  early open
  early open-app
  canvas ptr temp
  vscaler ptr pos
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" No Title" open-component ;
  : open-app new DF[ 0 ]DF s" No Title" open-application ;
class;

thermometer implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          CV[ outer with pos get endwith >r - >r
4 r@ 2+ steps 1 r> 1+ home!
path 0 r@ to 2 0 to 0 r> negate to fill ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $20 $1 *hfilll $C8 $1 *vfilll canvas new  ^^bind temp
        &1 habox new -&2 borderbox
        ^^ &0 &99 SC[ drop temp draw ]SC ( MINOS ) vscaler new  ^^bind pos
          $10 $1 *hfil $1 $2 *vfill glue new 
          ^^ S[ close ]S ( MINOS )  TT" Close application" S"  OK " button new 
          $10 $1 *hfil $1 $1 *vfill glue new 
          ^^ S[ calc open ]S ( MINOS )  TT" Open calculator window" S" Calc" button new 
          $10 $1 *hfil $1 $2 *vfill glue new 
        &5 vabox new
      &3 habox new &1 hskips
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

calc implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        &12341234. ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) S" A:" infotextfield new  ^^bind a#
        &1234. ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) S" B:" infotextfield new  ^^bind b#
          ^^ S[ a# get b# get d+ r# assign ]S ( MINOS ) S" +" button new 
          ^^ S[ a# get b# get d- r# assign ]S ( MINOS ) S" -" button new 
          ^^ S[ a# get b# get d* r# assign ]S ( MINOS ) S" *" button new 
          ^^ S[ a# get b# get drop ud/mod r# assign drop ]S ( MINOS ) S" /" button new 
          ^^ S[ a# get 1. b# get drop 0 ?DO 2over d* LOOP r# assign 2drop ]S ( MINOS ) S" ^" button new 
          ^^ S[ r# get a# assign ]S ( MINOS ) S" >A" button new 
          ^^ S[ r# get b# assign ]S ( MINOS ) S" >B" button new 
        &7 hatbox new &1 hskips
        &0. ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) S" R:" infotextfield new  ^^bind r#
      &4 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  thermometer open-app
  calc open-app
  $2 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
