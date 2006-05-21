#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class hello
public:
  early widget
  early open
  early dialog
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Hello World!" open-component ;
  : dialog   new DF[ 0 ]DF s" Hello World!" open-dialog ;
  : open-app new DF[ 0 ]DF s" Hello World!" open-application ;
class;

hello implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        S" Hello World!" text-label new 
      &1 vabox new &5 vskips &5 hskips
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  hello open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
