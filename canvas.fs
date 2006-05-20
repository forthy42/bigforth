minos also

canvas ptr canvas-test

: canvas-win
    screen self window new window with
	1 1 ^ viewport new viewport with
	    :[ canvas with clear endwith ]:
	    $200 0 $200 0 ^ canvas new dup [ also Forth ] bind canvas-test [ toss ]
	    assign ^
	endwith
	^ sliderview new s" Canvas-Test" assign
	map
    endwith ;
