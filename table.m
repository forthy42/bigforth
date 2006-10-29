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
  : open     new DF[ 0 ]DF s" Table test" open-component ;
  : open-app new DF[ 0 ]DF s" Table test" open-application ;
class;

foo implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          ^^ S[  ]S ( MINOS ) S" Dies ist ein Text" button new 
          ^^ S[  ]S ( MINOS ) S" Auch einer" button new 
          ^^ S[  ]S ( MINOS ) S" Und so" button new 
          ^^ S[  ]S ( MINOS )  icon" icons/printer" S" Printer" big-icon new 
        &4 hatab new
          ^^ S[  ]S ( MINOS ) S" Hier" button new 
          ^^ S[  ]S ( MINOS ) S" ist leider kein" button new 
          ^^ S[  ]S ( MINOS ) S" Text :-(" button new 
          ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" Toggle Button" rbutton new 
        &4 hatab new
      &2 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  foo open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
