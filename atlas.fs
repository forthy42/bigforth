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

library liblapack liblapack.so.3

legacy off


liblapack clapack_dgesv
 int int int int int int int int (int) dgesv_ ( -- )

liblapack clapack_dgetrf
 int int int int int int (int) dgetrf_ ( -- )

liblapack clapack_dgetri
 int int int int int (int) dgetri_ ( -- )

liblapack dgesvd_
 int int int int int int int int int int int int int int (void) dgesvd_ ( -- )

legacy on
previous
Module;
