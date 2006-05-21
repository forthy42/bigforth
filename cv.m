#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class dxy
public:
  early widget
  early open
  early dialog
  early open-app
  canvas ptr cv
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Draw Mouse" open-component ;
  : dialog   new DF[ 0 ]DF s" Draw Mouse" open-dialog ;
  : open-app new DF[ 0 ]DF s" Draw Mouse" open-application ;
class;

dxy implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        CV[  ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) ~~ nip 1 and 0= IF  2drop  EXIT  THEN
DOPRESS  ~~  2drop 2drop ]CK ( MINOS ) $100 $1 *hfil $100 $1 *vfil canvas new  ^^bind cv
      &1 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  dxy open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
