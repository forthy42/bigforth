#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

menu-component class designer
public:
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
  : params   DF[ 0 ]DF X" Theseus" ;
class;

component class file-menu
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" File menu" ;
class;

component class edit-menu
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" Edit menu" ;
class;

component class help-menu
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" Help Menu" ;
class;

component class minos-about
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" About Theseus" ;
class;

minos-about implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          $0 $1 *hfill $0 $1 *vfill glue new 
           icon" icons/minos" icon new 
          $0 $1 *hfill $0 $1 *vfill glue new 
        #3 habox new #2 borderbox
          X" Theseus 23feb1999" text-label new 
          X" based on MINOS" text-label new 
          X" (c) 1997-1999 by Bernd Paysan" text-label new 
        #3 vabox new #2 borderbox
          $10 $1 *hfill $10 $1 *vfill glue new 
          ^^ S[ close ]S ( MINOS ) X"  OK " button new 
          $10 $1 *hfill $10 $1 *vfill glue new 
        #3 habox new #1 vskips
      #3 vabox new
    ( [dumpend] ) ;
class;

help-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[ 0" cd help; ${BROWSER-./netscape.sh} file://$PWD/theseus.html >/dev/null 2>/dev/null &"
[ also DOS ] system [ previous ] drop ]S ( MINOS ) X" Using Theseus" menu-entry new 
        ^^ S[ minos-about open ]S ( MINOS ) X" About Theseus" menu-entry new 
      #2 vabox new #2 borderbox
    ( [dumpend] ) ;
class;

edit-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[  ]S ( MINOS ) X" New Dialog" menu-entry new 
        ^^ S[  ]S ( MINOS ) X" New Menu Window" menu-entry new 
      #2 vabox new #2 borderbox
    ( [dumpend] ) ;
class;

file-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[  ]S ( MINOS ) X" Load File..." menu-entry new 
        ^^ S[  ]S ( MINOS ) X" Save as..." menu-entry new 
        ^^ S[  ]S ( MINOS ) X" Run Application" menu-entry new 
        ^^ S[  ]S ( MINOS ) X" Save as module..." menu-entry new 
        ^^ S[  ]S ( MINOS ) X" New Designer" menu-entry new 
        ^^ S[  ]S ( MINOS ) X" Quit" menu-entry new 
      #6 vabox new #2 borderbox
    ( [dumpend] ) ;
class;

