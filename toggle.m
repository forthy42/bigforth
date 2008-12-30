#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class toggle-actor
public:
  | topindex ptr (topindex-00)
  | topindex ptr (topindex-01)
  | topindex ptr (topindex-02)
  | topindex ptr (topindex-03)
 ( [varstart] )  ( [varend] ) 
how:
  : params   DF[ 0 ]DF s" Toggle Actors" ;
class;

toggle-actor implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
            0 -1 flipper X" Toggle" topindex new ^^bind (topindex-00)
            0 0 flipper X" Toggle-Var" topindex new ^^bind (topindex-01)
            0 0 flipper X" Toggle-Num" topindex new ^^bind (topindex-02)
            0 0 flipper X" Toggle-State" topindex new ^^bind (topindex-03)
            topglue new 
          #5 harbox new
                T" " ^^ ST[  ]ST ( MINOS ) X" On:" infotextfield new 
                T" " ^^ ST[  ]ST ( MINOS ) X" Off:" infotextfield new 
              #2 varbox new panel
            #1 habox new dup ^^ with C[ (topindex-00) ]C ( MINOS ) endwith 
                T" " ^^ ST[  ]ST ( MINOS ) X" Var:" infotextfield new 
                $10 $1 *hfil $1 $1 *vfill glue new 
              #2 vabox new panel
            #1 habox new flipbox  dup ^^ with C[ (topindex-01) ]C ( MINOS ) endwith 
                T" " ^^ ST[  ]ST ( MINOS ) X" Var:" infotextfield new 
                #0. ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) X" Num:" infotextfield new 
              #2 varbox new panel
            #1 habox new flipbox  dup ^^ with C[ (topindex-02) ]C ( MINOS ) endwith 
                T" " ^^ ST[  ]ST ( MINOS ) X" Fetch:" infotextfield new 
                T" " ^^ ST[  ]ST ( MINOS ) X" Store:" infotextfield new 
              #2 varbox new panel
            #1 habox new flipbox  dup ^^ with C[ (topindex-03) ]C ( MINOS ) endwith 
          #4 habox new $10  noborderbox  #2 borderbox
        #2 vabox new
      #1 habox new
    ( [dumpend] ) ;
class;

: main
  toggle-actor open-app
  event-loop bye ;
script? [IF]  main  [THEN]
previous previous previous
