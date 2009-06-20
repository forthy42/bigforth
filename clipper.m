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
        clipper new  D[ 
          CV[  ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $10 $1 *hfil $10 $1 *vfil canvas new 
        #1 habox new ]D ( MINOS ) 
      #1 vabox new panel
    ( [dumpend] ) ;
class;

: main
  comp0000 open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
