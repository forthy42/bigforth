#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class comp0000
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" No Title" ;
class;

comp0000 implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          GL[  ]GL ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $10 $1 *hfil $10 $1 *vfil glcanvas new 
          GL[  ]GL ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $10 $0 *hfil $10 $0 *vfil glcanvas new 
        #2 habox new panel
          GL[  ]GL ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $10 $0 *hfil $10 $0 *vfil glcanvas new 
          $10 $0 *hfil *hglue new 
        #2 habox new panel
          $0 $1 *hfil $0 $1 *vfil glue new 
          ^^ S[ close ]S ( MINOS ) X" Exit" button new 
          $0 $1 *hfil $0 $1 *vfil glue new 
        #3 habox new #1 vskips
      #3 vabox new #1 vskips
    ( [dumpend] ) ;
class;

: main
  comp0000 open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
