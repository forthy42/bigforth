#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class adjust
public:
  early widget
  early open
  early open-app
  infotextfield ptr path#
  infotextfield ptr id#
  button ptr path-ok
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ path-ok self ]DF s" Adjust Configuration" open-component ;
  : open-app new DF[ path-ok self ]DF s" Adjust Configuration" open-application ;
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
        ^^ ST[  ]ST ( MINOS ) T" " S" Pathes" infotextfield new  ^^bind path#
        ^^ ST[  ]ST ( MINOS ) T" py" S" ID" infotextfield new  ^^bind id#
          $10 $1 *hfill $10 $1 *vfil glue new 
          ^^ S[ id# get path# get s" xbigforth.cnf" adjust-path-id
id# get path# get  s" bigforth.cnf" adjust-path-id
close ]S ( MINOS ) S" OK" button new  ^^bind path-ok
          $10 $1 *hfil $10 $1 *vfil glue new 
          ^^ S[ close ]S ( MINOS ) S" Cancel" button new 
          $10 $1 *hfill $10 $1 *vfil glue new 
        &5 hatbox new
      &3 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  adjust open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
