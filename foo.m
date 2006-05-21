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
  : open     new DF[ 0 ]DF s" hrtsizer" open-component ;
  : open-app new DF[ 0 ]DF s" hrtsizer" open-application ;
class;

component class bar
public:
  early widget
  early open
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" hxrtsizer" open-component ;
  : open-app new DF[ 0 ]DF s" hxrtsizer" open-application ;
class;

component class zap
public:
  early widget
  early open
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" hsizer" open-component ;
  : open-app new DF[ 0 ]DF s" hsizer" open-application ;
class;

zap implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
            ^^ S[  ]S ( MINOS ) S" String" button new 
            hsizer new 
          &2 hasbox new
          ^^ S[  ]S ( MINOS ) S" String" button new 
        &2 habox new
      &1 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

bar implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
            ^^ S[  ]S ( MINOS ) S" String" button new 
            hxrtsizer new 
          &2 hasbox new
          ^^ S[  ]S ( MINOS ) S" String" button new 
        &2 habox new
      &1 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

foo implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
            ^^ S[  ]S ( MINOS ) S" String" button new 
            hrtsizer new 
          &2 hasbox new
          ^^ S[  ]S ( MINOS ) S" String" button new 
        &2 habox new
      &1 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  zap open-app
  bar open-app
  foo open-app
  $3 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
