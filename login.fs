(*              *** Multiuser bigFORTH ***             13aug92py

bigFORTH is a multiuser environment. The next few lines serve
as prove. LOGIN <name> logs a new user in. A module with this
name is created. There is no password requested. CONNECT opens
a new terminal, in this case a window (with OPENW).

*)

\ Multiuser bigFORTH                                   13aug92py

defer connect    \ open a terminal
defer disconnect \ close connection

Variable logins  1 logins !

: login ( -- )  bl parse
  dup 0= IF ." login: " 2drop pad dup $1F accept THEN
  errorhandler @ 3 Rspace mroot $14 + @ over - NewTask pass
  -rot  pad place  pad count
  Mroot @ dup thisModule ! dup @ + dp !
  >tib off  cols &80 max newtib #tibs 1- moretibs
  ">tib forth joined Module
  [ root ] lastcfa @ IS Forth [ Forth ]
  rdrop deltib onlyforth  ['] (quit Is 'quit
  connect ( standardi/o page ) errorhandler !  1 logins +!
  cr  FORTHstart 2+ count  cols over - 2/ spaces  type cr quit ;

: bye ( -- )  -1 logins +!
  logins @ 0=  IF  bye  THEN
  up@ mroot @ <
  IF  disconnect  BEGIN  stop  AGAIN  THEN
  thismodule @ rm-module  disconnect r0 @ rp! ;

[IFDEF] MINOS
MINOS also Forth
' openw IS connect
: winend
    BEGIN  >tib @  WHILE  deltib  REPEAT
[IFDEF] go32
    term @ widget with dpy dispose endwith
[ELSE]
    term dpy dpy dispose
[THEN] ;
' winend  IS disconnect
Onlyforth
