#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

menu-component class notepad
public:
  stredit ptr notes
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" Notepad" ;
class;

component class file-menu
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" File Menu" ;
class;

component class edit-menu
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" Edit Menu" ;
class;

component class help-menu
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" Help Menu" ;
class;

help-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[  ]S ( MINOS ) X" Help" menu-entry new 
      #1 vabox new #2 borderbox
    ( [dumpend] ) ;
class;

edit-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[  ]S ( MINOS ) X" Cut" menu-entry new 
        ^^ S[  ]S ( MINOS ) X" Copy" menu-entry new 
        ^^ S[  ]S ( MINOS ) X" Paste" menu-entry new 
        ^^ S[  ]S ( MINOS ) X" Search" menu-entry new 
      #4 vabox new #2 borderbox
    ( [dumpend] ) ;
class;

file-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[  ]S ( MINOS ) X" New" menu-entry new 
        ^^ S[  ]S ( MINOS ) X" Open" menu-entry new 
        ^^ S[  ]S ( MINOS ) X" Save" menu-entry new 
        ^^ S[  ]S ( MINOS ) X" Save as..." menu-entry new 
        ^^ S[ bye ]S ( MINOS ) X" Exit" menu-entry new 
      #5 vabox new #2 borderbox
    ( [dumpend] ) ;
class;

notepad implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        M: file-menu menu X" File" menu-title new 
        M: edit-menu menu X" Edit" menu-title new 
        $1 $1 *hfilll $1 $1 *vfil rule new 
        M: help-menu menu X" Help" menu-title new 
      #4 hbox new vfixbox 
        1 1 vviewport new  DS[ 
           (straction stredit new  ^^bind notes $40 setup-edit 
          $10 $1 *hfil $10 $1 *vfil rule new 
        #2 vabox new ]DS ( MINOS ) 
      #1 vabox new
    ( [dumpend] ) ;
class;

: main
  notepad open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
