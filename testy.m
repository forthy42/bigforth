#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class comp0000
public:
  early widget
  early open
  early dialog
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" No Title" open-component ;
  : dialog   new DF[ 0 ]DF s" No Title" open-dialog ;
  : open-app new DF[ 0 ]DF s" No Title" open-application ;
class;

comp0000 implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          GL[  ]GL ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $10 $1 *hfil $10 $1 *vfil glcanvas new 
          GL[  ]GL ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $10 $0 *hfil $10 $0 *vfil glcanvas new 
        &2 habox new panel
          GL[  ]GL ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $10 $0 *hfil $10 $0 *vfil glcanvas new 
          $10 $0 *hfil *hglue new 
        &2 habox new panel
          $0 $1 *hfil $0 $1 *vfil glue new 
          ^^ S[ close ]S ( MINOS ) S" Exit" button new 
          $0 $1 *hfil $0 $1 *vfil glue new 
        &3 habox new &1 vskips
      &3 vabox new &1 vskips
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  comp0000 open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
