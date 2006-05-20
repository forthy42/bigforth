\ athena-like slider widget                            28dec01py

\needs xconst | import xconst

also minos also xconst also

hslider implements
    : >steps ( x y b n -- step 0 0 0 n ) >r 2drop >r
      parent xywh drop nip swap r> swap - swap
      get drop nip swap */ 1+ 0 0 0 r> ;
    : athup ( x y b n -- )
      >steps WHILEPRESS dup >r get nip nip r> - 0max reslide ;
    : athdown ( x y b n -- )
      >steps WHILEPRESS dup >r get >r - r> r> + min reslide ;
    : athmove ( x y b n -- )  >r 2drop >r
      0 parent xywh 2drop drop
      r> 0 r> 1 and IF  2drop dpy moved! DOPRESS  THEN
      do-slide ;
    : athslide ( x y b n -- )
      over 1 and IF  athdown drop  EXIT  THEN
      over 2 and IF  athmove       EXIT  THEN
      over 4 and IF  athup drop    EXIT  THEN
      2drop 2drop ;
    : init ( callback -- )  >callback
      ^ CK[ athslide ]CK ['] part1 ['] part0 arule new
      ^ CK[ athslide ]CK ['] part2 ['] part0 arule new
      arule with $02000003 assign ^ endwith
      ^ CK[ athslide ]CK ['] part3 ['] part0 arule new
      3 super init ^ vfixbox drop ;
    : moved ( x y -- ) 2drop ^ dpy set-rect
      [IFDEF] x11  XC_sb_h_double_arrow [THEN]
      [IFDEF] win32  IDC_SIZEWE         [THEN] dpy set-cursor ;
class;

hslider0 implements
    hslider :: init
    hslider :: moved
class;

vslider implements
    : >steps ( x y b n -- steps 0 0 0 n ) >r drop
      >r drop parent xywh nip rot drop swap r> swap - swap
      get drop nip swap */ 1+ 0 0 0 r> ;
    : athup ( x y b n -- )
      >steps WHILEPRESS dup >r get nip nip r> - 0max reslide ;
    : athdown ( x y b n -- )
      >steps WHILEPRESS dup >r get >r - r> r> + min reslide ;
    : athmove ( x y b n -- )  >r drop >r  drop
      0 parent xywh 2drop nip
      0 r> r> 1 and IF  2drop dpy moved! DOPRESS  THEN
      do-slide ;
    : athslide ( x y b n -- )
      over 1 and IF  athdown drop  EXIT  THEN
      over 2 and IF  athmove       EXIT  THEN
      over 4 and IF  athup drop    EXIT  THEN
      2drop 2drop ;
    : init ( callback -- )  >callback
      ^ CK[ athslide ]CK ['] part0 ['] part1 arule new
      ^ CK[ athslide ]CK ['] part0 ['] part2 arule new
      arule with $02000003 assign ^ endwith
      ^ CK[ athslide ]CK ['] part0 ['] part3 arule new
      3 super init ^ hfixbox drop ;
    : moved ( x y -- ) 2drop ^ dpy set-rect
      [IFDEF] x11  XC_sb_v_double_arrow [THEN]
      [IFDEF] win32  IDC_SIZENS         [THEN] dpy set-cursor ;
class;

vslider0 implements
        vslider :: init
        vslider :: moved
class;

previous previous previous
