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
  glcanvas ptr gl1
  glcanvas ptr gl2
  glcanvas ptr gl3
  button ptr gl2
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" No Title" open-component ;
  : dialog   new DF[ 0 ]DF s" No Title" open-dialog ;
  : open-app new DF[ 0 ]DF s" No Title" open-application ;
class;

comp0000 implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
            GL[  ]GL ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop dopress cr ." zx " . . . . ]CK ( MINOS ) $64 $1 *hfil $64 $1 *vfil glcanvas new  ^^bind gl1
            GL[  ]GL ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop dopress cr ." xy " . . . . ]CK ( MINOS ) $64 $1 *hfil $64 $1 *vfil glcanvas new  ^^bind gl2
          &2 vabox new
            GL[  ]GL ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop dopress cr ." zy " . . . . ]CK ( MINOS ) $64 $1 *hfil $64 $1 *vfil glcanvas new  ^^bind gl3
            $64 $1 *hfil $64 $1 *vfil glue new
          &2 vabox new &2 habox new &1 hskips
          ^^ S[ close ]S ( MINOS ) S" done" button new  ^^bind gl2
        &1 vabox new vfixbox 
      &2 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  comp0000 open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
