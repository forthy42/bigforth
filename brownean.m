#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class brownean
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" Brownean move" ;
class;

brownean implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        CV[ timer@ seed !
640 480 steps $FF 0 0 rgb> drawcolor  0 480 2/ home!
path  640 0 DO  1  21 random 10 - to  LOOP  stroke ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $280 $1 *hfil $1E0 $1 *vfil canvas new 
      #1 vabox new
    ( [dumpend] ) ;
class;

: main
  brownean open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
