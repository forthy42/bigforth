#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class comp0000
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" No Title" ;
class;

comp0000 implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ #3 #100 SC[ ( pos -- ) drop ]SC ( MINOS ) hscaler new  #-30 SC# 
      #1 vabox new panel
    ( [dumpend] ) ;
class;

: main
  comp0000 open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
