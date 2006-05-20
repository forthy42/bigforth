\ debug in a window                                    20jun93py

[IFUNDEF] pipes
   include pipes.fb
\   ' import catch pipes "error off
\   [IF] include pipes.fb  [THEN]
[THEN]

pipes also forth

Module ttys

: tty  ( -- pipein pipeout )
  Pipe Pipe over <pipe! dup pipe>! -1 pdelim! ;
: tty! ( pipein pipeout -- )  <pipe! pipe>! -1 pdelim! ;

?head @ 1 ?head !

\ services                                             20jun93py

: "send ( addr n -- )  dup pemit ptype ;
: send ( x1 .. xn n -- ) dup >r
  1-  FOR  pad i@ + c!  NEXT  pad r> "send ;

: sat   ( addr n -- )  bounds ?DO  i c@  LOOP at ;
: sat?  ( -- )  at?  2 send ;
: sform ( -- )  form 2 send ;
: skey  ( -- )  key  $100 /mod 2 send ;
: skey? ( -- )  key? 1 send ;
: semit? ( -- ) emit? 1 send ;
also memory
: saccept ( addr 1 -- )  drop nexttib  >tib @ Hlock
  tib swap c@ accept dup #tib ! tib swap "send
  >tib @ Hunlock ;
toss
: seot? ( -- )  eot? 1 send ;

\ serve packets                                        20jun93py

Table: server
    type    semit?  cr      type
    del     page    sat     sat?
    sform   bot     eot     tflush
    curon   curoff  curleft currite clrline
    skey    skey?   saccept seot? [

: serve ( n -- ) sp@ cell+ >r
  pad 1- swap paccept 1- pad tuck 1- c@ cells
  server + perform r> sp! ;

\ ptty terminal emulation

[IFDEF] ~~ : ~~  output push display ~~ ; [THEN]
: dotty  ( -- )  !name
  >tib off $FF newtib #tibs moretibs
  BEGIN  pkey dup 0> WHILE  serve  peot? UNTIL
  ELSE  drop  THEN
  peot deltib ;

\ client side                                          10jul93py

User cstate

: service  ( count n -- ) swap 1+ pemit pemit ;
: service: ( n -- )  Create c,  DOES> c@ 0 swap service ;

: cextend ( c -- n )  dup $80 and 0<> -$80 and or ;

: recieve ( -- )  pkey  0 ?DO  pkey  LOOP ;

: (cat?   ( -- )  0 7 service  recieve
  swap cstate 2+ c!+ c! ;
: +out   ( n -- )
  cstate 3+ c@ + dup cstate 3+ c!
  cstate 1+ c@ >= IF  (cat?  THEN ;
: cemit  ( key -- ) 1 0 service pemit 1 +out ;
: cemit? ( -- flag )  0 1 service recieve cextend ;
: ccr    ( -- )  0 2 service (cat? ;
: ctype  ( addr n -- ) dup 3 service tuck ptype +out ;
 4 service: cdel
 5 service: cpage
: cat    ( y x -- )
  2 6 service  swap pemit pemit (cat? ;
: cat?   ( -- y x )  cstate 2+ c@+ c@ ;
: cform  ( -- y x )
  cstate w@ 0=
  IF  0 8 service  recieve  cstate 1+ c! cstate c!  THEN
  cstate c@+ c@ ;
$B service: cflush
$C service: ccuron
$D service: ccuroff
$E service: ccurleft
$F service: ccurrite
$10 service: cclrline
: ckey  ( -- n )    0 $11 service  recieve $100 * + ;
: ckey? ( -- flag ) 0 $12 service  recieve cextend ;
: caccept ( addr n -- n )
  1 $13 service pemit  pkey paccept ;
: ceot? ( -- flag ) 0 $14 service  recieve cextend ;
: ceot  ( -- )  peot  BEGIN pause peot?  UNTIL  standardi/o ;

\ open pipe window                                     13nov94py

MINOS also
: cbot ( -- )
    <pipe @ pipe> @ or ?EXIT ['] pause >r
    pushi/o tty 2 $1000 $1000 NewTask pass
    tty!
    openw
    cstate off  dotty
[IFDEF] go32
    term @ widget with dpy dispose endwith
[ELSE]
    term dpy dpy dispose
[THEN] ;
toss forth

\ ptty                                                 10jul93py

Output: pttyout
  cemit cemit? ccr ctype cdel cpage cat cat? cform
  cbot ceot cflush ccuron ccuroff ccurleft ccurrite cclrline [
Input: pttyin
  ckey ckey? PCdecode caccept ceot? [

?head !

: ptty  pttyout pause pttyin pause bot ;

tools also
export: drop ['] ptty IS debugi/o  export ttys ptty ;
toss

Module;

Onlyforth
