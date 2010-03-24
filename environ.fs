\               *** ENVIRONMENT? ***                   03may92py

Vocabulary environment  environment definitions
$FF Constant /counted-string
&66 Constant /hold
: /pad  &84 ;
8 Constant address-unit-bits
' true Alias floored
$FF Constant max-char
: max-d $7FFFFFFFFFFFFFFF. ;
$7FFFFFFF Constant max-n
-1 Constant max-u
: max-ud -1. ;
: return-stack-cells  r0 @ up@ udp @ + - cell/ ;
: stack-cells         s0 @        s^ @ - cell/ ;

base @ decimal
s" version.h" r/o open-file throw
pad $100 2 pick read-line throw drop  swap close-file throw
pad swap '" skip '" -skip
2dup 2dup '. scan nip - s>number drop -rot '. scan '. skip
2dup 2dup '. scan nip - s>number drop -rot '. scan '. skip
s>number drop
swap &100 * +

: bigFORTH  ( -- minor*100+minor-minor major ) [ -rot ] Literal Literal ;

s" minos-version.h" r/o open-file throw
pad $100 2 pick read-line throw drop  swap close-file throw
pad swap '" skip '" -skip
2dup 2dup '. scan nip - s>number drop -rot '. scan '. skip
2dup 2dup '. scan nip - s>number drop -rot '. scan '. skip
s>number drop
swap &100 * +

: MINOS  ( -- minor*100+minor-minor major ) [ -rot ] Literal Literal ;
base !

: forthid s" bigFORTH" ;
: forthver ( -- major minor minor-minor )
    bigFORTH &100 /mod swap ;

true Value core
true Value core-ext

\ other wordsets                                       28dec94py

true Value block
true Value block-ext

true Value double
true Value double-ext

true Value exception
true Value exception-ext

true Value facility
true Value facility-ext

true Value file
true Value file-ext

false Value floating
false Value floating-ext
8 Value floating-stack

-1 Value #locals
false Value locals
false Value locals-ext

true Value memory-alloc
true Value memory-alloc-ext

true Value tools
true Value tools-ext

true Value search-order
true Value search-order-ext
$10 Constant wordlists

true Value string
true Value string-ext

' noop alias X:deferred
' noop alias X:defined
' noop alias X:ekeys
' noop alias X:extension-query
' noop alias X:fp-stack
' noop alias X:ftrunc
' noop alias X:number-prefixes
' noop alias X:parse-name
\ ' noop alias X:required
' noop alias X:structures
' noop alias X:xchar

Forth definitions

: environment? ( addr u -- values t / f )
  & environment search-wordlist
  IF  execute true  ELSE  false  THEN ;
: environmental ( -- values t / f )  name count environment? ;
