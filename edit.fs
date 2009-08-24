/*             *** Streamfile-Editor ***               01sep97py

  Wie versprochen, ist hier ein voll in den Screen-Editor 
eingebundener Streamfile-Editor.

Seine Features:
   * Volle Tastaturkomptibilitaet
   * Views aus dem Editor (auch auf im Screenfile definierte 
     Woerter)
Seine Nachteile:
   * Nur bedingt zum Editieren von "normalen" Texten geeignet

  Da ich nach wie vor Screen-Befuerworter bin, ist diese Datei 
auch wie eine Screenfile formatiert, wenn auch die logischen 
Einheiten nicht alle gleich lang sind. Auf alle Faelle laesst 
man sich seltener hinreissen, neue logische Einheiten zu 
erzeugen, da es so einfach ist, eine neue physikalische (die 
Zeile) aufzumachen.    */

\ Datenstrukturen                                      27apr91py

\ Textzeile: { NextMP | PrevMP | count.b | Text.count }
\ Allokiert wird in 16-er Schritten, angefangen bei 12 Bytes
\ fuer kurze Zeilen...

\ Stream editor widget                                 01sep97py

forward ?clearbuffer

Variable do!schib
Variable :done

[defined] ?head [IF] ?head @ ?head off [THEN]

scredit class stredit
\    early !cursor
public:
    cell var thisline
    cell var changed
    cell var thisline#
    early pos@
    early pos!
    early pos+!
    early line#@
    early line#!
    early line#+!
    early dump
    early add
    early maketitle
class;

[defined] ?head [IF] ?head ! [THEN]

stredit implements
    Variable drawbuf
    : pos@     pos @ cols @ modf ;
    : pos!     pos @ cols @ /f cols @ * + pos ! ;
    : pos+!    pos +! pos @ 0 max pos ! ;

    : line#@   pos @ cols @ /f 1+ ;
    : line#!   1- pos @ cols @ modf swap cols @ * + pos !
               line#@ thisline# ! ;
    : line#+!  cols @ * pos+! line#@ thisline# ! ;
class;

: cur       stredit postpone pos@ ; immediate
: pos!      stredit postpone pos! ; immediate
: pos+!     stredit postpone pos+! ; immediate
: line#@    stredit postpone line#@ ; immediate
: line#!    stredit line#!  do!schib on ;
: line#+!   stredit line#+! do!schib on ;
: thisline# stredit postpone thisline# ; immediate

\ Zeile löschen, verschieben und einfügen              07may91py

