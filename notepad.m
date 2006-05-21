#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

menu-window class notepad
public:
  early widget
  early open
  early all-open
  early modal-open
  stredit ptr notes
 ( [varstart] )  ( [varend] ) 
how:
  : open       screen self new >o show o> ;
  : all-open   screen self new >o show up@ app ! o> ;
  : modal-open screen self new >o show stop o> ;
class;

component class file-menu
public:
  early widget
  early open
  early dialog
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" File Menu" open-component ;
  : dialog   new DF[ 0 ]DF s" File Menu" open-dialog ;
  : open-app new DF[ 0 ]DF s" File Menu" open-application ;
class;

component class edit-menu
public:
  early widget
  early open
  early dialog
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Edit Menu" open-component ;
  : dialog   new DF[ 0 ]DF s" Edit Menu" open-dialog ;
  : open-app new DF[ 0 ]DF s" Edit Menu" open-application ;
class;

component class help-menu
public:
  early widget
  early open
  early dialog
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Help Menu" open-component ;
  : dialog   new DF[ 0 ]DF s" Help Menu" open-dialog ;
  : open-app new DF[ 0 ]DF s" Help Menu" open-application ;
class;

help-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[  ]S ( MINOS ) S" Help" menu-entry new 
      &1 vabox new &2 borderbox
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

edit-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[  ]S ( MINOS ) S" Cut" menu-entry new 
        ^^ S[  ]S ( MINOS ) S" Copy" menu-entry new 
        ^^ S[  ]S ( MINOS ) S" Paste" menu-entry new 
        ^^ S[  ]S ( MINOS ) S" Search" menu-entry new 
      &4 vabox new &2 borderbox
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

file-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[  ]S ( MINOS ) S" New" menu-entry new 
        ^^ S[  ]S ( MINOS ) S" Open" menu-entry new 
        ^^ S[  ]S ( MINOS ) S" Save" menu-entry new 
        ^^ S[  ]S ( MINOS ) S" Save as..." menu-entry new 
        ^^ S[ bye ]S ( MINOS ) S" Exit" menu-entry new 
      &5 vabox new &2 borderbox
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

notepad implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        M: file-menu widget S" File" menu-title new 
        M: edit-menu widget S" Edit" menu-title new 
        $1 $1 *hfilll $1 $1 *vfil rule new 
        M: help-menu widget S" Help" menu-title new 
      &4 hbox new vfixbox 
        1 1 vviewport new  DS[ 
           (straction stredit new  ^^bind notes $40 setup-edit 
          $10 $1 *hfil $10 $1 *vfil rule new 
        &2 vabox new ]DS ( MINOS ) 
      &1 vabox new
    ( [dumpend] ) ;
  : title$  s" Notepad" ;
  : init  super init  ^ to ^^
    widget 1 0 modal new  title$ assign ;
class;

: main
  notepad all-open
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
