\ (ins (del                                            28dec04py

: at? 0 getpos-gen throw swap ;
: at  swap at-xy ;
: clrline  lf-gen ;

: (del   ( m s addr pos1 -- m s addr pos2 ) 2 pick 0= ?exit
  at? >r >r  2dup 4 pick swap /string 1- 2dup
             over 1+ -rot move type space rot 1- -rot
  r> r> at ;
: cur+       >r at? r> + c/cols @ /mod swap >r + r> at ;
: >string  ( span addr pos1 -- span addr pos1 addr2 len )
    over 3 pick 2 pick chars /string ;
: <ins>   ( m s addr pos1 char -- m s addr pos2 ) >r
  >string  2dup over 1+ swap move 1+
  r> 2 pick c!  tuck type 1- negate cur+ rot 1+ -rot 1+ ;
: curleft    -1 cur+ ;
: currite     1 cur+ ;

\ decode                                               06apr96py

: <forw>  dup 3 pick < IF currite 1+  THEN                0 ;
: <back>  dup          IF curleft 1-  THEN                0 ;
: <del>   dup 3 pick < IF (del        THEN                0 ;
: <bs>    dup IF curleft 1- (del THEN                     0 ;
: <beg>   negate cur+ 0                                   0 ;
: <end>   >r over dup r> - cur+                           0 ;
: <clr>   negate cur+ over spaces swap negate cur+ 0 tuck 0 ;
Create ctrlkeys
' false , ' <beg> , ' <back> , ' false ,
' <del> , ' <end> , ' <forw> , ' false ,
' <bs> , ' false , ' true , ' <clr> ,
' false , ' true , ' false , ' false ,
' false , ' false , ' false , ' false ,
' false , ' false , ' false , ' false ,
' false , ' false , ' false , ' false ,
' false , ' false , ' false , ' false ,

\ decode                                               28dec04py

Variable lastkey

: ctype? ( key -- char type )  dup lastkey !
  dup $7F =  IF  drop $08  THEN  dup bl >= ;

Defer everychar ' noop IS everychar

\ accept keyboard                                      05apr96py

Defer decode

: PCdecode ( max span addr pos1 key -- max span addr pos2 flag )
  everychar  ctype?
  IF    >r 2over = IF  rdrop bell 0 exit  THEN  r> <ins> false
  ELSE  cells ctrlkeys + perform  THEN ;

' PCdecode IS decode

: PCaccept   ( addr len -- len )
  dup 0< IF abs over dup 1- c@ tuck type ELSE 0 THEN rot over
  BEGIN  key decode  UNTIL
  nip over - negate cur+ nip space ;

\ vt100 key interpreter                                30jun98py

Create translate $100 allot
translate $100 erase
Create transcode $100 allot
transcode $100 erase

Variable fcode

: trans  ( char index -- ) translate + c! ;
: tcode  ( char index -- ) transcode + c! ;

: vt100-decode ( max span addr pos1 -- max span addr pos2 flag )
    key '[' = IF  0  base @ >r decimal
        BEGIN  key dup digit?  WHILE  nip swap #10 * +  REPEAT
        r> base !
        dup '~' =  IF  drop transcode  ELSE  nip translate  THEN
        over fcode ! + c@ dup  IF  decode  THEN
    ELSE  0  THEN ;

ctrl B 'D' trans
ctrl F 'C' trans
ctrl P 'A' trans
ctrl N 'B' trans

ctrl A 1 tcode
ctrl D 3 tcode
ctrl E 4 tcode

' vt100-decode  ctrlkeys $1B cells + !