designer implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        M: file-menu menu X"  File " menu-title new 
        M: edit-menu menu X"  Edit " menu-title new 
        $10 $1 *hfilll $1 $1 *vfil rule new 
        M: help-menu menu X"  Help " menu-title new 
      #4 hbox new vfixbox  #2 borderbox
                    0 -1 flipper X" Buttons" topindex new ^^bind (topindex-00)
                    0 0 flipper X" Toggles" topindex new ^^bind (topindex-01)
                    0 0 flipper X" Text Fields" topindex new ^^bind (topindex-02)
                    0 0 flipper X" Sliders" topindex new ^^bind (topindex-03)
                    0 0 flipper X" Menues" topindex new ^^bind (topindex-04)
                    0 0 flipper X" Labels" topindex new ^^bind (topindex-05)
                    0 0 flipper X" Glues" topindex new ^^bind (topindex-06)
                    0 0 flipper X" Canvas" topindex new ^^bind (topindex-07)
                    0 0 flipper X" Displays" topindex new ^^bind (topindex-08)
                    topglue new 
                  #10 harbox new
                      ^^ S[  ]S ( MINOS ) X" Button" button new 
                      ^^ S[  ]S ( MINOS ) X" LButton" button new 
                      ^^ S[  ]S ( MINOS ) X" Icon-Button" button new 
                      ^^ S[  ]S ( MINOS ) X" Icon" button new 
                      ^^ S[  ]S ( MINOS ) X" Big-Icon" button new 
                      $1 $1 *hfilll $1 $1 *vfil glue new 
                    #6 habox new panel dup ^^ with C[ (topindex-00) ]C ( MINOS ) endwith 
                      ^^ S[  ]S ( MINOS ) X" String" button new 
                    #1 habox new flipbox  panel dup ^^ with C[ (topindex-01) ]C ( MINOS ) endwith 
                      ^^ S[  ]S ( MINOS ) X" String" button new 
                    #1 habox new flipbox  panel dup ^^ with C[ (topindex-02) ]C ( MINOS ) endwith 
                      ^^ S[  ]S ( MINOS ) X" String" button new 
                    #1 habox new flipbox  panel dup ^^ with C[ (topindex-03) ]C ( MINOS ) endwith 
                      ^^ S[  ]S ( MINOS ) X" String" button new 
                    #1 habox new flipbox  panel dup ^^ with C[ (topindex-04) ]C ( MINOS ) endwith 
                      ^^ S[  ]S ( MINOS ) X" String" button new 
                    #1 harbox new flipbox  panel dup ^^ with C[ (topindex-05) ]C ( MINOS ) endwith 
                      ^^ S[  ]S ( MINOS ) X" String" button new 
                    #1 harbox new flipbox  panel dup ^^ with C[ (topindex-06) ]C ( MINOS ) endwith 
                      ^^ S[  ]S ( MINOS ) X" String" button new 
                    #1 harbox new flipbox  panel dup ^^ with C[ (topindex-07) ]C ( MINOS ) endwith 
                      ^^ S[  ]S ( MINOS ) X" String" button new 
                    #1 harbox new flipbox  panel dup ^^ with C[ (topindex-08) ]C ( MINOS ) endwith 
                  #9 habox new $10  noborderbox  #2 borderbox
                #2 vabox new
              #1 habox new
                  ^^ S[  ]S ( MINOS ) X" hbox" button new 
                  ^^ S[  ]S ( MINOS ) X" vbox" button new 
                #2 vabox new
              #1 habox new hfixbox  panel #2 borderbox
            #2 habox new vfixbox 
                  ^^ -1 T[  ][ ( MINOS )  ]T ( MINOS )  TT" Edit-Text/Code/Name-Mode"  icon" icons/ecn" flipicon new 
                  ^^  0 T[  ][ ( MINOS )  ]T ( MINOS )  TT" Cut/Copy/Paste-Mode"  icon" icons/cut+copy+paste" flipicon new 
                  ^^  0 T[  ][ ( MINOS )  ]T ( MINOS )  TT" Try-Mode"  icon" icons/try" flipicon new 
                #3 varbox new #2 borderbox
                  ^^  0 T[  ][ ( MINOS )  ]T ( MINOS )  icon" icons/head" flipicon new 
                  ^^ -1 T[  ][ ( MINOS )  ]T ( MINOS )  icon" icons/tail" flipicon new 
                  ^^  0 T[  ][ ( MINOS )  ]T ( MINOS )  icon" icons/before" flipicon new 
                  ^^  0 T[  ][ ( MINOS )  ]T ( MINOS )  icon" icons/after" flipicon new 
                #4 varbox new #2 borderbox
                    $0 $1 *hfill $0 $1 *vfil glue new 
                    ^^ S[  ]S ( MINOS ) :up tributton new 
                    $0 $1 *hfill $0 $1 *vfil glue new 
                  #3 habox new
                    ^^ S[  ]S ( MINOS ) :left tributton new 
                    $10 $1 *hfil $0 $1 *vfil glue new 
                    ^^ S[  ]S ( MINOS ) :right tributton new 
                  #3 habox new
                    $0 $1 *hfill $0 $1 *vfil glue new 
                    ^^ S[  ]S ( MINOS ) :down tributton new 
                    $0 $1 *hfill $0 $1 *vfil glue new 
                  #3 habox new
                #3 vabox new #2 borderbox
                  ^^ S[  ]S ( MINOS )  icon" icons/load" icon-but new 
                  ^^ S[  ]S ( MINOS )  icon" icons/save" icon-but new 
                  ^^ S[  ]S ( MINOS )  icon" icons/run" icon-but new 
                  ^^ S[  ]S ( MINOS )  icon" icons/mod" icon-but new 
                #4 vabox new #2 borderbox
                $0 $1 *hfil $0 $1 *vfilll glue new 
              #5 vabox new hfixbox 
                  1 1 viewport new  DS[ 
                    $10 $1 *hfil $10 $1 *vfil rule new 
                  #1 habox new panel ]DS ( MINOS ) 
                #1 habox new panel
              #1 habox new #2 borderbox
            #2 habox new
          #2 vabox new
                ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) X" horizontal" tbutton new 
                ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) X" active" tbutton new 
                ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) X" radio" tbutton new 
                ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) X" tabbing" tbutton new 
                ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) X" hfixbox" tbutton new 
                ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) X" vfixbox" tbutton new 
                ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) X" flipbox" tbutton new 
                ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) X" rzbox" tbutton new 
              #8 vabox new vfixbox 
                  0 -1 flipper X" Low" topindex new ^^bind (topindex-09)
                  0 0 flipper X" Detail" topindex new ^^bind (topindex-0A)
                #2 harbox new
                      ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) X" hskip" tbutton new 
                      ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) X" vskip" tbutton new 
                      ^^  0 T[  ][ ( MINOS )  ]T ( MINOS ) X" border" tbutton new 
                      $10 $1 *hfil $0 $1 *vfill glue new 
                    #4 vabox new
                  #1 habox new dup ^^ with C[ (topindex-09) ]C ( MINOS ) endwith 
                      ^^ #0 #10 SC[ drop ]SC ( MINOS )  TT" hskip" hscaler new 
                      ^^ #0 #10 SC[ drop ]SC ( MINOS )  TT" vskip" hscaler new 
                      ^^ #10 #20 SC[ drop ]SC ( MINOS )  TT" border" hscaler new 
                    #3 vabox new
                  #1 habox new flipbox  dup ^^ with C[ (topindex-0A) ]C ( MINOS ) endwith 
                #2 habox new $10  noborderbox  #2 borderbox
              #2 vabox new vfixbox 
            #2 vabox new panel #2 borderbox
            $0 $1 *hfil $0 $1 *vfilll glue new 
          #2 vabox new hfixbox 
        #2 habox new
          vrtsizer new 
            1 1 viewport new  DS[ 
              $0 $1 *hfil $0 $1 *vfil glue new 
                $0 $1 *hfil $0 $1 *vfil rule new 
                 icon" icons/minos" icon new 
                $0 $1 *hfil $0 $1 *vfil rule new 
              #3 habox new
              $0 $1 *hfil $0 $1 *vfil glue new 
            #3 vabox new ]DS ( MINOS ) 
            $0 $0 *hfil $68 $1 *vfil rule new 
          #2 habox new
        #2 vasbox new
      #2 vabox new
    ( [dumpend] ) ;
class;

: main
  designer open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
