#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class 4wins
public:
  early widget
  early open
  early dialog
  early open-app
  icon-but ptr drop0
  icon-but ptr drop1
  icon-but ptr drop2
  icon-but ptr drop3
  icon-but ptr drop4
  icon-but ptr drop5
  icon-but ptr drop6
  text-label ptr game-state
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Four Wins" open-component ;
  : dialog   new DF[ 0 ]DF s" Four Wins" open-dialog ;
  : open-app new DF[ 0 ]DF s" Four Wins" open-application ;
class;

include 4wins.fs
4wins implements
 ( [methodstart] ) : score-all ( -- )  board *rows *cols * bounds ?DO
  I cx@ ?dup IF  dup abs / cur-stone ! I score? drop  THEN
  LOOP ;
: >icon ( icon n -- )
  CASE  0 OF  drop0 assign  ENDOF
        1 OF  drop1 assign  ENDOF
        2 OF  drop2 assign  ENDOF
        3 OF  drop3 assign  ENDOF
        4 OF  drop4 assign  ENDOF
        5 OF  drop5 assign  ENDOF
        6 OF  drop6 assign  ENDOF
        drop  ENDCASE ;
: 4win-move ( n -- )
  gameover @ IF  4init dpy draw THEN
  #cols 0 ?DO  icon" icons/arrow0.png" I >icon  LOOP
  0 over b[] c@ 0= IF
        icon" icons/arrow1.png" over >icon
        dup >r -1 swap stone? dpy draw
        s" My move" game-state assign dpy sync s" Your move"
        gameover @ 0= IF  c
            icon" icons/arrow0.png" r> >icon
            icon" icons/arrow2.png" swap >icon
            <lost> #depth + <= IF  2drop s" I'm going to lose"  THEN
            gameover @ IF  2drop s" I win!"  THEN
        ELSE
            icon" icons/arrow0.png" r> >icon
            2drop  s" You win!"  THEN
        true #cols 0 ?DO  0 I b[] cx@ 0<> and  LOOP
	IF  2drop s" tie"  gameover on  THEN
        gameover @ IF  score-all  THEN
        game-state assign dpy draw
  ELSE  drop  THEN ;
: draw-row ( n -- ) ^ canvas with
  2 #rows 2* steps 1 1 textpos
  $00 $00 $00 rgb> backcolor clear
  0 swap b[] #rows bounds ?DO
      1 #rows I' I - - 2* 1+ home!
      I cx@
      dup 0= IF  icon" icons/empty.png"  icon  THEN
      dup 0< IF  icon" icons/piece0.png" icon  THEN
      dup 0> IF  icon" icons/piece1.png" icon  THEN
      abs #win >= IF  icon" icons/star0.png" icon  THEN
  LOOP endwith ; ( [methodend] ) 
  : widget  ( [dumpstart] )
            ^^ S[ 0 4win-move ]S ( MINOS )  icon" icons/arrow0.png" icon-but new  ^^bind drop0
            CV[ 0 draw-row ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) >released 0 4win-move ]CK ( MINOS ) $20 $1 *hfil $D8 $0 *vfil canvas new 
          &2 vabox new vfixbox 
            ^^ S[ 1 4win-move ]S ( MINOS )  icon" icons/arrow0.png" icon-but new  ^^bind drop1
            CV[ 1 draw-row ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) >released 1 4win-move ]CK ( MINOS ) $20 $1 *hfil $D8 $0 *vfil canvas new 
          &2 vabox new vfixbox 
            ^^ S[ 2 4win-move ]S ( MINOS )  icon" icons/arrow0.png" icon-but new  ^^bind drop2
            CV[ 2 draw-row ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) >released 2 4win-move ]CK ( MINOS ) $20 $1 *hfil $D8 $0 *vfil canvas new 
          &2 vabox new vfixbox 
            ^^ S[ 3 4win-move ]S ( MINOS )  icon" icons/arrow0.png" icon-but new  ^^bind drop3
            CV[ 3 draw-row ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) >released 3 4win-move ]CK ( MINOS ) $20 $1 *hfil $D8 $0 *vfil canvas new 
          &2 vabox new vfixbox 
            ^^ S[ 4 4win-move ]S ( MINOS )  icon" icons/arrow0.png" icon-but new  ^^bind drop4
            CV[ 4 draw-row ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) >released 4 4win-move ]CK ( MINOS ) $20 $1 *hfil $D8 $0 *vfil canvas new 
          &2 vabox new vfixbox 
            ^^ S[ 5 4win-move ]S ( MINOS )  icon" icons/arrow0.png" icon-but new  ^^bind drop5
            CV[ 5 draw-row ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) >released 5 4win-move ]CK ( MINOS ) $20 $1 *hfil $D8 $0 *vfil canvas new 
          &2 vabox new vfixbox 
            ^^ S[ 6 4win-move ]S ( MINOS )  icon" icons/arrow0.png" icon-but new  ^^bind drop6
            CV[ 6 draw-row ]CV ( MINOS ) ^^ CK[ ( x y b n -- ) >released 6 4win-move ]CK ( MINOS ) $20 $1 *hfil $D8 $0 *vfil canvas new 
          &2 vabox new vfixbox 
        &7 habox new
        S" Your move" text-label new  ^^bind game-state
      &2 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  4wins open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
