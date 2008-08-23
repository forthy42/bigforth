\ combined widgets attributes                          08apr00py

$01 Constant :hfix
$02 Constant :vfix
$04 Constant :flip
$08 Constant :resized
$10 Constant :notshadow
$20 Constant :norshadow
$40 Constant :nobshadow
$80 Constant :nolshadow

\ combined widgets                                     27may00py

widget class combined
public: gadget ptr childs       cell var n
        2 cells var hglues      2 cells var vglues
        1 var hskip             1 var vskip
        1 var borderw           1 var attribs
        gadget ptr active       cell var tab-steps
        method compose          method (clicked
        early hskip@            early vskip@
        method add              method remove
        method flip
        method +flip            method -flip
        method tab!             method tab@
        method tabs
        early >box              early ALLCHILDS

\ combined widgets                                     21mar00py
how:    : >box  'nil bind childs  'nil bind active
          0 ?DO  childs self >r  bind childs
                 r> childs bind widgets
                 self childs bind parent
          LOOP  first-active ;
        : init ( widget_1 .. widget_n n -- )
          focuscol @ color !
          super init  dup n ! >box ;
        : ALLCHILDS ( .. -- ..' )  childs self
          BEGIN  dup 'nil <>  WHILE
                 r@ swap >o execute widgets self o>
          REPEAT  drop rdrop ;
        : (dpy!  ALLCHILDS  dup dpy! ;
        : dpy! ( dpy -- )  dup bind dpy (dpy! drop
          0, hglues 2!  0, vglues 2! ;

\ combined widgets                                     28mar99py

        : remove  ( o -- )
          link childs childs delete -1 n +! resized ;
        : add ( o before -- )
          over dpy self swap >o dpy! o>
          dup childs self =
          IF    swap bind childs  childs bind widgets
                self childs bind parent
          ELSE  childs append  THEN  1 n +! ( resized ) ;

        : ?nodraw ( -- flag )  attribs c@ :flip and 0<>
	  flags #hidden bit@ or ;

\ combined widgets                                     29aug99py

        : (font!  ALLCHILDS  dup font! ;
        : font! ( font -- )  (font! drop
          dpy self IF  !resized  THEN ;

        : ?2b ( c n -- )  twoborders < IF  2 +  THEN ;

        : draw ( -- )   borderw c@  0= ?EXIT
          shadow borderw cx@ xS * 2/ xywh    attribs c@
          $F0 and 0= IF  widget :: drawshadow  EXIT  THEN

\ combined widgets                                     25mar99py

	  { lc sc n x y w h } n 0< IF  lc sc to lc to sc  THEN
	  n abs 0
          ?DO  attribs c@ :notshadow and 0=
               IF  x I + y I + w I 2* - 1- 1
                                          lc I ?2b dpy box  THEN
               attribs c@ :nolshadow and 0=
               IF  x I + y I + 1+ 1 h I 2* - 2-
                                          lc I ?2b dpy box  THEN
               attribs c@ :norshadow and 0=
               IF  x w + I - 1- y I + 1 h i 2* -
                                          sc I ?2b dpy box  THEN
               attribs c@ :nobshadow and 0=
               IF  x I + y h + I - 1- w I - 1
                                          sc I ?2b dpy box  THEN
          LOOP ;

\ combined widgets                                     21mar00py

        2Variable xy
        Variable +skip          Variable /skip
        Variable sclip          Variable eclip
        : x+ ( dx -- )  xy cell+ +! ;
        : y+ ( dy -- )  xy +! ;
        : maxglue ( g1 g2 -- g )  rot min -rot max swap ;
        : gsize   ( g1 g2 -- g n )  swap
          2over 2over >r 2* rot ?dup  IF  */  ELSE  min  THEN
          1+ 2/ >r drop negate r@ negate p+ r> r> + ;
        : >range  vglues 2@ range swap hglues 2@ range swap ;
        : resize  ( x y w h -- ) hglue@ 2drop vglue@ 2drop
          >range  h ! w ! repos  compose  2drop 2drop ;
        : close  active close ;

\ combined widgets: event handling                     09mar07py
        : hskip@  ( -- n )  hskip c@ dpy xrc xn @ * ;
        : vskip@  ( -- n )  vskip c@ dpy xrc xn @ * ;
        : -border ( x y w h -- x' y' w' h' )
          borderw cx@ abs xS * 2/ xywh- ;
        : dispose-childs  ALLCHILDS  dispose ;
        : tab-step-off  tab-steps HandleOff ;
        : dispose dispose-childs tab-step-off super dispose ;
        : focus    ?nodraw ?EXIT  ALLCHILDS  focus   ;
        : defocus  ?nodraw ?EXIT  ALLCHILDS  defocus ;
        : show     super show ?nodraw ?EXIT  ALLCHILDS  show ;
        : hide     super hide ALLCHILDS  hide ;
        : keyed    ( key sh -- )  active keyed ;
        : handle-key? active handle-key? ;
        : !resized  0, hglues 2!  0, vglues 2!  tab-step-off
          ALLCHILDS !resized ;

\ combined widgets: event handling                     19oct99py

        : (moved ( x y -- x y flag )
          2dup inside? dup 0= ?EXIT  0 and
          ALLCHILDS  drop  2dup inside?
          IF  rdrop 2dup moved -1 o> rdrop  ELSE  0  THEN ;
        : moved ( x y -- )  (moved
          0= IF  mouse_cursor dpy set-cursor  THEN  2drop ;

        : (clicked  ( x y b n -- x y b n / -1 )  2over inside?
          0= IF  2drop 2drop -1  EXIT  THEN
          ALLCHILDS  2over inside?
          IF  rdrop  clicked -1 o> rdrop  THEN ;
        : clicked   ( x y b n -- )  (clicked dup 0<
          IF  drop  ELSE  >released  THEN ;

\ combined widgets: active point                       21mar00py

        : next-active ( -- flag )
          active self 'nil =
          IF    childs self
          ELSE  active next-active  dup ?EXIT  drop
                active widgets self
          THEN  >o
          BEGIN  ^ 'nil <>  WHILE
                 handle-key?  0=  WHILE
                 widgets self op!  REPEAT  THEN  ^ o>
          dup  bind active  'nil <> dup 0= ?EXIT
          active first-active ;

        : first-active  'nil bind active
	  [defined] VFXFORTH [ 0= ] [IF] next-active drop [THEN] ;

\ combined widgets: active point                       14sep97py
        : prev-active ( -- flag )
          active prev-active  dup  ?EXIT  drop
          active self dup >r  'nil bind active
          BEGIN  drop  active self  next-active  WHILE
                 active self  r@ =  UNTIL  THEN  rdrop
          dup bind active
          BEGIN  active next-active  0=  UNTIL
          active prev-active drop  'nil <> ;
        : inside? ( x y -- f )  borderw cx@ abs xS * 2/ >r
          r@ - y @ - h @ r@ 2* - 0max u< swap
          r@ - x @ - w @ r> 2* - 0max u< and ;
        : >inc ( glue o inc -- glue' )
          dup 1 =   IF  2drop  EXIT  THEN
          2 pick 0= IF  2drop  EXIT  THEN
          { o inc } o - dup negate inc mod + 0max o + ;

\ combined widgets                                     08apr00py
        : ?vfix  attribs c@ :vfix and 0= and ;
        : ?hfix  attribs c@ :hfix and 0= and ;
        : hglue@  hglues 2@ 2dup d0= IF 2drop hglue THEN ?hfix ;
        : vglue@  vglues 2@ 2dup d0= IF 2drop vglue THEN ?vfix ;
        : resized ( -- )  attribs c@ :resized and 0=
          IF    hglues 2@ hglue d=  vglues 2@ vglue d= and
                IF  xywh 2dup 2>r resize xywh 2swap 2drop
                    >range 2r> d=  IF  draw  EXIT  THEN  THEN
          ELSE  hglue 2drop vglue 2drop  THEN
          parent resized ;
        : flip  attribs c@ :flip and IF +flip ELSE -flip THEN ;
        : -flip  attribs c@ :flip or         attribs c!
          hide parent !resized parent resized ;
        : +flip  attribs c@ :flip invert and attribs c!
          parent !resized parent resized show draw ;

\ combined widgets                                     21mar00py

        0 Value bc
        : tabs ( -- n )  tab-steps @ 0= IF  0  EXIT  THEN
          tab-steps $@len 2/ cell/ ;
        : tab-size! ( n -- )
          tab-steps @ 0= IF  S" " tab-steps $!  THEN
          1+ 2* cells tab-steps $@len
          dup >r max tab-steps $!len
          tab-steps $@ r> /string erase ;
        : tab@ ( n -- glue )
          dup 0< IF  drop 0 0  EXIT  THEN
          dup tab-size! 2* cells tab-steps $@ drop + 2@ ;
        : tab! ( glue n -- ) dup >r tab@ rot max -rot max swap
          r> 2* cells tab-steps $@ drop + 2! ;
class;

\ public visible ALLCHILDS                             02jul00py

:  ALLCHILDS ( .. -- ..' ) r> swap >o >r combined childs self
   BEGIN  dup 'nil <>  WHILE
          r@ swap >o execute combined widgets self o>
   REPEAT  drop rdrop o> ;

\ hbox                                                 19dec99py

combined class hbox
how:    : >hglue ( -- min glue ) 0 0  ALLCHILDS hglue@ p+ ;
        : >vglue ( -- min glue ) 0 mi n @ 0<> and
          ALLCHILDS vglue@ maxglue ;
        : >vmax  ( g -- gmin )  ALLCHILDS  vglue@ + min ;
        : hskips+ ( n -- n' )
          n @ vskip@ IF  1+  ELSE  1-  THEN  0max  hskip@ * +
          borderw cx@ abs xS * + ;
        : hglue  attribs c@ :flip and
          IF    0 0
          ELSE  >hglue  swap  hskips+  swap
          THEN  2dup hglues 2! ?hfix ;
        : hglue@  attribs c@ :flip and
          IF  0 0  ELSE  super hglue@  THEN ;

\ hbox                                                 19dec99py

        : vskips+ ( n -- n' )
          vskip@ borderw cx@ abs xS * 2/ + 2* + ;
        : vglue ( -- glue )
          >vglue over + >vmax  swap
          1- yinc 2dup >r >r swap y @ - tuck
          >r >r - r> tuck / 1+ * r> + tuck -
          swap vskips+
          tuck + swap  r> r>  >inc tuck -
          dup 0< IF  + 0  THEN
          2dup vglues 2! ?vfix ;

\ hbox                                                 21oct99py
        : draw ( -- ) ?nodraw  ?EXIT  super draw
          sclip push  eclip push  +skip push  /skip push
          & bc push  dpy clipx  over + eclip ! sclip !
          hskip@ +skip !  vskip@ /skip !  defocuscol @ @ to bc
          xywh -border 2over 2over drop /skip @  bc dpy box
          /skip @ - rot + swap /skip @           bc dpy box
          /skip @
          IF xywh -border nip +skip @ swap bc dpy box THEN
          ALLCHILDS xywh drop +skip @ + nip over +
                    eclip @ umin sclip @ umax swap
                    eclip @ umin sclip @ umax u<= ?EXIT
depth 1+ >r ['] draw catch depth r> <> or IF .class THEN
                    /skip @ 0= IF widgets self 'nil = ?EXIT THEN
                    +skip @ 0= ?EXIT  x @ w @ + y @ +skip @ h @
                    bc dpy box ;

\ hbox                                                 19dec99py

        : compose  ( -- g1 g2 )  xy dup push cell+ push
          +skip push  hskip@ +skip !  borderw cx@ abs xS * 2/ >r
          x @ r@ + vskip@ 0= IF  hskip@ -  THEN
          y @ r@ + vskip@ + xy 2!
          hglues 2@ swap negate w @ +
          vglues 2@ swap h @ max vskip@ 2* - r> 2* -
          ALLCHILDS 2swap +skip @ 0 gsize x+  hglue@ gsize >r
                    2swap r@ over xy 2@ 2swap resize
                    r> x+ ;

\ hbox                                                 21oct99py

        : xinc  0 1  attribs c@ :flip and ?EXIT
          swap hskips+ swap
          ALLCHILDS  xinc { mi o i }  i 1 <>
          IF    o +  mi i max
          ELSE  hglue@ drop + mi  THEN ;
        : yinc  0 1  attribs c@ :flip and ?EXIT
          swap vskips+ swap
          ALLCHILDS  yinc { mi o i }  i 1 <>
          IF  o +  mi i max  ELSE  mi  THEN ;
class;

\ vbox                                                 19dec99py
combined class vbox
how:    : >hglue ( -- min glue ) 0 mi n @ 0<> and
          ALLCHILDS hglue@ maxglue ;
        : >vglue ( -- min glue ) 0 0  ALLCHILDS vglue@ p+ ;
        : >hmax  ( g -- gmin )  ALLCHILDS  hglue@ + min ;
        : vskips+ ( n -- n' )
          n @ hskip@ IF  1+  ELSE  1-  THEN  0max  vskip@ * +
          borderw cx@ abs xS * + ;
        : vglue  attribs c@ :flip and
          IF    0 0
          ELSE  >vglue  swap  vskips+ swap
          THEN  2dup vglues 2! ?vfix ;
        : vglue@  attribs c@ :flip and
          IF  0 0  ELSE  super vglue@  THEN ;

\ vbox                                                 19dec99py

       : >hglue' ( -- min glue ) 0 mi n @ 0<> and
          ALLCHILDS hglue maxglue ;
        : hskips+ ( n -- n' )
          hskip@ borderw cx@ abs xS * 2/ + 2* + ;
        : hglue ( -- glue )  tab-step-off  0,
          BEGIN   2drop tabs 1- tab@ >r >r  >hglue
                  r> r> tabs 1- tab@ d= UNTIL
          over + >hmax  swap
          1- xinc 2dup >r >r swap x @ - tuck
          >r >r - r> tuck / 1+ * r> + tuck -
          swap hskips+
          tuck + swap  r> r>  >inc tuck -
          dup 0< IF  + 0  THEN
          2dup hglues 2! ?hfix ;

\ vbox                                                 21oct99py
        : draw ( -- ) ?nodraw  ?EXIT  super draw
          sclip push  eclip push  +skip push  /skip push
          & bc push  dpy clipy  over + eclip ! sclip !
          vskip@ +skip !  hskip@ /skip !  defocuscol @ @ to bc
          xywh -border 2over 2over nip /skip @ swap bc dpy box
          >r /skip @ - rot + swap /skip @ r>        bc dpy box
          /skip @
          IF xywh -border drop +skip @ bc dpy box  THEN
          ALLCHILDS xywh xS + +skip @ + nip rot drop over +
                    eclip @ umin sclip @ umax swap
                    eclip @ umin sclip @ umax u<= ?EXIT
depth 1+ >r ['] draw catch depth r> <> or IF .class THEN
                    /skip @ 0= IF widgets self 'nil = ?EXIT THEN
                    +skip @ 0= ?EXIT  x @ y @ h @ + w @ +skip @
                    bc dpy box ;

\ vbox                                                 19dec99py

        : compose  ( -- g1 g2 )  xy dup push cell+ push
          +skip push  vskip@ +skip !  borderw cx@ abs xS * 2/ >r
          x @ r@ + hskip@ +
          y @ r@ + hskip@ 0= IF  vskip@ -  THEN  xy 2!
          hglues 2@ swap w @ max hskip@ 2* - r> 2* -
          vglues 2@ swap negate h @ +
          ALLCHILDS  +skip @ 0 gsize y+ vglue@ gsize >r
                     2swap dup r@ xy 2@ 2swap resize 2swap
                     r> y+ ;

\ vbox                                                 14sep97py

        : yinc  0 1  attribs c@ :flip and ?EXIT
          swap vskips+ swap
          ALLCHILDS  yinc { mi o i }  i 1 <>
          IF    o +  mi i max
          ELSE  vglue@ drop +  mi  THEN ;
        : xinc  0 1  attribs c@ :flip and ?EXIT
          swap hskips+ swap
          ALLCHILDS  xinc { mi o i }  i 1 <>
          IF  o +  mi i max  ELSE  mi  THEN ;
class;

\ box management                                       08apr00py
: borderbox ( o n -- o )  swap
  combined with  borderw c! ^ endwith ;
: noborderbox ( o flags -- o )  swap
  combined with  attribs dup c@ rot   or swap c! ^ endwith ;
: hfixbox ( o -- o )
  combined with  attribs dup c@ :hfix or swap c! ^ endwith ;
: vfixbox ( o -- o )
  combined with  attribs dup c@ :vfix or swap c! ^ endwith ;
: fixbox ( o -- o )
  combined with  attribs dup c@ [ :hfix :vfix or ] Literal
                                      or swap c! ^ endwith ;
: flipbox ( o -- o )
  combined with  attribs dup c@ :flip xor swap c! ^ endwith ;
: rzbox ( o -- o )  combined with
      attribs dup c@ :resized xor swap c! ^ endwith ;

\ box management                                       07dec97py

: hskips ( o n -- o )  swap
  combined with  hskip c!             ^ endwith ;
: vskips ( o n -- o )  swap
  combined with  vskip c!             ^ endwith ;

: hskip ( o -- o )  1 hskips ;
: vskip ( o -- o )  1 vskips ;
: panel ( o -- o )  hskip vskip ;

\ boxes with focus                                     21mar00py
[defined] VFXFORTH 0= [IF]
    ' noop Alias component immediate
[THEN]
vbox class vabox
how:    : focus   ( -- )   attribs c@ :flip and
          0= IF  active focus    THEN ;
        : defocus ( -- )  attribs c@ :flip and
          0= IF  active defocus  THEN ;
        : show-you ( -- ) active show-you ;
        : keyed ( key sh -- ) over dup $0009 = swap $FE20 = or
          IF  defocus  nip 1 and
              IF    prev-active 0=  IF  prev-active drop  THEN
              ELSE  next-active 0=  IF  next-active drop  THEN
              THEN  focus show-you  EXIT
          THEN  super keyed ;
        : handle-key?  attribs c@ :flip and IF false  EXIT THEN
          0  ALLCHILDS  handle-key? or ;

\ boxes with focus                                     11nov06py

        : (clicked  ( x y b n -- x y b n / -1 )
          ALLCHILDS  2over inside?
          IF  rdrop over $18 and 0= handle-key? and
              IF  ^ r@ >o  dup active self <>
                  IF    defocus bind active  focus
                  ELSE  drop  THEN  o>  THEN
              clicked -1 o> rdrop  THEN ;
class;
[defined] VFXFORTH [IF]
    synonym component vabox
[THEN]

hbox class habox
how:    vabox :: (clicked       vabox :: show-you
        vabox :: focus          vabox :: defocus
        vabox :: keyed          vabox :: handle-key?
class;

\ tabulator box                                        19dec99py

hbox class htbox
how:    Create minmax 0 , 0 ,
        : >hglues ( -- min max )  0 n @ 0<>
          ALLCHILDS   & glue @ class? 0=
             IF  >r hglue@ over + >r umax r> r> umin  THEN ;
        : >hglue+ ( -- min glue )
          >hglues over - minmax 2!
          0 0  ALLCHILDS  & glue @ class?
            IF  hglue@  ELSE  minmax 2@  THEN  p+ ;
        : hglue  attribs c@ :flip and  IF  0 0
          ELSE  >hglue+  swap  n @ vskip@ IF  1+  ELSE  1-  THEN
                0max  hskip@ * +  borderw cx@ abs xS * +  swap
          THEN  2dup hglues 2! ?hfix ;

\ tabulator box                                        28mar99py
        : compose  ( -- g1 g2 )
          minmax dup push cell+ push
          >hglues over - minmax 2!
          xy     dup push cell+ push
          +skip push  hskip@ +skip !  borderw cx@ abs xS * 2/ >r
          x @ r@ + vskip@ 0= IF  hskip@ -  THEN
          y @ r@ + vskip@ + xy 2!
          hglues 2@ swap negate w @ +
          vglues 2@ swap h @ max vskip@ 2* - r> 2* -
          ALLCHILDS  2swap +skip @ 0 gsize x+
                     & glue @ class?
                     IF  hglue@  ELSE  minmax 2@  THEN
                     gsize >r
                     2swap r@ over xy 2@ 2swap resize
                     r> x+ ;                    class;

\ tabulator box                                        19dec99py

vbox class vtbox
how:    Create minmax 0 , 0 ,
        : >vglues ( -- min max )  0 n @ 0<>
          ALLCHILDS   & glue @ class? 0=
             IF  >r vglue over + >r umax r> r> umin  THEN ;
        : >vglue+ ( -- min glue )
          >vglues over - minmax 2!
          0 0  ALLCHILDS  & glue @ class?
            IF  vglue  ELSE  minmax 2@  THEN  p+ ;
        : vglue  attribs c@ :flip and  IF  0 0
          ELSE  >vglue+  swap  n @ hskip@ IF  1+  ELSE  1-  THEN
                0max  vskip@ * +  borderw cx@ abs xS * + swap
          THEN  2dup vglues 2! ?vfix ;

\ tabulator box                                        28mar99py
        : compose  ( -- g1 g2 )
          minmax dup push cell+ push
          >vglues over - minmax 2!
          xy     dup push cell+ push
          +skip push  vskip@ +skip !  borderw cx@ abs xS * 2/ >r
          x @ r@ + hskip@ +
          y @ r@ + hskip@ 0= IF  vskip@ -  THEN  xy 2!
          hglues 2@ swap w @ max hskip@ 2* - r> 2* -
          vglues 2@ swap negate h @ +
          ALLCHILDS  +skip @ 0 gsize y+
                     & glue @ class?
                     IF  vglue@  ELSE  minmax 2@  THEN
                     gsize >r
                     2swap dup r@ xy 2@ 2swap resize 2swap
                     r> y+ ;                    class;

\ htbox variants                                       21may97py

htbox class hatbox
how:    habox :: (clicked       habox :: show-you
        habox :: focus          habox :: defocus
        habox :: keyed          habox :: handle-key?
class;

vtbox class vatbox
how:    vabox :: (clicked       vabox :: show-you
        vabox :: focus          vabox :: defocus
        vabox :: keyed          vabox :: handle-key?
class;

\ table boxes                                          19dec99py

hbox class htab
how:    : >htglue ( -- n )
          0  ALLCHILDS  { n }
          hglue@ parent parent with
                 n 1- tab@ p+ n tab! endwith
          n 1+ ;
        : hglue  attribs c@ :flip and  IF  0 0
          ELSE  >htglue drop parent with tabs 1- tab@ endwith
                swap hskips+ swap
          THEN  2dup hglues 2! ?hfix ;
        : hglue@  hglue ;
        : resized  parent resized ;

\ table boxes                                          19dec99py
        Variable >I
        : compose  ( -- g1 g2 )
          xy dup push cell+ push  >I push  >I off
          +skip push  hskip@ +skip !  borderw cx@ abs xS * 2/ >r
          x @ r@ + vskip@ 0= IF  hskip@ -  THEN
          y @ r@ + vskip@ + xy 2!
          hglues 2@ swap negate w @ +
          vglues 2@ swap h @ max vskip@ 2* - r> 2* -
          ALLCHILDS  2swap +skip @ 0 gsize x+
                     parent parent with >I @ dup
                        >r tab@ r> 1- tab@ p- endwith
                     gsize >r
                     2swap r@ over xy 2@ 2swap resize
                     r> x+  1 >I +! ;
class;

\ table boxes                                          26apr01py

vbox class vtab
how:    : >vtglue ( -- n )
          0  ALLCHILDS  { n }
          vglue@ parent parent with
                 n 1- tab@ p+ n tab! endwith
          n 1+ ;
        : vglue  attribs c@ :flip and  IF  0 0
          ELSE  >vtglue drop parent with tabs 1- tab@ endwith
                swap vskips+ swap
          THEN  2dup vglues 2! ?vfix ;
        : vglue@  vglue ;
        : resized  parent resized ;

\ table boxes                                          19dec99py
        Variable >I
        : compose  ( -- g1 g2 )
          xy dup push cell+ push  >I push  >I off
          +skip push  vskip@ +skip !  borderw cx@ abs xS * 2/ >r
          x @ r@ + hskip@ +
          y @ r@ + hskip@ 0= IF  vskip@ -  THEN  xy 2!
          hglues 2@ swap w @ max hskip@ 2* - r> 2* -
          vglues 2@ swap negate h @ +
          ALLCHILDS  +skip @ 0 gsize y+
                     parent parent with >I @ dup
                       >r tab@ r> 1- tab@ p- endwith
                     gsize >r
                     2swap dup r@ xy 2@ 2swap resize 2swap
                     r> y+  1 >I +! ;
class;

\ table boxes                                          19dec99py

htab class hatab
how:    habox :: (clicked       habox :: show-you
        habox :: focus          habox :: defocus
        habox :: keyed          habox :: handle-key?
class;

vtab class vatab
how:    vabox :: (clicked       vabox :: show-you
        vabox :: focus          vabox :: defocus
        vabox :: keyed          vabox :: handle-key?
class;

\ parbox                                               10aug05py

: indent-glue  widget xM 0      *hglue new ;
: null-glue    0 0              *hglue new ;
: fill-glue    0 1 *fill        *hglue new ;
: space-glue   widget xN dup    *hglue new ;

[defined] T] [IF]
Create block-par
 T] indent-glue  null-glue  null-glue  fill-glue  space-glue [
\   first-left   right      left       last-right space
Create center-par
 T] fill-glue    fill-glue  fill-glue  fill-glue  space-glue [
Create left-par
 T] indent-glue  fill-glue  null-glue  fill-glue  space-glue [
Create right-par
 T] fill-glue    null-glue  fill-glue  null-glue  space-glue [
[ELSE]
Create block-par
 ' indent-glue , ' null-glue , ' null-glue , ' fill-glue , ' space-glue ,
\   first-left   right      left       last-right space
Create center-par
 ' fill-glue , ' fill-glue , ' fill-glue , ' fill-glue , ' space-glue ,
Create left-par
 ' indent-glue , ' fill-glue , ' null-glue , ' fill-glue , ' space-glue ,
Create right-par
 ' fill-glue , ' null-glue , ' fill-glue , ' null-glue , ' space-glue ,
[THEN]

\ parbox                                               19mar00py

[defined] VFXFORTH 0= [IF] \ !!!FIXME: missing array operators
vbox class parbox
public:
    cell var glues
    cell var n'
    gadget [] items
    method hboxing
    method unhbox
    static sub-box
how:
    & hbox @ sub-box !
    : init ( widget_1 .. widget_n n format -- )
      glues ! dup n' ! [],  over bind[] items
      ?DO  I !  -cell +LOOP
      0 hboxing super init ;

\ parbox                                               19mar00py

    : hglue  ( -- glue )  super hglue 2drop
      0 n' @ 0 ?DO  I items hglue drop max  LOOP  1 *fill ;
    : vglue  ( -- glue )  super vglue drop 0 ;
    : hglue@  hglue ;
    : vglue@  super vglue@ drop 0 ;

/*
    : unbox ( parbox i -- parbox i )
      ALLCHILDS  dup 0  ?DO  I over2 with items self endwith
                             link childs delete  LOOP ;
    : dispose-childs  ALLCHILDS  dispose ;
    : unhbox  ( -- parbox i' )
      self n' @  unbox  dispose-childs ;
*/
\ parbox                                               27may00py

    : check-hbox ( w -- w )
      ALLCHILDS  hglue + over <
                 IF  fill-glue 'nil add
                     1 n +!  hglue 2drop  THEN ;

    : unglue ( parbox i -- parbox i' )
      ALLCHILDS  over with dup items self endwith ^ <>
                 IF  dispose  ELSE  1+  THEN ;
    : unhbox  ( -- parbox i' )
      self 0 ALLCHILDS
      unglue  'nil bind childs  dispose ;

\ parbox                                               27jun02py

    : resize  ( x y w h -- )  h @ >r
      h ! w @ over w ! <> dpy self and
      IF  unhbox 2drop w @ hboxing >box
          dpy self dpy! w @ check-hbox drop
          vglue drop h !
      THEN  w @ h @ super resize
      r> 0= IF  parent resized  THEN ;
    : dispose
      unhbox 2drop  dispose[] items  'nil bind childs
      super dispose ;

\ formating child boxes                                19mar00py

    : glue' ( n -- glue )  cells glues @ + perform ;
    : glueW ( widget -- n )  widget with hglue endwith drop ;
    : disposeW ( widget -- ) widget with dispose endwith ;
    : hbox-new  sub-box @ new, ;

\ formating child boxes                                19mar00py
    : hboxing ( width -- box1 .. boxn n )
      dpy self 0= IF  drop
         n' @ 0 ?DO  I items self  LOOP  n' @ hbox-new 1  EXIT
      THEN
      dup 0 0  0 glue' { w w0 p q sp }
      n' @ 0 ?DO
          I items hglue drop sp glueW + w0 <
          IF    sp I items self  p 2+ to p
                w0 I items hglue drop sp glueW + - to w0
          ELSE  p IF  1 glue' p 1+ hbox-new  q 1+ to q  THEN
                sp disposeW  2 glue' I items self  2 to p
                over glueW over glueW + w swap - to w0
          THEN  4 glue' to sp
      LOOP  sp disposeW  3 glue' p 1+ hbox-new q 1+ ;
class;

\ formating child boxes                                19mar00py

parbox class parabox
how:
    & habox @ sub-box !
    vabox :: (clicked
    vabox :: show-you
    vabox :: focus
    vabox :: defocus
    vabox :: keyed
    vabox :: handle-key?
class;
[THEN]

