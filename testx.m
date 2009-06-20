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
          GL[  ]GL ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $4 $1 *hfil $4 $1 *vfil glcanvas new 
          GL[  ]GL ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $4 $1 *hfil $4 $1 *vfil glcanvas new 
        #2 habox new panel
          GL[  ]GL ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $4 $1 *hfil $4 $1 *vfil glcanvas new 
          $4 $1 *hfil $4 $1 *hfil glue new 
        #2 habox new panel
          ^^ S[         close ]S ( MINOS ) X" Exit" button new 
        #1 habox new panel
      #3 vabox new panel
    ( [dumpend] ) ;
class;

: main
  comp0000 open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
