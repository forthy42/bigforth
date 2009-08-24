#! xbigforth
\ graphical widget editor                              14sep97py

[defined] VFXFORTH [IF]
    include vfx-minos/fileop.fs
[ELSE]
    [defined] fileop 0= [IF]  include fileop.fb  [THEN]
[THEN]
also minos [defined] float-action 0= [IF]
    previous include minos-float.fs
[ELSE]
    previous
[THEN]

[defined] x11 [IF]  \needs xconst | import xconst
[THEN]

Module theseus

fileop also memory also minos also editor also forth also

include theseus-classes.fs

: $@? ( var -- flag )  dup @  IF  $@ nip 0<>  ELSE  @  THEN ;

: .class" ( object -- )  >class" type ;
\ ' .class" IS .class

\ icons

ficon: test-icon icons/computer"
ficon: on-icon icons/mini-exclam"
ficon: off-icon icons/mini-cross"
ficon: minos-icon icons/minos"
ficon: minos-win icons/minos1+"
[defined] alias [IF]
' dir-icon Alias res-icon
' diro-icon Alias resopen-icon
[ELSE]
    synonym res-icon dir-icon
    synonym resopen-icon diro-icon
[THEN]

\ tools

: >child ( o -- o' )
    combined with childs self endwith ;
: >child2 ( o -- o' )
    combined with childs widgets self endwith ;

Variable comp#

: anonymous-component ( -- addr u )  base push hex
    comp# @ 0 <# 3 0 ?DO  #  LOOP  #S
                 S" pmoc" bounds ?DO  I c@ hold  LOOP #>
    1 comp# +! ;

\ empty box display

widget class cross
how:
    : hglue  parent self combined with n @ endwith 1 <= IF
        xM 1 *fill  ELSE  0 0  THEN ;
    : vglue  parent self combined with n @ endwith 1 <= IF
        xM 1 *fill  ELSE  0 0  THEN ;
    : draw  parent self combined with n @ endwith 1 > ?EXIT
        xywh defocuscol @ @ dpy box
        xywh 2over p+ 0 dpy line
        x @ y @ h @ + x @ w @ + y @ 0 dpy line ;
class;

: new-code ( addr u content -- o )
    codeedit new
    codeedit with c/l cols ! add-lines
        0 0 at ^
    endwith ;

\ link list

links implements
    : init ( linked linker -- )  bind linker bind linked ;
    : find-linked ( linker -- linked )
        dup linker self = IF  drop linked self  EXIT  THEN
        next self IF  next goto find-linked  ELSE  drop 0 THEN ;
    : find-linker ( linked -- linker )
        dup linked self = IF  drop linker self  EXIT  THEN
        next self IF  next goto find-linker  ELSE  drop 0 THEN ;
    : update-linked ( new-linked old-linked -- )
        dup linked self = IF  drop bind linked
            linked self linker callback bind called  EXIT  THEN
        next self IF  next goto update-linked  ELSE 2drop THEN ;
    : update-linker ( new-linker old-linker -- )
        dup linker self = IF  drop bind linker
            linked self linker callback bind called  EXIT  THEN
        next self IF  next goto update-linker  ELSE 2drop THEN ;
    : dump ( -- )
        cr linked self . linker self .
        next self IF  next goto dump  THEN ;
class;

links ptr first-link

: find-linked
    first-link self IF  first-link find-linked
    ELSE  drop 0  THEN ;
: find-linker
    first-link self IF  first-link find-linker
    ELSE  drop 0  THEN ;
: update-linked
    first-link self IF  first-link update-linked
    ELSE  2drop  THEN ;
: update-linker
    first-link self IF  first-link update-linker
    ELSE  2drop  THEN ;

: new-link ( linked linker -- )
    links new links with
        first-link self bind next
        ^ 
    endwith bind first-link ;
[defined] DoNotSin [IF] DoNotSin [THEN]

\ name hints for boxes and displays

hint-name ptr names

hint-name implements
    : find-name ( o -- o / 0 )
        dup hint self = IF  drop self  EXIT  THEN
        next self 0= IF  drop 0 EXIT  THEN
        next goto find-name ;
    : update-hint ( newo oldo -- )
        dup hint self = IF  drop bind hint  EXIT  THEN
        next self 0= IF  2drop EXIT  THEN
        next goto update-hint ;
    : update-name ( addr u -- )
        name $! ;
    : init ( addr u o -- )
        bind hint name $! names self bind next ;
class;

s" " 0 hint-name new bind names

\ box classifying

Create &boxes
& vbox @ A,     & vtbox @ A,    & vrbox @ A,    & vrtbox @ A,
& vabox @ A,    & vatbox @ A,   & varbox @ A,   & vartbox @ A,
& hbox @ A,     & htbox @ A,    & hrbox @ A,    & hrtbox @ A,
& habox @ A,    & hatbox @ A,   & harbox @ A,   & hartbox @ A,
0 ,             0 ,             0 ,             & vtab @ A,
& vresize @ A,  & vasbox @ A,   0 ,             & vatab @ A,
0 ,             0 ,             0 ,             & htab @ A,
& hresize @ A,  & hasbox @ A,   0 ,             & hatab @ A,

: ?hbox ( object -- flag )
    @ 0  $20 0 DO  I 8 and IF  over &boxes Ith = or THEN
    LOOP  nip ;
: ?abox ( object -- flag )
    @ 0  $20 0 DO  I 4 and IF  over &boxes Ith = or THEN
    LOOP  nip ;
: ?rbox ( object -- flag )
    @ 0  $10 0 DO  I 2 and IF  over &boxes Ith = or THEN
    LOOP  nip ;
: ?tbox ( object -- flag )
    @ 0  $10 0 DO  I 1 and IF  over &boxes Ith = or THEN
    LOOP  nip ;
: ?table ( object -- flag )
    @ 0  $20 $13 DO  over &boxes Ith = or
    4 +LOOP  nip ;

: (makebox ( n -- )
    cells &boxes + @ new, ;

Variable indent

#40 Value delay-to

\ class descriptor

descriptor implements
    : DELAY ( -- )
        ^ screen cleanup screen sync
        r> ^ delay-to after screen schedule ;
    : edit-field ;
    : null ;
    : make ;
    : dump ;
    : post-dump ( -- ) ;
    : assign ;
    : get ;
class;

forward bind-cur
forward dump-names
forward dump-box
forward addinstead
forward dispose-box

widget ptr edit-string
widget ptr code-string
widget ptr code2-string
widget ptr code-label
widget ptr code2-label
widget ptr name-string
    
infotextfield class infocodefield
    codeedit ptr code-lines
    cell var ^content
  how:
    : init ( act xxx addr2 u2 -- )
        rot ^content !
        text-label new dup bind info
        0 1 *fill 2dup glue new
        2 vabox new
        ^content @ HLock
        get ^content @ new-code dup bind code-lines
        dup F bind code-string
        ^content @ HUnLock
        1 habox new -2 borderbox
        0 1 *fill 2dup glue new
        3 super super super init ;
    : assign ( addr u -- )  ^content @ $! ;
    : get ( -- addr u )  ^content @ $@ ;
class;

: minos-design ( o -- o )
    hxrtsizer new 2 hasbox new
    vxrtsizer new 2 vasbox new
    0 1 *filll 2dup rule new dup rule with $D assign endwith
    2 habox new
    1 vabox new ;

: link-resource ( .. o -- o' )
    resource:dialog with
        link-designer
        ^ var-box self methods-box self endwith
    >r rot r> swap 4 vabox new ;

: minos:dialog ( -- o )
    cross new 1 vabox new  panel  1 designerbox new
    dup >r minos-design
    dup resource:dialog new
    r@ swap link-resource
    r> designerbox with <box> endwith ;

: minos:menu-window ( -- o )
    cross new 1 hbox new  vfixbox
    cross new 1 vabox new   panel  2 vbox new  1 designerbox new
    dup >r minos-design
    dup resource:menu-window new
    r@ swap link-resource
    r> designerbox with <box> endwith ;

theseus

designer ptr cur

forward auto-save-minos
forward do-auto-save
: schedule-auto-save ( -- )
    ['] do-auto-save cur self #120000 after
    cur dpy schedule ;
: do-auto-save
    cur self >r  ^ bind cur
    cur save-state @
    IF  auto-save-minos  schedule-auto-save  THEN
    r> bind cur ;

Variable loading  loading off
: changed ( -- ) loading @ ?EXIT
  cur save-state @ cur save-state on
    0= IF
        cur with s"  *" title+!  endwith
        schedule-auto-save  THEN ;
: bind-cur  ^ bind cur ;
: box-name ( o -- )  >r s" " r> hint-name new bind names ;

: (addbox ( object n -- o )
    cur +boxmode  @ 0<> 8 and
    cur +activate @ 0<> 4 and or
    cur +tabular  @ IF
        $13 or
    ELSE
        cur +radio    @ 0<> 2 and or
        cur +tabbing  @ 0<> 1 and or
    THEN
    (makebox dup box-name
    cur +hfixbox @  IF hfixbox  THEN
    cur +vfixbox @  IF vfixbox  THEN
    cur +flipbox @  IF flipbox  THEN
\    cur +rzbox   @  IF rzbox  THEN
    cur +hskip @    IF cur +hskip @ hskips  THEN
    cur +vskip @    IF cur +vskip @ vskips  THEN
    cur +borderw @  IF cur +borderw @ borderbox THEN
    cur +noborder @ IF cur +noborder @ noborderbox THEN
    changed ;

Variable set-var
Variable nvar

forward >current-name

forward addcardfile

\ editor variant

Create newline  1 c, #lf c,
: $+line ( addr u string -- )  >r
    r@ $@len IF
        newline count r@ dup $@len $ins  THEN
    r> dup $@len $ins ;

codeedit implements
    : init ( content -- )
        (straction super init ^content ! ;
    : xinc 0 1 ;
    : yinc 0 1 ;
    : save-contents ( addr u -- )
        ^content @ $+line ;
    : backup  s" " ^content @ $!
        ['] save-contents dump ;
    : add-lines ( addr u -- )
        dup 0= IF  add  EXIT  THEN
        BEGIN
            2dup #lf scan dup >r 2swap r> -
            dup 1+ cols @ max cols ! add  dup  WHILE
            1 /string  REPEAT
        2drop
        thisline @ 0= ?EXIT  thisline @
        BEGIN  dup @ cell+ @  dup  WHILE  nip  REPEAT  drop thisline !
        1 line#! 0 pos! ;
    : dispose  backup :: dispose ;
    : defocus  backup :: defocus ;
class;

\ descriptors

include theseus-desc.fs

\ designer

AVariable do-it

: do-click  do-it @       perform ;
: do-key    do-it @ cell+ perform ;

: <rebox> ( -- )  cur self 0= ?EXIT  cur box self 0= ?EXIT
    cur box self names find-name ?dup 0= IF
        s" " cur box self hint-name new dup bind names
    THEN
    cur bind cur-box-name
    cur cur-box-name name $@ cur cur-box-edit assign ;
: <redpy> ( -- )  cur self 0= ?EXIT  cur widget self 0= ?EXIT
    cur widget dpy self all-descs find-object
    dup cur bind cur-dpy
    IF  cur cur-dpy with descriptors name $@ endwith
        cur cur-dpy-edit assign
    ELSE
        s" " cur cur-dpy-edit assign
    THEN ;

designerbox implements
    : hide ( -- )
        0 cur bind box  0 cur bind widget super hide ;
    : draw-decor ( -- )
        cur box self
        IF
            $FF 0 0 rgb> $BF 0 0 rgb>
            2 cur box xywh 2swap -2 -2 p+ 2swap 4 4 p+
            cur box drawshadow
        THEN
        cur widget self
        IF
            0 $7F $FF rgb> 0 $5F $BF rgb>
            1 cur widget xywh 2swap -1 -1 p+ 2swap 2 2 p+
            cur widget drawshadow
        THEN
        cur resources default $@ dup
        IF    >r >r shadow swap xS 2/
              r> r> all-descs find-name ?dup
              IF    descriptors with  item with  xywh
                        xN 1+ 2/ negate xywh- drawshadow endwith  endwith
              ELSE  drop 2drop  THEN
        ELSE  2drop  THEN ;
    : draw ( -- )  super draw  draw-decor ;
    : <box> ( -- )
	& resource:menu-window @ cur resources class? IF
            childs with childs self         endwith cur bind menubox
	    childs with childs widgets self endwith
        ELSE
	    childs self
	THEN
	dup  cur bind topbox  cur bind box  ;
    : clicked ( x y b n -- ) <box> do-click <rebox> <redpy>
        ['] draw-decor ^ #50 after screen schedule ;
    : keyed ( key sh -- )  <box> do-key
        ['] draw-decor ^ #50 after screen schedule ;
    : moved ( x y -- )
        2drop  do-it @ 2 cells + @ dpy set-cursor ;
    : init ( o1 .. on n -- )
        super init ^ panel drop ;
class;

8 colors designerbox defocuscol !
8 colors designerbox focuscol !

\ resource bar

resource:dialog ptr dialog-stack

resource:dialog implements
    : ?menu-call ( flag -- )
	IF  menu-call toggle  THEN ;
    : edit-toggle combined +flip changed ;
    : edit-box { addr u taddr tu content icon } ( --> box edit )
        s" " content $!
        addr u text-label new
        content codeedit new dup >r
        dup codeedit with s" " add-lines 0 0 at $40 cols !
            edifile off endwith
        1 vabox new -2 borderbox
        0 1 *fill 2dup glue new
        2 habox new 2 vabox new
        dup >r flipbox
        false ['] edit-toggle combined ' -flip toggle new
        taddr tu TT-string
        icon flipicon new r> r> ;
    : send-key ( c -- )  0 callwind keyed ;
    : send-keys ( addr u -- )
        bounds ?DO  I c@ send-key  LOOP ;
    : prev-resource-link ( -- addr )
        cur link resources
        BEGIN  dup @  WHILE
            dup @ ^ <>  WHILE
            @ >o link next-resource o>  REPEAT
        ELSE  true abort" not found"  THEN ;
    : cut ( -- )
        parent self parent parent with combined remove endwith
        prev-resource-link next-resource self swap !
        dialog-stack self bind next-resource
        ^ F bind dialog-stack ;
    : paste-before ( -- )  dialog-stack self 0= ?EXIT
        dialog-stack parent self  parent self
        parent parent with combined add resized endwith
        dialog-stack self
        dialog-stack next-resource self F bind dialog-stack
        next-resource self over >o bind next-resource o>
        bind next-resource ;
    : paste-after ( -- )  dialog-stack self 0= ?EXIT
        dialog-stack parent self  parent widgets self
        parent parent with combined add resized endwith
        dialog-stack self
        dialog-stack next-resource self F bind dialog-stack
        dup prev-resource-link !
        self swap >o bind next-resource o> ;
    : dispose ( -- )  \ uncomplete
        default HandleOff
        topbox self dispose-box
        next-resource self prev-resource-link !
        super dispose ;
    : init ( box -- )
        s" " default $!
        cur callwind self bind callwind
        cur resources self bind next-resource
        ^ cur bind resources

        true combined ' +flip combined ' -flip toggle new
        TT" Dialog Editor"
        res-icon resopen-icon toggleicon new

        s" Vars:" s" Edit Variables"
        var-content icon" icons/vars" edit-box
        bind var-edit bind var-box

        s" Methods:" s" Edit Methods"
        methods-content icon" icons/code" edit-box
        bind methods-edit bind methods-box

        TV[ ^ shown changed ]T[ changed ]TV shown on
        TT" Show Dialog"
        off-icon on-icon toggleicon new
        dup bind show-state

        ^ S[ ^ S[ cut ]S s" Cut Dialog"  menu-entry new
             dialog-stack self
             IF  ^ S[ paste-before ]S s" Paste Before" menu-entry new
                 ^ S[ paste-after  ]S s" Paste After"  menu-entry new
             THEN
             hline
             ^ S[ name-string self IF name-string get default $! THEN
                  cur resources topbox parent draw ]S
             s" Set Default" menu-entry new
             ^ S[ S" " default $! cur pane draw ]S
             s" No Default" menu-entry new
             hline
             ^ S[ s" ed " send-keys
                class-file $@?
                IF    class-file $@ send-keys
                ELSE  name-field get send-keys s" -classes.fs" send-keys  THEN
                #cr send-key
             ]S s" Edit decl" menu-entry new  1 habox new hfixbox
             class-file @
             IF  class-file $@  ELSE  s" "  THEN
             ^ ST[ CF-field get class-file $! ]ST
             ( s" CF:" info) textfield new dup bind CF-field  2 habox new
             ^ S[ s" ed " send-keys
                implementation-file $@?
                IF    implementation-file $@ send-keys
                ELSE  name-field get send-keys s" .fs" send-keys  THEN
                #cr send-key
             ]S s" Edit impl" menu-entry new  1 habox new hfixbox
             implementation-file @
             IF  implementation-file $@  ELSE  s" "  THEN
             ^ ST[ IF-field get implementation-file $! ]ST
             ( s" SF:" info) textfield new dup bind IF-field  2 habox new
             dialog-stack self IF  9  ELSE  7  THEN  vabox new
             2 borderbox dup >r [defined] x11 [IF] dpy get-win swap [THEN]
	     menu-icon with menu-frame popup endwith ?menu-call
             r> with dispose endwith
           ]S TT" Dialog Menu"
           icon" icons/menu" icon-but new dup bind menu-icon

        5 hatbox new hfixbox
        t" No Title" 0 ST[ ]ST s" Title:" infotextfield new
            dup bind title-field
        anonymous-component 0 ST[ ]ST s" Name:"  infotextfield new
            dup bind name-field
        2 habox new 2 borderbox
        2 super init self ( rzbox) drop ;
    : .default ( -- )
        default $@ dup 0= IF  2drop ." 0"  EXIT  THEN
        type ."  self" ;
    : dump-declaration ( -- )
        next-resource self
        IF  next-resource dump-declaration  THEN
        name-field get nip 0<> IF
            class-file $@?
            IF  cr ." include " class-file $@ type  THEN
            cr base-class type ."  class " name-field get type
            cr ." public:"
            nvar off set-var on 2 indent !
            dump-names'  var-edit backup
            cr ."  ( [varstart] ) " var-content $@ type
            ."  ( [varend] ) "
            nvar off set-var off 6 indent !
            cr ." how:" cr
            .'   : params   DF[ '  .default
                .'  ]DF X" ' title-field get type
                .' " ;' cr
            ." class;" cr
        THEN ;
    : dump-implementation ( -- )
        name-field get nip 0<> IF
            implementation-file $@?
            IF  cr ." include " implementation-file $@ type  THEN
            cr name-field get type ."  implements"  methods-edit backup
            cr ."  ( [methodstart] ) " methods-content $@ type
            ."  ( [methodend] ) "
            cr ."   : widget  ( [dumpstart] )"
            dump-contents
            cr .'     ( [dumpend] ) ;'
            cr ." class;" cr
        THEN
        next-resource self 0= ?EXIT
        next-resource goto dump-implementation ;
    : dump-script ( n -- n+1 )
        name-field get nip 0<> IF
            shown @ IF
                cr 2 spaces name-field get type
                .'  open-app' 1+
            THEN
        THEN
        next-resource self 0= ?EXIT
        next-resource goto dump-script ;
    : script? ( flag -- flag' )
        shown @ or
        next-resource self 0= ?EXIT
        next-resource goto script? ;
    : find-name ( addr u -- o/0 )
        2dup name-field get compare 0=
        IF  2drop ^  EXIT  THEN
        next-resource self 0= IF  2drop 0  EXIT  THEN
        next-resource goto find-name ;
\ object-specific parts
    : base-class ( -- addr u )
        s" component" ;
    : dump-contents ( -- )
        topbox self dump-box ;
    : dump-names' ( -- )
        topbox self dump-names ;
    : link-designer ( o -- )
        >child bind topbox ;
    : add-box ( o -- )
        topbox self dup cur bind box cur bind topbox addinstead ;
    : >cur ( -- )
        cur topbox self bind topbox topbox show ;
class;

resource:menu-window implements
    : base-class s" menu-component" ;
    : .default ( -- )
        default $@ dup 0= IF  2drop ." 0"  EXIT  THEN
        type ."  self" ;
    resource:dialog :: dump-declaration ( -- )
    resource:dialog :: dump-implementation ( -- )
    resource:dialog :: dump-script ( n -- n+1 )
    : dump-contents ( -- )
	topbox childs self dump-box
	topbox childs widgets self dump-box ;
    resource:dialog :: link-designer
    resource:dialog :: dump-names'
    resource:dialog :: >cur
    : add-box ( o1 o2 -- ) 2 vbox new resource:dialog :: add-box ;
    resource:dialog :: init
    resource:dialog :: script?
    resource:dialog :: find-name
class;

\ boxes

Patch +object
Patch cur-dpy
forward addbox
Variable reenter

: ?emptybox
    cur box self 0= IF  cur topbox self cur bind box  THEN
    cur box childs self @ & cross @ =
    IF  cur box childs self  dup  cur box remove
        dup cur widget self = IF  0 cur bind widget  THEN
        widget with dispose endwith
    THEN  changed ;

\ check for cardfile structure: vbox{harbox, hbox}

: ?cardfile ( -- )
    cur box self cur topbox self <>  IF
        cur box parent self cur topbox self <>  IF
            & harbox @ cur box class?  IF
                & vbox @ cur box parent class?  IF
                    & hbox @ cur box widgets class?  ?EXIT
                THEN
            THEN
        THEN
    THEN
    cross new 1 $E (makebox dup box-name vfixbox dup >r
    cross new 1 $C (makebox dup box-name
    2 borderbox :notshadow noborderbox
    2 $4 (makebox dup box-name +object r> cur bind box ;
            
: addfirst  ?emptybox
    cur box childs self
    cur box with add resized endwith ;
: addlast   ?emptybox
    'nil cur box with add resized endwith ;
: addafter
    cur widget self 0= IF  addlast  EXIT  THEN
    cur widget parent self cur bind box  ?emptybox
    cur widget widgets self
    cur widget parent with combined add resized endwith ;
: addbefore
    cur widget self 0= IF  addfirst  EXIT  THEN
    cur widget parent self cur bind box  ?emptybox
    cur widget self
    cur widget parent with combined add resized endwith ;
: addcardfile ( -- o )
    cross new 1 $C (makebox panel dup box-name dup
    ?cardfile
    cur box self >r
    cur box widgets self cur bind box  addlast
    r> cur bind box cur box resized ;
: addinstead ( o -- )
     cur box self 2dup
     cur box parent with combined add combined remove endwith
     cur box self cur topbox  self =  IF   dup cur bind topbox   THEN
     cur box self cur menubox self =  IF   dup cur bind menubox  THEN
     cur bind box ;

' addlast IS +object

: cur-box-dpy  cur box dpy self ;
: cur-obj-dpy  cur widget dpy self ;

' cur-box-dpy is cur-dpy

: addbox ( flag -- ) cur +boxmode !
    1 cur +hskip ! 1 cur +vskip !
    cross new
    1 (addbox +object ;

: redraw  cur pane draw cur status draw ;

: rebox ( -- )
    cur box childs self
    cur box n @ 0 ?DO  dup widget with widgets self endwith LOOP
    cur box bind childs
    cur box n @ (addbox dup >r
    & displays @ cur box parent class?
    IF
        r> cur box parent with
            dup cur bind box
            assign  drop
        endwith
    ELSE
        cur box widgets self
        cur box parent self
        cur box self cur box parent with combined remove endwith
        r@ cur box self update-linked
        r@ names find-name
        IF  names next self  names dispose  bind names
            r@ cur box self names update-hint  THEN
        cur box self cur topbox self = IF
	    r@ cur bind topbox
	    r@ cur resources with
	    BEGIN
		cur box self topbox self = IF  dup bind topbox  THEN
		next-resource self  WHILE
		    next-resource self op!
	    REPEAT  endwith drop
        THEN
        cur box self cur menubox self = IF
	    r@ cur bind menubox
	    r@ cur resources self resource:menu-window with
	    BEGIN
		cur box self menubox self = IF  dup bind menubox  THEN
		next-resource self  WHILE
		    next-resource self op!
	    REPEAT  endwith drop
        THEN
        0 cur box n !  cur box dispose
        r> cur bind box
        combined with add resized endwith
    THEN ;

: ?cur-box   cur box self 0= IF  rdrop  THEN ;
: ?cur-box:0 cur box self 0= IF  0 rdrop  THEN ;

: box-low ( -- o )
    0 TS[ 1 and cur +hskip ! ?cur-box
          cur +hskip @ cur box hskip c! cur box resized ][
          ?cur-box:0 cur box hskip c@ dup cur +hskip ! 0<> ]TS
          s" hskip" tbutton new
    0 TS[ 1 and cur +vskip ! ?cur-box
          cur +vskip @ cur box vskip c! cur box resized ][
          ?cur-box:0 cur box vskip c@ dup cur +vskip !  0<> ]TS
          s" vskip" tbutton new
    0 TS[ 2 and cur +borderw ! ?cur-box
          cur +borderw @ cur box borderw c! cur box resized ][
          ?cur-box:0 cur box borderw cx@ dup cur +borderw !
          0<> ]TS
          s" border" tbutton new
    0 1 *fill 2dup glue new
  4 vabox new ;

: box-detail ( -- o )
      0 :[ cur +hskip ! ?cur-box
           cur +hskip @ cur box hskip c! cur box resized ]:
        :[ ?cur-box:0 cur box hskip c@ dup cur +hskip ! ]:
        9 scale-act new   TT" hskip"        hscaler new
      0 :[ cur +vskip ! ?cur-box
           cur +vskip @ cur box vskip c! cur box resized ]:
        :[ ?cur-box:0 cur box vskip c@ dup cur +vskip ! ]:
        9 scale-act new   TT" vskip"        hscaler new
      0 :[ cur +borderw ! ?cur-box
           cur +borderw @ cur box borderw c! cur box resized ]:
        :[ ?cur-box:0 cur box borderw cx@ dup cur +borderw ! ]:
        #18 scale-act new TT" border"       hscaler new
        hscaler with  #-9 offset !  ^ endwith
        0 1 *fill 2dup glue new
    4 vabox new ;

: >hfbox  ( o flag -- o o +do -do )
    >r 1 habox new r@ 0= IF flipbox THEN dup
    r> flipper ;
: box-setting ( -- o )
    box-low    -1 >hfbox s" Low"         topindex new >r
    box-detail  0 >hfbox s" Details"     topindex new >r
    2 habox new 2 borderbox :notshadow noborderbox
    r> r> swap 2 harbox new swap
    2 vabox new vfixbox ;

: box-attr! ( flag attr -- ) >r
    cur box attribs c@ swap
    IF  r> or  ELSE  r> invert and  THEN
    cur box attribs c! cur box resized ;
: box-attr@ ( attr -- flag )
    cur box attribs c@ and 0<> ;

: boxes ( -- o )
    backing new D[ \ backing noback on
      0 TS[ cur +boxmode  !  rebox ][
          ?cur-box:0
          cur box self ?hbox dup cur +boxmode !
          cur box attribs c@ $F0 and cur +noborder ! ]TS
          TT" toggle horizontal/vertical box"
          s" horizontal" tbutton new
      0 TS[ cur +activate !  rebox ][
          ?cur-box:0
          cur box self ?abox dup cur +activate ! ]TS
          TT" toggle single active object/all objects active"
          s" activate" tbutton new
      0 TS[ cur +radio    !  rebox ][
          ?cur-box:0
          cur box self ?rbox dup cur +radio    ! ]TS
          TT" toggle radio button behavior"
          s" radio"    tbutton new
      0 TS[ cur +tabbing  !  rebox ][
          ?cur-box:0
          cur box self ?tbox dup cur +tabbing  ! ]TS
          TT" toggle tabbing the box (all widgets equal size)"
          s" tabbing"  tbutton new
      0 TS[ cur +tabular  !  rebox ][
          ?cur-box:0
          cur box self ?table dup cur +tabular ! ]TS
          TT" toggle tabular box (table in outer box)"
          s" tabular"  tbutton new
      0 TS[ dup cur +hfixbox  !  :hfix box-attr!
         ][ ?cur-box:0 :hfix box-attr@ dup cur +hfixbox !
         ]TS TT" horizontal size fixed to minimum"
         s" hfixbox"   tbutton new
      0 TS[ dup cur +vfixbox  !  :vfix box-attr!
         ][ ?cur-box:0 :vfix box-attr@ dup cur +vfixbox !
         ]TS TT" vertical size fixed to minimum"
         s" vfixbox"   tbutton new
      0 TS[ dup cur +flipbox  !  :flip box-attr!
         ][ ?cur-box:0 :flip box-attr@ dup cur +flipbox !
         ]TS TT" toggle show/hide box"
         s" flipbox"   tbutton new
\      0 TS[ dup cur +rzbox  !  :resized box-attr!
\         ][ ?cur-box:0 :resized box-attr@ dup cur +rzbox !
\         ]TS TT" toggle dump resize behavior"
\         s" rzbox"   tbutton new
    8 vabox new
    box-setting
        s" box name:" text-label new
        t" " 0 ST[ text@ cur cur-box-name update-name ]ST
          textfield new dup cur bind cur-box-edit
        s" display name:" text-label new
        t" " 0 ST[ cur cur-dpy self IF
                  text@ cur cur-dpy with
                      descriptors name $! endwith
              THEN ]ST
          textfield new dup cur bind cur-dpy-edit
    4 vabox new
    3 vabox new panel 2 borderbox hfixbox vfixbox
    dup cur bind status  ]D
    0 1 *filll 2dup glue new
    2 vabox new ;

\ single objexts

: +descs ( o class -- )
    >r bind cur-descs
    all-descs self cur-descs bind next
    cur-descs self bind all-descs
    cur-descs null r> new,
    dup cur-descs assign ;

: >vfbox  ( o flag -- o o +do -do )
    >r 1 vabox new r@ 0= IF flipbox THEN dup
    r> flipper ;
: 0fill ( -- o )  0 1 *fill 0 1 *fill glue new ;

include theseus-save.fs

\ groups and entities

Variable entities

: entity, ( -- estart )
    here entities @ A, entities ! here
    BEGIN  source >in @ /string -trailing nip  WHILE
        ' >body A,
    REPEAT  0 A, ;

: show-field ( -- )
    0 bind edit-string
    0 bind code-string
    0 bind code2-string
    0 bind name-string
    cur back self backing with
        0 1 *fill 0 1 *fill glue new
        cur-descs self IF  cur-descs edit-field swap 2  ELSE  1  THEN
        vabox new  assign resized
    endwith changed ;

forward new:dialog

: ?cur-box  ( -- )
    cur self IF  cur box self  ELSE  0  THEN  0= IF
        new:dialog cur pane resized
    THEN ;

: new-entity ( addr -- n )
    cell+ 0 >r
    BEGIN
        dup @ WHILE
        dup >r @ @ new, r> cell+ r> 1+ >r
    REPEAT
    drop r> ;

: ?entity ( -- )
    ?cur-box  cur box self 0= IF  drop rdrop  THEN ;
: +entity ( o class -- )  >r
    dup +object
    cur bind widget
    r> cell+  show-field drop  changed ;

: make-entity ( addr -- )  ?entity
    dup >r  new-entity
    descriptors new r@ @ @ +descs
    r> +entity ;

: make-font-entity ( addr -- )  ?entity
    dup >r  new-entity
    font-descriptors new r@ @ @ +descs
    r> +entity ;

: make-ref-entity ( addr -- )  ?entity
    dup >r  new-entity
    referred-descs new r@ @ @ +descs
    r> +entity ;

: make-edit-entity ( addr -- )  ?entity
    dup >r  new-entity
    font-descriptors new r@ @ @ +descs  $40 setup-edit
    r> +entity ;

: make-component-entity ( -- )
    component-des new component-des with
        s" no-comp" s" " assign
        null endwith dup +object cur bind widget show-field ;

: make-dentity ( addr -- )  ?entity
    dup >r new-entity
    descriptors new r@ @ @ +descs
    displays with  cross new 1 habox new assign  ^ endwith
    r> +entity ;

: make-ventity ( addr -- )  ?entity
    dup >r new-entity
    descriptors new r@ @ @ +descs
    displays with  cross new 1 vabox new \ rzbox
      assign  ^ endwith
    dup asliderview new +object
    cur bind widget
    r> cell+  show-field drop ;

: make-hsentity ( addr -- )  ?entity
    dup >r  new-entity
    descriptors new r@ @ @ +descs  1 hasbox new
    r> +entity ;

: make-vsentity ( addr -- )  ?entity
    dup >r  new-entity
    descriptors new r@ @ @ +descs  1 vasbox new
    r> +entity ;

Variable #entities

: group  #entities off  : [defined] discard-sinline [IF] discard-sinline [THEN] ;
: endgroup  postpone 0fill  #entities @ 1+ postpone Literal
    & habox @  postpone ALiteral postpone new,
    postpone panel postpone ; ; immediate

: (entity ( addr u -- ) simple new -rot button new ;

[defined] lastdes [IF]
    : lastdes-reset  $80 lastdes ! ;
[ELSE]
    : lastdes-reset ;
[THEN]

: entity ( -- )
    postpone AHEAD  entity, >r  lastdes-reset postpone THEN
    postpone 0
    postpone :[  r> postpone ALiteral postpone make-entity
    postpone ]:  postpone (entity
    1 #entities +!
; immediate

: font-entity ( -- )
    postpone AHEAD  entity, >r  lastdes-reset postpone THEN
    postpone 0
    postpone :[  r> postpone ALiteral postpone make-font-entity
    postpone ]:  postpone (entity
    1 #entities +!
; immediate

: ref-entity ( -- )
    postpone AHEAD  entity, >r  lastdes-reset postpone THEN
    postpone 0
    postpone :[  r> postpone ALiteral postpone make-ref-entity
    postpone ]:  postpone (entity
    1 #entities +!
; immediate

: edit-entity ( -- )
    postpone AHEAD  entity, >r  lastdes-reset postpone THEN
    postpone 0
    postpone :[  r> postpone ALiteral postpone make-edit-entity
    postpone ]:  postpone (entity
    1 #entities +!
; immediate

: component-entity ( -- )
    postpone 0
    postpone :[  postpone make-component-entity
    postpone ]:  postpone (entity
    1 #entities +!
; immediate

: dentity ( -- )
    postpone AHEAD  entity, >r  lastdes-reset postpone THEN
    postpone 0
    postpone :[  r> postpone ALiteral postpone make-dentity
    postpone ]:  postpone (entity
    1 #entities +!
; immediate

: ventity ( -- )
    postpone AHEAD  entity, >r  lastdes-reset postpone THEN
    postpone 0
    postpone :[  r> postpone ALiteral postpone make-ventity
    postpone ]:  postpone (entity
    1 #entities +!
; immediate

: hsentity ( -- )
    postpone AHEAD  entity, >r  lastdes-reset postpone THEN
    postpone 0
    postpone :[  r> postpone ALiteral postpone make-hsentity
    postpone ]:  postpone (entity
    1 #entities +!
; immediate

: vsentity ( -- )
    postpone AHEAD  entity, >r  lastdes-reset postpone THEN
    postpone 0
    postpone :[  r> postpone ALiteral postpone make-vsentity
    postpone ]:  postpone (entity
    1 #entities +!
; immediate

\ object description:

group buttons:
s" Button"       font-entity button        simple-des string-des
s" LButton"      font-entity lbutton       simple-des string-des
s" Icon-Button"  font-entity icon-button   simple-des icon-des string-des
s" Icon"         font-entity icon-but      simple-des icon-des
s" Big-Icon"     font-entity big-icon      simple-des icon-des string-des
s" Tri-Button"   entity tributton          simple-des tri-des
endgroup

group toggles:
s" Toggle"       font-entity tbutton       toggle-des string-des
s" Radio"        font-entity rbutton       toggle-des string-des
s" Flip"         font-entity flipbutton    toggle-des string-des
s" Togglebutton" font-entity togglebutton  toggle-des string-des text-des
s" Icon"         font-entity toggleicon    toggle-des 2icon-des
s" Flip-Icon"    font-entity flipicon      toggle-des icon-des
s" Iconbutton"   font-entity ticonbutton   toggle-des 2icon-des string-des
s" Topindex"     ref-entity topindex       index-des string-des
endgroup

group fields:
s" Text"            font-entity textfield          text-des stroke-des
s" Infotext"        font-entity infotextfield      text-des stroke-des string-des
s" Tab-Infotext"    font-entity tableinfotextfield text-des stroke-des string-des
s" Number"          font-entity textfield          number-des nstroke-des
s" Infonumber"      font-entity infotextfield      number-des nstroke-des string-des
s" Tab-Infonumber"  font-entity tableinfotextfield number-des nstroke-des string-des
s" Float"           font-entity textfield          float-des fstroke-des
s" Infofloat"       font-entity infotextfield      float-des fstroke-des string-des
s" Tab-Infofloat"   font-entity tableinfotextfield float-des fstroke-des string-des
endgroup

group slides:
s" Hslider"         entity hslider          slider-des slider-code
s" Hslider0"        entity hslider0         slider-des slider-code
s" Hscaler"         font-entity hscaler     scaler-des scaler-code
s" Vslider"         entity vslider          slider-des slider-code
s" Vslider0"        entity vslider0         slider-des slider-code
s" Vscaler"         font-entity vscaler     scaler-des scaler-code
endgroup

group labels:
0 :[ & hbox @ cur box class?
     cur pane with IF vline ELSE hline THEN endwith
     +object ]: simple new s" Line"
     button new [ 1 #entities +! ]
s" Label"
  font-entity text-label       string-des
s" Icon"
  entity icon             icon-des
endgroup

group sizer:
s" Hrtsizer"  hsentity hrtsizer
s" Hsizer"    hsentity hsizer
s" Hxrtsizer" hsentity hxrtsizer
s" Vrtsizer"  vsentity vrtsizer
s" Vsizer"    vsentity vsizer
s" Vxrtsizer" vsentity vxrtsizer
endgroup

\ object creator

group menues:
s" Menu-Title"  font-entity menu-title       menu-des string-des
s" Info-Menu"   font-entity info-menu        menu-des string-des
s" Sub-Menu"    font-entity sub-menu         menu-des string-des
s" Menu-Entry"  font-entity menu-entry       action-des string-des
endgroup

group displays:
s" Viewport"      ventity viewport        step-des viewport-des
s" HViewport"     ventity hviewport       step-des viewport-des
s" VViewport"     ventity vviewport       step-des viewport-des
s" Backing"       dentity backing         display-des
s" Doublebuffer"  dentity doublebuffer    display-des
s" Clipper"       dentity clipper         display-des
s" Beamer"        dentity beamer          beam-des display-des
endgroup

group glues: ( -- o )
s" Glue"     entity glue             hglue-des vglue-des
s" HGlue"    entity *hglue           hglue-des
s" VGlue"    entity *vglue           vglue-des
s" Rule"     entity rule             hglue-des vglue-des
s" HRule"    entity hrule            hglue-des
s" VRule"    entity vrule            vglue-des
s" Topglue"  entity topglue          topglue-des
endgroup

group complex:
s" Canvas"        entity canvas        canvas-des click-des hglue-des vglue-des
s" OpenGL Canvas" entity glcanvas      glcanvas-des click-des hglue-des vglue-des

s" Terminal"      font-entity terminal term-des
s" Editor"        edit-entity stredit  edit-des

s" Component"     component-entity
endgroup

: classes ( -- o )
    backing new D[
    displays: 0 >vfbox s" Displays"    topindex new >r
    complex:  0 >vfbox s" Complex"     topindex new >r
    glues:    0 >vfbox s" Glues"       topindex new >r
    labels:   0 >vfbox s" Labels"      topindex new >r
    sizer:    0 >vfbox s" Sizer"       topindex new >r
    menues:   0 >vfbox s" Menues"      topindex new >r
    slides:   0 >vfbox s" Sliders"     topindex new >r
    fields:   0 >vfbox s" Text Fields" topindex new >r
    toggles:  0 >vfbox s" Toggles"     topindex new >r
    buttons: -1 >vfbox s" Buttons"     topindex new >r
    #10 vabox new  2 borderbox :notshadow noborderbox
    r> r> r> r> r> r> r> r> r> r>
    topglue new
    #11 harbox new swap
    2 vabox new
    0 S[ ?cur-box  true  addbox ]S s" hbox"       button new
    0 1 *fil 2dup glue new
    0 S[ ?cur-box  false addbox ]S s" vbox"       button new
    3 vabox new 1 vabox new panel 2 borderbox
    2 habox new
    vfixbox ]D ;

\ load file

include theseus-load.fs

\ modes

: find-box ( o -- o' )
    BEGIN
        dup all-descs find-object WHILE
        dup 'nil <> WHILE
        gadget with widgets self endwith
    REPEAT THEN ;

: up-box ( o -- up-o )
    widget with
        & displays @ class?
        IF  & viewport @ class?
            IF    parent parent parent parent self
            ELSE  parent self  THEN
        ELSE  self  THEN
    endwith ;

: go-up ( -- )
    cur box self cur topbox self = ?EXIT
    cur box self cur menubox self = ?EXIT
    cur box parent self up-box
    cur bind box <rebox> redraw ;
: go-down ( -- )
    cur box childs self find-box
    dup 'nil <>
    over widget with & combined @ class? endwith and
    IF  cur bind box <rebox> redraw  ELSE  drop  THEN ;
: go-right ( -- )
    cur box widgets self find-box
    dup 'nil <> IF  cur bind box  ELSE  drop  THEN
    <rebox> redraw ;
: go-left ( -- )
    cur box self >r
    cur box parent self >child
    find-box
    BEGIN  dup r@ <> WHILE
        dup 'nil <> IF  cur bind box  ELSE 2drop rdrop EXIT THEN
        cur box widgets self find-box
    REPEAT  drop   <rebox> redraw  rdrop ;

forward find-object

: ?backing ( -- o )
    ^ backing with
        & backing @ class?
        IF
            trans child self find-object
            ?dup 0= IF
                child self >child
            THEN
        ELSE
            ^
        THEN
    endwith ;

: find-object ( x y o -- x y o/0 )
    combined with
        2dup inside? IF
            ^ all-descs find-object
            IF
                ?backing
            ELSE
                & sliderview @ class?
                IF  sliderview inner self op!  THEN
                & combined @ class?
                IF
                    childs self >r
                    BEGIN
                        r@  recurse
                        dup 0=  WHILE
                        drop r> widget with widgets self endwith
                        dup >r 'nil = UNTIL  0  THEN
                    rdrop
                    ?dup 0= IF  ^  THEN
                ELSE
                    ?backing
                THEN
            THEN
        ELSE
            0
        THEN
    endwith ;

: >object ( x y b n -- x y o )
    2drop 2dup cur inside
    0= IF  drop 2drop rdrop  false  EXIT  THEN
    find-object  dup 0= IF  drop  rdrop false  EXIT  THEN ;

: (click-edit ( x y b n -- flag )  >object
    dup all-descs find-object 0=
    over widget with & combined @ class? endwith and
    IF
        cur bind box drop  redraw  EXIT
    ELSE
        dup widget with parent self endwith cur bind box  redraw
    THEN
    dup cur widget self = IF  drop 2drop true  EXIT  THEN
    dup all-descs find-object dup 0=
    IF  2drop 2drop false  ( redraw )  EXIT  THEN
    bind cur-descs cur bind widget
    show-field ( redraw ) 2drop true ;
: click-edit ( x y b n -- )
    edit-string self 0= ?EXIT
    edit-string xywh p+ 1 1 p- edit-string dpy transback
    1 2 edit-string dpy dpy clicked
    $FF57 0 edit-string keyed ;
: click-code ( x y b n -- )
    code-string self 0= ?EXIT
    code-string xywh p+ 1 1 p- code-string dpy transback
    1 2 code-string dpy dpy clicked
    $FF57 0 code-string keyed ;    
: click-name ( x y b n -- )
    name-string self 0= ?EXIT
    name-string xywh p+ 1 1 p- name-string dpy transback
    1 2 name-string dpy dpy clicked
    $FF57 0 name-string keyed ;

: click-ecn ( x y b n -- )  over >r (click-edit
    IF
        r@ 1 and IF  click-edit  rdrop  EXIT  THEN
        r@ 2 and IF  click-code  rdrop  EXIT  THEN
        r@ 4 and IF  click-name  rdrop  EXIT  THEN
    THEN
    rdrop ;

: key-edit ( key sh -- ) edit-string self 0= ?EXIT
    edit-string keyed ;
: key-code ( key sh -- ) code-string self 0= ?EXIT
    code-string keyed ;
: key-name ( key sh -- ) name-string self 0= ?EXIT
    name-string keyed ;

widget ptr cut-stack

: up-cut ( o -- up-o )
    widget with
        & displays @ parent class?
        IF  & viewport @ parent class?
            IF    parent parent parent parent self
            ELSE  parent self  THEN
        ELSE  self  THEN
    endwith ;

: remove-me ( o -- )
    widget with
        ^ parent self combined with remove
            n @ 0= IF  cross new 'nil add resized  THEN
        endwith
        cut-stack self bind widgets
        ^ F bind cut-stack
    endwith ;

: is-parent? ( o1 o2 -- flag )
    dup 0= IF  2drop false  EXIT  THEN
    BEGIN  2dup <>  WHILE
        widget with parent self endwith
        dup 0= UNTIL  2drop false  EXIT  THEN
    2drop true ;

: click-cut ( x y o -- )
    dup widget with & displays @ parent class? endwith
    IF  up-cut
    ELSE  dup widget with & cross @ class? endwith
        IF  widget with parent self up-cut endwith  THEN
    THEN
    dup cur topbox self  = IF  drop 2drop  EXIT  THEN
    dup cur menubox self = IF  drop 2drop  EXIT  THEN
    dup cur box self is-parent?
    IF  cur topbox self cur bind box  THEN
    dup cur widget self is-parent? IF
        0 cur bind widget 0 bind cur-descs show-field
    THEN
    remove-me  2drop ;

: click-copy ( o -- )  drop ;
: click-paste ( x y o -- )
    cut-stack self 0= IF  drop 2drop  EXIT  THEN
    combined with
        cut-stack self
        cut-stack widgets self F bind cut-stack
        o@ & cross @ =
        IF
            'nil parent self op!
        ELSE
            ^ all-descs find-object
            IF  xywh p2/ p+
                & vbox @ parent class?
                IF  nip 2 pick  ELSE  drop 3 pick  THEN
                < IF  widgets self  ELSE  self  THEN
                parent self op!
            ELSE
                & vbox @ class?
                IF
                    over childs self
                    n @ 0 ?DO  widget with
                        dup xywh p2/ p+ nip <
                        IF  self true  ELSE  widgets self false  THEN
                        endwith ?LEAVE  dup 'nil = ?LEAVE  LOOP  nip
                ELSE
                    2 pick childs self
                    n @ 0 ?DO  widget with
                        dup xywh p2/ p+ drop <
                        IF  self true  ELSE  widgets self false  THEN
                        endwith ?LEAVE  dup 'nil = ?LEAVE  LOOP  nip
                THEN
            THEN
        THEN
        add
        childs self @ & cross @ =
        IF  childs self remove  THEN
        !resized resized
    endwith 2drop ;

: click-ccp ( x y b n -- )  over >r  >object
    r@ 1 and  IF  click-cut   rdrop  EXIT  THEN
    r@ 2 and  IF  click-paste rdrop  EXIT  THEN
    r@ 4 and  IF  click-paste rdrop  EXIT  THEN
    drop rdrop ;

: click-try  vabox :: clicked ;
: key-try  vabox :: keyed ;

: click-all ( x y b n -- )
    kbshift @ 4 and  IF  click-try  EXIT THEN
    kbshift @ 1 and  IF  click-ccp  EXIT THEN
                         click-ecn ;

[defined] x11 [IF]
also xconst
Create do-edit ' click-all A, ' key-edit A, XC_crosshair ,
previous
[THEN]

[defined] win32 [IF]
also win32api
Create do-edit ' click-all A, ' key-edit A,  IDC_IBEAM ,
previous
[THEN]

do-edit do-it !

also dos

Variable $acc
: +$ ( addr u -- ) $acc $+! ;

: auto-save-add ( --- )
    cur file-name @  IF  cur file-name $@ '/' -scan +$  THEN
    s" .#" +$
    cur file-name @  IF  cur file-name $@ 2dup '/' -scan nip /string +$  THEN
    base push hex cur self dup $10 >> + $FFFF and dup $8 >> + $FF and
    0 <# '#' hold #S '-' hold #> +$ ;
: auto-save-name ( -- addr u )
    s" " $acc $!  auto-save-add
    $acc $@ ;

: auto-save-minos
    auto-save-name dump-file
    cur save-state @ IF  1 cur save-state !  THEN ;

: run-minos
    cur save-state @  IF  auto-save-minos  THEN
    s" xbigforth '" $acc $!
    cur save-state @  cur file-name @ 0= or
    IF    auto-save-add
    ELSE  cur file-name $@ +$  THEN  s" '" +$
[defined] win32 [ 0= ] [IF]
    s"  &" +$
[THEN]
    $acc $@ drop system drop ;

Create quote 1 c, '"' c,

: mod-minos
    s" Create Module:" s" " s" *.fm"
    0 S[ path+file
         '.' -scan 1-  2dup 2dup '/' -scan nip /string
         s" theseus-test" dump-file
         s" " $acc $!
         s' xbigforth -e "' +$ quote count +$ S" minos openw forth " +$
         s"  false to script? " +$
         s"  module " +$ 2dup +$
         s"  include theseus-test main: main ; MODULE;" +$
         s"  m' " +$ +$ s"  savemod " +$ +$
         s'  bye"' +$ quote count +$
         $acc $@ drop system drop ]S fsel-dialog ;

Variable ren-files
Variable auto-save-file
         
: rename-old ( addr u -- )
    ren-files $! s" ~" ren-files $+!
    ren-files $@ 1- ren-files $@ rename-file drop ;

: rename-save ( addr u -- )
    ren-files $! s" +" ren-files $+!
    ren-files $@ ren-files $@ 1- rename-file drop ;

: set-title ( -- )
    cur with
        s" Theseus: " window title!  file-name $@ title+!
    endwith ;

: try-save ( -- )
    s" +" cur file-name $+!
    :[ cur file-name $@ dump-file ]: catch
    cur file-name $@len 1- cur file-name $!len
    0= IF
	cur file-name $@ rename-old
	cur file-name $@ rename-save
	auto-save-file dup @ IF $@ delete-file drop ELSE drop THEN
	cur save-state off
    THEN ;

: save-minos...
    s" Save as:" s" " s" *.m"
    cur self S[ ^ bind cur ( s" .m" ?suffix )
                auto-save-name auto-save-file $!
                path+file
                cur file-name $! set-title
                try-save ]S
    fsel-dialog ;

: save-minos
    cur file-name @ 0= IF  save-minos...  EXIT  THEN
    try-save set-title ;

: append-modes ( -- o )
   0 false T[ ['] addfirst F IS +object ['] cur-box-dpy F IS cur-dpy ][ ]T
               TT" Add first in box"
               icon" icons/head"        flipicon new
   0 true  T[ ['] addlast  F IS +object ['] cur-box-dpy F IS cur-dpy ][ ]T
               TT" Add last in box"
               icon" icons/tail"        flipicon new
   0 false T[ ['] addbefore F IS +object ['] cur-obj-dpy F IS cur-dpy ][ ]T
               TT" Add before current object"
               icon" icons/before"        flipicon new
   0 false T[ ['] addafter  F IS +object ['] cur-obj-dpy F IS cur-dpy ][ ]T
               TT" Add after current object"
               icon" icons/after"        flipicon new
   4 varbox new vfixbox ;

: navigation ( -- o )
   0fill 0 ['] go-up simple new
          TT" Up in hierarchy"
          1        tributton new 0fill 3 habox new
   0fill 0 ['] go-left simple new
          TT" Previous in hierarchy"
          0        tributton new
   2skip 0 ['] go-right simple new
          TT" Next in hierarchy"
          2        tributton new 0fill 5 habox new
   0fill 0 ['] go-down simple new
          TT" First child in hierarchy"
          3        tributton new 0fill 3 habox new
   3 vabox new vfixbox ;

: file-io ( -- o )
   0 ['] load-minos simple new
          TT" Load file..."
          icon" icons/load"        icon-but new
   0 ['] save-minos simple new
          TT" Save"
          icon" icons/save"        icon-but new
   0 ['] run-minos  simple new
          TT" Run application"
          icon" icons/run"        icon-but new
   0 ['] mod-minos  simple new
          TT" Save as module"
          icon" icons/mod"        icon-but new
   4 vabox new vfixbox ;

: modes ( -- o )
    backing new D[
    [defined] edit-modes [IF]
        edit-modes
        hline
    [THEN]
    append-modes
    hline
    navigation
    hline
    file-io
    [defined] edit-modes [IF]  7  [ELSE]  5  [THEN]
    vabox new 2 borderbox ]D
    0fill
    2 vabox new hfixbox ;

: designer-file ( -- )
    s" Load:" s" " s" *.m"
    cur self S[ ^ bind cur
    cur callwind self bind term
    designer open-file ]S fsel-dialog ;

: file-menu ( -- o )
    ^ S[ load-minos    ]S s" Load file..."      menu-entry new
    ^ S[ save-minos... ]S s" Save as..."        menu-entry new
    ^ S[ run-minos     ]S s" Run application"   menu-entry new
    ^ S[ mod-minos     ]S s" Save as module..." menu-entry new
    hline
    ^ S[ cur callwind self bind term designer open ]S
                       s" New designer"      menu-entry new
\    ^ S[ designer-file ]S
\                      s" Load to new designer..."    menu-entry new
    hline
    ^ S[ cur close ]S           s" Quit"              menu-entry new
    8 vabox new 2 borderbox ;

: edit-menu ( -- o )
    ^ S[ new:dialog cur pane !resized ]S      s" New Dialog"      menu-entry new
    ^ S[ new:menu-window cur pane !resized ]S s" New Menu Window" menu-entry new
    2 vabox new 2 borderbox ;

[defined] gpl-about 0= [IF] include gpl-about.m [THEN]
include theseus-help.m

also dos

: help-menu ( -- o )
[defined] win32 [IF]
    ^ S[ 0" help/theseus.html" system drop ]S
[ELSE]
    ^ S[ 0" ${BROWSER-/usr/local/lib/bigforth/help/netscape.sh} http://www.jwdt.com/~paysan/help/theseus.html >/dev/null 2>/dev/null &" system drop ]S
[THEN]
    s" Using Theseus" menu-entry new
    hline
    ^ S[ minos-about open ]S s" About Theseus" menu-entry new
    3 vabox new 2 borderbox ;

previous

: designer-menu ( -- o )
    file-menu s"  File " menu-title new
    edit-menu s"  Edit " menu-title new
    0 1 *fill 2dup rule new
    help-menu s"  Help " menu-title new
    4 hbox new 2 borderbox vfixbox ;

designer implements
    : init  super init +activate on ;
    : open-file ( -- )
        open load-minos ;
    : close  save-state @ 0=  IF  super close  EXIT  THEN
	[defined] NewTask [IF] $8000 $3000 NewTask activate [THEN]
        s" Data may have been modified!"
        s" Really want to close?"
        2 s" No" s" Yes" 2 1 alert
        1 = IF  save-state off super close  THEN ;
    : click ( x y b n -- )  bind-cur super click ;
    : keyed ( key sh -- ) bind-cur super keyed ;
    : inside ( x y -- o flag )
        topbox self IF
            2dup topbox  inside? IF  2drop topbox  self true  EXIT  THEN
        THEN
        menubox self IF
            2dup menubox inside? IF  2drop menubox self true  EXIT  THEN
        THEN
        2drop 0 false ;
    : open ( -- )
        screen self new dup F bind cur op!
        term self bind callwind
        designer-menu
        classes
        modes
        1 1 viewport new DS[
            ^ cur bind pane
            0 1 *filll 2dup rule new
            dup rule with $D assign endwith
            dup cur bind end-rule
            1 vabox new ]DS
        1 vabox new 2 borderbox panel 2 habox new
        2 vabox new
        boxes
        2 habox new
        vrtsizer new
        1 1 viewport new dup cur bind back
        DS[ 2fill 2fill minos-icon
            [ also minos ] icon new [ previous ]
            2fill 3 habox new 2fill 3 vabox new ]DS
        2 vasbox new dup >r
        2 0 modal new 0 hskips 0 vskips
        s" Theseus" assign
        r> vasbox with xN 7 * xS #16 * + dpy xrc hM @ 6 * + xS 3 * 2/ + vsize !
            resized endwith
        minos-win set-icon
        resized show ;
class;

\ include theseus-try.fs

\ : run-here ( -- )
\     s" minos-test" dump-file
\     s" minos-test" included
\     s" marker forget-it dialog open forget-it" evaluate ;

previous previous previous previous previous previous

also -options definitions

: --theseus ( addr u -- )
    2dup dup 2- /string s" .m" str=  IF  included-minos 2
    ELSE  [defined] defers [IF] defers -i [ELSE] --include [THEN] THEN ;
' --theseus IS -i

previous theseus definitions

[defined] VFXForth [IF]
    Module;
    
    also theseus synonym designer designer previous
[ELSE]
    export theseus designer ;

Module;
[THEN]

script? [IF] minos openw forth designer open [THEN]
