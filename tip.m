#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class tip1
public:
  early widget
  early open
  early open-app
  text-label ptr tip-text
  button ptr i-know
 ( [varstart] ) cell var tip-file ( [varend] ) 
how:
  : open     new DF[ i-know self ]DF s" Tip 1" open-component ;
  : open-app new DF[ i-know self ]DF s" Tip 1" open-application ;
class;

tip1 implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
               icon" icons/INFO" icon new 
              $0 $1 *hfil $0 $1 *vfil glue new 
            &2 vabox new
              S" Did you know?" text-label new 
              S" Click on ``next tip'' to get a tip" text-label new  ^^bind tip-text
            &2 vabox new &1 vskips
          &2 habox new &1 hskips
            ^^ S[ close ]S ( MINOS ) S" I know" button new  ^^bind i-know
            ^^ S[ [ also DOS ] tip-file @ 0=
IF  s" tips.txt" r/o open-file throw tip-file !  THEN
BEGIN  scratch $100 tip-file @ read-line throw  0= WHILE
       drop 0. tip-file @ reposition-file throw  REPEAT
scratch swap tip-text assign [ previous ] ]S ( MINOS ) S" Next Tip" button new 
            $0 $1 *hfilll $0 $1 *vfilll glue new 
          &3 hatbox new &1 hskips
        &2 vabox new panel
        $0 $1 *hfil $0 $1 *vfil glue new 
      &2 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  tip1 open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
