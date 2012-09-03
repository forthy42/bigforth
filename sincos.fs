\ fraction sincos

\ uses sin(a+b) = sin a cos b + cos a sin b
\ uses cos(a+b) = cos a cos b - sin a sin b
\ sin a = a for small a, cos a = 1 for small a
\ thus sin(a+b) = a cos b + sin b
\      cos(a+b) = cos b + a sin b
\ furthermore: cos(a) = 1-2*sin²(a/2)

base @ hex

| Create sintab
     0 w,   100 w,   200 w,   300 w,   400 w,   500 w,   600 w,   700 w,
   7FF w,   8FF w,   9FF w,   AFF w,   BFF w,   CFE w,   DFE w,   EFE w,
   FFD w,  10FD w,  11FC w,  12FB w,  13FB w,  14FA w,  15F9 w,  16F8 w,
  17F7 w,  18F6 w,  19F4 w,  1AF3 w,  1BF2 w,  1CF0 w,  1DEE w,  1EEC w,
  1FEA w,  20E8 w,  21E6 w,  22E4 w,  23E1 w,  24DF w,  25DC w,  26D9 w,
  27D6 w,  28D3 w,  29D0 w,  2ACC w,  2BC8 w,  2CC5 w,  2DC1 w,  2EBC w,
  2FB8 w,  30B3 w,  31AF w,  32AA w,  33A4 w,  349F w,  359A w,  3694 w,
  378E w,  3888 w,  3981 w,  3A7A w,  3B74 w,  3C6C w,  3D65 w,  3E5E w,
  3F56 w,  404E w,  4145 w,  423D w,  4334 w,  442B w,  4521 w,  4618 w,
  470E w,  4804 w,  48F9 w,  49EE w,  4AE3 w,  4BD8 w,  4CCC w,  4DC0 w,
  4EB4 w,  4FA8 w,  509B w,  518E w,  5280 w,  5372 w,  5464 w,  5556 w,
  5647 w,  5738 w,  5828 w,  5918 w,  5A08 w,  5AF8 w,  5BE7 w,  5CD5 w,
  5DC4 w,  5EB2 w,  5FA0 w,  608D w,  617A w,  6266 w,  6352 w,  643E w,
  6529 w,  6614 w,  66FF w,  67E9 w,  68D3 w,  69BC w,  6AA5 w,  6B8E w,
  6C76 w,  6D5E w,  6E45 w,  6F2C w,  7012 w,  70F8 w,  71DD w,  72C2 w,
  73A7 w,  748B w,  756F w,  7652 w,  7735 w,  7817 w,  78F9 w,  79DB w,
  7ABB w,  7B9C w,  7C7C w,  7D5B w,  7E3A w,  7F19 w,  7FF7 w,  80D4 w,
  81B1 w,  828E w,  836A w,  8445 w,  8520 w,  85FA w,  86D4 w,  87AE w,
  8886 w,  895F w,  8A36 w,  8B0E w,  8BE4 w,  8CBA w,  8D90 w,  8E65 w,
  8F39 w,  900D w,  90E1 w,  91B3 w,  9286 w,  9357 w,  9428 w,  94F9 w,
  95C9 w,  9698 w,  9767 w,  9835 w,  9902 w,  99CF w,  9A9C w,  9B67 w,
  9C33 w,  9CFD w,  9DC7 w,  9E90 w,  9F59 w,  A021 w,  A0E8 w,  A1AF w,
  A275 w,  A33B w,  A400 w,  A4C4 w,  A588 w,  A64B w,  A70D w,  A7CF w,
  A890 w,  A950 w,  AA10 w,  AACF w,  AB8D w,  AC4B w,  AD08 w,  ADC4 w,
  AE80 w,  AF3B w,  AFF5 w,  B0AF w,  B167 w,  B220 w,  B2D7 w,  B38E w,
  B444 w,  B4FA w,  B5AE w,

base !

$6487F Constant tau
tau 1 + 4 / Constant pi/2
tau 3 + 8 / Constant pi/4
 $B505 Constant sqrt.5

| : (sini ( n -- sin[n] ) 2* sintab + w@ ;
| : (cosi ( n -- cos[n] ) 2/ (sini dup * $F rshift $10000 swap - ;

: sin ( n -- )  pi/4 /mod 7 and >r
    r@ 1 and IF  pi/4 swap -  THEN
    r@ 3 and 1 3 within  IF  2/  THEN
    dup $FF and >r 8 rshift dup (sini swap (cosi r> * $10 rshift +
    r@ 3 and 1 3 within  IF  dup * $F rshift $10000 swap -  THEN
    r> 4 and IF  negate  THEN ;

: cos ( n -- )  pi/2 + sin ;

: sincos ( n -- sin cos )  dup sin swap cos ;

[defined] VFXFORTH [IF]
    \ *** Steve: Write a better d>> ***
    : ud/mod ( d n -- n d )
	>r 0 r@ um/mod r> swap >r um/mod r> ;
    : d>> ( d n -- d' )
	1 swap lshift ud/mod rot drop ;
[ELSE]
    Code d>> ( d n -- d' )
	AX CX mov  AX pop  DX pop  
	AX DX shrd   AX sar  DX push  Next end-code
[THEN]