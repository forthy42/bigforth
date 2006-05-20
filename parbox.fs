\ parbox                                               18mar00py

minos also

: test-parbox ( box -- )
  screen self window new window with
     s" Test" assign show up@ app !  endwith ;

Variable sample$

: samples ( -- boxes... n )
s" This program is free software; you can redistribute it"
sample$ $!
s"  and/or modify it under the terms of the GNU General Public"
sample$ $+!
s"  License as published by the Free Software Foundation;"
sample$ $+!
s"  either version 2 of the License, or (at your option) any"
sample$ $+!
s"  later version."
sample$ $+!
sample$ $@ ;

text-parbox ptr test1

: do-samples
  samples block-par text-parbox new dup bind test1
  font" -*-times-medium-r-normal--14-*-*-*-p-*-iso8859-1"
  samples center-par text-parbox new
  font" -*-times-medium-r-normal--14-*-*-*-p-*-iso8859-1"
  samples left-par text-parbox new
  font" -*-times-medium-r-normal--14-*-*-*-p-*-iso8859-1"
  samples right-par text-parbox new
  font" -*-times-medium-r-normal--14-*-*-*-p-*-iso8859-1"
  320 1 *fill 0 0 glue  new
  0 S[ widget dpy close ]S s" Ok" button new
  dup 1 habox new panel
  6 rot modal new
 test-parbox ;

script? [IF] do-samples stop bye [THEN]

toss
