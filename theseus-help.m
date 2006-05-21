#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class minos-about
public:
  early widget
  early open
  early open-app
  button ptr theseus-ok
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ theseus-ok self ]DF s" Minos Help" open-component ;
  : open-app new DF[ theseus-ok self ]DF s" Minos Help" open-application ;
class;

minos-about implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
              $0 $1 *hfill $0 $1 *vfill glue new 
               icon" icons/minos.png" icon new 
              $0 $1 *hfill $0 $1 *vfill glue new 
            &3 habox new
            S" Theseus 10sep2000" text-label new 
            S" based on MINOS" text-label new 
            S" (c) 1997-2000 by Bernd Paysan" text-label new 
              S" Theseus is available under " text-label new 
              ^^ S[ gpl-about new 0 s" About GPL" open-component ]S ( MINOS ) S" GPL" button new 
              $10 $1 *hfilll $10 $1 *vfil glue new 
            &3 habox new
          &5 vabox new &2 borderbox
        &1 habox new
            $10 $1 *hfill $10 $1 *vfill glue new 
            ^^ S[ close ]S ( MINOS ) S"  OK " button new  ^^bind theseus-ok
            $10 $1 *hfill $10 $1 *vfill glue new 
          &3 habox new &1 vskips
        &1 vabox new
      &2 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

previous previous previous
