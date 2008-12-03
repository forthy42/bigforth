#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class dxy
public:
  canvas ptr cv
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Draw Mouse" ;
class;

dxy implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        CV[  ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) ~~ nip 1 and 0= IF  2drop  EXIT  THEN
DOPRESS  ~~  2drop 2drop ]CK ( MINOS ) $100 $1 *hfil $100 $1 *vfil canvas new  ^^bind cv
      #1 vabox new
    ( [dumpend] ) ;
class;

: main
  dxy open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
