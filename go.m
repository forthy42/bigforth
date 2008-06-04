#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

menu-window class go
public:
  early widget
  early open
  early all-open
  early modal-open
  infotextfield ptr white
  infotextfield ptr black
  infotextfield ptr white#
  infotextfield ptr black#
  infotextfield ptr handicap
  text-label ptr comment
  canvas ptr go-field
 ( [varstart] ) 19 dup * var field
$100 var fpad
cell var goin
cell var goout
cell var taskid
cell var done
method go-task
method new-game
method close-game
method go-write ( [varend] ) 
how:
  : open       screen self new >o show o> ;
  : all-open   screen self new >o show up@ app ! o> ;
  : modal-open screen self new >o show stop o> ;
class;

component class file-menu
public:
  early widget
  early open
  early dialog
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" No Title" open-component ;
  : dialog   new DF[ 0 ]DF s" No Title" open-dialog ;
  : open-app new DF[ 0 ]DF s" No Title" open-application ;
class;

component class help-menu
public:
  early widget
  early open
  early dialog
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" No Title" open-component ;
  : dialog   new DF[ 0 ]DF s" No Title" open-dialog ;
  : open-app new DF[ 0 ]DF s" No Title" open-application ;
class;

component class go-game-menu
public:
  early widget
  early open
  early dialog
  early open-app
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" No Title" open-component ;
  : dialog   new DF[ 0 ]DF s" No Title" open-dialog ;
  : open-app new DF[ 0 ]DF s" No Title" open-application ;
class;

go-game-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[ s" pass" go go-write ]S ( MINOS ) S" Pass" menu-entry new 
        ^^ S[ s" resign" go go-write ]S ( MINOS ) S" Resign" menu-entry new 
      &2 vabox new &2 borderbox
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

help-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[  ]S ( MINOS ) S" About" menu-entry new 
        ^^ S[  ]S ( MINOS ) S" Contents" menu-entry new 
      &2 vabox new &2 borderbox
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

file-menu implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
        ^^ S[ [ also dos ]
  go close-game
  go new-game
[ previous ] ]S ( MINOS ) S" New Game" menu-entry new 
        ^^ S[ go close ]S ( MINOS ) S" Exit" menu-entry new 
      &2 vabox new &2 borderbox
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

go implements
 ( [methodstart] ) also DOS
: go-write ( addr u -- )
  ( 2dup type cr ) goin @ write-line throw ;
: go-line ( addr u field -- )
  >r 4 /string r> 19 bounds ?DO
    over c@ 0 swap
    CASE  'O OF  1+  ENDOF
          'X OF  1-  ENDOF
    ENDCASE  >r over 1+ c@ r> swap ') = IF 2* THEN  I c!
    2 /string  LOOP  2drop ;
: prefix? ( addr1 u1 addr2 u2 -- flag )
  tuck 2>r min 2r> compare 0= ;
: eval-line ( addr u -- )  \ dup IF  2dup type cr  THEN
  2dup s" Game over." prefix?  IF  1 done !  EXIT  THEN
  2dup s" White("  prefix? IF  comment assign  EXIT  THEN
  2dup s" white("  prefix? IF  comment assign  EXIT  THEN
  2dup s" Black("  prefix? IF  comment assign  EXIT  THEN
  2dup s" black("  prefix? IF  comment assign  EXIT  THEN
  2dup s" GNU Go"  prefix? IF  comment assign  EXIT  THEN
  2dup s" Result:" prefix? IF  comment assign  EXIT  THEN
  2dup s"     White has captured " prefix?
  IF 23 /string 0. 2swap >number 2drop white assign EXIT THEN
  2dup s"     Black has captured " prefix?
  IF 23 /string 0. 2swap >number 2drop black assign EXIT THEN
  2dup s"     aprox. White territory: " prefix?
  IF 28 /string 0. 2swap >number 2drop white# assign EXIT THEN
  2dup s"     Estimated score: White " prefix?
  IF 39 /string 0. 2swap >number 2drop white# assign
                                    0. black# assign EXIT THEN
  2dup s"     aprox. Black territory: " prefix?
  IF 28 /string 0. 2swap >number 2drop black# assign EXIT THEN
  2dup s"     Estimated score: Black " prefix?
  IF 39 /string 0. 2swap >number 2drop black# assign 
                                    0. white# assign EXIT THEN
  over 3 bl skip 0. 2swap >number 0=
  IF 2drop dup >r 19 swap - 19 * field + go-line
     r> 1 = IF  go-field draw done @ negate done !  THEN
     EXIT  THEN
  drop 2drop 2drop ;
