\ Char+ chars postpone compile, char [char] key        20apr92py

: ANS ;

' key Alias ekey                ' key? Alias ekey?

| User pending  pending off
| : char? ( key -- flag )
    $FF and 0<>  kbshift @ 0>= and ;
: key? ( -- flag )  pending c@ IF  true exit  THEN
  ekey? dup  IF  drop ekey dup pending c! char?
                 dup 0= IF  0 pending c!  THEN  THEN ;
: key  key? IF  pending c@  0 pending c! exit  THEN
  BEGIN  ekey dup $100 u>=  WHILE  drop  REPEAT  ;
: ekey>char ( ekey -- char t / f )  dup $100 u>=
  dup 0= IF nip THEN ;
\ Simpler: : key ekey $FF and ;

\ defer@/defer!/action-of

: defer@ ( xt -- xt )  >body @ ;
: defer! ( xt1 xt2 -- )  >body ! ;
' what's Alias action-of

\ obsolescent
User span
: expect  accept span ! ;
: convert  -1 >number drop ;
\ end obsolescent

\needs environment? include environ.fs
\needs locals| include locals.fs
