\ MINOS descriptor classes

: .d  base push decimal dup 0< IF '- emit THEN '& emit abs . ;

descriptor class descriptors
public:
    ptr next
    cell var content
    cell var name
    cell var number
    method find-object
    method find-name
    method set-name
    method dump-name
    method dump-class
    method dump-ptr
    method create-ptr
class;

descriptors ptr cur-descs
descriptors ptr all-descs

descriptors implements
    : init ( class1 .. classn n -- )
        s" " name $!
        dup 1+ cells content Handle!
        dup content @ !
        0 ?DO  I' I - cells content @ + !  LOOP ;
    : delete-desc ( -- )
        cur-descs self ^ = IF  0 F bind cur-descs  THEN
        F link all-descs
        BEGIN  dup @ >o next self self o> <> WHILE
            dup @ ^ = IF  next self swap ! EXIT  THEN
            @ >o link next o>  REPEAT  drop ;
    : dispose ( -- )  delete-desc
        content HandleOff  name HandleOff  super dispose ;
    : assign ( o -- )
        dup bind item
        content @ @ 1+ 1 ?DO  dup content @ Ith
            descriptor with bind item endwith  LOOP  drop
        item self 0 update-linker ;
    : null ( -- null1 .. nulln )
        content @ @ 1+ 1 ?DO  content @ Ith
            descriptor with null endwith  LOOP ;
    : make ( -- make1 .. maken )
        content @ @ 1+ 1 ?DO  content @ Ith
            descriptor with make endwith  LOOP ;
    : rightcase ( addr1 u1 -- addr2 u2 )
        scratch place
        0 scratch count bounds ?DO
            IF    I c@ dup tolower dup I c! <>
            ELSE  true  THEN
        LOOP  drop scratch count ;
    : edit-field ( -- o )
        0 ST[ text@ >current-name ]ST
        name $@ item self >class" rightcase
        infotextfield new dup F bind name-string
        content @ @ 1+ 1 ?DO  content @ Ith
            descriptor with edit-field endwith  LOOP
        content @ @  1+ vabox new panel ;
    : dump ( -- )  cr indent @ spaces
        content @ @ 1+ 1 ?DO  content @ Ith
            descriptor with dump endwith  LOOP
        item self >class" lctype
        ."  new "  name $@ nip IF  ."  ^^bind " dump-name  THEN
        content @ @ 1+ 1 ?DO  content @ Ith
            descriptor with post-dump endwith  LOOP ;
    : find-object ( o -- desc-o )
        next self self =  IF  drop 0  EXIT  THEN
        dup item self = IF  drop self  EXIT  THEN
        next goto find-object ;
    : find-name ( addr u -- desc-o )
        next self self = IF  drop 0  EXIT  THEN
        2dup name $@ compare 0= IF  2drop self  EXIT  THEN
        next goto find-name ;
    
    : set-name ( addr u -- )  name $! ;
    : dump-name ( -- )
        name $@ nip IF
            name $@ type
        ELSE
            set-var @ IF  nvar @ number !  1 nvar +!  THEN
            ." (" item self >class" lctype ." -" number @ 0
            <# # # #> type  ." )"
        THEN ;
    : dump-class ( -- )
        item self >class" lctype ;
    : dump-ptr ( -- )
        name $@ nip 0=  ?EXIT  \ IF ." | "  THEN
        cr indent @ spaces
        dump-class ."  ptr " dump-name ;
    : create-ptr ( -- )
        name $@ nip IF
            name $@ ">tib item ptr >in off
            item self F postpone bind
        THEN ;
class;

Variable tmp-contents

descriptors class font-descriptors
public:
    font ptr fnt
    method font!
    window ptr chooser
