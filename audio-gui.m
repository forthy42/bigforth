#! ./xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

include audio-gui.fs
component class audio-gui
public:
  early widget
  early open
  early dialog
  early open-app
  topindex ptr bridges
  topindex ptr vindex
  vscaler ptr master
  vscaler ptr l0
  vscaler ptr r0
  vscaler ptr l1
  vscaler ptr r1
  vscaler ptr l2
  vscaler ptr r2
  vscaler ptr l3
  vscaler ptr r3
  tbutton ptr ball
  tbutton ptr bl0
  tbutton ptr br0
  tbutton ptr bl1
  tbutton ptr br1
  tbutton ptr bl2
  tbutton ptr br2
  tbutton ptr bl3
  tbutton ptr br3
 ( [varstart] ) 9 cells Var bridgestates ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Audio GUI" open-component ;
  : dialog   new DF[ 0 ]DF s" Audio GUI" open-dialog ;
  : open-app new DF[ 0 ]DF s" Audio GUI" open-application ;
class;

audio-gui implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
            0 -1 flipper S" Bridges" topindex new ^^bind bridges
            0 0 flipper S" Volume" topindex new ^^bind vindex
            topglue new 
          &3 harbox new vfixbox 
                ^^ &0 &255 SC[ ( pos -- ) $B7 + 8 lshift $E000 spiw! ]SC ( MINOS ) vscaler new  ^^bind master -&183 SC# 
              &1 habox new
              $A $0 *hfil $100 $1 *vfil glue new 
                ^^ &0 &255 SC[ ( pos -- ) $B7 + 8 lshift r0 get nip nip $B7 + $FF and or $A000 spiw! ]SC ( MINOS ) vscaler new  ^^bind l0 -&183 SC# 
                ^^ &0 &255 SC[ ( pos -- ) l0 get nip nip $B7 + 8 lshift swap $B7 + $FF and or $A000 spiw! ]SC ( MINOS ) vscaler new  ^^bind r0 -&183 SC# 
                ^^ &0 &255 SC[ ( pos -- ) $B7 + 8 lshift r1 get nip nip $B7 + $FF and or $A800 spiw! ]SC ( MINOS ) vscaler new  ^^bind l1 -&183 SC# 
                ^^ &0 &255 SC[ ( pos -- ) l1 get nip nip $B7 + 8 lshift swap $B7 + $FF and or $A800 spiw! ]SC ( MINOS ) vscaler new  ^^bind r1 -&183 SC# 
                ^^ &0 &255 SC[ ( pos -- ) $B7 + 8 lshift r2 get nip nip $B7 + $FF and or $B000 spiw! ]SC ( MINOS ) vscaler new  ^^bind l2 -&183 SC# 
                ^^ &0 &255 SC[ ( pos -- ) l2 get nip nip $B7 + 8 lshift swap $B7 + $FF and or $B000 spiw! ]SC ( MINOS ) vscaler new  ^^bind r2 -&183 SC# 
                ^^ &0 &255 SC[ ( pos -- ) $B7 + 8 lshift r3 get nip nip $B7 + $FF and or $B800 spiw! ]SC ( MINOS ) vscaler new  ^^bind l3 -&183 SC# 
                ^^ &0 &255 SC[ ( pos -- ) l3 get nip nip $B7 + 8 lshift swap $B7 + $FF and or $B800 spiw! ]SC ( MINOS ) vscaler new  ^^bind r3 -&183 SC# 
              &8 habox new
            &3 habox new flipbox  &1 vskips dup ^^ with C[ vindex ]C ( MINOS ) endwith 
                ^^ TV[ bridgestates ]T[ ( MINOS ) bridgestates @ 9 1 DO dup bridgestates I cells + ! LOOP drop bl0 draw bl1 draw bl2 draw bl3 draw br0 draw br1 draw br2 draw br3 draw ]TV ( MINOS ) S" Bridges" tbutton new  ^^bind ball
                $10 $1 *hfil $10 $1 *vfil glue new 
                  ^^ TV[ bridgestates 1 cells + ]T[ ( MINOS ) 0 9 1 DO bridgestates Ith or LOOP bridgestates ! ball draw ]TV ( MINOS ) S" Bridge l0" tbutton new  ^^bind bl0
                  ^^ TV[ bridgestates 2 cells + ]T[ ( MINOS ) 0 9 1 DO bridgestates Ith or LOOP bridgestates ! ball draw ]TV ( MINOS ) S" Bridge r0" tbutton new  ^^bind br0
                &2 hatbox new
                  ^^ TV[ bridgestates 3 cells + ]T[ ( MINOS ) 0 9 1 DO bridgestates Ith or LOOP bridgestates ! ball draw ]TV ( MINOS ) S" Bridge l1" tbutton new  ^^bind bl1
                  ^^ TV[ bridgestates 4 cells + ]T[ ( MINOS ) 0 9 1 DO bridgestates Ith or LOOP bridgestates ! ball draw ]TV ( MINOS ) S" Bridge r1" tbutton new  ^^bind br1
                &2 hatbox new
                  ^^ TV[ bridgestates 5 cells + ]T[ ( MINOS ) 0 9 1 DO bridgestates Ith or LOOP bridgestates ! ball draw ]TV ( MINOS ) S" Bridge l2" tbutton new  ^^bind bl2
                  ^^ TV[ bridgestates 6 cells + ]T[ ( MINOS ) 0 9 1 DO bridgestates Ith or LOOP bridgestates ! ball draw ]TV ( MINOS ) S" Bridge r2" tbutton new  ^^bind br2
                &2 hatbox new
                  ^^ TV[ bridgestates 7 cells + ]T[ ( MINOS ) 0 9 1 DO bridgestates Ith or LOOP bridgestates ! ball draw ]TV ( MINOS ) S" Bridge l3" tbutton new  ^^bind bl3
                  ^^ TV[ bridgestates 8 cells + ]T[ ( MINOS ) 0 9 1 DO bridgestates Ith or LOOP bridgestates ! ball draw ]TV ( MINOS ) S" Bridge r3" tbutton new  ^^bind br3
                &2 hatbox new
                $10 $1 *hfil $10 $1 *vfill glue new 
              &7 vabox new
            &1 habox new panel dup ^^ with C[ bridges ]C ( MINOS ) endwith 
          &2 habox new $10  noborderbox  &2 borderbox
        &2 vabox new
      &1 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  audio-gui open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
