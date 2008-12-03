#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class adjust
public:
  infotextfield ptr path#
  infotextfield ptr id#
  button ptr path-ok
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ path-ok self ]DF s" Adjust Configuration" ;
class;

include adjust-path.fs
adjust implements
 ( [methodstart] ) : show
  pathes count '; -skip path# assign
  screen xywh 2swap 2drop p2/
  dpy self window with  xywh resize
      xywh  2swap 2drop p2/ p-
      repos endwith
  super show ; ( [methodend] ) 
  : widget  ( [dumpstart] )
        T" " ^^ ST[  ]ST ( MINOS ) X" Pathes" infotextfield new  ^^bind path#
        T" py" ^^ ST[  ]ST ( MINOS ) X" ID" infotextfield new  ^^bind id#
          $10 $1 *hfill $10 $1 *vfil glue new 
          ^^ S[ id# get path# get s" xbigforth.ini" adjust-path-id
id# get path# get  s" bigforth.ini" adjust-path-id
close ]S ( MINOS ) X" OK" button new  ^^bind path-ok
          $10 $1 *hfil $10 $1 *vfil glue new 
          ^^ S[ close ]S ( MINOS ) X" Cancel" button new 
          $10 $1 *hfill $10 $1 *vfil glue new 
        #5 hatbox new
      #3 vabox new panel
    ( [dumpend] ) ;
class;

: main
  adjust open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
