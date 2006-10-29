#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

menu-window class designer
public:
  early widget
  early open
  early all-open
  early modal-open
  topindex ptr (topindex-00)
  topindex ptr (topindex-01)
  topindex ptr (topindex-02)
  topindex ptr (topindex-03)
  topindex ptr (topindex-04)
  topindex ptr (topindex-05)
  topindex ptr (topindex-06)
  topindex ptr (topindex-07)
  topindex ptr (topindex-08)
  topindex ptr (topindex-09)
  topindex ptr (topindex-0A)
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
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" File menu" open-component ;
  : open-app new DF[ 0 ]DF s" File menu" open-application ;
class;

component class edit-menu
public:
  early widget
  early open
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Edit menu" open-component ;
  : open-app new DF[ 0 ]DF s" Edit menu" open-application ;
class;

component class help-menu
public:
  early widget
  early open
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Help Menu" open-component ;
  : open-app new DF[ 0 ]DF s" Help Menu" open-application ;
class;

component class minos-about
public:
  early widget
  early open
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" About Theseus" open-component ;
  : open-app new DF[ 0 ]DF s" About Theseus" open-application ;
class;

minos-about implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          $0 $1 *hfill $0 $1 *vfill glue new 
           icon" icons/minos" icon new 
          $0 $1 *hfill $0 $1 *vfill glue new 
        &3 habox new &2 borderbox
          S" Theseus 23feb1999" text-label new 
          S" based on MINOS" text-label new 
          S" (c) 1997-1999 by Bernd Paysan" text-label new 
        &3 vabox new &2 borderbox
          $10 $1 *hfill $10 $1 *vfill glue new 
          ^^ S[ close ]S ( MINOS ) S"  OK " button new 
          $10 $1 *hfill $10 $1 *vfill glue new 
        &3 habox new &1 vskips
      &3 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

help-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[ 0" cd help; ${BROWSER-./netscape.sh} file://$PWD/theseus.html >/dev/null 2>/dev/null &"
[ also DOS ] system [ previous ] drop ]S ( MINOS ) S" Using Theseus" menu-entry new 
        ^^ S[ minos-about open ]S ( MINOS ) S" About Theseus" menu-entry new 
      &2 vabox new &2 borderbox
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

edit-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[  ]S ( MINOS ) S" New Dialog" menu-entry new 
        ^^ S[  ]S ( MINOS ) S" New Menu Window" menu-entry new 
      &2 vabox new &2 borderbox
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

file-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[  ]S ( MINOS ) S" Load File..." menu-entry new 
        ^^ S[  ]S ( MINOS ) S" Save as..." menu-entry new 
        ^^ S[  ]S ( MINOS ) S" Run Application" menu-entry new 
        ^^ S[  ]S ( MINOS ) S" Save as module..." menu-entry new 
        ^^ S[  ]S ( MINOS ) S" New Designer" menu-entry new 
        ^^ S[  ]S ( MINOS ) S" Quit" menu-entry new 
      &6 vabox new &2 borderbox
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

