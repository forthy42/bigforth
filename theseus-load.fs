\ MINOS file format parser/loader

Vocabulary minos-load

Variable last-file

: parse-string ( addr1 u1 -- addr2 u2 )  dup >r
    source >in @ safe/string 2swap search 0= abort" not found" nip
    source >in @ safe/string rot - dup r> + >in +! ;

: parse-string? ( addr1 u1 -- addr2 u2 flag ) dup >r
    source >in @ safe/string 2swap search 0=
    IF  2drop source >in @ safe/string dup >in +! false
        rdrop EXIT  THEN
    nip source >in @ safe/string rot - dup r> + >in +! true ;

: scan-strings { addr u string }
    BEGIN  addr u parse-string? 0= WHILE
        string $+line refill 0= UNTIL
    ELSE  string $+line  THEN ;

: find-entity ( class -- entity )
    >r 0 0 entities @
    BEGIN  nip nip dup 2@ over r@ = over 0= or  UNTIL
    2drop cell+ rdrop ;

: #classes ( addr -- n )
    0 swap  BEGIN  @+ swap  WHILE  swap 1+ swap  REPEAT  drop ;

: new-desc ( o class -- o )
    >r bind cur-descs
    all-descs self cur-descs bind next
    cur-descs self bind all-descs
    cur-descs make r> new,
    dup cur-descs assign ;

: 'entity, ( "name" -- )  >in @ ' >body find-entity swap >in !
    Create A, ;

: entity: ( "name" -- ) 'entity,
DOES> ( class1 .. classn -- o )
    @ >r  r@ cell+ #classes
    descriptors new
    r> @ @ new-desc ;

: fentity: ( "name" -- ) 'entity,
DOES> ( class1 .. classn -- o )
    @ >r  r@ cell+ #classes
    font-descriptors new
    r> @ @ new-desc ;

: ref-entity: ( "name" -- ) 'entity,
DOES> ( class1 .. classn -- o )
    @ >r  r@ cell+ #classes
    referred-descs new
    r> @ @ new-desc ;

stroke-des ptr last-stroke

: tentity: ( "name" -- ) 'entity,
DOES> ( class1 .. classn -- o )
    @ >r  2 pick stroke-des with & nstroke-des @ class? 0= endwith
    IF
        r> cell- @ @ @ cell+ >r
    THEN
    r@ cell+ #classes
    font-descriptors new
    r> @ @ new-desc ;

: dentity: ( "name" -- ) 'entity,
DOES> ( class1 .. classn -- o )  @ >r
    display-des new  r@ cell+ #classes
    descriptors new
    r> @ @ new-desc ;

: ventity: ( "name" -- ) 'entity,
DOES> ( class1 .. classn -- o )  @ >r
    step-des new step-des with assign ^ endwith
    viewport-des new  r@ cell+ #classes
    descriptors new
    r> @ @ new-desc ;

: tgentity: ( "name" -- ) 'entity,
DOES> ( class1 .. classn -- o )
    @ >r  topglue-des new r@ cell+ #classes
    descriptors new
    r> @ @ new-desc ;

: ?(name) ( addr u -- flag )
    over c@ '(' = >r + 1- c@ ')' = r> and ;

also minos-load definitions