: (DelLine ( lineMP -- ) @
  dup 2@  dup  IF  @ cell+ !  ELSE  2drop  THEN
  2@ swap dup  IF  @       !  ELSE  2drop  THEN ;
: MoveLine ( lineMP thisMP -- )  >r dup (DelLine
  r@ @ @ ?dup  IF  over @ !  THEN  r@ over @ cell+ !
  dup r> @ !
  dup @ @ dup  IF  @ cell+ !  ELSE  2drop  THEN ;
: AddLine  ( lineMP thisMP -- )  >r dup (DelLine
  r@ @ cell+ @ ?dup  IF  over @ cell+ !  THEN
  r@ over @ !  dup r> @ cell+ !
  dup @ cell+ @ dup  IF  @ !  ELSE  2drop  THEN ;

: thisline  stredit postpone thisline ; immediate
: retscr    stredit postpone retscr   ; immediate
: retbuf    stredit postpone start    ; immediate

: MakeLine ( -- MP )  $18 NewHandle  dup @ $18 erase ;
: (InsLine ( MP -- ) thisline @
  dup 0=  IF    drop thisline !  ELSE  MoveLine  THEN ;
: InsLine ( -- ) MakeLine  (InsLine ;

\ Zeile setzen                                         29apr91py

: Lalign  ( len -- alen ) $-4 cells and 8 cells + ;
: (Line! ( addr len -- MP )
  ( pad place pad count ) dup Lalign  NewHandle >r
  ( dup pos! ) r@ @ cell+ cell+ place  0. r@ @ 2!
  0 r@ @ cell+ cell+ count + c! r> ;
: Line!  ( addr len -- )  (Line! (InsLine ;

\ Zeile löschen                                        27apr91py

[defined] VFXForth [IF]
    : (thisline! ( MP / 0 -- )
	dup  IF  thisline !  EXIT  THEN  drop rdrop ; DoNotSin
    : thisline! postpone (thisline! discard-sinline ; immediate
[ELSE]
    : thisline! ( MP / 0 -- )
	dup  IF  thisline !  EXIT  THEN  drop rdrop ;
[THEN]

: NextLine ( -- )  thisline @ @       @ thisline!  1 line#+! ;
: PrevLine ( -- )  thisline @ @ cell+ @ thisline! -1 line#+! ;

: +lines ( n -- lineMP )  dup 0= IF  drop thisline @ EXIT  THEN
  >r thisline @ dup  0= IF  rdrop EXIT  THEN
  r> dup thisline# @ 1- + over 0<
  IF    0 min - negate  0 ?DO  @ cell+ @  LOOP
  ELSE  stredit rows @ - 0 max - 0 ?DO  @ @  LOOP
  THEN ;

\ Text löschen                                         29apr91py

: Top    ( -- ) \ 0 0 stredit at ;
  thisline @ 0= ?EXIT  thisline @
  BEGIN  dup @ cell+ @  dup  WHILE  nip  REPEAT  drop thisline!
  1 line#! 0 pos! do!schib on ;
: Bottom ( -- ) \ stredit rows @ 1- 0 stredit at
  thisline @ 0= ?EXIT  thisline @ 0
  BEGIN  >r dup @      @  dup  WHILE  nip r> 1+ REPEAT
  drop thisline!  r> line#+!
  thisline @ @ 8+ c@ pos!  do!schib on ;
: DelText ( -- ) thisline @ 0= ?EXIT  Top  thisline @
  BEGIN  dup @ @ >r  DisposHandle r> dup 0= UNTIL
  drop  thisline off ;

\ Streamfile sichern                                   06may91py

$2000 Constant sbuf#   \ A long buffer for fast saving
Create CRLF  1 c, #lf c,
Variable epos

: SaveLine ( addr len MP handle -- )  >r >r
  BEGIN  r@ @ sbuf# epos @ safe/string 2 pick over >  WHILE
         3 pick rot 2 pick move  epos off
         r> dup @ sbuf# r@ write-file throw
         >r safe/string  REPEAT
  drop swap dup epos +! move  rdrop rdrop ;

\ Save Text                                            12may91py

: saveText  ( -- )
  thisline push  loadfile push
  line#@ >r cur >r
  stredit edifile @ >r epos off
  [defined] filehandle [IF]
      r@ filehandle @ 0< IF  r@ r/w (open throw  THEN
  [THEN]
  0. r@ resize-file throw
  sbuf# NewHandle >r  Top thisline @
  BEGIN  dup  WHILE  dup @ 8+ count r> r> 2dup >r >r
         SaveLine  CRLF  count r> r> 2dup >r >r SaveLine
         @ @  REPEAT  drop
  r> dup @ epos @ r@ write-file >r  epos off
  DisposHandle  r> r> flush-file
  r> pos! r> line#! throw throw
  stredit changed off  do!schib on ;

\ Aktionen auf thisline                                27apr91py

: LineLen ( -- len )  stredit cols @ 1- ;

Variable ?reformat
Forward FormatPar

: enough? ( len -- )  LineLen > ?reformat ! ;
: Line@  ( -- addr count )  thisline @ @ 8 + count ;
: SetLineLen  ( len -- )  dup enough?
    Lalign dup thisline @ @ 8 + c@ Lalign
    <> IF  thisline @ over SetHandleSize  THEN drop
    0 Line@ + c! ;
: +LineLen  ( addlen -- )  thisline @ @ 8 + c@ + SetLineLen ;
: Liner@ ( -- addr count )  Line@ cur safe/string ;
: 'cursor ( -- addr ) Liner@ drop ;
: LineLen+! ( n -- ) >r thisline @ @ 8+ dup c@ r> + swap c!
  0 Line@ + c! ?reformat @ IF  FormatPar  THEN ;

\ Mausknopfreaktion                                    10may91py

: +#Line ( n -- )  line#@ + stredit rows @ min
  dup line#@ - +lines thisline! line#! ;

: mark!       scredit 'r# ! scredit 'scr ! scredit 'edifile ! ;
: (mark       isfile@  scr @  cur  mark! ;
: (mark  line#@ scr !  cur r# !  (mark ;
: str:view ( -- )  Line@ cur 2dup >
  IF    stredit curoff (mark word@ find! >view !view
        stredit curon
  ELSE  2drop drop  THEN ;
: ins+ ( n -- )
    stredit 'scr @ line#@ > IF  dup stredit 'scr +!  THEN
    stredit rows +!
\  stredit child hmin +!
\  stredit child hmin @  stredit sh @ max  stredit rows !
    stredit resized ;

\ stredit implementation

stredit implements
    : at ( r c -- )  curoff super at
      line#@ thisline# @ - +lines thisline !
      line#@ thisline# ! curon ;

    : 'line ( n -- addr u )  cols @ /modf swap >r
      thisline# @ 1- - +lines @ 8+ count r> safe/string ;

    : .line ( -- )
        pos@ >r 0 pos!
        pos @ 'line cols @ showtext  r> pos! ;

\ click and mark selection                             27feb95py

    : :view  ( -- )  edicatch str:view ;
    : paste-selection ( addr u -- ) bounds
      selection HLock
      ?DO  I c@ #lf case?  IF  $0D 1  ELSE  0  THEN
           keyed pause  LOOP
      selection HUnLock ;
        
\ resize, glue and title$                              24may94py

    : updated?  changed @ ;
    : title$ ( -- string ) s" " scratch$ $!  edifile @ ?dup
	IF  [defined] filename [IF] filename >len scratch$ $+!
	    [ELSE] drop [THEN] THEN
        S"   Line # " scratch$ $+!  base push decimal
        line#@ 0 <# bl hold # # # #S #> scratch$ $+!
        update$ scratch$ $+!  scratch$ $@ ;
    : maketitle  edifile @ 0= ?EXIT
        title$ dpy get-dpy window with title! endwith
        do!schib off ;

\ Streamfile scannen                                   29apr91py

    : dump ( xt -- ) line#@ >r pos@ >r thisline @ >r  >r
      Top thisline @
      BEGIN  dup  WHILE
             dup @ 8+ count r@ execute @ @  REPEAT
      drop  rdrop  r> thisline ! r> pos! r> line#! ;
    : add ( addr u -- ) Line! NextLine 1 rows +! ;

    | Create readbuf  $100 allot
    | Create readbuf' $200 allot
    : un-tab ( addr u -- addr' u' )
      0 -rot bounds
      ?DO  I c@ #tab = IF    dup 8 + -8 and tuck swap
                             ?DO  bl readbuf' I + c!  LOOP
                       ELSE  I c@ over readbuf' + c! 1+  THEN
      LOOP  readbuf' swap ;
    : loadText  ( -- linelen #lines )  DelText
        $100 cols !   1 line#!
        edifile @ dup isfile ! loadfile ! fpos off  open  c/l 0
        BEGIN  -eof?  WHILE
            dup $3F and $3F = IF  pause  THEN
            readbuf dup $100 ReadLine dup $100 =
            IF    3 FOR  over c/l -trailing add
                         c/l safe/string  NEXT  2drop 4+
            ELSE  un-tab
                  dup >r add  1+ swap r> max swap  THEN
        REPEAT
        dup 0= IF  scratch 0 Line!  1+  THEN
        Top F close  changed off ;
\ loadText jedenfalls liest so problemlos auch Screenfiles ein..

    : assign ( file -- )
        ?dup 0= IF  $100 1 terminal :: assign  EXIT  THEN
        edifile @ IF  edifile @ close-file throw  THEN
        edifile !
        next-buffer self 0= IF  add-to-buffer  THEN
        'edifile0 @ 'edifile !  'scr0 @ 'scr !  'r#0 @ 'r# !
        F r# @ pos!  scr @ scr# !
        ^ edit-o !
        loadText rows ! 1+ cols !
        gotoline ;
    : init ( action -- )
        term self bind callwind
        actiontable ! 0 super super init
        $31415926 retscr !  1 line#!  1 thisline# !
        0 pos! 0 rows ! ?clearbuffer ;
    
    : dispose ( -- )
        updated? edifile @ and  IF  changed off SaveText  THEN
        DelText  line#@  super dispose invert scr ! ;

    : keyed ( kb sh -- )  super keyed
        do!schib @ IF  maketitle  THEN ;
    : clicked ( x y b n -- )  dup >r  super super clicked
      r> 4 = IF  :view  THEN  maketitle ;
    : close  ( -- )  super close do!schib off ;
class;

\ Einfügen                                             07may91py

: InsChar  ( char -- )  dup xc-size >r
  r@ +LineLen Liner@ over dup r@ + rot move xc!+ drop r> LineLen+! ;
: InsString  ( addr count -- )  dup +LineLen
  Liner@ >r tuck 2dup + r> move dup >r move r> LineLen+! ;
: InsSpaces  ( len -- )  thisline @ @ 8+ c@ -
  dup 0> IF    dup +LineLen Line@ + over blank LineLen+!
         ELSE  drop  THEN ;
: DelString  ( count -- )  >r
  Liner@ dup 0=  IF  rdrop 2drop EXIT  THEN
  >r dup r> r@ safe/string >r swap r> move
  r> negate dup +LineLen LineLen+! ;

\ Zeile einfügen                                       09may91py

: >ret  ( addr count -- )  retbuf @ place ;
: mod-on
  stredit changed @ 0= IF  do!schib on  THEN
  stredit changed on ;
: modify  ( -- )  mod-on
  line#@ retscr @ case? ?EXIT
  retscr !  Line@ >ret ;
: lmodify ( -- )  modify  retscr @ negate retscr ! ;

\ Line stack                                           11oct93py

Variable linebuffer
: ?line         linebuffer @ 0= abort" line buffer empty" ;
: @line ( addr len -- )  (Line! dup
  linebuffer @ ?dup IF AddLine ELSE drop THEN linebuffer ! ;
: !lineMP  ( -- MP )  linebuffer @ dup @ @ linebuffer ! ;
: !line"  ( -- addr len )
  !lineMP  dup (DelLine dup @ 8+ count pad place 
  DisposHandle  pad count ;
                                                    
\ Vorbereitung für Undo und Redo                       01aug92py


\ Verlassen des Editors                                30apr91py

: ??done  ( -- )  stredit edifile @ 0= IF  rdrop  THEN ;
: edone   ( tf string -- )
  line#@ invert cur r# ! scr ! :done on done ;
: cdone   ( -- )    ??done  cancel-alert  1 = ?EXIT
  stredit changed off  false c" canceled"  edone ;
: sdone   ( -- )    ??done  false    c" saved"     edone ;
: xdone   ( -- )    ??done  false stredit update$ pad place
  pad edone ;
: ldone   ( -- )    ??done  true   c" loading"     edone ;

\ Cursorbewegung                                       04may91py

: curleft  cur 0=
  IF    line#@ 1 = ?EXIT  PrevLine  Line@ nip pos!
  ELSE  liner@ drop dup xchar- swap - pos+!  THEN ;
: currite  cur LineLen >=
  IF    line#@ stredit rows @ = ?EXIT  NextLine  0 pos!
  ELSE  liner@ drop dup xchar+ swap - pos+!  THEN ;

\ Tabulator                                            07may91py

: curleft+  cur 0=
  IF    line#@ 1 = ?EXIT  PrevLine  LineLen 1- pos!
  ELSE  -1 pos+!  THEN ;

: +tab         ( -- ) $10 cur    $10  modf - 0 ?DO currite LOOP ;
: -tab         ( -- ) cur  8 modf  negate  dup 0=  8 * + negate
  0 ?DO curleft+  LOOP ;

: b  $F FOR  PrevLine  NEXT ;
: n  $F FOR  NextLine  NEXT ;
: w  line#@ 1- stredit rows @ stredit >shadow
  line#@ - 1+ +#Line ;
: a  stredit 'r# @ stredit 'scr @ (mark scr ! pos! gotoline ;
: mark        (mark  true abort" marked !" ;

: gotoline cur >r Top scr @ 1 max 1 ?DO  NextLine  LOOP
    scr on r> pos! ;

\ Zeilen                                               07may91py

: linemodified  stredit .line ;
: cr  0 pos!  thisline @ @ @ 
  0= IF  pad 0 Line! 1 ins+ ( modified ) THEN
  NextLine  linemodified ;
: lineinsert   ( line# -- )  drop scredit draw ;
: (linsert  line#@ 1- lineinsert 1 ins+ stredit draw ;
: (split ( insspaces -- ) pad over blank dup pad +
  liner@ >r swap r@ move pad over r@ +
  cur >r Line! r> pos!  r> DelString linemodified
  cr pos! (linsert modify linemodified ;
: split    0   (split ;
: lfsplit  cur (split ;
: InstLine  MakeLine dup thisline @ AddLine thisline!
  (linsert mod-on ;

\ Zeile löschen                                        09may91py

: ClrRight  modify Liner@ nip negate dup +LineLen LineLen+!
  LineModified ;
: ClrLine  cur >r 0 pos!  ClrRight  r> pos! ;

: DelLine  ( -- )  lmodify  thisline @ dup @ @
  dup 0=  IF  2drop ClrLine  EXIT  THEN  thisline!
  dup (DelLine  DisposHandle  -1 ins+  stredit draw ;

: BackLine ( -- )  thisline @ @ cell+ @ 0= ?EXIT
  PrevLine DelLine ;

\ Edi line handling                                    08may91py

: copyline      line@ @line NextLine ;
: line>buf      line@ @line DelLine ;

: !line         !lineMP thisline @ AddLine
                thisline @ @ cell+ @ thisline! stredit draw ;

: buf>line      ?line  line#@ 1- lineinsert  cur >r !line
                1 ins+ mod-on r> pos! ;

\ move part of the line by one char                    30aug93py

: <char ; hmacro
: char> ; hmacro

\ (putchar del bs                                      01aug92py

: ((putchar ( char flag -- )
    modify cur over invert - InsSpaces
    IF  char> InsChar  ELSE  'cursor c!  THEN
    linemodified currite ;
: (putchar ( -- ) ?key imode @ ((putchar ;

: instchar ( -- ) bl true ((putchar curleft ;

: delchar    ( -- )  Liner@ nip 0=
  IF  thisline @ @ @ 0= ?EXIT  NextLine  Line@ pad place
      thisline @ @ @ 0=
      IF    thisline @ PrevLine thisline @ @ off
            DisposHandle -1 ins+ true
      ELSE  DelLine PrevLine false  THEN
      cur InsSpaces  pad count InsString
      IF  stredit draw  ELSE  LineModified  THEN  EXIT  
  THEN  modify Liner@ drop dup xchar+ swap -
  DelString 0 Liner@ + c! <char linemodified ;
: backspace  ( -- )  pos @ 0= ?EXIT
    curleft delchar ;

\ Character Stack                                      12may91py

: @char        liner@ IF  c@  ELSE  drop bl  THEN  (@char ;
: copychar     @char currite ;
: char>buf     @char delchar ;
: buf>char     ?chars  'chars 1- c@ true ((putchar
               -1 #chars +! curleft ;

\ Wer suchet, der findet                               13may91py

: find?  ( -- n f )   'find count dup liner@
  rot over 1+ < IF 2swap 2over 2swap search >r nip - nip r>
  ELSE 2drop 2drop 0. THEN ;
: s   BEGIN find? IF  'find+ pos+! EXIT  THEN  drop
            fscreen @ line#@ - ?dup
      WHILE 0< IF  PrevLine  ELSE  NextLine  THEN  0 pos!
      REPEAT  <scrs>  >last? IF  >1st  ELSE  >last cell+  THEN
  @ fscreen !  stredit show-you  nofound ;
: r  LineLen Line@ nip - 'insert c@ 'find c@ - < ?reformat !
  'find c@  dup negate pos+!  DelString
  'insert count InsString stredit show-you linemodified
  'insert c@ pos+! modify pause
  ?reformat @ IF  FormatPar  THEN ;

\ High level search and replace                        13may91py

forward replace-it'
: >rep     >cancel edicatch r s replace-it' ;
: >search  >cancel edicatch s replace-it' ;

\ Replacing                                            03aug97py

: handle-replace ( x y -- )
  screen self menu-frame new menu-frame with
      noop-act 1 tributton new  1 habox new hfixbox
      s" Replace?" text-label new 2 habox new
        0 S[ >rep    ]S S" Yes" button new  dup >r
        0 S[ >search ]S S" No" button new  r> over >r >r
        0 S[ >cancel ]S S" Cancel" button new
      3 hatbox new hskip vskip
    2 r> modal new 0 hskips 0 vskips 2 borderbox
  ( s" " ) assign show ( xwin @ grab )
  focus r> widget with xywh endwith 2/ swap 2/ swap p+
  2dup 1 0 clicked mousexy!  endwith ;

: replace-it'   ( -- )
  ?show_replace @ 0= IF  BEGIN  r s  AGAIN  EXIT  THEN
  pos push  'find c@ negate pos+!
  stredit show-you  stredit curpos  stredit x @ stredit y @ p+
  stredit dpy transback
  stredit dpy dpy screenpos p+ ^ edit-o !  handle-replace ;

\ Editor's find and replace                            26apr87py

: repfind  ( -- )  edicatch
  ?findfirst  fscreen @  stredit rows @ min  fscreen !
  <caps> @ caps !  s ?replace @  IF  replace-it'  THEN
  stredit show-you  stredit maketitle ;
: >find ( -- )  find-field get 'find place
  insert-field get 'insert place
  <some> @ 0= ?show_replace !
  last-scr get drop  1st-scr get drop  <scrs> 2!
  <scrs> >last?  IF  cell+  THEN  @ fscreen !
  cancel  <caps> @ caps !  repfind ;
: >repl ( -- )  ?replace on  >find ;

: button-field ( -- o w )
        0 S[ cancel ]S s" Cancel"        button new
        0 S[ >find  ]S s" Find:"         button new dup >r
  2 habox new hskip
        0 S[ >repl  ]S s" Replace with:" button new
  2 vabox new vskip hfixbox r> ;

: text-button  ( -- o w )
  text-field button-field >r 2skip rot 3 habox new r> ;
: do_find ( -- )  ?replace off
  S" Search and Replace" MODAL:
  screen-field switch-field text-button 3 swap find-field self ;
: edifind  ( -- )  (findbox on  do_find ;

\ Put a stamp to the end of the line                   14may91py

: stamp ( -- )  modify \ hide_c GEMcuroff
  cur LineLen id 1+ c@ - dup InsSpaces pos!
  liner@ nip DelString  id 1+ count InsString
  pos! linemodified ( GEMcuron 1 show_c ) ;

\ undo                                                 01aug92py

: ret@  retbuf @ count ;
: undo  retscr @ $31415926 = ?EXIT  retscr @
  0< IF  ret@ @line buf>line EXIT  THEN
  retscr @ line#@ = 0= ?EXIT
  cur >r 0 pos! ret@ pad place
  line@ tuck >ret  DelString
  pad count InsString  linemodified r> pos! ;

\ setmaxlen                                            17may91py

: !len  textfield get cancel
    drop 1+ stredit cols !
    stredit resized ;

: setmaxlen  S" Set Line Length" MODAL:
  ^ edit-o @ >o LineLen o> 0 SN[ ]SN
                     s" Line Length:" infotextfield new
  2fill over   S[ !len   ]S s"   OK  " button new dup >r
  2skip 3 pick S[ cancel ]S s" Cancel" button new
  2fill 5 habox new  2 r>  0 ;

\ format                                               06oct93py

: concline ( addr1 n1 addr2 n2 -- addr1 n1+n2 )
  dup >r 2over + swap move r> + ;
: inpar?  ( addr count -- flag )
  dup 0<= IF  nip EXIT  THEN  1- + c@  bl = ;

\ format                                               06oct93py

: extractlines ( addr n -- addr' n' )  LineLen >r
  BEGIN  dup r@ >  WHILE
         over r@ bl -scan dup 0= IF  drop LineLen  THEN
         tuck (Line! thisline @ AddLine
         1 line#+! safe/string
         tuck pad 1+ swap move  pad 1+ swap
  REPEAT  rdrop ;
: reformat ( -- -deleted )  pad 1+ 0  0 >r
  BEGIN  line@ concline  line@ inpar?  WHILE
         thisline @  dup @ @  dup  WHILE  thisline!
         dup (DelLine DisposHandle  r> 1- >r  extractlines
  REPEAT  2drop  THEN  extractlines
  0 pos! Line@ nip DelString InsString r> ;
: FormatPar ( -- )  cur >r line#@ >r  reformat
  line#@ r> - dup 0 ?DO  PrevLine  LOOP  + ins+
  r> dup pos! Line@ nip - dup
  0>= IF  NextLine pos!  ELSE  drop  THEN
  stredit draw ?reformat off ;

\ Table of actions                                     22apr91py

[defined] ?head [IF] ?head @ ?head off [THEN]
Create (straction
\ File
' UseFile A,         ' MakeFile A,        ' KillFile A,        ' MakeDir A,
' saveText A,        ' edibye A,
\ Exits
' cdone A,           ' sdone A,           ' xdone A,           ' ldone A,
' undo A,            ' undo A,
\ Screens
' n A,               ' b A,               ' n A,               ' b A,
' w A,               ' a A,               ' jumpscreen A,      ' do_view A,
' noop A,            ' noop A,            ' noop A,            ' mark A,
\ Lines
' line>buf A,        ' buf>line A,        ' copyline A,        ' clrright A,
' backline A,        ' delline A,         ' instline A,        ' clrline A,
' split A,           ' split A,           ' lfsplit A,         ' lfsplit A,
' FormatPar A,
\ Table of actions continue                            18mar89py
\ Chars
' char>buf A,        ' buf>char A,        ' copychar A,
' backspace A,       ' delchar A,         ' instchar A,
' cr A,              ' cr A,
\ Cursor
' Prevline A,        ' NextLine A,        ' curleft A,         ' currite A,
' +tab A,            ' -tab A,            ' top A,             ' bottom A,
\ Specials
' edifind A,         ' repfind A,         ' setimode A,        ' clrimode A,
' do_getid A,        \ DoKontrol       (  )          do_copyr
( do_menuhelp     mousehelp       f1-10help  )  ' setmaxlen A,
\ Windows
' noop A,            ' stamp A,           ' stamp A,
\ 8x8font         8x16font
' (putchar A,

\ open window                                          02jul94py

: opentwind ( -- )
    screen self menu-window new menu-window with
        stredi-menu
        1 1 viewport new  viewport with
            (straction stredit new  dup >r
            0 1 *fil 2dup glue new
            2 vbox new
            assign ^ r> endwith
        ^ swap stredit with
            bind win-title
            isfile@ assign title$ endwith assign
        edit-o @ stredit with 0 ins+ show-you endwith
        c/l 1+ l/s 2* geometry show
    endwith ;

: setup-edit ( addr n -- ) swap
    stredit with s" " add  1+ cols ! ^ endwith ;

[defined] ?head [IF] ?head ! [THEN]
