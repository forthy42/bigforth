#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class foo
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" Table test" ;
class;

foo implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          ^^ S[  ]S ( MINOS ) X" Dies ist ein Text" button new 
          ^^ S[  ]S ( MINOS ) X" Auch einer" button new 
          ^^ S[  ]S ( MINOS ) X" Und so" button new 
          ^^ S[  ]S ( MINOS )  icon" icons/printer" X" Printer" big-icon new 
        #4 hatab new
          ^^ S[  ]S ( MINOS ) X" Hier" button new 
          ^^ S[  ]S ( MINOS ) X" ist leider kein" button new 
          ^^ S[  ]S ( MINOS ) X" Text :-(" button new 
          ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) X" Toggle Button" rbutton new 
        #4 hatab new
      #2 vabox new
    ( [dumpend] ) ;
class;

: main
  foo open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