how:
    : font! ( font -- )  bind fnt
        fnt self item font!
        item self widget with dpy self endwith
        IF  item resized  THEN ;
    : dump ( -- )  super dump
        fnt self 0= ?EXIT
        fnt with X-font name-string endwith $@
        dup IF  .'  font" ' type .' "'  ELSE  2drop  THEN ;
    : font-selector ( -- )
        0 ST[ text@ tmp-contents $! ]ST
        fnt self IF  fnt with X-font name-string endwith $@
            ELSE  s" "  THEN
        s" Font:" infotextfield new
        0 1 *fill *hglue new
        ^ S[ tmp-contents $@ nip fnt self 0= and
        IF    tmp-contents $@ X-font new bind fnt
        ELSE  tmp-contents $@ fnt assign  THEN    
        fnt self font! chooser close ]S s"  OK " button new
        ^ S[ chooser close ]S s" Cancel" button new
        ^ S[ 0" xfontsel &" [ also DOS ] system [ previous ] drop ]S
        s" xfontsel" button new
        0 1 *fill *hglue new
        5 hatbox new hskip
        2 vabox new panel
        screen self window new window with
            s" Font Selection" assign show ^
        endwith bind chooser ;
    : edit-field ( -- o )
        0 ST[ text@ >current-name ]ST
        name $@ item self >class" rightcase infotextfield new dup
        ^ S[ font-selector ]S s" Change Font" button new 1 habox new hfixbox
        2 habox new hskip
        swap F bind name-string
        content @ @ 1+ 1 ?DO  content @ Ith
            descriptor with edit-field endwith  LOOP
        content @ @  1+ vabox new panel ;
class;

font-descriptors class referred-descs
how:
    : dump-ptr ( -- )
        cr indent @ spaces
        name $@ nip 0= IF ." | "  THEN
        dump-class ."  ptr " dump-name ;
    : dump ( -- )  cr indent @ spaces
        content @ @ 1+ 1 ?DO  content @ Ith
            descriptor with dump endwith  LOOP
        item self >class" lctype
        ."  new ^^bind " dump-name
        content @ @ 1+ 1 ?DO  content @ Ith
            descriptor with post-dump endwith  LOOP
        fnt self 0= ?EXIT
        .'  font" ' fnt with X-font name-string endwith $@ type .' "' ;
class;

: >current-name cur-descs set-name ;

descriptor class tri-des
public:
    cell var content
how:
    : init ( -- )  0 assign ;
    : assign ( n -- )  content !
        item self 0= ?EXIT
        DELAY  get item assign item draw  changed ;
    : get  ( -- n )  content @ ;

    : edit-field ( -- )
          ^ TN[ 0 content ]T[ content @ assign ]TN S" Left" rbutton new
          ^ TN[ 1 content ]T[ content @ assign ]TN S" Up" rbutton new
          ^ TN[ 3 content ]T[ content @ assign ]TN S" Down" rbutton new
          ^ TN[ 2 content ]T[ content @ assign ]TN S" Right" rbutton new
          $0 $1 *hfill $0 $1 *vfil glue new
        &5 harbox new hskip ;
    : null ( -- 0 )  0 ;
    : make ( -- n )  get ;
    : dump ( -- )  get
        :left  case? IF  ." :left "  EXIT  THEN
        :up    case? IF  ." :up "    EXIT  THEN
        :down  case? IF  ." :down "  EXIT  THEN
        :right case? IF  ." :right " EXIT  THEN  .d  ." TRI: " ;
class;

descriptor class string-des
public:
    cell var content
how:
    : init ( -- ) s" String" assign ;
    : dispose ( -- ) content HandleOff super dispose ;
    : assign ( addr n -- )  content $!
        item self 0= ?EXIT
        DELAY get item text!  item resized  changed ;
    : get ( -- addr n )  content $@ ;
    
    : edit-field ( -- o )  ^ F cur bind string
        0 ST[ text@ pad place pad count cur string assign ]ST
        get s" String:" infotextfield new
        dup F bind edit-string ;
    : null ( -- addr u )  s" String" ;
    : make ( -- addr u )  get ;
    : dump ( -- ) .' S" ' get type .' " ' ;
class;

