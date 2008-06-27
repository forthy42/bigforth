\ window                                               15aug99py

displays class window
public: gadget ptr child        cell var title
        method make-window      method decoration
        gadget ptr innerwin     & innerwin viewport asptr viewp
        cell var app
        method title!           method title+!
        method stop             method set-icon
        method set-parent
how:    : xinc  child xinc ;
        : yinc  child yinc ;
        : schedule ( xt o time -- )  dpy schedule ;
        : invoke ( -- flag )  dpy invoke ;
        : cleanup ( o -- )  dpy cleanup ;

\ window                                               10may99py

        Variable border-size

[IFDEF] x11
        Variable wm_delete_window
        : set-protocol ( -- )
          xrc dpy @ 0" WM_DELETE_WINDOW" 0 XInternAtom
          wm_delete_window !
	  xrc dpy @ xwin @
	  xrc dpy @ 0" WM_PROTOCOLS" 0 XInternAtom
	  4 &32 1 wm_delete_window 1
	  XChangeProperty drop ;
        :noname  event XClientMessageEvent data @
          wm_delete_window @ =  IF  close  THEN ;
        ClientMessage cells Handlers + !

\ window transient subclassing                         13nov99py

        : set-parent ( win -- )
          xrc dpy @ xwin @ rot XSetTransientForHint ;

\ window                                               16aug98py
        Create WMhints sizeof XWMHints allot
        Create hints   sizeof XSizeHints allot
        : set-hint ( -- )  1 WMhints XWMHints input !
          NormalState WMhints XWMhints initial_state !
          [ InputHint StateHint or ] Literal
          WMhints XWMHints flags !
          xrc dpy @ xwin @ WMhints XSetWMHints ;
        : set-icon ( o -- )
          icon-pixmap with 0 0 draw-at endwith
          >r >r 2drop 2drop 2drop r> r>
          WMhints XWMHints icon_pixmap !
          dup WMhints XWMHints icon_mask !
	  IconPixmapHint swap -1 <> IF  IconMaskHint or  THEN
          WMhints XWMHints flags !
          xrc dpy @ xwin @ WMhints XSetWMHints ;

\ window                                               19dec04py

        : set-xswa ( -- )  0 xrc color 3 xrc color
                    xswa XSetWindowAttributes background_pixel !
                    xswa XSetWindowAttributes border_pixel !
          backing-mode xswa XSetWindowAttributes backing_store !
        NorthWestGravity xswa XSetWindowAttributes bit_gravity !
        NorthWestGravity xswa XSetWindowAttributes win_gravity !
          None     xswa XSetWindowAttributes background_pixmap !
          None      xswa XSetWindowAttributes border_pixmap !
          xrc cmap @   xswa XSetWindowAttributes colormap !
          event-mask   xswa XSetWindowAttributes event_mask ! ;

\ window                                               28oct06py
        : set-hints  flags #shown bit@ 0= ?EXIT  x @ y @ d0= 0= 5 and
          $178  or hints XSizeHints flags !
          yinc  xinc rot swap
                hints XSizeHints width_inc 2!
                hints XSizeHints base_width 2!
          hglue 2dup + w @ min 2 pick max
                hints XSizeHints width !
          over  hints XSizeHints min_width !
          +     hints XSizeHints max_width !
          vglue 2dup + h @ min 2 pick max
                hints XSizeHints height !
          over  hints XSizeHints min_height !
          +     hints XSizeHints max_height !
          y @ x @ hints XSizeHints x 2!
          xrc dpy @ xwin @ hints XSetWMNormalHints ;

\ window                                               23jan07py

        : make-window ( n -- )  >r  set-xswa
	  xrc dpy @ dpy xwin @
	  0 0 w @ 1 max h @ 1 max
	  border-size @ border-size off
	  xrc get-visual 0 rot
	  xswavals r> or xswa
          XCreateWindow xwin !   set-protocol set-hint
          xwin @ xrc get-ic ;
[THEN]

