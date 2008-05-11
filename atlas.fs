\ atlas library bindings
\ Copyright (C) 2007, Sergey Plis
\
\ This program is free software; you can redistribute it and/or modify
\ it under the terms of the GNU General Public License as published by
\ the Free Software Foundation; either version 2 of the License, or
\ (at your option) any later version.
\
\ This program is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\ GNU General Public License for more details.

Module atlas

also DOS

library libatlas libatlas.so
library liblapack liblapack.so depends libatlas

legacy off warnings off

\ these functions are provided by atlas, they make life so much easier
\ that inversion of matrices is implemented through them rather than
\ plain fortran functions of lapack, drawback is only one -- you need
\ atlas installed, but this binding is to atlas, remember!
liblapack clapack_dgesv  [ 8 ] ints (int) clapack_dgesv  ( -- )
liblapack clapack_dgetrf [ 6 ] ints (int) clapack_dgetrf ( -- )
liblapack clapack_dgetri [ 5 ] ints (int) clapack_dgetri ( -- )

\ plain fortran routines
liblapack dgetrf         [ 6 ] ints (void) dgetrf_ 
liblapack dgetri         [ 7 ] ints (void) dgetri_ 
liblapack dgesvd        [ 14 ] ints (void) dgesvd_ ( -- )
liblapack dpotrf         [ 5 ] ints (void) dpotrf_ ( -- )

legacy on warnings on

previous

Module;
