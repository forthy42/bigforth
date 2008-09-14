#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class abacus
public:
  early widget
  early open
  early dialog
  early open-app
  infotextfield ptr num
 ( [varstart] ) cell var v0    cell var i0
cell var v1    cell var i1
cell var v2    cell var i2
cell var v3    cell var i3
cell var v4    cell var i4
cell var v5    cell var i5
cell var v6    cell var i6
cell var v7    cell var i7
cell var v8    cell var i8
cell var v9    cell var i9
method re-calc ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Abacus" open-component ;
  : dialog   new DF[ 0 ]DF s" Abacus" open-dialog ;
  : open-app new DF[ 0 ]DF s" Abacus" open-application ;
class;

component class abacus-comp
public:
  early widget
  early open
  early dialog
  early open-app
 ( [varstart] ) cell var *v
cell var *i
abacus ptr outer ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" No Title" open-component ;
  : dialog   new DF[ 0 ]DF s" No Title" open-dialog ;
  : open-app new DF[ 0 ]DF s" No Title" open-application ;
class;

abacus-comp implements
 ( [methodstart] ) : re-calc  outer re-calc ;
: assign  *i ! *v ! bind outer ; ( [methodend] ) 
  : widget  ( [dumpstart] )
          ^^ TN[ 0 *v @ ]T[ ( MINOS ) re-calc  ]TN ( MINOS )  2icon" icons/blue-dot"icons/gold-hwire" toggleicon new 
          ^^ TN[ 5 *v @ ]T[ ( MINOS ) re-calc  ]TN ( MINOS )  2icon" icons/blue-dot"icons/gold-hwire" toggleicon new 
          ^^ TN[ &10 *v @ ]T[ ( MINOS ) re-calc  ]TN ( MINOS )  2icon" icons/blue-dot"icons/gold-hwire" toggleicon new 
        #3 harbox new
          ^^ TN[ 0 *i @ ]T[ ( MINOS ) re-calc  ]TN ( MINOS )  2icon" icons/red-dot"icons/gold-hwire" toggleicon new 
          ^^ TN[ 1 *i @ ]T[ ( MINOS ) re-calc  ]TN ( MINOS )  2icon" icons/red-dot"icons/gold-hwire" toggleicon new 
          ^^ TN[ 2 *i @ ]T[ ( MINOS ) re-calc  ]TN ( MINOS )  2icon" icons/red-dot"icons/gold-hwire" toggleicon new 
          ^^ TN[ 3 *i @ ]T[ ( MINOS ) re-calc  ]TN ( MINOS )  2icon" icons/red-dot"icons/gold-hwire" toggleicon new 
          ^^ TN[ 4 *i @ ]T[ ( MINOS ) re-calc  ]TN ( MINOS )  2icon" icons/red-dot"icons/gold-hwire" toggleicon new 
          ^^ TN[ 5 *i @ ]T[ ( MINOS ) re-calc  ]TN ( MINOS )  2icon" icons/red-dot"icons/gold-hwire" toggleicon new 
        #6 harbox new
      #2 habox new #1 hskips
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 :: init ;
class;

abacus implements
 ( [methodstart] ) : re-calc
  0 v0 &20 cells bounds ?DO  I @ 0< or  cell +LOOP  ?EXIT
  0. v0 
  &10 0 DO  >r &10. d* r@ 2@ + m+ r> 2 cells +  LOOP drop
  num assign ;
: re-number  num get
  v0 v9 DO
      &10 ud/mod rot 5 /mod 5 * I ! I cell+ !  -2 cells +LOOP
  2drop draw ; ( [methodend] ) 
  : widget  ( [dumpstart] )
        #0, ]N ( MINOS ) ^^ SN[ re-number ]SN ( MINOS ) X" #" infotextfield new  ^^bind num
          ^^ CP[ v9 i9 ]CP ( MINOS ) abacus-comp new 
          ^^ CP[ v8 i8 ]CP ( MINOS ) abacus-comp new 
          ^^ CP[ v7 i7 ]CP ( MINOS ) abacus-comp new 
          ^^ CP[ v6 i6 ]CP ( MINOS ) abacus-comp new 
          ^^ CP[ v5 i5 ]CP ( MINOS ) abacus-comp new 
          ^^ CP[ v4 i4 ]CP ( MINOS ) abacus-comp new 
          ^^ CP[ v3 i3 ]CP ( MINOS ) abacus-comp new 
          ^^ CP[ v2 i2 ]CP ( MINOS ) abacus-comp new 
          ^^ CP[ v1 i1 ]CP ( MINOS ) abacus-comp new 
          ^^ CP[ v0 i0 ]CP ( MINOS ) abacus-comp new 
        #10 vabox new
      #2 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 :: init ;
class;

: main
  abacus open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
