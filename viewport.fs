\ subwindow based viewport                             25jul98py

also Memory also

viewport class sub-viewport
public: cell var xwin2
how:    : make-win ( wino -- win )  >r
          0 xswa XSetWindowAttributes backing_store !
          NorthWestGravity
              xswa XSetWindowAttributes bit_gravity !
          xrc dpy @ xrc screen @ BlackPixel
              xswa XSetWindowAttributes border_pixel !
          defocuscol @ @ xrc color
              xswa XSetWindowAttributes background_pixel !
          event-mask   xswa XSetWindowAttributes event_mask !
          xswa xswavals CopyFromParent dup 0 0
          1 1 0 0 r> xrc dpy @ XCreateWindow ;
        : init ( sx sy -- )  super init
          maxclicks 8* cell+ clicks 2dup Handle! @ swap erase
          self dpy get-dpy displays with dpy append endwith
          dpy get-win make-win dup xwin2 ! make-win xwin ! ;
        : trans ( x y -- x' y' ) ;
        : trans' ( x y -- x' y' ) ;
        : screenpos ( -- x y )  dpy screenpos
          orgx @ hstep @ * x @ -
          orgy @ vstep @ * y @ - p- ;
        : resize-win ( -- )  h @ w @
          orgy @ vstep @ * negate orgx @ hstep @ * negate
          xwin @ xrc dpy @ XMoveResizeWindow drop ;
        : resize-win2 ( -- )  sh @ sw @ y @ x @
          xwin2 @ xrc dpy @ XMoveResizeWindow drop ;
        : show ( -- )  resize-win resize-win2
          xwin @ xrc dpy @ XMapWindow drop
          xwin2 @ xrc dpy @ XMapWindow drop ;
        : hide ( -- )
          xwin2 @ xrc dpy @ XUnmapWindow drop ;
        : resize ( x y w h -- )  super resize
          draw? @ 0= ?EXIT
          resize-win resize-win2 ;
        : draw  child draw ;
        : line  draw? @  IF  displays :: line
          ELSE  drop 2drop 2drop  THEN ;
        : text  draw? @  IF  displays :: text
          ELSE  drop 2drop 2drop  THEN ;
        : box   draw? @  IF  displays :: box
          ELSE  drop 2drop 2drop  THEN ;
        : image  draw? @  IF  displays :: image
          ELSE  drop 2drop 2drop 2drop  THEN ;
        : mask  draw? @  IF  displays :: mask
          ELSE  2drop 2drop 2drop 2drop  THEN ;
        : fill  draw? @  IF  displays :: fill
          ELSE  drop 2drop 2drop  THEN ;
        : stroke  draw? @  IF  displays :: stroke
          ELSE  drop 2drop 2drop  THEN ;
        : drawer  draw? @  IF  displays :: drawer
          ELSE  2drop 2drop  THEN ;
        displays :: set-cursor
        displays :: click
        displays :: click?
        displays :: moved?
        displays :: moved!
        displays :: mouse
        : xpos! orgx @ case? 0=
          IF  orgx ! resize-win  THEN ;
        : ypos! orgy @ case? 0=
          IF  orgy ! resize-win  THEN ;
        : create-pixmap ;
        : handle-event ( -- )
          event XAnyEvent window @ xwin @ =
          event XAnyEvent type @
          dup FocusIn = swap FocusOut = or
          IF    event XEnterWindowEvent subwindow @ xwin @ = or
          THEN
          IF    event @ cells Handlers + perform  EXIT  THEN
          nextwin goto handle-event ;
        window :: delete
        : clicked  ( x y b n -- )
          child clicked ;
        : child-moved ( -- )  pointed self
          IF  mx @ my @ pointed xywh >r >r
              p- r> r> rot swap u< -rot u< and
              IF  & backing @ pointed class?
                  IF mx @ my @ pointed moved THEN  EXIT  THEN
              pointed leave  0 bind pointed  THEN
          child self  IF  mx @ my @ child moved  THEN ;
        : schedule-event ( -- )  flush-queue
          clicks @ @  IF   click clicked  THEN
          moved?  IF  child-moved  THEN
          nextwin goto schedule-event ;
class;

previous previous
