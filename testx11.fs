\ test X11

\needs X11    include x11.fs
\needs XConst include x.fs

X11 also XConst also

0 Value dpy
0 Value screen
0 Value win

dos legacy on
1 libc getenv getenv
Forth

: open-x ( -- )
  0" DISPLAY" getenv XOpenDisplay  to dpy
  dpy XDefaultScreenOfDisplay  to screen ;

: simple-win ( events cstring -- )
  0 0 1 $100 $200 0 0 dpy XDefaultRootWindow dpy XCreateSimpleWindow  to win
  dpy win rot XStoreName
  dpy win rot XSelectInput
  dpy win XMapWindow
  dpy 0 XSync ;

open-x
0
KeyPressMask or
KeyReleaseMask or
ButtonPressMask or
ButtonReleaseMask or
EnterWindowMask or
LeaveWindowMask or
PointerMotionMask or
\ PointerMotionHintMask or
\ Button1MotionMask or
\ Button2MotionMask or
\ Button3MotionMask or
\ Button4MotionMask or
\ Button5MotionMask or
\ ButtonMotionMask or
KeymapStateMask or
ExposureMask or
VisibilityChangeMask or
StructureNotifyMask or
ResizeRedirectMask or
SubstructureNotifyMask or
SubstructureRedirectMask or
FocusChangeMask or
PropertyChangeMask or
ColormapChangeMask or
OwnerGrabButtonMask or
0" bigFORTH Test" simple-win

Create Handlers
       MappingNotify [FOR]  ' noop  A,  [NEXT]

: Event! ( n -- ) cells Handlers + ! ;


Create event sizeof XEvent allot
Variable events

: handle-event ( flag -- )
  IF  dpy XPending  ELSE  true  THEN
  IF  dpy event XNextEvent 1 events +!
      event @ cells Handlers + perform  THEN ;

Variable comp_stat 0 ,
Variable look_key
Create look_chars 4 allot
		 
:noname ( -- )
  ." key pressed: " event XKeyEvent keycode @ .
                    event XKeyEvent state @ .
		    dpy event XKeyEvent keycode @ 0 XKeycodeToKeysym .
\		    comp_stat look_key 3 look_chars event XLookupString
\		    IF  look_chars c@  ELSE  look_key @  THEN
		    . cr
		    ;  KeyPress Event!
:noname ( -- )
  ." key released: " event XKeyEvent keycode @ . cr ;  KeyRelease Event!
:noname ( -- )
  ." button pressed: " event XButtonEvent button @ . base push hex
                       event XButtonEvent state @ .
                       event XButtonEvent time @ u. cr ; ButtonPress Event!
:noname ( -- )
  ." button released: " event XButtonEvent button @ . base push hex
                        event XButtonEvent state @ .
                        event XButtonEvent time @ u. cr ; ButtonRelease Event!
:noname
  ." motion: " event XMotionEvent is_hint @ . cr ; MotionNotify Event!
:noname
  ." enter: " event XCrossingEvent focus @ .
              event XCrossingEvent detail @ . cr ; EnterNotify Event!
:noname
  ." leave: " event XCrossingEvent focus @ .
              event XCrossingEvent detail @ . cr ; LeaveNotify Event!
:noname
  ." focus in: " event XFocusChangeEvent mode @ .
                 event XFocusChangeEvent detail @ . cr ; FocusIn Event!
:noname
  ." focus out: " event XFocusChangeEvent mode @ .
                  event XFocusChangeEvent detail @ . cr ; FocusOut Event!
\ :noname
\  ." keymap: " event XKeymapEvent key_vector $20 dump cr ; KeymapNotify Event!
:noname
  ." expose: " event XExposeEvent width @ 0 u.r ." x"
               event XExposeEvent height @ .
               event XExposeEvent count @ . cr ; Expose Event!
:noname
  ." graphics expose: " event XGraphicsExposeEvent width @ 0 u.r ." x"
               event XGraphicsExposeEvent height @ .
               event XGraphicsExposeEvent count @ .
               event XGraphicsExposeEvent major_code @ .
               event XGraphicsExposeEvent minor_code @ . cr ; GraphicsExpose Event!
:noname
  ." no expose: "
               event XNoExposeEvent major_code @ .
               event XNoExposeEvent minor_code @ . cr ; NoExpose Event!
:noname
  ." visibility: " event XVisibilityEvent state @ . cr ; VisibilityNotify Event!
:noname
  ." create: " cr ; CreateNotify Event!
:noname
  ." destroy: " cr ; DestroyNotify Event!
:noname
  ." unmap: " cr ; UnmapNotify Event!
:noname
  ." map: " cr ; MapNotify Event!
:noname
  ." map request: " cr ; MapRequest Event!
:noname
  ." reparent: " cr ; ReparentNotify Event!
:noname
  ." configure: " event XConfigureEvent width @ 0 u.r ." x"
                  event XConfigureEvent height @ . cr ; ConfigureNotify Event!
:noname
  ." configure request: "
                  event XConfigureRequestEvent width @ 0 u.r ." x"
                  event XConfigureRequestEvent height @ . cr ; ConfigureRequest Event!
:noname
  ." gravity: " cr ; GravityNotify Event!
:noname
  ." resize: " event XResizeRequestEvent width @ 0 u.r ." x"
               event XResizeRequestEvent height @ . cr ; ResizeRequest Event!
:noname
  ." circulate: " event XCirculateEvent place @ . cr ; CirculateNotify Event!
:noname
  ." circulate request: "
   event XCirculateRequestEvent place @ . cr ; CirculateRequest Event!
:noname
  ." property: " event XPropertyEvent state @ . cr ; PropertyNotify Event!
:noname
  ." selection clear: " cr ; SelectionClear Event!
:noname
  ." selection request: " cr ; SelectionRequest Event!
:noname
  ." selection: " cr ; SelectionNotify Event!
:noname
  ." colormap: " cr ; ColormapNotify Event!
:noname
  ." client message: " event XClientMessageEvent message_type @ . cr ; ClientMessage Event!
:noname
  ." mapping: "
  event XMappingEvent request @ .
  event XMappingEvent first_keycode @ .
  event XMappingEvent count @ . ; MappingNotify Event!

: event-loop  BEGIN true handle-event pause AGAIN ;

\ event-loop

