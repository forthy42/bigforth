#! xbigforth
\ automatic generated code
\ do not edit

also editor also minos also forth

component class wave-form
public:
  early widget
  early open
  early dialog
  early open-app
  infotextfield ptr distance
  hscaler ptr steps-x
  hviewport ptr switches
  button ptr wave-flip
  button ptr #load
  viewport ptr wave-s
  button ptr wave-space
 ( [varstart] ) cell var wave-x
cell var wave-y
cell var step-x
cell var time-x
cell var waves
cell var preamble
cell var file-name
canvas [] waveforms
method add-wave
method >wave-name
method init-waves
method show-waves
method wave-file
method step-act
method set-dist ( [varend] ) 
how:
  : open     new DF[ 0 ]DF s" Waveform Viewer" open-component ;
  : dialog   new DF[ 0 ]DF s" Waveform Viewer" open-dialog ;
  : open-app new DF[ 0 ]DF s" Waveform Viewer" open-application ;
class;

component class about-w
public:
  early widget
  early open
  early dialog
  early open-app
  button ptr i-see
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ i-see self ]DF s" About Waveform Viewer" open-component ;
  : dialog   new DF[ i-see self ]DF s" About Waveform Viewer" open-dialog ;
  : open-app new DF[ i-see self ]DF s" About Waveform Viewer" open-application ;
class;

component class help-w
public:
  early widget
  early open
  early dialog
  early open-app
  button ptr help-ok
 ( [varstart] )  ( [varend] ) 
how:
  : open     new DF[ help-ok self ]DF s" Waveform Help" open-component ;
  : dialog   new DF[ help-ok self ]DF s" Waveform Help" open-dialog ;
  : open-app new DF[ help-ok self ]DF s" Waveform Help" open-application ;
class;

help-w implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          S" Left button: set/drag&drop red line" text-label new 
          S" Rigth button: set/drag&drop blue line" text-label new 
          S" Both/middle buttons: switch between dec/hex" text-label new 
          S" +/- switch: show/hide wave" text-label new 
            $10 $1 *hfill $10 $1 *vfill glue new 
            ^^ S[ close ]S ( MINOS ) S"  OK " button new  ^^bind help-ok
            $10 $1 *hfill $10 $1 *vfill glue new 
          &3 hatbox new &1 vskips
        &5 vabox new
      &1 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

about-w implements
 ( [methodstart] )  ( [methodend] ) 
  : widget  ( [dumpstart] )
          S" Waveform viewer" text-label new 
          S" (c) 1997-1999 by Bernd Paysan/Mixed Mode" text-label new 
          S" Written with bigFORTH/MINOS" text-label new 
            $10 $1 *hfilll $10 $1 *vfil glue new 
            ^^ S[ close ]S ( MINOS ) S" I see" button new  ^^bind i-see
            $10 $1 *hpix $10 $1 *vfill glue new 
            ^^ S[ [IFDEF] win32
0" bigforth ##include genwave.fs >file wave.trc $2000 genwave eot bye"
[ELSE]
0" bigforth genwave.fs -e '>file wave.trc $2000 genwave eot bye'"
[THEN]
[ also dos ] system [ previous ] drop ]S ( MINOS ) S" Generate Test Pattern" button new 
            $10 $1 *hfilll $10 $1 *vfil rule new 
          &5 habox new &1 vskips
        &4 vabox new
      &1 vabox new panel
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

include wave-form.fs
wave-form implements
 ( [methodstart] ) also dos also memory also
: add-wave ( n i -- )
  waveforms with waveform add-wave  endwith ;
: >wave-name ( addr u i -- )  waveforms text! ;
: ?preamble ( -- )
  preamble @
  IF  source dup preamble @ !
      tuck preamble @ cell+ @+ + swap move
      preamble @ cell+ +!  THEN ;
: >wave-names ( -- )
  preamble @ @ negate preamble @ cell+ +!
  preamble @ 2@ / dup NewPtr preamble @ @ cells NewPtr
  { u addr idx |
    0 preamble @ @+ @+ + over - -rot 0
    ?DO  over I + c@ bl <> IF  I over cells idx + ! 1+  THEN
    LOOP  2drop
    waves @ 0 ?DO
       preamble @ @+ cell+
       u 0 ?DO  dup idx J cells + @ + c@ addr I + c! over +
          LOOP 2drop
       addr u bl skip I >wave-name
     LOOP addr DisposPtr idx DisposPtr }
  preamble HandleOff ;
: init-waves ( n -- ) ^>^^ dup waves !
  0 ?DO ( Ith )  S" "  LOOP
  waves @ waveform new[] bind[] waveforms ;
