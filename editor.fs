\                 *** Screen Editor ***                19may97py

\ This file contains the editor for screen files

\ Load Screen for the Editor                           20may00py

[defined] arguments 0= [IF]
    : arguments depth 1- > abort" arguments?!" ;
[THEN]
[defined] vfxforth 0= [IF]
From edit.fs       From ediwind.fs
    Module Editor
[ELSE]
    Vocabulary editor
    also editor definitions
[THEN]
[defined] win32api [IF] win32api also  [THEN]
[defined] X11      [IF] X11 also       [THEN]
MINOS also Dos also Memory also Editor

include ediwind.fs   \ Fenster
\ warning @ warning off
\ warning !

\ Editor Variable                                      21apr97py

[defined] vfxforth 0= [IF]
    ?head @         1 ?head !
[THEN]

Variable minvert#               AVariable "done
Variable loading                Variable callwind

Variable jingle   jingle on
Variable ?hide    ?hide off
: alarm   [defined] con! [IF] 7 con! [THEN]
    jingle off ;

: blank    ( addr count --)     bl fill ;

forward edi_open
forward replace-it

\ Move the Editor's cursor around                      26jun94py
: top          ( -- )       0. scredit at ;
: col          ( -- n )     cur [ c/l 1- ] Literal and ;
: row          ( -- n )     cur 6 rshift ;
: 'cursor      ( -- addr )  scredit 'start cur + ;
: 'line        ( -- addr )  'cursor col -  ;
: #after       ( -- n )     c/l col -  ;
: #remaining   ( -- n )     b/blk cur    - ;
: #end         ( -- n )     #remaining col +  ;
: +tab         ( -- )       $10 cur $F and - c ;
: -tab         ( -- )       cur 7 and negate dup 0=  8 * +  c ;
: ecr          ( -- )       #after c ;
: scr@         ( -- addr len ) scredit 'start b/blk ;
: >""end       ( -- )       scr@ 1- -trailing nip  pos ! ;
: word@        ( addr count skip -- addr' count' )  swap >r
  -trailing 2dup + r> 2 pick - tuck bl scan nip - >r + r> ;

\ buffers                                              02oct94py

: modified   ( -- )             'start drop  update ;

#42 Constant c/buf
\ "Es wird euch nicht gefallen, aber das Ergebnis ist 42"

Variable insbuf
: 'insert   ( -- ins-buf )      insbuf @ ;
: 'find     ( -- find-buf )     'insert c/buf + ;
: 'find+    ( n1 -- n2 )        'find c@ +  ;

: !scr  scredit edifile @ !files
  scredit scr# @  scr !  cur r# ! ;

\ Errorchecking                                        11apr94py

: 'lastline ( -- addr len )  scr@ + c/l -  c/l ;
: 'line@    ( -- addr len )  'line c/l ;

: ?bottom  ( -- )     'lastline  -trailing nip
                      abort" You would lose a line" ;

