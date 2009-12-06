#! /usr/local/bin/xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

include minos-float.fs
component class fcalc
public:
  tableinfotextfield ptr a#
  tableinfotextfield ptr b#
  tableinfotextfield ptr r#
 ( [varstart] )  ( [varend] )
how:
  : params   DF[ 0 ]DF X" Calculator" ;
class;

fcalc implements
 ( [methodstart] ) also float
: show
    18 a# self #pre
    18 b# self #pre
    18 r# self #pre
    super show ; ( [methodend] )
  : widget  ( [dumpstart] )
        0.00000e0 ]F ( MINOS ) ^^ SF[  ]SF ( MINOS ) X" A:" tableinfotextfield new  ^^bind a#
        0.00000e0 ]F ( MINOS ) ^^ SF[  ]SF ( MINOS ) X" B:" tableinfotextfield new  ^^bind b#
          ^^ S[ a# get b# get f+ r# assign ]S ( MINOS ) X" +" button new 
          ^^ S[ a# get b# get f- r# assign ]S ( MINOS ) X" -" button new 
          ^^ S[ a# get b# get f* r# assign ]S ( MINOS ) X" *" button new 
          ^^ S[ a# get b# get f/ r# assign ]S ( MINOS ) X" /" button new 
          ^^ S[ r# get a# assign ]S ( MINOS ) X" >A" button new 
          ^^ S[ r# get b# assign ]S ( MINOS ) X" >B" button new 
        #6 hatbox new #1 hskips
        0.00000e0 ]F ( MINOS ) ^^ SF[  ]SF ( MINOS ) X" R:" tableinfotextfield new  ^^bind r#
      #4 vabox new panel
    ( [dumpend] ) ;
class;

: main
  fcalc open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
