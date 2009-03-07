\ (ins (del                                            28dec04py

: at? 0 getpos-gen throw swap ;
: at  swap at-xy ;
: clrline  lf-gen ;

2Variable curpos

: save-cursor ( -- ) at? curpos 2! ;
: restore-cursor ( -- )  curpos 2@ at ;
: cur-correct ( addr u -- )  2drop ; \ !!!FIXME!!!
: .rest ( addr pos1 -- addr pos1 )
    key? ?EXIT
    restore-cursor 2dup type 2dup cur-correct ;
: .all ( span addr pos1 -- span addr pos1 )
    key? ?EXIT
    restore-cursor >r 2dup swap type 2dup swap cur-correct r> ;

: (del   ( m s addr pos1 -- m s addr pos2 ) 2 pick 0= ?exit
  at? >r >r  2dup 4 pick swap /string 1- 2dup
             over 1+ -rot move type space rot 1- -rot
  r> r> at ;
: cur+       >r at? r> + c/cols @ /mod swap >r + r> at ;
: >string  ( span addr pos1 -- span addr pos1 addr2 len )
    over 3 pick 2 pick chars /string ;
: <xcins>  ( max span addr pos1 xcchar -- max span addr pos2 )
    >r  2over r@ xc-size + u< IF  rdrop bell  EXIT  THEN
    >string over r@ xc-size + swap move 2dup chars + r@ swap xc!+ drop
    r> xc-size >r  rot r@ chars + -rot r> chars + ;
: <ins>  ( max span addr pos1 xcchar -- max span addr pos2 )
    <xcins> .all .rest ;
: <back>  ( max span addr pos1 -- max span addr pos2 f )
    dup  IF  over + xchar- over -  0 max .all .rest
    THEN 0 ;
: <forw>  ( max span addr pos1 -- max span addr pos2 f )
    2 pick over <> IF  over + xc@+ emit over -  THEN 0 ;
: (xcdel)  ( max span addr pos1 -- max span addr pos2 )
    over + dup xchar- tuck - >r over -
    >string over r@ + -rot move
    rot r> - -rot ;
: <bs> ( max span addr pos1 -- max span addr pos2 0 )
  dup  IF  over2 >r (xcdel) .all over2 r> swap - spaces .rest  THEN  0 ;
: <del> ( max span addr pos1 -- max span addr pos2 0 )
  2 pick over <>
    IF  <forw> drop <bs> EXIT  THEN 0 ;
\ : xceof  2 pick over or 0=  IF  bye  ELSE  <xcdel>  THEN ;

: <beg>  ( max span addr pos1 -- max span addr 0 0 )
  drop 0 .all .rest 0 ;
: <end>  ( max span addr pos1 -- max span addr span 0 )
  drop over .all 0 ;

: xcclear-line ( max span addr pos1 -- max addr )
    drop restore-cursor swap spaces restore-cursor ;
: <clr> ( max span addr pos -- max 0 addr 0 false )
    xcclear-line 0 tuck dup ;

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

: PCaccept   ( addr len -- len )  save-cursor
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
