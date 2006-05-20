float also

: SAVAGE  !1 !0  0 DO  FOVER F+ F**2  FSQRT  FLN  FEXP  FATAN  FTAN   LOOP  ;
: test  !time  250000 savage  !250000 f/ !1 f- fe. fdrop .time ;
		    
