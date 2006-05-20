\ editor also                                          21dec97py

dos also
[IFDEF] win32api  win32api also [THEN]
[IFDEF] xconst    xconst also   [THEN]
MINOS also

AVariable 'edifile0             & forth.fb cell+ 'edifile0 !
Variable 'scr0                  1 'scr0 !
Variable 'r#0                   0 'r#0 !
Variable uclose  uclose off
Variable edit-o
Variable do-done do-done off
Variable closing closing off

forward edicatch
forward (scraction
forward ev-key
forward done
forward ?stamp
forward scr:view

terminal class scredit
public:
    cell var edifile
    cell var 'edifile
    cell var scr#
    cell var r#
    cell var 'scr
    cell var 'r#
    cell var retscr
    cell var actiontable
    ptr shadowscr
    ptr next-buffer
    terminal ptr callwind
    window ptr win-title
    early scrslide
    early !scr
    early >shadow
    early !window
    method updated?
    method update$
    method title$
    method .line
    method slided
    method showerror
class;

scredit ptr edit-buffer

scredit implements
    : >shadow      ( n1 n -- n2 )  dup 1 and
        IF    over 0=  IF  drop  EXIT  THEN  2/ 2dup > 0=
        ELSE  2/  2dup <  THEN  IF + ELSE - THEN ;
    : 'start ( -- addr )  scr# @ edifile @ (block ;
    : add-to-buffer ( -- )
        edit-buffer self bind next-buffer
        self F bind edit-buffer ;   
    : init ( action file -- )
        term self bind callwind
        edifile ! actiontable ! c/l l/s super init
        'edifile0 @ 'edifile !  'scr0 @ 'scr !  'r#0 @ 'r# !
        F r# @ pos !  scr @ scr# !
        ^ edit-o !  add-to-buffer ;
    : updated?  ( -- f )
        'start 4- @ $14 + wx@ 0< ;
    : update$   ( -- string )  updated? 0=
        IF  S" not modified"  EXIT  THEN  S" modified" ;
    : workblank  scratch $sum !
        scratch c/l bl fill  0 scratch c! ;
    : title$ ( -- addr u )
        base push decimal  workblank
        edifile @ filename >len $add
        S"   Scr # " $add  scr# @ 0 <# bl hold # # #S #> $add
        update$ $add
        scratch 1+ c/l 4- -trailing ;
    : !window  bind win-title ;
    : (slided ( -- )
        draw win-title self IF  title$ win-title title!  THEN
        & viewport @ dpy class?
        IF  dpy self viewport with hspos self
            IF  hspos draw  THEN  endwith  THEN ;
    : slided ( -- )
        (slided  shadowscr self
        IF  scr# @ capacity >shadow shadowscr scr# !
            shadowscr with (slided endwith
        THEN ;
    : showerror ( addr -- )
        title$  >r >r s"  *** " $add count $add
        r> r> win-title self IF  win-title title!
        ELSE  2drop  THEN ;
    : scrslide  self
        TS[ isfile push  edifile @ isfile ! scr# ! ?stamp slided
        ][ isfile push  edifile @ isfile ! capacity 1 scr# @ ]TS
    ;
    : close
        shadowscr self
        IF  0 shadowscr bind shadowscr  0 bind shadowscr  THEN
        do-done @ do-done off
        closing push closing @ closing on or
        0= IF  edicatch false " closed" done  EXIT  THEN
        dpy close ;
    : !scr   edifile @ isfile !  scr# @ scr !  pos @ r# ! ;
    : type  super type update ;
    : scrollup  pos @ b/blk mod pos ! ;
    : .line ( y -- )  >r at? r> 0 at
        pos @ 'line c/l showtext  at ;
    : keyed ( key sh -- )
        dup $100 and IF  drop $100 /mod swap at  EXIT  THEN
        -$13 and over shift-keys?  IF  2drop  EXIT  THEN
        dup 2 and IF  swap tolower swap  THEN
        !scr $D and ev-key ;
    : clicked ( x y b n -- )  dup >r super clicked
      r> 4 = IF  edicatch scr:view  THEN ;
    : dispose ( -- )
        F link edit-buffer
        BEGIN  dup @ ^ <>  WHILE
            dup @ 0<>   WHILE
            @ >o link next-buffer o>  REPEAT  THEN
        next-buffer self swap !  super dispose ;
class;

menu-entry class edimenu-entry
    cell var item
how:
Variable action#
    : do-action
        scredit !scr edicatch
        scredit actiontable @ action# @ cells +
        perform ;
    : menu-action
        window innerwin self
        viewport with child with
            & combined @ class?
            IF  combined childs self op!  THEN
            do-action
        endwith endwith ;
    : clicked ( x y b n -- )
        dup 0= IF 2drop 2drop EXIT THEN
        super clicked  item @ action# ! ;
    : init ( n addr count -- )
        ^^ ['] menu-action simple new -rot super init  item ! ;
class;

viewport class scrviewport
    & child scredit asptr screen-edit
how:
    : 'hslide  screen-edit scrslide ;
    : hglue  super hglue + 0 ;
    : scr#+! ( n -- )  screen-edit with
        scr# @ + 0 max edifile @ isfile ! capacity 1- min
        scr# ! slided  endwith ; 
    : clicked ( x y b n -- )
      over $18 and over 1 and 0= and IF  \ scroll
         over $10 and  IF   1 scr#+!   THEN
         over $08 and  IF  -1 scr#+!  THEN
         over $18 and  IF  slided  THEN
         2drop 2drop
       ELSE  super clicked  THEN ;
\    backing :: keyed
class;

: (menu"  "lit count edimenu-entry new ;               restrict
: menu"   postpone (menu" ," ;               immediate restrict
: (label" "lit count       menu-label new ;          restrict
: label"  postpone (label" ," ;              immediate restrict

: file-menu: ( -- o )
    label"  File System"
    0 menu" Use File...  &M-u"
    1 menu" Make File...  &M-m"
    2 menu" Kill File...  &M-k"
    4 menu" Save  &M-w"
    label"  Folders"
    3 menu" Make Dir..."
    label"  Leave Editor"
    6    menu" cancel changes  &Esc"
    8    menu" close window  &C-x"
    7    menu" save and leave  &C-s"
    9    menu" save and load  &C-l"
    &12 vabox new 2 borderbox ;

: edit-menu: ( flag -- o )   >r
  &10    menu" Undo  &C-z"
        label"  Searching"
  &53    menu" Find  &C-f"
  &54    menu" Repeat  &C-r"
        label"  Write mode"
  &55    menu" Insert  &C-i"
  &56    menu" Overwrite  &C-o"
        label"  Author"
  &57    menu" Get ID...  &C-g"
  r>   IF
        label"  Line"
  &58    menu" Set Length  &M-l"
  &60    menu" Stamp  &M-s"
  &12  ELSE
  &09  THEN
      vabox new 2 borderbox ;

: screen-menu:  ( -- o )
  &12    menu" Next Scr  &C-n"
  &13    menu" Back Scr  &C-b"
  &16    menu" Shadow Scr  &C-w"
  &17    menu" Jump to Mark  &C-a"
  &18    menu" Jump to Scr...  &C-j"
  &19    menu" View...  &C-v"
        label"  don't move"
  &20    menu" Clear Scr  &M-c"
  &21    menu" Insert Scr  &M-i"
  &22    menu" Delete Scr  &M-d"
  &23    menu" Set Mark  &C-m"
  &11 vabox new 2 borderbox ;

: line-menu:    ( -- o )
        label"  wag Tail of Scr"
  &28    menu" Backspace Line  &S-bs"
  &29    menu" Delete Line  &S-del"
  &30    menu" Insert Line  &S-ins"
  &32    menu" Split Line  &S-ret"
  &34    menu" Linefeed  &C-ret"
  &24    menu" Cut to Stack  &S-up"
  &25    menu" Paste from Stack  &S-down"
        label"  don't wag Tail of Scr"
  &26    menu" Copy to Stack  &C-down"
  &31    menu" Erase Line  &C-e"
  &27    menu" Erase Line-rest  &C-del"
  &12 vabox new 2 borderbox ;

: char-menu:    ( -- o )
        label"  wag Tail of Line"
  &37    menu" Cut to Stack  &S-left"
  &38    menu" Paste from Stack  &S-right"
        label"  don't wag Tail of Line"
  &39    menu" Copy to Stack  &C-right"
  5 vabox new 2 borderbox ;

: cursor-menu:  ( -- o )
        label"  move Cursor quick"
  &51    menu" Home  &home"
  &52    menu" > Text-End  &S-home"
  &49    menu" 1/4 Line >  &tab"
  &50    menu" 1/8 Line <  &S-tab"
  5 vabox new 2 borderbox ;

: window-menu:  ( -- o )
        label"  Open"
  &59    menu" Duplicate  &M-o"
  &60    menu" Shadow  &M-s"
  3 vabox new 2 borderbox ;

: make-menu ( flag -- o )
    >r ^ to ^^
         file-menu: s"  File "    menu-title new
   r@    edit-menu: s"  Edit "    menu-title new
       screen-menu: s"  Screen "  menu-title new
         line-menu: s"  Line "    menu-title new
         char-menu: s"  Char "    menu-title new
       cursor-menu: s"  Cursor "  menu-title new
    r> IF  6
     ELSE  window-menu: s"  Window "  menu-title new  7 THEN
    2fill swap 1+
  hbox new vfixbox ;

: scredi-menu ( -- o )  false make-menu ;
: stredi-menu ( -- o )  true  make-menu ;

: wi_open ( -- )
    screen self menu-window new menu-window with
        scredi-menu
        1 1 scrviewport new  scrviewport with
            (scraction isfile@ scredit new  dup >r
            assign ^ r> endwith
        ^ swap scredit with
            !window title$ endwith
        assign c/l l/s geometry show
    endwith ;

\ window shortcuts                                     02jul94py

| : pos  scredit pos ; hmacro
: cur          ( -- n )     pos @ ;  macro
: c            ( n -- )     dup cur + b/blk 0 within
  abort" Border!" scredit c ;
: updated?  scredit updated? ;
: curup        c/l negate c ;
: curdown      c/l c ;
: curleft      scredit 'start cur + dup xchar- swap - c ;
: currite      scredit 'start cur + dup xchar+ swap - c ;
: 'start    scredit 'start ;

\ print buffers                                        27dec99py

?head @ ?head off

: .edit-buffers ( -- )
  edit-buffer self
  BEGIN  dup  WHILE  cr
      scredit with  edifile @ .file  next-buffer self  endwith
  REPEAT  drop ;

?head !

: search-buffer ( -- o / 0 )
  edit-buffer self
  BEGIN  dup  WHILE
      scredit with
          next-buffer self
          edifile @ isfile @ = self and
      endwith  ?dup IF  nip  EXIT  THEN
  REPEAT  drop 0 ;

[IFDEF] x11
: mousexy!  0 0 0 0 window xwin @ 0 window xrc dpy @
  XWarpPointer drop ;
[ELSE]
: mousexy!  2drop ;
[THEN]