string-des class text-des
how:
    : init ( -- ) s" Text" assign ;
    : edit-field ( -- o )  ^ F cur bind text
        0 ST[ text@ pad place pad count cur text assign ]ST
        get s" Text" infotextfield new
    ;
    : assign ( addr n -- )  content $!
        item self 0= ?EXIT
        DELAY get item assign  item resized  changed ;
    : null ( -- addr u )  s" Text" ;
    : dump ( -- ) .' T" ' get type .' " ' ;
class;

string-des class menu-des
how:
    : init ( -- ) s" <menu>" assign ;
    : assign ( addr u -- ) content $! ;
    : null ( -- widget )
        0 S[ ]S s" <empty>" menu-entry new
        1 vabox new 2 borderbox ;
    : make ( -- widget )  null ;
    : dump ( -- ) ." M: " get type ."  widget " ;
    : edit-field ( -- o )  ^ F cur bind text
        0 ST[ text@ pad place pad count cur text assign ]ST
        get s" Menu:" infotextfield new ;
class;

descriptor class char-des
public:
    cell var content
how:
    : init ( -- ) bl assign ;
    : assign ( addr u -- )
        dup 0= IF  2drop content off
        ELSE  1 umin  content move  THEN ;
    : get ( -- addr u ) content 1 ;
    : edit-field ( -- o )  ^ F cur bind string
        0 ST[ text@ dup 1- 0 max /string
              cur string assign ]ST
        get s" Text:" infotextfield new ;
    : null ( -- char ) bl ;
    : make ( -- char ) bl ;
    : dump ( -- ) space
        infotextfield with get endwith
        0= IF ." bl" drop ELSE ." '" c@ emit THEN space ;
class;

descriptor class 2char-des
how:
    : edit-field ( -- o )
        0 ST[ ]ST s" +" s" On-Char:" infotextfield new
        0 ST[ ]ST s" -" s" Off-Char:" infotextfield new
        2 habox new 1 hskips ;
    : null ( -- char- char+ ) '- '+ ;
    : make ( -- char- char+ ) '- '+ ;
    : .char ( char -- )
        0= IF ." bl" drop  ELSE ." '" c@ emit THEN space ;
    : dump ( -- ) space
        habox with
            childs widgets self
            infotextfield with get endwith .char
            childs self infotextfield with get endwith .char
        endwith ;
class;

descriptor class number-des
    2 cells var content
how:
    : get  content 2@ ;
    : assign content 2!
        item self 0= ?EXIT
        DELAY get item assign  item resized  changed ;
    : edit-field ( -- o )
        ^ cur bind num
        0 SN[ text@ cur num assign ]SN get s" Number:"
        infotextfield new
    ;
    : null ( -- addr u )  0. ;
    : make ( -- addr u ) get ;
    : dump ( -- )  base push decimal
        get ." &" 0 d.r ." . ]N ( MINOS ) " ;
class;