: wave-file ( addr u -- )  base push hex
  r/o open-file throw $4000 input-file
  only previous scan-it
  preamble @ IF  preamble HandleOff  THEN
  $4000 preamble Handle!  0. preamble @ 2!
  BEGIN  refill  WHILE
      bl word count 2dup + 1- c@ ': =
      IF
          time-x @ 0= IF  1- decimal s>number drop time-x !
          ELSE  2drop  THEN
          F depth >r hex interpret F depth r> -
          waves @ 0= IF  init-waves >wave-names
          ELSE  dup waves @ <>
              IF  ~~ 0 ?DO  drop  LOOP
                  ." Left because of wrong line in line "
                  F line @ . cr onlyforth
                  loadfile @ close-file deltib throw  EXIT
              THEN
              drop
          THEN
          waves @ 1-
          FOR  I add-wave  NEXT
      ELSE
          2drop ?preamble
      THEN
  REPEAT
  preamble @ IF  preamble HandleOff  THEN
  loadfile @ close-file throw onlyforth
  ( ." finished" cr ) ;
: forget-waves ( -- )
  link[] waveforms cell- dup @ 1+ cells dispose,  waves off ;
: dispose ( -- )
  file-name @  IF  file-name HandleOff  THEN
  forget-waves  super dispose ;
: step-act ( -- actor )
  ^ 0 &30 :[ step-x ! !resized resized ]:
  scale-do new scale-do with  4 pos ! ^ endwith ;
: show-waves ( -- )  ^>^^
  0 BEGIN  dup waves @ <  WHILE
         dup waveforms self dup >r
         0 1 *fill 2dup glue new  2 habox new
      vxrtsizer new  2 vasbox new
      dup -1 combined ' +flip combined ' -flip
      toggle new r> waveform with comment $@ endwith TT-string
      '+ '- togglechar new >r
      swap 1+
  REPEAT
  0 1 *fill 2dup glue new  swap 1+
  vabox new \ 1 vskips
  wave-s assign
  0 BEGIN  dup waves @ <  WHILE
      1+ r> over -roll
  REPEAT
  habox new \ hfixbox 1 vskips
  switches assign ;
: create-waves ( addr u -- )  ^>^^
  ['] wave-file catch  ?dup  IF  onlyforth throw  THEN
  show-waves ;
: show-load ( -- )
  old-file @ IF  old-file $@  ELSE  s" wave.trc"  THEN
  file-name $!
  s" Load Wave File"
  old-file @ IF  old-file $@  ELSE  s" "  THEN
  old-path @ IF  old-path $@  ELSE  s" *.trc"  THEN
  ^ S[ 2over 2swap path+file
       2dup 2dup '/ -scan nip /string old-file $!
       file-name $!  old-path $!
       file-name $@ dpy with window title! endwith
       file-name $@ create-waves dpy !resized
       s" Reload" #load assign ]S fsel-dialog ;
: show ( -- )  super show
  &800 &600 dpy geometry ;
: set-dist ( -- ) \ distance get d0= >r
  wave-y @ wave-x @ - time-x @ m* distance assign ;
previous previous previous ( [methodend] ) 
  : widget  ( [dumpstart] )
              $10 $1 *hfill $0 $1 *vfill glue new 
              &0. ]N ( MINOS ) ^^ SN[  ]SN ( MINOS ) S" Steps" infotextfield new  ^^bind distance
              $10 $1 *hfill $0 $1 *vfill glue new 
            &3 vabox new hfixbox  &1 hskips
              $10 $1 *hfill $0 $1 *vfill glue new 
              ^^ &4 &28 SC[ step-x @ over step-x ! <> IF  dpy !resized  THEN ]SC ( MINOS ) hscaler new  ^^bind steps-x -&20 SC# 
              $64 $1 *hfil $0 $1 *vfil glue new 
              $10 $1 *hfill $0 $1 *vfill glue new 
            &4 vabox new hfixbox  &1 hskips
          &2 habox new hfixbox 
            1 1 hviewport new  ^^bind switches DS[ 
              ^^ S[  ]S ( MINOS ) S" +-" button new  ^^bind wave-flip
            &1 habox new ]DS ( MINOS ) 
          &1 habox new
            ^^ S[ about-w dialog ]S ( MINOS ) S" About" button new 
            ^^ S[ help-w dialog ]S ( MINOS ) S" Help" button new 
            ^^ S[ file-name @ 0= IF  show-load
ELSE  waves off file-name $@ create-waves !resized  THEN ]S ( MINOS ) S" Load" button new  ^^bind #load
          &3 hatbox new hfixbox  panel
        &3 habox new vfixbox 
        1 1 viewport new  ^^bind wave-s DS[ 
              ^^ S[  ]S ( MINOS ) S" Waves" button new  ^^bind wave-space
            &1 vabox new
          &1 vabox new
        &1 habox new ]DS ( MINOS ) 
      &2 vabox new
    ( [dumpend] ) ;
  : init  ^>^^  assign  widget 1 super init ;
class;

: main
  wave-form open-app
  $1 0 ?DO  stop  LOOP bye ;
script? [IF]  main  [THEN]
previous previous previous
