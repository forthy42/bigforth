\ misc canvas drawing

minos also forth

Variable step

decimal

: draw-mux ( state -- )
    ^ canvas with
	IF
	    2 0 to  2 2 to  -2 0 to  0 -4 to  2 0 to  0 4 to  -2 -2 to  -2 0 to
	ELSE
	    2 0 to  2 -2 to -2 0 to  0  4 to  2 0 to  0 -4 to -2  2 to  -2 0 to
	THEN
    endwith ;

: draw-misc ( o -- )
    canvas with
	clear
	2 linewidth
	 20  22 steps
	 02  20 home!  path
	 00  18 to  s" Bus" text
	 04  00 to
	 00 -08 to step @ 1 and draw-mux
	 step @ 3 and 3 =
	 IF  1 1 to -1 -1 to 1 -1 to -1 1 to
	 ELSE  2 0 to  -1 1 to 1 -1 to -1 -1 to 1 1 to -2 0 to
	 THEN
	 00 -06 to step @ 1 and draw-mux
	 1 1 to -1 -1 to 1 -1 to -1 1 to
	 00 -04 to
	-04  00 to stroke
	 10  22 steps
	 05  18 home!  path
	 01  00 to  00  01 to  s" PC" text
	 02  00 to 00 -02 to -02  00 to 00  01 to stroke
	 10  22 steps
	 05  14 home!  path
	 01  00 to  00  02 to  s" Inst" text  -01 00 to
	 03  00 to 00 -02 to -02  00 to 00  01 to stroke
	 10  22 steps
	 05  08 home!  path
	 01  00 to  00  01 to  s" DTR" text
	 02  00 to 00 -02 to -02  00 to 00  01 to stroke
	0 linewidth
     endwith ;

: misc-window ( -- )
    screen self window new window with
	['] draw-misc
	$80 $100 $80 $100 ^ canvas new
	0 S[ 1 step +! dpy draw ]S s" Next" ^ button new
	1 ^ habox new vfixbox
	2 ^ vabox new s" Misc" assign map
    endwith ;
