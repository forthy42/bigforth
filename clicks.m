#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class ccount
public:
  text-label ptr click#
 ( [varstart] ) cell var clicks ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Click counter" ;
class;

ccount implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        X" There have been no clicks yet" text-label new  ^^bind click#
        ^^ S[ 1 clicks +!
clicks @ 0 <# #S s" Number of clicks: " holds #> click# assign ]S ( MINOS ) X" Click me" button new 
      #2 vabox new panel
    ( [dumpend] ) ;
class;

: main
  ccount open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
