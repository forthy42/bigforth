\ MINOS saving

: dump-attribs ( attrib -- )
    dup :hfix    and IF  ."  hfixbox "  THEN
    dup :vfix    and IF  ."  vfixbox "  THEN
    dup :flip    and IF  ."  flipbox "  THEN
\    dup :resized and IF  ."  rzbox "    THEN
    dup $F0 and IF base push hex ."  $" dup . ."  noborderbox "  THEN
    drop ;

: dump-skips ( hskip vskip -- )
    2dup 1 1 d= IF  ."  panel" 2drop
    ELSE  ?dup IF  space .d ." vskips" THEN
          ?dup IF  space .d ." hskips" THEN
    THEN ;

: dump-border ( border -- )
    ?dup IF  space .d ." borderbox"  THEN ;

: @vars ( o -- border hskip vskip attrib )
    combined with
        borderw cx@  hskip cx@  vskip cx@  attribs c@
    endwith ;

: dump-vars ( o -- )
    @vars dump-attribs dump-skips dump-border ;

: dump-link ( o -- )
    find-linker
    ?dup IF  ."  dup ^^ with C[ " all-descs find-object
        descriptors with dump-name endwith ."  ]C ( MINOS ) endwith "
    THEN ;

: dump-bind ( o -- )
    names find-name ?dup IF
        hint-name with name $@ endwith
        dup IF  ."  ^^bind " type  ELSE  2drop  THEN
    THEN ;

forward (dump-box
Defer do-dump
Defer do-boxdump
Defer do-bug

: dump-childs ( o n -- )
    0 ?DO
        sliderview with widgets self & sliderview @ class?
            IF  inner self  ELSE  self  THEN  endwith
        gadget with
            ^ ^ all-descs find-object 0= & combined @ class? and
        endwith
        IF
            (dump-box
        ELSE
            all-descs find-object
            dup IF
                do-dump
            ELSE
                do-bug
            THEN
        THEN
    LOOP drop ;

: (dump-box ( o -- )
    2 indent +!
    dup >r combined with childs self n @ endwith
    dup >r dump-childs
    -2 indent +!
    r> r> do-boxdump ;

: dump-box ( o -- )
    :[ descriptors with dump endwith ]: IS do-dump
    :[ >r cr indent @ spaces
       .d r@ >class" lctype ."  new"
       r@ dump-bind
       r@ dump-vars
       r> dump-link ]: IS do-boxdump
    :[ cr indent @ spaces ." ( bug at: " . ." ) " ]: IS do-bug
    (dump-box ;

: dispose-box ( o -- )
    :[ descriptors with dispose endwith ]: IS do-dump
    ['] 2drop IS do-boxdump
    ['] drop  IS do-bug
    (dump-box ;

: dump-name ( o -- )
    all-descs find-object
    descriptors with  dump-ptr  endwith ;

: ?dump-box-name ( o -- )
    names find-name ?dup IF
        hint-name with
            name $@ nip
            IF  cr indent @ spaces
                hint self >class" lctype
                ."  ptr " name $@ type  THEN
        endwith
    THEN ;

| : >slider-o ( o -- o' )
    gadget with & sliderview @ class?
        IF  sliderview inner self  ELSE  self  THEN
    endwith ;
| : >backing-o ( o -- o' )
    gadget with & backing @ class?
        IF    self dump-name backing child self  ELSE  self  THEN
    endwith ;
| : box-o? ( o -- o flag )
    gadget with
        ^ ^ all-descs find-object 0= & combined @ class? and
    endwith ;

: dump-names ( o -- )
    dup ?dump-box-name
    combined with childs self n @ endwith
    0 ?DO
        gadget with widgets self ^ endwith
        >slider-o >backing-o box-o?
        IF  recurse  ELSE  dump-name  THEN
    LOOP drop ;

: dump-all ( -- ) base push hex
    cur resources dump-declaration
    cur resources dump-implementation
    0 cur resources script? IF
        cr ." : main"
        0 cur resources dump-script
        cr drop ."   event-loop bye ;"
        cr ." script? [IF]  main  [THEN]"
    THEN ;

also dos

: dump-file ( addr u -- )
    r/w exe output-file
    ." #! " 0 arg type cr
    ." \ automatic generated code" cr
    ." \ do not edit" cr
    cr
    ." also editor also minos also forth" cr
    dump-all
    cr ." previous previous previous" cr eot ;

previous
