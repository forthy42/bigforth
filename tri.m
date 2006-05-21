#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class foo
public:
  early widget
  early open
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Tributtons" open-component ;
  : open-app new DF[ 0 ]DF s" Tributtons" open-application ;
class;

foo implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          ^^ S[ cr ." left" ]S ( MINOS ) :left tributton new 
          ^^ S[ cr ." up" ]S ( MINOS ) :up tributton new 
          ^^ S[ cr ." down" ]S ( MINOS ) :down tributton new 
          ^^ S[ cr ." right" ]S ( MINOS ) :right tributton new 
        &4 habox new &1 hskips
      &1 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  foo open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
