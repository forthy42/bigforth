\ Print screen files
\
\ $Id: scr2ps.fs,v 1.1 2002/12/28 17:07:32 bernd Exp $

FORTH DEFINITIONS
DECIMAL


VOCABULARY POSTSCRIPT


\ increment byte in memory
: C+!  ( u c-addr -- )  DUP C@  ROT +  SWAP C! ;


POSTSCRIPT ALSO  DEFINITIONS

\ get # of pages in SCR file
: #PAGES  ( -- u )  CAPACITY 3 + 3 / ;

: PS-HEADER  ( -- ) ." %!PS-Adobe-1.0"
  CR ." %%Title: " file?
  CR ." %%Creator: scr2ps"
  CR ." %%Pages: " #PAGES U.
  CR ." %%DocumentMedia: a4 595 842 0 () ()"
  CR ." %%DocumentNeededResources: font Courier"
  CR ." %%EndComments"  ;

: PS-PROLOG  ( -- )
  CR ." %%BeginProlog"
  \ move to the next line
  CR ." /n {"
  CR ."   /y0 y0 bfs sub store"
  CR ."   x0 y0 moveto"
  CR ." } bind def"
  \ show and move to the next line
  CR ." /N {"
  CR ."   show n"
  CR ." } bind def"
  CR ." %%EndProlog" ;


\ Initialize page description variables
: INIT-VARIABLES  ( -- )
  CR ." /sw 842 def"		\ upper boarder
  CR ." /bfs 11 def"		\ font scale
  CR ." /x0 72 def"		\ initial x position of line
  CR ." /Courier findfont bfs scalefont setfont"
  ;

\ append string to PAD
: APPEND  ( c-addr1 u -- ) TUCK  PAD COUNT +  SWAP MOVE  PAD C+! ;


\ Redefine some standard words.
\ Output is written to buffer at PAD.
: SPACE  ( -- )  S"  " APPEND ;
: SPACES  ( u -- )  DUP 0> IF  1- FOR SPACE NEXT  EXIT THEN  DROP ;

: U.  ( u -- )  0 <# #S #>  APPEND ;
: U.R  ( u1 u2 -- )  >R  0 <# #S #>  R> OVER - SPACES  APPEND ;


\ escape special characters
: \EMIT  ( c -- )
  DUP [CHAR] ( =  OVER [CHAR] ) = OR  OVER [CHAR] \ = OR
  IF [CHAR] \ EMIT THEN  EMIT ;

: \TYPE  ( c-addr u -- )  ?DUP 0= IF DROP EXIT THEN
  1- FOR  COUNT \EMIT  NEXT  DROP ;


\ Type string in PAD enclosed with paranthesis and clear buffer.
: (TYPE  ( -- )   ." (" PAD COUNT  \TYPE  ." )"  0 PAD C! ;

\ type screen number
: .SCR#  ( -- )  S" SCR# " PAD PLACE  SCR @ U. ;

\ type line u in block
: .LINE ( u -- )   DUP 2 U.R  SPACE
  SCR @ BLOCK  SWAP C/L * +  C/L -TRAILING APPEND ;

\ type screen with PostScript commands
: LIST  ( u -- )  SCR !   CR .SCR# (TYPE ." N" 
  0  L/S 1- FOR  CR DUP .LINE (TYPE ." N"  1+  NEXT  DROP ;

: NEWLINE  ( -- )  CR ." n n" ;

\ One page contains three screens
: TRIAD  ( u -- )  3 / 3 * 
  CR ." %%Page: ? ?"		\ help Ghostview
  CR ." x0 sw 72 sub moveto"	\ goto title string position
  CR ." (" file? ." ) show"	\ bigFORTH: show file name at top
  CR ." /y0 sw 144 sub def"	\ initial y position of line
  CR ." x0 y0 moveto"		\ goto origin
  2 FOR  DUP CAPACITY U<  IF DUP LIST NEWLINE  1+ THEN  NEXT DROP
  CR ." showpage" ;

\ type complete file
: LISTALL  ( -- )
  0  CAPACITY 3 /  FOR  DUP TRIAD  3 +  NEXT  DROP ;

: PS-MAIN  ( -- )  INIT-VARIABLES LISTALL CR ;


FORTH DEFINITIONS

: SCR2PS  ( filename ( -- ) USE  PS-HEADER PS-PROLOG PS-MAIN ;



\ scr2ps minos.fb
\ bye

\\ Why can't this be used?
\ bigforth scr2ps.fs -e "scr2ps minos.fb bye" > minos.ps

