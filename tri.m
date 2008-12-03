#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class foo
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Tributtons" ;
class;

foo implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          ^^ S[ cr ." left" ]S ( MINOS ) :left tributton new 
          ^^ S[ cr ." up" ]S ( MINOS ) :up tributton new 
          ^^ S[ cr ." down" ]S ( MINOS ) :down tributton new 
          ^^ S[ cr ." right" ]S ( MINOS ) :right tributton new 
        #4 habox new #1 hskips
      #1 vabox new panel
    ( [dumpend] ) ;
class;

: main
  foo open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
