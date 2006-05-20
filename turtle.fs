\ Turtle graphics window                               09jun02py

script?  0 to script?
include turtle.m
to script?

turtle ptr turtle-win

also minos

: init-turtle
  turtle new bind turtle-win
  turtle-win self 0 s" Turtle Graphics" open-component ;

: fd  turtle-win graphics with path fd stroke endwith ;
: bk  turtle-win graphics with path bk stroke endwith ;
: lt  turtle-win graphics lt ;
: rt  turtle-win graphics rt ;
: bezier ( s2 alpha s1 -- )
  turtle-win graphics with
      path <bezier fd rt fd 3 bezier> stroke
  endwith ;
: steps ( w h -- )  2dup 2/ swap 2/ swap turtle-win homepos 2!
  turtle-win graphics with 2dup steps
        2/ swap 2/ swap home! endwith ;
: home! ( x y -- ) turtle-win graphics home! ;
: text  ( addr u -- )  turtle-win graphics text ;
: textpos ( x y -- )  turtle-win graphics textpos ;
: clear ( x y -- )  turtle-win graphics clear ;
: up ( -- )  turtle-win graphics up ;
: down ( -- )  turtle-win graphics down ;
: home turtle-win homepos 2@ turtle-win graphics with
        path home! 0 0 to stroke
  endwith ;

\ some examples                                        09jun02py

: rectangle ( n -- )  4 0 DO  dup fd 90 rt  LOOP  drop ;
: circle ( n -- )  36 0 DO  dup fd 10 rt  LOOP  drop ;

previous

\ usage examples                                       09jun02py

init-turtle

100 100 steps
40 rectangle
4 circle

