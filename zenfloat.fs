( Very compressed one-screen floating-point          2116may98py
: D10*  D2* 2DUP D2* D2* D+ ; : D+-  0< IF DNEGATE THEN ;
: TRIM  >R TUCK DABS BEGIN OVER 0< OVER OR WHILE 0 10 UM/MOD >R
  10 UM/MOD NIP R> R> 1+ >R REPEAT ROT D+- DROP R> ;
: F+  ROT 2DUP - DUP 0< IF NEGATE ROT >R NIP >R SWAP R> ELSE
  SWAP >R NIP THEN >R S>D R> DUP 0 ?DO >R D10* R> 1- OVER ABS
  6553 > IF LEAVE THEN LOOP R> OVER + >R IF ROT DROP ELSE ROT
  S>D D+ THEN R> TRIM ;
: FNEGATE  >R NEGATE R> ; : F-  FNEGATE F+ ;
: F*  ROT + >R 2DUP XOR >R ABS SWAP ABS UM* R> D+- R> TRIM ;
: F/  OVER 0= ABORT" 0/" ROT SWAP - >R 2DUP XOR -ROT ABS DUP
  6553 MIN  ROT ABS 0 BEGIN 2DUP D10* NIP 3 PICK U< WHILE D10*
  R> 1- >R REPEAT 2SWAP DROP UM/MOD NIP 0 ROT D+- R> TRIM ;
: FLOAT  DPL @ NEGATE 1+ TRIM ; : F.  >R DUP ABS 0 <# R@ 0 MAX 0
  ?DO ASCII 0 HOLD LOOP R@ 0< IF R@ NEGATE 0 MAX 0 ?DO # LOOP
  ASCII . HOLD THEN R> DROP #S ROT SIGN #> TYPE SPACE ;