\ window                                               28jul07py
[IFDEF] win32
        : make-window ( n -- )   >r  x @ y @ d0=
          IF  $80000000 dup x ! y !  THEN
          0 xrc inst @ 0  r@ $80000000 and
          0= IF    0        y @ x @ h @ w @
                   WS_OVERLAPPEDWINDOW
             ELSE  owner @  y @ x @ h @ w @
                   WS_POPUP border-size @ border-size off
                   IF  WS_BORDER or  THEN
             THEN  dup style !
          popupW minosW
          r> $7FFFFFFF and  CreateWindowExW xwin !
          0 0 0 0 sp@ xwin @ GetWindowRect drop x ! y ! 2drop
          app-win @ 0= IF  xwin @ app-win !  THEN ;
        : set-icon  ( o -- ) drop ;

\ window                                               13nov99py

        : set-parent ( win -- ) dup  owner !
          xwin @ SetParent drop ;
[THEN]
        : init ( dpy -- )   bind dpy  self dpy append
          dpy xrc clone bind xrc
          0 make-window  xwin @ xrc get-gc  0 set-font
          maxclicks 8* cell+ clicks 2dup Handle! @ swap erase
          title off ;
        : ?app  app @   IF  app @ wake pause  app off  THEN ;

\ window                                               22sep07py

        : dispose ( -- )
          child self  drop child dispose  self cleanup
          title $off
          xwin @  IF
[IFDEF] x11           xrc ic @ dup IF  XDestroyIC  THEN  drop
                      xrc dpy @ xwin @ XDestroyWindow drop
[THEN]
[IFDEF] win32         xwin @ DestroyWindow drop
                      dpy handle-event
[THEN]    THEN
          self dpy delete ?app super dispose ;

\ window                                               09aug04py

[IFDEF] x11
        : show   ( -- )  child show
          h @ w @ d0= IF  xywh resize THEN
          flags #shown bit@  super show  set-hints  \ dpy sync
          0= IF  xrc dpy @ xwin @  xywh 2over d0=
              IF    2swap 2drop XResizeWindow
              ELSE  XMoveResizeWindow  THEN  dpy sync  THEN
          xrc dpy @ xwin @ XMapRaised ;
[THEN]

\ window                                               13nov99py

[IFDEF] win32
        : show   ( -- )  child show
          h @ w @ d0= IF  xywh resize THEN
          super show   SWP_NOZORDER SWP_SHOWWINDOW or
          owner @ IF  SWP_NOACTIVATE or  THEN
          h @ w @ 0 0 sp@ >r 0 style @ r>
          AdjustWindowRect drop p-
          y @ x @
          owner @ IF  HWND_TOPMOST  ELSE  HWND_TOP  THEN
          xwin @ SetWindowPos drop ;
[THEN]

\ window                                               01nov06py

        : hide ( -- )  super hide  child hide \ ?app
[IFDEF] x11
          xrc dpy @ xwin @ XUnmapWindow  [THEN]
