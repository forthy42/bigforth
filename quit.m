#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class quit
public:
  early widget
  early open
  early open-app
  button ptr yes-button
  button ptr no-button
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ yes-button self ]DF s" Quit?" open-component ;
  : open-app new DF[ yes-button self ]DF s" Quit?" open-application ;
class;

quit implements
 ( [methodstart] ) : show
  screen w @ hglue drop - 0 dpy repos
  super show ; ( [methodend] ) 
  : widget  ( [dumpstart] )
        S" Do you really want to leave?" text-label new 
          $10 $1 *hfill $10 $1 *vfil glue new 
          ^^ S[ ." Yes" cr close ]S ( MINOS ) S" Yes" button new  ^^bind yes-button
          ^^ S[ ." No" cr close ]S ( MINOS ) S" No" button new  ^^bind no-button
          $10 $1 *hfill $10 $1 *vfil glue new 
        &4 hatbox new &2 hskips
      &2 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  quit open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
