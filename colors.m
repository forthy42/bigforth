#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class set-colors
public:
  early widget
  early open
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Set Colors" open-component ;
  : open-app new DF[ 0 ]DF s" Set Colors" open-application ;
class;

set-colors implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ &192 &256 SC[ to red re-color ]SC ( MINOS )  TT" red" hscaler new 
        ^^ &192 &256 SC[ to green re-color ]SC ( MINOS )  TT" green" hscaler new 
        ^^ &192 &256 SC[ to blue re-color ]SC ( MINOS )  TT" blue" hscaler new 
        ^^ &92 &50 SC[ to contrast re-color ]SC ( MINOS )  TT" Contrast" hscaler new  &50 SC# 
        $120 $1 *hfilll $0 $0 *vfil rule new 
      &5 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  set-colors open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
