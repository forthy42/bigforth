#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class turtle
public:
  canvas ptr graphics
 ( [varstart] ) 2 cells var homepos
 ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Turtle Graphics" ;
class;

turtle implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        backing new  D[ 
          CV[  ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $100 $1 *hfil $100 $1 *vfil canvas new  ^^bind graphics
        #1 habox new ]D ( MINOS ) 
      #1 vabox new
    ( [dumpend] ) ;
class;

: main
  turtle open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