: ^  ;
: ^^ cur pane self ;
: with  drop ;
: endwith ;
: ( ')' parse 2drop ; immediate
: dup dup ;
: cross  postpone cross ; immediate

: M:
    S" " menu-des new menu-des with assign
        parse-name content $!  parse-name 2drop
        self endwith ;
: S[  drop
    S" " simple-des new simple-des with assign
        s"  ]S ( MINOS ) " content scan-strings
        0 typ !
        self endwith ;
: R[  drop
    S" " simple-des new simple-des with assign
        s"  ]R ( MINOS ) " content scan-strings
        1 typ !
        self endwith ;
: M[  drop
    S" " simple-des new simple-des with assign
        s"  ]M ( MINOS ) " content scan-strings
        2 typ !
        self endwith ;
: T[   nip S" " S" "
    toggle-des new toggle-des with assign
        s"  ][ ( MINOS ) " content  scan-strings
        s"  ]T ( MINOS ) " content2 scan-strings
        flag !  0 typ ! self
    endwith ;
: TV[   drop S" " S" "
    toggle-des new toggle-des with assign
        s"  ]T[ ( MINOS ) " content  scan-strings
        s"  ]TV ( MINOS ) " content2 scan-strings
        1 typ ! self
    endwith ;
: TN[   drop S" " S" "
    toggle-des new toggle-des with assign
        s"  ]T[ ( MINOS ) " content  scan-strings
        s"  ]TN ( MINOS ) " content2 scan-strings
        2 typ ! self
    endwith ;
: TS[   drop S" " S" "
    toggle-des new toggle-des with assign
        s"  ][ ( MINOS ) "  content  scan-strings
        s"  ]TS ( MINOS ) " content2 scan-strings
        3 typ ! self
    endwith ;
: TB[   drop S" " S" "
    toggle-des new toggle-des with assign
        s"  ]T[ ( MINOS ) " content  scan-strings
        s"  ]TB ( MINOS ) " content2 scan-strings
        4 typ ! self
    endwith ;
: CK[  drop
    S" " click-des new click-des with assign
        s"  ]CK ( MINOS ) " content scan-strings
        self endwith ;

: ST[   drop S" "
    stroke-des new stroke-des with assign
        s"  ]ST ( MINOS ) " content scan-strings self
    endwith ;

: SN[   drop S" "
    nstroke-des new nstroke-des with assign
        s"  ]SN ( MINOS ) " content scan-strings self
    endwith ;

: SF[   drop S" "
    fstroke-des new fstroke-des with assign
        s"  ]SF ( MINOS ) " content scan-strings self
    endwith ;

: CV[
    canvas-des new canvas-des with s" " assign
        s"  ]CV ( MINOS ) " content scan-strings
        self endwith ;

: GL[
    glcanvas-des new glcanvas-des with s" " assign
        s"  ]GL ( MINOS ) " content scan-strings
        self endwith ;

: CP[ ( o -- o )
    drop component-des new component-des with
        s"  ]CP ( MINOS ) " parse-string cparam $!
        s"  new"            parse-string cname  $!
        null endwith ;

: ]N ( d -- o ) number-des new number-des with assign self endwith ;
also float
: ]F ( f -- o ) float-des new float-des with assign self endwith ;
previous
: ]TERM ( d -- ) term-des new term-des with assign self endwith ;
: SC[ ( o 0 n -- o o )
    scaler-des new scaler-des with assign pos ! drop self endwith
    scaler-code new scaler-code with s" " assign
        s"  ]SC ( MINOS ) " content scan-strings
        self endwith ;
: SC# ( o n -- o )
    over hscaler with offset ! endwith ;

: SL[ ( o p n s -- o o ) 2swap 2drop
    slider-des new slider-des with assign self endwith
    slider-code new slider-code with s" " assign
        s"  ]SL ( MINOS ) " content scan-strings
        self endwith ;

: icon" ( -- o )  '"' parse
    icon-des new icon-des with assign self endwith ;
    
: 2icon" ( -- o )  '"' parse '"' parse 2swap
    2icon-des new 2icon-des with assign self endwith ;

: font" ( o -- o )  '"' parse
    new-font over all-descs find-object
    font-descriptors with
        & font-descriptors @ class?  IF  font!  ELSE  drop  THEN
    endwith ;

: *h: ( i -- )  Create ,
    DOES> @ ( n f i -- )
    hglue-des new hglue-des with assign self endwith ;

: *v: ( i -- )  Create ,
    DOES> @ ( n f i -- )
    vglue-des new vglue-des with assign self endwith ;

0 *h: *hpix     1 *h: *hfil     2 *h: *hfill    3 *h: *hfilll
0 *v: *vpix     1 *v: *vfil     2 *v: *vfill    3 *v: *vfilll

: TRI:
    tri-des new tri-des with assign
        self endwith ;

: :left   0 TRI: ;
: :up     1 TRI: ;
: :right  2 TRI: ;
: :down   3 TRI: ;

: :beamer  beam-des new ;

: flipper ( 0 state -- ) index-des new
    index-des with  fstate !  drop self endwith ;

: C[ ( o -- )  base push hex
    s"  ]C ( MINOS ) " parse-string all-descs find-name
    descriptors with item self endwith
    2dup new-link
    topindex with callback bind called endwith ;

: D[ ( o -- o ) ;
: ]D ( o1 o2 -- o1 )
    swap backing with  assign
        self endwith ;

: DS[ ( o -- o ) ;
: ]DS ( o1 o2 -- o1 )
    swap backing with  assign
        self endwith
    asliderview new ;

: bind ( o "name" -- )
    >r bl word count r@
    all-descs find-object
    ?dup IF  descriptors with set-name endwith
    ELSE     r@ hint-name new bind names  THEN
    rdrop ;

: ^^bind ( o "name" -- o ) dup bind ;

: S" [char] " parse string-des new string-des with assign
    self endwith ;

: X" [char] " parse string-des new string-des with assign
    self endwith ;

: T" [char] " parse text-des new text-des with assign
    self endwith ;

: TT"  dup >r [char] " parse r> action-des with assign-tip
    endwith ;

Variable last-stredit

: (straction ( -- o )
    edit-des new dup last-stredit ! ;

also editor

: setup-edit ( o n -- )  dup last-stredit @ edit-des with assign endwith
    setup-edit ;

previous

fentity: button
fentity: lbutton
fentity: icon-button
fentity: icon-but
fentity: big-icon
entity: tributton

fentity: tbutton
fentity: rbutton
fentity: flipbutton
fentity: togglebutton
fentity: toggleicon
fentity: flipicon
fentity: ticonbutton
ref-entity: topindex

tentity: textfield
tentity: infotextfield
tentity: tableinfotextfield

fentity: stredit
fentity: terminal

entity: hslider
entity: hslider0
fentity: hscaler
entity: vslider
entity: vslider0
fentity: vscaler

entity: hrtsizer
entity: hsizer
entity: hxrtsizer
entity: vrtsizer
entity: vsizer
entity: vxrtsizer

fentity: menu-title
fentity: info-menu
fentity: sub-menu
fentity: menu-entry

fentity: text-label
entity: icon

entity: glue
entity: *hglue
entity: *vglue
entity: rule
entity: hrule
entity: vrule

entity: canvas
entity: glcanvas

ventity: viewport
ventity: hviewport
ventity: vviewport
dentity: backing
dentity: clipper
dentity: doublebuffer
dentity: beamer

tgentity: topglue

: new ;

previous Theseus definitions

\ Minos load

: add-stream ( addr o -- )
    over HLock over $@ rot codeedit with
        thisline @ DisposHandle
        thisline off rows off $40 cols ! add-lines
        0 0 at edifile off
    endwith HUnLock ;

also dos

: search-dumpstart ( resources -- ) >r
    BEGIN  refill  WHILE
        source s" ( [methodstart] ) " search nip nip
        IF
            ')' parse 2drop 1 >in +!
            s" " cur resources methods-content $!
            s"  ( [methodend] ) "
            r@ resource:dialog with methods-content endwith
            dup >r scan-strings r>
            r@ resource:dialog with methods-edit self endwith
            add-stream  THEN
        source s" ( [dumpstart] )" search nip nip
    UNTIL  THEN  rdrop ;

: dump-end? ( -- flag )
    source s" ( [dumpend] )" search nip nip ;

: strip-names ( -- )
    all-descs self
    BEGIN  descriptors with name $@ ?(name)
           IF  S" " name $!  THEN
           next self content @ @ endwith  0= UNTIL  drop ;

: prefix? ( addr1 u1 addr2 u2 -- flag )
    tuck >r >r min r> r> compare 0= ;

: postfix? ( addr1 u1 addr2 u2 -- flag )
    tuck >r >r over swap - 0 max safe/string r> r> compare 0= ;

: new:dialog ( -- )
    cur pane with minos:dialog endwith
    cur end-rule parent self
    combined with cur end-rule self add endwith ;

: new:menu-window ( -- )
    cur pane with minos:menu-window endwith
    cur end-rule parent self
    combined with cur end-rule self add endwith ;

: !class-file ( -- )
    last-file @
    IF  last-file $@ file-status nip 0=
        IF  last-file $@ resource:dialog class-file $!  THEN
        last-file $off  THEN ;
        
: !implementation-file ( -- )
    last-file @
    IF  last-file $@ file-status nip 0=
        IF  last-file $@ resource:dialog implementation-file $!  THEN
        last-file $off  THEN ;
        
: create-dialog ( -- )
    bl word drop bl word drop
    new:dialog cur resources self
    bl word count
    rot resource:dialog with
        name-field assign  shown off  !class-file
    endwith ;

: create-menu-window ( -- )
    bl word drop bl word drop
    new:menu-window cur resources self
    bl word count
    rot resource:menu-window with
        name-field assign shown off !class-file
    endwith ;

: add-vars ( -- )
    ')' parse 2drop 1 >in +!
    s" " cur resources var-content $!
    s"  ( [varend] ) "
    cur resources var-content dup >r scan-strings
    r> cur resources var-edit self add-stream ;

: set-title ( o -- ) >r  s" "
    BEGIN  2drop refill  WHILE  s'  s" ' parse-string? nip nip
        IF  '"' parse true  ELSE  s" " s" class;" source compare 0=  THEN
        UNTIL  THEN
    r> resource:dialog with
        dup IF  title-field assign  ELSE  2drop  THEN
    endwith ;

: set-title' ( o -- ) >r >in @ >r
    s'  s" ' parse-string? nip nip
    IF  rdrop '"' parse
    ELSE  r> >in !
	s'  X" ' parse-string? nip nip
	IF  '"' parse  ELSE  s" "  THEN
    THEN
    r> resource:dialog with
        dup IF  title-field assign  ELSE  2drop  THEN
    endwith ;

: load-dialog ( o -- )
    \ traceall
    resource:dialog with
        topbox self ^ !implementation-file
    endwith >r
    cur bind box
    r@ search-dumpstart
    cur box dpy with
        BEGIN
            refill  WHILE
            dump-end? 0=  WHILE
            interpret
        REPEAT THEN
    endwith
    r@ resource:dialog with add-box >cur  endwith
    r> set-title ;

: read-titles ( -- )
    BEGIN  refill  WHILE
        source s" open" search nip nip
        IF
            bl word count cur resources find-name ?dup
            IF
                resource:dialog with shown on
                    show-state draw endwith
            THEN
        THEN
    REPEAT ;

: create-classes ( -- )
    reenter dup push on
    BEGIN  refill  WHILE
        true CASE
            source s' " included?' postfix?
            source s' s" ' prefix? and
            OF  bl word drop '"' parse last-file $!  ENDOF
            source s" include " prefix?
            OF  bl word drop bl parse last-file $!  ENDOF
            source s" component class " prefix?
            OF  create-dialog  ENDOF
\ FIXME dated alternatives: to be removed for final version
            source s" window class " prefix?
            OF  create-dialog  ENDOF
            source s" vabox class " prefix?
            OF  create-dialog  ENDOF
            source s" menu-window class " prefix?
            OF  create-menu-window  ENDOF
\ FIXME end dated alternatives
            source s" menu-component class " prefix?
            OF  create-menu-window  ENDOF
            source s" ( [varstart] ) " search nip nip
            OF  add-vars  ENDOF
            source s"   : open-app new DF[ " prefix?
            OF  s"   : open-app new DF[ " >in ! drop bl parse
                2dup s" 0" compare
                IF  cur resources default $!  ELSE  2drop  THEN  ENDOF
            source s"   : params   DF[ " prefix?
            OF  s"   : params   DF[ " >in ! drop bl parse
                2dup s" 0" compare
		IF  cur resources default $!  ELSE  2drop  THEN
		cur resources self  set-title'  ENDOF
            source s"     widget 1 " prefix?
            OF  s"     widget 1 " >in ! drop bl parse
                2dup s" 0" compare
                IF  cur resources default $!  ELSE  2drop  THEN  ENDOF
            source s" implements" search nip nip
            OF 
                bl word count cur resources find-name ?dup
                IF  load-dialog  THEN  ENDOF
            source s" : main" prefix?
            OF  read-titles  ENDOF
        ENDCASE
    REPEAT
    cur pane !resized
    cur pane resized
    cur status resized <rebox> <redpy> ;

also minos-load definitions
[defined] Synonym [IF]
    Synonym #! create-classes
[ELSE]
    ' create-classes Alias #!
[THEN]
previous theseus definitions

[defined] VFXForth [IF] -258 Constant open-failed# [THEN]
[defined] bigForth [IF] -1026 Constant open-failed# [THEN]

: included-minos ( addr u -- )  loading on
    [ also float [defined] f-init [IF] ] f-init [ [THEN] previous ]
    2dup cur file-name $!
    Onlyforth minos also minos-load also
    ['] included catch dup open-failed# <> IF  throw 0  THEN
    drop strip-names Onlyforth cur save-state off
    cur with
        s" Theseus: " window title! file-name $@ window title+!
    endwith
    loading off ;

: load-minos ( -- )
    s" Load:" s" " s" *.m"
    cur self S[ ^ bind cur path+file pad place pad count included-minos ]S
    fsel-dialog ;

previous
