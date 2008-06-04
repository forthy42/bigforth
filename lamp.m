#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class lamp-field
public:
  early widget
  early open
  early open-app
  canvas ptr field
  infotextfield ptr which
 ( [varstart] ) cell var red
cell var green
cell var blue
64 var lamps ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Lamp Field" open-component ;
  : open-app new DF[ 0 ]DF s" Lamp Field" open-application ;
class;

lamp-field implements
 ( [methodstart] ) : draw-lamps canvas with
  32 32 steps 0 0 0 rgb> backcolor clear
  8 0 DO
      8 0 DO
          i 4* 1+ j 4* 3+ home!
          i j 8* + outer with lamps endwith + c@ fillcolor
          path 0 2 to 2 0 to 0 -2 to -2 0 to fill
      LOOP
  LOOP endwith ;
: lamp! ( r g b i j -- )
  7 umin 8* swap 7 umin + lamps + >r rgb> r> c! ;
: an ( i j -- )  >r >r
  red @  green @  blue @ r> r> lamp! ;
: aus ( i j -- )  >r >r 0 0 0 r> r> lamp! ; ( [methodend] ) 
  : widget  ( [dumpstart] )
        CV[ ^ draw-lamps ]CV ( MINOS ) ^^ CK[ 2drop 2drop  ]CK ( MINOS ) $100 $1 *hfil $100 $1 *vfil canvas new  ^^bind field
          &0. ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) S" Welchen" infotextfield new  ^^bind which
          ^^ S[ which get drop 8 /mod an field draw ]S ( MINOS ) S" An" button new 
          ^^ S[ which get drop 8 /mod aus field draw ]S ( MINOS ) S" Aus" button new 
          ^^ S[ close ]S ( MINOS ) S" Ende" button new 
        &4 habox new vfixbox  &1 hskips
          ^^ &255 &255 SC[ red ! ]SC ( MINOS )  TT" Rot" hscaler new 
          ^^ &255 &255 SC[ green ! ]SC ( MINOS )  TT" Grün" hscaler new 
          ^^ &255 &255 SC[ blue ! ]SC ( MINOS )  TT" Blau" hscaler new 
        &3 vatbox new
      &3 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  lamp-field open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
