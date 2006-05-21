#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class glmouse
public:
  early widget
  early open
  early dialog
  early open-app
  glcanvas ptr glw
 ( [varstart] ) 2 cells var glxy ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" GL mouse" open-component ;
  : dialog   new DF[ 0 ]DF s" GL mouse" open-dialog ;
  : open-app new DF[ 0 ]DF s" GL mouse" open-application ;
class;

glmouse implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        GL[   ]GL ( MINOS ) ^^ CK[ ( x y b n -- ) ~~ nip 1 and 0= IF  2drop  EXIT  THEN
DOPRESS  ~~ glxy 2! 2drop glw draw ]CK ( MINOS ) $100 $1 *hfil $100 $1 *vfil glcanvas new  ^^bind glw
      &1 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  glmouse open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
