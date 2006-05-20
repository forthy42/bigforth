\ CSS bindings: use libcss

dos also forth

library libcss libcss.so

legacy off

libcss CSSAuthDisk int int (int) CSSAuthDisk ( fd *key_disk -- r )
libcss CSSAuthTitle int int int (int) CSSAuthTitle ( fd *key_title lba -- r )
libcss CSSGetASF int (int) CSSGetASF ( fd -- r )
libcss CSSDecryptTitleKey int int (int) CSSDecryptTitleKey ( *key_title *key_disk -- r )
libcss CSSDescramble int int (void) CSSDescramble ( *sec *key -- )

\ pack start, header, etc.

base c@ base @ = [IF] \ little endian
    $BA010000 Constant pack-start
    $BB010000 Constant system-header
    $00010000 Constant pes-mark
    $00FFFFFF Constant pes-mask
[ELSE]
    $000001BA Constant pack-start
    $000001BB Constant stystem-header
    $00000100 Constant pes-mark
    $FFFFFF00 Constant pes-mask
[THEN]

\ Buffers

Create disk_key  $800 allot
Create title_key  5 allot

: fill-buffer ( buf-addr u1 file u2 -- )  r/o open-file throw
    dup >r read-file r> close-file throw throw drop ;
: >disk_key  s" /dev/dvd" r/o open-file throw >r
  r@ filehandle @ disk_key CSSAuthDisk drop
  r> close-file throw ;
\ disk_key $800 s" disk-key" fill-buffer ;
: >title_key title_key 5 s" title-key" fill-buffer ;

\ un_css

Create sector_buf $800 allot
0 Value fd
0 Value fd-video
0 Value fd-audio
Variable track

Variable do-audio  do-audio on
Variable do-video  do-video on

: cssfill ( -- $800 )  sector_buf $800 fd read-file throw ;
: pes ( -- addr )  sector_buf &13 + count 7 and + ;
: peslen ( addr -- addr len ) 4+ c@+ c@+ -rot swap 8 << + ;
: hdrlen ( addr -- len ) 2+ c@ 3 + ; macro
: pestype ( addr -- type ) dup @ pes-mask and pes-mark =
    IF  3+ c@  ELSE  drop -1  THEN ;
: video? ( type -- flag )  $F0 and $E0 = do-video @ and ; macro
: audio? ( type -- flag )  $BD = do-audio @ and ; macro
: skip-sys-hdr ( addr -- addr' )
    dup @ system-header = IF  peslen +  THEN ;
: write-video ( addr u -- )    fd-video write-file throw ;
: write-audio ( addr u -- )    fd-audio write-file throw ;
: descramble ( -- ) sector_buf title_key CSSDescramble ;
: cssdump ( -- )
    pes skip-sys-hdr
    dup pestype >r peslen over hdrlen /string
    r@ video? IF  descramble write-video
    ELSE
        r@ audio? IF
            over c@ track @ $80 or =
            IF    4 /string descramble write-audio
            ELSE  2drop  THEN
        ELSE  2drop  THEN  THEN
    rdrop ;
: un-css ( -- )
    BEGIN  cssfill $800 = WHILE  sector_buf @ pack-start =
        IF  cssdump  THEN  pause
    REPEAT ;

: out
    do-video @ IF  s" /tmp/video" w/o open-file throw to fd-video  THEN
    do-audio @ IF  s" /tmp/audio" w/o open-file throw to fd-audio  THEN ;
: vts1  s" /misc/cd/video_ts/vts_01_1.vob" r/o open-file throw to fd ;
: vts2  s" /misc/cd/video_ts/vts_01_2.vob" r/o open-file throw to fd ;
: vts3  s" /misc/cd/video_ts/vts_01_3.vob" r/o open-file throw to fd ;
: vts4  s" /misc/cd/video_ts/vts_01_4.vob" r/o open-file throw to fd ;
: vts5  s" /misc/cd/video_ts/vts_01_5.vob" r/o open-file throw to fd ;
: vts6  s" /misc/cd/video_ts/vts_01_6.vob" r/o open-file throw to fd ;
: vts7  s" /misc/cd/video_ts/vts_01_7.vob" r/o open-file throw to fd ;
: vts8  s" /misc/cd/video_ts/vts_01_8.vob" r/o open-file throw to fd ;
: vts9  s" /misc/cd/video_ts/vts_01_9.vob" r/o open-file throw to fd ;

: css-cat
    >disk_key >title_key
    title_key disk_key CSSDecryptTitleKey 0< abort" Decrypting title"
    out
    vts1 un-css
    vts2 un-css
    vts3 un-css
    vts4 un-css
    vts5 un-css
    vts6 un-css
    vts7 un-css
    vts8 un-css
    vts9 un-css ;
