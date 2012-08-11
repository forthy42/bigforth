\ Display                                              05aug99py
gadget class displays
public: ptr dpy                 ptr childs
        xresource ptr xrc       ptr nextwin
        cell var xwin           cell var cur-cursor
        cell var rw             cell var rh
        cell var mx             cell var my
        cell var mb
        cell var !moved         cell var clicks
        cell var lastclick      cell var lasttime
        cell var tx             cell var ty
        cell var clipregion     cell var counter
        cell var clip-is        cell var clip-should
        cell var clip-r         gadget ptr pointed
        font ptr cur-font       cell var events

\ Display                                              11nov06py
[defined]  xrender [IF]
        cell var xpict          method ?xpict   [THEN]
[defined]  x11 [IF]   sizeof XRectangle var xrect
        cell var lastcal        cell var timeoffset    [THEN]
[defined]  win32 [IF]   sizeof rect       var xrect
        cell var style          cell var owner  [THEN]
        cell var xft-draw

\ Display                                              11nov06py
        early drawable
        method line             method text
        method image            method box
        method mask             method fill
        method stroke           method drawer
        method mouse            method screenpos
        method trans            method trans'
        method transback        method sync
        method set-cursor       method set-rect
        method set-color        method set-font
        method set-hints        method set-linewidth
        method moreclicks       method transclick

\ Display                                              04aug05py
        method clip-rect        method txy!
        method get-event        method handle-event
        method schedule-event   method size-event
        method click            method click?
        method moved?           method moved!
        method get-win          method get-dpy
        method click^           method flush-queue
        method show-me          method scroll
        method clipx            method clipy
        method geometry         method geometry?
        method >exposed         method child-moved
[defined]  win32 [IF]                   method win>o     [THEN]
        early xS                method mxy!
        method schedule         method invoke
        method cleanup          method do-idle

\ Display                                              06feb00py
how:    : dispose  clicks HandleOff
          xrc dispose  super dispose ;
        : xS ( -- n )  xrc xN @ 2+ 2/ 2/ ;
        : dpy! ( dpy -- )  bind dpy !resized ;
        | Variable schedules-root
        : cleanup ( o -- )  >r events
          BEGIN  dup @ WHILE
                 dup @ cell+ cell+ @ r@ =
                 IF  dup @ dup >r @ over !
                     r> schedules-root DelFix
                 ELSE  @  THEN
          REPEAT  drop rdrop ;
[defined]  x11 [IF]
        : ?calib ( -- )  lastcal @ 0= XTime lastcal @ - #60000 >
          or  IF  xrc calibrate XTime lastcal !  THEN ;  [THEN]

\ Display tasking                                      29aug99py
        : schedule ( xt object time -- )
          schedules-root 4 cells $40 NewFix dup >r cell+ !
          r@ 2 cells + 2!   events
          BEGIN  dup @  WHILE
                 dup @ cell+ @ r@ cell+ @ - 0<  WHILE
                 @  REPEAT  THEN
          dup @ r@ ! r> swap ! ;
        : invoke ( -- ms )  events @
          IF    events @ cell+ @ timer@ - dup 0<
                IF    events @ dup @ events ! dup 2 cells + 2@
		      rot schedules-root DelFix
                      object with send endwith
		ELSE  >us #1000 m/mod nip  THEN
          ELSE  minwait  THEN  -1 max minwait min ;
        : do-idle ( n -- ) dup 0>
          IF  xrc fid swap idle  ELSE  drop sync  THEN ;

\ Display tasking                                      09jul00py

      [defined] VFXFORTH [IF]
	Variable pass^
	Variable events-lock  events-lock off
	: .catch-rest ( n -- )  ." Error " . cr ;
	: handle-events ( -- )  events-lock @ ?EXIT
	    events-lock on
	    [: handle-event  invoke drop ;] catch
	    events-lock off throw ;
        : do-event  pass^ @ op!
          up@ TO event-task'
