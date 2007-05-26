\ win32 functions                                      20jul97py

Module win32api

DOS also forth also

\ lib: kernel kernel32
\ library user32 user32
library gdi32 gdi32

\ KERNEL functions currently used                      20jul97py

1 kernel32 GetModuleHandle GetModuleHandleA ( lib -- handle )
2 kernel32 GlobalAlloc GlobalAlloc ( bytes flags -- handle )
1 kernel32 GlobalLock GlobalLock ( hmem -- addr )
1 kernel32 GlobalUnlock GlobalUnlock ( hmem -- bool )
1 kernel32 GlobalSize GlobalSize ( hmem -- n )
0 kernel32 GetTickCount GetTickCount ( -- ticks )
0 kernel32 GetLastError GetLastError ( -- n )
1 kernel32 SetErrorMode SetErrorMode ( u -- u' )
1 kernel32 SetUnhandledExceptionFilter SetUnhandledExceptionFilter
        ( addr -- addr' )

\ USER functions currently used                        20jul97py

2 user32 LoadCursor LoadCursorA ( cursorname hinst -- hcursor )
1 user32 SetCursor SetCursor ( hcursor -- r )
0 user32 GetDesktopWindow GetDesktopWindow ( -- wnd )
2 user32 GetWindowRect GetWindowRect ( *rect wnd -- flag )
1 user32 GetDC GetDC ( win -- dc )
2 user32 ReleaseDC ReleaseDC ( dc win -- flag )
1 user32 GetQueueStatus GetQueueStatus ( flags -- dword )
5 user32 PeekMessage PeekMessageA ( remove fmax fmin wnd msg -- 
        flag )
4 user32 GetMessage GetMessageA ( fmax fmin wnd msg -- flag )
1 user32 GetSysColor GetSysColor ( index -- color )
1 user32 OpenClipboard OpenClipboard ( wnd/0 -- bool )
0 user32 EmptyClipboard EmptyClipboard ( -- bool )
1 user32 GetClipboardData GetClipboardData ( format -- handle )
2 user32 SetClipboardData SetClipboardData ( mem format -- hand )
0 user32 CloseClipboard CloseClipboard ( -- bool )
1 user32 RegisterClass RegisterClassA ( class -- atom )
&12 user32 CreateWindowEx CreateWindowExA ( param inst menu wndpar 
        h w y x style name class exstyle -- win )
1 user32 DestroyWindow DestroyWindow ( win -- bool )
4 user32 DefWindowProc DefWindowProcA ( lparam wparam msg hwnd --
        lresult )
6 user32 MoveWindow MoveWindow ( repaint h w y x wnd -- bool )
7 user32 SetWindowPos SetWindowPos ( flag h w y x top wnd -- r )
2 user32 ShowWindow ShowWindow ( cmd wnd - bool )
2 user32 ShowOwnedPopups ShowOwnedPopups ( cmd wnd -- bool )
2 user32 LoadIcon LoadIconA ( x y -- icon )
7 user32 CreateIcon CreateIcon ( x y -- icon )
2 user32 SetWindowText SetWindowTextA ( str win -- bool )
1 user32 SetCapture SetCapture ( win -- win )
0 user32 ReleaseCapture ReleaseCapture ( -- bool )
1 user32 TranslateMessage TranslateMessage ( msg -- bool )
1 user32 DispatchMessage DispatchMessageA ( msg -- bool )
3 user32 AdjustWindowRect AdjustWindowRect ( menu style rect -- 
        flag )
3 user32 SetClassLong SetClassLongA ( arg n wnd -- r )
2 user32 BeginPaint BeginPaint ( paint wnd -- dc )
2 user32 EndPaint EndPaint ( paint wnd -- bool )
1 user32 GetKeyState GetKeyState ( vkey -- flag )
4 user32 SetTimer SetTimer ( func elapse ide wnd -- n )
2 user32 KillTimer KillTimer ( ide wnd -- n )
0 user32 WaitMessage WaitMessage ( -- r )
1 user32 SetFocus SetFocus ( wnd -- r )
2 user32 SetParent SetParent ( parent wnd -- r )

\ GDI functions currently used                         20jul97py

3 gdi32 CreatePen CreatePen ( color width penstyle -- hpen )
1 gdi32 CreateSolidBrush CreateSolidBrush ( color -- hbrush )
&14 gdi32 CreateFont CreateFontA ( face pitch&family clippres 
       outpres charset strikeout underline italic fw or esc
       width height -- hfont )
4 gdi32 GetTextExtentPoint32 GetTextExtentPoint32A ( size len str 
       hdc -- bool )
2 gdi32 SelectObject SelectObject ( gdiobj dc -- gdiobj )
4 gdi32 MoveToEx MoveToEx ( *point y x dc -- flag )
3 gdi32 LineTo LineTo ( y x dc -- flag )
5 gdi32 TextOut TextOutA ( u addr y x dc -- flag )
7 gdi32 ExtTextOut ExtTextOutA ( dx u addr opt y x dc -- flag )
&12 gdi32 MaskBlt MaskBlt ( rop ym xm bm ys xs hs nh nw yd xd hd -- 
       bool )
9 gdi32 BitBlt BitBlt ( rop ys xs hs h w yd xd hd -- bool )
5 gdi32 Rectangle Rectangle ( yb xr yt xl dc -- bool )
1 gdi32 BeginPath BeginPath ( hdc -- bool )
1 gdi32 EndPath EndPath ( hdc -- bool )
1 gdi32 CloseFigure CloseFigure ( hdc -- bool )
1 gdi32 FillPath FillPath ( hdc -- bool )
1 gdi32 StrokePath StrokePath ( hdc -- bool )
4 gdi32 CreateRectRgn CreateRectRgn ( y x y x -- reg )
4 gdi32 CombineRgn CombineRgn ( flag r1 r2 dr -- n )
2 gdi32 SelectClipRgn SelectClipRgn ( reg dc -- n )
1 gdi32 DeleteObject DeleteObject ( obj -- r )
3 gdi32 CreateCompatibleBitmap CreateCompatibleBitmap ( h w dc -- 
        bm )
1 gdi32 CreateCompatibleDC CreateCompatibleDC ( dc -- dc )
3 gdi32 GetPixel GetPixel ( y x dc -- pixel )
2 gdi32 SetTextColor SetTextColor ( color hdc -- color )
2 gdi32 SetPolyFillMode SetPolyFillMode ( mode hdc -- n )
2 gdi32 SetBkMode SetBkMode ( mode dc -- n )
6 gdi32 CreateDIBitmap CreateDIBitmap ( usage bmi binit winit 
        bmih hdc -- bitmap )
2 gdi32 GetDeviceCaps GetDeviceCaps ( index hdc -- n )
1 gdi32 DeleteDC DeleteDC ( dc -- flag )
4 gdi32 SetPixel SetPixel ( color y x dc -- color )
3 gdi32 PolylineTo PolylineTo ( n *point dc -- bool )
3 gdi32 Polyline Polyline ( n *point dc -- bool )
3 gdi32 Polygon Polygon ( n *point dc -- bool )
2 gdi32 SetWorldTransform SetWorldTransform ( *xform dc -- bool )

\ structures

struct{
  cell cx
  cell cy
} size

struct{
  cell left
  cell top
  cell right
  cell bottom
} rect

struct{
  cell x
  cell y
} POINT

struct{
  cell   hwnd
  cell   message
  cell   wParam
  cell   lParam
  cell   time
  cell   pt.x
  cell   pt.y
} MSG 

struct{
  cell style
  ptr lpfnWndProc
  cell cbClsExtra
  cell cbWndExtra
  cell hInstance
  cell hIcon
  cell hCursor
  cell hbrBackground
  ptr lpszMenuName
  ptr lpszClassName
} WNDCLASS

struct{
  cell   biSize 
  cell   biWidth 
  cell   biHeight 
  2      biPlanes 
  2      biBitCount 
  cell   biCompression 
  cell   biSizeImage 
  cell   biXPelsPerMeter 
  cell   biYPelsPerMeter 
  cell   biClrUsed 
  cell   biClrImportant 
} BITMAPINFOHEADER

struct{
    cell   ControlWord
    cell   StatusWord
    cell   TagWord
    cell   ErrorOffset
    cell   ErrorSelector
    cell   DataOffset
    cell   DataSelector
    &80    RegisterArea
    cell   Cr0NpxState
} FLOATING_SAVE_AREA

| sizeof FLOATING_SAVE_AREA constant FSA

struct{
    cell ContextFlags

    cell   Dr0
    cell   Dr1
    cell   Dr2
    cell   Dr3
    cell   Dr6
    cell   Dr7

    FSA FloatSave

    cell   SegGs
    cell   SegFs
    cell   SegEs
    cell   SegDs

    cell   Edi
    cell   Esi
    cell   Ebx
    cell   Edx
    cell   Ecx
    cell   Eax

    cell   Ebp
    cell   Eip
    cell   SegCs 
    cell   EFlags
    cell   Esp
    cell   SegSs
} EX_CONTEXT

struct{
    cell hwnd
    cell hwndInsertAfter
    cell x
    cell y
    cell cx
    cell cy
    cell flags
} WINDOWPOS

\ CreatePen, ExtCreatePen                              20jul97py

\ : #define  >in @ >r bl word drop interpret >in @ r> >in ! >r
\   Constant r> >in ! ;

$00000 Constant PS_SOLID
$00001 Constant PS_DASH
$00002 Constant PS_DOT
$00003 Constant PS_DASHDOT
$00004 Constant PS_DASHDOTDOT
$00005 Constant PS_NULL
$00006 Constant PS_INSIDEFRAME
$00007 Constant PS_USERSTYLE
$00008 Constant PS_ALTERNATE

$00000 Constant PS_ENDCAP_ROUND
$00100 Constant PS_ENDCAP_SQUARE
$00200 Constant PS_ENDCAP_FLAT

$00000 Constant PS_JOIN_ROUND
$01000 Constant PS_JOIN_BEVEL
$02000 Constant PS_JOIN_MITER

$00000 Constant PS_COSMETIC
$10000 Constant PS_GEOMETRIC

$0000F Constant PS_STYLE_MASK
$00F00 Constant PS_ENDCAP_MASK
$F0000 Constant PS_TYPE_MASK

\ GetIconInfo                                          20jul97py

32512 Constant IDC_ARROW
32513 Constant IDC_IBEAM
32514 Constant IDC_WAIT
32515 Constant IDC_CROSS
32516 Constant IDC_UPARROW
32642 Constant IDC_SIZENWSE
32643 Constant IDC_SIZENESW
32644 Constant IDC_SIZEWE
32645 Constant IDC_SIZENS
32646 Constant IDC_SIZEALL
32648 Constant IDC_NO
32650 Constant IDC_APPSTARTING
32651 Constant IDC_HELP
32640 Constant IDC_SIZE
32641 Constant IDC_ICON

32512 Constant IDI_APPLICATION
\ 32513 Constant IDI_HAND
\ 32514 Constant IDI_QUESTION
\ 32515 Constant IDI_EXCLAMATION
\ 32516 Constant IDI_ASTERISK
\ 32517 Constant IDI_WINLOGO

\ fonts                                                20aug97py

  0 Constant FW_DONTCARE
100 Constant FW_THIN
200 Constant FW_EXTRALIGHT
300 Constant FW_LIGHT
400 Constant FW_NORMAL
500 Constant FW_MEDIUM
600 Constant FW_SEMIBOLD
700 Constant FW_BOLD
800 Constant FW_EXTRABOLD
900 Constant FW_HEAVY

100 Constant BOLD_FONTTYPE
200 Constant ITALIC_FONTTYPE
400 Constant REGULAR_FONTTYPE

  0 Constant ANSI_CHARSET
  1 Constant DEFAULT_CHARSET
  2 Constant SYMBOL_CHARSET
128 Constant SHIFTJIS_CHARSET
129 Constant HANGEUL_CHARSET
134 Constant GB2312_CHARSET
136 Constant CHINESEBIG5_CHARSET
161 Constant GREEK_CHARSET
162 Constant TURKISH_CHARSET
177 Constant HEBREW_CHARSET
178 Constant ARABIC_CHARSET
186 Constant BALTIC_CHARSET
204 Constant RUSSIAN_CHARSET
222 Constant THAI_CHARSET
238 Constant EASTEUROPE_CHARSET
255 Constant OEM_CHARSET

0 Constant OUT_DEFAULT_PRECIS
1 Constant OUT_STRING_PRECIS
2 Constant OUT_CHARACTER_PRECIS
3 Constant OUT_STROKE_PRECIS
4 Constant OUT_TT_PRECIS
5 Constant OUT_DEVICE_PRECIS
6 Constant OUT_RASTER_PRECIS
7 Constant OUT_TT_ONLY_PRECIS
8 Constant OUT_OUTLINE_PRECIS

  0 Constant CLIP_DEFAULT_PRECIS
  1 Constant CLIP_CHARACTER_PRECIS
  2 Constant CLIP_STROKE_PRECIS
 15 Constant CLIP_MASK
 16 Constant CLIP_LH_ANGLES
 32 Constant CLIP_TT_ALWAYS
128 Constant CLIP_EMBEDDED
0 Constant DEFAULT_QUALITY
1 Constant DRAFT_QUALITY
2 Constant PROOF_QUALITY

0 Constant DEFAULT_PITCH
1 Constant FIXED_PITCH
2 Constant VARIABLE_PITCH

80 Constant FF_DECORATIVE
 0 Constant FF_DONTCARE
48 Constant FF_MODERN
16 Constant FF_ROMAN
64 Constant FF_SCRIPT
32 Constant FF_SWISS

\ CombineRgn                                           21jul97py

1 Constant RGN_AND
2 Constant RGN_OR
3 Constant RGN_XOR
5 Constant RGN_COPY
4 Constant RGN_DIFF

0 Constant ERROR
1 Constant NULLREGION
2 Constant SIMPLEREGION
3 Constant COMPLEXREGION

\ GetQueueStatus                                       21jul97py

$01 Constant QS_KEY
$02 Constant QS_MOUSEMOVE
$04 Constant QS_MOUSEBUTTON
$06 Constant QS_MOUSE
$07 Constant QS_INPUT
$08 Constant QS_POSTMESSAGE
$10 Constant QS_TIMER
$20 Constant QS_PAINT
$40 Constant QS_SENDMESSAGE
$80 Constant QS_HOTKEY

$BF Constant QS_ALLEVENTS
$FF Constant QS_ALLINPUT

\ PeekMessage                                          21jul97py

0 Constant PM_NOREMOVE
1 Constant PM_REMOVE
2 Constant PM_NOYIELD

\ Window messages                                      22jul97py

 0 Constant WM_NULL
 1 Constant WM_CREATE
 2 Constant WM_DESTROY
 3 Constant WM_MOVE
 5 Constant WM_SIZE
 6 Constant WM_ACTIVATE
 7 Constant WM_SETFOCUS
 8 Constant WM_KILLFOCUS
10 Constant WM_ENABLE
11 Constant WM_SETREDRAW
12 Constant WM_SETTEXT
13 Constant WM_GETTEXT
14 Constant WM_GETTEXTLENGTH
15 Constant WM_PAINT
16 Constant WM_CLOSE
17 Constant WM_QUERYENDSESSION
18 Constant WM_QUIT
19 Constant WM_QUERYOPEN
20 Constant WM_ERASEBKGND
21 Constant WM_SYSCOLORCHANGE
22 Constant WM_ENDSESSION
24 Constant WM_SHOWWINDOW
26 Constant WM_SETTINGCHANGE
26 Constant WM_WININICHANGE
27 Constant WM_DEVMODECHANGE
28 Constant WM_ACTIVATEAPP
29 Constant WM_FONTCHANGE
30 Constant WM_TIMECHANGE
31 Constant WM_CANCELMODE
32 Constant WM_SETCURSOR
33 Constant WM_MOUSEACTIVATE
34 Constant WM_CHILDACTIVATE
35 Constant WM_QUEUESYNC
36 Constant WM_GETMINMAXINFO
38 Constant WM_PAINTICON
39 Constant WM_ICONERASEBKGND
40 Constant WM_NEXTDLGCTL
42 Constant WM_SPOOLERSTATUS
43 Constant WM_DRAWITEM
44 Constant WM_MEASUREITEM
45 Constant WM_DELETEITEM
46 Constant WM_VKEYTOITEM
47 Constant WM_CHARTOITEM
48 Constant WM_SETFONT
49 Constant WM_GETFONT
50 Constant WM_SETHOTKEY
51 Constant WM_GETHOTKEY

55 Constant WM_QUERYDRAGICON
57 Constant WM_COMPAREITEM
65 Constant WM_COMPACTING
70 Constant WM_WINDOWPOSCHANGING
71 Constant WM_WINDOWPOSCHANGED
72 Constant WM_POWER
74 Constant WM_COPYDATA
75 Constant WM_CANCELJOURNAL
78 Constant WM_NOTIFY
80 Constant WM_INPUTLANGCHANGEREQUEST
81 Constant WM_INPUTLANGCHANGE
82 Constant WM_TCARD
83 Constant WM_HELP
84 Constant WM_USERCHANGED
85 Constant WM_NOTIFYFORMAT

123 Constant WM_CONTEXTMENU
124 Constant WM_STYLECHANGING
125 Constant WM_STYLECHANGED
126 Constant WM_DISPLAYCHANGE
127 Constant WM_GETICON
128 Constant WM_SETICON
129 Constant WM_NCCREATE
130 Constant WM_NCDESTROY
131 Constant WM_NCCALCSIZE
132 Constant WM_NCHITTEST
133 Constant WM_NCPAINT
134 Constant WM_NCACTIVATE
135 Constant WM_GETDLGCODE

160 Constant WM_NCMOUSEMOVE
161 Constant WM_NCLBUTTONDOWN
162 Constant WM_NCLBUTTONUP
163 Constant WM_NCLBUTTONDBLCLK
164 Constant WM_NCRBUTTONDOWN
165 Constant WM_NCRBUTTONUP
166 Constant WM_NCRBUTTONDBLCLK
167 Constant WM_NCMBUTTONDOWN
168 Constant WM_NCMBUTTONUP
169 Constant WM_NCMBUTTONDBLCLK

 256 Constant WM_KEYDOWN
 257 Constant WM_KEYUP
 258 Constant WM_CHAR
 259 Constant WM_DEADCHAR
 260 Constant WM_SYSKEYDOWN
 261 Constant WM_SYSKEYUP
 262 Constant WM_SYSCHAR
 263 Constant WM_SYSDEADCHAR

 269 Constant WM_IME_STARTCOMPOSITION
 270 Constant WM_IME_ENDCOMPOSITION
 271 Constant WM_IME_COMPOSITION
 272 Constant WM_INITDIALOG
 273 Constant WM_COMMAND
 274 Constant WM_SYSCOMMAND
 275 Constant WM_TIMER
 276 Constant WM_HSCROLL
 277 Constant WM_VSCROLL
 278 Constant WM_INITMENU
 279 Constant WM_INITMENUPOPUP
 287 Constant WM_MENUSELECT
 288 Constant WM_MENUCHAR
 289 Constant WM_ENTERIDLE

 306 Constant WM_CTLCOLORMSGBOX
 307 Constant WM_CTLCOLOREDIT
 308 Constant WM_CTLCOLORLISTBOX
 309 Constant WM_CTLCOLORBTN
 310 Constant WM_CTLCOLORDLG
 311 Constant WM_CTLCOLORSCROLLBAR
 312 Constant WM_CTLCOLORSTATIC

 512 Constant WM_MOUSEMOVE
 513 Constant WM_LBUTTONDOWN
 514 Constant WM_LBUTTONUP
 515 Constant WM_LBUTTONDBLCLK
 516 Constant WM_RBUTTONDOWN
 517 Constant WM_RBUTTONUP
 518 Constant WM_RBUTTONDBLCLK
 519 Constant WM_MBUTTONDOWN
 520 Constant WM_MBUTTONUP
 521 Constant WM_MBUTTONDBLCLK
 522 Constant WM_MOUSEWHEEL

 528 Constant WM_PARENTNOTIFY
 529 Constant WM_ENTERMENULOOP
 530 Constant WM_EXITMENULOOP
 532 Constant WM_SIZING
 533 Constant WM_CAPTURECHANGED
 534 Constant WM_MOVING
 536 Constant WM_POWERBROADCAST
 537 Constant WM_DEVICECHANGE

 544 Constant WM_MDICREATE
 545 Constant WM_MDIDESTROY
 546 Constant WM_MDIACTIVATE
 547 Constant WM_MDIRESTORE
 548 Constant WM_MDINEXT
 549 Constant WM_MDIMAXIMIZE
 550 Constant WM_MDITILE
 551 Constant WM_MDICASCADE
 552 Constant WM_MDIICONARRANGE
 553 Constant WM_MDIGETACTIVE
 560 Constant WM_MDISETMENU
 561 Constant WM_ENTERSIZEMOVE
 562 Constant WM_EXITSIZEMOVE
 563 Constant WM_DROPFILES
 564 Constant WM_MDIREFRESHMENU

 641 Constant WM_IME_SETCONTEXT
 642 Constant WM_IME_NOTIFY
 643 Constant WM_IME_CONTROL
 644 Constant WM_IME_COMPOSITIONFULL
 645 Constant WM_IME_SELECT
 646 Constant WM_IME_CHAR

 656 Constant WM_IME_KEYDOWN
 657 Constant WM_IME_KEYUP

 768 Constant WM_CUT
 769 Constant WM_COPY
 770 Constant WM_PASTE
 771 Constant WM_CLEAR
 772 Constant WM_UNDO
 773 Constant WM_RENDERFORMAT
 774 Constant WM_RENDERALLFORMATS
 775 Constant WM_DESTROYCLIPBOARD
 776 Constant WM_DRAWCLIPBOARD
 777 Constant WM_PAINTCLIPBOARD
 778 Constant WM_VSCROLLCLIPBOARD
 779 Constant WM_SIZECLIPBOARD
 780 Constant WM_ASKCBFORMATNAME
 781 Constant WM_CHANGECBCHAIN
 782 Constant WM_HSCROLLCLIPBOARD
 783 Constant WM_QUERYNEWPALETTE
 784 Constant WM_PALETTEISCHANGING
 785 Constant WM_PALETTECHANGED
 786 Constant WM_HOTKEY
 791 Constant WM_PRINT
 792 Constant WM_PRINTCLIENT

 1024 Constant WM_USER

 1024 Constant WM_PSD_PAGESETUPDLG
 1025 Constant WM_CHOOSEFONT_GETLOGFONT
 1025 Constant WM_PSD_FULLPAGERECT
 1026 Constant WM_PSD_MINMARGINRECT
 1027 Constant WM_PSD_MARGINRECT
 1028 Constant WM_PSD_GREEKTEXTRECT
 1029 Constant WM_PSD_ENVSTAMPRECT
 1030 Constant WM_PSD_YAFULLPAGERECT

 1125 Constant WM_CHOOSEFONT_SETLOGFONT
 1126 Constant WM_CHOOSEFONT_SETFLAGS

\ GetClipboardFormat, SetClipboardData                 23jul97py

   1 Constant CF_TEXT
   2 Constant CF_BITMAP
   3 Constant CF_METAFILEPICT
   4 Constant CF_SYLK
   5 Constant CF_DIF
   6 Constant CF_TIFF
   7 Constant CF_OEMTEXT
   8 Constant CF_DIB
   9 Constant CF_PALETTE
  10 Constant CF_PENDATA
  11 Constant CF_RIFF
  12 Constant CF_WAVE
  13 Constant CF_UNICODETEXT
  14 Constant CF_ENHMETAFILE
  15 Constant CF_HDROP
  16 Constant CF_LOCALE
 128 Constant CF_OWNERDISPLAY
 130 Constant CF_DSPBITMAP
 142 Constant CF_DSPENHMETAFILE
 131 Constant CF_DSPMETAFILEPICT
 129 Constant CF_DSPTEXT
 512 Constant CF_PRIVATEFIRST
 767 Constant CF_PRIVATELAST
 768 Constant CF_GDIOBJFIRST
1023 Constant CF_GDIOBJLAST

/* GlobalAlloc, GlobalFlags */
    0 Constant GMEM_FIXED
    2 Constant GMEM_MOVEABLE
   16 Constant GMEM_NOCOMPACT
   32 Constant GMEM_NODISCARD
   64 Constant GPTR
   64 Constant GMEM_ZEROINIT
   66 Constant GHND
  256 Constant GMEM_DISCARDABLE
  255 Constant GMEM_LOCKCOUNT
 4096 Constant GMEM_LOWER
 4096 Constant GMEM_NOT_BANKED
 8192 Constant GMEM_DDESHARE
 8192 Constant GMEM_SHARE
16384 Constant GMEM_NOTIFY
16384 Constant GMEM_DISCARDED
32768 Constant GMEM_INVALID_HANDLE

\ WNDCLASS structure                                   24jul97py

    1 Constant CS_VREDRAW
    2 Constant CS_HREDRAW
    4 Constant CS_KEYCVTWINDOW
    8 Constant CS_DBLCLKS
   30 Constant DLGWINDOWEXTRA
   32 Constant CS_OWNDC
   64 Constant CS_CLASSDC
  128 Constant CS_PARENTDC
  256 Constant CS_NOKEYCVT
  512 Constant CS_NOCLOSE
 2048 Constant CS_SAVEBITS
 4096 Constant CS_BYTEALIGNCLIENT
 8192 Constant CS_BYTEALIGNWINDOW
16384 Constant CS_GLOBALCLASS

/* CreateWindow */

$00000000 Constant WS_OVERLAPPED
$00000000 Constant WS_TILED
$00010000 Constant WS_MAXIMIZEBOX
$00010000 Constant WS_TABSTOP
$00020000 Constant WS_MINIMIZEBOX
$00020000 Constant WS_GROUP
$00040000 Constant WS_SIZEBOX
$00040000 Constant WS_THICKFRAME
$00080000 Constant WS_SYSMENU
$00100000 Constant WS_HSCROLL
$00200000 Constant WS_VSCROLL
$00400000 Constant WS_DLGFRAME
$00800000 Constant WS_BORDER
$00c00000 Constant WS_CAPTION
$00cf0000 Constant WS_OVERLAPPEDWINDOW
$00cf0000 Constant WS_TILEDWINDOW
$01000000 Constant WS_MAXIMIZE
$02000000 Constant WS_CLIPCHILDREN
$04000000 Constant WS_CLIPSIBLINGS
$08000000 Constant WS_DISABLED
$10000000 Constant WS_VISIBLE
$20000000 Constant WS_ICONIC
$20000000 Constant WS_MINIMIZE
$40000000 Constant WS_CHILD
$40000000 Constant WS_CHILDWINDOW
$80000000 Constant CW_USEDEFAULT
$80000000 Constant WS_POPUP
$80880000 Constant WS_POPUPWINDOW

\ ShowWindow                                           24jul97py

 0 Constant SW_HIDE
 1 Constant SW_SHOWNORMAL
 1 Constant SW_NORMAL
 2 Constant SW_SHOWMINIMIZED
 3 Constant SW_SHOWMAXIMIZED
 3 Constant SW_MAXIMIZE
 4 Constant SW_SHOWNOACTIVATE
 5 Constant SW_SHOW
 6 Constant SW_MINIMIZE
 7 Constant SW_SHOWMINNOACTIVE
 8 Constant SW_SHOWNA
 9 Constant SW_RESTORE
10 Constant SW_SHOWDEFAULT

\ ExtTextOut                                           24jul97py

2 Constant ETO_OPAQUE
4 Constant ETO_CLIPPED
16 Constant ETO_GLYPH_INDEX
128 Constant ETO_RTLREADING

\ SetBkMode                                            24jul97py

1 Constant TRANSPARENT
2 Constant OPAQUE

\ GetDeviceCaps                                        30jul97py

12 Constant BITSPIXEL

\ CreateDIBitmap                                       30jul97py

4 Constant CBM_INIT
1 Constant DIB_PAL_COLORS
0 Constant DIB_RGB_COLORS

\ BITMAPINFOHEADER structure                           30jul97py

0 Constant BI_RGB
1 Constant BI_RLE8
2 Constant BI_RLE4
3 Constant BI_BITFIELDS

\ GetClassLong, GetClassWord                           05aug97py

-32 Constant GCW_ATOM
-20 Constant GCL_CBCLSEXTRA
-18 Constant GCL_CBWNDEXTRA
-10 Constant GCL_HBRBACKGROUND
-12 Constant GCL_HCURSOR
-14 Constant GCL_HICON
-34 Constant GCL_HICONSM
-16 Constant GCL_HMODULE
-08 Constant GCL_MENUNAME
-26 Constant GCL_STYLE
-24 Constant GCL_WNDPROC

\ GetSysColor                                          19aug97py

0 Constant COLOR_SCROLLBAR
1 Constant COLOR_DESKTOP
1 Constant COLOR_BACKGROUND
2 Constant COLOR_ACTIVECAPTION
3 Constant COLOR_INACTIVECAPTION
4 Constant COLOR_MENU
5 Constant COLOR_WINDOW
6 Constant COLOR_WINDOWFRAME
7 Constant COLOR_MENUTEXT
8 Constant COLOR_WINDOWTEXT
9 Constant COLOR_CAPTIONTEXT
10 Constant COLOR_ACTIVEBORDER
11 Constant COLOR_INACTIVEBORDER
12 Constant COLOR_APPWORKSPACE
13 Constant COLOR_HIGHLIGHT
14 Constant COLOR_HIGHLIGHTTEXT
15 Constant COLOR_BTNFACE
15 Constant COLOR_3DFACE
16 Constant COLOR_3DSHADOW
16 Constant COLOR_BTNSHADOW
17 Constant COLOR_GRAYTEXT
18 Constant COLOR_BTNTEXT
19 Constant COLOR_INACTIVECAPTIONTEXT
20 Constant COLOR_3DHILIGHT
20 Constant COLOR_BTNHILIGHT
20 Constant COLOR_BTNHIGHLIGHT
21 Constant COLOR_3DDKSHADOW
22 Constant COLOR_3DLIGHT
23 Constant COLOR_INFOTEXT
24 Constant COLOR_INFOBK

\ Virtual Key codes                                    15sep97py

16 Constant VK_SHIFT
17 Constant VK_CONTROL
18 Constant VK_MENU

\ CreatePolygonRgn                                     15sep97py

1 Constant ALTERNATE
2 Constant WINDING

\ SetErrorMode                                         29sep97py

    1 Constant SEM_FAILCRITICALERRORS
    4 Constant SEM_NOALIGNMENTFAULTEXCEPT
    2 Constant SEM_NOGPFAULTERRORBOX
$8000 Constant SEM_NOOPENFILEERRORBOX

\ SetUnhandledExceptionFilter                          29sep97py

 1 Constant EXCEPTION_EXECUTE_HANDLER
-1 Constant EXCEPTION_CONTINUE_EXECUTION
 0 Constant EXCEPTION_CONTINUE_SEARCH

\ SWP_xxx

$0001 Constant SWP_NOSIZE          
$0002 Constant SWP_NOMOVE          
$0004 Constant SWP_NOZORDER        
$0008 Constant SWP_NOREDRAW        
$0010 Constant SWP_NOACTIVATE      
$0020 Constant SWP_FRAMECHANGED    
$0040 Constant SWP_SHOWWINDOW      
$0080 Constant SWP_HIDEWINDOW      
$0100 Constant SWP_NOCOPYBITS      
$0200 Constant SWP_NOOWNERZORDER   
$0400 Constant SWP_NOSENDCHANGING  
SWP_FRAMECHANGED Constant SWP_DRAWFRAME       
SWP_NOOWNERZORDER Constant SWP_NOREPOSITION    
$2000 Constant SWP_DEFERERASE      
$4000 Constant SWP_ASYNCWINDOWPOS  

\ CreateWindowEx

$10    Constant WS_EX_ACCEPTFILES           
$40000 Constant WS_EX_APPWINDOW     
$200   Constant WS_EX_CLIENTEDGE            
$400   Constant WS_EX_CONTEXTHELP           
$10000 Constant WS_EX_CONTROLPARENT     
$1     Constant WS_EX_DLGMODALFRAME     
0      Constant WS_EX_LEFT                  
$4000  Constant WS_EX_LEFTSCROLLBAR     
0      Constant WS_EX_LTRREADING            
$40    Constant WS_EX_MDICHILD          
$4     Constant WS_EX_NOPARENTNOTIFY    
$300   Constant WS_EX_OVERLAPPEDWINDOW  
$188   Constant WS_EX_PALETTEWINDOW     
$1000  Constant WS_EX_RIGHT             
0      Constant WS_EX_RIGHTSCROLLBAR    
$2000  Constant WS_EX_RTLREADING            
$20000 Constant WS_EX_STATICEDGE            
$80    Constant WS_EX_TOOLWINDOW            
$8     Constant WS_EX_TOPMOST           
$20    Constant WS_EX_TRANSPARENT           
$100   Constant WS_EX_WINDOWEDGE            

\ SetWindowPost

$FFFF  Constant HWND_BROADCAST
    1  Constant HWND_BOTTOM
   -2  Constant HWND_NOTOPMOST
    0  Constant HWND_TOP
   -1  Constant HWND_TOPMOST
    0  Constant HWND_DESKTOP

toss toss

Module;
