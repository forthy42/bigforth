\ xft test

include xft-font.fs

minos also
Create XFT_MATRIX ,0" XFT_MATRIX"

screen xrc dpy @
screen xrc screen @
0" -xft-arial-medium-r-normal--12-*-*-*-p-*-iso8859-15" 0 0 XftXlfdParse
dup XFT_MATRIX Xft90Matrix XftPatternAddMatrix drop
pad
XftFontMatch
Value verdana-pat

screen xrc dpy @ verdana-pat XftFontOpenPattern Value xft-verdana-pat

screen xrc dpy @
screen xrc screen @
0" -xft-verdana-bold-r-normal--12-*-*-*-p-*-iso8859-15"
XftFontOpenXlfd Value xft-verdana

\ verdana-pat .pat

term dpy dpy xrc dpy @
term dpy dpy xwin @
term dpy dpy xrc vis @
term dpy dpy xrc cmap @ XftDrawCreate Value xft-draw

Create xr-black 0 w, $0 w, 0 w, $FFFF w,
Create xft-black sizeof XftColor allot

term dpy dpy xrc dpy @
term dpy dpy xrc vis @
term dpy dpy xrc cmap @ xr-black xft-black XftColorAllocValue drop

xft-draw xft-black xft-verdana 200 160 S" Dies ist ein Test" XftDrawString8
xft-draw xft-black xft-verdana-pat 20 180 S" Dies ist ein Test" XftDrawString8

\ Debugging tools

import float float also

: .val ( val -- )
    BEGIN  dup  WHILE  dup XftValueList value
	dup XftValue i swap XftValue type @ CASE
	    0  OF  ." void" drop  ENDOF
	    1  OF  @ .  ENDOF
	    2  OF  df@ f.  ENDOF
	    3  OF  @ >len type  ENDOF
	    4  OF  @ IF  ." true"  ELSE  ." false"  THEN  ENDOF
	    5  OF  @ '( emit  2 FOR  dup df@ f. ', emit dfloat+  NEXT
		df@ f. ') emit  ENDOF
	    nip  ENDCASE
	@  REPEAT  drop ;

: .pat ( pattern -- )
    dup XftPattern elts @ swap XftPattern num @ 0 ?DO  cr
	dup I sizeof XftPatternElt * +
	dup XftPatternElt object @ >len type space
	XftPatternElt values @ .val
    LOOP
    drop ;