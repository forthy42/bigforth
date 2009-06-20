#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class hello
public:
  | topindex ptr (topindex-00)
  | topindex ptr (topindex-01)
  | topindex ptr (topindex-02)
  | topindex ptr (topindex-03)
  | topindex ptr (topindex-04)
  | topindex ptr (topindex-05)
  | topindex ptr (topindex-06)
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" Hello World" ;
class;

hello implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
            0 -1 flipper X" Schwäbisch" topindex new ^^bind (topindex-00)
            0 0 flipper X" English" topindex new ^^bind (topindex-01)
            0 0 flipper X" Français" topindex new ^^bind (topindex-02)
            0 0 flipper X" 中文" topindex new ^^bind (topindex-03)
            0 0 flipper X" Русский" topindex new ^^bind (topindex-04)
            0 0 flipper X" 日本语" topindex new ^^bind (topindex-05)
            0 0 flipper X" Tiếng Việt" topindex new ^^bind (topindex-06)
            topglue new 
          #8 harbox new vfixbox 
              X" Hallöle Weltle!" text-label new 
            #1 habox new panel dup ^^ with C[ (topindex-00) ]C ( MINOS ) endwith 
              X" Hello World!" text-label new 
            #1 habox new flipbox  panel dup ^^ with C[ (topindex-01) ]C ( MINOS ) endwith 
              X" 世界,你好!" text-label new 
            #1 habox new flipbox  panel dup ^^ with C[ (topindex-03) ]C ( MINOS ) endwith 
              X" Здравствуй, мир!" text-label new 
            #1 habox new flipbox  panel dup ^^ with C[ (topindex-04) ]C ( MINOS ) endwith 
              X" 今日は, 世界!" text-label new 
            #1 habox new flipbox  panel dup ^^ with C[ (topindex-05) ]C ( MINOS ) endwith 
              X" Bonjour le monde!" text-label new 
            #1 habox new flipbox  panel dup ^^ with C[ (topindex-02) ]C ( MINOS ) endwith 
              X" sin chào thế giới" text-label new 
            #1 habox new flipbox  panel dup ^^ with C[ (topindex-06) ]C ( MINOS ) endwith 
          #7 habox new $10  noborderbox  #2 borderbox
        #2 vabox new
          $10 $1 *hfill *hglue new 
          ^^ S[ close ]S ( MINOS ) X"   OK  " button new 
          $10 $1 *hfill *hglue new 
        #3 habox new
      #2 vabox new panel
    ( [dumpend] ) ;
class;

: main
  hello open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