[defined]  win32 [IF]    up@ 'event-task !              [THEN]
          BEGIN  depth >r ['] handle-event catch
                 ?dup IF  .catch-rest  THEN
[defined]  x11 [IF]    (  err-dpy @ IF  .Xerror  THEN ) ?calib   [THEN]
                 ['] invoke catch
                 ?dup IF  .catch-rest  ELSE  do-idle  THEN
                 depth r> <> IF  ~~  clearstack  THEN
          AGAIN ;
\	  task eventtask
\	  : event-task ^ pass^ ! ['] do-event eventtask initiate ;
      [ELSE]
        : .catch-rest ( n -- )  .  "error @ ?dup
	  IF  count type  THEN  "error off
	  cr ['] .except catch drop
	  cr ['] .back   catch drop ;
	: event-task  $20000 $10000 NewTask activate
	  >tib off $100 newtib
	  Onlyforth dynamic   " event-task" r0 @ cell+ !
          up@ TO event-task'
[defined]  win32 [IF]    up@ 'event-task !              [THEN]
          BEGIN  depth >r ['] handle-event catch
	         ?dup IF  .catch-rest  THEN
[defined]  x11 [IF]      err-dpy @ IF  .Xerror  THEN  ?calib   [THEN]
                 ['] invoke catch
                 ?dup IF  .catch-rest  ELSE  do-idle  THEN
                 depth r> <> IF  ~~  clearstack  THEN
          AGAIN ;
      [THEN]
        : set-hints ;

\ Display                                              07jan07py

        : init ( resource -- )  bind xrc  self bind dpy
          'nilscreen bind childs  'nilscreen bind nextwin
[defined]  x11 [IF]
          xrc dpy @ dup xrc screen @ RootWindow xwin !
          xrc screen @ 2dup DisplayWidth  w ! DisplayHeight h !
[THEN]
[defined]  win32 [IF]
          GetDesktopWindow xwin !
          0 0 0 0 sp@ xwin @ GetWindowRect drop 2drop w ! h !
[THEN]
          maxclicks 8* cell+ clicks 2dup Handle! @ swap erase
          -1 rw ! -1 rh !  -1 cur-cursor !
[defined] VFXFORTH [ 0= ] [IF] event-task [THEN] ;

\ Display                                              27jun02py

        : append  ( addr -- )  childs self swap  bind childs
          childs bind nextwin  self childs bind parent ;
        : delete  ( addr -- )  link childs  childs delete ;
        : show-me ( x y -- )  2drop ;
        : scroll ( x y -- )  2drop ;
        : clipx ( -- x w ) 0 w @ ;
        : clipy ( -- y h ) 0 h @ ;
        : ALLCHILDS ( .. -- ..' )  childs self
          BEGIN  dup 'nilscreen <>  WHILE
                 r@ swap >o execute nextwin self o>
          REPEAT  drop rdrop ;
        : !resized 0 set-font  xrc !font  ALLCHILDS  !resized ;
        : resized  ALLCHILDS  resized ;
        : draw     ALLCHILDS  draw ;

\ Display                                              05jan07py

        : get-win [defined]  win32 [IF]  xrc dc @
                  [ELSE]  xwin @  [THEN] ;
        : ?clip ( -- )  clip-is @ clip-should @ <>
          IF  0 clip-rect   self >o
              BEGIN   clip-is @  clip-is off  clip-should off
                      dup op!  0= UNTIL  o>
              ( clip-should @ clip-is ! )  THEN ;
        : txy! ( x y -- )  txy 2@ p+ ty ! tx ! ;
        : geometry ( w h -- )
          x @ y @ 2swap resize ;
        : geometry? ( -- w h )  w @ h @ ;
        : transback ;    : trans ;   : trans' ;
        : set-rect ( o -- )  bind pointed ;

\ Display                                              18jul99py

        : clip-box ( x y w h -- x' y' w' h' ) >xyxy
          clipy over + >r max r> min >r
          clipx over + >r max r> min >r
          clipy over + >r max r> min >r
          clipx over + >r max r> min r> r> r> >xywh ;

        : drawer ( x y o xt -- )  ?clip
          ^ rot >o swap execute o> ;

\ Xrender extras                                       11nov06py

[defined]  xrender [IF]
        Create pict_attrib sizeof XRenderPictureAttributes allot
        1 pict_attrib XRenderPictureAttributes dither !
        : ?xpict ( -- )  xpict @ ?EXIT  xrc dpy @ xwin @
          over dup dup DefaultScreen DefaultVisual
          XRenderFindVisualFormat
          $800 pict_attrib XRenderCreatePicture xpict ! ;
        Create dummy_rect 0 w, 0 w, $7FFF w, $7FFF w,
        : ?pclip ( -- )  xrc dpy @ xpict @ clip-r @ dup
          IF    XRenderSetPictureClipRegion
          ELSE  drop 0 0 dummy_rect 1
                XRenderSetPictureClipRectangles
          THEN ;
[THEN]

\ Display                                              11nov06py

[defined]  x11 [IF]
        : drawable ( -- dpy win gc ) xrc dpy @ xwin @ xrc gc @ ;
        : set-font ( n -- )  xrc font@ bind cur-font ;
        : set-color ( color -- )  ?clip $FF and dup $10 u<
          IF  dup cells Pixmaps + @  ELSE  0  THEN ?dup
          IF    nip  tx @ ty @ rot xrc set-tile
          ELSE  xrc color xrc set-color  THEN ;
        : set-linewidth ( n -- ) >r
	  drawable nip r> 0 0 1 XSetLineAttributes drop ;
        : set-cursor ( n -- )
          cur-cursor @ over = IF  drop  EXIT  THEN
          dup cur-cursor !
	  xrc cursor drawable drop rot XDefineCursor drop
      ;

\ Display                                              21aug99py

        : line ( x y x y color -- )  set-color
          2>r 2>r drawable 2r> 2r> XDrawLine ;
        : text ( addr u x y color -- )  set-color
          self cur-font draw ;
        : box ( x y w h color -- )  set-color \ clip-box
          2>r 2>r drawable 2r> 2r> XFillRectangle ;
        : fill ( x y array n color -- )  set-color
	  2swap over $-4000 $4000 within over $-4000 $4000 within and
	  IF  swap 2over drop w!+ w!
	      2>r drawable 2r> Complex CoordModePrevious
	      XFillPolygon
	  ELSE  2drop 2drop  THEN ;
        : stroke ( x y array n color -- )  set-color
	  2swap over $-4000 $4000 within over $-4000 $4000 within and
	  IF  swap 2over drop w!+ w!
	      2>r drawable 2r> CoordModePrevious XDrawLines
	  ELSE  2drop 2drop  THEN ;

\ Display                                              11nov06py

        : clip-mask ( y x w -- )
	  drawable nip rot XSetClipMask
          drawable nip 2swap swap XSetClipOrigin ;
        : mask { xs ys w h x y win1 win2 }  ?clip
          win1 0= IF
	      drawable win2 -rot xs ys w h x y XCopyArea
[defined]  xrender [IF]  ELSE  win1 -1 = IF
              ?xpict ?pclip
              xrc dpy @ 3 win2 0 xpict @ xs ys xs ys x y w h
              XRenderComposite  [THEN]

\ Display                                              21mar04py

          ELSE  clip-is @ 0= IF  y x win1 clip-mask
	      drawable win2 -rot xs ys w h x y XCopyArea
              0 0 0 clip-mask
          ELSE  1 xrc set-function  0 xrc set-color
	      drawable win1 -rot xs ys w h x y 1 XCopyPlane
              6 xrc set-function
	      drawable win2 -rot xs ys w h x y XCopyArea
              3 xrc set-function
          THEN THEN [defined]  xrender [IF] THEN [THEN] ;
        : image { xs ys w h x y win }  ?clip
	  drawable win -rot xs ys w h x y XCopyArea ;
[THEN]

\ Display                                              27jun02py

[defined]  win32 [IF]
        : drawable ( -- dc ) xrc dc @ ;
        : set-font ( n -- )  xrc font@ bind cur-font ;
        : set-color ( color -- )  ?clip
          dup xrc color drawable SelectObject drop
          dup xrc pen   drawable SelectObject drop
              xrc rgb   drawable SetTextColor drop ;
        : set-cursor ( n -- )
          cur-cursor @ over = IF  drop  EXIT  THEN
          dup cur-cursor !
          xrc cursor dup SetCursor drop
          GCL_HCURSOR xwin @ SetClassLong drop ;
        : set-linewidth drop ;

\ Display                                              27jun02py

        : line ( x y x y color -- )      set-color
          2swap swap 0 -rot drawable MoveToEx ?err
          swap drawable LineTo ?err ;
        : text ( addr u x y color -- )   set-color
          self cur-font draw ;
        : box ( x y w h color -- )
          >r 2dup 1 1 d= IF
               2drop r> xrc rgb -rot swap drawable SetPixel drop
               EXIT  THEN  r>  set-color
          2over p+ swap 2swap swap drawable Rectangle ?err ;

\ Display                                              27jun02py

        : make-path ( x y addr n color -- n polygon )
          set-color
          2over swap (poly'' @ 2!
          (poly'' @ 2 cells + -rot  dup >r  cells bounds
          ?DO  I wx@+ wx@ \ swap dup 0< + dup 0> - swap
               rot >r p+ 2dup swap r> !+ !+
               cell +LOOP  drop 2drop
          r> 1+ (poly'' @ ;
        : fill ( x y array n color -- )  make-path
          drawable Polygon  ?err ;
        : stroke ( x y array n color -- )  make-path
          drawable Polyline ?err ;

\ Display                                              10aug02py

\        : ?gc ( win -- gc )  dup GetDC dup 0=
\          IF  drop  ELSE  nip  THEN ;
        : mask { xs ys w h x y win1 win2 } ?clip
          win1 IF  :srcand ys xs win1 \ ?gc
                   h w y x drawable BitBlt ?err
                   :srcor  ELSE  $00CC0020  THEN  ys xs win2 \ ?gc
          h w y x drawable BitBlt ?err ;
        : image { xs ys w h x y win }      ?clip
          $00CC0020 ys xs win \ ?gc
          h w y x  drawable BitBlt ?err ;
[THEN]

\ Display       Clipping rectangle x11                 12may02py
[defined]  x11 [IF]
        : rect>reg ( rect -- r )  XCreateRegion
          dup >r dup XUnionRectWithRegion r> ;
        : clip-r? ( -- )  clipregion @ clip-r @ <>
          clip-r @ and  IF  clip-r @ XDestroyRegion drop  clip-r off  THEN ;

        : clip-rect ( rect -- )  clip-r? clipregion @
          IF    dup IF    rect>reg >r r@ clipregion @ r@
                          XIntersectRegion r>
                    ELSE  drop clipregion @  THEN
          ELSE  dup IF    rect>reg
                    ELSE  drop drawable nip None XSetClipMask
                          clip-r off EXIT  THEN
          THEN  dup clip-r ! drawable nip rot XSetRegion ;
[THEN]

\ Display       Clipping rectangle win32               20oct99py
[defined]  win32 [IF]
        : clip-rect ( rect -- )  clipregion @
          IF    dup IF    w@+ w@+ w@+ w@ 2over p+
                          swap 2swap swap
                          CreateRectRgn >r
                          RGN_AND r@ clipregion @ r@
                          CombineRgn drop
                          r@ xrc dc @ SelectClipRgn drop
                          rdrop \ no need to destroy it?
                    ELSE  drop clipregion @ xrc dc @
                          SelectClipRgn drop  THEN
          ELSE  dup IF    w@+ w@+ w@+ w@ 2over p+
                          swap 2swap swap CreateRectRgn
                    THEN  xrc dc @ SelectClipRgn drop
          THEN ;                                [THEN]

\ Display                                              25jan03py

[defined]  x11 [IF]
        : add-region ( x y w h -- )
\          base push cr hex xwin @ . decimal
\          ." : add region " 4 0 DO I pick . LOOP
          clipregion @ dup 0= IF  drop XCreateRegion  THEN  >r
          swap 2swap swap  xrect  w!+ w!+ w!+ w!
          xrect r@ dup  XUnionRectWithRegion
          clipregion @ clip-r @ = IF  r@ clip-r !  THEN
          r> clipregion ! ;

\ Display                                              12may02py

        : do-exposed ( -- )
          flags #exposed bit@ IF  clipregion @
	      IF  clipregion @ >r
		  drawable nip r@ XSetRegion   draw
		  clip-r? clipregion off  clip-r off
		  r> XDestroyRegion drop  0 clip-rect  THEN  THEN ;
        : do-resize ( -- )
          rw @ -1 <> rh @ -1 <> or
          IF  clipregion @ ?dup IF XDestroyRegion drop  THEN
              clip-r? clipregion off  clip-r off  0 clip-rect
              w @ rw @ <>  h @ rh @ <> or
              IF  xywh 2drop rw @ rh @ resize  THEN  draw
              -1 rw !  -1 rh !  THEN ;
        : size-event ( -- )
	  do-resize  do-exposed
          nextwin goto size-event ;
[THEN]

\ Display                                              28mar99py

[defined]  win32 [IF]
        : size-event ( -- )
          rw @ -1 <> rh @ -1 <> or
          IF  w @ rw @ dup w ! <>  h @ rh @ dup h !  <> or
              IF  xywh resize draw  THEN
              -1 rw !  -1 rh !  THEN ;
[THEN]

\ Display                                              07jan07py
[defined]  x11 [IF]
        : get-event ( mask -- ) dup >r
          IF  BEGIN  xrc dpy @ r@ event XCheckMaskEvent  WHILE
                     event 0 XFilterEvent
                     0= IF  childs handle-event  THEN  REPEAT
          ELSE  BEGIN  xrc dpy @  XPending  WHILE
                       xrc dpy @ event XNextEvent
                       event 0 XFilterEvent
                       0= IF  childs handle-event  THEN
                REPEAT  childs size-event  \ drop
          THEN  rdrop ;
        : sync ( -- )  xrc dpy @ 0 XSync ;
	: mouse ( -- x y b )  0 sp@ >r 0 sp@ >r 0 sp@ >r
	  drawable drop dummy dup dup dup r> r> r>
          XQueryPointer drop swap rot 8 >> ;    [THEN]

\ Display                                              18oct98py

[defined]  win32 [IF]
        : dispatch-event ( -- )
	  event TranslateMessage drop  maxascii $80 =
	  IF    event DispatchMessageW drop
	  ELSE  event DispatchMessage drop  THEN ;
        : get-event ( event -- )  drop
	  BEGIN  PM_REMOVE 0 0  0 event PeekMessageW  WHILE
		  dispatch-event pause  REPEAT
	  size-event ;
        : sync ( -- ) ;
        : mouse ( -- x y b )  QS_MOUSEMOVE  get-event
          mx @ my @ mb @ ;

        : win>o ( win -- win o )
          0 ALLCHILDS  over xwin @ = IF  drop ^  THEN ;
[THEN]

\ Display                                              07jan07py

        : screenpos ( -- x y )  x @ y @ ;
        : schedule-event ( -- )  flush-queue
          clicks @ @  IF   click clicked  THEN
          moved?  IF  child-moved  THEN
          nextwin goto schedule-event ;
        : handle-event ( -- )
          0 get-event
          childs schedule-event
          ( invoke ) ;
        : moved!  !moved on ;
        : moved?  ( -- flag )  !moved @ !moved off ;
        : get-dpy ( -- addr )  ^ ;
        : mxy! ( mx my -- )  my ! mx ! ;

\ Display                                              04aug05py
[defined]  x11 [IF]
        :noname event XMotionEvent time @ event-time !
          event XMotionEvent x @+ @ mxy!
          event XMotionEvent state @ 8 >> mb !  moved! ;
        MotionNotify cells Handlers + !
        | 2Variable comp_stat
        | Variable look_key
        | Create look_chars $100 allot

\ Display                                              04jan07py

        :noname ( -- ) \ cr 'd emit 'o emit
          do-exposed
          event XKeyEvent time @ event-time !
[defined]  has-utf8 [IF]  xrc ic @ ?dup >r [THEN]
          event look_chars $FF look_key comp_stat
[defined]  has-utf8 [IF]  r>
          IF    Xutf8LookupString
          ELSE  XLookupString  THEN
[ELSE]    XLookupString  [THEN]
          ?dup IF  look_chars swap bounds ?DO
                   I xc@+ swap >r event XKeyEvent state @ $3F and keyed
               r> I - +LOOP  EXIT  THEN
          look_key @ event XKeyEvent state @ $3F and $80 or keyed ;
        KeyPress cells Handlers + !

\ Display                                              11sep05py

        :noname \ cr ." mapping notify"
          event XRefreshKeyboardMapping drop ;
        MappingNotify cells Handlers + !
        : click^ ( -- event )  clicks @ @+ swap 8* + ;
        : transclick ( x y -- x' y' ) ;
	: sendclick ( count event -- )  flags #pending +bit  click^ >r >r
          r@ XButtonEvent state @
          r@ XButtonEvent button @ $80 swap << xor
          r@ XButtonEvent x @
          r> XButtonEvent y @ transclick swap
          r> w!+ w!+ w!+ w!  ;
        : !xyclick ( event -- )  click^ >r
          dup XButtonEvent x @ swap
              XButtonEvent y @ transclick swap r> w!+ w! ;

\ Display                                              05aug99py

        : in-time? ( time flag -- flag )
          lasttime @ rot - swap lastclick @ =
          IF  sameclick  ELSE  twoclicks  THEN  < ;
        : samepos? ( event -- flag )  flags #pending bit@
          IF    XButtonEvent x @+ @  click^ w@+ w@ p-
                dup * swap dup * + samepos <
          ELSE  drop false  THEN ;
        : moreclicks ( -- )
          clicks @ @ maxclicks 1- < negate clicks @ +! ;
        : flush-queue ( -- )  XTime xrc timeoffset @ @ +
          lasttime @ - twoclicks > flags #pending bit@ and
          IF  click^ 6+ w@ 1 and
              IF  moreclicks  THEN  flags #pending -bit  THEN  ;
        : +clicks ( -- ) click^ 6+ dup w@ 2+ -2 and swap w! ;

\ Display                                              09mar99py
        : .button cr base push hex event XButtonEvent window
          @+ @+ @ swap rot xwin @ 9 .r 9 .r 9 .r 9 .r
          space event XButtonEvent x @+ swap . @ . ;
        :noname ( -- )
          do-exposed
          event XButtonEvent time @ event-time !
          event XButtonEvent time @ dup true in-time?
          swap lasttime !  IF   event samepos?
               IF  lastclick @
                   IF    1 event XButtonEvent button @ 1- <<
                         click^ 5 + dup c@ rot or swap c!
                   ELSE  click^ 6 + w@ -2 and 1+ event sendclick
                         lastclick on
                   THEN  EXIT  THEN   event !xyclick flags #pending -bit
          THEN  flags #pending bit@  IF  moreclicks  THEN
          1 event sendclick lastclick on ;
          ButtonPress cells Handlers + !

\ Display                                              09mar99py
	:noname ( -- )
          do-exposed
	  event XButtonEvent time @ event-time !
          event XButtonEvent time @ dup 0 in-time?
          swap lasttime !
          IF  event samepos?  IF  lastclick @
              IF    +clicks  lastclick off
                    moreclicks 2 event sendclick
              ELSE  click^ 6+ w@ 1 and
                    IF  1 event XButtonEvent button @ 1- <<
                        click^ 5 + dup c@ rot invert and swap c!
                    THEN  THEN  EXIT  THEN  click^ 6+ w@  1 and
              ELSE true THEN  \ output push display .button
          IF  event !xyclick +clicks moreclicks  THEN
          flags #pending bit@ 0= >r
	  2 event sendclick  lastclick off
	  r> IF  flags #pending -bit  THEN ;
          ButtonRelease cells Handlers + !

\ Display                                              28jun98py

        : click?  ( -- n )  clicks @ @ 0=
          IF  0 get-event  THEN  clicks @ @ ;
        : click   ( -- x y b n )
          BEGIN  pause click?  UNTIL
          -1 clicks @ +!  clicks @ cell+ wx@+ wx@+ c@+ c@+ w@
          rot kbshift !
          clicks @ $C + dup 8 - clicks @ @ 8* move ;

\ Display                                              04aug05py

        :noname
          event XExposeEvent x @+ @+ @+ @  add-region
\         event XExposeEvent count @ 0= IF ."  draw"  draw  THEN
          flags #exposed +bit ;
        dup Expose         cells Handlers + !
            GraphicsExpose cells Handlers + !
        :noname do-exposed pointed self
          IF  mx @ my @ pointed moved  THEN ;
        EnterNotify    cells Handlers + !
        :noname do-exposed pointed self
          IF  pointed leave  0 bind pointed  moved? drop  THEN ;
        LeaveNotify    cells Handlers + !

\ Display                                              23apr06py

        Create xev  here  sizeof XEvent  dup allot erase

        :noname \ cr  ." Selection Notify "
          event XSelectionRequestEvent time @ event-time !
          event XSelectionEvent property @
          event XSelectionEvent requestor @
          xrc dpy @ fetch-property ;
        SelectionNotify cells Handlers + !

        :noname \ cr  ." Selection Clear "
          own-selection off ;
        SelectionClear  cells Handlers + !

\ Display                                              16jan05py

: rest-request { addr n mode format type }
    xrc dpy @
    event XSelectionRequestEvent requestor @
    event XSelectionRequestEvent property @ dup >r
    type format mode addr n
    XChangeProperty drop sync
  r> xev XSelectionEvent property ! ;
: string-request ( -- )
  (@select PropModeReplace 8 XA_STRING rest-request ;
: string8-request ( -- )
  (@select PropModeReplace 8 XA_STRING8 rest-request ;
: compound-request ( -- )  string-request ;

\ Display                                              23apr06py

| Create 'string XA_STRING , XA_STRING8 ,
: target-request ( -- )
  XA_STRING8 XA_STRING 'string 2!
  'string 2 PropModeReplace #32 4 rest-request ;

\ Display                                              16jan05py
        :noname \ cr  ." Selection Request "
          event XSelectionRequestEvent time @ event-time !
          event xev 4 cells move
          event XSelectionRequestEvent requestor
          xev XSelectionEvent requestor 6 cells move
          xev XSelectionEvent property off
          SelectionNotify xev XSelectionEvent type !
          xrc dpy @ event XSelectionRequestEvent target @
          XGetAtomName >len  1 0 DO \ output push display
          2dup s" STRING"  str= IF  string8-request LEAVE  THEN
      2dup s" UTF8_STRING" str= IF  string-request  LEAVE  THEN
          2dup s" TARGETS" str= IF  target-request  LEAVE  THEN
          2dup s" COMPOUND_TEXT" str=
                                IF  compound-request LEAVE THEN
          LOOP  ( 2dup type cr ) 2drop

\ Display                                              07jan05py

          xrc dpy @
	  event XSelectionRequestEvent requestor @
          0 0 xev XSendEvent drop ;
        SelectionRequest cells Handlers + !

\ Display                                              07jan07py

        :noname  flags #exposed +bit ;  NoExpose cells Handlers + !
        :noname ( -- ) \ resize request
           event XConfigureEvent x @ x !
           event XConfigureEvent y @ y !
           event XConfigureEvent width  @ rw !
           event XConfigureEvent height @ rh ! ;
        ConfigureNotify cells Handlers + !
	:noname do-exposed  focus ;
	  FocusIn  cells Handlers + !
	:noname do-exposed  defocus ;
	  FocusOut cells Handlers + !
        : >exposed ( -- )  sync  flags #exposed -bit
          BEGIN  ( ExposureMask ) 0 get-event
                 pause  flags #exposed bit@  UNTIL ;

\ Display                                              02aug98py
        :noname ( -- )
          event sizeof XClientMessageEvent dump ;
        ClientMessage cells Handlers + !

[THEN]

\ Display                                              19jan00py
[defined]  win32 [IF]        Create paint  $40 allot
        :noname ( lparam wparam msg win -- ret )
          xrc dc @ >r  paint xwin @ BeginPaint xrc dc !
          Xform0 xrc dc @ SetWorldTransform drop
          draw  paint xwin @ EndPaint drop  r> xrc dc !
          2drop 2drop 0 flags #exposed +bit ;         WM_PAINT Handler@ !
        :noname  3 pick >lohi y ! x ! DefWindowProc ;
                                             WM_MOVE  Handler@ !
        :noname  2drop 2drop close 0 ;       WM_CLOSE Handler@ !
        :noname  3 pick WINDOWPOS flags @ SWP_NOSIZE and
          IF  DefWindowProc  EXIT  THEN  2drop drop
          WINDOWPOS cx 2@ 0. 0. sp@ 0 style @ rot
          AdjustWindowRect drop p- p- rw ! rh !
          size-event 0 ;          WM_WINDOWPOSCHANGED Handler@ !

\ Display                                              28jul07py
        :noname 2drop drop { rect |
          vglue >r hglue >r 0 0 sp@ >r 0 style @ r>
          AdjustWindowRect drop p- rect 2@ p+
          dup r> + 2 pick r> + >r >r
          rect 2 cells + 2@ rot max >r max r>
          r> min swap r> min swap
          yinc >r xinc >r 0 0 sp@ >r 0 style @ r>
          AdjustWindowRect drop p- rect 2@ p+
          rot over - r@ 2/ + r@ / r> * + -rot swap
              over - r@ 2/ + r@ / r> * + swap
          rect 2 cells + 2! 0 } ;           WM_SIZING Handler@ !
 \        :noname ( lparam wparam msg win -- ret )
 \        DefWindowProc ;          WM_INPUTLANGCHANGE Handler@ !
 \       :noname ( lparam wparam msg win -- ret )
 \        DefWindowProc ;   WM_INPUTLANGCHANGEREQUEST Handler@ !

\ Display                                              19jan00py

        : >kshift ( n -- n' )  VK_SHIFT - 1 swap <<
          dup 1 <> IF  2*  THEN ;
        : shift@ ( -- kbshift )  0
          VK_MENU 1+ VK_SHIFT ?DO
              I GetKeyState $8000 and IF  I >kshift or  THEN
          LOOP ;
public: displays ptr grab-key
private:
        : ?keyed  grab-key self
          IF  grab-key keyed  ELSE  keyed  THEN ;
        : ?grab  grab-key self
          IF  r> grab-key self >o execute o>  THEN ;

\ Display                                              29jul07py

        | Create xkeys  $FF55 , $FF56 , $FF57 , $FF50 ,
                        $FF51 , $FF52 , $FF53 , $FF54 ,
                        0 ,     0 ,     0 ,     0 ,
                        $0000 , $007F ,
        :noname 2drop nip dup $21 $2F within
          IF    $21 - cells xkeys + @ ?dup
                IF  shift@ ?keyed  THEN
          ELSE  drop  THEN  0 ;            WM_KEYDOWN Handler@ !
        :noname  2drop nip shift@       ?keyed 0 ;
                                              WM_CHAR Handler@ !
 \       :noname  2drop nip shift@       ?keyed 0 ;
 \                                        WM_IME_CHAR Handler@ !
        :noname  2drop nip shift@ ( 8 or )  ?keyed 0 ;
                                           WM_SYSCHAR Handler@ !

\ Display                                              12aug00py

        : click^ ( -- event )  clicks @ @+ swap 8* + ;
        : >mshift ( fwkeys -- mstate ) 0
          over $01 and 0<> $100 and or
          over $02 and 0<> $400 and or
          over $10 and 0<> $200 and or
          dup $500 = emulate-3button @ and IF  $700 xor  THEN
          nip shift@ or ;
        : !xyclick ( event -- )  click^ >r
          MSG lparam lohi@ swap r> w!+ w! ;
        : sendclick ( count event -- )
          flags #pending +bit  click^ >r
          dup MSG wparam @ >mshift swap
              MSG lparam lohi@ swap r> w!+ w!+ w!+ w! ;

\ Display                                              19jan00py
        : in-time? ( time flag -- flag )
          lasttime @ rot swap - lastclick @ =
          IF  sameclick  ELSE  twoclicks  THEN  < ;
        : samepos? ( event -- flag )  flags #pending bit@
          IF    MSG lparam lohi@ ( swap ) click^ w@+ w@ p-
                dup * swap dup * + samepos <
          ELSE  drop false  THEN ;
        : moreclicks ( -- )
          clicks @ @ maxclicks 1- < negate clicks @ +! ;
        : flush-queue ( -- )  GetTickCount
          lasttime @ - twoclicks > flags #pending bit@ and
          IF  click^ 6+ w@ 1 and
              IF  moreclicks  THEN  flags #pending -bit
              ( ReleaseCapture drop )  THEN  ;
        : +clicks ( -- ) click^ 6+ dup w@ 2+ -2 and swap w! ;

\ Display                                              19jan00py
        :noname ( lparam wparam msg win -- 0 ) ?grab \ add press
          SetCapture 2drop 2drop
          event MSG time @ dup true in-time?
          swap lasttime !
          IF   event samepos?
               IF  lastclick @
                   IF   event MSG wparam @ >mshift click^ 4 + w!
                   ELSE  click^ 6 + w@ -2 and 1+ event sendclick
                         lastclick on
                   THEN  0 EXIT  THEN event !xyclick flags #pending -bit
          THEN  flags #pending bit@  IF  moreclicks  THEN
          1 event sendclick lastclick on 0 ;
                                   dup WM_LBUTTONDOWN Handler@ !
                                   dup WM_RBUTTONDOWN Handler@ !
                                       WM_MBUTTONDOWN Handler@ !

\ Display                                              19jan00py
        :noname  2drop $13 and 0= IF ReleaseCapture drop THEN
          ?grab  drop event MSG time @ dup 0 in-time?
          swap lasttime !
          IF  event samepos?  IF  lastclick @
              IF    +clicks  lastclick off
                    moreclicks 2 event sendclick
              ELSE  click^ 6+ w@ 1 and
                    IF  event MSG wparam @ >mshift click^ 4 + w!
                    THEN  THEN 0 EXIT  THEN  click^ 6+ w@  1 and
              ELSE true THEN  \ output push display .button
          IF  event !xyclick +clicks moreclicks  THEN
          flags #pending bit@ 0= >r
	  2 event sendclick  lastclick off 0
	  r> IF  flags #pending -bit  THEN ;
                                   dup WM_LBUTTONUP   Handler@ !
        dup WM_RBUTTONUP   Handler@ !  WM_MBUTTONUP   Handler@ !

\ mouse wheel                                          01jun07py
        : >wshift ( fwkeys -- count mstate ) dup >r >mshift
          r@ 0< $1000 and or  r@ 0> $800 and or
          r> 16 >> #60 / abs swap ;
        : sendwheel ( event -- )  flags #pending +bit
          dup MSG wparam @ >wshift drop 0 ?DO
             dup   MSG wparam @ >wshift nip I 2+ swap
             over2 MSG lparam lohi@ y @ - swap x @ -
             click^ w!+ w!+ w!+ w!
             moreclicks
          2 +LOOP drop ;
        :noname ( lparam wparam msg win -- ) moved!
          flags #pending bit@ IF  moreclicks flags #pending -bit  THEN
          event MSG time @  lasttime !
          2drop 2drop event sendwheel 0 ;
                                        WM_MOUSEWHEEL Handler@ !

\ Display                                              12aug00py
        : click?  ( -- n )  clicks @ @ 0=
          IF  0 get-event  THEN  clicks @ @ ;
        : click   ( -- x y b n )
          BEGIN  pause click?  UNTIL
          -1 clicks @ +!  clicks @ cell+ wx@+ wx@+ c@+ c@+ w@
          rot kbshift ! \ kb-shift off
          clicks @ $C + dup 8 - clicks @ @ 8* move
          ( 2over 2over cr . . . . ) ;

        :noname  2drop drop >r
          vglue + hglue +
          0. sp@ 0 style @ rot AdjustWindowRect drop p-
          r> $8 + 2! 0 ;
                                     WM_GETMINMAXINFO Handler@ !

\ Display                                              29jul07py
        :noname ( lparam wparam msg win -- ) ?grab moved!
          2drop >mshift $FF and mb ! >lohi mxy! 0 ;
                                         WM_MOUSEMOVE Handler@ !
        :noname  pointed self
          IF  pointed leave 0 bind pointed  THEN
          DefWindowProc ;              WM_NCMOUSEMOVE Handler@ !

        :noname focus   2drop 2drop 0 ;  WM_SETFOCUS  Handler@ !
        :noname defocus 2drop 2drop 0 ;  WM_KILLFOCUS Handler@ !
        :noname ( lparam wparam msg win -- )
          2drop 2drop get-sys-colors xrc free-colors
          xrc colors 0 ;            WM_SYSCOLORCHANGE Handler@ !
        : >exposed ;
[THEN]
class;

\ Display                                              21oct99py

displays ptr screen

[defined]  x11 [IF]
    :noname  ( -- )  0 screen get-event ; IS screen-event
    :noname  ( -- )   screen sync ; IS screen-sync
    :noname  ( ic -- ) screen xrc ic ! ; IS screen-ic!
[THEN]

[defined]  win32 [IF]
    :noname ( win -- o )  screen win>o nip ; IS win>o
[THEN]

\ font implementation                                  21aug99py

font implements
        : display  >r color @
          r@ displays with  set-color  endwith
          addr @ u @ 2swap r> draw ;
class;