: try-icon ( addr u -- icon )
    2dup icon?  ?dup  IF  nip nip icon@  EXIT  THEN
    dup 9 + NewPtr >r
    r@ 8+ place  icons @ r@ !  r@ cell+ off
    r@ ['] icon@ catch
    0= IF  r> icons !  EXIT  THEN
    drop test-icon r> DisposPtr ;

string-des class icon-des
how:
    : edit-field ( -- o )
        ^ cur bind icon
        0 ST[ text@ cur icon assign ]ST
        content $@ s" Icon:" infotextfield new ;
    : null ( -- icon )  test-icon ;
    : make ( -- icon )  get try-icon ;
    : assign ( addr n -- )  content $!
        item self 0= ?EXIT
        DELAY  make  item assign item resized ;
    : dump ( -- )  .'  icon" ' get type .' " ' ;
class;

icon-des class 2icon-des
public:
    cell var content2
how:
    : edit-field ( -- o )
        ^ cur bind icon
        0 ST[ text@ cur icon get 2swap 2drop cur icon assign ]ST
        content $@ s" On-Icon:" infotextfield new
        0 ST[ text@ cur icon get 2drop 2swap cur icon assign ]ST
        content2 $@ s" Off-Icon:" infotextfield new
        2 hatbox new 1 hskips ;
    : null ( -- icon1 icon2 )  off-icon on-icon ;
    : make ( -- icon1 icon2 )  get try-icon >r try-icon r> swap ;
    : get ( -- addr1 u1 addr2 u2 ) content $@ content2 $@ ;
    : assign ( addr1 u1 addr2 u2 -- )
        content2 $! content $!
        item self 0= ?EXIT
        DELAY  make item assign item resized ;
    : dump ( -- )  .'  2icon" ' get type .' "' type .' " ' ;
    : init  s" " super init ;
    : dispose  content2 HandleOff super dispose ;
class;

descriptor class glue-des
public:
    cell var pixels
    cell var fills
    cell var quantity
how:
    : init  $10 pixels !  1 fills !  1 quantity ! ;
    : >assign ( pix fill -- pix fill quan )
        dup 0= IF  0  EXIT  THEN
        dup 1 *fil   1- and 0= IF 1 *fil   / 1  EXIT  THEN
        dup 1 *fill  1- and 0= IF 1 *fill  / 2  EXIT  THEN
        dup 1 *filll 1- and 0= IF 1 *filll / 3  EXIT  THEN
        0 ;
    : null  make ;
    | Create 'fills
      F ' noop A, F ' *fil A, F ' *fill A, F ' *filll A,
    : make  get cells 'fills + perform swap 3 max swap ;
    : get  pixels @ fills @ quantity @ ;
    : assign  quantity ! fills ! pixels ! ;
class;

glue-des class hglue-des
how:
    : edit-field ( -- o ) ^ cur bind hglue
        0 SN[ text@ drop cur hglue get rot drop cur hglue assign ]SN
        pixels @ extend s" HPixels:"
            infotextfield new
        0 SN[ cur hglue get nip text@ drop swap cur hglue assign ]SN
        fills @  extend s" Fills:"
            infotextfield new
        ^ TN[ 0 quantity ]T[ get assign ]TN
            s" pixel" rbutton new
        ^ TN[ 1 quantity ]T[ get assign ]TN
            s" fil"   rbutton new
        ^ TN[ 2 quantity ]T[ get assign ]TN
            s" fill"  rbutton new
        ^ TN[ 3 quantity ]T[ get assign ]TN
            s" filll" rbutton new
        4 harbox new hfixbox
        3 habox new hskip ;
    : assign  dup 0< IF  drop 2drop  EXIT  THEN
        super assign  item self 0= ?EXIT
        make item with glue w+ ! 3 max glue wmin ! parent resized 
        endwith ;
    : dump ( -- ) base push hex
        ." $" pixels @ . ." $" fills @ . quantity @ 0=
        IF  ." *hpix "  EXIT  THEN
        s" *hfilll" drop quantity @ 4 + type space ;
class;

glue-des class vglue-des
how:
    : edit-field ( -- o ) ^ cur bind vglue
        0 SN[ text@ drop cur vglue get rot drop cur vglue assign ]SN
        pixels @ extend s" VPixels:"
           infotextfield new
        0 SN[ cur vglue get nip text@ drop swap cur vglue assign ]SN
        fills @  extend s" Fills:"
             infotextfield new
        ^ TN[ 0 quantity ]T[ get assign ]TN
             s" pixel" rbutton new
        ^ TN[ 1 quantity ]T[ get assign ]TN
             s" fil"   rbutton new
        ^ TN[ 2 quantity ]T[ get assign ]TN
             s" fill"  rbutton new
        ^ TN[ 3 quantity ]T[ get assign ]TN
             s" filll" rbutton new
        4 harbox new hfixbox
        3 habox new hskip ;
    : assign  dup 0< IF  drop 2drop  EXIT  THEN
        super assign  item self 0= ?EXIT
        make item with glue h+ ! 3 max glue hmin ! parent resized
        endwith ;
    : dump ( -- ) base push hex
        ." $" pixels @ . ." $" fills @ . quantity @ 0=
        IF  ." *vpix "  EXIT  THEN
        s" *vfilll" drop quantity @ 4 + type space ;
class;

descriptor class topglue-des
how:
    : edit-field ( -- o )
        s" Topglue" text-label new ;
    : null  ;
    : make  ;
    : dump  ;
class;

descriptor class term-des
    cell var w
    cell var h
how:
    : init  1 w ! 1 h ! super init ;
    : get   w @ h @ ;
    : assign  2dup h ! w !
      item self IF  item assign  ELSE  2drop  THEN ;
    : null  1 1 ;
    : make  get ;
    : edit-field ( -- o )
        ^ cur bind num
        0 SN[ text@ drop 1 max cur num get nip cur num assign ]SN
              w @ 0 s" W:"
        infotextfield new
        0 SN[ cur num get drop text@ drop 1 max cur num assign ]SN
              h @ 0 s" H:"
        infotextfield new 2 habox new hskip
    ;
    : dump  base push hex
      ." $" w @ . ." $" h @ . ."  ]TERM " ; 
class;

descriptor class edit-des
    cell var linew
how:
    : get     linew @ ;
    : assign  linew !
        DELAY  get item self stredit with 1+ cols ! resized
        endwith  changed ;
    : edit-field ( -- o )
        ^ cur bind num
        0 SN[ text@ drop cur num assign ]SN get 0 s" Line width:"
        infotextfield new ;
    : null (straction  $40 linew ! ;
    : make (straction ;
    : dump ."  (straction " ;
    : post-dump base push hex ."  $" get 0 .r  ."  setup-edit " ;
class;

string-des class action-des
public:
    method assign-tip
    method get-tip
    method add-code
    cell var tooltip-string
    codeedit ptr code-lines
how:
    : get-tip    ( -- addr u )  tooltip-string $@ ;
    : assign-tip ( addr u -- )  tooltip-string $! ;
    : add-code   ( addr u -- )  content $+line ;
    : tooltip-field ( -- o )
        0 ST[ text@ cur action with assign-tip endwith ]ST
        get-tip s" Tooltip:" infotextfield new ;
    : edit-field ( -- o )  ^ F cur bind action
        s" Code:" text-label new
        0 1 *fill 2dup glue new
        2 vabox new
        content HLock
        get content new-code dup bind code-lines
        dup F bind code-string
        content HUnLock
        1 habox new -2 borderbox
        0 1 *fill 2dup glue new
        3 habox new
        tooltip-field 2 vabox new vskip ;
    : assign ( addr n -- )  content $! ;
    : null ( -- actor ) 0 ['] noop simple new ;
    : make ( -- actor ) 0 ['] noop simple new ;
    : dump-tooltip ( -- )
        tooltip-string $@ nip
        IF  .'  TT" ' tooltip-string $@ type .' " '  THEN ;
    : dump ( -- ) ." ^^ S[ " get type ."  ]S ( MINOS ) " dump-tooltip ;
    : init ( addr u -- ) s" " tooltip-string $!  s" " assign ;
    : dispose  tooltip-string HandleOff  code-lines dispose super dispose ;
class;

action-des class click-des
how:
    : init ( -- ) s" " tooltip-string $!
        s" ( x y b n -- ) 2drop 2drop" assign ;
    : dump ( -- ) ." ^^ CK[ " get type ."  ]CK ( MINOS ) " ;
class;

action-des class canvas-des
how:
    : edit-field ( -- o )  ^ F cur bind action
        s" Draw:" text-label new
        0 1 *fill 2dup glue new
        2 vabox new
        content HLock
        get content new-code dup bind code-lines
        dup F bind edit-string
        content HUnLock
        1 habox new -2 borderbox
        0 1 *fill 2dup glue new
        3 habox new ;
    : null ( -- actor ) CV[ ]CV ;
    : make ( -- actor ) null ;
    : dump ( -- ) ." CV[ " get type ."  ]CV ( MINOS ) " ;
class;

canvas-des class glcanvas-des
how:
    : null ( -- actor ) GL[ ]GL ;
    : make ( -- actor ) null ;
    : dump ( -- ) ." GL[ " get type ."  ]GL ( MINOS ) " ;
class;

action-des class stroke-des
how:
    : edit-field ( -- o )  ^ F cur bind action
        s" Dostroke:" text-label new
        0 1 *fill 2dup glue new
        2 vabox new
        content HLock
        get content new-code dup bind code-lines
        dup F bind code-string
        content HUnLock
        1 habox new -2 borderbox
        0 1 *fill 2dup glue new
        3 habox new ;
    : assign ( addr n -- )  content $! ;
    : null ( -- ) 0 ST[ ]ST ;
    : make ( -- ) null ;
    : dump ( -- )
        ." ^^ ST[ " get type ."  ]ST ( MINOS ) " ;
    : post-dump ( -- ) ;
class;

action-des class nstroke-des
how:
    : edit-field ( -- o )  ^ F cur bind action
        s" Dostroke:" text-label new
        0 1 *fill 2dup glue new
        2 vabox new
        content HLock
        get content new-code dup bind code-lines
        dup F bind code-string
        content HUnLock
        1 habox new -2 borderbox
        0 1 *fill 2dup glue new
        3 habox new ;
    : assign ( addr n -- )  content $! ;
    : null ( -- ) 0 SN[ ]SN ;
    : make ( -- ) null ;
    : dump ( -- )
        ." ^^ SN[ " get type ."  ]SN ( MINOS ) " ;
    : post-dump ( -- ) ;
class;

descriptor class display-des
how:
    : edit-field ( -- o )
        s" Display" text-label new ;
    : assign ( object -- )
        item self 0= IF  drop  EXIT  THEN
        item assign ;
    : null ( -- ) ;
    : make ( -- ) ;
    : dump ( -- ) ;
    : post-dump ( -- ) ."  D[ "
        item self backing with child self endwith dump-box
        ."  ]D ( MINOS ) " ;
class;

display-des class viewport-des
how:
    : post-dump ( -- ) ."  DS[ "
        item self backing with child self endwith dump-box
        ."  ]DS ( MINOS ) " ;
class;

Create toggle-on$  ," On-Xt ( -- ):" ," Var ( -- addr ):" ," Num Var ( -- n addr ):" ," Fetch-Xt ( -- flag ):"
Create toggle-off$ ," Off-Xt ( -- ):" ," Change-Xt ( -- ):" ," Change-Xt ( -- ):" ," Store-Xt ( flag -- ):"

: typ$ ( addr n -- addr' u )  0 ?DO  count +  LOOP  count ;

descriptor class toggle-des
public:
    method assign-tip
    method get-tip
    cell var flag
    cell var content
    cell var content2
    cell var tooltip-string
    cell var typ
how:
    : init ( -- ) s" " s" " assign s" " tooltip-string $! ;
    : dispose  content HandleOff  content2 HandleOff  tooltip-string HandleOff
        super dispose ;
    : get-tip    ( -- addr u )  tooltip-string $@ ;
    : assign-tip ( addr u -- )  tooltip-string $! ;
    : tooltip-field ( -- o )
        0 ST[ text@ cur toggle with assign-tip endwith ]ST
	get-tip s" Tooltip:" infotextfield new ;
    : edit-field ( -- o )
        ^ F cur bind toggle
        0 TN[ 0 typ ]T[ [ toggle-on$  0 typ$ ] SLiteral code-string text!
                        [ toggle-off$ 0 typ$ ] SLiteral code2-string text! ]TN
            s" Toggle" rbutton new
        0 TN[ 1 typ ]T[ [ toggle-on$  1 typ$ ] SLiteral code-string text!
                        [ toggle-off$ 1 typ$ ] SLiteral code2-string text! ]TN
            s" Toggle-Var" rbutton new
        0 TN[ 2 typ ]T[ [ toggle-on$  2 typ$ ] SLiteral code-string text!
                        [ toggle-off$ 2 typ$ ] SLiteral code2-string text! ]TN
            s" Toggle-Num" rbutton new
        0 TN[ 3 typ ]T[ [ toggle-on$  3 typ$ ] SLiteral code-string text!
                        [ toggle-off$ 3 typ$ ] SLiteral code2-string text! ]TN
            s" Toggle-State" rbutton new
        cur back with 2fill endwith 5 hartbox new
        content toggle-on$ typ @ typ$ infocodefield new
        dup F bind code-string
        content2 toggle-off$ typ @ typ$ infocodefield new
        dup F bind code2-string
        tooltip-field
        4 vabox new vskip ;
    : null ( -- actor ) 0 flag ['] noop toggle-var new ;
    : make ( -- actor ) 0 flag ['] noop toggle-var new ;
    : assign ( addr1 n1 addr2 n2 -- )  content2 $!  content $! ;
    : dump-tooltip ( -- )
        tooltip-string $@ nip
        IF  .'  TT" ' tooltip-string $@ type .' " '  THEN ;
    : dump ( -- ) ." ^^"
        get 2swap
        typ @
        0 case? IF
            space flag @ 2 .r
            ."  T[ " type ."  ][ ( MINOS ) " type ."  ]T ( MINOS ) "
        ELSE 1 case? IF
            ."  TV[ " type ."  ]T[ ( MINOS ) " type ."  ]TV ( MINOS ) "
        ELSE 2 case? IF
            ."  TN[ " type ."  ]T[ ( MINOS ) " type ."  ]TN ( MINOS ) "
        ELSE
            drop
            ."  TS[ " type ."  ][ ( MINOS ) " type ."  ]TS ( MINOS ) "
        THEN THEN THEN
        dump-tooltip ;
    : get ( -- addr1 n1 addr2 n2 )  content $@ content2 $@ ;
class;

descriptor class index-des
public:
    cell var fstate
how:
    : init ( -- )  super init  fstate on ;
    : edit-field ( -- o )
        item xywh 2drop 1 2 item parent clicked
        s" Flipper" text-label new ;
    : null ( -- o )
        cur box widgets self ?hbox cur +boxmode !
        addcardfile
        dup item self new-link  -1 flipper ;
    : make ( -- o )  item self fstate @ flipper ;
    : dump ( -- o )  ." 0 "
        item self find-linked
        combined with attribs c@ endwith :flip and 0= .
        ." flipper " ;
class;

descriptor class step-des
    cell var hstep
    cell var vstep
how:
    : init 1 hstep ! 1 vstep ! ;
    : edit-field ( -- o )
        ^ F cur bind step
        0 SN[ ]SN
        get drop 0 s" Hstep:" infotextfield new
        dup F bind edit-string
        0 SN[ ]SN
        get nip  0 s" Vstep:" infotextfield new
        2 habox new 1 hskips ;
    : get  hstep @ vstep @ ;
    : assign ( hstep vstep -- )  vstep ! hstep ! ;
    : null ( -- hstep vstep ) 1 1 ;
    : make ( -- hstep vstep ) hstep @ vstep @ ;
    : dump ( -- ) hstep @ . vstep @ . ;
class;

descriptor class beam-des
how:
    : init ;
    : edit-field ( -- o )
        s" Beamer" text-label new ;
    : null  0 0 ;
    : make  0 0 ;
    : assign ;
    : dump ( -- )  ." :beamer " ;
class;

descriptor class slider-des
    cell var steps
    cell var width
how:
    : init &10 steps ! &1 width ! ;
    : edit-field ( -- o )
        ^ F cur bind slider
        0 SN[ text@ drop cur slider get nip cur slider assign ]SN
        get drop 0 s" Steps:" infotextfield new
        dup F bind edit-string 
        0 SN[ cur slider get drop text@ drop cur slider assign ]SN
        get nip 0 s" Width:" infotextfield new
        2 habox new 1 hskips ;
    : get ( -- steps width )  steps @ width @ ;
    : assign ( steps width -- )  2dup width ! steps !
        item self 0= IF  2drop  EXIT  THEN
         item self widget with 0 -rot callback assign endwith
        item !resized  item resized ;
    : null ( -- actor ) cur pane self 0 &10 1 slider-var new ;
    : make ( -- actor ) cur pane self 0 get slider-var new ;
    : dump ( -- )
        base push decimal ." ^^ 0 &" get swap . ." &" . ;
class;

descriptor class scaler-des
    cell var contents
public:
    cell var pos
    early offset!
    early text*!
    early text/!
how:
    : init &10 contents ! ;
    : h-offset ( -- addr )
	item self hscaler with offset endwith ;
    : text*/ ( -- addr )
	item self hscaler with text*/ endwith ;
    : assign ( n -- ) dup contents !
        item self 0= IF  drop  EXIT  THEN
        item self widget with get endwith nip nip
        over min h-offset @ + swap
        item self widget with callback assign endwith
        item !resized  item resized ;
    : offset! ( n -- )
        item self 0= IF  drop  EXIT  THEN
        h-offset !
	item !resized  item resized ;
    : text*! ( n -- )
	item self 0= IF  drop  EXIT  THEN
	text*/ cell+ !
	item !resized  item resized ;
    : text/! ( n -- )
	item self 0= IF  drop  EXIT  THEN
	text*/ !
	item !resized  item resized ;
    : edit-field ( -- o )
        ^ F cur bind slider
        0 SN[ text@ drop cur slider assign ]SN
        get 0 s" Steps:" infotextfield new
        0 SN[ text@ drop cur slider with offset! endwith ]SN
        h-offset @ s>d s" Offset:" infotextfield new
        0 SN[ text@ drop cur slider with text*! endwith ]SN
        text*/ cell+ @ s>d s" Scale:" infotextfield new
        0 SN[ text@ drop cur slider with text/! endwith ]SN
        text*/ @ s>d s" Div:" infotextfield new
        4 habox new hskip
        dup F bind edit-string ;
    : null ( -- actor ) cur pane self pos @ &9 scale-var new ;
    : make ( -- actor ) cur pane self pos @ get scale-var new ;
    : get ( -- n ) contents @ ;
    : dump ( -- ) base push decimal ." ^^ "
        item self hscaler with get nip nip endwith .d
        get .d ;
    : post-dump ( -- )
	h-offset @ ?dup  IF  space .d ." SC# " THEN
	text*/ 2@ 1 1 d= 0= IF  space text*/ 2@ swap .d .d ." SC*/ " THEN ;
class;

action-des class slider-code
how:
    : null ( -- ) ;
    : make ( -- ) ;
    : init ( -- ) s" " tooltip-string $!  s" ( pos -- ) drop" assign ;
    : dump ( -- ) ." SL[ " get type ."  ]SL ( MINOS ) " dump-tooltip ;
class;

slider-code class scaler-code
how:
    : dump ( -- ) ." SC[ " get type ."  ]SC ( MINOS ) " dump-tooltip ;
class;

descriptors class component-des
    cell var cparam
    cell var cname
how:
    : init  0 super init ;
    : assign ( addr1 u1 addr2 u2 -- )
        cparam $!  cname $! ;
    : dump-class ( -- ) cname $@ type ;
    : dump ( -- )  cr indent @ spaces
        ." ^^ CP[ " cparam $@ type ."  ]CP ( MINOS ) "
        dump-class ."  new "
        name $@ nip IF  ."  ^^bind " dump-name  THEN ;
    : null 0 S[ ]S cname $@ button new dup bind item
        self F bind cur-descs
        all-descs self cur-descs bind next
        cur-descs self F bind all-descs ;
    : make null ;
    : edit-field ( -- o )
        0 ST[ text@ >current-name ]ST
        name $@ s" Component"
        tableinfotextfield new dup F bind name-string
        0 ST[ text@ cur-descs with cname $! cname $@ item assign endwith ]ST
        cname $@ s" Class"
        tableinfotextfield new dup F bind edit-string
        0 ST[ text@ cur-descs with cparam $! endwith ]ST
        cparam $@ s" Params"
        tableinfotextfield new dup F bind code-string
        3 vabox new panel ;
class;

0 descriptors : nil-desc
    nil-desc self nil-desc bind next
    nil-desc self bind all-descs
