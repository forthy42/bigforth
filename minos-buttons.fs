\ boxchar                                              21aug99py

widget class boxchar
public: font ptr fnt            cell var color
        cell var textwh         cell var texth
        method !textwh          early push?
        early textcenter        early textleft
        early ]C                early shadedbox
how:    : init ( cb char -- )  super init assign >callback ;
        : !textwh ( addr u -- )
          fnt self 0= IF  2drop  EXIT  THEN
          fnt size swap textwh 2! ;
        : .text ( addr u x y c -- )  >r 2swap r>
          fnt select  fnt self fnt ' display dpy drawer ;

\ boxchar                                              21aug99py

        : textcenter ( string len ox oxy -- ) { x xy }
          xywh textwh @+ @ p- p2/ p+ x xy + xy p+
          color @ 8 >> .text ;
        : textleft ( string len ox oxy -- ) { x xy }
          xywh nip texth @ - 2/ +
          x xy + xy p+ color @ 8 >> .text ;

\ boxchar                                              21mar00py

        : Xshadow ( -- n )  color @ $18 >> dup 0< - xS * 2/ ;
        : ~shadow ( -- ) color @ $FF000000 xor color ! ;
        : push? ( -- flag )  color @ 0< ;
        : +push ( -- ) push? 0=
          IF   ~shadow  1 1 dpy txy!  draw  0 0 dpy txy!  THEN ;
        : -push ( -- ) push?  IF   ~shadow  draw  THEN ;

        : (released ( x y b n -- )
          DOPRESS  inside?  IF  +push  ELSE  -push  THEN 2drop ;
        : >released ( x y b n -- flag )
          /step @ after >r +push nip 1 and
          IF    rdrop (released
          ELSE  2drop dpy sync r> till  THEN  push? -push ;
        : handle-key?  true ;

\ boxchar                                              20oct99py

        : shadedbox ( -- )
          xywh Xshadow abs xywh- color @ dpy box
          shadow Xshadow xywh drawshadow ;
        : clicked ( x y b n -- ) callback click ;
        : keyed ( key sh -- )  callback key ;
        : defocus  color defocuscol chcol  draw ;
        : focus    super focus color focuscol   chcol  draw ;
        : hglue  textwh @  xS 2* + 1+ 1 *fil ;
        : vglue  texth  @  xS 2* + 1+ 1 *fil ;

\ boxchar                                              26sep99py

        : moved ( x y -- )  super moved  callback enter ;
        : leave ( -- )  callback leave ;
        : assign ( char -- )  $10 << defocuscol @ @ or
          [ 2 $18 << ] Literal or color !
          dpy self IF  !resized  THEN ;
        : !resized  color 2+ 1 !textwh ;
        : ]C ( o -- ) callback set-called ;
        : dpy! ( dpy -- )  super dpy!
          fnt self 0= IF  0 dpy xrc font@ font!  THEN ;
        : font! ( font -- )  bind fnt
          dpy self IF  !resized  THEN ;
class;

\ triangle button                                      28mar99py

boxchar class tributton
public: early ltri              early rtri
        early utri              early dtri
how:    \ init  ( callback n -- )
        : assign ( n -- )  $FF and super assign ;

\ triangle button                                      23aug97py
        | Create pd1  0 c, 1 c, 2 c, 2 c, 2 c, 1 c, 0 c, 0 c,
        : pd ( x y h n -- )
          dup 7 and pd1 + c@ swap 2- 7 and pd1 + c@
          rot tuck * 2/ -rot * 2/ swap p+ ;
        : triangle abs { x y h c1 c2 c3 pos n }
          x y h pos    pd <poly x y h pos 3+ pd poly#
          x y h pos 6+ pd poly# poly> color @ dpy fill
          n 0 ?DO x y h pos    pd  x y h pos 3+ pd  c1 dpy line
                  x y h pos 3+ pd  x y h pos 6+ pd  c2 dpy line
                  x y h pos 6+ pd  x y h pos    pd  c3 dpy line
                  x 1+ to x  y 1+ to y  h 2- to h  LOOP ;
        : ltri ( x y h lc sc -- ) tuck      4 Xshadow triangle ;
        : rtri ( x y h lc sc -- ) over      0 Xshadow triangle ;
        : utri ( x y h lc sc -- ) dup       6 Xshadow triangle ;
        : dtri ( x y h lc sc -- ) swap dup  2 Xshadow triangle ;

\ triangle button                                      07nov99py

        public:
        [defined] Table: [IF]
            Table: tritable  ltri utri rtri dtri [
        [ELSE]
            Create tritable  ' ltri , ' utri , ' rtri , ' dtri ,
        [THEN]
[defined] x11 [IF]
        Create triarrows XC_sb_left_arrow  ,
                         XC_sb_up_arrow    ,
                         XC_sb_right_arrow ,
                         XC_sb_down_arrow  ,
[THEN]
[defined] win32 [IF]
        Create triarrows 3 [FOR] mouse_cursor , [NEXT]
[THEN]
        : moved ( x y -- ) super moved
          color @ $E >> $C and triarrows + @ dpy set-cursor ;
class;

\ triangle button                                      11apr99py

tributton class slidetri
how:    : hglue  xM 0 ;
        : vglue  xM 0 ;
class;

[defined] alias [IF]
' noop alias TRI:                               immediate
[ELSE]
    synonym TRI: noop-i
[THEN]
0 Constant :left
1 Constant :up
2 Constant :right
3 Constant :down

\ togglechar                                           11nov06py
boxchar class togglechar
public: cell var oncolor
        method set              method reset
how:    #20 /step V!
        : assign  ( char- char+ -- )
          $10 << defocuscol @ @ or [ 2 $18 << ] Literal or
          oncolor ! super assign ;
        : reverse ( -- )  color @ oncolor @ color ! oncolor !
          draw ;
        : >released  reverse super >released ;
        : defocus  oncolor defocuscol chcol super defocus ;
        : focus    oncolor   focuscol chcol super focus   ;
        : reset  callback reset ;
        : set  callback set ;

\ togglechar                                           13nov06py

        : click-it ( -- )  bl 0 parent keyed
          parent self combined with active show-you draw
          endwith ;
        : keyed ( key sh -- )
          over $FF52 & hbox @ parent class? + =
          IF  2drop parent prev-active  0=
              IF   parent prev-active drop
              THEN  click-it
              EXIT  THEN
          over $FF54 & hbox @ parent class? + =
          IF  2drop parent next-active  0=
              IF  parent first-active  THEN
              click-it  EXIT  THEN
          super keyed ;

\ button                                               20may06py

        : clicked ( x y b n --)  over $08 and
          IF  2drop 2drop parent prev-active  0=
              IF   parent prev-active drop
              THEN  click-it
              EXIT  THEN
          over $10 and
          IF  2drop 2drop parent next-active  0=
              IF  parent first-active  THEN
              click-it  EXIT  THEN
          callback click draw ;
        : get  callback fetch ;
class;

\ button                                               27jun02py
boxchar class button
public: cell var text
how:    : init ( callback addr len -- )
          defocuscol @ @ [ 2 $18 << ] Literal or color !
          super init ;
        : dispose ( -- )  text $off super dispose ;
        : text! ( addr n -- )  text $!
          dpy self IF  !resized  THEN ;
        : get ( -- addr len ) text $@ ;
        : assign ( addr len -- )  text!
          dpy self IF  parent resized  THEN ;
        : !resized  text $@ !textwh ;
class;

\ button variants                                      13may00py

button class lbutton
class;

lbutton class text-label
how:    : init ( addr len -- ) ( >r)
          noop-act -rot ( r>) super init ;
        widget :: handle-key?
class;

text-label class text-word
class;

\ button variants                                      05mar00py

text-label class menu-label
how:    7 colors focuscol !     7 colors defocuscol !
        widget :: focus
        widget :: defocus
class;

\ toggle buttons                                       11apr99py

togglechar class tbutton
public: cell var text           early halfshade
how:    : init ( callback addr len -- )
          defocuscol @ @ oncolor !
          defocuscol @ @   color !
          boxchar :: init ;
        : halfshade ( -- )  color @ $18 >>
          IF    xywh xS 2/ xywh- color @ dpy box
                shadow swap xS 2/ xywh drawshadow
          ELSE  xywh color @ dpy box  THEN ;

\ toggle buttons                                       27jun02py

        : text! ( addr n -- )  text $!
          dpy self IF  !resized  THEN ;
        : assign ( addr len -- )  text!
          w @ 0= ?EXIT  w @   textwh @+ @
          xS 2* + h @ max h ! dup xS 2* + 1+ w @ max w !
          < IF  parent resized  ELSE  draw  THEN ;
        : !resized  text $@ !textwh ;
        : dispose text $off super dispose ;
class;

\ radio button, flipbutton, togglebutton               10aug05py

tbutton class rbutton                           class;
tbutton class flipbutton                        class;
tbutton class topindex                          class;

*hglue class topglue
how:    : init ( -- )  0 1 *filll super init ;
class;

: flipper  combined ' +flip
    :[ combined attribs c@ :flip or combined attribs c! combined hide ]:
    toggle new ;

\ Topindex, topglue                                    11apr99py
tbutton class togglebutton
public: cell var text1
        cell var textwh1        cell var texth1
how:    : ?size ( -- )  dpy self IF  !resized  ELSE  EXIT  THEN
          w @ 0= ?EXIT
          w @   textwh @ textwh1 @ max  texth @ texth1 @ max
          xS 2* + h @ max h ! dup xS 2* + 1+ w @ max w !
          < IF  parent resized  ELSE  draw  THEN ;
        : !resized  ( -- )  super !resized
          text1 $@ 0 textsize swap textwh1 2! ;
        : text! ( addr n -- )  text1 $!  ?size ;
        : assign ( addr n -- )  text $!  ?size ;
        : init ( toggle addr1 n1 addr2 n2 -- )
          text! super init [ 2 $18 << ] Literal
          dup color +!  oncolor +! ;            class;

\ Icon+text button                                     11apr99py

button class icon-button
public: icon-pixmap ptr icon
how:    : init ( callback icon addr len -- )  text! super init ;
        : assign ( icon -- )  bind icon
          w @ IF  parent resized  THEN ;
class;

icon-button class icon-but
how:    \ init ( callback icon -- )
        : text!  s" " super text! ;
class;

\ toggleicon, flipicon                                 28dec97py

togglechar class toggleicon
public: icon-pixmap ptr icon-
        icon-pixmap ptr icon+
how:    : assign  ( icon- icon+ -- )
          bind icon+ bind icon- bl bl super assign ;
class;

togglechar class flipicon
public: icon-pixmap ptr icon
how:    : assign  ( icon -- )
          bind icon  bl bl super assign ;
class;

\ togglebutton with text                               11apr99py

tbutton class ticonbutton
public: icon-pixmap ptr icon-   icon-pixmap ptr icon+
how:    : assign ( icon- icon+ -- )
          bind icon+ bind icon- ;
        : init ( callback icon- icon+ addr len -- )
          defocuscol @ @ oncolor !
          defocuscol @ @   color !
          text!  boxchar :: init ;
class;

\ icon with small text                                 21mar04py
icon-button class big-icon
how:    : inside? ( x y -- ) 2dup super inside?
          0= IF  2drop false  EXIT  THEN
          2dup x @ w @ icon w @ - 2/ + y @ p-
          over icon w @ u< over icon h @ u< and
          IF
[defined] x11 [IF]   icon shape @ -1 = IF
                    dpy xrc dpy @ icon image @ 1- 2swap 1 1 -1 ZPixmap
                    XGetImage >r
                    r@ IF  0 0 r@ XGetPixel r> XDestroyImage
                           0< >r  THEN
              ELSE  dpy xrc dpy @ icon shape @ 2swap 1 1 1 ZPixmap
                    XGetImage >r
                    r@ IF  0 0 r@ XGetPixel r> XDestroyImage
                           0<> >r  THEN  THEN            [THEN]

\ icon with small text                                 21mar04py

[defined] win32 [IF] swap icon shape @ GetDC GetPixel 0<> >r    [THEN]
          ELSE  2drop false >r  THEN
          xywh textwh @+ @ p- drop 2/ icon h @ p+ p-
          textwh @+ @ rot swap u< >r u< r> and r> or ;
class;

\ simple text field                                    19dec99py
button class (textfield
public: cell var curpos         cell var selw
        cell var curx           cell var curw
        cell var old-h          cell var ds
        method ins              method del
        method c                method cur!
        early 'text+            early 'text-
how:    0 colors focuscol !     7 colors defocuscol !
        : init  ( dostroke addr len -- )
          1 selw !  super init ;
        : show-you ( -- )  curx 2@ swap 2/ + x @ +
          y @ h @ 1+ 2/ + dpy show-me ;
        : hglue  textwh @ xS +
          1- ds @ >> 1+ ds @ << dup old-h ! 1 *fil ;
        : vglue  texth  @                   1 *fil ;

\ simple text input field                              13jan05py

        : textwh@ ( addr u -- w h )
          fnt self 0= IF  2drop 0 0  ELSE  fnt size  THEN ;
        : !curxw ( -- )
          text $@ 1+ 2dup curpos @ selw @ 0max + min
          textwh@ drop curx !
          curpos @ selw @ 0min + /string
          selw @ abs min over dup xchar+ swap - max
          textwh@ drop dup curw !
          negate curx +! ;

\ simple text input field                              20feb00py
        : text! ( -- )  dpy self 0= ?EXIT  !resized
          0 text $@ + c!  flags #hidden bit@ ?EXIT
          hglue drop dup w @ <= swap r> = and
          IF  draw  ELSE
              parent self 0= ?EXIT  parent resized  THEN
          callback toggle ;
        : assign ( addr n -- )  tuck text $! bl text $@ + c!
          curpos ! text! ;
        : get ( -- addr len )  text $@ ;
        : >pos ( x -- n )
          text $@ 0
          ?DO  2dup I 1+ textwh@ drop <=
               IF  2drop I unloop EXIT  THEN
          LOOP  2drop text $@len ;
        : !resized ( -- )  text $@ 1+ !textwh !curxw ;

\ simple text input field                              06jan05py

        : moved ( x y -- )  2drop
[defined] x11 [IF]   XC_xterm   [THEN] [defined] win32 [IF] IDC_IBEAM  [THEN]
          dpy set-cursor  ^ dpy set-rect ;
        : (dpy  [defined] x11 [IF]    dpy get-win  dpy xrc dpy @
          [ELSE] 0 0 [THEN] ;

        : 'text ( -- addr )  text $@ curpos @ /string
          0= IF  bl over c!  THEN ;
        : 'text+ ( -- len )  'text dup xchar+ swap - ;
        : 'text- ( -- len )  'text dup
          curpos @ IF  xchar-  THEN swap - ;

\ simple text input field                              04nov06py

        : ins ( addr u -- )  text curpos @ $ins text! ;
        : del ( n -- )  text curpos @ rot $del text! ;
        : c ( n -- flag )
          dup 0>= IF    dup IF  'text+ max  THEN
                        curpos @ text $@len >=
                  ELSE  'text- min
                        curpos @ 0<>  THEN  swap
          curpos @ + 0max text $@len min curpos !
          'text+ selw !  !curxw draw show-you ;
        : cur! ( n -- )  curpos @ - c drop ;

\ simple text input field                              16jan05py

        : ins-sel ( -- )
          (dpy @select dup >r ins r> c drop ;
        : >select ( n -- )  selw @ >r
          dup selw ! text $@ rot dup >r
          0min curpos @ + /string r> abs min
          -select +select
          [defined] x11 [IF]    dpy get-win  dpy xrc dpy @
          [ELSE] 0 0 [THEN]  !select !curxw
          selw @ r> <> IF  draw  THEN ;
        : sel-word ( -- )
          text $@ 2dup curpos @ /string bl scan nip -
          2dup -trailing nip /string >r
          text $@ drop - curpos ! r> >select ;
        : sel-all ( -- ) curpos off text $@len >select ;

\ simple text input field                              14apr01py

        Variable click
        : clicked ( x y b n -- )
          click @ 0= IF  click on  callback click  click off
                         EXIT  THEN  1 selw !
          swap >r 2 pick x @ - >pos cur!  r>
              1 and 0=  IF  2drop drop ins-sel  EXIT  THEN
          dup 1 and 0=  IF  nip nip
             dup 4 = IF  drop sel-word  EXIT  THEN
                 4 > IF       sel-all         THEN  EXIT THEN
          drop
          DOPRESS  drop dup y @ h @ 2/ + dpy scroll
                   x @ - >pos curpos @ - >select 2drop ;
class;

\ text input actor                                     06jan05py

actor class edit-action
public: static key-methods      & caller (textfield asptr edit
        early bind-key          early find-key
        cell var stroke
how:    0 key-methods !
        : find-key ( key -- addr )  >r key-methods
          BEGIN  @ dup  WHILE  dup 2 cells + @ r@ =
                 IF  rdrop  EXIT  THEN  REPEAT  rdrop ;
        : key ( key sh -- ) drop  dup 0= IF  drop  EXIT  THEN
          dup shift-keys? IF  drop  EXIT  THEN  dup find-key dup
          IF    cell+ @ caller send drop
          ELSE  drop char$ edit with ins 1 c drop endwith
          THEN ;
        : click ( x y b n -- ) caller clicked ;

\ text input actor                                     15apr01py

        : bind-key ( key method -- )
          here key-methods @ A, key-methods ! A, , ;
        : init ( o xt -- ) stroke ! super init ;
        : store ( addr u -- ) edit assign ;
        : toggle ( -- ) stroke @ called send ;
        : fetch ( -- addr u ) edit get ;
class;

[defined] alias [IF]
' :[ alias ST[                               immediate restrict
[ELSE]
    synonym ST[ :[
[THEN]
: ]ST postpone ]: edit-action postpone new ; immediate restrict

\ text input key binding                               15apr01py

: K[ ( key -- )  (textfield postpone with postpone :[ ;
: ]K ( key sys ) postpone ]: >r (textfield postpone endwith r>
  & edit-action >o edit-action bind-key o> ;          immediate
: K-alias ( key1 key2 -- ) swap edit-action find-key
  ?dup IF  cell+ @
           & edit-action >o edit-action bind-key o> THEN ;

\ text input key binding                               07jan07py

$FF08  K[ selw @ 'text+ <> IF  selw @ dup 0< IF  dup c drop THEN
          abs del  ELSE -1 c IF  selw @ del 0 c drop THEN  THEN
                                      ]K  $FF08 ctrl H K-alias
$FF51  K[ -1 c drop                   ]K
$FF53  K[  1 c drop                   ]K
$FFFF  K[ selw @ 'text+ <> IF  selw @ dup 0< IF  dup c drop THEN
          abs del  ELSE 0 c 0= IF  selw @ del 0 c drop THEN THEN
                                      ]K  $FFFF ctrl ? K-alias
$FF1B  K[ s" " assign                 ]K  $FF1B ctrl [ K-alias
ctrl L K[ parent resized              ]K
ctrl K K[ text $@ curpos @ min assign ]K
$FF50  K[ 0 cur!                      ]K  $FF50 ctrl A K-alias
$FF57  K[ text $@len cur!             ]K  $FF57 ctrl E K-alias
ctrl W K[ selw @ dup 0min c drop abs del ]K

\ key action with text pointer                         14apr01py

edit-action class edit-var
public: cell var text
how:    : update  edit get text @ $! ;
        : store   super store  update ;
        : init ( o addr -- )  text ! ['] noop  super init ;
        : key ( kb sh -- )  super key  update ;
        : click ( x y b n -- )  super click  update ;
class;

[defined] alias [IF]
' noop alias VT[                             immediate restrict
[ELSE]
    synonym VT[ noop-i
[THEN]
: ]VT edit-var postpone new ;                immediate restrict

\ number input field                                   27apr98py
edit-action class number-action
public: cell var nbase
how:    : ># ( d -- addr u )  base push nbase @ base ! tuck dabs
          <# #S  nbase @ $10 = IF  '$' hold  THEN
                 nbase @ %10 = IF  '%' hold  THEN  rot sign  #> ;
        : key ( key sh -- ) drop base push nbase @ base !
          dup shift-keys? IF  drop  EXIT  THEN  dup find-key dup
          IF    cell+ @ caller send drop
          ELSE  drop dup digit? nip 0= ?EXIT
                sp@ 1 edit with ins drop 1 c drop endwith
          THEN  stroke @ called send ;
        : store ( d -- ) ># edit assign ;
        : fetch ( -- d ) edit get base push decimal s>number ;
        : init  ( o addr -- ) #10 nbase ! super init ;
class;

\ number input field                                   28aug99py

: #[ ( key -- )  (textfield postpone with postpone :[ ;
: ]# ( key sys ) postpone ]: >r (textfield postpone endwith r>
  & number-action >o number-action bind-key o> ;      immediate
'$' #[ callback self number-action with
      fetch $10 nbase ! store endwith ]#
'%' #[ callback self number-action with
      fetch %10 nbase ! store endwith ]#
'&' #[ callback self number-action with
      fetch #10 nbase ! store endwith ]#
'#' #[ callback self number-action with
      fetch #10 nbase ! store endwith ]#
'-' #[ callback self number-action with
      fetch dnegate store endwith ]#
[defined] alias [IF]
' :[ alias SN[                               immediate restrict
[ELSE]
    synonym SN[ :[
[THEN]
: ]SN postpone ]: number-action postpone new ;
                                             immediate restrict

\ number edit variables                                15apr01py

number-action class edit-int
public: cell var int
how:    : update  fetch drop int @ ! ;
        : store   super store  update ;
        : init ( o addr -- )  int ! ['] noop  super init ;
        : key ( kb sh -- )  super key  update ;
        : click ( x y b n -- )  super click  update ;
class;

[defined] alias [IF]
' noop alias IV[                             immediate restrict
[ELSE]
    synonym IV[ noop-i
[THEN]
: ]IV edit-int postpone new ;                immediate restrict

\ text field derivates                                 19dec99py

habox class textfield
public: (textfield ptr edit
how:    : init ( xxx act -- )
          s" " (textfield new
          bind edit  assign  5 edit ds !  edit self
          1 super init -2 borderw c! ;
        : assign ( xxx -- ) edit callback store ;
        : get ( -- xxx ) edit callback fetch ;
        : clicked ( x y b n -- ) dup 0= IF 2drop 2drop EXIT THEN
          super clicked ;
class;

: text@  (textfield callback fetch ;

\ text field derivates                                 19dec99py

textfield class infotextfield
public: text-label ptr info
how:    : init ( xxx act addr2 u2 -- )
          text-label new bind info
          s" " (textfield new
          bind edit assign  5 edit ds !
          info self 1 habox new hfixbox
          edit self 1 habox new -2 borderbox
          2 super super init ;
        : text! ( addr u -- ) info assign ;
class;

\ text field derivates                                 19dec99py
hatab class tableinfotextfield
public: (textfield ptr edit
        text-label ptr info
how:    : init ( xxx act addr2 u2 -- )
          text-label new bind info
          s" " (textfield new
          bind edit  assign  5 edit ds !
          0 1 *fil 2dup glue new
          info self 1 habox new hfixbox 2 habox new hfixbox
          edit self 1 habox new -2 borderbox
          2 super super init ;
        infotextfield :: text! ( addr u -- )
        textfield :: assign ( xxx -- )
        textfield :: get ( -- xxx )
        textfield :: clicked ( x y b n -- )
class;

\ vrbox (radio box)                                    27may00py
vbox class vrbox
public: early reset-childs      early activate?
how:    : reset-childs  ALLCHILDS
          & togglechar @ class?  IF  togglechar reset draw THEN
          & combined   @ class?  IF  recurse  THEN ;
        : (clicked ( x y b n -- )  reset-childs super (clicked ;
        : activate? ( key -- flag )
           dup bl = swap #cr = or ;
        : keyed   ( key sh -- )  over  activate?
          IF  reset-childs  THEN  super keyed ; class;
vabox  class varbox
how:    : (clicked ( x y b n -- )
          vrbox :: reset-childs super (clicked ;
        : keyed   ( key sh -- )   over vrbox :: activate?
          IF  vrbox :: reset-childs  THEN  super keyed ;  class;

\ hboxes                                               04sep97py
hbox   class hrbox
how:    vrbox  :: (clicked      vrbox  :: keyed         class;
habox  class harbox
how:    varbox :: (clicked      varbox :: keyed         class;

htbox   class hrtbox
how:    vrbox  :: (clicked      vrbox  :: keyed         class;
hatbox  class hartbox
how:    varbox :: (clicked      varbox :: keyed         class;

vtbox   class vrtbox
how:    vrbox  :: (clicked      vrbox  :: keyed         class;
vatbox  class vartbox
how:    varbox :: (clicked      varbox :: keyed         class;

\ dialog management                                    28aug99py

vabox class modal               \ cell var app
public: gadget ptr default      method default!
how:    : init ( widget1 .. widgetn n default -- )
          ( up@ app ! ) ( swap ) ?dup IF  bind default  THEN
          super init ;
        : close ( -- )  dpy close ;
        : keyed ( key sh -- )  over #cr = default self and
          IF  default keyed  ELSE  super keyed  THEN ;
        : default!  default self over bind default
          <> IF  draw  THEN ;
class;

\ text with parbox                                     27may00py

[defined] parbox [IF]
parbox class text-parbox
how:    Variable text-string
        : init ( addr u format -- )  >r
          text-string $! 0 text-string bl
          :[ -trailing bl skip text-word new swap 1+ ]: $iter
          r> super init  text-string $off ;
        : assign ( addr u -- ) text-string $!  dispose-childs
          unhbox 2drop dispose[] items 'nil bind childs
          0 text-string bl
          :[ -trailing bl skip text-word new swap 1+ ]: $iter
          dup n' !  text-string $off  [],  over bind[] items
          ?DO  I !  -cell +LOOP  0 hboxing dup n ! >box
          dpy self dpy! ;
class;
[THEN]

\ new slider                                           20oct99py
widget class arule
public: cell var color
        cell var gethglue       cell var getvglue
how:    : init ( actor hxt vxt -- )
          super init getvglue ! gethglue ! >callback
          defocuscol @ @ assign ;
        : assign ( color -- )  color ! ;
        : Xshadow ( -- n )  color @ $18 >> dup 0< - ;
        : hglue ( -- glue ) gethglue @ callback called send ;
        : vglue ( -- glue ) getvglue @ callback called send ;
        : clicked  callback click ;
        : keyed    callback key ;
        : defocus Xshadow 0= ?EXIT color defocuscol tocol draw ;
        : focus   Xshadow 0= ?EXIT color focuscol   tocol draw ;
        boxchar :: >released                    class;

\ new slider                                           11dec04py
hbox class hslider public:
        early part0             early part1
        early part2             early part3
        method lstep            method rstep
        method lpage            method rpage
        method slide            method do-slide
        method reslide          method subbox
how:    : get  ( -- steps step pos )
          callback self 0=  IF 1 1 0 EXIT THEN callback fetch ;
        : ?fil ( n1 n2 -- )  1 *fil < IF  *fil  THEN ;
        : part0 ( -- glue )  xM    1 *fil            ;
        : part1 ( -- glue )  0     get nip swap  ?fil ;
        : part2 ( -- glue )  xS 3* get drop swap ?fil ;
        : part3 ( -- glue )  0     get + over swap - swap ?fil ;
        : draw ( -- )  xywh resize super draw ;

\ new slider                                           08mar07py
        : lstep ( -- ) get nip nip 1- 0max             reslide ;
        : rstep ( -- ) get >r - r> 1+ min              reslide ;
        : lpage ( -- ) get swap 2 max - 1+ nip 0max    reslide ;
        : rpage ( -- ) get over 2 max + 1- >r - r> min reslide ;
        : init ( callback -- )  >callback subbox super init ;
        : reslide ( n -- )  get nip nip case? ?EXIT
          assign draw ;
        : do-slide  drop  ( pos x0 x )
          over - >r over r> ( pos dx )
          get 2drop 2* w @ hglue drop -
          ?dup IF  */f 1+ 2/  ELSE  2drop 0  THEN  +
          0max get drop - min reslide 2drop ;
        : slide ( x y b n -- ) drop
          nip 1 and 0= IF  2drop 0  EXIT  THEN
          drop get nip nip swap 0 -rot DOPRESS do-slide ;

\ new slider                                           08mar07py
        : keyed ( key sh -- )  drop
          $FF51 case?  IF  lstep  EXIT  THEN
          $FF53 case?  IF  rstep  EXIT  THEN
          $FF50 case?  IF  0 reslide           EXIT  THEN
          $FF57 case?  IF  get drop - reslide  EXIT  THEN
          $FF55 case?  IF  lpage  EXIT  THEN
          $FF56 case?  IF  rpage  EXIT  THEN  drop ;
        : assign   ( pos -- )  callback store ;
        : hglue@ hglue ;
        : moved ( x y -- )  widget :: moved  callback enter ;
        : leave ( -- )  callback leave ;
        : clicked ( x y b n -- )  leave
          over $10 and  IF  2drop 2drop rpage  EXIT  THEN
          over $08 and  IF  2drop 2drop lpage  EXIT  THEN
          super clicked ;                       class;

\ new slider                                           11dec04py
vbox class vslider public:
        early part0             early part1
        early part2             early part3
        method lstep            method rstep
        method lpage            method rpage
        method slide            method do-slide
        method reslide          method subbox
how:    : get  ( -- steps step pos )
          callback self 0=  IF 1 1 0 EXIT THEN callback fetch ;
        : ?fil ( n1 n2 -- )  1 *fil < IF  *fil  THEN ;
        : part0 ( -- glue )  xM    1 *fil            ;
        : part1 ( -- glue )  0     get nip swap  ?fil ;
        : part2 ( -- glue )  xS 3* get drop swap ?fil ;
        : part3 ( -- glue )  0     get + over swap - swap ?fil ;
        : draw ( -- )  xywh resize super draw ;

\ new slider                                           08mar07py

        : lstep ( -- ) get nip nip 1- 0max             reslide ;
        : rstep ( -- ) get >r - r> 1+ min              reslide ;
        : lpage ( -- ) get swap 2 max - 1+ nip 0max    reslide ;
        : rpage ( -- ) get over 2 max + 1- >r - r> min reslide ;
        hslider :: reslide ( n -- )
        : init ( callback -- )  >callback subbox super init ;
        : do-slide  nip  ( pos y0 y )
          over - >r over r> ( pos dy )
          get 2drop 2* h @ vglue drop -
          ?dup IF  */f 1+ 2/  ELSE  2drop 0  THEN  +
          0max get drop - min reslide 2drop ;
        : slide ( x y b n -- ) drop
          nip 1 and 0= IF  2drop 0  EXIT  THEN
          nip get nip nip swap 0 -rot DOPRESS do-slide ;

\ new slider                                           08mar07py

        : keyed ( key sh -- )  drop
          $FF52 case?  IF  lstep  EXIT  THEN
          $FF54 case?  IF  rstep  EXIT  THEN
          $FF50 case?  IF  0 reslide           EXIT  THEN
          $FF57 case?  IF  get drop - reslide  EXIT  THEN
          $FF55 case?  IF  lpage  EXIT  THEN
          $FF56 case?  IF  rpage  EXIT  THEN  drop ;
        : assign   ( pos -- ) callback store ;
        : vglue@ vglue ;
        : moved ( x y -- )  widget :: moved  callback enter ;
        : leave ( -- )  callback leave ;
        hslider :: clicked
class;

\ hslider0 vslider0                                    21mar00py

hslider class hslider0
how:    : handle-key?  get drop <> ;
        : draw    handle-key?  IF  super draw  THEN ;
        : vglue   handle-key?
          IF  super vglue   ELSE  0 0  THEN ;
        : vglue@  handle-key?
          IF  super vglue@  ELSE  0 0  THEN ;
        : hglue   super hglue  drop 1 *fill ;
        : hglue@  super hglue@ drop 1 *fill ;
        : focus   handle-key?  IF  super focus    THEN ;
        : defocus handle-key?  IF  super defocus  THEN ;
class;

\ hslider0 vslider0                                    21mar00py

vslider class vslider0
how:    : handle-key?  get drop <> ;
        : draw    handle-key?  IF  super draw  THEN ;
        : hglue   handle-key?
          IF  super hglue   ELSE  0 0  THEN ;
        : hglue@  handle-key?
          IF  super hglue@  ELSE  0 0  THEN ;
        : vglue   super vglue  drop 1 *fill ;
        : vglue@  super vglue@ drop 1 *fill ;
        : focus   handle-key?  IF  super focus    THEN ;
        : defocus handle-key?  IF  super defocus  THEN ;
class;

\ scaler helper words                                  11dec04py

: max10 ( n max -- n' )  >r  #1000000000
  BEGIN  tuck mod dup r@ u>  WHILE
         swap #10 /f  REPEAT  nip rdrop ;
: digit+ ( digit max n -- max n' )
  #10 * rot '0' - over 0< IF  -  ELSE  +  THEN ;

\ new scaler                                           03dec06py

hslider class hscaler public:   cell var offset
        cell var textwh         cell var texth
        cell var text*/         cell var text/
        font ptr fnt            cell var color
        early part0a            early part0b
        early part1             early part3
        early part4             early part5
        early slide1
public: method #>text           \ early scalekey

\ new scaler                                           03dec06py

how:    : #>text ( n -- addr u )  base push decimal
          text/ @ m* tuck dabs  <#
          text*/ @ 1 ?DO  # I 9 * +LOOP
          text*/ @ 1 > IF  '.' hold  THEN  #S rot sign #> ;
        : .text ( addr u x y c -- )  >r 2swap r>
          fnt select  fnt self fnt ' display dpy drawer ;
        : get  ( -- steps step pos )  super get 0 swap ;
        : o+  ( n -- n' )  offset @  + ;
        : o-  ( n -- n' )  offset @  - ;
        : o'+ ( n -- n' )  offset @  text*/ 2@ */f  + ;
        : o'- ( n -- n' )  offset @  text*/ 2@ */f  - ;
        : init  1 1 text*/ 2!  super init ;

\ new scaler                                           03dec06py
        : keyed ( k s -- k s )  over '0' '9' 1+ within
          IF  drop get >r - text*/ 2@ */f r> text*/ 2@ */f digit+
              dup 0< IF  nip negate 0 o'- max10 negate
                   ELSE  swap o'+ max10  THEN
              text*/ 2@ swap */f
              reslide  EXIT  THEN                    over #bs =
          IF  2drop get nip nip s>d #10 sm/rem nip o- 0max o+
              reslide  EXIT  THEN                     over '%' =
          IF  2drop get >r - r> 0max #100 min
              #100 */f o+ reslide  EXIT  THEN          over '-' =
          IF  2drop get >r - 1- r> negate o- 0max min o+ reslide
              EXIT  THEN  >r
          $FF50 case? IF  0 o+ reslide          rdrop  EXIT THEN
          $FF57 case? IF  get drop - o+ reslide rdrop  EXIT THEN
          r> super keyed ;

\ new scaler                                           06mar07py
        : !resized  fnt self 0= ?EXIT
          get drop - #>text fnt size swap textwh 2!
          super !resized ;
        boxchar :: handle-key?
        : show   get nip o- 0max min o+ assign  super show ;
        : part0a ( -- glue ) 0 1 *fil ;
        : part0b ( -- glue ) xM 0 ;
        : part1 ( -- glue )  0 get o- nip swap  ?fil ;
        : part3 ( -- glue )  0 get o- + over swap - swap ?fil ;
        : part4 ( -- glue )  textwh @ 2/ xS + 0 ;
        : part5 ( -- glue )  texth @ 0 ;

\ new scaler                                           08mar07py

        : lstep get o- nip nip 1- 0max             o+ reslide ;
        : rstep get o- >r - r> 1+ min              o+ reslide ;
        : lpage get o- nip nip text*/ 2@ swap /f 1 max
          - 0max     o+ reslide ;
        : rpage get o- >r - r> text*/ 2@ swap /f 1 max
          + min      o+ reslide ;
        : clicked ( x y b n -- )
          over $10 and  IF  2drop 2drop rpage  EXIT  THEN
          over $08 and  IF  2drop 2drop lpage  EXIT  THEN
          super clicked ;

\ new scaler                                           20dec04py
        : do-slide  ( pos x0 x y  -- )  drop  ( pos x0 x )
          over - >r over r> ( pos dx )
          get 2drop 2* w @ hglue drop -
          ?dup IF  */f 1+ 2/  ELSE  2drop 0  THEN  +
          o- 0max get drop - min o+ reslide 2drop ;
        : slide1 ( x y b n -- ) drop >r drop  0 -rot
          0 o+ x @ part4 drop 2* + 2swap
          r> 1 and 0= IF  do-slide  EXIT  THEN
          2drop  DOPRESS  do-slide ;
        : font! dup bind fnt super font! ;
        : focus    focuscol @ @ color !   super focus   draw ;
        : defocus  defocuscol @ @ color ! super defocus draw ;
        : dpy!  super dpy!
          fnt self 0= IF  0 dpy xrc font@ font!  THEN ;
class;

\ new scaler                                           03dec06py

vslider class vscaler public:   cell var offset
        cell var textwh         cell var texth
        cell var text*/         cell var text/
        font ptr fnt            cell var color
        early part0a            early part0b
        early part1             early part3
        early part4             early part5
        early slide1
public: method #>text           \ early scalekey

\ new scaler                                           08mar07py
how:    : #>text ( n -- addr u )  base push decimal
          text/ @ m* tuck dabs  <#
          text*/ @ 1 ?DO  # I 9 * +LOOP
          text*/ @ 1 > IF  '.' hold  THEN  #S rot sign #> ;
        : .text ( addr u x y c -- )  >r 2swap r>
          fnt select  fnt self fnt ' display dpy drawer ;
        : get  ( -- steps step pos )  super get 0 swap ;
        : o+ ( n -- n' ) offset @ + ;
        : o- ( n -- n' ) offset @ - ;
        : o'+ ( n -- n' ) offset @  text*/ 2@ */f  + ;
        : o'- ( n -- n' ) offset @  text*/ 2@ */f  - ;
        : clicked ( x y b n -- )  leave
          over $10 and  IF  2drop 2drop lpage  EXIT  THEN
          over $08 and  IF  2drop 2drop rpage  EXIT  THEN
          super clicked ;

\ new scaler                                           08mar07py
        : init  1 1 text*/ 2!  super init ;
        : keyed ( k s -- k s )  over '0' '9' 1+ within
          IF  drop get >r - text*/ 2@ */f r> text*/ 2@ */f digit+
              dup 0< IF  nip negate 0 o'- max10 negate
                   ELSE  swap o'+ max10  THEN
              text*/ 2@ swap */f
              reslide  EXIT  THEN                    over #bs =
          IF  2drop get nip nip s>d #10 sm/rem nip o- 0max o+
              reslide  EXIT  THEN                     over '%' =
          IF  2drop get >r - r> 0max #100 min
              #100 */f o+ reslide  EXIT  THEN          over '-' =
          IF  2drop get >r - 1- r> negate o- 0max min o+ reslide
              EXIT  THEN  drop

\ new scaler                                           20may06py
          $FF52 case?  IF  rstep  EXIT  THEN
          $FF54 case?  IF  lstep  EXIT  THEN
          $FF50 case?  IF  get drop - o+ reslide  EXIT  THEN
          $FF57 case?  IF  0 o+ reslide           EXIT  THEN
          $FF55 case?  IF  rpage  EXIT  THEN
          $FF56 case?  IF  lpage  EXIT  THEN  0  super keyed ;
        : !resized fnt self 0= ?EXIT  get drop -
          o+ #>text fnt size swap  0 o+ #>text fnt size drop
          max xS + textwh 2! super !resized ;
        : part4 ( -- glue )  texth @ 2/ xS + 0 ;
        : part5 ( -- glue )  textwh @ 0 ;
        : part0a ( -- glue ) 0 1 *fil ;
        : part0b ( -- glue ) xM 0 ;
        : part1 ( -- glue )  0 get o- nip swap  ?fil ;
        : part3 ( -- glue )  0 get o- + over swap - swap ?fil ;

\ new scaler                                           11dec04py
        : do-slide  ( pos y0 x y -- )  nip  ( pos y0 y )
          over - >r over r> ( pos dy )
          get 2drop 2* h @ vglue drop -
          ?dup IF  */f 1+ 2/  ELSE  2drop 0  THEN  +
          o- 0max get 2drop tuck min - o+ reslide 2drop ;
        : slide1 ( x y b n -- ) drop >r drop  0 -rot
          0 o+ y @ part4 drop + xS + 2swap
          r> 1 and 0= IF  do-slide  EXIT  THEN
          2drop  DOPRESS  do-slide ;
        : font! dup bind fnt super font! ;
        : focus    focuscol @ @ color !   super focus   draw ;
        : defocus  defocuscol @ @ color ! super defocus draw ;
        : dpy!  super dpy!
          fnt self 0= IF  0 dpy xrc font@ font!  THEN ;

\ new scaler                                           11dec04py

        hscaler :: lstep
        hscaler :: rstep
        hscaler :: lpage
        hscaler :: rpage
        boxchar :: handle-key?
        : slide ( x y b n -- ) drop
          nip 1 and 0= IF  2drop 0  EXIT  THEN
          nip  get nip o- - o+ swap 0 -rot DOPRESS do-slide ;
        : show  ( -- )  get nip o- 0max min o+ assign
          super show ;
class;

: SC# ( o offset -- o )  over hscaler with  offset !  endwith ;
: SC*/ ( o * / -- o )  over2 hscaler with text*/ 2! endwith ;

