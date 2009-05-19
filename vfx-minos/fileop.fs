\ Fileoutput                                           12sep08py

Module FileOP

\ Alle FILEoutput Routinen                             25may97py

1 cells +User fhandle    fhandle off
1 cells +User oldout
| Create fchar 0 c,
| Create crlf  1 c, #lf c,
: FILEtype ( addr u sid -- ) drop fhandle @ write-file throw ;
: FILEemit ( char sid -- ) >r fchar tuck c! 1 r> FILEtype ;
: FILEemit? ( sid -- flag )  drop true ;
: FILEcr ( sid -- ) crlf count rot FILEtype ;
: FILEpage ( sid -- ) drop FILEcr $0C FILEemit FILEcr ;
: FILEflush ( sid -- ior ) drop 0 ;
: FILEeot ( sid -- ior ) drop
  oldout @ op-handle !  fhandle @ fhandle off close-file ;

create filedev-vectors
  ' .s ,
  ' FILEeot ,
  ' drop ,
  ' drop ,
  ' drop ,
  ' drop ,
  ' drop ,
  ' drop ,
  ' drop ,
  ' FILEemit ,
  ' FILEemit? ,
  ' FILEtype ,
  ' FILEcr ,
  ' FILEcr ,
  ' FILEpage ,
  ' drop ,
  ' noop ,
  ' drop ,
  ' drop ,
  ' 2drop ,
  ' FILEflush ,
  ' noop ,
Create FILEio-sid 0 , filedev-vectors , 0 ,
: FILEout  FILEio-sid op-handle ! ;


\ 0name >FILE +FILE                                    12mar00py

Module;

also fileop

: ?close ( -- )
    fhandle @ ?dup  IF  close-file drop  fhandle off  THEN ;

: output-file ( addr u mode -- ) ?close
  create-file throw fhandle !
  op-handle @ oldout !  FILEout ;

: eot  FILEio-sid op-handle @ = IF  FILEeot  THEN ;

previous
