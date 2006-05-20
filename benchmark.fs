\ Forth Inc. benchmark tests adapted by Tom Zimmer, MPE, et. al.
\ Other tests added by MPE.

\ The application tests have been separated from the primitive tests.
\ Constants have been declared and modified so that the runtimes
\ of the application tests (Sieve, Fibonacci, QuickSort) can be
\ made similar.

\ The QuickSort test has been refactored to reduce the effect of the
\ array initialisation, and this is tested in a separate test.

\ Note that SwiftForth 2.0 includes special optimiser rules to eliminate
\ some of the benchmark code! This is seen in the some of the primitive
\ test results which are faster than the DO ... LOOP test.

\ Note th use of the word [o/n] whose job is to stop some optimising compilers
\ from throwing away the multiply and divide operations.
\ The implementation of [o/n] should lay a NOP opcode opcode on optimising
\ systems, and may be an immediate NOOP on others

decimal

\ ************************************************
\ Select system to be tested, set FORTHSYSTEM
\ to value of selected target.
\ Set SPECIFICS false to avoid system dependencies
\ ************************************************

7 constant ForthSystem
false constant specifics        \ true to use system dependent code

1  constant PfwVfx              \ MPE ProForth VFX 3.0
2  constant Pfw22               \ MPE ProForth 2.2
3  constant SwiftForth20        \ FI SwiftForth 2.0
4  constant SwiftForth15        \ FI SwiftForth 1.5
5  constant Win32Forth          \ Win32Forth 4.2
6  constant BigForth            \ BigForth 26sep1999
7  constant BigForth-Linux      \ BigForth 26sep1999

: .specifics    \ -- ; display trick state
  ."  using"  specifics 0=
  if  ."  no"  then
  ."  extensions"
;


\ ********************
\ ProForth VFX harness
\ ********************

PfwVfx ForthSystem = [if]
extern: DWORD PASCAL GetTickCount( void )

: COUNTER       \ -- ms
  GetTickCount ;

: >pos          \ n -- ; step to position n
  out @ - spaces
;

: [o/n]         \ --
  postpone []
; immediate
[then]


\ ********************
\ ProForth 2.2 harness
\ ********************

Pfw22 ForthSystem = [if]

include valPFW22

: COUNTER       \ -- ms
  WinGetTickCount ;

: >pos          \ n -- ; step to position n
  out @ - spaces
;

: M/            \ d n1 -- quot
  m/mod nip
;

: buffer:       \ n -- ; -- addr
  create
    here  over allot  swap erase
;

: m+            \ d n -- d'
  s>d d+
;

: [o/n]         \ -- ; stop optimiser treating * DROP etc as no code
; immediate

: SendMessage   \ hwn msg wparam lparam -- result
  WinSendMessage
;
[then]


\ ********************
\ SwiftForth15 harness
\ ********************

SwiftForth15 ForthSystem = [if]
: >pos          \ n -- ; step to position n
  c# @ - spaces
;

: [o/n]         \ -- ; stop optimiser treating * DROP etc as no code
; immediate
[then]


\ ********************
\ SwiftForth20 harness
\ ********************

SwiftForth20 ForthSystem = [if]
: >pos          \ n -- ; step to position n
  get-xy drop - spaces
;

: [o/n]         \ -- ; stop optimiser treating * DROP etc as no code
  postpone noop
; immediate
[then]


\ ******************
\ Win32Forth harness
\ ******************

Win32Forth ForthSystem = [if]
: COUNTER       \ -- ms
  Call GetTickCount ;

: >pos          \ n -- ; step to position n
  getxy drop - spaces
;

: M/            \ d n1 -- quot
  fm/mod nip
;

: buffer:       \ n -- ; -- addr
  create
    here  over allot  swap erase
;

: 2-            \ n -- n-2
  2 -
;

: [o/n]         \ -- ; stop optimiser treating * DROP etc as no code
; immediate

: SendMessage   \ h m w l -- res
  swap 2swap swap               \ Win32Forth uses reverse order
  Call SendMessage
