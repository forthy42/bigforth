#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class toggle-test
public:
 ( [varstart] ) cell var xx ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Toggle Test" ;
class;

toggle-test implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ TN[ 0 xx ]T[ ( MINOS )  ]TN ( MINOS ) X" Choise A" rbutton new 
        ^^ TN[ 1 xx ]T[ ( MINOS )  ]TN ( MINOS ) X" Choise B" rbutton new 
        ^^ TN[ 2 xx ]T[ ( MINOS )  ]TN ( MINOS ) X" Choise C" rbutton new 
        ^^ TN[ 3 xx ]T[ ( MINOS )  ]TN ( MINOS ) X" Choise D" rbutton new 
        ^^ TN[ 4 xx ]T[ ( MINOS )  ]TN ( MINOS ) X" Choise E" rbutton new 
      #5 varbox new
    ( [dumpend] ) ;
class;

: main
  toggle-test open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
