#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class hello
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" Hello World!" ;
class;

hello implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        X" Hello World!" text-label new 
      #1 vabox new #5 vskips #5 hskips
    ( [dumpend] ) ;
class;

: main
  hello open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
