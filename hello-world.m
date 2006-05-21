#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class hello
public:
  early widget
  early open
  early dialog
  early open-app
  | topindex ptr (topindex-00)
  | topindex ptr (topindex-01)
  | topindex ptr (topindex-02)
  | topindex ptr (topindex-03)
  | topindex ptr (topindex-04)
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Hello World" open-component ;
  : dialog   new DF[ 0 ]DF s" Hello World" open-dialog ;
  : open-app new DF[ 0 ]DF s" Hello World" open-application ;
class;

hello implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
            0 -1 flipper S" Schwäbisch" topindex new ^^bind (topindex-00)
            0 0 flipper S" English" topindex new ^^bind (topindex-01)
            0 0 flipper S" 中国" topindex new ^^bind (topindex-02)
            0 0 flipper S" Руссию" topindex new ^^bind (topindex-03)
            0 0 flipper S" 日本" topindex new ^^bind (topindex-04)
          &5 harbox new vfixbox 
              S" Hallöle Weltle!" text-label new 
            &1 habox new panel dup ^^ with C[ (topindex-00) ]C ( MINOS ) endwith 
              S" Hello World!" text-label new 
            &1 habox new flipbox  panel dup ^^ with C[ (topindex-01) ]C ( MINOS ) endwith 
              S" 世界公民好" text-label new 
            &1 habox new flipbox  panel dup ^^ with C[ (topindex-02) ]C ( MINOS ) endwith 
              S" сдравствуйте!" text-label new 
            &1 habox new flipbox  panel dup ^^ with C[ (topindex-03) ]C ( MINOS ) endwith 
              S" 今日は" text-label new 
            &1 habox new flipbox  panel dup ^^ with C[ (topindex-04) ]C ( MINOS ) endwith 
          &5 habox new $10  noborderbox  &2 borderbox
        &2 vabox new
          $10 $1 *hfill *hglue new 
          ^^ S[ close ]S ( MINOS ) S"   OK  " button new 
          $10 $1 *hfill *hglue new 
        &3 habox new
      &2 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  hello open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
