#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class quit
public:
  button ptr yes-button
  button ptr no-button
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF X" Quit?" ;
class;

quit implements
 ( [methodstart] ) : show
  screen w @ hglue drop - 0 dpy repos
  super show ; ( [methodend] ) 
  : widget  ( [dumpstart] )
        X" Do you really want to leave?" text-label new 
          $10 $1 *hfill $10 $1 *vfil glue new 
          ^^ S[ ." Yes" cr close ]S ( MINOS ) X" Yes" button new  ^^bind yes-button
          ^^ S[ ." No" cr close ]S ( MINOS ) X" No" button new  ^^bind no-button
          $10 $1 *hfill $10 $1 *vfil glue new 
        #4 hatbox new #2 hskips
      #2 vabox new panel
    ( [dumpend] ) ;
class;

: main
  quit open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
