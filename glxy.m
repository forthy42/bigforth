#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class glmouse
public:
  glcanvas ptr glw
 ( [varstart] ) 2 cells var glxy ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" GL mouse" ;
class;

glmouse implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        GL[   ]GL ( MINOS ) ^^ CK[ ( x y b n -- ) ~~ nip 1 and 0= IF  2drop  EXIT  THEN
DOPRESS  ~~ glxy 2! 2drop glw draw ]CK ( MINOS ) $100 $1 *hfil $100 $1 *vfil glcanvas new  ^^bind glw
      #1 vabox new
    ( [dumpend] ) ;
class;

: main
  glmouse open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