: (end?               abort" You would lose a char" ;
: ?end  ( -- )        'line c/l + 1- c@  bl - (end? ;

: ?range  ( n -- n )   dup  capacity 0 within
                          abort" Out of range!" ;

\ Graphics for display                                 25jun94py

: lineinsert   ( line# -- )  drop scredit draw ;
\   org @ - cheight * >r
\   inscalk r@ - cheight - rot r> + -rot
\   2over cheight +  scr>scr ;

: linedelete   ( line# -- )  drop scredit draw ;
\   org @ - cheight * >r
\   inscalk r@ - cheight - rot r> + cheight + -rot
\   2over cheight -  scr>scr ;

: lastlineoff  ( -- )
   'lastline blank  update   l/s 1- scredit .line ;
\   wi_h org @ + 1- .line   ;

\ move part of the line by one char                    19may97py

: char>   row scredit .line ;
\  curpos c/l  col 1+ - cwidth * cheight
\  2over swap cwidth + swap scr>scr  ;

: <char   row scredit .line ;
\  curpos swap cwidth + swap
\  c/l col 1+ - cwidth * cheight
\  curpos scr>scr
\  b? dup push off cur >r
\  row c/l 1- at space GEMflush r> pos ! ;

\ screen display                                       21apr97py

#18 Constant id-len  Create id   id-len allot   id id-len erase
: stamp       id  1+ count scr@ drop c/l +  over -  swap move ;
: ?stamp      updated? scr@ -trailing nip 0<> and
              IF  stamp  THEN  ;
: edilist     ( edistate off )
              scr @ scredit scr# !  scredit slided ;
: retallot    scredit start @
              0= IF b/blk scredit start Handle!  THEN ;
: undo        retallot updated? 0=
              IF  scredit retscr @ scr @ =
                  IF  scredit start @ scr@ move update
                      edilist scredit retscr on THEN  EXIT  THEN
              scr@ scredit start @ swap move
              purgebuf  edilist scr @ scredit retscr ! ;

\ more-alert cancel-alert                              12oct97py

: more-alert ( -- )
[defined] VFXForth [ 0= ] [IF]
  r> r> ^ 3 $3000 $2000 NewTask pass  >o rdrop edicatch  >r >r
[THEN]
  s" Add a screen?" 1
  s"  No " s"  Yes "  2 1 alert ;

: cancel-alert ( -- )
[defined] VFXForth [ 0= ] [IF]
  r> r> ^ 3 $3000 $2000 NewTask pass  >o rdrop edicatch  >r >r
[THEN]
  s" All datas are lost!" 1
  s"  Yes "  s"  No " 2 2 alert ;

\ more?                                                31aug97py
Variable next-slide
: draw-edislide  timer@ next-slide @ - $-1000 0 within ?EXIT
  #50 after next-slide !   scredit dpy self
  viewport with hspos draw endwith ;
: onemore ( -- )  1 more draw-edislide ;
: more? isfile@ 0= ?EXIT more-alert  1 = IF  onemore  THEN  ;
: nofile? isfile@ 0= abort" Not for direct access!" ;
: delete?  scr@ -trailing nip 0= ?EXIT
  cancel-alert 1 = IF  rdrop  THEN ;
: (clrscr scr @ buffer b/blk blank update edilist ;
: clrscr updated?  IF  delete?  THEN  (clrscr ;
: insscr nofile? ?stamp capacity 1- block b/blk -trailing nip
  IF  onemore  THEN  scr @ capacity 2- over 1+ convey (clrscr ;
: delscr nofile? delete? scr @ 1+ capacity 1- over 1- convey
  capacity 1- block b/blk blank update edilist ;

\ Edi Variables,                                       11oct93py

\ : memtop  ( -- addr )   thisModule @ dup cell+ @ + ;
\ : membot  ( -- addr )   'find c/buf + c/l 2* + ;

Variable chars                  Variable #chars
: 'chars  ( -- addr )   chars @  #chars @ + ;

Variable (key

Variable imode  imode on

\ Edi line handling                                    02jul94py

: linemodified   modified  row scredit .line ;

: clrline        'line c/l blank        linemodified ;
: clrright       'cursor #after blank   linemodified ;

: delline        'line #end c/l delete
                 row linedelete lastlineoff   modified ;
: backline       c/l negate c  delline ;

: instline       ?bottom   'line c/l over #end insert
                 row lineinsert clrline ;

\ Edi line handling                                    05jul95py

: -line         'line c/l -trailing ;

forward @line
forward ?line
forward !line"

: copyline      -line @line c/l c ;
: line>buf      -line @line delline ;

: !line         !line" 'line swap c/l min move linemodified ;
: buf>line      ?line  ?bottom  instline  !line ;

\ Edi char handling                                    07jan07py
: delchar      'cursor #after cursize delete  <char modified ;
: backspace col 0= IF  -line >r c/l negate c -line dup pos +!
                       dup r@ + c/l > (end? + r> move
                       linemodified c/l c delline c/l negate c
                 ELSE  curleft  delchar THEN ;
: instX        ?end   'cursor swap over #after insert ;
: inst1        1 instX ;
: instchar     inst1  char> bl 'cursor c! 0 c modified ;
: (@char       #chars @ $100 u>= abort" char buffer full"
               'chars c! 1 #chars +! ;
: copychar     'cursor c@  (@char 1 c ;
: char>buf     'cursor c@  (@char delchar ;
: !char        -1 #chars +!  'chars c@ 'cursor c! 1 c ;
: ?chars       #chars @ 0= abort" char buffer empty" ;
: buf>char     ?chars inst1  char> !char -1 c  modified ;

\ from Screen to Screen ...                            21apr97py
: setscreen  ( n -- )  ?stamp ?range scr ! edilist ;
: n    scr @  1+  dup capacity =  IF  more?  THEN  setscreen ;
: b    scr @  1-  setscreen ;
: w           scr @ capacity scredit >shadow  setscreen ;
: mark!       scredit 'r# ! scredit 'scr ! scredit 'edifile ! ;
: (mark       isfile@  scr @  cur  mark!
(  scredit shadow @ ?dup
  IF  >o isfile@ scredit scr# @ cur mark! o>  THEN ) ;
: mark        (mark  true abort" marked !" ;
: a           ?stamp   scredit 'edifile @
  dup  IF   dup searchfile drop  THEN
  scredit 'r# @  scredit 'scr @ (mark  rot dup scredit edifile !
  !files  ?range scr !  pos !
  edilist ;

\ splitting a line, replace                            21apr97py

: split          ?bottom  scratch c/l 2dup blank
   'cursor #remaining insert   linemodified
   col   ecr  row lineinsert
   'cursor c/l  rot  delete  linemodified ;
: lfsplit        ?bottom  scratch c/l 2dup blank
   'cursor #remaining insert   linemodified
   c/l c  row lineinsert  linemodified ;
: ins      'insert count dup 0= IF 2drop EXIT THEN
  tuck 'cursor #after  insert  c ;
: ?room  'insert c@ 'find c@ - < abort" not enough room" ;
: r   c/l   'line over -trailing  nip  -  ?room
   'find c@  dup negate c  'cursor #after rot  delete  ins
   'insert c@ dup negate c linemodified row swap c row -
   IF linemodified THEN ;

\ exiting the Editor                                   30oct99py

Create comport  0 , 0 , 0 ,
: array!  1- cells over + DO  I !  -cell +LOOP ;
: array@  cells bounds ?DO  I @  cell +LOOP ;
[defined] VFXForth [IF]
    : communicate ( x1 .. xn n cfa -- ) drop
	0 ?DO drop LOOP ;
[ELSE]
: communicate ( x1 .. xn n cfa -- ) >r comport swap array!
  (Ftast @ r> (Ftast ! >r  $FFBE 0 scredit callwind keyed
    BEGIN  pause comport @ 0<  UNTIL  r> (Ftast ! ;
[THEN]
: ."done ( -- )  scr @ dup 0<
  IF  invert ." Line #" 4  ELSE  ." Scr #" 3  THEN
  over scr !  .r  2 spaces  "done @ count type  space ;
: !filepos  comport 3 array@ r# ! scr ! !files
  comport on pause  ."done
  loading @ 0= ?EXIT  scr @ r# @
  isfile@ str?  IF  (#load  ELSE  (load  THEN ;

\ leave the editor                                     30oct99py
: scr>  scredit 'edifile @ 'edifile0 !
        scredit 'scr     @ 'scr0 !
        scredit 'r#      @ 'r#0 ! ;
: done ( ff addr -- )  do-done on  "done !  loading !
  scr>  scredit edifile @ scr @ r# @
  3 ['] !filepos communicate ( scredit close ) 2 throw ;
: +done ( ff addr -- )  cur r# ! done ;
: cdone   ( -- tf ) cancel-alert  1 = ?EXIT 'start drop
                    purgebuf   false c" canceled" +done ;
: sdone   ( -- tf ) ?stamp save-buffers  false c" saved" +done ;
: xdone   ( -- tf ) ?stamp  scredit update$  scratch place
  false scratch  +done ;
: ldone   ( -- tf ) ?stamp save-buffers true c" loading" +done ;
: edibye  ( -- tf ) ['] sdone catch
  [defined] VFXForth [ 0= ] [IF]
  #100 wait singletask pccuron bye [THEN] ;

\ get User's ID                                        15jul00py

: cancel  widget dpy close  edit-o @ op! ;
: set-id  textfield get id-len min id 1+ place  cancel ;
: clr-id  0 id 1+ c!  cancel ;
window ptr current-win
: ?set-parent ( -- )  [defined] x11 [IF]  edit-o @ IF edit-o @
  scredit with dpy get-win endwith window set-parent  THEN
[THEN] ;
: MODAL: ( addr len -- )
  ^ edit-o ! r> screen self window new window with
  execute  >r modal new  panel  dup >r  -rot assign
  ?set-parent mousemap
  r> r> swap modal with ?dup IF  modal bind active  THEN
  endwith ^ F bind current-win endwith ;
[defined] DoNotSin [IF] DoNotSin [THEN]

\ get id                                               04jun08py

: do_getid  ( -- )  S" Enter your ID" MODAL:
  id 1+ count 0 ST[ ]ST s" ID:" infotextfield new
  2fill  over   S[ set-id ]S s" OK"     button new  dup >r
  2skip  3 pick S[ clr-id ]S s" No ID"  button new
  2skip  5 pick S[ cancel ]S s" Cancel" button new
  2fill  7 hatbox new  2 r> 0 ;
[defined] DoNotSin [IF] DoNotSin [THEN]

Forward date-id
[defined] VFXForth [IF]
: get-id  id c@  ?EXIT
  s" " date-id  0 >o  do_getid  o>  current-win stop ;
[ELSE]
: get-id  id c@  ?EXIT
  FORTHstart 2+ count + count + count 6 safe/string -trailing
  date-id  0 >o  do_getid  o>  current-win stop ;
[THEN]

\ insert- and overwrite-mode, jump to screen           04jun08py

: setimode   imode on  ( :imode checkon  :omode checkoff ) ;
: clrimode   imode off ( :omode checkon  :imode checkoff ) ;
forward gotoline
: jump-to   textfield get drop cancel  edicatch
  isfile@ str?  IF  scr ! gotoline  ELSE  setscreen  THEN ;
: jumpscreen  S" Screen-Nr:" MODAL:
  0 0 ^ SN[ ]SN textfield new
  2fill over   S[ jump-to ]S s" OK"   button new  dup >r
  2skip 3 pick S[ cancel  ]S s" Cancel" button new
  2fill 5 hatbox new  2  r>  0 ;
[defined] DoNotSin [IF] DoNotSin [THEN]
[defined] VFXForth [IF]
: >view ( -- )  true abort" hand made" ;
[ELSE]
: voc-find ( true string -- f NFA / t string )
  voc-link LIST>  8 - >r over r> (find
  IF  swap  UNLIST  nip nip 0 swap  EXIT  THEN  drop ;
: >view   ( -- )  'find count 1- 1 safe/string scratch place
  scratch capitalize bl scratch count + c! find 0=
  IF    true swap  voc-find  swap abort" Huh?"
  ELSE  >name  THEN  ?dup 0= abort" no view-field"
  6 - w@  ?dup 0= abort" hand made"  (view scr ! ;
[THEN]

\ viewing words                                        12oct97py
: fview  'find count tuck  scr @ block b/blk c/l safe/string
  caps push caps on
  2swap search  IF nip b/blk swap - + 1- ELSE 2drop 0 THEN ;
: !view ( -- ) isfile@ str?
  IF  isfile@ scredit edifile @ =
      IF pos off gotoline ELSE r# off edi_open  THEN  EXIT THEN
  scredit edifile @ str?  IF  fview r# ! edi_open  EXIT THEN
  scr @ scredit scr# !  isfile@ scredit edifile !
  fview pos !
  edilist ;

\ viewing words                                        04jun08py

: find! ( addr count -- )  tuck 'find 2+ swap move 2+
  'find c! bl 'find 1+ c! bl 'find count + 1- c! ;
: >viewit ( -- )
  infotextfield get  find! cancel  edicatch  >view !view ;
: >markv  edit-o @ >o (mark o> >viewit ;
: do_view ( -- ) S" View Word" MODAL:
  t" " 0 ST[ ]ST s" Word:" infotextfield new
  2fill over   S[ >viewit ]S s" OK"     button new
  2skip 3 pick S[ >markv  ]S s" Mark"   button new   dup >r
  2skip 5 pick S[ cancel  ]S s" Cancel" button new
  2fill 7 hatbox new  2 r> 0 ;
[defined] DoNotSin [IF] DoNotSin [THEN]
: scr:view ( -- )  edicatch (mark
  'line c/l col word@ find! >view !view ;
Variable ?show_replace ?show_replace on

\ find und search                                      01may97py
true Value >last?
: >last                true  to >last? ;
: >1st                 false to >last? ;
Variable fscreen                2Variable <scrs>
: find?  ( -- n f )   'find count dup 'cursor #remaining
  rot over 1+ < IF  2swap 2over 2swap search >r nip - nip r>
  ELSE  2drop 2drop 0.  THEN ;
: nofound  true abort" not found" ;
: s   scr @ >r  BEGIN  find? IF  'find+ c  r> scr @ = 0=
                                 IF  edilist  THEN  EXIT  THEN
                       drop fscreen @ scr @ - ?dup
                WHILE  0< 2* 1+ scr +! top
                    scr @ scredit scr# ! draw-edislide
  REPEAT  <scrs>  >last? IF  >1st  ELSE  >last cell+  THEN
  @ fscreen !  r> scr @ <> IF  edilist  THEN  nofound ;

\ Replacing                                            09jun08py
: >cancel ( widget dpy self frame with ungrab endwith )cancel ;
: >rep     >cancel edicatch r s replace-it ;
: >search  >cancel edicatch s replace-it ;
: handle-replace
  screen self menu-frame new menu-frame with
      noop-act 1 tributton new  1 habox new hfixbox
      s" Replace?" text-label new 2 habox new
        0 S[ >rep    ]S S" Yes"     button new  dup >r
        0 S[ >search ]S S" No"      button new  r> over >r >r
        0 S[ >cancel ]S S" Cancel"  button new
      3 hatbox new hskip vskip
    2 r> modal new 0 hskips 0 vskips 2 borderbox
  ( s" " ) assign show ( xwin @ grab )
  focus r> widget with xywh endwith 2/ swap 2/ swap p+
  2dup 1 0 clicked mousexy!  endwith ;

\ Replacing ...                                        31aug97py

[defined] VFXForth [IF]
:noname   ( -- )
  ?show_replace @ 0= IF  BEGIN  r s  AGAIN  EXIT  THEN
  pos push  'find c@ negate c
  scredit show-you  scredit curpos  scredit dpy transback
  scredit dpy dpy screenpos p+ ^ edit-o !  handle-replace ;
IS replace-it
[ELSE]
: replace-it   ( -- )
  ?show_replace @ 0= IF  BEGIN  r s  AGAIN  EXIT  THEN
  pos push  'find c@ negate c
  scredit show-you  scredit curpos  scredit dpy transback
  scredit dpy dpy screenpos p+ ^ edit-o !  handle-replace ;
[THEN]

\ Editor's find and replace                            16aug98py

Variable (findbox   (findbox off
Variable ?replace

: ?findfirst
  (findbox @  'find c@  and   0= abort" use find first" ;
: repfind  ( -- )  edicatch ?findfirst
  ?stamp  fscreen @  capacity 1-  min  fscreen !
  s ?replace @  IF  replace-it  THEN  ;

: size@  scredit edifile @
  IF  scredit edifile @ str?  ELSE  true  THEN
  IF    scredit rows @
  ELSE  scredit edifile @ isfile ! capacity 1-  THEN ;

\ find!                                                07dec97py

Variable <caps>                 Variable <some>
infotextfield ptr 1st-scr       infotextfield ptr last-scr
textfield ptr find-field        textfield ptr insert-field
\ : dir!   togglebutton set? @ to >last? ;
\ : caps!  togglebutton set? @ <caps> ! ;
\ : some!  togglebutton set? @ <some> ! ;
: >find ( -- )  find-field get 'find place
  insert-field get 'insert place
  <some> @ 0= ?show_replace !
  last-scr get drop  1st-scr get drop  <scrs> 2!
  <scrs> >last?  IF  cell+  THEN  @ fscreen !
  cancel  caps push  <caps> @ caps !  repfind ;
: >repl ( -- )  ?replace on  >find ;

\ find box                                             04jun08py

\ : >select ( o flag -- o )  over >o togglechar set? ! o> ;

: screen-field ( -- o )
  2fill  1. ^ SN[ ]SN s" 1st Scr:" infotextfield new
         dup bind 1st-scr
  2skip  0 & >last? ['] noop toggle-var new
         s"  > " flipbutton new
  2skip  edit-o @ >o size@ o> 0 ^ SN[ ]SN
         s" Last Scr:" infotextfield new
         dup bind last-scr
  5 habox new hfixbox
  2fill  3 habox new ;
[defined] DoNotSin [IF] DoNotSin [THEN]

\ search and replace                                   04jun08py

: switch-field ( -- o )
         S" Case:" text-label new
  2skip  0 <caps> ['] noop toggle-var new S" Ignore"
           flipbutton new
  2fill  S" Replace:" text-label new
  2skip  0 <some> ['] noop toggle-var new S" All"
           flipbutton new
         7 habox new ;
[defined] DoNotSin [IF] DoNotSin [THEN]
: text-field  ( -- o )
  'find   count 0 ST[ ]ST textfield new  dup bind find-field
  'insert count 0 ST[ ]ST textfield new dup bind insert-field
  2 vabox new vskip ;
[defined] DoNotSin [IF] DoNotSin [THEN]

\ search and replace                                   15jul00py

: button-field ( -- o w )
        0 S[ cancel ]S s" Cancel"        button new
        0 S[ >find  ]S s" Find:"         button new dup >r
  2 habox new hskip
        0 S[ >repl  ]S s" Replace with:" button new
  2 vabox new vskip hfixbox r> ;
[defined] DoNotSin [IF] DoNotSin [THEN]

: text-button  ( -- o w )
  text-field button-field >r 2skip rot 3 habox new r> ;
[defined] DoNotSin [IF] DoNotSin [THEN]
: do_find ( -- )  ?replace off
  S" Search and Replace" MODAL:
  screen-field switch-field text-button 3 swap find-field self ;
[defined] DoNotSin [IF] DoNotSin [THEN]

: edifind  ( -- )  (findbox on  do_find ;

\ do_file isfile?                                      20may00py

: ?str  scredit edifile @ str? dup 0= IF  ?stamp  THEN ;
[defined] VFXForth [IF]
    : isfile? false ;
[ELSE]
| : isfile?     ( fcb -- fcb f ) \ is addr a fcb ?
    dup cfa@ [ ' forth.fb cfa@ ] ALiteral = ;
[THEN]
Variable path
Variable file
: !str ( addr len var -- )  dup >r $! 0 r> $@ + c! ;
: @str ( var -- addr len )
  dup @ IF  $@  ELSE  drop  s" "  THEN ;
\ : do_file ( info count string count -- exitflag )
\   r> ^ 6 $3000 $2000 NewTask pass >o rdrop edicatch
\   scredit curon >r path @str 2swap path+file  s" " 2swap
\   fsel-input >r path !str  r@ IF  file !str  THEN
\   r> 0= IF  rdrop  THEN  scredit curoff ;

\ (use                                                 19jun02py
[defined] VFXForth [IF]
    : \use ( addr count -- )  r/w open-file throw isfile ! ;
    : (use ( flag -- )  drop ; \ stub
[ELSE]
: \use ( addr count -- )  here place bl here count + c!
  here find  IF  isfile?  IF  execute EXIT  THEN THEN
  drop  NewMP  isfile ! ;
: (use ( flag -- )  file @str isfile@ assign
  isfile@ str? or IF  1 scr ! r# off edi_open  EXIT  THEN
  0 block drop isfile@ capacity 1- 0 max 1 min 0 mark! !scr a ;
[THEN]
: fp!  2over path !str path+file file !str ;
: wildcard  path @str 2swap path+file path !str path $@ ;
: >file ( f l -- f' l' )  2dup + >r  '/' -scan + r> over - ;
: UseFile  s" Use File:"  s" "
  [defined] win32 [IF] s" *.f?" [ELSE] s" *.f[sb]" [THEN] wildcard
  ^ S[ fp! [defined] win32 [IF] true [ELSE] ?str [THEN]
       file @str >file \use (use ]S fsel-dialog ;
: save-file  save-buffers  edilist ;

\ UseFile MakeFile KillFile MakeDir                    18may03py
: MakeFile s" Make File:" s" "   s" *.f?" wildcard
  ^ S[ fp! file @str >file '.' scan nip 0=
       IF  file @str isfile@ str? IF c" .fs" ELSE c" .fb" THEN
           path+file  ELSE  file @str  THEN
       2dup + 3 - 3 s" .fs" compare 0= -rot
       r/w create-file throw >r
       IF    S" ^J" r@ write-file throw
       ELSE  $800 NewHandle dup @ $800 blank  dup @ $800
             r@ write-file  swap DisposHandle throw  THEN
       r> close-file throw ]S fsel-dialog ;
: KillFile s" Kill File:"  s" "  S" *.*"  wildcard
  ^ S[ fp! file @str drop fdelete ?diskabort ]S fsel-dialog ;
: MakeDir  s" Make Dir:"   s" "  S" ."    wildcard
  ^ S[ fp! file @str drop dcreate ?diskabort ]S fsel-dialog ;

\ Window handling                                      04may97py

: wdup  scr> edi_open ;
: wshadow  scredit shadowscr self ?dup
  IF  drop ( raise-window ) EXIT THEN
  scr @ capacity scredit >shadow scr ! wdup
  ^ edit-o @ >o scredit bind shadowscr ^ o>
  scredit bind shadowscr ;

\ Table of keystrokes                                  21apr97py
Create keytable \ File
char u 8 w, w,  char m 8 w, w,  char k 8 w, w,       0 8 w, w,
char w 8 w, w,       0 8 w, w,
\ Exits
ctrl [ 0 w, w,  ctrl s 4 w, w,  ctrl x 4 w, w,  ctrl l 4 w, w,
ctrl z 4 w, w,  ctrl z 4 w, w,
\ Screens
ctrl n 4 w, w,  ctrl b 4 w, w,   $FF56 0 w, w,   $FF55 0 w, w,
ctrl w 4 w, w,  ctrl a 4 w, w,  ctrl j 4 w, w,  ctrl v 4 w, w,
char c 8 w, w,  char i 8 w, w,  char d 8 w, w,  ctrl m 4 w, w,
\ Lines
 $FF52 1 w, w,   $FF54 1 w, w,   $FF54 4 w, w,  ctrl ? 4 w, w,
ctrl h 1 w, w,  ctrl ? 1 w, w,   $FF63 1 w, w,  ctrl e 4 w, w,
ctrl m 1 w, w,  ctrl m 1 w, w,  ctrl m 4 w, w,  ctrl m 4 w, w,
ctrl p 4 w, w,

\ Table of keystrokes continue                         12dec99py
\ Chars
 $FF51 1 w, w,   $FF53 1 w, w,   $FF53 4 w, w,
ctrl h 0 w, w,  ctrl ? 0 w, w,   $FF63 0 w, w,
ctrl m 0 w, w,  ctrl j 0 w, w,
\ Cursor
 $FF52 0 w, w,   $FF54 0 w, w,   $FF51 0 w, w,   $FF53 0 w, w,
ctrl i 0 w, w,  ctrl i 1 w, w,   $FF50 0 w, w,   $FF57 0 w, w,
\ Specials
ctrl f 4 w, w,  ctrl r 4 w, w,  ctrl i 4 w, w,  ctrl o 4 w, w,
ctrl g 4 w, w,   \ $1900 4 w, w,   ( ^N^O )          $1E00 4 w, w,
( $6200 0 w, w,   $FFFF 4 w, w, $FFFF 4 w, w, ) char l 8 w, w,
\ Windows
char o 8 w, w,  char s 8 w, w,       0 4 w, w,
\ $7F00 4 w, w,   $2600 4 w, w,
here keytable - 4/  Constant #keys

\ Key event                                            13nov05py

: visible?  ( key -- f )  ( $FF and ) ;  hmacro
: ?key  ( -- key )  (key @  dup visible? 0=  abort" What?" ;
: (putchar   ( -- )  ?key dup xc-size >r
   imode @  IF  r@ instX  char>   THEN
   'cursor xc!+ drop linemodified r> c ;
: actiontable  scredit actiontable @ ;
: findkey  ( d_key -- addr )  swap $10 << or
   0 swap
     #keys 0 DO  dup  keytable  I 4* +  @ =
       IF  nip actiontable Ith swap LEAVE  THEN
             LOOP  drop ;

\ Table of actions                                     09oct94py
here to (scraction  \ File
' UseFile A,         ' MakeFile A,        ' KillFile A,        ' MakeDir A,
' save-file A,       ' edibye A,
\ Exits
' cdone A,           ' sdone A,           ' xdone A,           ' ldone A,
' undo A,            ' undo A,
\ Screens
' n A,               ' b A,               ' n A,               ' b A,
' w A,               ' a A,               ' jumpscreen A,      ' do_view A,
' clrscr A,          ' insscr A,          ' delscr A,          ' mark A,
\ Lines
' line>buf A,        ' buf>line A,        ' copyline A,        ' clrright A,
' backline A,        ' delline A,         ' instline A,        ' clrline A,
' split A,           ' split A,           ' lfsplit A,         ' lfsplit A,
' noop A,

\ Table of actions continue                            08apr95py
\ Chars
' char>buf A,        ' buf>char A,        ' copychar A,
' backspace A,       ' delchar A,         ' instchar A,
' ecr A,             ' ecr A,
\ Cursor
' curup A,           ' curdown A,         ' curleft A,         ' currite A,
' +tab A,            ' -tab A,            ' top A,             ' >""end A,
\ Specials
' edifind A,         ' repfind A,         ' setimode A,        ' clrimode A,
' do_getid A,        \ DoKontrol       ( ^N^O )          do_copyr
( do_menuhelp     mousehelp       f1-10help  )  ' noop A,
\ Windows
' Wdup A,              ' WShadow A,          ' noop A,
( 8x8font         8x16font )       ' (putchar A,

\ !nokey edierror edicatch                             19may97py

Variable nokey?    nokey? off
: !nokey ;
: edierror  jingle @  IF  alarm  THEN  scredit showerror ;

[defined] VFXForth [IF]
:noname   r>  updated? not >r
  scredit curoff  catch
  updated? r> and IF  scredit slided  THEN
  2 case? IF  scredit close  EXIT  THEN
  IF [defined] "error [IF]
	  "error @ dup IF edierror 0 THEN "error !
      [THEN] THEN
  scredit curon ; IS edicatch
[ELSE]
: edicatch   r>  updated? not >r
  scredit curoff  catch
  updated? r> and IF  scredit slided  THEN
  2 case? IF  scredit close  EXIT  THEN
  IF [defined] "error [IF]
	  "error @ dup IF edierror 0 THEN "error !
      [THEN] THEN
  scredit curon ;
[THEN]

\ Key event                                            14sep97py

: putchar  actiontable #keys cells + perform ;
Variable (shift
: ev-key ( key st -- )  (shift !  (key !
  edicatch !scr  (key @ (shift @  findkey
  ?dup IF  execute  ELSE  putchar  THEN  jingle on ;

include edit.fs

\ Installing the Editor                                14aug94py

: Makeinsbuf   insbuf @ 0= IF c/buf 2* insbuf Handle!  THEN ;
: ?clearbuffer
   chars @ ?EXIT  $100 chars Handle! #chars off
   Makeinsbuf  'insert c/buf 2* erase ;

\ Installing the Editor                                05mar00py

[defined] VFXForth [IF]
    : fit?  ;
[ELSE]
    : fit?  isfile@ #80 > IF  handle 0= IF open THEN  THEN ;
[THEN]
: pushes ;           hmacro
: settings  ( flag -- )  ?clearbuffer ;
: setmenu ;          hmacro
: ?resource ( -- ) ; hmacro

[defined] vfxforth 0= [IF]
    ?head !
[THEN]
: finstall  ( -- )
  fit? ?resource get-id settings ;
[defined] VFXForth [IF]
: date-id ( addr u -- )
  1 id c! id 1+ place ;
[ELSE]
: date-id ( addr u -- )
  1 id c!  $sum push  id 1+ $sum !  dattime
  [defined] win32 [IF]  $1 >>           [THEN]
  [defined] unix [IF]   drop timeval @  [THEN]
  base push decimal  >date id 1+ place  $add ;
[THEN]

\ Entering the Editor                                  03dec04py
: edi_open  ^ IF o@ & scredit @ =  o@ & stredit @ = or
  IF  scredit callwind self  bind term  THEN THEN
  isfile@ str?  IF  opentwind
  ELSE  scr @ capacity dup 0= IF  1 more  1+  THEN
        1- umin scr !  wi_open  THEN ;

: v   ( -- )     finstall edi_open ;
[defined] F' [IF]  #10 F' V [THEN]
: l   ( scr -- ) 1 arguments
  capacity dup 0= IF  1 more 1+  THEN  1- umin scr ! r# off v ;

: vc  ( -- )  search-buffer  ?dup
  IF    >o isfile@ str?
        IF    stredit curoff gotoline stredit curon
        ELSE  r# @ pos ! scr @ setscreen  THEN o>
  ELSE  v  THEN ;

\ Entering the Editor                                  28dec99py

: view  ( -- )  bl word count
  ?clearbuffer find! >view
  fit? isfile@ str? 0= IF  fview  ELSE  0  THEN  r# ! vc ;
: view-name ( view addr u -- )  ?clearbuffer find! (view scr !
  fit? isfile@ str? 0= IF  fview  ELSE  0  THEN  r# ! vc ;
[defined] VFXForth 0= [IF]
    ' view-name debugging view!
[THEN]

\ cold: bye:                                           27feb00py

[defined] VFXForth 0= [IF]
cold: r# off  1 scr ! ;

bye:  r> id push ( linebuffer push )
      (findbox push insbuf push \ assigned push
      ( linebuffer off ) insbuf off \ assigned off
      id off  (findbox off >r ;
[THEN]

\ ed                                                   09dec01py

| : get# ( string -- string false / # true ) dup c@ dup 0= ?EXIT
    drop number?  dup 0= ?EXIT  0>  IF  drop  THEN  true ;
| : (edfile ( addr count -- )
    scratch place scratch count 2dup 2dup '/' -scan nip safe/string
    \use isfile@ assign open 1 scr !  0 r# ! ;
: ed ( {"name.suffix" [scr/line] [char]} -- ) true >r bl word count
  BEGIN  2dup '.' scan  nip WHILE
         r>  IF  finstall  THEN  false >r
         ['] (edfile catch  dup
         IF  [defined] forth.fb [IF] forth.fb [THEN]  THEN  throw
         bl word get#
         IF  scr ! bl word get#  IF  r# ! bl word  THEN  THEN  count
         ['] edi_open catch UNTIL THEN
  >in @ #tib @ <> - negate >in +!  drop rdrop ;

\ export stuff

[defined] vfxforth 0= [IF]
export editor v l view ed ;

Module;  Onlyforth
[ELSE]
    only forth definitions
[THEN]
