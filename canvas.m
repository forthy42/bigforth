#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class canvas1
public:
  early widget
  early open
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Canvas-Test" open-component ;
  : open-app new DF[ 0 ]DF s" Canvas-Test" open-application ;
class;

canvas1 implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        CV[ 12 12 steps  1 11 home!  clear 2 linewidth  path
4 0 DO  10 fd 90 rt  LOOP
fill stroke  0 linewidth ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $50 $A *hfill $50 $A *vfill canvas new 
      &1 vabox new panel &2 borderbox
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  canvas1 open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
