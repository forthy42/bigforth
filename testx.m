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
~~          GL[  ]GL ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) 4 1 *hfil 4 1 *vfil glcanvas new
~~          GL[  ]GL ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) 4 1 *hfil 4 1 *vfil glcanvas new
~~        &2 habox new panel
~~          GL[  ]GL ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) 4 1 *hfil 4 1 *vfil glcanvas new
~~          4 1 *hfil 4 1 *hfil glue new
~~        &2 habox new panel
~~          ^^ S[         close ]S ( MINOS ) S" Exit" button new 
~~        &1 habox new panel
~~      &3 vabox new panel
~~    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  comp0000 open-app
   0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
