\ component                                            04mar00py

[defined] VFXFORTH 0= [IF]
s" COMPONENT" pad place  bl pad count + c!
' vabox pad context @ (find drop (name> !
[THEN]
: get-win ( -- win )  & displays @ object class?
  IF  displays get-win  ELSE  widget dpy get-win  THEN ;
: new-component ( o od addr u -- o )
  >r >r  1 swap modal new  r> r>
  screen self window new  window with  assign ^  endwith ;
: open-component    ( o od addr u -- )
  new-component  window with  show  endwith ;
: open-dialog       ( o od addr u -- )
  new-component  get-win
  swap window with  set-parent show  endwith ;
: open-application  ( o od addr u -- )
  new-component  window with  show up@ app !  endwith ;

\ OpenGL canvas                                        22jun02py

[defined] VFXFORTH 0= [IF]

opengl also glconst also

[defined] win32 [IF]
        | Create pfd  sizeof PIXELFORMATDESCRIPTOR w, 1 w,
          0 ( PFD_DRAW_TO_WINDOW or ) PFD_DRAW_TO_BITMAP or
          PFD_SUPPORT_OPENGL or \ PFD_SUPPORT_GDI or
          ( PFD_DOUBLEBUFFER or ) ,
          PFD_TYPE_RGBA c,
          &24 c, 0 c, 0 c, 0 c, 0 c, 0 c, 0 c, 0 c, 0 c, 0 c,
          0 c, 0 c, 0 c, 0 c, &32 c, 0 c, 0 c,
          PFD_MAIN_PLANE c, 0 c, 0 , 0 , 0 ,
        | Create bih sizeof BITMAPINFOHEADER ,
          0 , 0 , 1 w, &24 w, BI_RGB , 0 , 0 , 0 , 0 , 0 ,
[THEN]

\ OpenGL canvas                                        15aug99py

0 Value canvas-mode

glue class glcanvas
public: defer drawer            method render
        cell var visinfo        cell var pixmap
        cell var ctx            cell var glxpm
        cell var glxwin         cell var rendered
        window-stub ptr stub    cell var shown
[defined] win32 [IF]
        cell var oldbm          cell var dibptr
[THEN] [defined] x11 [IF]
        cell var cmap
[THEN]
        widget ptr outer

\ OpenGL canvas                                        08jul00py
how:
[defined] x11 [IF]
        | Create attrib GLX_DOUBLEBUFFER ,
                        GLX_RGBA ,
                        GLX_RED_SIZE   ,   1 ,
                        GLX_GREEN_SIZE ,   1 ,
                        GLX_BLUE_SIZE  ,   1 ,
                        GLX_DEPTH_SIZE ,  $10 ,  0 ,
        : init  ( exec actor w w+ h h+ -- )
          super init  >callback  IS drawer  ^^ bind outer ;
        : dpy!  super dpy!
          dpy xrc with dpy @ screen @ endwith
          attrib canvas-mode 1 and cells +
          glXChooseVisual visinfo !
          dpy xrc dpy @ visinfo @ 0 1 glXCreateContext ctx ! ;

\ OpenGL canvas                                        09dec07py
        : destroy-pixmap ( -- ) dpy xrc dpy @
          glxwin @ ?dup  IF  over swap XDestroyWindow drop
                             glxwin off  THEN
          glxpm  @ ?dup  IF  over swap glXDestroyGLXPixmap
                             glxpm  off  THEN
          pixmap @ ?dup  IF  over swap XFreePixmap
                             pixmap off  THEN
          cmap   @ ?dup  IF  over swap XFreeColormap
                             cmap   off  THEN  drop ;
        : set-context ( -- )
          dpy xrc dpy @ glxpm @ glxwin @ or
          ctx @ glXMakeCurrent drop ;
        : dpyscreen ( -- dpy screen )
          dpy xrc dpy @ visinfo @ XVisualInfo screen @ ;

\ OpenGL canvas                                        09jan00py
        : new-window   xswa sizeof XSetWindowAttributes erase
          AllocNone visinfo @ XVisualInfo visual @
          dup dpy xrc vis @ <> canvas-mode 4 and or
          IF    dpy drawable' drop 2swap swap XCreateColormap dup cmap !
          ELSE  2drop dpy xrc cmap @  THEN
              xswa XSetWindowAttributes colormap !
          dpyscreen BlackPixel dup
              xswa XSetWindowAttributes border_pixel !
              xswa XSetWindowAttributes background_pixel !

	  event-mask  xswa XSetWindowAttributes event_mask !

	  dpy xrc dpy @ dpy get-win
	  x @ y @ w @ 1 max h @ 1 max
	  0           visinfo @ XVisualInfo depth  @
	  InputOutput visinfo @ XVisualInfo visual @
	  glxvals xswa XCreateWindow
          self over window-stub new bind stub ;

