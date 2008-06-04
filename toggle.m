#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class toggle-actor
public:
  early widget
  early open
  early open-app
  | topindex ptr (topindex-00)
  | topindex ptr (topindex-01)
  | topindex ptr (topindex-02)
  | topindex ptr (topindex-03)
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Toggle Actors" open-component ;
  : open-app new DF[ 0 ]DF s" Toggle Actors" open-application ;
class;

toggle-actor implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
            0 -1 flipper S" Toggle" topindex new ^^bind (topindex-00)
            0 0 flipper S" Toggle-Var" topindex new ^^bind (topindex-01)
            0 0 flipper S" Toggle-Num" topindex new ^^bind (topindex-02)
            0 0 flipper S" Toggle-State" topindex new ^^bind (topindex-03)
            topglue new 
          &5 harbox new
                ^^ ST[  ]ST ( MINOS ) S" " S" On:" infotextfield new 
                ^^ ST[  ]ST ( MINOS ) S" " S" Off:" infotextfield new 
              &2 varbox new panel
            &1 habox new dup ^^ with C[ (topindex-00) ]C ( MINOS ) endwith 
                ^^ ST[  ]ST ( MINOS ) S" " S" Var:" infotextfield new 
                $10 $1 *hfil $1 $1 *vfill glue new 
              &2 vabox new panel
            &1 habox new flipbox  dup ^^ with C[ (topindex-01) ]C ( MINOS ) endwith 
                ^^ ST[  ]ST ( MINOS ) S" " S" Var:" infotextfield new 
                &0. ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) S" Num:" infotextfield new 
              &2 varbox new panel
            &1 habox new flipbox  dup ^^ with C[ (topindex-02) ]C ( MINOS ) endwith 
                ^^ ST[  ]ST ( MINOS ) S" " S" Fetch:" infotextfield new 
                ^^ ST[  ]ST ( MINOS ) S" " S" Store:" infotextfield new 
              &2 varbox new panel
            &1 habox new flipbox  dup ^^ with C[ (topindex-03) ]C ( MINOS ) endwith 
          &4 habox new $10  noborderbox  &2 borderbox
        &2 vabox new
      &1 habox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  toggle-actor open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
