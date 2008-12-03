#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class tip1
public:
  text-label ptr tip-text
  button ptr i-know
 ( [varstart] ) cell var tip-file ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Tip 1" ;
class;

tip1 implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
               icon" icons/INFO" icon new 
              $0 $1 *hfil $0 $1 *vfil glue new 
            #2 vabox new
              X" Did you know?" text-label new 
              X" Click on ``next tip'' to get a tip" text-label new  ^^bind tip-text
            #2 vabox new #1 vskips
          #2 habox new #1 hskips
            ^^ S[ close ]S ( MINOS ) X" I know" button new  ^^bind i-know
            ^^ S[ [ also DOS ] tip-file @ 0=
IF  s" tips.txt" r/o open-file throw tip-file !  THEN
BEGIN  scratch $100 tip-file @ read-line throw  0= WHILE
       drop 0. tip-file @ reposition-file throw  REPEAT
scratch swap tip-text assign [ previous ] ]S ( MINOS ) X" Next Tip" button new 
            $0 $1 *hfilll $0 $1 *vfilll glue new 
          #3 hatbox new #1 hskips
        #2 vabox new panel
        $0 $1 *hfil $0 $1 *vfil glue new 
      #2 vabox new
    ( [dumpend] ) ;
class;

: main
  tip1 open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
