\ Menu for dialog window                               17may98py

include gpl-about.m

also MINOS also
: main-menu
    key" v"       s" Editor" menu-entry new
    key" [IFUNDEF] designer show-splash include theseus.fs [THEN] designer open"
                    s" Theseus" menu-entry new
    key" browser" s" Class Browser" menu-entry new
    hline
    key" [IFUNDEF] dragon include dragon.m [THEN] dragon open"
                    s" Dragon" menu-entry new
    key" [IFUNDEF] wave-form include waveform.m [THEN] wave-form open"
                    s" Wave Viewer" menu-entry new
    hline
    key" also minos hide-menu previous"
    s" Hide menu" menu-entry new      
    hline
    ^ S[ bye ]S   s" Bye" menu-entry new
    &10 vabox new 2 borderbox
    s"  File " menu-title new add-menu ;
: game-menu
    key" sh xbigforth 4wins.m &" s" 4 Wins" menu-entry new
    key" sh xbigforth go.m &" s" GNU Go" menu-entry new
    &2 vabox new 2 borderbox
    s"  Games " menu-title new add-menu ;
: color-menu
    ^ S[ gray-colors ]S   s" Gray"   menu-entry new
    ^ S[ bisque-colors ]S s" Bisque" menu-entry new
    ^ S[ red-colors ]S    s" Red"    menu-entry new
    ^ S[ blue-colors ]S   s" Blue"   menu-entry new
    hline
    ^ S[ paper ]S         s" Paper"  menu-entry new
    ^ S[ wood ]S          s" Wood"   menu-entry new
    ^ S[ water ]S         s" Water"  menu-entry new
    ^ S[ water1 ]S        s" Caustics" menu-entry new
    ^ S[ marble ]S        s" Red Marble" menu-entry new
    ^ S[ cracle ]S        s" Cracle" menu-entry new
    ^ S[ mud ]S           s" Mud"    menu-entry new
    ^ S[ mono ]S          s" Mono"   menu-entry new
    &13 vabox new 2 borderbox
    s"  Colors " menu-title new add-menu ;
: font-menu
    ^ S[ large-font ]S    s" Large Font"  menu-entry new
    ^ S[ normal-font ]S   s" Normal Font" menu-entry new
[IFDEF] chinese-font
    ^ S[ chinese-font ]S  s" Chinese Font"  menu-entry new
    ^ S[ japanese-font ]S s" Japanese Font" menu-entry new
    4 vabox new 2 borderbox
[ELSE]
    2 vabox new 2 borderbox
[THEN]
    s"  Fonts " menu-title new add-menu ;
: help-menu
[IFDEF] win32
    key" sh help/index.html"
[ELSE]
    key" sh ${BROWSER-help/netscape.sh} file://$PWD/help/index.html &"
[THEN]
    s" Help" menu-entry new
    key" [IFUNDEF] tip1  include tip.m [THEN] tip1 open"
    s" Tips" menu-entry new
    ^ S[ bigforth-about open ]S s" About" menu-entry new
    3 vabox new 2 borderbox
    [IFDEF] win32 s"  ? " [ELSE] s"  Help " [THEN]
    menu-title new add-help ;

: show-splash ( -- )
    minos-splash new
    screen self frame new frame with
        dup s" MINOS Splash Screen" assign
        screen xywh 2swap 2drop xywh 2swap 2drop p- p2/ show
        modal ' close swap &1000 after schedule
    endwith ;
:noname  show-splash
    main-menu color-menu game-menu font-menu help-menu ; IS terminal-menu

previous previous