;

: GetTickCount  \ -- ms
  Call GetTickCount
;
[then]


\ ******************
\ BigForth harness
\ ******************

BigForth ForthSystem =
BigForth-Linux ForthSystem =  or
[if]

include ans.fs

Code u2/        \ n -- n/2
  1 # AX shr
  Next
end-code  macro

: COUNTER       \ -- ms
  timer@ >us &1000 um/mod nip ;

: >pos          \ n -- ; step to position n
  at? swap drop - spaces
;

: buffer:       \ n -- ; -- addr
  create
    here  over allot  swap erase
;

: [o/n]         \ -- ; stop optimiser treating * DROP etc as no code
; immediate

BigForth ForthSystem = [if]
also DOS
1 legacy !
4 User32 SendMessage SendMessageA       ( l w m h -- res )
0 kernel32 GetTickCount GetTickCount    ( -- ticks )
legacy on
previous

0 constant HWND_DESKTOP
16 constant WM_CLOSE
[else]
: GetTickCount  timer@ >us &1000 um/mod nip ;
: SendMessage  2drop 2drop 0 ;

0 constant HWND_DESKTOP
16 constant WM_CLOSE
[then]
[then]


\ ************************************
\ FORTH, Inc.  32 Bit Benchmark Source
\ ************************************

CELL NEGATE CONSTANT -CELL

CR .( Loading benchmark routines)


\ ***********************
\ Benchmark support words
\ ***********************

\ column positions
40 constant time-pos
50 constant iter-pos
60 constant each-pos

: .HEADER       \ -- ; display test header
  cr ." Test time including overhead"
  time-pos 3 + >pos  ." ms"
  iter-pos >pos ." times"
  each-pos >pos  ." ns (each)"
;

: TIMER ( ms iterations -- )
  >r                                    \ number of iterations
  counter swap -                        \ elapsed time in ms
  time-pos >pos  dup 5 .r
  iter-pos >pos  r@ .
  r@ 1 >
  if
    each-pos >pos
    1000000 r> */ 5 .r
  else
    drop  r> drop
  then
;

: .ann          \ -- ; banner announcment
  CR  ;

: [$            \ -- ms
  COUNTER ;

\ $]  is the suffix to a testing word.  It takes the fast ticks
\    timer value and calculates the elapsed time.  It does do
\    some display words before calculating the time, but it is
\    assumed that this will take minimal time to execute.

: $]            ( n -- )   TIMER ;

\ CARRAY  creates a byte size array.
: CARRAY ( n)   CREATE  ALLOT
                DOES> ( n - a)  + ;

\ ARRAY  creates a word size array.
: ARRAY ( n)    CREATE  CELLS ALLOT
                DOES> ( n - a) SWAP CELLS + ;


\ ****************************
\ Basic FORTH, Inc. Benchmarks
\ ****************************
\ This series of tests analyses the Forth primitives.

1000000 constant /prims
\ -- #iterations; all of these words return the number of iterations
: $DO$    .ann ." DO LOOP"  [$  /prims DUP 0 DO  I [o/n] DROP LOOP  $] ;
: $*$     .ann ." *"        [$  /prims DUP 0 DO  I I * [o/n] DROP  LOOP  $] ;
: $/$     .ann ." /"        [$  /prims DUP 1+ 1 DO  1000 I / [o/n] DROP LOOP  $] ;
: $+$     .ann ." +"        [$  /prims DUP 1+ 1 DO  1000 I + [o/n] DROP  LOOP  $] ;
: $M*$    .ann ." M*"       [$  /prims DUP    0 DO  I I M* [o/n] 2DROP  LOOP  $] ;
: $M/$    .ann ." M/"       [$  /prims DUP 1+ 1 DO  1000 0 I M/ [o/n] DROP  LOOP  $] ;
: $M+$    .ann ." M+"       [$  /prims DUP 1+ 1 DO  1000 0 I M+ [o/n] 2DROP  LOOP  $] ;
: $/MOD$  .ann ." /MOD"     [$  /prims DUP 1+ 1 DO  1000 I /MOD [o/n] 2DROP  LOOP  $] ;

