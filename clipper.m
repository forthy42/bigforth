#! ./xbigforth
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
        clipper new  D[ 
          CV[  ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $10 $1 *hfil $10 $1 *vfil canvas new 
        &1 habox new ]D ( MINOS ) 
      &1 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  comp0000 open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