designer implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        M: file-menu widget S"  File " menu-title new 
        M: edit-menu widget S"  Edit " menu-title new 
        $10 $1 *hfilll $1 $1 *vfil rule new 
        M: help-menu widget S"  Help " menu-title new 
      &4 hbox new vfixbox  &2 borderbox
                    0 -1 flipper S" Buttons" topindex new ^^bind (topindex-00)
                    0 0 flipper S" Toggles" topindex new ^^bind (topindex-01)
                    0 0 flipper S" Text Fields" topindex new ^^bind (topindex-02)
                    0 0 flipper S" Sliders" topindex new ^^bind (topindex-03)
                    0 0 flipper S" Menues" topindex new ^^bind (topindex-04)
                    0 0 flipper S" Labels" topindex new ^^bind (topindex-05)
                    0 0 flipper S" Glues" topindex new ^^bind (topindex-06)
                    0 0 flipper S" Canvas" topindex new ^^bind (topindex-07)
                    0 0 flipper S" Displays" topindex new ^^bind (topindex-08)
                    topglue new 
                  &10 harbox new
                      ^^ S[  ]S ( MINOS ) S" Button" button new 
                      ^^ S[  ]S ( MINOS ) S" LButton" button new 
                      ^^ S[  ]S ( MINOS ) S" Icon-Button" button new 
                      ^^ S[  ]S ( MINOS ) S" Icon" button new 
                      ^^ S[  ]S ( MINOS ) S" Big-Icon" button new 
                      $1 $1 *hfilll $1 $1 *vfil glue new 
                    &6 habox new panel dup ^^ with C[ (topindex-00) ]C ( MINOS ) endwith 
                      ^^ S[  ]S ( MINOS ) S" String" button new 
                    &1 habox new flipbox  panel dup ^^ with C[ (topindex-01) ]C ( MINOS ) endwith 
                      ^^ S[  ]S ( MINOS ) S" String" button new 
                    &1 habox new flipbox  panel dup ^^ with C[ (topindex-02) ]C ( MINOS ) endwith 
                      ^^ S[  ]S ( MINOS ) S" String" button new 
                    &1 habox new flipbox  panel dup ^^ with C[ (topindex-03) ]C ( MINOS ) endwith 
                      ^^ S[  ]S ( MINOS ) S" String" button new 
                    &1 habox new flipbox  panel dup ^^ with C[ (topindex-04) ]C ( MINOS ) endwith 
                      ^^ S[  ]S ( MINOS ) S" String" button new 
                    &1 harbox new flipbox  panel dup ^^ with C[ (topindex-05) ]C ( MINOS ) endwith 
                      ^^ S[  ]S ( MINOS ) S" String" button new 
                    &1 harbox new flipbox  panel dup ^^ with C[ (topindex-06) ]C ( MINOS ) endwith 
                      ^^ S[  ]S ( MINOS ) S" String" button new 
                    &1 harbox new flipbox  panel dup ^^ with C[ (topindex-07) ]C ( MINOS ) endwith 
                      ^^ S[  ]S ( MINOS ) S" String" button new 
                    &1 harbox new flipbox  panel dup ^^ with C[ (topindex-08) ]C ( MINOS ) endwith 
                  &9 habox new $10  noborderbox  &2 borderbox
                &2 vabox new
              &1 habox new
                  ^^ S[  ]S ( MINOS ) S" hbox" button new 
                  ^^ S[  ]S ( MINOS ) S" vbox" button new 
                &2 vabox new
              &1 habox new hfixbox  panel &2 borderbox
            &2 habox new vfixbox 
                  ^^ -1 T[  ][ ( MINOS )  ]T ( MINOS )  TT" Edit-Text/Code/Name-Mode"  icon" icons/ecn" flipicon new 
                  ^^  0 T[  ][ ( MINOS )  ]T ( MINOS )  TT" Cut/Copy/Paste-Mode"  icon" icons/cut+copy+paste" flipicon new 
                  ^^  0 T[  ][ ( MINOS )  ]T ( MINOS )  TT" Try-Mode"  icon" icons/try" flipicon new 
                &3 varbox new &2 borderbox
                  ^^  0 T[  ][ ( MINOS )  ]T ( MINOS )  icon" icons/head" flipicon new 
                  ^^ -1 T[  ][ ( MINOS )  ]T ( MINOS )  icon" icons/tail" flipicon new 
                  ^^  0 T[  ][ ( MINOS )  ]T ( MINOS )  icon" icons/before" flipicon new 
                  ^^  0 T[  ][ ( MINOS )  ]T ( MINOS )  icon" icons/after" flipicon new 
                &4 varbox new &2 borderbox
                    $0 $1 *hfill $0 $1 *vfil glue new 
                    ^^ S[  ]S ( MINOS ) :up tributton new 
                    $0 $1 *hfill $0 $1 *vfil glue new 
                  &3 habox new
                    ^^ S[  ]S ( MINOS ) :left tributton new 
                    $10 $1 *hfil $0 $1 *vfil glue new 
                    ^^ S[  ]S ( MINOS ) :right tributton new 
                  &3 habox new
                    $0 $1 *hfill $0 $1 *vfil glue new 
                    ^^ S[  ]S ( MINOS ) :down tributton new 
                    $0 $1 *hfill $0 $1 *vfil glue new 
                  &3 habox new
                &3 vabox new &2 borderbox
                  ^^ S[  ]S ( MINOS )  icon" icons/load" icon-but new 
                  ^^ S[  ]S ( MINOS )  icon" icons/save" icon-but new 
                  ^^ S[  ]S ( MINOS )  icon" icons/run" icon-but new 
                  ^^ S[  ]S ( MINOS )  icon" icons/mod" icon-but new 
                &4 vabox new &2 borderbox
                $0 $1 *hfil $0 $1 *vfilll glue new 
              &5 vabox new hfixbox 
                  1 1 viewport new  DS[ 
                    $10 $1 *hfil $10 $1 *vfil rule new 
                  &1 habox new panel ]DS ( MINOS ) 
                &1 habox new panel
              &1 habox new &2 borderbox
            &2 habox new
          &2 vabox new
                ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" horizontal" tbutton new 
                ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" active" tbutton new 
                ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" radio" tbutton new 
                ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" tabbing" tbutton new 
                ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" hfixbox" tbutton new 
                ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" vfixbox" tbutton new 
                ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" flipbox" tbutton new 
                ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" rzbox" tbutton new 
              &8 vabox new vfixbox 
                  0 -1 flipper S" Low" topindex new ^^bind (topindex-09)
                  0 0 flipper S" Detail" topindex new ^^bind (topindex-0A)
                &2 harbox new
                      ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" hskip" tbutton new 
                      ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" vskip" tbutton new 
                      ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) S" border" tbutton new 
                      $10 $1 *hfil $0 $1 *vfill glue new 
                    &4 vabox new
                  &1 habox new dup ^^ with C[ (topindex-09) ]C ( MINOS ) endwith 
                      ^^ &0 &10 SC[ drop ]SC ( MINOS )  TT" hskip" hscaler new 
                      ^^ &0 &10 SC[ drop ]SC ( MINOS )  TT" vskip" hscaler new 
                      ^^ &10 &20 SC[ drop ]SC ( MINOS )  TT" border" hscaler new 
                    &3 vabox new
                  &1 habox new flipbox  dup ^^ with C[ (topindex-0A) ]C ( MINOS ) endwith 
                &2 habox new $10  noborderbox  &2 borderbox
              &2 vabox new vfixbox 
            &2 vabox new panel &2 borderbox
            $0 $1 *hfil $0 $1 *vfilll glue new 
          &2 vabox new hfixbox 
        &2 habox new
          vrtsizer new 
            1 1 viewport new  DS[ 
              $0 $1 *hfil $0 $1 *vfil glue new 
                $0 $1 *hfil $0 $1 *vfil rule new 
                 icon" icons/minos" icon new 
                $0 $1 *hfil $0 $1 *vfil rule new 
              &3 habox new 
              $0 $1 *hfil $0 $1 *vfil glue new 
            &3 vabox new  ]DS ( MINOS ) 
            $0 $0 *hfil $68 $1 *vfil rule new 
          &2 habox new
        &2 vasbox new
      &2 vabox new
    ( [dumpend] ) ;
  : title$  s" Theseus" ;
  : init  super init  ^ to ^^
    widget 1 0 modal new  title$ assign ;
class;

: main
  designer all-open
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
