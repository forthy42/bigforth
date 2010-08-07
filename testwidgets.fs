\ test testwin                                         18sep96py
\ traceall

minos also forth
[IFUNDEF] noop sub-viewport
  \needs xconst | import xconst
  xconst also x11 also forth
  include viewport.fs
  ' sub-viewport alias viewport
  onlyforth minos also forth
[THEN]

screen self menu-window : win0

\ test testwin                                         24sep96py
\ ' win0 Alias view0
\ backing : view0
1 1 viewport : view0
\ dbuf0 self doublebuffer : view0
\ view0 noback off
0 ' noop edit-action new s" " s" Text:" infotextfield : tfield0
\ 0 edit-action new s" " (textfield : tfield0
: .button  button text @ @+ swap tfield0 assign ;
: .pos base push decimal dup 0 <<# #s #> tfield0 assign #>> ;

0 :noname  s" Test 0" tfield0 assign ; simple new s" Menu 0" menu-entry : menu0
0 :noname  s" Test 1" tfield0 assign ; simple new s" Menu 1" menu-entry : menu1
0 :noname  s" Test 2" tfield0 assign ; simple new s" Menu 2" menu-entry : menu2

menu0 self menu1 self menu2 self 3 vabox : test-menu0
2 test-menu0 borderw c!

test-menu0 self s" Sub-Menu" sub-menu : sub0

0 :noname s" Test 0" tfield0 assign ; simple new s" Menu 0" menu-entry : menu0a
0 :noname s" Test 1" tfield0 assign ; simple new s" Menu 1" menu-entry : menu1a
0 :noname s" Test 2" tfield0 assign ; simple new s" Menu 2" menu-entry : menu2a

0 ' wood          simple new s" Wood"    menu-entry : menu-wood
0 ' water         simple new s" Water"   menu-entry : menu-water
0 ' water1        simple new s" Caustics" menu-entry : menu-water1
0 ' marble        simple new s" Marble"  menu-entry : menu-marble
0 ' cracle        simple new s" Cracle"  menu-entry : menu-cracle
0 ' paper         simple new s" Paper"   menu-entry : menu-paper
0 ' mono          simple new s" Mono"    menu-entry : menu-mono
0 ' gray-colors   simple new s" Gray"    menu-entry : menu-gray
0 ' red-colors    simple new s" Red"     menu-entry : menu-red
0 ' blue-colors   simple new s" Blue"    menu-entry : menu-blue
0 ' bisque-colors simple new s" Bisque"  menu-entry : menu-bisque

menu0a self menu1a self sub0 self menu2a self 4 vabox : test-menu1
2 test-menu1 borderw c!

menu-wood self menu-water self menu-water1 self menu-marble self
menu-cracle self menu-paper self menu-mono self
hline menu-gray self menu-red self menu-blue self menu-bisque self
&12 vabox : test-menu2
2 test-menu2 borderw c!

test-menu0 self s" bigFORTH" menu-title : title0
test-menu1 self s" File" menu-title : title1
test-menu2 self s" Options" menu-title : title2

title0 self title1 self title2 self 2fill 4 hbox : menu-title0

menu-title0 self vfixbox drop

0 ' .button simple new s" Testbutton" lbutton : button0
0 ' .button simple new s" Button 1"   button : button1
0 ' .button simple new s" But 2"      button : button2
0 ' .button simple new s" Button 3"   button : button3
0 ' .button simple new s" Button 4"   button : button4
\ 0 ' .button simple new s" Button 5"   button : button5

ficon: printer-icon ./icons/printer"
0 ' .button simple new printer-icon s" Printer-Icon" big-icon : button5

[IFUNDEF] win32s
doublebuffer : dbuf1
[ELSE]
' view0 alias dbuf1
[THEN]

Variable cv1task

: xto ( x y n1 n2 -- )
    2dup >=
    IF    - $100 min tuck * 8 >> >r * 8 >> r> canvas to
    ELSE  2drop 2drop  THEN ;

: draw-cv1 ( o -- flag )
    canvas with
        $300 $400 steps
        0 1 textpos
        timer@ $E << $1000 um* nip >r
        clear
         2 linewidth
         $00 $00 $FF rgb> drawcolor
         $FF $00 $00 rgb> fillcolor
         $080  $380 home!  path
         $000  $200 r@ $100 xto
         $100  $100 r@ $200 xto
         $100 -$100 r@ $300 xto
        -$200  $000 r@ $400 xto
         $200 -$200 r@ $500 xto
         $000  $200 r@ $600 xto
        -$200 -$200 r@ $700 xto
         $200  $000 r@ $800 xto
        r@ $900 >= IF  fill  THEN  stroke
        r@ $000 $200 within IF s" wer" THEN
        r@ $200 $300 within IF s" malt" THEN
        r@ $300 $400 within IF s" das" THEN
        r@ $400 $500 within IF s" Haus" THEN
        r@ $500 $600 within IF s" vom" THEN
        r@ $600 $700 within IF s" Ni-" THEN
        r@ $700 $800 within IF s" Niko-" THEN
        r@ $800 >= IF s" Nikolaus" THEN  text
        0 linewidth
        r> $900 >=
    endwith ;

:noname
    cv1task @ IF  draw-cv1 drop  EXIT  THEN
    1 $1000 dup NewTask dup cv1task !  pass
    canvas with
        shown on
        BEGIN  ^ draw-cv1 dpy sync
            IF
                BEGIN
                    -1 &30 idle
                    timer@ $E << &1000 um* nip $100 <
                UNTIL
            ELSE  -1 &30 idle  THEN
            shown @ 0=  UNTIL
    endwith cv1task off ;

0 :noname 2drop 2drop ; simple new $41 $2 *fill $61 $3 *fill canvas : canvas1

canvas1 self dbuf1 assign

Forward show-it

0 :noname s" Bisque" show-it bisque-colors ; simple new
                                        s" Bisque" menu-entry : info0a
0 :noname s" Gray" show-it gray-colors ; simple new
                                        s" Gray" menu-entry : info0b
0 :noname s" Red" show-it red-colors ; simple new
                                        s" Red" menu-entry : info0c
0 :noname s" Blue" show-it blue-colors ; simple new
                                        s" Blue" menu-entry : info0d
0 :noname s" Paper" show-it paper ;        simple new s" Paper" menu-entry : info1
0 :noname s" Wood" show-it wood ;     simple new s" Wood" menu-entry : info2
0 :noname s" Water" show-it water ;        simple new s" Water" menu-entry : info3
0 :noname s" Caustics" show-it water1 ;     simple new s" Caustics" menu-entry : info4
0 :noname s" Red Marble" show-it marble ; simple new s" Red Marble" menu-entry : info5
0 :noname s" Cracle" show-it cracle ; simple new s" Cracle" menu-entry : info6
0 :noname s" Mono" show-it mono ; simple new s" Mono" menu-entry : info7

info0a self info0b self info0c self info0d self hline
info1 self info2 self info3 self info4 self info5 self info6 self info7 self
&12 vabox : info-menu0
2 info-menu0 borderw c!

info-menu0 self s" Coolness:" info-menu : info-m0

: show-it  ( addr u -- ) info-m0 assign ;
\ ' (show-it IS show-it

button1 self 2skip button2 self 2fill button3 self
   5 hatbox : box0
   2 box0 borderw c!
   
Variable pos    Variable pos1   Variable pos2   Variable pos3
: par!  .pos pos ! ;            : slpar  $40 $10 pos @ ;
: par1!  .pos pos1 ! ;          : slpar1 $40 $10 pos1 @ ;
: par2!  .pos pos2 ! ;          : slpar2 &10000 pos2 @ ;
: par3!  pos3 ! ;               : slpar3 &1000 pos3 @ ;
0 ' par!  ' slpar  toggle-state new hslider : slider0
0 ' par1! ' slpar1 toggle-state new hslider : slider1
0 ' par3! ' slpar3 toggle-state new hscaler : scaler0
-&500 scaler0 offset !
slider0 self slider1 self 2 habox new
scaler0 self 2 vabox : box1
0 ' par2! ' slpar2 toggle-state new vscaler : slider2

2skip
   2skip
      0 ' .button simple new s" Sub-Topic A.0" lbutton new
      0 ' .button simple new s" Sub-Topic A.1" lbutton new
      0 ' .button simple new s" Sub-Topic A.2" lbutton new
   3 vabox new tuck 2 habox new
   ( vflip new dup ) swap flipbox
   combined ' flip  simple new s" Topic A" lbutton new swap
   0 ' .button simple new s" Topic B" lbutton new
   0 ' .button simple new s" Topic C" lbutton new
4 vabox new flipbox
tuck 2 habox : fbox0

\ boxl self vflip : fbox0

\ fbox0 self fbox0 ' flip  S" Flip It!" lbutton : fbutton
0 hbox ' +flip hbox ' -flip toggle new s" -" t" +" togglebutton : fbutton0
0 ' .button simple new s" Flip It!" lbutton : fbutton1
fbutton0 self 1 vabox new hfixbox
fbutton1 self 2 habox : fbutton

\ variants

minos
0 ' .button simple new s" Index0" button : bvar0a
0 ' .button simple new s" Klick mal" button : bvar0b
bvar0a self bvar0b self 2fill 3 habox : var0l  var0l self panel drop

0 ' .button simple new s" Index 1" button : bvar1a
0 ' .button simple new s" Kuck mal" button : bvar1b
bvar1a self bvar1b self 2fill 3 hatbox : var1l  var1l self panel drop

0 ' .button simple new s" Kuck" button : bvar2a
0 ' .button simple new s" mal" button : bvar2b
0 ' .button simple new s" wer" button : bvar2c
0 ' .button simple new s" da" button : bvar2d
0 ' .button simple new s" spricht" button : bvar2e
bvar2a self bvar2b self bvar2c self bvar2d self bvar2e self
5 habox : var2l  var2l self panel drop

forth

: -flip   vbox attribs c@ :flip or         vbox attribs c! ;

var0l self 1 vabox new dup
    -1 vabox ' +flip ' -flip toggle new s" Index 0" topindex : tindex0
var1l self 1 vabox new flipbox dup
    0 vabox ' +flip ' -flip toggle new s" Index 1" topindex : tindex1
var2l self 1 vabox new flipbox dup
    0 vabox ' +flip ' -flip toggle new s" Index 2" topindex : tindex2

3 vabox : variant0
tindex0 self tindex1 self tindex2 self 3 harbox : variant1

variant0 self 2 borderbox :notshadow noborderbox drop

variant1 self variant0 self 2 vabox : variants

button0 self box0 self info-m0 self
variants self
button4 self
button5 self
[IFDEF] win32
    dbuf1 self
[ELSE]
    :beamer beamer : beam1
    dbuf1 self beam1 assign beam1 self
[THEN]
  1 habox new -2 borderbox
2 habox new
tfield0 self ( 1 habox new -2 borderbox ) box1 self
fbutton self fbox0 self 2 vabox new
9 vabox : box2:  box2: self panel drop

box2: self 1 habox : box2

\ box2 self vfixbox drop

\ Radiobuttons                                         03oct96py

1 1 vviewport : view1
\ view1 noback off

ficon: exclam-icon ./icons/mini-exclam"
ficon: cross-icon ./icons/mini-cross"
ficon: ball-icon ./icons/red-dot"
ficon: rball-icon ./icons/green-dot"

: noop-sw 0 ['] noop dup toggle new ;

0 noop-sw cross-icon exclam-icon s" Toggle 0" ticonbutton : rbutton0
0 noop-sw cross-icon exclam-icon s" Toggle 1" ticonbutton : rbutton1
0 noop-sw cross-icon exclam-icon s" Toggle 2" ticonbutton : rbutton2
0 noop-sw cross-icon exclam-icon s" Toggle 3" ticonbutton : rbutton3
0 noop-sw s" Toggle 4" rbutton : rbutton4
0 noop-sw s" Toggle 5" rbutton : rbutton5
0 noop-sw s" Toggle 6" rbutton : rbutton6
0 noop-sw s" Toggle 7" rbutton : rbutton7

0 noop-sw ball-icon rball-icon s" Toggle 8" ticonbutton : rbutton8
0 noop-sw ball-icon rball-icon s" Toggle 9" ticonbutton : rbutton9
0 noop-sw ball-icon rball-icon s" Toggle A" ticonbutton : rbuttonA
0 noop-sw ball-icon rball-icon s" Toggle B" ticonbutton : rbuttonB
0 noop-sw s" Toggle C" tbutton : rbuttonC
0 noop-sw s" Toggle D" tbutton : rbuttonD
0 noop-sw s" Toggle E" tbutton : rbuttonE
0 noop-sw s" Toggle F" tbutton : rbuttonF

rbutton0 self  rbutton1 self  rbutton2 self  rbutton3 self
rbutton4 self  rbutton5 self  rbutton6 self  rbutton7 self
8 varbox : boxT0
rbutton8 self  rbutton9 self  rbuttonA self  rbuttonB self
rbuttonC self  rbuttonD self  rbuttonE self  rbuttonF self
8 vabox : boxT1
boxT0 self boxT1 self 2 habox : boxT2
boxT2 self vfixbox drop
boxT2 self view1 assign
\ view1 'vslide vslider0 : slider3
\ slider3 self view1 bind vspos

\ test testwin                                         25sep96py
box2 self slider2 self 2 habox : box3
view1 self asliderview : box3-

    box3- self
    1 vabox : box3-+
    box3 self
    vrtsizer new
    2 vasbox : box3+
box3+ self \ vsiz0 self
$0 1 *fill 2dup glue new
\ vsiz1 self
    vrtsizer new
     $0 1 *fil 2dup glue new
  hrtsizer new
  2 hasbox new
  box3-+ self \ vsiz1 self box3-+ self 2 vabox new
  hrtsizer new
  2 hasbox new
     $0 1 *fil 2dup glue new
     3 habox new
     2 vasbox new
3 vabox : box3''

box3'' self view0 assign
\ dbuf0 assign
menu-title0 self view0 self s" bigFORTH" win0 assign
0 0 &300 &400 win0 resize
win0 show \        win0 sync

[IFDEF] beam1
: beam-clone
  screen self window new window with
      beam1 clone s" Clone" assign show
  endwith ;

beam-clone
[THEN]

\ Another window: Add two numbers

screen self window : win1

0 ' noop number-action new 0. s" A:" infotextfield : a
0 ' noop number-action new 0. s" B:" infotextfield : b
0 ' noop number-action new 0. s" R:" infotextfield : result
0 :noname  a get b get d+ result assign ; simple new
s" +" button : add
0 :noname  a get b get d- result assign ; simple new
s" -" button : sub
0 :noname  a get b get d* result assign ; simple new
s" *" button : mul
0 :noname  result get a assign ; simple new
s" A" button : toa
0 :noname  result get b assign ; simple new
s" B" button : tob

add self 2skip sub self 2skip mul self
2skip toa self 2skip tob self 9 hatbox : box4
a self b self box4 self hline result self
5 vabox : box5  box5 self panel drop

box5 self s" Calculator" win1 assign
0 0 &200 &100 win1 resize
win1 show

\ test testwin                                         25sep96py

\ : get-event  screen handle-event &20 idle ;
\ : get-events  $1000 dup NewTask activate BEGIN get-event AGAIN ;

