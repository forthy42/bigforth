\ Fileoutput                                           12sep08py

Module FileOP

\ Alle FILEoutput Routinen                             25may97py

1 cells +User fhandle    fhandle off
1 cells +User oldout
| Create fchar 0 c,
| Create crlf  1 c, #lf c,
: FILEtype ( addr u sid -- ) drop fhandle @ write-file throw ;
: FILEemit ( char sid -- ) drop fchar tuck c! 1 FILEtype ;
: FILEcr ( sid -- ) drop  crlf count FILEtype ;
: FILEpage ( sid -- ) drop FILEcr $0C FILEemit FILEcr ;
: FILEeot ( sid -- ) drop
  oldout @ op-handle !  fhandle @ fhandle off close-file throw ;

create filedev-vectors
  ' drop ,
  ' FILEeot ,
  ' drop ,
  ' drop ,
  ' drop ,
  ' drop ,
  ' drop ,
  ' drop ,
  ' drop ,
  ' FILEemit ,
  ' drop ,
  ' FILEtype ,
  ' FILEcr ,
  ' drop ,
  ' FILEpage ,
  ' drop ,
  ' drop ,
  ' drop ,
  ' drop ,
  ' drop ,
  ' drop  ,
  ' drop ,
  ' drop ,
  ' drop ,
  ' drop ,
: FILEout  filedev-vectors op-handle ! ;


\ 0name >FILE +FILE                                    12mar00py

Module;

also fileop

: ?close ( -- )
    fhandle @ ?dup  IF  close-file drop  fhandle off  THEN ;

: output-file ( addr u mode -- ) ?close
  create-file throw fhandle !
  op-handle @ oldout !  FILEout ;

: eot  ?close  oldout @ op-handle ! ;

previous