[IFDEF] win32
          SW_HIDE xwin @ ShowWindow drop  [THEN] ;
        : stop  up@ app !  F stop ;
        : delete ( addr addr' -- )  over self =
          IF    nextwin self swap ! drop
          ELSE  drop link nextwin  nextwin goto delete  THEN ;
        : append ( o before -- )  nextwin self over =
          IF    swap bind nextwin  nextwin bind nextwin
                parent self nextwin bind parent
          ELSE  nextwin goto append  THEN ;

\ window                                               22sep07py
        : decoration ( o -- o' )
          & viewport @ innerwin class?
          IF  sliderview new  THEN ;
        : focus  [IFDEF] x11
          xrc ic @ dup IF  dup >r XNFocusWindow xwin @
	                   XNClientWindow xwin @ 0
                           XSetICValues_2 drop
                           r> XSetICFocus
          THEN  drop  [THEN]
          child focus   ;
        : defocus
          child defocus ;

\ window                                               25jan03py

[IFDEF] x11
        : get-event ( mask -- )  dpy get-event  flush-queue ;
        : handle-event ( -- )
          event XAnyEvent window @ xwin @ =
          event XAnyEvent type @
          dup FocusIn = swap FocusOut = or
          IF    event XEnterWindowEvent subwindow @ xwin @ = or
          THEN
          IF \ cr ." sending event " event @ . ." to win "
             \ base @ event XAnyEvent window @ hex . base !
             event @ cells Handlers + perform
             ( ."  done " ) EXIT  THEN
          nextwin goto handle-event ;
[THEN]

\ window                                               29jul07py
[IFDEF] win32
        : .event base push hex cell+ @+ swap 4 .r
          @+ swap 5 .r @+ swap 9 .r @+ swap 9 .r @+ @ swap
          5 .r 5 .r cr ;
        : get-event ( mask -- )  drop
          BEGIN  PM_REMOVE 0 0 xwin @ event PeekMessageW
                 WHILE  handle-event  REPEAT
          size-event ;
        : handle-event ( -- )  \ event .event
          event TranslateMessage drop  maxascii $80 =
          IF    event DispatchMessageW drop
          ELSE  event DispatchMessage drop  THEN
          pause ;
[THEN]

\ window                                               25jan03py

        : !resized  xrc !font
          0 set-font  child !resized resized ;
        : geometry ( w h -- ) { gw gh |
          1 counter ! rw off rh off
          x @ y @ xinc gw * + yinc gh * + resize
          0 counter ! rw on  rh on
          x @ y @ xinc gw * + yinc gh * + resize }
[IFDEF] win32  output push display ." "  [THEN] ;
        : geometry? ( -- w h )
          w @ xinc >r - r> /
          h @ yinc >r - r> / ;
        : draw ( -- ) \ base push hex xwin @ . ." : w-draw "
          clip-should off  clip-is off
          0 clip-rect  child draw ;

\ window                                               05oct07py
[IFDEF] x11
        Create 'textprop 0 , 0 , 8 , 1 ,
        : !title ( -- )  0 title $@ + c!
          0" MINOS" title $@ drop sp@
          xrc dpy @ xwin @ rot XSetClassHint 2drop
          XA_STRING title @ cell+ 'textprop 2!
          title @ @ 'textprop 3 cells + !
	  xrc dpy @ xwin @ 'textprop
	  over2 0" _NET_WM_NAME" 0 XInternAtom  XSetTextProperty
	  xrc dpy @ xwin @ 'textprop
          over2 0" _NET_WM_ICON_NAME" 0 XInternAtom  XSetTextProperty
          xrc dpy @ xwin @ title @ cell+ XStoreName
          xrc dpy @ xwin @ title @ cell+ XSetIconName ;
        : title!  ( addr u -- ) title $!  !title ;
        : title+! ( addr u -- ) title $+! !title ;  [THEN]

\ window                                               29jul07py

[IFDEF] win32
        : !title ( -- )  title $@ >utf16 drop
          xwin @ SetWindowTextW drop ;
        : title!  ( addr u -- ) title $!  !title ;
        : title+! ( addr u -- ) title $+! !title ;
        : screenpos ( -- x y )
          0 0 0 0 sp@ xwin @ GetWindowRect drop 2swap 2drop
          h @ w @ 0 0
          sp@ >r 0 style @ r> AdjustWindowRect drop 2swap 2drop
          p- swap ;
        : mxy! ( mx my -- ) 2dup super mxy!
          screenpos p+ screen mxy! ;
[THEN]

\ window                                               17dec00py
        : assign ( widget addr n -- )
          child self IF  child dispose  THEN  title!
          dup bind innerwin  decoration  bind child
          self child dpy!  self child bind parent ;
        : adjust-inc ( n off inc -- n' )
          >r tuck - r@ 2/ + r@ / r> * + ;
        : min-max ( n glue -- n' ) over + >r umax r> umin ;
        : child-size? ( -- x y )  child xywh 2swap 2drop  2dup
          yinc adjust-inc vglue min-max h !
          xinc adjust-inc hglue min-max w ! ;
        : child-resize ( -- )
          BEGIN  0 0 w @ h @ 2dup 2>r child resize
                 2r> child-size? 2over w @ h @ d= >r
                 d= r> and  UNTIL ;

\ window                                               19oct99py
[IFDEF] x11
        : re-size ( -- )
          rw @ rh @ w @ h @ d= 0= IF
              xrc dpy @ xwin @ w @ h @ XResizeWindow
          THEN ;
[THEN]
[IFDEF] win32
        : re-size ( -- )
          rw @ rh @ w @ h @ d= 0= IF
              1 h @ w @ 0 0
              sp@ >r 0 style @ r> AdjustWindowRect drop
              2dup >r >r p- screenpos swap r> r> p+
              xwin @ MoveWindow drop
          THEN ;
[THEN]

\ window                                               07jan07py

        : (resized ( -- )
          child-resize  child-moved
\          rw @ rh @  child-size?  d= 0=  IF  draw  THEN
          set-hints dpy sync  re-size ;
        : close  ( -- )  flags #closing bit@ dup >r flags #closing +bit
          IF    hide ['] dispose self &10 after schedule
	  ELSE  innerwin close  THEN
	  r> flags #closing bit! ;

\ window                                               15jul01py

        : repos ( x y -- )   2dup y ! x !
[IFDEF] x11   set-hints
          xrc dpy @ xwin @ 2swap XMoveWindow sync ; [THEN]
[IFDEF] win32
          >r >r 0 h @ w @ r> r> swap
          xwin @ MoveWindow drop ;  [THEN]
        : resized  ( -- )  (resized counter @ ?EXIT  draw ;
        : child-moved ( -- )  pointed self
          IF  mx @ my @ pointed xywh >r >r
              p- r> r> rot swap u< -rot u< and
              IF  & backing @ pointed class?
                  IF mx @ my @ pointed moved THEN  EXIT  THEN
              pointed leave  0 bind pointed  THEN
          child self  IF  mx @ my @  child moved  THEN ;

\ window                                               28mar99py
        : resize ( x y w h -- )
          h ! w ! 2drop (resized ;
        : mouse ( -- x y b ) mx @ my @ mb @ ;
        : clicked  ( x y b n -- )  child clicked ;
        : hglue ( -- glue ) child hglue ;
        : vglue ( -- glue ) child vglue ;
        : keyed ( key -- )  dup 8 and
          IF  over $FF51 =  2 pick $FF53 = or
              & vviewport @ innerwin class? not and
              IF  viewp hspos keyed  EXIT  THEN
              over $FF52 =  2 pick $FF54 = or
              & hviewport @ innerwin class? not and
              IF  viewp vspos keyed  EXIT  THEN
          THEN  innerwin keyed ;
class;

\ menu-entry                                           05jan07py

actor uptr menu-call

'& Value menu-sep
button class menu-entry
how:    \ init ( act addr len -- )
        2 colors focuscol !     3 colors defocuscol !
        : clicked ( x y b n -- ) dup 0= IF 2drop 2drop EXIT THEN
          >released drop
          dpy hide callback self F bind menu-call ;
        : keyed ( key sh -- )  drop  dup bl = swap #cr = or
          IF  x @ y @  1 2 clicked  THEN ;
        : focus  super focus color   focuscol chcol +push draw ;
        : defocus color defocuscol chcol -push draw ;

\ menu-entry                                           12dec99py

        : hglue  text $@ menu-sep scan nip
          IF    0 text menu-sep :[ fnt size drop 1 *fil
                               2 pick parent with
                                   dup >r 1- combined tab@ p+
                                   r> combined tab!
                               endwith  1+ ]: $iter
                1- parent with combined tab@ endwith
                xM xS + 1+ 0 p+
          ELSE  textwh @ xM + xS + 1+ 1 *fil  THEN ;
class;

\ event handler for sub-window                         30aug05py
window class window-stub
how:    : init ( widget win -- )  xwin !  title off
          dup bind innerwin bind child
          child with dpy self endwith bind dpy
          self dpy with dpy append endwith
          dpy xrc clone bind xrc
          xwin @ xrc get-gc  0 set-font
          maxclicks 8* cell+ clicks 2dup Handle! @ swap erase ;
        : resize-win ( -- )  h @ w @ y @ x @ or or or 0= ?EXIT
[IFDEF] win32  SWP_NOZORDER SWP_SHOWWINDOW or
          h @ w @ y @ x @
          owner @ IF  HWND_TOPMOST  ELSE  0  THEN
          xwin @ SetWindowPos drop  [THEN]
[IFDEF] x11    xrc dpy @ xwin @ xywh XMoveResizeWindow  [THEN] ;

\ event handler for sub-window                         20nov07py
        : show ( -- )  resize-win
[IFDEF] win32  SWP_SHOWWINDOW xwin @ ShowWindow drop [THEN]
[IFDEF] x11    xrc dpy @ xwin @ XMapWindow  [THEN] ;
        : dispose-it ( -- )  self cleanup
          self dpy get-dpy with dpy delete endwith
          title $off
          xrc dispose gadget :: dispose ;
        : dispose ( -- )
[IFDEF] win32  xwin @ IF  xwin @ DestroyWindow drop  THEN
          ['] dispose-it  self &20 after schedule ;  [THEN]
[IFDEF] x11  dispose-it ;  [THEN]
        : resize  h ! w ! y ! x !  resize-win ;

\ event handler for sub-window                         30aug05py

        : moved!  dpy moved! ;
        : moved?  dpy moved? ;
        : click^  dpy click^ ;
        : moreclicks dpy moreclicks ;
        : mxy!    transclick dpy mxy! ;
        : keyed   dpy keyed ;
        : transclick ( x y -- x' y' ) x @ y @ p+ ;
class;

\ window without border                                12dec99py
[IFDEF] win32   Variable owner-win  [THEN]
window class frame
public: cell var map?           method set-dpys
        method grab             method ungrab
        method handle [IFDEF] win32  displays ptr ?grab  [THEN]
how:    : make-window  ( attrib -- )
[IFDEF] x11  mouse_cursor xrc cursor
          xswa XSetWindowAttributes cursor !
          1 xswa XSetWindowAttributes override_redirect !
          1 xswa XSetWindowAttributes save_under !
          CWSaveUnder or CWOverrideRedirect or CWCursor or
[THEN]
[IFDEF] win32  owner-win @ owner ! owner-win off  $80000000 or
          WS_EX_TOPMOST or WS_EX_TOOLWINDOW or  [THEN]
          super make-window ;

\ frame                                                08aug04py

        : handle ( -- flag )
          -1 -1 0 0 child clicked  true
          BEGIN  click? 0=
                 IF  moved?
                     IF   mouse drop child inside?
                          mouse 0 child clicked tuck <>
                          IF dup IF   child focus
                                 ELSE child defocus  THEN THEN
                     THEN  dpy xrc fid &30 idle
                 ELSE  click 2over child inside? dup >r
                       IF    child clicked
                       ELSE  hide 2drop 2drop
                       THEN  drop r>
                 THEN  map? @ 0=  UNTIL ;

\ frame                                                09mar07py

[IFDEF] x11
        Variable grab-win       grab-win on
        : Xgrab ( win -- )  grab-win @ map? ! grab-win !
	  xrc dpy @ grab-win @ 0
          [ ButtonPressMask ButtonReleaseMask PointerMotionMask
            or or ] Literal
	  GrabModeAsync dup  None dup  CurrentTime
	  XGrabPointer drop
	  xrc dpy @ grab-win @ RevertToParent CurrentTime
	  XSetInputFocus ;
        : grab  xwin @ Xgrab ;
        : ungrab ( -- )  map? @ dup grab-win !
          dup -1 <>  IF  Xgrab map? off  EXIT  THEN drop
          xrc dpy @ CurrentTime XUngrabPointer map? off ;
[THEN]

\ frame                                                27jun02py
[IFDEF] win32
        : Wgrab ( win -- ) dup re-time  grab-key self bind ?grab
          SetCapture dup 0= or map? !  ^ F bind grab-key ;
        : grab ( -- )  xwin @ Wgrab ;
        : ungrab ( -- )  map? @
          IF    ?grab self  F bind grab-key  0 bind ?grab
                map? @ -1 <>  IF  map? @ grab  ?grab self
                   F bind grab-key  0 bind ?grab  THEN  map? off
          ELSE  ReleaseCapture drop  app-win @ re-time  THEN ;
 [THEN] : dispose ( -- )
          title $off
[IFDEF] x11  xwin @ IF xrc dpy @ xwin @ XDestroyWindow drop THEN
 [THEN] [IFDEF] win32
          xwin @  IF  xwin @ DestroyWindow drop  THEN
 [THEN]   self dpy delete  displays :: dispose ;

\ window without border                                29aug98py

        : show ( x y -- )  y ! x !  super show ;
        : set-dpys ( widget -- )  recursive
          BEGIN  dup 0<> over 'nil <> and  WHILE  ^ swap >o
                 widget bind dpy   widget widgets self
                 & combined @ class?
                 IF    combined childs self o> set-dpys
                 ELSE  o>  THEN
          REPEAT  drop ;
class;

\ tool tips                                            16jul00py

frame class frame-tip
public: displays ptr owner-dpy
how:    : make-window ( n -- )  1 border-size !
          super make-window ;
        : init ( owner dpy -- )  super init  bind owner-dpy ;
        : keyed  owner-dpy keyed ;
class;

\ tool tips                                            27jun02py

minos

&1000 Value tooltip-delay

actor class tooltip
public: widget ptr tip          actor ptr feed
        frame-tip ptr tip-frame early show-tip
how:    : init ( actor tip -- )  bind tip  bind feed
          feed called self  set-called ;
        : dispose leave  tip dispose super dispose ;
        : leave  ^ screen cleanup  tip-frame self 0= ?EXIT
          tip-frame hide  tip-frame dispose 0 bind tip-frame ;

\ tool tips                                            07nov99py
        : show-tip ( -- )
[IFDEF] win32  caller with widget dpy get-dpy endwith
               displays with xwin @ endwith owner-win ! [THEN]
          caller with widget dpy pointed self ^ =
              IF   0 widget dpy set-rect  THEN  endwith
          caller xywh  & hbox @ caller parent class?
          IF  nip 0 swap  ELSE  drop 0  THEN  p+
          caller self widget with xN endwith dup p+
          caller self widget with dpy screenpos endwith p+
[IFDEF] x11  caller with widget dpy get-win endwith  [THEN]
          tip self caller self widget with dpy self endwith
          screen self frame-tip new dup bind tip-frame
          frame-tip with s" tooltip" assign
              [IFDEF] x11  set-parent  [THEN]  show focus
          endwith ;

\ tool tips                                            21sep07py

        : enter  [IFDEF] x11  leave  [THEN]
          [IFDEF] win32  tip-frame self ?EXIT  [THEN]
          ['] show-tip ^ tooltip-delay after screen schedule ;
        : toggle leave feed toggle ;
        : fetch  leave feed fetch ;
        : store  leave feed store ;
        : set-called  dup super set-called feed set-called ;
class;

: TT[  ;                                         immediate
: ]TT  tooltip postpone new ;                    immediate
: TT-string  text-label new tooltip new ;
: TT"  postpone X" postpone TT-string ;          immediate

\ menu-frame                                           09mar07py

frame class menu-frame
public: early popup
how:    : assign ( widget -- ) child self IF child dispose THEN
          dup bind child   bind innerwin
          self child dpy!  self child bind parent
          resized ;
        : screenpos ( -- x y )  x @ y @ ;
        : hide  ( -- )  super hide  ungrab ;
        : show ( x y -- ) super show  grab ;
        : keyed ( key sh -- )
          over #esc =  IF  2drop drop 0 hide EXIT  THEN
          super keyed ;

\ menu-frame                                           05mar07py

        : submenu-vpos { x y w h w' h' | ( --> x y )
          x y h + dup h' + screen h @ >  IF  h' - h - 0max  THEN
          swap screen w @ w' - min 0max swap } ;
        : submenu-hpos { x y w h w' h' | ( --> x y )
          x w + y screen h @ h' - min 0max
          swap dup w' + screen w @ >  IF  w' - w - 0max  THEN
          swap } ;

\ menu-frame                                           09mar07py
        : popup ( [xwin] child -- flag )  >r
[IFDEF] win32+  dpy get-dpy displays with xwin @ endwith
                owner-win !   [THEN]
          r@ widget with dpy self endwith
          dpy screenpos  xywh  >r >r p+ r> r>
          & hbox @ parent class?
          r> screen self new  with  assign  defocus
             >r  ( !resized ) 0 0 0 0 resize
             child with w @ h @ endwith
             r>  IF  submenu-vpos  ELSE  submenu-hpos  THEN
             >r rot [IFDEF] x11 set-parent [ELSE] drop [THEN] r>
             show  focus   handle  swap child dpy!
             dispose  endwith ;
class;

\ menu title                                           05mar07py
menu-entry class menu-title
        method menu-action
public: widget ptr callw
how:    0 colors focuscol !     1 colors defocuscol !
        : init  ( widget addr len -- )
          noop-act -rot super init bind callw ;
        : dispose callw dispose super dispose ;
        : menu-action  menu-call called self
          0= IF  dpy self menu-call set-called  THEN
          menu-call toggle ;
        : >released ( x y b n -- ) 2drop 2drop
          1 color 2+ c!  draw
          dpy get-win callw self menu-frame popup
          0=   IF    callback self F bind menu-call
               ELSE  dpy focus  THEN    0 color 2+ c!  draw ;

\ menu title                                           21apr01py

        : clicked  ( x y b n -- )
          dup 0= IF  2drop 2drop  EXIT  THEN
          >released  callw hide  menu-action ;
        : !resized  super !resized ( callw !resized ) ;
class;

\ sub-menu                                             27dec99py

menu-title class sub-menu
how:    \ : init ( widget addr u -- )  super init ;
        : menu-action ( -- )
          menu-call self callback self <> IF  dpy hide  THEN ;
class;

' noop alias M:                                 immediate

\ info-menu                                            27dec99py
hbox class info-menu
public: textfield ptr text      tributton ptr tri
        text-label ptr info     gadget ptr callw
how:    : assign ( addr u -- ) text assign ;
        : get ( -- addr u ) text get ;
        : text!  ( addr u -- ) info assign ;
        : menu-action  menu-call self 0= ?EXIT
          menu-call called self
          0= IF  dpy self menu-call set-called  THEN
          menu-call toggle ;
        : keyed ( key sh -- )
          over bl =  IF  tri keyed  ELSE  text keyed  THEN ;
        gadget :: prev-active
        gadget :: next-active
        gadget :: first-active

\ info-menu                                            02dec00py
        : init  ( widget addr len -- )
          text-label new bind info  bind callw
          callw self combined with childs get endwith 0 ST[ ]ST
                    textfield new dup bind text
          0 text edit ds !
            ^ M[ clicked ]M :down tributton new bind tri
            info self 1 habox new hfixbox  text self
            ^ S[ ]S :[ text childs vglue ]: :[ xS 0 ]: arule new
               tri self
            ^ S[ ]S :[ text childs vglue ]: :[ xS 0 ]: arule new
            3 vbox new hfixbox 2 hbox new
            ^ S[ ]S :[ callw hglue ]: :[ 0 0 ]: arule new
          2 vbox new  +fill 3 super init drop ;
        : dpy!  dup callw dpy!  super dpy! ;
        : !resized  super !resized  callw !resized ;

\ info-menu                                            05mar07py

        : >released ( x y b n -- ) 2drop 2drop
          :up tri assign tri draw  0 F bind menu-call
          dpy get-win
          callw self text with menu-frame popup endwith
          0=   IF callback self F bind menu-call THEN
          :down tri assign tri draw ;
        : clicked  ( x y b n -- ) \ first-active
          dup 0= IF  2drop 2drop  EXIT  THEN
          >released  menu-action ;
        : dispose  callw dispose  super dispose ;
        boxchar :: handle-key?
class;

\ window with menu                                     27dec99py

window class menu-window
how:    : decoration ( menu widget -- )
          super decoration 2 vbox new ;
class;

