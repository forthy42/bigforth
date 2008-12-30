#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class calc
public:
  infotextfield ptr a#
  infotextfield ptr b#
  infotextfield ptr r#
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Calculator" ;
class;

component class thermometer
public:
  canvas ptr temp
  vscaler ptr pos
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Thermometer" ;
class;

thermometer implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        CV[ outer with pos get endwith >r - >r
4 r@ 2+ steps 1 r@ 1+ home!
$FF r> r@ $FF rot */ 0 rgb>fill fillcolor
path 0 r@ to 2 0 to 0 r> negate to -2 0 to fill stroke ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $20 $1 *hfilll $C8 $1 *vfilll canvas new  ^^bind temp
        ^^ #0 #99 SC[ drop temp draw ]SC ( MINOS ) vscaler new  ^^bind pos
          $10 $1 *hfil $10 $1 *vfill glue new 
          ^^ S[ close ]S ( MINOS )  TT" Close application" X"  OK " button new 
          $10 $1 *hfil $10 $1 *vfill glue new 
          ^^ S[ calc open ]S ( MINOS )  TT" Open calculator window" X" Calc" button new 
          $10 $1 *hfil $10 $1 *vfill glue new 
        #5 vabox new
      #3 habox new #1 hskips
    ( [dumpend] ) ;
class;

calc implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        #0. ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) X" A:" infotextfield new  ^^bind a#
        #0. ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) X" B:" infotextfield new  ^^bind b#
          ^^ S[ a# get b# get d+ r# assign ]S ( MINOS ) X" +" button new 
          ^^ S[ a# get b# get d- r# assign ]S ( MINOS ) X" -" button new 
          ^^ S[ a# get b# get d* r# assign ]S ( MINOS ) X" *" button new 
          ^^ S[ a# get b# get drop ud/mod r# assign drop ]S ( MINOS ) X" /" button new 
          ^^ S[ a# get 1. b# get drop 0 ?DO 2over d* LOOP r# assign 2drop ]S ( MINOS ) X" ^" button new 
          ^^ S[ r# get a# assign ]S ( MINOS ) X" >A" button new 
          ^^ S[ r# get b# assign ]S ( MINOS ) X" >B" button new 
        #7 hatbox new #1 hskips
        #0. ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) X" R:" infotextfield new  ^^bind r#
      #4 vabox new panel
    ( [dumpend] ) ;
class;

: main
  thermometer open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
