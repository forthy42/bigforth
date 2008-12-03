#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class set-colors
public:
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Set Colors" ;
class;

set-colors implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ #192 #256 SC[ to red re-color dpy draw ]SC ( MINOS )  TT" red" hscaler new 
        ^^ #192 #256 SC[ to green re-color dpy draw ]SC ( MINOS )  TT" green" hscaler new 
        ^^ #192 #256 SC[ to blue re-color dpy draw ]SC ( MINOS )  TT" blue" hscaler new 
        ^^ #92 #50 SC[ to contrast re-color dpy draw ]SC ( MINOS )  TT" Contrast" hscaler new  #50 SC# 
        $120 $1 *hfilll $0 $0 *vfil rule new 
      #5 vabox new
    ( [dumpend] ) ;
class;

: main
  set-colors open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
