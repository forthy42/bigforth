#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class toggle-test
public:
  early widget
  early open
  early open-app
 ( [varstart] ) cell var xx ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Toggle Test" open-component ;
  : open-app new DF[ 0 ]DF s" Toggle Test" open-application ;
class;

toggle-test implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ TN[ 0 xx ]T[ ( MINOS )  ]TN ( MINOS ) S" Choise A" rbutton new 
        ^^ TN[ 1 xx ]T[ ( MINOS )  ]TN ( MINOS ) S" Choise B" rbutton new 
        ^^ TN[ 2 xx ]T[ ( MINOS )  ]TN ( MINOS ) S" Choise C" rbutton new 
        ^^ TN[ 3 xx ]T[ ( MINOS )  ]TN ( MINOS ) S" Choise D" rbutton new 
        ^^ TN[ 4 xx ]T[ ( MINOS )  ]TN ( MINOS ) S" Choise E" rbutton new 
      &5 varbox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  toggle-test open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