\ OpenGL canvas                                        09dec07py

        : new-pixmap ( -- )  glxwin @ ?EXIT  glxpm @ ?EXIT
          dpy xwin @ dpy get-win = canvas-mode 2 and 0= and  IF
              new-window glxwin ! rendered off  EXIT THEN
	  dpy xrc dpy @ dpy get-win
          w @ 4 max 3 + -4 and h @ 4 max
          visinfo @ XVisualInfo depth @
	  XCreatePixmap dup pixmap !
          dpy xrc dpy @ visinfo @ rot glxCreateGLXPixmap
          glxpm ! rendered off ;
        : show ( -- )  shown @ shown on ?EXIT
          new-pixmap stub self 0= ?EXIT
          xywh stub resize stub show ;
        : hide ( -- )  shown @ shown off 0= ?EXIT
          stub self 0= ?EXIT  stub hide ;
[THEN]

\ OpenGL canvas                                        23sep99py
[defined] win32 [IF]
        : set-context ctx @ pixmap @ wglMakeCurrent ?err ;
        : add-dib-section  h @ 1 max w @ 1 max  bih cell+ 2!
          0 0 0 DIB_RGB_COLORS bih
          pixmap @ CreateDIBSection dup ?err glxpm !
          glxpm @ pixmap @ SelectObject dup ?err oldbm !
          pfd dup pixmap @ ChoosePixelFormat dup ?err
          pixmap @ SetPixelFormat ?err ;
        : new-pixmap ( -- )  0 0 wglMakeCurrent drop
          screen xrc dc @ CreateCompatibleDC dup ?err pixmap !
          add-dib-section
          pixmap @ wglCreateContext dup ?err ctx !
          rendered off ;
        : init  ( exec actor w w+ h h+ -- )
          super init  >callback  IS drawer  ^^ bind outer ;

\ OpenGL canvas                                        01nov06py

        : destroy-pixmap ( -- )
          ctx    @ ?dup  IF  0 0 wglMakeCurrent drop
                             wglDeleteContext drop ctx off THEN
          pixmap @ ?dup  IF  DeleteObject drop pixmap off THEN
          glxpm  @ ?dup  IF  DeleteObject drop glxpm  off THEN ;

[THEN]

\ OpenGL canvas                                        09dec07py

        : resize ( x y w h -- )
          super resize rendered off
[defined] win32 [IF]
          oldbm @ pixmap @ SelectObject ?err
          glxpm  @ ?dup  IF  DeleteObject drop glxpm  off THEN
          add-dib-section
[ELSE]    glxpm  @   IF  destroy-pixmap  THEN  new-pixmap
          stub self  IF  xywh stub resize  stub show  THEN
[THEN]  ;
        : dispose destroy-pixmap  [defined] x11 [IF]
          ctx @ ?dup  IF
              dpy xrc dpy @ swap glXDestroyContext  THEN
[THEN]    stub self IF  stub dispose  THEN  glFlush
          super dispose ;

\ OpenGL canvas                                        08dec07py

        : render ( -- ) \ ." render "
          pixmap @ glxwin @ or 0= IF  new-pixmap  THEN
          set-context ^ drawer  glFlush
[defined] x11 [IF]
          glxpm @
          IF  dpy xrc dpy @ glxpm @ glXSwapBuffers  THEN
[THEN]    rendered on ;

\ OpenGL canvas                                        22oct06py

        : draw ( -- )
          rendered @ 0=  IF  render  THEN
[defined] x11 [IF]   pixmap @
          IF    0 0 xywh 2swap pixmap @ dpy image
          ELSE  glxwin @
            IF  dpy xrc dpy @ glxwin @ glXSwapBuffers
                rendered off  THEN
          THEN
[THEN]
[defined] x11_ximage [IF]   0 0 xywh 2swap 0 sp@ >r 0 sp@ r>
          pixmap @ dpy xrc dpy @ XMesaFindBuffer
          XMesaGetBackBuffer drop nip dpy ximage  [THEN]
[defined] win32 [IF]   0 0 xywh 2swap pixmap @ dpy image  [THEN]
        ;

\ OpenGL canvas                                        04aug05py

        boxchar :: clicked ( x y b n -- )
        boxchar :: keyed ( key sh -- )
        : moved ( x y -- ) 2drop  stub self
          IF    mouse_cursor stub set-cursor  ^ stub set-rect
          ELSE  mouse_cursor dpy  set-cursor  ^ dpy  set-rect
          THEN  callback enter ;
        boxchar :: leave ( -- )
class;

\ canvas                                               11jul99py

previous previous

: CV[  postpone :[ canvas postpone with ;        immediate
: ]CV  canvas postpone endwith  postpone ]: ;    immediate
: GL[  postpone :[ glcanvas postpone with ;        immediate
: ]GL  glcanvas postpone endwith  postpone ]: ;    immediate

\ helper words for Theseus                             21sep07py
[THEN]

: T"   postpone S" ;                             immediate

: ^^bind  postpone dup  postpone bind ;      immediate restrict

