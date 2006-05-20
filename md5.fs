\ $Id: md5.fs,v 1.1 2002/12/28 17:07:32 bernd Exp $
\
\ An Implementation of the MD5 algorithm in ANS-Forth
\
\ This implementation is based on the reference implementation
\ by RSA Data Security, Inc. as published in RFC 1321
\
\ This program is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public License as 
\ published by the Free Software Foundation; either version 2 of the 
\ License, or (at your option) any later version. 
\
\ Questions and comments are welcome. Please write or mail.
\
\ Ulrich Hoffmann, Sehestedter Str. 26, D-24340 Eckernfoerde
\ uho@informatik.uni-kiel.de, uho@pizzicato.forth-ev.de
\
\ Standard conformant labeling 
\
\ This is an ANS Forth Program with environmental dependencies, 
\    - Requiring HEX <> .( \ from the Core Extensions word set.
\    - Requiring the Exception word set.
\    - Requiring ABORT from the Exception Extensions word set.
\    - Requiring the Locals word set.
\    - Requiring the Locals Extensions word set.
\    - Requiring the Memory-Allocation word set.
\    - Requiring [THEN] [IF] from the Programming-Tools Extensions word set.
\    - Requiring the String word set.
\ 
\ Required program documentation 
\ 
\    - Environmental dependencies
\      * the size of integer numbers must be 32 bits
\      * the integer arithmetic must be 32 bits 2's complement cyclic 
\        without overflow detection
\      * Declaring more than eight locals in a single definition.
\ 
\    - Other program documentation
\      * No operator terminal facilities are required.
\      * After loading this program, a Standard System still exists.
\      * This program requires no blocks of mass storage.
 
 
-1 4294967295 <> [IF]  
  .( This program requires 32 Bit cyclic arithmetic! )
  ABORT
[THEN]


HEX

: low-order! ( x addr -- ) 
    OVER 0000000FF AND              OVER            C!  
    OVER 00000FF00 AND  8 RSHIFT    OVER CHAR+      C!  
    OVER 000FF0000 AND 10 RSHIFT    OVER 2 CHARS +  C!  
    SWAP 0FF000000 AND 18 RSHIFT    SWAP 3 CHARS +  C! ;

: low-order@ ( addr -- x )
   DUP C@
   OVER   CHAR+   C@  8 LSHIFT OR
   OVER 2 CHARS + C@ 10 LSHIFT OR
   SWAP 3 CHARS + C@ 18 LSHIFT OR ;
 

DECIMAL

: 3-roll ( a b c d -- d a b c ) SWAP >R SWAP >R SWAP R> R> ;

: ROTATE ( x1 u -- x2 )  \ cyclic rotate 32 Bit word left u bits 
    2DUP LSHIFT >R    32 SWAP - RSHIFT  R> OR ;



HEX

CREATE T 
  D76AA478 , ( 1 )    E8C7B756 , ( 2 )   242070DB , ( 3 )   C1BDCEEE , ( 4 )
  F57C0FAF , ( 5 )    4787C62A , ( 6 )   A8304613 , ( 7 )   FD469501 , ( 8 )
  698098D8 , ( 9 )    8B44F7AF , ( 10 )  FFFF5BB1 , ( 11 )  895CD7BE , ( 12 )
  6B901122 , ( 13 )   FD987193 , ( 14 )  A679438E , ( 15 )  49B40821 , ( 16 )
  F61E2562 , ( 17 )   C040B340 , ( 18 )  265E5A51 , ( 19 )  E9B6C7AA , ( 20 )
  D62F105D , ( 21 )    2441453 , ( 22 )  D8A1E681 , ( 23 )  E7D3FBC8 , ( 24 )
  21E1CDE6 , ( 25 )   C33707D6 , ( 26 )  F4D50D87 , ( 27 )  455A14ED , ( 28 )
  A9E3E905 , ( 29 )   FCEFA3F8 , ( 30 )  676F02D9 , ( 31 )  8D2A4C8A , ( 32 )
  FFFA3942 , ( 33 )   8771F681 , ( 34 )  6D9D6122 , ( 35 )  FDE5380C , ( 36 )
  A4BEEA44 , ( 37 )   4BDECFA9 , ( 38 )  F6BB4B60 , ( 39 )  BEBFBC70 , ( 40 )
  289B7EC6 , ( 41 )   EAA127FA , ( 42 )  D4EF3085 , ( 43 )   4881D05 , ( 44 )
  D9D4D039 , ( 45 )   E6DB99E5 , ( 46 )  1FA27CF8 , ( 47 )  C4AC5665 , ( 48 )
  F4292244 , ( 49 )   432AFF97 , ( 50 )  AB9423A7 , ( 51 )  FC93A039 , ( 52 )
  655B59C3 , ( 53 )   8F0CCC92 , ( 54 )  FFEFF47D , ( 55 )  85845DD1 , ( 56 )
  6FA87E4F , ( 57 )   FE2CE6E0 , ( 58 )  A3014314 , ( 59 )  4E0811A1 , ( 60 )
  F7537E82 , ( 61 )   BD3AF235 , ( 62 )  2AD7D2BB , ( 63 )  EB86D391 , ( 64 )

DECIMAL

: md5-Operation: ( xt -- ) 
    CREATE ,
  DOES> ( a b c d k s i -- a' b c d )
    LOCALS| 'xt X i s k d c b a |
    a   
    b c d    'xt @ EXECUTE +  
    X k CELLS + low-order@ +  
    T i 1- CELLS + @       +
    s ROTATE 
    b +
    TO a
    a b c d ;

: md5-F ( x y z -- r )  >R OVER AND SWAP INVERT R> AND OR ;
: md5-G ( x y z -- r )  DUP >R INVERT AND SWAP R> AND OR ;
: md5-H ( x y z -- r )  XOR XOR ;
: md5-I ( x y z -- r )  INVERT ROT OR XOR ;

' md5-F md5-Operation: md5-FF
' md5-G md5-Operation: md5-GG
' md5-H md5-Operation: md5-HH
' md5-I md5-Operation: md5-II

: md5-block ( a b c d addr -- a' b' c' d' )
   LOCALS| X dd cc bb aa |
   aa bb cc dd 

   ( round 1 )
     0  7  1 X md5-FF   3-roll    1 12  2 X md5-FF   3-roll   
     2 17  3 X md5-FF   3-roll    3 22  4 X md5-FF   3-roll
     4  7  5 X md5-FF   3-roll    5 12  6 X md5-FF   3-roll   
     6 17  7 X md5-FF   3-roll    7 22  8 X md5-FF   3-roll
     8  7  9 X md5-FF   3-roll    9 12 10 X md5-FF   3-roll  
    10 17 11 X md5-FF   3-roll   11 22 12 X md5-FF   3-roll
    12  7 13 X md5-FF   3-roll   13 12 14 X md5-FF   3-roll  
    14 17 15 X md5-FF   3-roll   15 22 16 X md5-FF   3-roll

   ( round 2 )
     1  5 17 X md5-GG   3-roll    6  9 18 X md5-GG   3-roll  
    11 14 19 X md5-GG   3-roll    0 20 20 X md5-GG   3-roll
     5  5 21 X md5-GG   3-roll   10  9 22 X md5-GG   3-roll  
    15 14 23 X md5-GG   3-roll    4 20 24 X md5-GG   3-roll
     9  5 25 X md5-GG   3-roll   14  9 26 X md5-GG   3-roll   
     3 14 27 X md5-GG   3-roll    8 20 28 X md5-GG   3-roll
    13  5 29 X md5-GG   3-roll    2  9 30 X md5-GG   3-roll   
     7 14 31 X md5-GG   3-roll   12 20 32 X md5-GG   3-roll

   ( round 3 ) 
    5  4 33 X md5-HH   3-roll     8 11 34 X md5-HH   3-roll  
   11 16 35 X md5-HH   3-roll    14 23 36 X md5-HH   3-roll
    1  4 37 X md5-HH   3-roll     4 11 38 X md5-HH   3-roll   
    7 16 39 X md5-HH   3-roll    10 23 40 X md5-HH   3-roll
   13  4 41 X md5-HH   3-roll     0 11 42 X md5-HH   3-roll   
    3 16 43 X md5-HH   3-roll     6 23 44 X md5-HH   3-roll
    9  4 45 X md5-HH   3-roll    12 11 46 X md5-HH   3-roll  
   15 16 47 X md5-HH   3-roll     2 23 48 X md5-HH   3-roll

   ( round 4 )
    0  6 49 X md5-II   3-roll     7 10 50 X md5-II   3-roll  
   14 15 51 X md5-II   3-roll     5 21 52 X md5-II   3-roll
   12  6 53 X md5-II   3-roll     3 10 54 X md5-II   3-roll  
   10 15 55 X md5-II   3-roll     1 21 56 X md5-II   3-roll
    8  6 57 X md5-II   3-roll    15 10 58 X md5-II   3-roll   
    6 15 59 X md5-II   3-roll    13 21 60 X md5-II   3-roll
    4  6 61 X md5-II   3-roll    11 10 62 X md5-II   3-roll   
    2 15 63 X md5-II   3-roll     9 21 64 X md5-II   3-roll

   dd + 3-roll   
   cc + 3-roll  
   bb + 3-roll  
   aa + 3-roll ;
    
8 CONSTANT bits/char

: md5-final ( a b c d  c-addr u len -- a' b' c' d' )  \ u < 16 CELLS
   16 CELLS ALLOCATE THROW  
   LOCALS| X len u c-addr |
   X 16 CELLS 0 FILL
   c-addr X u CHARS CMOVE
   [ HEX ] 80  X u CHARS + C! [ DECIMAL ]
   u 1+ CHARS 14 CELLS < 0= IF \ padding will exceed block
      X md5-block
      X 16 CELLS 0 FILL 
   THEN
   len bits/char UM* 
   SWAP X 14 CELLS + low-order! 
        X 15 CELLS + low-order!
   X md5-block 
   X FREE THROW ;

: md5 ( c-addr u -- u1 u2 u3 u4 ) 
   DUP >R  >R >R  
   [ HEX ]  
   67452301 ( a )  
   EFCDAB89 ( b )  
   98BADCFE ( c )   
   10325476 ( d ) 
   [ DECIMAL ]
   R> R> ( a b c d c-addr u ) 
   BEGIN
      DUP 16 CELLS < 0= 
   WHILE ( a b c d c-addr u )
      >R DUP >R ( a b c d c-addr )  
      md5-block ( a b c d )
      R> R> 16 CELLS /STRING 
   REPEAT  ( a b c d c-addr u )
   R> ( a b c d c-addr u len )
   md5-final 
;

HEX
: ##.##.##.## ( x -- )
    DUP 0FF000000 AND 18 RSHIFT 0 # #  2DROP 
    DUP 000FF0000 AND 10 RSHIFT 0 # #  2DROP
    DUP 00000FF00 AND  8 RSHIFT 0 # #  2DROP
        0000000FF AND           0 # #  2DROP ;
DECIMAL

: fingerprint ( a b c d -- c-addr u )
    BASE @ >R HEX
    <# ##.##.##.##  
       ##.##.##.## 
       ##.##.##.## 
       ##.##.##.## 0. #> 
    R> BASE ! ;

: .md5 ( c-addr u -- )  md5 fingerprint TYPE ;


: test-md5 ( c-addr1 u1 c-addr2 u2 -- )
   >R >R
   ." MD5 ("  [CHAR] " EMIT  2DUP TYPE  [CHAR] " EMIT ." ) = " 
   md5 fingerprint 2DUP TYPE 
   R> R> COMPARE IF  ." , FAILED!"  ELSE  ." , passed."  THEN
   CR ;

: test-suite ( -- )
   CR ." MD5 test suite:" CR
   S" "   
   S" D41D8CD98F00B204E9800998ECF8427E" test-md5
   S" a" 
   S" 0CC175B9C0F1B6A831C399E269772661" test-md5
   S" abc" 
   S" 900150983CD24FB0D6963F7D28E17F72" test-md5
   S" message digest" 
   S" F96B697D7CB7938D525A2F31AAF161D0" test-md5
   S" abcdefghijklmnopqrstuvwxyz" 
   S" C3FCD3D76192E4007DFB496CCA67E13B" test-md5
   S" ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789" 
   S" D174AB98D277D9F5A5611C2C9F419D9F" test-md5
   S" 12345678901234567890123456789012345678901234567890123456789012345678901234567890" 
   S" 57EDF4A22BE3C955AC49DA2E2107B67A" test-md5
;
 
CR .( Usage:  <c-addr> <len> .md5 )
CR .(         test-suite )
