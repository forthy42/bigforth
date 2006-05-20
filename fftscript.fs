\ checks FFT

\needs fft include fft.fb
8 points
!1  !0  0 values cf!
!1  !1  1 values cf!
!0  !1  2 values cf!
!-1 !1  3 values cf!
!-1 !0  4 values cf!
!-1 !-1 5 values cf!
!0  !-1 6 values cf!
!1  !-1 7 values cf!
cr .values
fft
cr .rvalues
rfft
cr .values

create testvector1 0   ,  1 ,   0 ,  1 ,   0 , 1 ,   0 ,   1 ,
  0 ,  1 ,  0 ,  1 ,   0 , 1 ,   0 ,   1 ,
create rresult1    8   ,  0 ,   0 ,  0 ,   0 , 0 ,   0 ,   0 ,
-8 ,  0 ,  0 ,  0 ,   0 , 0 ,   0 ,   0 ,
create iresult1    0   ,  0 ,   0 ,  0 ,   0 , 0 ,   0 ,   0 ,
  0 ,  0 ,  0 ,  0 ,   0 , 0 ,   0 ,   0 ,

create testvector2 1   ,  2 ,   1 ,  0 ,   1 , 2 ,   1 ,   0 ,
  1 ,  2 ,  1 ,  0 ,   1 , 2 ,   1 ,   0 ,
create rresult2    16  ,  0 ,   0 ,  0 ,   0 , 0 ,   0 ,   0 ,
  0 ,  0 ,  0 ,  0 ,   0 , 0 ,   0 ,   0 ,
create iresult2    0   ,  0 ,   0 ,  0 ,  -8 , 0 ,   0 ,   0 ,
  0 ,  0 ,  0 ,  0 ,   8 , 0 ,   0 ,   0 ,

create testvector3 1   ,  2 ,   3 ,  4 ,   1 , 2 ,   3 ,   4 ,
  1 ,  2 ,  3 ,  4 ,   1 , 2 ,   3 ,   4 ,
create rresult3    40  ,  0 ,   0 ,  0 ,  -8 , 0 ,   0 ,   0 ,
-8 ,  0 ,  0 ,  0 ,  -8 , 0 ,   0 ,   0 ,
create iresult3    0   ,  0 ,   0 ,  0 ,   8 , 0 ,   0 ,   0 ,
  0 ,  0 ,  0 ,  0 ,  -8 , 0 ,   0 ,   0 ,

create testvector4 -15 , -1 ,   0 , 15 ,   2 , 1 ,   0 , -10 ,
-15 , -1 ,  0 , 15 ,   2 , 1 ,   0 , -10 ,
create rresult4    -16 ,  0 , -72 ,  0 , -26 , 0 ,   4 ,   0 ,
-36 ,  0 ,  4 ,  0 , -26 , 0 , -72 ,   0 ,
create iresult4    0   ,  0 , -32 ,  0 ,  10 , 0 , -32 ,   0 ,
  0 ,  0 , 32 ,  0 , -10 , 0 ,  32 ,   0 ,

create testvector5 2   ,  0 , -2  ,  0 ,   2 , 0 ,  -2 ,   0 ,
  2 ,  0 , -2 ,  0 ,   2 , 0 ,  -2 ,   0 ,
create rresult5    0   ,  0 ,   0 ,  0 ,  16 , 0 ,   0 ,   0 ,
  0 ,  0 ,  0 ,  0 ,  16 , 0 ,   0 ,   0 ,
create iresult5    0   ,  0 ,   0 ,  0 ,   0 , 0 ,   0 ,   0 ,
  0 ,  0 ,  0 ,  0 ,   0 , 0 ,   0 ,   0 ,

16 points

: c=       ( re2 im2 re1 im1 -- flag )
     frot f=
     f= and
;

: setup-fft ( in -- )
    16 0 DO dup I cells + @ s>d d>f 0e i values cf! LOOP
    drop ;
: check-fft ( outre outim -- )
    16 0 DO
	over i cells + @ s>d d>f
	dup  i cells + @ s>d d>f
	i values cf@ c=
	IF
	    cr I 3 .r ."  tests OK"
	ELSE
	    cr I 3 .r ."  wrong, test data is: "
	    over i cells + @ s>d d>f
	    dup  i cells + @ s>d d>f c.
	    ."  computed data is: " i values cf@ c.
	then
    LOOP
    2drop ;

: (test-fft)  ( in outre outim -- )
    rot setup-fft
    16 true (fft
    check-fft
;

: test1 testvector1 rresult1 iresult1 (test-fft) ;
: test2 testvector2 rresult2 iresult2 (test-fft) ;
: test3 testvector3 rresult3 iresult3 (test-fft) ;
: test4 testvector4 rresult4 iresult4 (test-fft) ;
: test5 testvector5 rresult5 iresult5 (test-fft) ;

test1
test2
test3
\ test4
test5