\ $*/$  tests the math primitive  */ .  This may or may not tell
\    you how the other math primitives perform depending on
\    how  */  has been coded.
: $*/$    .ann ." */"       [$  /prims DUP 1+ 1 DO  I I I */ [o/n] DROP  LOOP  $] ;


\ ****************************************
\ Eratosthenes sieve benchmark program
\ This is NOT the original BYTE benchmark.
\ ****************************************

8190 CONSTANT SIZE
SIZE buffer: FLAGS

: DO-PRIME
      1000 0 DO
               FLAGS SIZE -1 FILL
               0 SIZE 0
               DO I FLAGS + C@
                    IF I 2* 3 + DUP I +
                         BEGIN DUP SIZE <
                         WHILE DUP FLAGS + 0 SWAP C! OVER +
                         REPEAT 2DROP
                              1+
                    THEN
               LOOP
               DROP
         LOOP
           ;

: $SIEVE$   .ann ." Eratosthenes sieve "  [$  DO-PRIME  SIZE 1000 *  ." 1899 Primes"  $] ;


\ *******************
\ Fibonacci recursion
\ *******************

34 constant /fib

: FIB ( n -- n' )
   DUP 1 > IF
      DUP 1- RECURSE  SWAP 2-  RECURSE  +
   THEN ;

: $FIB$
   .ann ." Fibonacci recursion ( "
   [$  /fib dup . ." -> " FIB dup . ." )" /fib - $] ;


\ *********************************
\ QuickSort from Hoare & Wil Baden
\ also contains the array fill test
\ *********************************

7 CELLS CONSTANT THRESHOLD

specifics [if]

PfwVfx ForthSystem = [if]
%macro Precedes ( n1 n2 -- f )
  u<
%endmacro
%macro Exchange ( a1 a2 -- )
  2dup @ swap @ rot ! swap !
%endmacro
[then]

Pfw22 ForthSystem = [if]
: Precedes  ( n n - f )    u< ;
: Exchange  ( a1 a2 -- )   2DUP  @ SWAP @ ROT !  SWAP ! ;
[then]

SwiftForth15 ForthSystem = [if]
: Precedes  ( n n - f )    u< ;
: Exchange  ( a1 a2 -- )   2DUP  @ SWAP @ ROT !  SWAP ! ;
[then]

SwiftForth20 ForthSystem = [if]
: Precedes  ( n n - f )    u< ;
: Exchange  ( a1 a2 -- )   2DUP  @ SWAP @ ROT !  SWAP ! ;
[then]

Win32Forth ForthSystem = [if]
: Precedes  ( n n - f )    u< ;
: Exchange  ( a1 a2 -- )   2DUP  @ SWAP @ ROT !  SWAP ! ;
[then]

BigForth ForthSystem =
BigForth-Linux ForthSystem = or [if]
: Precedes  ( n n - f )    u< ; macro
: Exchange  ( a1 a2 -- )   2DUP  @ SWAP @ ROT !  SWAP ! ; macro
[then]

[else]
: Precedes  ( n n - f )    u< ;
: Exchange  ( a1 a2 -- )   2DUP  @ SWAP @ ROT !  SWAP ! ;
[then]

: Both-Ends  ( f l pivot - f l )
    >R  BEGIN   OVER @ R@ precedes
        WHILE  CELL 0 D+   REPEAT
        BEGIN   R@ OVER @ precedes
        WHILE  CELL -      REPEAT   R> DROP ;

: Order3  ( f l - f l pivot)   2DUP OVER - 2/ -CELL AND + >R
      DUP @ R@ @ precedes IF DUP R@ Exchange THEN
      OVER @ R@ @ SWAP precedes
        IF OVER R@ Exchange  DUP @ R@ @ precedes
          IF DUP R@ Exchange THEN  THEN   R>  ;

: Partition  ( f l - f l' f' l)   Order3 @ >R  2DUP
      CELL -CELL D+  BEGIN    R@ Both-Ends 2DUP 1+ precedes
      IF  2DUP Exchange CELL -CELL D+  THEN
      2DUP SWAP precedes   UNTIL R> DROP SWAP ROT ;

: Sink  ( f key where - f)   ROT >R
   BEGIN   CELL - 2DUP @ precedes
   WHILE  DUP @ OVER CELL + !  DUP R@ =
        IF  ! R> EXIT THEN   ( key where)
   REPEAT  CELL + ! R> ;

: Insertion  ( f l)   2DUP precedes
    IF  CELL + OVER CELL +   DO  I @ I Sink  CELL +LOOP DROP
    ELSE  ( f l) 2DROP  THEN ;

: Hoarify  ( f l - ...)
    BEGIN   2DUP THRESHOLD 0 D+ precedes
    WHILE  Partition  2DUP - >R  2OVER - R> >  IF 2SWAP THEN
    REPEAT Insertion ;

: QUICK  ( f l)   DEPTH >R   BEGIN  Hoarify DEPTH R@ <
      UNTIL  R> DROP ;

: SORT  ( a n)   DUP 0= ABORT" Nothing to sort "
    1- CELLS  OVER +  QUICK ;

10000 constant /array
/array 1+ array pointers

: fillp         \ -- ; fill sort array once
  /array 0 ?DO  /array I -  I POINTERS !  LOOP ;

: $FILL$  .ann ." ARRAY fill"   [$  100 0 DO  fillp  LOOP  100 /array * $]  ;

: (sort)  100 0 DO   fillp  0 POINTERS 10000 SORT   LOOP ;

: $SORT$
  .ann ." Hoare's quick sort (reverse order)  "
  [$  (sort) 100 /array *  $] ;


\ *********************************
\ "Random" Numbers
\ *********************************

1024 constant /random

variable ShiftRegister
       1 ShiftRegister !

: RandBit       \ -- 0..1 ; Generates a "random" bit.
  ShiftRegister @ $00000001 and         \ Gen result bit for this time thru.
  dup 0<>                               \ Tap at position 31.
  ShiftRegister @ $00000008 and 0<>     \ Tap at position 28.
  xor 0<>                               \ If the XOR of the taps is non-zero...
  if
    $40000000                           \ ...shift in a "one" bit else...
  else
    $00000000                           \ ...shift in a "zero" bit.
  then
  ShiftRegister @ u2/                   \ Shift register one bit right.
  or                                    \ OR in new left-hand bit.
  ShiftRegister !                       \ Store new shift register value.
;

: RandBits      \ n -- 0..2^(n-1) ; Generate an n-bit "random" number.
  0                                     \ Result's start value.
  swap 0
  do
    2* RandBit or                       \ Generate next "random" bit.
  loop
;

: (randtest)    \ --
  1 ShiftRegister !
  /random 256 cells * allocate
  if
    cr ." Failed to allocate " /random . ." kb for test"
    abort
  then
  /random 256 * 0 do 32 RandBits over i cells + ! loop
  free drop
;

: $RAND$
  .ann ." Generate random numbers (" /random . ." kb array)"
  [$  (randtest)  /random 256 * $] ;


\ *********************************
\ LZ77 compression
\ *********************************

0       Value   lz77-buffer
0       Value   lz77-Pos
0       Value   lz77-BytesLeft

100 constant /lz77-size

: init-test-buffer
  /lz77-size 256 cells * to lz77-BytesLeft
  lz77-BytesLeft allocate
  if
    cr ." Failed to allocate " /lz77-size . ." kb for test"
    abort
  then
  dup to lz77-buffer to lz77-pos
  /lz77-size 256 * 0
  do  32 randbits lz77-buffer i cells + !  loop
;

: free-test-buffer
  lz77-buffer free drop
;

: getnextchar           \ -- char true | false
  lz77-BytesLeft dup
  if
    drop
    lz77-BytesLeft 1- to lz77-BytesLeft
    lz77-Pos dup 1+ to lz77-Pos
    c@
    true
  then
;

: lz77-read-file        \ addr len fileid -- u2 ior
  drop
  0 rot rot
  0 do                  \ done addr --
    getnextchar if
      over c! 1+ swap 1+ swap
    else
      leave
    then
  loop
  drop 0
;

: lz77-write-file       \ addr len fileid -- ior
  drop 2drop 0
;

: closed
  drop
;

: checked       \ flag --
  ABORT" File Access Error. " ;

: read-char     \ file -- char 
        drop getnextchar 0= if -1 then
;

(     LZSS -- A Data Compression Program )
(     89-04-06 Standard C by Haruhiko Okumura )
(     94-12-09 Standard Forth by Wil Baden )
(     Use, distribute, and modify this program freely. )

4096  CONSTANT    N     ( Size of Ring Buffer )
18    CONSTANT    F     ( Upper Limit for match-length )
2     CONSTANT    Threshold ( Encode string into position & length
                        ( if match-length is greater. )
N     CONSTANT    Nil   ( Index for Binary Search Tree Root )

VARIABLE    textsize    ( Text Size Counter )
VARIABLE    codesize    ( Code Size Counter )
\ VARIABLE  printcount  ( Counter for Reporting Progress )

( These are set by InsertNode procedure. )

VARIABLE    match-position
VARIABLE    match-length

N F + 1 -   carray text-buf   ( Ring buffer of size N, with extra
                  ( F-1 bytes to facilitate string comparison. )

( Left & Right Children and Parents -- Binary Search Trees )

N 1 +             array lson
N 257 +           array rson
N 1 +             array dad

specifics  PfwVfx ForthSystem = and [if]
  0 lson constant .lson   %cmacro lson  cells .lson +  %endmacro
  0 rson constant .rson   %cmacro rson  cells .rson +  %endmacro
  0 dad  constant .dad    %cmacro dad   cells .dad  +  %endmacro
[then]

( Input & Output Files )

0 VALUE     infile      0 VALUE     outfile

( For i = 0 to N - 1, rson[i] and lson[i] will be the right and
( left children of node i.  These nodes need not be initialized.
( Also, dad[i] is the parent of node i.  These are initialized to
( Nil = N, which stands for `not used.'
( For i = 0 to 255, rson[N + i + 1] is the root of the tree
( for strings that begin with character i.  These are initialized
( to Nil.  Note there are 256 trees. )

( Initialize trees. )

: InitTree                                ( -- )
      N 257 +  N 1 +  DO    Nil  I rson !    LOOP
      N  0  DO    Nil  I dad !    LOOP
;

( Insert string of length F, text_buf[r..r+F-1], into one of the
( trees of text_buf[r]'th tree and return the longest-match position
( and length via the global variables match-position and match-length.
( If match-length = F, then remove the old node in favor of the new
( one, because the old one will be deleted sooner.
( Note r plays double role, as tree node and position in buffer. )

: InsertNode                              ( r -- )

      Nil OVER lson !    Nil OVER rson !    0 match-length !
      DUP text-buf C@  N +  1 +                 ( r p)

      1                                         ( r p cmp)
      BEGIN                                     ( r p cmp)
            0< not IF                           ( r p)

                  DUP rson @ Nil = not IF
                        rson @
                  ELSE

                        2DUP rson !
                        SWAP dad !              ( )
                        EXIT

                  THEN
            ELSE                                ( r p)

                  DUP lson @ Nil = not IF
                        lson @
                  ELSE

                        2DUP lson !
                        SWAP dad !              ( )
                        EXIT

                  THEN
            THEN                                ( r p)

            0 F DUP 1 DO                        ( r p 0 F)

                  3 PICK I + text-buf C@        ( r p 0 F c)
                  3 PICK I + text-buf C@ -      ( r p 0 F diff)
                  ?DUP IF
                        NIP NIP I
                        LEAVE
                  THEN                          ( r p 0 F)

            LOOP                                ( r p cmp i)

            DUP match-length @ > IF

                  2 PICK match-position !
                  DUP match-length !
                  F < not

            ELSE
                  DROP FALSE
            THEN                                ( r p cmp flag)
      UNTIL                                     ( r p cmp)
      DROP                                      ( r p)

      2DUP dad @ SWAP dad !
      2DUP lson @ SWAP lson !
      2DUP rson @ SWAP rson !

      2DUP lson @ dad !
      2DUP rson @ dad !

      DUP dad @ rson @ OVER = IF
            TUCK dad @ rson !
      ELSE
            TUCK dad @ lson !
      THEN                                      ( p)

      dad Nil SWAP !    ( Remove p )            ( )
;

( Deletes node p from tree. )

: DeleteNode                              ( p -- )

      DUP dad @ Nil = IF    DROP EXIT    THEN   ( Not in tree. )

      ( CASE )                                  ( p)
            DUP rson @ Nil =
      IF
            DUP lson @
      ELSE
            DUP lson @ Nil =
      IF
            DUP rson @
      ELSE

            DUP lson @                          ( p q)

            DUP rson @ Nil = not IF

                  BEGIN
                        rson @
                        DUP rson @ Nil =
                  UNTIL

                  DUP lson @ OVER dad @ rson !
                  DUP dad @ OVER lson @ dad !

                  OVER lson @ OVER lson !
                  OVER lson @ dad OVER SWAP !
            THEN

            OVER rson @ OVER rson !
            OVER rson @ dad OVER SWAP !

      ( ESAC ) THEN THEN                        ( p q)

      OVER dad @ OVER dad !

      OVER DUP dad @ rson @ = IF
            OVER dad @ rson !
      ELSE
            OVER dad @ lson !
      THEN                                      ( p)

      dad Nil SWAP !                            ( )
;

      17 carray   code-buf

      VARIABLE    len
      VARIABLE    last-match-length
      VARIABLE    code-buf-ptr

      VARIABLE    mask

: Encode                                  ( -- )

      0 textsize !    0 codesize !

      InitTree    ( Initialize trees. )

      ( code_buf[1..16] saves eight units of code, and code_buf[0]
      ( works as eight flags, "1" representing that the unit is an
      ( unencoded letter in 1 byte, "0" a position-and-length pair
      ( in 2 bytes.  Thus, eight units require at most 16 bytes
      ( of code. )

      0 0 code-buf C!
      1 mask C!   1 code-buf-ptr !
      0    N F -                                ( s r)

      ( Clear the buffer with any character that will appear often. )

      0 text-buf  N F -  BL  FILL

      ( Read F bytes into the last F bytes of the buffer. )

      DUP text-buf F infile LZ77-READ-FILE checked   ( s r count)
      DUP len !    DUP textsize !
      0= IF    EXIT    THEN                     ( s r)

      ( Insert the F strings, each of which begins with one or more
      ( `space' characters.  Note the order in which these strings
      ( are inserted.  This way, degenerate trees will be less
      ( likely to occur. )

      F 1 + 1 DO                                ( s r)
            DUP I - InsertNode
      LOOP

      ( Finally, insert the whole string just read.  The
      ( global variables match-length and match-position are set. )

      DUP InsertNode

      BEGIN                                     ( s r)
\            key? drop
            ( match_length may be spuriously long at end of text. )
            match-length @ len @ > IF    len @ match-length !   THEN

            match-length @ Threshold > not IF

                  ( Not long enough match.  Send one byte. )
                  1 match-length !
                  ( `send one byte' flag )
                  mask C@ 0 code-buf C@ OR 0 code-buf C!
                  ( Send uncoded. )
                  DUP text-buf C@ code-buf-ptr @ code-buf C!
                  1 code-buf-ptr +!

            ELSE
                  ( Send position and length pair.
                  ( Note match-length > Threshold. )

                  match-position @  code-buf-ptr @ code-buf C!
                  1 code-buf-ptr +!

                  match-position @  8 RSHIFT  4 LSHIFT ( . . j)
                        match-length @  Threshold -  1 -  OR
                        code-buf-ptr @  code-buf C!  ( . .)
                  1 code-buf-ptr +!

            THEN

            ( Shift mask left one bit. )        ( . .)

            mask C@  2*  mask C!    mask C@ 0= IF

                  ( Send at most 8 units of code together. )

                  0 code-buf  code-buf-ptr @    ( . . a k)
                        outfile LZ77-WRITE-FILE checked ( . .)
                  code-buf-ptr @  codesize  +!
                  0 0 code-buf C!    1 code-buf-ptr !    1 mask C!

            THEN                                ( s r)

            match-length @ last-match-length !

            last-match-length @ DUP 0 DO        ( s r n)

                  infile read-char              ( s r n c)
                  DUP 0< IF   2DROP I LEAVE   THEN

                  ( Delete old strings and read new bytes. )

                  3 PICK DeleteNode
                  DUP 3 1 + PICK text-buf C!

                  ( If the position is near end of buffer, extend
                  ( the buffer to make string comparison easier. )

                  3 PICK F 1 - < IF             ( s r n c)
                        DUP 3 1 + PICK N + text-buf C!
                  THEN
                  DROP                          ( s r n)

                  ( Since this is a ring buffer, increment the
                  ( position modulo N. )

                  >R >R                         ( s)
                        1 +    N 1 - AND
                  R>                            ( s r)
                        1 +    N 1 - AND
                  R>                            ( s r n)

                  ( Register the string in text_buf[r..r+F-1]. )

                  OVER InsertNode

            LOOP                                ( s r i)
            DUP textsize +!

            \ textsize @  printcount @ > IF

            \     ( Report progress each time the textsize exceeds
            \     ( multiples of 1024. )
            \     textsize @ 12 .R
            \     1024 printcount +!

            \ THEN

            ( After the end of text, no need to read, but
            ( buffer may not be empty. )

            last-match-length @ SWAP ?DO        ( s r)

                  OVER DeleteNode

                  >R  1 +  N 1 - AND  R>
                  1 +  N 1 - AND

                  -1 len +!    len @ IF
                        DUP InsertNode
                  THEN
            LOOP

            len @ 0> not
      UNTIL                                     2DROP

      ( Send remaining code. )

      code-buf-ptr @ 1 > IF
            0 code-buf  code-buf-ptr @  outfile  LZ77-WRITE-FILE checked
            code-buf-ptr @ codesize +!
      THEN
;

: code77        \ --
  init-test-buffer
  encode
  free-test-buffer
;

: $CODE77$
  .ann ." LZ77 Comp. (" /lz77-size . ." kb Random Data Mem>Mem)"
  [$  code77  1 $] ;


\ *********************************
\ API Call OverHead
\ *********************************

HWND_DESKTOP VALUE hWnd

40000 constant /api1

: (api1)        \ -- ; SENDMESSAGE is probably the most used API function there is!
  hWnd WM_CLOSE 0 0 SendMessage drop
;

: $API1$        \ --
  .ann ." Win32 API: SendMessage"
  [$  /api1 0 do  (api1)  loop  /api1 $]
;

1000000 constant /api2

: $API2$        \ --
  .ann ." Win32 API: GetTickCount"
  [$  /api2 0 do  counter drop  loop  /api2 $]
;


\ *************************
\ The main benchmark driver
\ *************************

: BENCHMARK
   .ann ." This system's primitives" .specifics cr
   .header
   [$  
     $DO$
     $+$  $M+$
     $*$  $/$  $M*$  $M/$  $/MOD$  $*/$
     $FILL$  $API1$  $API2$
   CR  ." Total:"  1 $]
   CR
   .ann ." This system's application performance" .specifics CR
   .header
   [$  
     $SIEVE$  $FIB$  $SORT$  $RAND$  $CODE77$
   CR  ." Total:"  1 $]
;

BENCHMARK

CR CR .( To run the benchmark program again, type BENCHMARK )


