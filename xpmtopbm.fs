\ convert xpm mask to pbm

x11 also
\needs xconst import xconst
xconst also
minos also forth

: (flip-byte ( n -- n' )
    0 8 0 DO 2* over 1 and or swap 2/ swap LOOP nip ;

Create flip-byte  $100 0 [DO] [I] (flip-byte c, [LOOP]

: type_be ( addr u -- )
    bounds ?DO  I c@ flip-byte + c@ emit  LOOP ;

: xpm2pbm ( filename -- )
    icon-pixmap new icon-pixmap with
        screen xrc dpy @ shape @ 0 0 w @ h @ 1 ZPixmap XGetImage
    endwith
    { img |
      ." P4" cr ." # Icon shape" cr
      img XImage width @ 0 .r space img XImage height @ 0 .r cr
      img XImage data @
      img XImage height @ img XImage bytes_per_line @ * bounds
      ?DO
          I img XImage width @ 1- 3 >> 1+
          img XImage bitmap_bit_order @
          IF  type  ELSE  type_be  THEN
          img XImage bytes_per_line @ +LOOP
      img XDestroyImage } ;
