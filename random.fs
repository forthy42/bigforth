\ generates random numbers                             12jan94py

\ Copyright (C) 1995 Free Software Foundation, Inc.

\ This file is part of Gforth.

\ Gforth is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public License
\ as published by the Free Software Foundation; either version 2
\ of the License, or (at your option) any later version.

\ This program is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\ GNU General Public License for more details.

\ You should have received a copy of the GNU General Public License
\ along with this program; if not, write to the Free Software
\ Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.

Variable seed  timer@ seed !

$10450405 Constant generator

Code rol9  AX 9 # rol  Next end-code macro
\ : rol9 $200 um* or ;

: rnd  ( -- n )  seed @ generator um* drop 1+ dup rol9 seed ! ;

: random ( n -- 0..n-1 )  rnd um* nip ;
