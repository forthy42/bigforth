#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class calc
public:
  tableinfotextfield ptr a#
  tableinfotextfield ptr b#
  tableinfotextfield ptr c#
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Calculator" ;
class;

calc implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        #0. ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) X" A:" tableinfotextfield new  ^^bind a#
        #0. ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) X" B:" tableinfotextfield new  ^^bind b#
          ^^ S[ a# get b# get d+ c# assign ]S ( MINOS ) X" +" button new 
          ^^ S[ a# get b# get d- c# assign ]S ( MINOS ) X" -" button new 
          ^^ S[ a# get b# get d* c# assign ]S ( MINOS ) X" *" button new 
          ^^ S[ a# get b# get drop ud/mod c# assign drop ]S ( MINOS ) X" /" button new 
          ^^ S[ c# get a# assign ]S ( MINOS ) X" >A" button new 
          ^^ S[ c# get b# assign ]S ( MINOS ) X" >B" button new 
        #6 hatbox new #1 hskips
        #0. ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) X" R:" tableinfotextfield new  ^^bind c#
      #4 vabox new panel
    ( [dumpend] ) ;
class;

: main
  calc open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
