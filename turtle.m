#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class turtle
public:
  early widget
  early open
  early dialog
  early open-app
  canvas ptr graphics
 ( [varstart] ) 2 cells var homepos
 ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Turtle Graphics" open-component ;
  : dialog   new DF[ 0 ]DF s" Turtle Graphics" open-dialog ;
  : open-app new DF[ 0 ]DF s" Turtle Graphics" open-application ;
class;

turtle implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        backing new  D[ 
          CV[  ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $100 $1 *hfil $100 $1 *vfil canvas new  ^^bind graphics
        &1 habox new ]D ( MINOS ) 
      &1 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  turtle open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