: close-game
  goin  @ IF  goin  @ close-file throw
              s" /tmp/goin" delete-file throw  THEN
  goout @ IF  goout @ close-file throw
              s" /tmp/goout" delete-file throw  THEN
  goin off  goout off ;
: go-task ( -- )  done off  taskid @ ?dup IF  kill  THEN
  ^ 1 $1000 dup NewTask dup taskid ! pass op! decimal
  BEGIN  $100 0 DO  BEGIN  fpad I + 1 goout @ read-file
                    WHILE  drop goout @ filehandle @ &5000 idle
                    REPEAT  drop fpad I + c@ #lf <> WHILE
         LOOP  fpad $100  ELSE  fpad I  UNLOOP  THEN
  eval-line done @ 0< UNTIL close-game taskid off ;
Variable go$
: new-game base push decimal
  go field 19 dup * erase go-field draw
  0" mknod /tmp/goin p" dup >len type cr system drop
  0" mknod /tmp/goout p" dup >len type cr system drop
  s" gnugo --mode ascii --handicap " go$ $!
  handicap get tuck dabs <# #S #> go$ $+!
  0< IF s"  --color white" go$ $+! THEN
  s"  </tmp/goin >/tmp/goout 2>/dev/null & " go$ $+!
  go$ $@ 2dup + 1- 0 swap c! drop dup >len type cr system drop
  s" /tmp/goin" w/o open-file throw goin !
  s" /tmp/goout" r/o nonblock open-file throw goout !
  go-task
  s" score" go-write ;
: close  close-game super close ;
previous ( [methodend] ) 
  : widget  ( [dumpstart] )
        M: file-menu widget S"  File " menu-title new 
        M: go-game-menu widget S"  Game " menu-title new 
        $1 $1 *hfilll $1 $1 *vfilll rule new 
        M: help-menu widget S"  Help " menu-title new 
      &4 hbox new vfixbox  &2 borderbox
            ^^ &0. ]N ( MINOS ) SN[  ]SN ( MINOS ) S" Captured: White" infotextfield new  ^^bind white
            ^^ &0. ]N ( MINOS ) SN[  ]SN ( MINOS ) S" Black" infotextfield new  ^^bind black
          &2 hatab new &1 hskips
            ^^ &0. ]N ( MINOS ) SN[  ]SN ( MINOS ) S" Territory: White" infotextfield new  ^^bind white#
            ^^ &0. ]N ( MINOS ) SN[  ]SN ( MINOS ) S" Black" infotextfield new  ^^bind black#
          &2 hatab new &1 hskips
            ^^ &0. ]N ( MINOS ) SN[  ]SN ( MINOS ) S" Handicap" infotextfield new  ^^bind handicap
            ^^ S[ s" pass" go-write ]S ( MINOS ) S" Pass" button new 
            ^^ S[ s" back" go-write
s" back" go-write ]S ( MINOS ) S" Back" button new 
          &3 habox new vfixbox  &1 hskips
          S" " text-label new  ^^bind comment
        &4 vabox new vfixbox  panel
          CV[ 38 dup steps $C0 dup dup rgb> backcolor clear
1 1 home! path
9 0 DO  0 -36 to 2 0 to 0 36 to 2 0 to  LOOP
0 -36 to
9 0 DO  -36 0 to 0 2 to 36 0 to 0 2 to  LOOP
-36 0 to stroke up 1 1 textpos 6 -30 to
3 0 DO
    3 0 DO  icon" icons/black-marker" icon  12 0 to  LOOP
    -36 12 to  LOOP  -6 -6 to
dpy with field endwith
19 dup * bounds ?DO
   I 19 bounds ?DO  I cx@
      CASE  1 OF  icon" icons/white-dot"  icon  ENDOF 
            2 OF  icon" icons/white-last" icon  ENDOF 
           -1 OF  icon" icons/black-dot"  icon  ENDOF 
           -2 OF  icon" icons/black-last" icon  ENDOF 
      ENDCASE
      2 0 to  LOOP
   -38 -2 to 19 +LOOP
down ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) dup 2 <> IF  2drop 2drop  EXIT  THEN
base push decimal
taskid @ 0= IF  new-game  THEN
2drop go-field xywh 2>r p- 2r> rot swap
19 swap */ >r 19 swap */ r>
19 swap - 0 <# #S rot 'a + dup 'i >= IF  1+  THEN  hold #>
goin @ IF  2dup go-write
THEN  2drop ]CK ( MINOS ) $130 $1 *hfil $130 $1 *vfil canvas new  ^^bind go-field
        &1 habox new
      &2 vabox new
    ( [dumpend] ) ;
  : title$  s" Go" ;
  : init  super init  ^ to ^^
    widget 1 0 modal new  title$ assign ;
class;

: main
  go all-open
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
