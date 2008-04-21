#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class gpl-about
public:
  early widget
  early open
  early dialog
  early open-app
  stredit ptr COPYING
  button ptr gpl-ok
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ gpl-ok self ]DF s" About bigFORTH+MINOS" open-component ;
  : dialog   new DF[ gpl-ok self ]DF s" About bigFORTH+MINOS" open-dialog ;
  : open-app new DF[ gpl-ok self ]DF s" About bigFORTH+MINOS" open-application ;
class;

component class minos-splash
public:
  early widget
  early open
  early dialog
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Minos Splash Screen" open-component ;
  : dialog   new DF[ 0 ]DF s" Minos Splash Screen" open-dialog ;
  : open-app new DF[ 0 ]DF s" Minos Splash Screen" open-application ;
class;

minos-splash implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
         icon" icons/minos" icon new 
      &1 vabox new &2 borderbox
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 :: init ;
class;

gpl-about implements
 ( [methodstart] ) : show ( -- ) [ also DOS ]
  s" COPYING" r/o open-file
  0= IF  copying assign  copying resized
         0 copying edifile !
         1 $10 dpy geometry
  ELSE   drop  THEN
  [ previous ] ; ( [methodend] ) 
  : widget  ( [dumpstart] )
            X" bigFORTH is a native code Forth system, MINOS is a GUI library." text-label new 
            X" Copyright (c) 1998-2008 by Bernd Paysan" text-label new 
              $10 $1 *hfil $10 $1 *vfil glue new 
               icon" icons/minos" icon new 
              $10 $1 *hfil $10 $1 *vfil glue new 
            &3 habox new vfixbox 
          &3 vabox new vfixbox 
            X" This program is free software; you can redistribute it and/or modify" text-label new 
            X" it under the terms of the GNU General Public License as published by" text-label new 
            X" the Free Software Foundation; either version 3 of the License, or" text-label new 
            X" (at your option) any later version." text-label new 
          &4 vabox new vfixbox 
            X" This program is distributed in the hope that it will be useful," text-label new 
            X" but WITHOUT ANY WARRANTY; without even the implied warranty of" text-label new 
            X" MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the" text-label new 
            X" GNU General Public License for more details." text-label new 
          &4 vabox new vfixbox 
            X" You should have received a copy of the GNU General Public License" text-label new 
            X" along with this program.  If not, see <http://www.gnu.org/licenses/>." text-label new 
          &2 vabox new vfixbox 
            1 1 vviewport new  DS[ 
               (straction stredit new  ^^bind COPYING $42 setup-edit 
            &1 vabox new ]DS ( MINOS ) 
            $0 $1 *hfil $0 $1 *vfil glue new 
          &2 habox new
        &5 vabox new &1 vskips
          $10 $1 *hfill $10 $1 *vfil glue new 
          ^^ S[ close ]S ( MINOS ) X"  OK " button new  ^^bind gpl-ok
          $10 $1 *hfill $10 $1 *vfil glue new 
        &3 habox new vfixbox 
      &2 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 :: init ;
class;

previous previous previous
