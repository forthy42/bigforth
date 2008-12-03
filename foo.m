#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class foo
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" hrtsizer" ;
class;

component class bar
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" hxrtsizer" ;
class;

component class zap
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" hsizer" ;
class;

zap implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
            ^^ S[  ]S ( MINOS ) X" String" button new 
            hsizer new 
          #2 hasbox new
          ^^ S[  ]S ( MINOS ) X" String" button new 
        #2 habox new
      #1 vabox new
    ( [dumpend] ) ;
class;

bar implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
            ^^ S[  ]S ( MINOS ) X" String" button new 
            hxrtsizer new 
          #2 hasbox new
          ^^ S[  ]S ( MINOS ) X" String" button new 
        #2 habox new
      #1 vabox new
    ( [dumpend] ) ;
class;

foo implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
            ^^ S[  ]S ( MINOS ) X" String" button new 
            hrtsizer new 
          #2 hasbox new
          ^^ S[  ]S ( MINOS ) X" String" button new 
        #2 habox new
      #1 vabox new
    ( [dumpend] ) ;
class;

: main
  zap open-app
  bar open-app
  foo open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
