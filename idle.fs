\ "idle" task handling                                 21dec97py

Variable timeout

\ | ' noop Alias linux-new

[IFDEF] win32
Variable app-timer
Variable timer-win
[THEN]

User waitfile   waitfile on

: -min ( n1 n2 -- n3 )  2dup - 0> IF nip EXIT  THEN  drop ;

[IFDEF] linux-new
Variable idle-task
Variable is-idle   is-idle off
[THEN]

: select-file ( file timeout -- )
    after dup 0= - dup wake-time ! timeout @ -min timeout !
    waitfile !
[IFDEF] linux-new
    is-idle @  IF  is-idle off idle-task @ wake  THEN
[THEN]
    stop  wake-time off waitfile on ;

[IFDEF] unix
Create fds here $400 8 / dup allot erase

also DOS also

Create >timeout 0 , 0 ,
: unix-wait ( n -- ) >us &1000000 um/mod
    ( swap  &9980 + &10000 / &10000 * swap ) >timeout 2!
    >timeout 0 0 fds [ $400 8 / ] Literal
    0 -skip 3 << select drop ;

[IFDEF] linux-new
&14 Constant SIGALRM
Create itimerval 4 cells allot

Code sigalarm ( n -- ) R: SI push  UP push
     'up A#) UP mov  -$100 SP D) SI lea ;c:
     recursive ['] sigalarm SIGALRM signal drop
     is-idle @ IF  is-idle off  idle-task @ wake  THEN
     >c: R: UP pop  SI pop  Next end-code
[THEN]

previous previous
[THEN]

[IFDEF] win32
also DOS
4 user32 SetTimer SetTimer ( func elapse ide wnd -- n )
2 user32 KillTimer KillTimer ( ide wnd -- r )
0 user32 WaitMessage WaitMessage ( -- r )
5 user32 PeekMessage PeekMessageW ( remove fmax fmin wnd msg -- 
        flag )
previous

Create event &28 allot

&20 Value time-slice

: app-timer! ( -- )
  app-timer @ ?EXIT
  app-win @ timer-win !
  0 time-slice timer-win @ dup 0<> negate swap
  SetTimer app-timer ! ;

: re-time ( win -- )  app-win push
  timer-win @ 0<> negate app-timer @ KillTimer drop
  app-win ! app-timer off  app-timer! ;

: windows-wait ( n -- )  drop
  app-timer!  app-timer @ 0= ?EXIT
  WaitMessage
  IF  BEGIN  1 &275 dup timer-win @ event PeekMessage
             0= UNTIL  THEN ;
\  IF  &275 dup timer-win @ event GetMessage drop  THEN ;
[THEN]

[IFDEF] unix
: idles ( -- idle n )
    &20 ms>time unix-wait
    timer@ 1 unix-wait timer@ swap - 5 ms>time <
    IF
        timer@ 1 unix-wait timer@ swap - 1 ms>time 2/ >
        IF  1 1  ELSE  9 2  THEN
    ELSE
        timer@ 1 unix-wait timer@ swap - &15 ms>time <
        IF  &10  ELSE  &20  THEN  &10 
    THEN ; idles
[ELSE] &20 &10 [THEN]
dup Value idle+
+ Value idle'

: min-wait ( -- n )
    idle' idle+ - after >r
    sleepers @ dup sleepers = IF  rdrop drop 0 EXIT  THEN
    $7FFFFFFF
    BEGIN
        over task's wake-time @
        ?dup
        IF  r@ - dup 0<
            IF    drop swap dup @ swap wake
            ELSE  min swap @  THEN
        ELSE  swap @  THEN
        tuck
        sleepers =  UNTIL
    nip rdrop ;

[IFDEF] unix
: ?select ( -- )
    fds $20 0 skip nip 0= ?EXIT
    sleepers @
    MEANWHILE
        pause
        dup task's waitfile @ fds over bit@ swap 0>= and
        IF    dup @ swap wake
        ELSE  @  THEN
    THEN
        dup sleepers =
    UNTIL drop ;

: !select ( -- )
    fds $20 erase
    sleepers @
    MEANWHILE
\        pause \ calling pause here gives a race condition!!!
        dup task's waitfile @ dup 0>= IF  fds over +bit  THEN
        drop @
    THEN
        dup sleepers =
    UNTIL  drop ;
[THEN]

: idling ( -- )
[IFDEF] linux-new
    is-idle off
[THEN]
    BEGIN
        min-wait 
        dup $7FFFFFFF <> over 0> and
        IF    up@ dup @ <>
              IF  drop 0  THEN
        ELSE  drop 0 THEN
[IFDEF] unix
[IFDEF] linux-new
        dup 0= IF  is-idle on  stop  THEN
[THEN]
        ?dup IF  !select unix-wait ?select  THEN
[THEN]
[IFDEF] win32
        ?dup IF  windows-wait  THEN
[THEN]
        &50 0 DO pause LOOP
        clearstack
    AGAIN ;

: idler ( -- )
[IFDEF] linux-new
   is-idle off
   idles dup TO idle+  + TO idle'
[THEN]
[IFDEF] unix
    $400 dup
[ELSE]
    $400 dup  \ $100000 $2000
[THEN]
    NewTask activate !name
[IFDEF] linux-new [ also DOS ]
    up@ idle-task !
    ['] sigalarm SIGALRM signal drop
    &10000 0 2dup itimerval 2! itimerval 2 cells + 2!
    0 0 itimerval setitimer drop
    [ previous ]
[THEN]
    BEGIN  ['] idling catch drop  AGAIN ;

idler

' select-file IS idle
