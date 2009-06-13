#! /usr/local/bin/xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class ampel
public:
  canvas ptr ampel-status
 ( [varstart] ) 1 var g#
1 var gy#
1 var y#
1 var o# ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Ampel Einstellung" ;
class;

ampel implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          CV[ $FF 0 0 rgb> backcolor clear ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $28 $1 *hfil $1 $1 *vfil canvas new 
            vrtsizer new 
            CV[ $FF $80 0 rgb> backcolor clear
h @ outer with o# c! ampel-status draw endwith ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $10 $1 *hfil $1 $1 *vfil canvas new 
          #2 vasbox new
            vrtsizer new 
            CV[ $FF $FF 0 rgb> backcolor clear
h @ outer with y# c! ampel-status draw endwith ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $10 $1 *hfil $1 $1 *vfil canvas new 
          #2 vasbox new
            vrtsizer new 
            CV[ $80 $FF 0 rgb> backcolor clear
h @ outer with gy# c! ampel-status draw endwith ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $10 $1 *hfil $1 $1 *vfil canvas new 
          #2 vasbox new
            vrtsizer new 
            CV[ 0 $FF 0 rgb> backcolor clear
h @ outer with g# c! ampel-status draw endwith ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $10 $1 *hfil $1 $1 *vfil canvas new 
          #2 vasbox new
        #5 vabox new
        CV[ 2 284 steps decimal
0 outer with o# c@ y# c@ gy# c@ g# c@ endwith
3 backcolor clear 0 1 textpos
4 0 DO  4 I - 7 * swap >r 0 swap 254 + r@ - home!
        r@ 0 <# #S #> text r> + LOOP
drop ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) 2drop 2drop ]CK ( MINOS ) $20 $0 *hpix $11C $0 *vpix canvas new  ^^bind ampel-status
      #2 habox new panel
    ( [dumpend] ) ;
class;

: main
  ampel open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
