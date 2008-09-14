#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class calc
public:
  early widget
  early open
  early dialog
  early open-app
  tableinfotextfield ptr a#
  tableinfotextfield ptr b#
  tableinfotextfield ptr c#
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Calculator" open-component ;
  : dialog   new DF[ 0 ]DF s" Calculator" open-dialog ;
  : open-app new DF[ 0 ]DF s" Calculator" open-application ;
class;

calc implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        #0, ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) X" A:" tableinfotextfield new  ^^bind a#
        #0, ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) X" B:" tableinfotextfield new  ^^bind b#
          ^^ S[ a# get b# get d+ c# assign ]S ( MINOS ) X" +" button new 
          ^^ S[ a# get b# get d- c# assign ]S ( MINOS ) X" -" button new 
          ^^ S[ a# get b# get d* c# assign ]S ( MINOS ) X" *" button new 
          ^^ S[ a# get b# get drop ud/mod c# assign drop ]S ( MINOS ) X" /" button new 
          ^^ S[ c# get a# assign ]S ( MINOS ) X" >A" button new 
          ^^ S[ c# get b# assign ]S ( MINOS ) X" >B" button new 
        #6 hatbox new #1 hskips
        #0, ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) X" R:" tableinfotextfield new  ^^bind c#
      #4 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 :: init ;
class;

: main
  calc open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