[defined] VFXFORTH 0= [IF]

\ IO-Window                                            26oct99py

: scan8 ( addr u -- addr u' )  2dup bounds
  ?DO  I c@ $80 and IF  drop I over - LEAVE  THEN  LOOP ;
: scan16 ( addr u -- addr' u' )  bounds  scratch 0 2swap
  ?DO  I c@ $80 and 0= ?LEAVE
       2dup + I c@ $7F and 8 << I 1+ c@ or
       swap w! 2+  2 +LOOP ;

\ IO-Window                                            12mar00py

0 Value do-scroll

boxchar class terminal
public: cell var cols           cell var rows
        cell var color          cell var cursor#
        cell var pos            cell var selw
        cell var keys           cell var start
        cell var scrolls        cell var typebuf
        cell var maxrows        cell var minrows
        cell var addr           cell var u
        1 var resize!           1 var flush!
        2 var text-color
        cell var sizew          font ptr fnt16
        & dpy viewport asptr vdpy

\ IO-Window                                            24oct99py

        method type             method page
        method emit             method flush
        method decode           method clrline
        method cr               method c
        method atxy?            method drawcur
        method at?              method at
        method curoff           method curon
        method key?             method key
        method 'start           method 'line
        method scrollup         method scrollback
        method paste-selection
        early showtext          early curpos
        early .text

\ IO-Window                                            06feb00py

how:    6 colors focuscol !     1 colors defocuscol !
        : assign ( w h -- )  1 max rows ! 1 max cols !
          rows @ maxrows !  rows @ minrows !
          typebuf  HandleOff
          start    HandleOff
          cols @ cell+ typebuf Handle!  typebuf @ off
          rows @ 1+ cols @ * start 2dup Handle! @ swap bl fill
          1 selw !  dpy self IF  resized  THEN ;

\ IO-Window                                            05jan07py
        : 'start ( -- addr ) start @
          scrolls @ cols @ * + ;
        : 'line  ( n -- addr u )
          scrolls @ cols @ * dup >r + rows @ cols @ * mod r> -
          'start + cols @ -trailing ;
        : !resized  s" M" !textwh
          4 dpy xrc font@ bind fnt16 ;
        : !tile  0 scrolls @ texth @ * negate dpy txy! ;
        : focus    focuscol   @ @ dup 8 >> swap $FF and 8 << or
                                  color !  drawcur super focus ;
        : defocus  defocuscol @ @ color !  drawcur ;
        : dpy! ( dpy -- )  widget :: dpy!
          fnt   self 0= IF  1 dpy xrc font@ font!       THEN
          fnt16 self 0= IF  4 dpy xrc font@ bind fnt16  THEN ;

\ mixed font output                                    24oct99py

        : .texts ( addr u x y dpy -- )
          fnt16 self 0= IF  fnt draw  EXIT  THEN  { x y dpy |
          BEGIN  dup  WHILE  2dup scan8 dup
                 IF    tuck x y dpy fnt draw
                       dup textwh @ * x + to x /string
                 ELSE  2drop  THEN
                 2dup scan16 dup
                 IF    tuck x y dpy fnt16 draw
                       dup textwh @ * x + to x /string
                 ELSE  2drop  THEN  REPEAT  2drop } ;

\ mixed font output                                    16jan05py

        : font-color! ( c dpy -- )
          over fnt color !  displays with  set-color  endwith ;
        : display-texts ( x y dpy -- )
          >r text-color @ r@ font-color!
          addr @ u @ 2swap r> .texts ;
        : .text ( addr u x y c -- )
          text-color ! 2swap u ! addr !
          ^ ['] display-texts dpy drawer ;

\ mixed font output                                    05may07py

        : expand16 ( -- )  maxascii $80 = IF
             selw @ pos @ 'line drop
             dup 1+ xchar- tuck - negate pos +!
             dup xchar+ swap - max 1 max selw !  EXIT
          THEN  fnt16 self 0= ?EXIT
          pos @ 1- 0max 'line drop c@ $80 and
          IF  -1 pos +!  1 selw +!  THEN
          pos @ selw @ 1- + 0max 'line drop c@ $80 and
          IF  1 selw +!  THEN ;
        : csize ( s i -- size )
          dup >r - 0max r> 'line rot 2dup swap - 0max >r
          min x-width r> +
          textwh @ * ;

\ IO-Window                                            20oct06py
        : drawcur  dpy self 0= ?EXIT  !tile  expand16
          cursor# @  IF  6 colors @  ELSE  color @  THEN
          pos @ typebuf @ @ +
          dup selw @ + 2dup min -rot max { color s e |
          x @ y @ cols @ rows @ * 0
          ?DO  s I - cols @ u<  e I - cols @ u< or
               I s e within or
               IF over s I csize dup >r + over r>
                  w @ e I csize min swap - 1 max
                  texth @ color dpy box
                  I 'line e I - 0max min s I - 0max
                  /string  2over swap s I csize +
                  swap color 8 >> .text
               THEN  texth @ + cols @ +LOOP
          2drop } ;

\ IO-Window                                            16jun02py

        : draw-io ( x y dpyo -- )
          dup displays with clipy endwith
          over + { dpyo sclip eclip |
          cols @ rows @ * 0
          ?DO  dup sclip eclip within
               IF  2dup w @ texth @
                   6 colors @ dpyo displays with box endwith
                   I 'line 2over 6 colors @ 8 >>
                   dpyo font-color!
                   dpyo .texts
               THEN  texth @ + cols @ +LOOP  2drop } ;
        : draw ( -- )  !tile
          x @ y @ ^ ['] draw-io dpy drawer
          drawcur  0 0 dpy txy! ;

\ IO-Window                                            12mar00py

        : resize-it2 ( -- )
          0 resize! c!  sizew off  parent resized  show-you ;
        : resize-it ( -- )
          vdpy sw @ cols @ textwh @ * min sizew !
          parent resized  dpy set-hints
          ['] resize-it2 ^ /step @ after dpy schedule ;
        : screen-resize
          start rows @ $20 + -$20 and
          cols @ * SetHandleSize
          resize! c@ ?EXIT  1 resize! c!
          ['] resize-it ^ /step @ after dpy schedule ;

        : xinc ( -- o inc ) sizew @ textwh @ ;
        : yinc ( -- o inc ) 0       texth @ ;

\ IO-Window                                            12mar00py

        : redraw-it ( -- )  0 resize! c!  draw ;
        : screen-redraw
          resize! c@ ?EXIT  1 resize! c!
          ['] redraw-it ^ /step @ after dpy schedule ;

\ IO-Window                                            12mar00py
        : scrollup ( -- )  rows @ maxrows @ <
          IF  1 rows +!   screen-resize
              cols @ rows @ 1- * 'line drop cols @ bl fill
              EXIT  THEN
          scrolls @ 1+ rows @ mod scrolls !
          cols @ dup negate pos +!
          cols @ rows @ 1- * 'line drop swap bl fill  do-scroll
          IF    x @ y @ texth @ dup >r +  dpy transback
                w @ h @ r> - x @ y @
                dpy get-win dpy image  !tile
                x @ y @ texth @ rows @ 1- * +
                w @ texth @ 6 colors @ dpy box
                dpy >exposed  0 0 dpy txy!
          ELSE  screen-redraw  THEN ;
        : scrollback ( n -- ) rows @ max maxrows ! ;

\ IO-Window                                            16jan05py
        : showtext ( addr u1 u2 -- )
          resize! c@ IF  drop 2drop  EXIT  THEN
          !tile drop cols @ >r  x @ y @
          at? drop 0 swap texth @ * p+
          2dup textwh 2@ r> * swap 6 colors @ dpy box
     pos @ at? nip - 'line 2swap 6 colors @ 8 >> .text 2drop ;
        : linetype ( addr u -- )
          tuck pos @ 'line drop swap 2dup -trailing >r drop move
          >r pos @ r@ + cols @ rows @ * >=
          IF  scrollup  THEN
          pos @ 'line drop r> r> over >r showtext  r> pos +! ;
        : vglue  rows @ texth @ *  0 ;
        : hglue  cols @ textwh @ *  0 ;
        : ?flush ( -- ) flush! c@ ?EXIT  1 flush! c!
          ['] flush ^ /step @ after dpy schedule ;

\ IO-Window                                            06jan05py
        : win-type  ( addr len -- )  cols @ >r
          BEGIN  dup pos @ r@ mod r@ - + dup 0>=  WHILE
                 tuck - >r over r@ + swap rot r> linetype REPEAT
          drop linetype rdrop  ;
        : type  ( addr len -- )  typebuf @ @ over + cols @ >=
          IF   flush curoff win-type curon
          ELSE ?flush tuck typebuf @ @+ + swap move typebuf @ +!
          THEN ;
        : emit  ( char -- )  char$ type ;
        : flush ( -- )  0 flush! c!  typebuf @ @
          IF  typebuf @ @+ swap
              curoff typebuf @ off  win-type  curon  THEN ;
        : moved ( x y -- )  2drop  ^ dpy set-rect
[defined] x11 [IF]            XC_xterm   [THEN]
[defined] win32 [IF]          IDC_IBEAM  [THEN]  dpy set-cursor  ;

\ IO-Window                                            12mar00py
        : page  ( -- )  flush curoff  pos off  typebuf @ off
          scrolls off  minrows @ rows !  screen-resize
          'start cols @ rows @ * bl fill  curon draw ;
        : at ( r c -- )  flush 0max cols @ 1- min
          swap 0max rows @ 1- min
          cols @ * + curoff pos ! curon ;
        : at? ( -- r c )
          pos @ typebuf @ @ + cols @ /mod swap ;
        : show-you ( -- ) dpy self 0= ?EXIT
          at? textwh 2@ rot * -rot * x @ y @ p+ dpy show-me ;
        : ?sel-scroll  ( c r -- c r )
          over textwh @ * over texth @ *
          x @ y @ p+ dpy scroll ;
        : curpos ( -- x y )
          at? textwh @ * swap 1+ texth @ * ;

\ IO-Window                                            24oct99py

        : at-sel ( r c -- )
          0max cols @ 1- min
          swap 0max rows @ 1- min  ?sel-scroll
          cols @ * + pos @ - cursor# @ pos @ selw @
          { s1 c# p s |
          s s1 xor 0<
          IF  1 cursor# ! drawcur      p       s1      0
          ELSE  s1 0max s 0max <  IF p s1 +  s s1 -  1  ELSE
                s1 0max s 0max >  IF p s +   s1 s -  0  ELSE
                s1 0min s 0min <  IF p s1 +  s s1 -  0  ELSE
                s1 0min s 0min >  IF p s +   s1 s -  1  ELSE
                p 0 1  THEN THEN THEN THEN
          THEN  cursor# ! selw ! pos ! drawcur
          c# cursor# ! p pos ! s1 selw ! } ;

\ IO-Window                                            30dec99py
        : clrline   flush  curoff pos @ dup cols @ mod - pos !
          pos @ 'line drop cols @
          2dup -trailing >r drop 2dup bl fill
          r> showtext curon ;
        : curon  ( -- )  -1 cursor# +!  cursor# @ 0> ?EXIT
          1 selw ! drawcur show-you  cursor# off ;
        : curoff ( -- )  cursor# @  1 cursor# +!  0> ?EXIT
          drawcur 0 selw !  1 cursor# ! ;
        : c  ( n -- )  flush  curoff  pos @ + 0max  pos !
          BEGIN pos @ cols @ rows @ * >= WHILE  scrollup  REPEAT
          curon ;
        : cr  ( -- ) flush cols @ pos @ over mod - c
          resize! c@ ?EXIT show-you ;
        : curup    cols @ negate c ;
        : curdown  cols @        c ;

\ IO-Window                                            09mar99py
        : selecting ( -- )  flush  textwh 2@ swap
          DOPRESS  x @ y @ p- 2swap swap >r / swap r> / at-sel ;
        : (dpy  [defined] x11 [IF]    dpy get-win  dpy xrc dpy @
          [ELSE] 0 0 [THEN] ;
        : mark-selection ( x y -- )  defocus  at? >r >r
          swap at pos @ >r selecting
          -select selw @ pos @ + r>
          2dup max -rot min  0 -rot
          ?DO  drop cols @ I over mod -
               I 'line ( drop over -trailing ) I' I - min  tuck
               +select  over I' I - min  <> swap  +LOOP
          IF  s" " +select  THEN  (dpy !select
          curoff  r> r> at focus  curon ;
        : paste-selection ( addr u -- )
          bounds ?DO  I xc@+ 0 keyed pause  I - +LOOP ;

\ IO-Window                                            21aug99py

        : readline  >r >r at? drop cols @ * 'line
          r@ swap 4 pick min dup 3 pin move
          r> over r> min ;
        : >atxy  ( msap xy -- msap )  at? >r >r $100 /mod swap
          2dup at r> rot <>
          IF  >r readline r> rdrop over >r  THEN  r>
          - + dup 0min dup negate c - 2 pick over -
          0min dup c + ;

\ IO-Window                                            07jun03py
        : keyed ( key state -- )
          over shift-keys?  IF  2drop  EXIT  THEN
          BEGIN  keys @ @ $1F >=  WHILE  pause  REPEAT $10 << or
          keys @ dup @ 1+ $1F min dup keys @ ! cells + ! ;
        boxchar :: handle-key?
        : key?  ( -- flag )
          keys @ @ 0= IF  pause  THEN  keys @ @ 0> ;
        : getkey ( -- key )  keys @ @
          IF    keys @ cell+ @  keys @ 8+ dup cell- $78 move
                -1 keys @ +! $10000 /mod kbshift !
          ELSE  0  THEN ;
        : key   ( -- key )  flush  1 cursor# ! curon
          BEGIN  key?  0= WHILE
                 dpy xrc fid &50 idle  REPEAT
          getkey curoff ;

\ IO-Window                                            06jan05py

        : decode ( m s addr pos char -- m s addr pos flag )
          kbshift @ $100 and  IF  >atxy  0 EXIT  THEN
[defined] (Ftast [IF]  dup $FFBE $FFCA within
          IF  $FFBE - cells (Ftast + -rot >r >r -rot >r >r
              perform r> r> r> r> prompt cr save-cursor
              over 3 pick type row over at 0 EXIT THEN
 [THEN]   $FF51 case?  IF  ctrl B  THEN
          $FF52 case?  IF  ctrl P  THEN
          $FF53 case?  IF  ctrl F  THEN
          $FF54 case?  IF  ctrl N  THEN
          dup $007F = IF  drop ctrl D  THEN
          dup $FF00 and $FF00 =  IF  drop 0 EXIT  THEN
[defined] utf-8 [IF]  xdecode [ELSE] PCdecode [THEN] ;

\ IO-Window                                            01jan05py

        : init ( w h -- )  $80 keys Handle!  keys @ off
        ^ CK[ 2swap y @ -
              texth @ / swap x @ - textwh @ / swap 2swap
              1 and  IF  drop mark-selection  EXIT  THEN
              1 and 0=  IF  2drop (dpy @select paste-selection
                            EXIT  THEN
              8 << or kbshift @ $100 or keyed ]CK >callback
          assign  defocuscol @ @ color ! ;
        : close  #cr 0 keyed S" bye"  bounds ?DO  i c@ 0 keyed  LOOP ;
        : dispose start HandleOff  keys HandleOff
          typebuf HandleOff  ^ dpy cleanup  super dispose ;
class;

\ Window IO words                                      10apr04py
terminal uptr term      Forward openw
| : term?  term self 0= IF  openw  THEN ;
: WINtype  ( addr l -- )  term? term type pause ;
: WINemit  ( char -- )    term? term emit ;
: WINflush ( -- )         term? term flush ;
: WINcr    ( -- )         term? term cr pause ;
: WINpage  ( -- )         term? term page ;
: WINat    ( rol col -- ) term? term at  ;
: WINat?   ( -- row col ) term? term at? ;
: WINform  ( -- rs cs )   term? term rows @ term cols @ ;
: WINcuron    ( -- )      term? term curon ;
: WINcuroff   ( -- )      term? term curoff ;
: WINcurleft  ( -- )      term? -1 term c ;
: WINcurrite  ( -- )      term?  1 term c ;
: WINclrline  ( -- )      term? term clrline ;

\ Window IO words                                      05jan05py

Output: WINdisplay
        WINemit true WINcr WINtype PCdel WINpage
        WINat WINat? WINform  noop noop WINflush
        WINcuron WINcuroff WINcurleft WINcurrite WINclrline [

: WINkey? ( -- flag )  term? term key? ;
: WINkey  ( -- key )   term? term key ;
: WINdecode            term? term decode ;
[defined] xaccept [IF]
        Input:  WINkeyboard
        WINkey WINkey? WINdecode xaccept false [
[ELSE]  Input:  WINkeyboard
        WINkey WINkey? WINdecode PCaccept false [  [THEN]
: WINi/o  WINdisplay  WINkeyboard ;

\ openw                                                10apr04py

2Variable map-size              PCform swap map-size 2!
2Variable map-pos
&1000 Value MaxScroll
hbox uptr term-menu             rule uptr term-last
Defer terminal-menu             ' noop IS terminal-menu

forward term-w

minos

\ openw                                                21jun05py

: openw ( -- )  screen self menu-window new
  menu-window with
      term-w set-icon
      0 1 *fill 0 1 *fil rule new dup F bind term-last
   1 hbox new vfixbox dup F bind term-menu 1 vbox new
      map-size 2@ 1 1 viewport new
          D[ terminal new dup F bind term ]D
      s" bigFORTH Dialog" assign
      terminal-menu
      map-size 2@ geometry
      map-pos 2@ d0= 0= IF  map-pos 2@ repos  THEN
      show endwith
  MaxScroll term scrollback
  event-task task's term dup @
  0= IF  term self swap !  ELSE  drop  THEN
  ['] WINi/o IS standardi/o  WINi/o ;

\ terminal menu operations                             10apr04py

: add-menu ( menu -- )  term-last self term-menu add ;
: add-help ( menu -- )  'nil           term-menu add ;
: hide-menu ( -- )  term-menu parent self
  vbox with -flip endwith ;
: show-menu ( -- )  term-menu parent self
  vbox with +flip endwith ;

: send-string ( addr u -- )
  bounds ?DO  i c@ 0 displays keyed  LOOP ;

\ terminal menu operations                             10apr04py

actor class key-actor
public: cell var string
how:    : init ( o addr u -- )  string $!  super init ;
        : fetch ( -- n ) 0 ;
        : store ( n -- ) string $@
          ['] send-string called send drop ;
class;

: key"  state @
  IF    postpone ^  postpone S" key-actor postpone new
  ELSE  ^ '" parse key-actor new  THEN ;        immediate

\ : term-dpy  term dpy dpy self ;


\ helper
forward ficons
forward dot-dir
forward dotdot-dir
forward diro-icon

\ file widget                                          10apr04py
DOS also
lbutton class file-widget
public: cell var size           cell var time
        cell var attr           cell var wsize
        cell var wtime          cell var wdate
how:    \ 6 colors defocuscol !
        : dispose 0 bind callback  super dispose ;
        : assign ( size time attr addr len -- )  base push
          super assign attr ! time ! size ! ;
        : !resized super !resized  decimal
          size @ 0 <# #S #> 0 textsize drop wsize !
          S" 00may99"       0 textsize drop wdate !
          S" 00:00:00"      0 textsize drop wtime ! ;
[defined] x11 [IF]   : dir@ attr @ $C >> ;             [THEN]
[defined] win32 [IF] : dir@ attr @ $10 and 0<> 4 and ; [THEN]

\ file widget                                          10apr04py
        : draw ( -- )  base push decimal  push? 1 and >r
          xywh color @ dpy box
          r@ IF  shadow swap xS xywh drawshadow  THEN
          text $@
          xywh nip texth @ - 2/ +  xS 1+ 0 p+
          r@ r@ p+  x @ xS + r@ + y @ xS + r@ +
          dir@ r> 4 << or ficons icon-pixmap with draw-at
          w @ endwith xS + xM color @ 8 >>
          { iw m cc |  dpy mask
          2swap 2over iw 0 p+ cc .text
          w @ wdate @ - 6 - 0 p+
          time @ >date 2over  cc .text
          m wtime @ + 0 p-   time @ >time 2over cc .text
          m wsize @ + 0 p- size @ 0 <# #S #>
          2swap cc .text } ;

\ file widget                                          10apr04py
        : hglue ( -- glue )  super hglue xM 3 *
          wdate @ wtime @ wsize @ + + +
          dir@ ficons >o icon-pixmap w @ o> + 8 + 0 p+ ;
        : vglue ( -- glue )  super vglue swap
          dir@ ficons >o icon-pixmap h @ o> xS 2* + 1+
          max swap ;
        : clicked  ( click -- )
          dup 0= IF  2drop 2drop  EXIT  THEN
          dup 2/ 1 > >r >released ( cc )
          0= IF  rdrop  EXIT THEN
          0 text $@ callback store
          r> IF  #cr 0 dpy dpy keyed  THEN ;
        : keyed ( key sh -- )  drop bl =
          IF  xywh 2drop  1 2 clicked  THEN ;
class;

\ file listbox                                         10apr04py
[defined] x11 [IF]     : dir? @attr $C >> 4 = ;        [THEN]
[defined] win32 [IF]   : dir? @attr $10 and 0<> ;      [THEN]
component class file-listbox
public: actor ptr file          actor ptr path
        cell var file<=         early name<=
        early date<=            early length<=
how:    : read-files ( addr attr -- w1 .. wn n )
          fsfirst 0 >r
          BEGIN  pause  0=  WHILE  dir? 0=
                 IF  \ cr ." file " dtaname >file type
                     file self
                     @length @time @attr
                     dtaname >len file-widget new
                     r> 1+ >r  THEN  fsnext
          REPEAT  r> ;

\ file listbox                                         10apr04py

        : read-dir  ( addr attr -- w1 .. wn n )
          over >len '/ -scan + dup push '* swap w!
          fsfirst  0 >r
          BEGIN  pause  0=  WHILE  dir?
                 dtaname >len s" ." compare 0<>
                 dtaname >len s" .." compare 0<> and and
                 IF  \ cr ." dir " dtaname >file type
                     path self
                     @length @time @attr
                     dtaname >len file-widget new
                     r> 1+ >r  THEN  fsnext  REPEAT  r> ;

        : close   dpy close ;

\ file listbox sort methods                            10apr04py

        : name<= ( w1 w2 -- flag )   >r
          file-widget with text $@ endwith  r>
          file-widget with text $@ endwith  compare 0>= ;

        : date<= ( w1 w2 -- flag )   2dup
          file-widget with time @ endwith  swap
          file-widget with time @ endwith
          2dup = IF  2drop name<=  ELSE  u>= nip nip  THEN ;

        : length<= ( w1 w2 -- flag ) 2dup
          file-widget with size @ endwith swap
          file-widget with size @ endwith
          2dup = IF  2drop name<=  ELSE  u>= nip nip  THEN ;

\ file listbox                                         10apr04py

        : newdir ( addr len attr -- object )
          scratch 0place
          file<= @ F IS lex
          scratch $1C0 read-dir   >r  sp@ r@ sort
          scratch $0C0 read-files >r  sp@ r@ sort
          r> r> + dup 0=
          IF  s" -Empty Directory-" text-label new swap 1+  THEN
          0 1 *filll 2dup   rule new swap 1+ vresize new
          ['] <= F IS lex ;
        : init ( addr u file-act path-act <= -- )
          file<= !  bind path  bind file
          newdir 1 super init ;
        : dispose path dispose file dispose super dispose ;
class;

\ file selector box                                    22sep07py
window class file-selector
public: icon-but ptr reloader   button ptr oker
        infotextfield ptr path  infotextfield ptr file
        viewport ptr file-list  cell var ok?
        vabox ptr sort-menu     info-menu ptr sort-title
        modal ptr close-it      actor ptr do-ok
        early by-name           early by-date
        early by-length         method reload
how:    AVariable file<=
        : cancel ( -- )  ok? off hide :: close ;
        : ok     ( -- )  ok? on  hide
          0 path get  file get  do-ok store :: close ;
        : close  cancel ;
        : !file  ( addr len -- )  file assign ;

\ file selector box                                    10apr04py

        : !path  ( addr len -- )
          2dup  s" ."  compare
          IF    path get  >r scratch r@ move scratch r@ '/ -scan
                2over s" .." compare 0=
                IF    2swap 2drop 2dup + >r 1- '/ -scan
                      over + r> over - r@ swap dup >r F delete
                      r> r> swap -
                ELSE  2 pick 1+ r> + >r r@ swap /string
                      s" /" 2over insert insert scratch r> THEN
[defined] x11 [IF]     over c@ '/ = IF  path assign  ELSE  2drop  THEN
[ELSE]          path assign   [THEN]
          ELSE  2drop  THEN
          sort-title get reload ;

\ file selector box                                    10apr04py

        : newdir ( addr len -- object ) \  dta fsetdta
          ^ S[ !file ]S ^ S[ !path ]S file<= @
          file-listbox new ;
        : reload ( addr len -- )
          sort-title assign  path get  newdir
          file-list with  assign  resized  endwith ;
        : by-name
          file-listbox ' name<=   file<= ! s" name"   reload ;
        : by-date
          file-listbox ' date<=   file<= ! s" date"   reload ;
        : by-length
          file-listbox ' length<= file<= ! s" length" reload ;

\ file selector box                                    10apr04py
        : >real-path ( addr n1 -- addr' n2 )
[defined] win32 [IF]
          over 1+ c@ ': <>
[ELSE]    over c@ '/ <>   [THEN]
          IF  2dup  pad dup 0 dgetpath drop >len
[defined] win32 [IF] 2dup bounds ?DO  I c@ '\ = IF  '/ I c!  THEN LOOP
[THEN]        dup IF  2dup + '/ swap c! 1+  THEN
              dup >r + swap move r> + nip pad swap
          THEN ;
        : sort-menu:    ( -- o )
          ^ ['] by-name   simple new s" name"   menu-entry new
          ^ ['] by-date   simple new s" date"   menu-entry new
          ^ ['] by-length simple new s" length" menu-entry new
          3 vabox new  widget :: xS borderbox ;

\ file selector window                                 10apr04py
        : panel-line ( info l file l path l -- widget )
          >real-path
          ^ ST[ reloader self close-it default! ]ST
          s" Path:" tableinfotextfield new bind path
          2swap ^ ST[ oker self close-it default! ]ST
          -rot  tableinfotextfield new bind file
          path self  file self   sort-title self  2fill
          ^ S[ s" ."  !path ]S dot-dir    icon-but new
                                   dup bind reloader
          ^ S[ s" .." !path ]S dotdot-dir icon-but new
          2 hatbox new 2 hskips 2skip
                ^ ['] ok     simple new s" OK"   button new
          dup >r                   dup bind oker
          2skip ^ ['] cancel simple new s" Cancel" button new
          3 hatbox new

\ file selector window                                 10apr04py

          5 habox new  3 r> modal new  panel  dup bind close-it
          1 habox new  vfixbox  path get
          1 1 viewport new
          D[ newdir ]D  dup bind file-list
          asliderview new  2 vabox new ;

\ file selector window                                 10apr04py

        : assign ( info len file len path len -- )
          sort-menu self
          s" Sort by" info-menu new bind sort-title
          panel-line  s" File Selector"  super assign ;
        : init ( action dpy -- )
          super init  bind do-ok  file-listbox ' name<= file<= !
          sort-menu: bind sort-menu  diro-icon set-icon ;
        : keyed  over #cr =
          IF  close-it keyed  ELSE  super keyed  THEN ;
class;

\ fsel-input                                           10apr04py

minos

: path+file ( path len file len -- file len )
  >r >r tuck scratch 2+ swap move  scratch 2+ swap  r> r> 2swap
  '/ -scan + swap 2dup + 0 swap c! move  scratch 2+ >len ;

: fsel-action ( info len file1 len1 path1 len1 simple -- )
  screen self file-selector new
  file-selector with  assign  0 $10 geometry  show  endwith ;

: fsel-dialog ( info len file1 len1 path1 len1 simple -- )
  screen self file-selector new  get-win  swap
  file-selector with  set-parent  assign  0 $10 geometry
  show endwith ;

\ fsel-input                                           10apr04py

: ?suffix ( path len suffix len -- path len' )
\  2swap tuck scratch 2+ move scratch 2+ swap 2swap
  dup >r 2over dup r> - 0max /string 2over compare
  IF    >r >r tuck scratch 2+ swap move  scratch 2+ swap  r> r>
        2swap + swap 2dup + 0 swap c! move  scratch 2+ >len
  ELSE  2drop  THEN ;

previous minos

[THEN]

