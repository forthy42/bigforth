#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class canvas1
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Canvas-Test" ;
class;

canvas1 implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        CV[ 12 12 steps  1 11 home!  clear 2 linewidth  path
4 0 DO  10 fd 90 rt  LOOP
fill stroke  0 linewidth ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $50 $A *hfill $50 $A *vfill canvas new 
      #1 vabox new panel #2 borderbox
    ( [dumpend] ) ;
class;

: main
  canvas1 open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
