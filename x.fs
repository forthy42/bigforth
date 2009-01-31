/*
 * $XConsortium: X.h,v 1.69 94/04/17 20:10:48 dpw Exp $
 */

/* Definitions for the X window system likely to be used by applications */

/*

Copyright (c) 1987  X Consortium

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
X CONSORTIUM BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN
AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Except as contained in this notice, the name of the X Consortium shall not be
used in advertising or otherwise to promote the sale, use or other dealings
in this Software without prior written authorization from the X Consortium.


Copyright 1987 by Digital Equipment Corporation, Maynard, Massachusetts.

                        All Rights Reserved

Permission to use, copy, modify, and distribute this software and its 
documentation for any purpose and without fee is hereby granted, 
provided that the above copyright notice appear in all copies and that
both that copyright notice and this permission notice appear in 
supporting documentation, and that the name of Digital not be
used in advertising or publicity pertaining to distribution of the
software without specific, written prior permission.  

DIGITAL DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE, INCLUDING
ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS, IN NO EVENT SHALL
DIGITAL BE LIABLE FOR ANY SPECIAL, INDIRECT OR CONSEQUENTIAL DAMAGES OR
ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS,
WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION,
ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS
SOFTWARE.

***************************************************************** */

\ Converted to bigFORTH by Bernd Paysan

Module Xconst

base @ decimal

11 Constant X_PROTOCOL /* current protocol version */
0 Constant X_PROTOCOL_REVISION /* current minor version */

/* Resources */

/*
 * _XSERVER64 must ONLY be defined when compiling X server sources on
 * systems where unsigned long is not 32 bits, must NOT be used in
 * client or library code.
 */

/* ***************************************************************
 * RESERVED RESOURCE AND CONSTANT DEFINITIONS
 *************************************************************** */

0 Constant None /* universal null resource or null atom */

1 Constant ParentRelative /* background pixmap in CreateWindow
        and ChangeWindowAttributes */

0 Constant CopyFromParent /* border pixmap in CreateWindow
           and ChangeWindowAttributes
       special VisualID and special window
           class passed to CreateWindow */

0 Constant PointerWindow /* destination window in SendEvent */
1 Constant InputFocus /* destination window in SendEvent */

1 Constant PointerRoot /* focus window in SetInputFocus */

0 Constant AnyPropertyType /* special Atom, passed to GetProperty */

0 Constant AnyKey /* special Key Code, passed to GrabKey */

0 Constant AnyButton /* special Button Code, passed to GrabButton */

0 Constant AllTemporary /* special Resource ID passed to KillClient */

0 Constant CurrentTime /* special Time */

0 Constant NoSymbol /* special KeySym */

/* *************************************************************** 
 * EVENT DEFINITIONS 
 **************************************************************** */

/* Input Event Masks. Used as event-mask window attribute and as arguments
   to Grab requests.  Not to be confused with event names.  */

0 Constant NoEventMask 
1 0 << Constant KeyPressMask 
1 1 << Constant KeyReleaseMask 
1 2 << Constant ButtonPressMask 
1 3 << Constant ButtonReleaseMask 
1 4 << Constant EnterWindowMask 
1 5 << Constant LeaveWindowMask 
1 6 << Constant PointerMotionMask 
1 7 << Constant PointerMotionHintMask 
1 8 << Constant Button1MotionMask 
1 9 << Constant Button2MotionMask 
1 10 << Constant Button3MotionMask 
1 11 << Constant Button4MotionMask 
1 12 << Constant Button5MotionMask 
1 13 << Constant ButtonMotionMask 
1 14 << Constant KeymapStateMask 
1 15 << Constant ExposureMask 
1 16 << Constant VisibilityChangeMask 
1 17 << Constant StructureNotifyMask 
1 18 << Constant ResizeRedirectMask 
1 19 << Constant SubstructureNotifyMask 
1 20 << Constant SubstructureRedirectMask 
1 21 << Constant FocusChangeMask 
1 22 << Constant PropertyChangeMask 
1 23 << Constant ColormapChangeMask 
1 24 << Constant OwnerGrabButtonMask 

/* Event names.  Used in "type" field in XEvent structures.  Not to be
confused with event masks above.  They start from 2 because 0 and 1
are reserved in the protocol for errors and replies. */

2 Constant KeyPress 
3 Constant KeyRelease 
4 Constant ButtonPress 
5 Constant ButtonRelease 
6 Constant MotionNotify 
7 Constant EnterNotify 
8 Constant LeaveNotify 
9 Constant FocusIn 
10 Constant FocusOut 
11 Constant KeymapNotify 
12 Constant Expose 
13 Constant GraphicsExpose 
14 Constant NoExpose 
15 Constant VisibilityNotify 
16 Constant CreateNotify 
17 Constant DestroyNotify 
18 Constant UnmapNotify 
19 Constant MapNotify 
20 Constant MapRequest 
21 Constant ReparentNotify 
22 Constant ConfigureNotify 
23 Constant ConfigureRequest 
24 Constant GravityNotify 
25 Constant ResizeRequest 
26 Constant CirculateNotify 
27 Constant CirculateRequest 
28 Constant PropertyNotify 
29 Constant SelectionClear 
30 Constant SelectionRequest 
31 Constant SelectionNotify 
32 Constant ColormapNotify 
33 Constant ClientMessage 
34 Constant MappingNotify 
35 Constant LASTEvent /* must be bigger than any event # */


/* Key masks. Used as modifiers to GrabButton and GrabKey, results of QueryPointer,
   state in various key-, mouse-, and button-related events. */

1 0 << Constant ShiftMask 
1 1 << Constant LockMask 
1 2 << Constant ControlMask 
1 3 << Constant Mod1Mask 
1 4 << Constant Mod2Mask 
1 5 << Constant Mod3Mask 
1 6 << Constant Mod4Mask 
1 7 << Constant Mod5Mask 

/* modifier names.  Used to build a SetModifierMapping request or
   to read a GetModifierMapping request.  These correspond to the
   masks defined above. */
0 Constant ShiftMapIndex 
1 Constant LockMapIndex 
2 Constant ControlMapIndex 
3 Constant Mod1MapIndex 
4 Constant Mod2MapIndex 
5 Constant Mod3MapIndex 
6 Constant Mod4MapIndex 
7 Constant Mod5MapIndex 


/* button masks.  Used in same manner as Key masks above. Not to be confused
   with button names below. */

1 8 << Constant Button1Mask 
1 9 << Constant Button2Mask 
1 10 << Constant Button3Mask 
1 11 << Constant Button4Mask 
1 12 << Constant Button5Mask 

1 15 << Constant AnyModifier /* used in GrabButton, GrabKey */


/* button names. Used as arguments to GrabButton and as detail in ButtonPress
   and ButtonRelease events.  Not to be confused with button masks above.
   Note that 0 is already defined above as "AnyButton".  */

1 Constant Button1 
2 Constant Button2 
3 Constant Button3 
4 Constant Button4 
5 Constant Button5 

/* Notify modes */

0 Constant NotifyNormal 
1 Constant NotifyGrab 
2 Constant NotifyUngrab 
3 Constant NotifyWhileGrabbed 

1 Constant NotifyHint /* for MotionNotify events */
         
/* Notify detail */

0 Constant NotifyAncestor 
1 Constant NotifyVirtual 
2 Constant NotifyInferior 
3 Constant NotifyNonlinear 
4 Constant NotifyNonlinearVirtual 
5 Constant NotifyPointer 
6 Constant NotifyPointerRoot 
7 Constant NotifyDetailNone 

/* Visibility notify */

0 Constant VisibilityUnobscured 
1 Constant VisibilityPartiallyObscured 
2 Constant VisibilityFullyObscured 

/* Circulation request */

0 Constant PlaceOnTop 
1 Constant PlaceOnBottom 

/* protocol families */

0 Constant FamilyInternet 
1 Constant FamilyDECnet 
2 Constant FamilyChaos 

/* Property notification */

0 Constant PropertyNewValue 
1 Constant PropertyDelete 

/* Color Map notification */

0 Constant ColormapUninstalled 
1 Constant ColormapInstalled 

/* GrabPointer, GrabButton, GrabKeyboard, GrabKey Modes */

0 Constant GrabModeSync 
1 Constant GrabModeAsync 

/* GrabPointer, GrabKeyboard reply status */

0 Constant GrabSuccess 
1 Constant AlreadyGrabbed 
2 Constant GrabInvalidTime 
3 Constant GrabNotViewable 
4 Constant GrabFrozen 

/* AllowEvents modes */

0 Constant AsyncPointer 
1 Constant SyncPointer 
2 Constant ReplayPointer 
3 Constant AsyncKeyboard 
4 Constant SyncKeyboard 
5 Constant ReplayKeyboard 
6 Constant AsyncBoth 
7 Constant SyncBoth 

/* Used in SetInputFocus, GetInputFocus */

None Constant RevertToNone 
PointerRoot Constant RevertToPointerRoot 
2 Constant RevertToParent 

/* ***************************************************************
 * ERROR CODES 
 **************************************************************** */

0 Constant Success /* everything's okay */
1 Constant BadRequest /* bad request code */
2 Constant BadValue /* int parameter out of range */
3 Constant BadWindow /* parameter not a Window */
4 Constant BadPixmap /* parameter not a Pixmap */
5 Constant BadAtom /* parameter not an Atom */
6 Constant BadCursor /* parameter not a Cursor */
7 Constant BadFont /* parameter not a Font */
8 Constant BadMatch /* parameter mismatch */
9 Constant BadDrawable /* parameter not a Pixmap or Window */
10 Constant BadAccess /* depending on context:
     - key/button already grabbed
     - attempt to free an illegal 
       cmap entry 
    - attempt to store into a read-only 
       color map entry.
     - attempt to modify the access control
       list from other than the local host.
    */
11 Constant BadAlloc /* insufficient resources */
12 Constant BadColor /* no such colormap */
13 Constant BadGC /* parameter not a GC */
14 Constant BadIDChoice /* choice not in range or already used */
15 Constant BadName /* font or color name doesn't exist */
16 Constant BadLength /* Request length incorrect */
17 Constant BadImplementation /* server is defective */

128 Constant FirstExtensionError 
255 Constant LastExtensionError 

/* ***************************************************************
 * WINDOW DEFINITIONS 
 **************************************************************** */

/* Window classes used by CreateWindow */
/* Note that CopyFromParent is already defined as 0 above */

1 Constant InputOutput 
2 Constant InputOnly 

/* Window attributes for CreateWindow and ChangeWindowAttributes */

1 0 << Constant CWBackPixmap 
1 1 << Constant CWBackPixel 
1 2 << Constant CWBorderPixmap 
1 3 << Constant CWBorderPixel 
1 4 << Constant CWBitGravity 
1 5 << Constant CWWinGravity 
1 6 << Constant CWBackingStore 
1 7 << Constant CWBackingPlanes 
1 8 << Constant CWBackingPixel 
1 9 << Constant CWOverrideRedirect 
1 10 << Constant CWSaveUnder 
1 11 << Constant CWEventMask 
1 12 << Constant CWDontPropagate 
1 13 << Constant CWColormap 
1 14 << Constant CWCursor 

/* ConfigureWindow structure */

1 0 << Constant CWX 
1 1 << Constant CWY 
1 2 << Constant CWWidth 
1 3 << Constant CWHeight 
1 4 << Constant CWBorderWidth 
1 5 << Constant CWSibling 
1 6 << Constant CWStackMode 


/* Bit Gravity */

0 Constant ForgetGravity 
1 Constant NorthWestGravity 
2 Constant NorthGravity 
3 Constant NorthEastGravity 
4 Constant WestGravity 
5 Constant CenterGravity 
6 Constant EastGravity 
7 Constant SouthWestGravity 
8 Constant SouthGravity 
9 Constant SouthEastGravity 
10 Constant StaticGravity 

/* Window gravity + bit gravity above */

0 Constant UnmapGravity 

/* Used in CreateWindow for backing-store hint */

0 Constant NotUseful 
1 Constant WhenMapped 
2 Constant Always 

/* Used in GetWindowAttributes reply */

0 Constant IsUnmapped 
1 Constant IsUnviewable 
2 Constant IsViewable 

/* Used in ChangeSaveSet */

0 Constant SetModeInsert 
1 Constant SetModeDelete 

/* Used in ChangeCloseDownMode */

0 Constant DestroyAll 
1 Constant RetainPermanent 
2 Constant RetainTemporary 

/* Window stacking method (in configureWindow) */

0 Constant Above 
1 Constant Below 
2 Constant TopIf 
3 Constant BottomIf 
4 Constant Opposite 

/* Circulation direction */

0 Constant RaiseLowest 
1 Constant LowerHighest 

/* Property modes */

0 Constant PropModeReplace 
1 Constant PropModePrepend 
2 Constant PropModeAppend 

/* ***************************************************************
 * GRAPHICS DEFINITIONS
 **************************************************************** */

/* graphics functions, as in GC.alu */

$0 Constant GXclear /* 0 */
$1 Constant GXand /* src AND dst */
$2 Constant GXandReverse /* src AND NOT dst */
$3 Constant GXcopy /* src */
$4 Constant GXandInverted /* NOT src AND dst */
$5 Constant GXnoop /* dst */
$6 Constant GXxor /* src XOR dst */
$7 Constant GXor /* src OR dst */
$8 Constant GXnor /* NOT src AND NOT dst */
$9 Constant GXequiv /* NOT src XOR dst */
$a Constant GXinvert /* NOT dst */
$b Constant GXorReverse /* src OR NOT dst */
$c Constant GXcopyInverted /* NOT src */
$d Constant GXorInverted /* NOT src OR dst */
$e Constant GXnand /* NOT src OR NOT dst */
$f Constant GXset /* 1 */

/* LineStyle */

0 Constant LineSolid 
1 Constant LineOnOffDash 
2 Constant LineDoubleDash 

/* capStyle */

0 Constant CapNotLast 
1 Constant CapButt 
2 Constant CapRound 
3 Constant CapProjecting 

/* joinStyle */

0 Constant JoinMiter 
1 Constant JoinRound 
2 Constant JoinBevel 

/* fillStyle */

0 Constant FillSolid 
1 Constant FillTiled 
2 Constant FillStippled 
3 Constant FillOpaqueStippled 

/* fillRule */

0 Constant EvenOddRule 
1 Constant WindingRule 

/* subwindow mode */

0 Constant ClipByChildren 
1 Constant IncludeInferiors 

/* SetClipRectangles ordering */

0 Constant Unsorted 
1 Constant YSorted 
2 Constant YXSorted 
3 Constant YXBanded 

/* CoordinateMode for drawing routines */

0 Constant CoordModeOrigin /* relative to the origin */
1 Constant CoordModePrevious /* relative to previous point */

/* Polygon shapes */

0 Constant Complex /* paths may intersect */
1 Constant Nonconvex /* no paths intersect, but not convex */
2 Constant Convex /* wholly convex */

/* Arc modes for PolyFillArc */

0 Constant ArcChord /* join endpoints of arc */
1 Constant ArcPieSlice /* join endpoints to center of arc */

/* GC components: masks used in CreateGC, CopyGC, ChangeGC, OR'ed into
   GC.stateChanges */

1 0 << Constant GCFunction 
1 1 << Constant GCPlaneMask 
1 2 << Constant GCForeground 
1 3 << Constant GCBackground 
1 4 << Constant GCLineWidth 
1 5 << Constant GCLineStyle 
1 6 << Constant GCCapStyle 
1 7 << Constant GCJoinStyle 
1 8 << Constant GCFillStyle 
1 9 << Constant GCFillRule 
1 10 << Constant GCTile 
1 11 << Constant GCStipple 
1 12 << Constant GCTileStipXOrigin 
1 13 << Constant GCTileStipYOrigin 
1 14 << Constant GCFont 
1 15 << Constant GCSubwindowMode 
1 16 << Constant GCGraphicsExposures 
1 17 << Constant GCClipXOrigin 
1 18 << Constant GCClipYOrigin 
1 19 << Constant GCClipMask 
1 20 << Constant GCDashOffset 
1 21 << Constant GCDashList 
1 22 << Constant GCArcMode 

22 Constant GCLastBit 
/* ***************************************************************
 * FONTS 
 **************************************************************** */

/* used in QueryFont -- draw direction */

0 Constant FontLeftToRight 
1 Constant FontRightToLeft 

255 Constant FontChange 

/* ***************************************************************
 *  IMAGING 
 **************************************************************** */

/* ImageFormat -- PutImage, GetImage */

0 Constant XYBitmap /* depth 1, XYFormat */
1 Constant XYPixmap /* depth == drawable depth */
2 Constant ZPixmap /* depth == drawable depth */

/* ***************************************************************
 *  COLOR MAP STUFF 
 **************************************************************** */

/* For CreateColormap */

0 Constant AllocNone /* create map with no entries */
1 Constant AllocAll /* allocate entire map writeable */


/* Flags used in StoreNamedColor, StoreColors */

1 0 << Constant DoRed 
1 1 << Constant DoGreen 
1 2 << Constant DoBlue 

/* ***************************************************************
 * CURSOR STUFF
 **************************************************************** */

/* QueryBestSize Class */

0 Constant CursorShape /* largest size that can be displayed */
1 Constant TileShape /* size tiled fastest */
2 Constant StippleShape /* size stippled fastest */

/* *************************************************************** 
 * KEYBOARD/POINTER STUFF
 **************************************************************** */

0 Constant AutoRepeatModeOff 
1 Constant AutoRepeatModeOn 
2 Constant AutoRepeatModeDefault 

0 Constant LedModeOff 
1 Constant LedModeOn 

/* masks for ChangeKeyboardControl */

1 0 << Constant KBKeyClickPercent 
1 1 << Constant KBBellPercent 
1 2 << Constant KBBellPitch 
1 3 << Constant KBBellDuration 
1 4 << Constant KBLed 
1 5 << Constant KBLedMode 
1 6 << Constant KBKey 
1 7 << Constant KBAutoRepeatMode 

0 Constant MappingSuccess 
1 Constant MappingBusy 
2 Constant MappingFailed 

0 Constant MappingModifier 
1 Constant MappingKeyboard 
2 Constant MappingPointer 

/* ***************************************************************
 * SCREEN SAVER STUFF 
 **************************************************************** */

0 Constant DontPreferBlanking 
1 Constant PreferBlanking 
2 Constant DefaultBlanking 

0 Constant DisableScreenSaver 
0 Constant DisableScreenInterval 

0 Constant DontAllowExposures 
1 Constant AllowExposures 
2 Constant DefaultExposures 

/* for ForceScreenSaver */

0 Constant ScreenSaverReset 
1 Constant ScreenSaverActive 

/* ***************************************************************
 * HOSTS AND CONNECTIONS
 **************************************************************** */

/* for ChangeHosts */

0 Constant HostInsert 
1 Constant HostDelete 

/* for ChangeAccessControl */

1 Constant EnableAccess 
0 Constant DisableAccess 

/* Display classes  used in opening the connection 
 * Note that the statically allocated ones are even numbered and the
 * dynamically changeable ones are odd numbered */

0 Constant StaticGray 
1 Constant GrayScale 
2 Constant StaticColor 
3 Constant PseudoColor 
4 Constant TrueColor 
5 Constant DirectColor 


/* Byte order  used in imageByteOrder and bitmapBitOrder */

0 Constant LSBFirst 
1 Constant MSBFirst 

/* $XConsortium: Xlib.h,v 11.237 94/09/01 18:44:49 kaleb Exp $ */
/* $XFree86: xc/lib/X11/Xlib.h,v 3.2 1994/09/17 13:44:15 dawes Exp $ */
/* 

Copyright (c) 1985, 1986, 1987, 1991  X Consortium

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
X CONSORTIUM BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN
AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Except as contained in this notice, the name of the X Consortium shall not be
used in advertising or otherwise to promote the sale, use or other dealings
in this Software without prior written authorization from the X Consortium.

 */


/*
 * Xlib.h - Header definition and support file for the C subroutine
 * interface library (Xlib) to the X Window System Protocol (V11).
 * Structures and symbols starting with "_" are private to the library.
 */

6 Constant XlibSpecificationRelease 

0 Constant QueuedAlready 
1 Constant QueuedAfterReading 
2 Constant QueuedAfterFlush 

/*
 * Extensions need a way to hang private data on some structures.
 */
struct{
 cell number       /* number returned by XRegisterExtension */
 ptr  next         /* next item on list of data for structure */
 ptr  free_private /* called to free private storage */
 ptr  private_data /* data private to this extension. */
} XExtData

/*
 * This file contains structures used by the extension mechanism.
 */
struct{  /* public to extension, cannot be changed */
 cell extension  /* extension number */
 cell major_opcode /* major op-code assigned by server */
 cell first_event /* first event number for the extension */
 cell first_error /* first error number for the extension */
} XExtCodes

/*
 * Data structure for retrieving info about pixmap formats.
 */

struct{
    cell depth
    cell bits_per_pixel
    cell scanline_pad
} XPixmapFormatValues


/*
 * Data structure for setting graphics context.
 */
struct{
 cell function  /* logical operation */
 cell plane_mask /* plane mask */
 cell foreground /* foreground pixel */
 cell background /* background pixel */
 cell line_width  /* line width */
 cell line_style   /* LineSolid, LineOnOffDash, LineDoubleDash */
 cell cap_style    /* CapNotLast, CapButt,  CapRound, CapProjecting */
 cell join_style   /* JoinMiter, JoinRound, JoinBevel */
 cell fill_style   /* FillSolid, FillTiled,  FillStippled, FillOpaeueStippled */
 cell fill_rule    /* EvenOddRule, WindingRule */
 cell arc_mode  /* ArcChord, ArcPieSlice */
 cell tile  /* tile pixmap for tiling operations */
 cell stipple  /* stipple 1 plane pixmap for stipping */
 cell ts_x_origin /* offset for tile or stipple operations */
 cell ts_y_origin
 ptr font         /* default text font for text operations */
 cell subwindow_mode     /* ClipByChildren, IncludeInferiors */
 cell graphics_exposures /* boolean, should exposures be generated */
 cell clip_x_origin /* origin for clipping */
 cell clip_y_origin
 cell clip_mask /* bitmap clipping; other calls for rects */
 cell dash_offset /* patterned/dashed line information */
 byte dashes
} XGCValues

/*
 * Graphics context.  The contents of this structure are implementation
 * dependent.  A GC should be treated as opaque by application code.
 */

struct{
    ptr ext_data /* hook for extension to hang data */
    ptr gid /* protocol ID for graphics context */
    /* there is more to this structure, but it is private to Xlib */
} GC

/*
 * Visual structure contains information about colormapping possible.
 */
struct{
 ptr ext_data /* hook for extension to hang data */
 cell visualid /* visual id of this visual */
 cell class  /* class of screen (monochrome, etc.) */
 cell red_mask cell green_mask cell blue_mask /* mask values */
 cell bits_per_rgb /* log base 2 of distinct color values */
 cell map_entries /* color map entries */
} Visual

/*
 * Depth structure contains information for each possible depth.
 */ 
struct{
 cell depth  /* this depth (Z) of the depth */
 cell nvisuals  /* number of Visual types at this depth */
 ptr visuals /* list of visuals possible at this depth */
} Depth

/*
 * Information about the screen.  The contents of this structure are
 * implementation dependent.  A Screen should be treated as opaque
 * by application code.
 */

struct{
 ptr ext_data /* hook for extension to hang data */
 ptr display /* back pointer to display structure */
 cell root  /* Root window id. */
 cell width   cell height /* width and height of screen */
 cell mwidth   cell mheight /* width and height of  in millimeters */
 cell ndepths  /* number of depths possible */
 ptr depths  /* list of allowable depths on the screen */
 cell root_depth  /* bits per pixel */
 ptr root_visual /* root visual */
 cell default_gc  /* GC for the root root visual */
 cell cmap  /* default color map */
 cell white_pixel
 cell black_pixel /* White and Black pixel values */
 cell max_maps   cell min_maps /* max and min color maps */
 cell backing_store /* Never, WhenMapped, Always */
 cell save_unders 
 cell root_input_mask /* initial root input mask */
} Screen

/*
 * Format structure; describes ZFormat data the screen will understand.
 */
struct{
 ptr ext_data /* hook for extension to hang data */
 cell depth  /* depth of this image format */
 cell bits_per_pixel /* bits/pixel at this depth */
 cell scanline_pad /* scanline must padded to this multiple */
} ScreenFormat

/*
 * Data structure for setting window attributes.
 */
struct{
    cell background_pixmap /* background or None or ParentRelative */
    cell background_pixel /* background pixel */
    cell border_pixmap /* border of the window */
    cell border_pixel /* border pixel value */
    cell bit_gravity  /* one of bit gravity values */
    cell win_gravity  /* one of the window gravity values */
    cell backing_store  /* NotUseful, WhenMapped, Always */
    cell backing_planes /* planes to be preseved if possible */
    cell backing_pixel /* value to use in restoring planes */
    cell save_under  /* should bits under be saved? (popups) */
    cell event_mask  /* set of events that should be saved */
    cell do_not_propagate_mask /* set of events that should not propagate */
    cell override_redirect /* boolean value for override-redirect */
    cell colormap  /* color map to be associated with window */
    cell cursor  /* cursor to be displayed (or None) */
} XSetWindowAttributes

struct{
    cell x  cell y   /* location of window */
    cell width   cell height  /* width and height of window */
    cell border_width  /* border width of window */
    cell depth           /* depth of window */
    ptr visual  /* the associated visual structure */
    cell root         /* root of screen containing window */
    cell class   /* InputOutput, InputOnly */
    cell bit_gravity  /* one of bit gravity values */
    cell win_gravity  /* one of the window gravity values */
    cell backing_store  /* NotUseful, WhenMapped, Always */
    cell backing_planes /* planes to be preserved if possible */
    cell backing_pixel /* value to be used when restoring planes */
    cell save_under  /* boolean, should bits under be saved? */
    ptr colormap  /* color map to be associated with window */
    cell map_installed  /* boolean, is color map currently installed */
    cell map_state  /* IsUnmapped, IsUnviewable, IsViewable */
    cell all_event_masks /* set of events all people have interest in */
    cell your_event_mask /* my event mask */
    cell do_not_propagate_mask /* set of events that should not propagate */
    cell override_redirect /* boolean value for override-redirect */
    ptr screen  /* back pointer to correct screen */
} XWindowAttributes

/*
 * Data structure for host setting; getting routines.
 *
 */

struct{
 cell family  /* for example Familyinternet */
 cell length  /* length of address, in bytes */
 ptr address  /* pointer to where to find the bytes */
} XHostAddress

/*
 * Data structure for "image" data, used by image manipulation routines.
 */
struct{
    cell width   cell height  /* size of image */
    cell xoffset  /* number of pixels offset in X direction */
    cell format   /* XYBitmap, XYPixmap, ZPixmap */
    ptr data   /* pointer to image data */
    cell byte_order  /* data byte order, LSBFirst, MSBFirst */
    cell bitmap_unit  /* quant. of scanline 8, 16, 32 */
    cell bitmap_bit_order /* LSBFirst, MSBFirst */
    cell bitmap_pad  /* 8, 16, 32 either XY or ZPixmap */
    cell depth   /* depth of image */
    cell bytes_per_line  /* accelarator to next line */
    cell bits_per_pixel  /* bits per pixel (ZPixmap) */
    cell red_mask /* bits in z arrangment */
    cell green_mask
    cell blue_mask
    ptr obdata  /* hook for the object routines to hang on */
    {  /* image manipulation routines */
       ptr create_image
       ptr destroy_image
       ptr get_pixel
       ptr put_pixel
       ptr sub_image
       ptr add_pixel
    }
} XImage

/* 
 * Data structure for XReconfigureWindow
 */
struct{
    cell x  cell y
    cell width   cell height
    cell border_width
    cell sibling
    cell stack_mode
} XWindowChanges

/*
 * Data structure used by color operations
 */
struct{
 cell pixel
 short red  short green  short blue
 byte flags  /* do_red, do_green, do_blue */
 byte pad
} XColor

/* 
 * Data structures for graphics operations.  On most machines, these are
 * congruent with the wire protocol structures, so reformatting the data
 * can be avoided on these architectures.
 */
struct{
    short x1  short  y1  short x2  short y2
} XSegment

struct{
    short x   short y
} XPoint
    
struct{
    short x   short y
    short width   short height
} XRectangle
    
struct{
    short x   short y
    short width   short height
    short angle1  short angle2
} XArc


/* Data structure for XChangeKeyboardControl */

struct{
        cell key_click_percent
        cell bell_percent
        cell bell_pitch
        cell bell_duration
        cell led
        cell led_mode
        cell key
        cell auto_repeat_mode   /* On, Off, Default */
} XKeyboardControl

/* Data structure for XGetKeyboardControl */

struct{
 cell key_click_percent
 cell bell_percent
 cell bell_pitch   cell bell_duration
 cell led_mask
 cell global_auto_repeat
 32 string auto_repeats
} XKeyboardState

/* Data structure for XGetMotionEvents.  */

struct{
        cell time
 short x   short y
} XTimeCoord

/* Data structure for X{Set,Get}ModifierMapping */

struct{
  cell max_keypermod /* The server's max # of keys per modifier */
  ptr modifiermap /* An 8 by max_keypermod array of modifiers */
} XModifierKeymap


/*
 * Display datatype maintaining display specific data.
 * The contents of this structure are implementation dependent.
 * A Display should be treated as opaque by application code.
 */

struct{
 ptr ext_data /* hook for extension to hang data */
 ptr  private1
 cell fd   /* Network socket. */
 cell private2
 cell proto_major_version /* major version of server's X protocol */
 cell proto_minor_version /* minor version of servers X protocol */
 ptr vendor  /* vendor of the server hardware */
 ptr private3
 ptr private4
 ptr private5
 cell private6
 ptr resource_alloc /* allocator function */
 cell byte_order  /* screen byte order, LSBFirst, MSBFirst */
 cell bitmap_unit /* padding and data requirements */
 cell bitmap_pad  /* padding requirements on bitmaps */
 cell bitmap_bit_order /* LeastSignificant or MostSignificant */
 cell nformats  /* number of pixmap formats in list */
 ptr pixmap_format /* pixmap format list */
 cell private8
 cell release  /* release of the server */
 ptr private9  ptr private10
 cell qlen  /* Length of input event queue */
 cell last_request_read /* seq number of last event read */
 cell request /* sequence number of last request. */
 ptr private11
 ptr private12
 ptr private13
 ptr private14
 cell max_request_size /* maximum number 32 bit words in request */
 ptr db
 ptr private15
 ptr display_name /* "host:display" string used on this connect */
 cell default_screen /* default screen for operations */
 cell nscreens  /* number of screens on this server */
 ptr screens /* pointer to list of screens */
 cell motion_buffer /* size of motion buffer */
 cell private16
 cell min_keycode /* minimum defined keycode */
 cell max_keycode /* maximum defined keycode */
 ptr private17
 ptr private18
 cell private19
 ptr xdefaults /* contents of defaults from server */
 /* there is more to this structure, but it is private to Xlib */
} Display

/*
 * Definitions of specific events.
 */
struct{
 cell type  /* of event */
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell window         /* "event" window it is reported relative to */
 cell root         /* root window that the event occured on */
 cell subwindow /* child window */
 cell time  /* milliseconds */
 cell x  cell y  /* pointer x, y coordinates in event window */
 cell x_root  cell y_root /* coordinates relative to root */
 cell state /* key or button mask */
 cell keycode /* detail */
 cell same_screen /* same screen flag */
} XKeyEvent
[defined] VFXFORTH [IF]
    synonym XKeyPressedEvent XKeyEvent
    synonym XKeyReleasedEvent XKeyEvent
[ELSE]
    ' XKeyEvent Alias XKeyPressedEvent immediate
    ' XKeyEvent Alias XKeyReleasedEvent immediate
[THEN]
struct{
 cell type  /* of event */
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell window         /* "event" window it is reported relative to */
 cell root         /* root window that the event occured on */
 cell subwindow /* child window */
 cell time  /* milliseconds */
 cell x  cell y  /* pointer x, y coordinates in event window */
 cell x_root  cell y_root /* coordinates relative to root */
 cell state /* key or button mask */
 cell button /* detail */
 cell same_screen /* same screen flag */
} XButtonEvent
[defined] VFXFORTH [IF]
    synonym XButtonPressedEvent XButtonEvent
    synonym XButtonReleasedEvent XButtonEvent
[ELSE]
    ' XButtonEvent Alias XButtonPressedEvent immediate
    ' XButtonEvent Alias XButtonReleasedEvent immediate
[THEN]
struct{
 cell type  /* of event */
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell window         /* "event" window reported relative to */
 cell root         /* root window that the event occured on */
 cell subwindow /* child window */
 cell time  /* milliseconds */
 cell x  cell y  /* pointer x, y coordinates in event window */
 cell x_root  cell y_root /* coordinates relative to root */
 cell state /* key or button mask */
 byte is_hint  /* detail */
 cell same_screen /* same screen flag */
} XMotionEvent
[defined] VFXFORTH [IF]
synonym XPointerMovedEvent XMotionEvent
[ELSE]
' XMotionEvent Alias XPointerMovedEvent immediate
[THEN]

struct{
 cell type  /* of event */
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell window         /* "event" window reported relative to */
 cell root         /* root window that the event occured on */
 cell subwindow /* child window */
 cell time  /* milliseconds */
 cell x  cell y  /* pointer x, y coordinates in event window */
 cell x_root  cell y_root /* coordinates relative to root */
 cell mode  /* NotifyNormal, NotifyGrab, NotifyUngrab */
 cell detail
 /*
  * NotifyAncestor, NotifyVirtual, NotifyInferior, 
  * NotifyNonlinear,NotifyNonlinearVirtual
  */
 cell same_screen /* same screen flag */
 cell focus  /* boolean focus */
 cell state /* key or button mask */
} XCrossingEvent
[defined] VFXFORTH [IF]
synonym XEnterWindowEvent XCrossingEvent
synonym XLeaveWindowEvent XCrossingEvent
[ELSE]
' XCrossingEvent Alias XEnterWindowEvent immediate
' XCrossingEvent Alias XLeaveWindowEvent immediate
[THEN]

struct{
 cell type  /* FocusIn or FocusOut */
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell window  /* window of event */
 cell mode  /* NotifyNormal, NotifyGrab, NotifyUngrab */
 cell detail
 /*
  * NotifyAncestor, NotifyVirtual, NotifyInferior, 
  * NotifyNonlinear,NotifyNonlinearVirtual, NotifyPointer,
  * NotifyPointerRoot, NotifyDetailNone 
  */
} XFocusChangeEvent
[defined] VFXFORTH [IF]
synonym XFocusInEvent XFocusChangeEvent
synonym XFocusOutEvent XFocusChangeEvent
[ELSE]
' XFocusChangeEvent Alias XFocusInEvent immediate
' XFocusChangeEvent Alias XFocusOutEvent immediate
[THEN]

/* generated on EnterWindow and FocusIn  when KeyMapState selected */
struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell window
 #32 string key_vector
} XKeymapEvent 

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell window
 cell x  cell y
 cell width   cell height
 cell count  /* if non-zero, at least this many more */
} XExposeEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell drawable
 cell x  cell y
 cell width   cell height
 cell count  /* if non-zero, at least this many more */
 cell major_code  /* core is CopyArea or CopyPlane */
 cell minor_code  /* not defined in the core */
} XGraphicsExposeEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell drawable
 cell major_code  /* core is CopyArea or CopyPlane */
 cell minor_code  /* not defined in the core */
} XNoExposeEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell window
 cell state  /* Visibility state */
} XVisibilityEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell parent  /* parent of the window */
 cell window  /* window id of window created */
 cell x  cell y  /* window location */
 cell width   cell height /* size of window */
 cell border_width /* border width */
 cell override_redirect /* creation should be overridden */
} XCreateWindowEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell event
 cell window
} XDestroyWindowEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell event
 cell window
 cell from_configure
} XUnmapEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell event
 cell window
 cell override_redirect /* boolean, is override set... */
} XMapEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell parent
 cell window
} XMapRequestEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell event
 cell window
 cell parent
 cell x  cell y
 cell override_redirect
} XReparentEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell event
 cell window
 cell x  cell y
 cell width   cell height
 cell border_width
 cell above
 cell override_redirect
} XConfigureEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell event
 cell window
 cell x  cell y
} XGravityEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell window
 cell width   cell height
} XResizeRequestEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell parent
 cell window
 cell x  cell y
 cell width   cell height
 cell border_width
 cell above
 cell detail  /* Above, Below, TopIf, BottomIf, Opposite */
 cell value_mask
} XConfigureRequestEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell event
 cell window
 cell place  /* PlaceOnTop, PlaceOnBottom */
} XCirculateEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell parent
 cell window
 cell place  /* PlaceOnTop, PlaceOnBottom */
} XCirculateRequestEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell window
 cell atom
 cell time
 cell state  /* NewValue, Deleted */
} XPropertyEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell window
 cell selection
 cell time
} XSelectionClearEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell owner
 cell requestor
 cell selection
 cell target
 cell property
 cell time
} XSelectionRequestEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell requestor
 cell selection
 cell target
 cell property  /* ATOM or None */
 cell time
} XSelectionEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell window
 ptr colormap /* COLORMAP or None */
 cell new
 cell state  /* ColormapInstalled, ColormapUninstalled */
} XColormapEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell window
 cell message_type
 cell format
 #20 string data
} XClientMessageEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell window  /* unused */
 cell request  /* one of MappingModifier, MappingKeyboard, MappingPointer */
 cell first_keycode /* first keycode */
 cell count  /* defines range of change w. first_keycode */
} XMappingEvent

struct{
 cell type
 ptr display /* Display the event was read from */
 ptr resourceid  /* resource id */
 cell serial /* serial number of failed request */
 byte error_code /* error code of failed request */
 byte request_code /* Major op-code of failed request */
 byte minor_code /* Minor op-code of failed request */
} XErrorEvent

struct{
 cell type
 cell serial /* # of last request processed by server */
 cell send_event /* true if this came from a SendEvent request */
 ptr display /* Display the event was read from */
 cell window /* window on which event was requested in event mask */
} XAnyEvent

/*
 * this union is defined so Xlib can always use the same sized
 * event structure internally, to avoid memory fragmentation.
 */
struct{ {
  cell type  /* must not be changed first element */
| struct XAnyEvent xany
| struct XKeyEvent xkey
| struct XButtonEvent xbutton
| struct XMotionEvent xmotion
| struct XCrossingEvent xcrossing
| struct XFocusChangeEvent xfocus
| struct XExposeEvent xexpose
| struct XGraphicsExposeEvent xgraphicsexpose
| struct XNoExposeEvent xnoexpose
| struct XVisibilityEvent xvisibility
| struct XCreateWindowEvent xcreatewindow
| struct XDestroyWindowEvent xdestroywindow
| struct XUnmapEvent xunmap
| struct XMapEvent xmap
| struct XMapRequestEvent xmaprequest
| struct XReparentEvent xreparent
| struct XConfigureEvent xconfigure
| struct XGravityEvent xgravity
| struct XResizeRequestEvent xresizerequest
| struct XConfigureRequestEvent xconfigurerequest
| struct XCirculateEvent xcirculate
| struct XCirculateRequestEvent xcirculaterequest
| struct XPropertyEvent xproperty
| struct XSelectionClearEvent xselectionclear
| struct XSelectionRequestEvent xselectionrequest
| struct XSelectionEvent xselection
| struct XColormapEvent xcolormap
| struct XClientMessageEvent xclient
| struct XMappingEvent xmapping
| struct XErrorEvent xerror
| struct XKeymapEvent xkeymap
| #96 string pad
} } XEvent

: XAllocID ( dpy -- id )  dup >r Display resource_alloc perform rdrop ; macro

/*
 * per character font metric information.
 */
struct{
    short lbearing /* origin to left edge of raster */
    short rbearing /* origin to right edge of raster */
    short width  /* advance to next char's origin */
    short ascent  /* baseline to top edge of raster */
    short descent /* baseline to bottom edge of raster */
    short attributes /* per char flags (not predefined) */
} XCharStruct

/*
 * To allow arbitrary information with fonts, there are additional properties
 * returned.
 */
struct{
    cell name
    cell card32
} XFontProp

struct{
    ptr ext_data /* hook for extension to hang data */
    ptr        fid            /* Font id for this font */
    cell direction /* hint about direction the font is painted */
    cell min_char_or_byte2 /* first character */
    cell max_char_or_byte2 /* last character */
    cell min_byte1 /* first row that exists */
    cell max_byte1 /* last row that exists */
    cell all_chars_exist /* flag if all characters have non-zero size */
    cell default_char /* char to print for undefined character */
    cell         n_properties   /* how many properties there are */
    ptr properties /* pointer to array of additional properties */
    struct XCharStruct min_bounds /* minimum bounds over all existing char */
    struct XCharStruct max_bounds /* maximum bounds over all existing char */
    ptr per_char /* first_char to last_char information */
    cell  ascent  /* log. extent above baseline for spacing */
    cell  descent /* log. descent below baseline for spacing */
} XFontStruct

/*
 * PolyText routines take these as arguments.
 */
struct{
    ptr chars  /* pointer to string */
    cell nchars   /* number of characters */
    cell delta   /* delta between strings */
    ptr font   /* font to print it in, None don't change */
} XTextItem

struct{  /* normal 16 bit characters are two bytes */
    byte byte1
    byte byte2
} XChar2b

struct{
    ptr chars  /* two byte characters */
    cell nchars   /* number of characters */
    cell delta   /* delta between strings */
    ptr font   /* font to print it in, None don't change */
} XTextItem16


struct{ {
  ptr display
| cell gc
| ptr visual
| ptr screen
| ptr pixmap_format
| ptr font } } XEDataObject

struct{
    struct XRectangle      max_ink_extent
    struct XRectangle      max_logical_extent
} XFontSetExtents

struct{
    ptr chars
    cell nchars
    cell delta
    ptr font_set
} XmbTextItem

struct{
    ptr chars
    cell nchars
    cell delta
    ptr font_set
} XwcTextItem


struct{
    cell charset_count
    ptr charset_list
} XOMCharSetList


0 Constant XOMOrientation_LTR_TTB
1 Constant XOMOrientation_RTL_TTB
2 Constant XOMOrientation_TTB_LTR
3 Constant XOMOrientation_TTB_RTL
4 Constant XOMOrientation_Context

struct{
    cell num_orient
    ptr orient /* Input Text description */
} XOMOrientation

struct{
    cell num_font
    ptr font_struct_list
    ptr font_name_list
} XOMFontInfo

struct{
    short count_styles
    ptr supported_styles
} XIMStyles

$0001 Constant XIMPreeditArea 
$0002 Constant XIMPreeditCallbacks 
$0004 Constant XIMPreeditPosition 
$0008 Constant XIMPreeditNothing 
$0010 Constant XIMPreeditNone 
$0100 Constant XIMStatusArea 
$0200 Constant XIMStatusCallbacks 
$0400 Constant XIMStatusNothing 
$0800 Constant XIMStatusNone 

XIMPreeditPosition XIMPreeditArea or
XIMPreeditNothing or XIMPreeditNone or Constant XIMPreedit
XIMStatusArea XIMStatusNothing or XIMStatusNone or Constant XIMStatus
XIMStatus XIMPreedit or Constant XIMSupported

-1 Constant XBufferOverflow 
1 Constant XLookupNone 
2 Constant XLookupChars 
3 Constant XLookupKeySym 
4 Constant XLookupBoth 

struct{
    ptr client_data
    ptr callback
} XIMCallback

1 Constant XIMReverse 
1 1 << Constant XIMUnderline 
1 2 << Constant XIMHighlight 
1 5 << Constant XIMPrimary 
1 6 << Constant XIMSecondary 
1 7 << Constant XIMTertiary 
1 8 << Constant XIMVisibleToForward 
1 9 << Constant XIMVisibleToBackword 
1 10 << Constant XIMVisibleToCenter 

struct{
    short length
    ptr feedback
    cell encoding_is_wchar 
    ptr string
} XIMText

0 Constant XIMPreeditUnKnown 
1 Constant XIMPreeditEnable 
1 1 << Constant XIMPreeditDisable 

struct{
   cell state
} XIMPreeditStateNotifyCallback

1 Constant XIMInitialState 
1 1 << Constant XIMPreserveState 

$00000001 Constant XIMStringConversionLeftEdge 
$00000002 Constant XIMStringConversionRightEdge 
$00000004 Constant XIMStringConversionTopEdge 
$00000008 Constant XIMStringConversionBottomEdge 
$00000010 Constant XIMStringConversionConcealed 
$00000020 Constant XIMStringConversionWrapped 

struct{
    short length
    ptr feedback
    cell encoding_is_wchar 
    ptr string
} XIMStringConversionText

$0001 Constant XIMStringConversionBuffer 
$0002 Constant XIMStringConversionLine 
$0003 Constant XIMStringConversionWord 
$0004 Constant XIMStringConversionChar 

$0001 Constant XIMStringConversionSubstitution 
$0002 Constant XIMStringConversionRetrival 

struct{
    short position
    short type
    short operation
    short factor
    ptr text
} XIMStringConversionCallback

struct{
    cell caret  /* Cursor offset within pre-edit string */
    cell chg_first /* Starting change position */
    cell chg_length /* Length of the change in character count */
    ptr text
} XIMPreeditDrawCallbackStruct

 0 Constant XIMForwardChar
 1 Constant XIMBackwardChar
 2 Constant XIMForwardWord
 3 Constant XIMBackwardWord
 4 Constant XIMCaretUp
 5 Constant XIMCaretDown
 6 Constant XIMNextLine
 7 Constant XIMPreviousLine
 8 Constant XIMLineStart
 9 Constant XIMLineEnd
$A Constant XIMAbsolutePosition
$B Constant XIMDontChange

0 Constant    XIMIsInvisible /* Disable caret feedback */ 
1 Constant    XIMIsPrimary /* UI defined caret feedback */
2 Constant    XIMIsSecondary /* UI defined caret feedback */

struct{
    cell position   /* Caret offset within pre-edit string */
    cell direction /* Caret moves direction */
    cell style  /* Feedback of the caret */
} XIMPreeditCaretCallbackStruct

0 Constant    XIMTextType
1 Constant    XIMBitmapType
 
struct{
    cell type
    {
      ptr text
    | cell bitmap
    }
} XIMStatusDrawCallbackStruct

struct{
    ptr  keysym
    cell   modifier
    cell   modifier_mask
} XIMHotKeyTrigger

struct{
    cell    num_hot_key
    ptr key
} XIMHotKeyTriggers

$0001 Constant XIMHotKeyStateON 
$0002 Constant XIMHotKeyStateOFF 

struct{
    short count_values
    ptr supported_values
} XIMValuesList

: ScreenOfDisplay ( dpy scr -- ) sizeof Screen *
  swap Display screens @ + ; macro
: ConnectionNumber ( dpy -- fd )  Display fd @ ; macro
: RootWindow ( dpy scr -- w ) ScreenOfDisplay Screen root @ ; macro
: DefaultScreen ( dpy -- scr )  Display default_screen @ ; macro
: DefaultRootWindow ( dpy -- ) dup DefaultScreen RootWindow ; macro
: DefaultVisual ( dpy scr -- visual ) ScreenOfDisplay Screen root_visual @ ; macro
: DefaultGC ( dpy scr -- GC ) ScreenOfDisplay Screen default_gc @ ; macro
: BlackPixel ( dpy scr -- ) ScreenOfDisplay Screen black_pixel @ ; macro
: WhitePixel ( dpy scr -- ) ScreenOfDisplay Screen white_pixel @ ; macro
0 Constant AllPlanes
: QLength ( dpy -- n )  Display qlen @ ; macro
: DisplayWidth ( dpy scr -- w ) ScreenOfDisplay Screen width @ ; macro
: DisplayHeight ( dpy scr -- h ) ScreenOfDisplay Screen height @ ; macro
: DisplayWidthMM ( dpy scr -- mw ) ScreenOfDisplay Screen mwidth @ ; macro
: DisplayHeightMM ( dpy scr -- mh ) ScreenOfDisplay Screen mheight @ ; macro
: DisplayPlanes ( dpy scr -- n ) ScreenOfDisplay Screen root_depth @ ; macro
: DisplayCells ( dpy scr -- n ) DefaultVisual Visual map_entries @ ; macro
: ScreenCount ( dpy -- n )  Display nscreens @ ; macro
: ServerVendor ( dpy -- addr )  Display vendor @ ; macro
: ProtocolVersion ( dpy -- n )  Display proto_major_version @ ; macro
: ProtocolRevision ( dpy -- n )  Display proto_minor_version @ ; macro
: VendorRelease ( dpy -- n )  Display release @ ; macro
: DisplayString ( dpy -- addr )  Display display_name @ ; macro
: DefaultDepth ( dpy scr -- n ) ScreenOfDisplay Screen root_depth @ ; macro
: DefaultColormap ( dpy scr -- addr ) ScreenOfDisplay Screen cmap @ ; macro
: BitmapUnit ( dpy -- n )  Display bitmap_unit @ ; macro
: BitmapBitOrder ( dpy -- n )  Display bitmap_bit_order @ ; macro
: BitmapPad ( dpy -- addr )  Display bitmap_pad @ ; macro
: ImageByteOrder ( dpy -- n )  Display byte_order @ ; macro
: NextRequest ( dpy -- n ) Display request @ 1+ ; macro
: LastKnownRequestProcessed ( dpy -- n )  Display last_request_read @ ; macro

/* macros for screen oriented applications (toolkit) */

: DefaultScreenOfDisplay ( dpy -- scr )  dup DefaultScreen ScreenOfDisplay ; macro
: DefaultVisualOfScreen ( s -- visual ) Screen root_visual @ ; macro
: DisplayOfScreen ( s -- n )  Screen display @ ; macro
: RootWindowOfScreen ( s -- n )  Screen root @ ; macro
: BlackPixelOfScreen ( s -- n )  Screen black_pixel @ ; macro
: WhitePixelOfScreen ( s -- n )  Screen white_pixel @ ; macro
: DefaultColormapOfScreen ( s -- n ) Screen cmap @ ; macro
: DefaultDepthOfScreen ( s -- n )  Screen root_depth @ ; macro
: DefaultGCOfScreen ( s -- n )  Screen default_gc @ ; macro
: WidthOfScreen ( s -- n )  Screen width @ ; macro
: HeightOfScreen ( s -- n )  Screen height @ ; macro
: WidthMMOfScreen ( s -- n )  Screen mwidth @ ; macro
: HeightMMOfScreen ( s -- n )  Screen mheight @ ; macro
: PlanesOfScreen ( s -- n )  Screen root_depth @ ; macro
: CellsOfScreen ( s -- visual )  DefaultVisualOfScreen Visual map_entries @ ; macro
: MinCmapsOfScreen ( s -- n )  Screen min_maps @ ; macro
: MaxCmapsOfScreen ( s -- n )  Screen max_maps @ ; macro
: DoesSaveUnders ( s -- n )  Screen save_unders @ ; macro
: DoesBackingStore ( s -- n )  Screen backing_store @ ; macro
: EventMaskOfScreen ( s -- n )  Screen root_input_mask @ ; macro

/* You must include <X11/Xlib.h> before including this file */

/* 
 * Bitmask returned by XParseGeometry().  Each bit tells if the corresponding
 * value (x, y, width, height) was found in the parsed string.
 */
$0000 Constant NoValue
$0001 Constant XValue
$0002 Constant YValue
$0004 Constant WidthValue
$0008 Constant HeightValue
$000F Constant AllValues
$0010 Constant XNegative
$0020 Constant YNegative

/*
 * new version containing base_width, base_height, and win_gravity fields;
 * used with WM_NORMAL_HINTS.
 */
struct{
        cell flags      /* marks which fields in this structure are defined */
        cell x  cell y          /* obsolete for new window mgrs, but clients */
        cell width  cell height /* should set so old wm's don't mess up */
        cell min_width  cell min_height
        cell max_width  cell max_height
        cell width_inc  cell height_inc
        cell min_aspect.x  cell min_aspect.y
        cell max_aspect.x  cell max_aspect.y
        cell base_width  cell base_height       /* added by ICCCM version 1 */
        cell win_gravity                        /* added by ICCCM version 1 */
} XSizeHints

/*
 * The next block of definitions are for window manager properties that
 * clients and applications use for communication.
 */

/* flags argument in size hints */
1   0 << Constant USPosition /* user specified x, y */
1   1 << Constant USSize /* user specified width, height */

1   2 << Constant PPosition /* program specified position */
1   3 << Constant PSize /* program specified size */
1   4 << Constant PMinSize /* program specified minimum size */
1   5 << Constant PMaxSize /* program specified maximum size */
1   6 << Constant PResizeInc /* program specified resize increments */
1   7 << Constant PAspect /* program specified min and max aspect ratios */
1   8 << Constant PBaseSize /* program specified base for incrementing */
1   9 << Constant PWinGravity /* program specified window gravity */

/* obsolete */
PPosition PSize or PMinSize or PMaxSize or PResizeInc or PAspect or
Constant PAllHints



struct{
        cell flags      /* marks which fields in this structure are defined */
        cell input      /* does this application rely on the window manager to
                        get keyboard input? */
        cell initial_state      /* see below */
        cell icon_pixmap        /* pixmap to be used as icon */
        cell icon_window        /* window to be used as icon */
        cell icon_x  cell icon_y        /* initial position of icon */
        cell icon_mask  /* icon mask bitmap */
        cell window_group       /* id of related window group */
        /* this structure may be extended in the future */
} XWMHints

/* definition for flags of XWMHints */

1   0 << Constant InputHint
1   1 << Constant StateHint
1   2 << Constant IconPixmapHint
1   3 << Constant IconWindowHint
1   4 << Constant IconPositionHint
1   5 << Constant IconMaskHint
1   6 << Constant WindowGroupHint
InputHint StateHint or IconPixmapHint or IconWindowHint or
IconPositionHint or IconMaskHint or WindowGroupHint or Constant AllHints
1   8 << Constant XUrgencyHint

/* definitions for initial window state */
0 Constant WithdrawnState       /* for windows that are not mapped */
1 Constant NormalState  /* most applications want to start this way */
3 Constant IconicState  /* application wants to start as an icon */

/*
 * Obsolete states no longer defined by ICCCM
 */
0 Constant DontCareState        /* don't know or care */
2 Constant ZoomState    /* application wants to start zoomed */
4 Constant InactiveState        /* application believes it is seldom used; */
                        /* some wm's may put it on inactive menu */


/*
 * new structure for manipulating TEXT properties; used with WM_NAME, 
 * WM_ICON_NAME, WM_CLIENT_MACHINE, and WM_COMMAND.
 */
struct{
    cell value          /* same as Property routines */
    cell encoding                       /* prop type */
    cell format                         /* prop data format: 8, 16, or 32 */
    cell nitems         /* number of data items in value */
} XTextProperty

-1 Constant XNoMemory
-2 Constant XLocaleNotSupported
-3 Constant XConverterNotFound

0 Constant XStringStyle         /* STRING */
1 Constant XCompoundTextStyle   /* COMPOUND_TEXT */
2 Constant XTextStyle           /* text in owner's encoding (current locale) */
3 Constant XStdICCTextStyle     /* STRING, else COMPOUND_TEXT */

struct{
        cell min_width cell min_height
        cell max_width cell max_height
        cell width_inc cell height_inc
} XIconSize

struct{
        cell res_name
        cell res_class
} XClassHint

/*
 * These macros are used to give some sugar to the image routines so that
 * naive people are more comfortable with them.
 */

[defined] nxcall [IF]
: XDestroyImage ( image -- )
    dup XImage destroy_image @ 1 nxcall drop drop ;
: XGetPixel ( y x image -- pixel )
    dup XImage get_pixel @ 2 nxcall >r drop 2drop r> ;
: XPutPixel ( pixel y x image -- pixel )
    dup XImage put_pixel @ 4 nxcall >r 2drop 2drop r> ;
: XSubImage ( h w y x image -- )
    dup XImage sub_image @ 5 nxcall 2drop 2drop 2drop ;
: XAddPixel ( value image -- )
    dup XImage get_pixel @ 2 nxcall drop 2drop ;
[ELSE]
Code XDestroyImage ( image -- )
     AX push  0 XImage destroy_image AX D) call
     1 cells # SP add  AX pop  Next end-code macro

Code XGetPixel ( y x image -- pixel )
     AX push  0 XImage get_pixel AX D) call
     3 cells # SP add  Next end-code macro

Code XPutPixel ( pixel y x image -- pixel )
     AX push  0 XImage put_pixel AX D) call
     4 cells # SP add  Next end-code macro

Code XSubImage ( h w y x image -- )
     AX push  0 XImage sub_image AX D) call
     5 cells # SP add  AX pop  Next end-code macro

Code XAddPixel ( value image -- )
     AX push  0 XImage add_pixel AX D) call
     1 cells # SP add  AX pop  Next end-code macro
[THEN]

/*
 * Compose sequence status structure, used in calling XLookupString.
 */
struct{
    cell compose_ptr    /* state table pointer */
    cell chars_matched          /* match state */
} XComposeStatus

$FF08 Constant XK_BackSpace             /* back space, back char */
$FF09 Constant XK_Tab                   
$FF0A Constant XK_Linefeed              /* Linefeed, LF */
$FF0B Constant XK_Clear         
$FF0D Constant XK_Return                /* Return, enter */
$FF13 Constant XK_Pause                 /* Pause, hold */
$FF14 Constant XK_Scroll_Lock           
$FF15 Constant XK_Sys_Req               
$FF1B Constant XK_Escape                
$FFFF Constant XK_Delete                /* Delete, rubout */

$FF50 Constant XK_Home                  
$FF51 Constant XK_Left                          /* Move left, left arrow */
$FF52 Constant XK_Up                            /* Move up, up arrow */
$FF53 Constant XK_Right                 /* Move right, right arrow */
$FF54 Constant XK_Down                          /* Move down, down arrow */
$FF55 Constant XK_Prior                 /* Prior, previous */
$FF55 Constant XK_Page_Up               
$FF56 Constant XK_Next                          /* Next */
$FF56 Constant XK_Page_Down             
$FF57 Constant XK_End                           /* EOL */
$FF58 Constant XK_Begin                 /* BOL */

/* Misc Functions */

$FF60 Constant XK_Select                        /* Select, mark */
$FF61 Constant XK_Print         
$FF62 Constant XK_Execute                       /* Execute, run, do */
$FF63 Constant XK_Insert                        /* Insert, insert here */
$FF65 Constant XK_Undo                          /* Undo, oops */
$FF66 Constant XK_Redo                          /* redo, again */
$FF67 Constant XK_Menu                  
$FF68 Constant XK_Find                          /* Find, search */
$FF69 Constant XK_Cancel                        /* Cancel, stop, abort, exit */
$FF6A Constant XK_Help                          /* Help */
$FF6B Constant XK_Break         
$FF7E Constant XK_Mode_switch                   /* Character set switch */
$FF7E Constant XK_script_switch          /* Alias for mode_switch */
$FF7F Constant XK_Num_Lock              

/* Keypad Functions, keypad numbers cleverly chosen to map to ascii */

$FF80 Constant XK_KP_Space                      /* space */
$FF89 Constant XK_KP_Tab                
$FF8D Constant XK_KP_Enter                      /* enter */
$FF91 Constant XK_KP_F1                 /* PF1, KP_A, ... */
$FF92 Constant XK_KP_F2         
$FF93 Constant XK_KP_F3         
$FF94 Constant XK_KP_F4         
$FF95 Constant XK_KP_Home               
$FF96 Constant XK_KP_Left               
$FF97 Constant XK_KP_Up         
$FF98 Constant XK_KP_Right              
$FF99 Constant XK_KP_Down               
$FF9A Constant XK_KP_Prior              
$FF9A Constant XK_KP_Page_Up            
$FF9B Constant XK_KP_Next               
$FF9B Constant XK_KP_Page_Down          
$FF9C Constant XK_KP_End                
$FF9D Constant XK_KP_Begin              
$FF9E Constant XK_KP_Insert             
$FF9F Constant XK_KP_Delete             
$FFBD Constant XK_KP_Equal                      /* equals */
$FFAA Constant XK_KP_Multiply           
$FFAB Constant XK_KP_Add                
$FFAC Constant XK_KP_Separator                  /* separator, often comma */
$FFAD Constant XK_KP_Subtract           
$FFAE Constant XK_KP_Decimal            
$FFAF Constant XK_KP_Divide             

$FFB0 Constant XK_KP_0                  
$FFB1 Constant XK_KP_1                  
$FFB2 Constant XK_KP_2                  
$FFB3 Constant XK_KP_3                  
$FFB4 Constant XK_KP_4                  
$FFB5 Constant XK_KP_5                  
$FFB6 Constant XK_KP_6                  
$FFB7 Constant XK_KP_7                  
$FFB8 Constant XK_KP_8                  
$FFB9 Constant XK_KP_9                  


/*
 * Auxilliary Functions; note the duplicate definitions for left and right
 * function keys;  Sun keyboards and a few other manufactures have such
 * function key groups on the left and/or right sides of the keyboard.
 * We've not found a keyboard with more than 35 function keys total.
 */

$FFBE Constant XK_F1                    
$FFBF Constant XK_F2                    
$FFC0 Constant XK_F3                    
$FFC1 Constant XK_F4                    
$FFC2 Constant XK_F5                    
$FFC3 Constant XK_F6                    
$FFC4 Constant XK_F7                    
$FFC5 Constant XK_F8                    
$FFC6 Constant XK_F9                    
$FFC7 Constant XK_F10                   
$FFC8 Constant XK_F11                   
$FFC8 Constant XK_L1                    
$FFC9 Constant XK_F12                   
$FFC9 Constant XK_L2                    
$FFCA Constant XK_F13                   
$FFCA Constant XK_L3                    
$FFCB Constant XK_F14                   
$FFCB Constant XK_L4                    
$FFCC Constant XK_F15                   
$FFCC Constant XK_L5                    
$FFCD Constant XK_F16                   
$FFCD Constant XK_L6                    
$FFCE Constant XK_F17                   
$FFCE Constant XK_L7                    
$FFCF Constant XK_F18                   
$FFCF Constant XK_L8                    
$FFD0 Constant XK_F19                   
$FFD0 Constant XK_L9                    
$FFD1 Constant XK_F20                   
$FFD1 Constant XK_L10                   
$FFD2 Constant XK_F21                   
$FFD2 Constant XK_R1                    
$FFD3 Constant XK_F22                   
$FFD3 Constant XK_R2                    
$FFD4 Constant XK_F23                   
$FFD4 Constant XK_R3                    
$FFD5 Constant XK_F24                   
$FFD5 Constant XK_R4                    
$FFD6 Constant XK_F25                   
$FFD6 Constant XK_R5                    
$FFD7 Constant XK_F26                   
$FFD7 Constant XK_R6                    
$FFD8 Constant XK_F27                   
$FFD8 Constant XK_R7                    
$FFD9 Constant XK_F28                   
$FFD9 Constant XK_R8                    
$FFDA Constant XK_F29                   
$FFDA Constant XK_R9                    
$FFDB Constant XK_F30                   
$FFDB Constant XK_R10                   
$FFDC Constant XK_F31                   
$FFDC Constant XK_R11                   
$FFDD Constant XK_F32                   
$FFDD Constant XK_R12                   
$FFDE Constant XK_F33                   
$FFDE Constant XK_R13                   
$FFDF Constant XK_F34                   
$FFDF Constant XK_R14                   
$FFE0 Constant XK_F35                   
$FFE0 Constant XK_R15                   

/* Modifiers */

$FFE1 Constant XK_Shift_L                       /* Left shift */
$FFE2 Constant XK_Shift_R                       /* Right shift */
$FFE3 Constant XK_Control_L                     /* Left control */
$FFE4 Constant XK_Control_R                     /* Right control */
$FFE5 Constant XK_Caps_Lock                     /* Caps lock */
$FFE6 Constant XK_Shift_Lock                    /* Shift lock */

$FFE7 Constant XK_Meta_L                        /* Left meta */
$FFE8 Constant XK_Meta_R                        /* Right meta */
$FFE9 Constant XK_Alt_L                 /* Left alt */
$FFEA Constant XK_Alt_R                 /* Right alt */
$FFEB Constant XK_Super_L                       /* Left super */
$FFEC Constant XK_Super_R                       /* Right super */
$FFED Constant XK_Hyper_L                       /* Left hyper */
$FFEE Constant XK_Hyper_R                       /* Right hyper */

/*
 *  Latin 1
 *  Byte 3 = 0, Byte 4 = ISO-latin1 character number
 */

/*
 * Keysym macros, used on Keysyms to test for classes of symbols
 */
: IsKeypadKey ( keysym -- flag )
  XK_KP_Space XK_KP_Equal 1+ within ;

: IsPrivateKeypadKey ( keysym -- flag )
  $11000000 $1100FFFF 1+ within ;

: IsCursorKey ( keysym -- flag )
  XK_Home XK_Select within ;

: IsPFKey ( keysym -- flag )
  XK_KP_F1 XK_KP_F4 1+ within ;

: IsFunctionKey ( keysym -- flag )
  XK_F1 XK_F35 1+ within ;

: IsMiscFunctionKey ( keysym -- flag )
  XK_Select XK_Break 1+ within ;

: IsModifierKey ( keysym -- flag )
  dup XK_Shift_L XK_Hyper_R 1+ within swap
  dup XK_Mode_switch = swap
  XK_Num_Lock = or or ;

/* Return values from XRectInRegion() */
 
0 Constant RectangleOut
1 Constant RectangleIn
2 Constant RectanglePart
 

/*
 * Information used by the visual utility routines to find desired visual
 * type from the many visuals a display may support.
 */

struct{
  cell visual
  cell visualid
  cell screen
  cell depth
  cell class
  cell red_mask
  cell green_mask
  cell blue_mask
  cell colormap_size
  cell bits_per_rgb
} XVisualInfo

$0 Constant VisualNoMask
$1 Constant VisualIDMask
$2 Constant VisualScreenMask
$4 Constant VisualDepthMask
$8 Constant VisualClassMask
$10 Constant VisualRedMaskMask
$20 Constant VisualGreenMaskMask
$40 Constant VisualBlueMaskMask
$80 Constant VisualColormapSizeMask
$100 Constant VisualBitsPerRGBMask
$1FF Constant VisualAllMask

/*
 * This defines a window manager property that clients may use to
 * share standard color maps of type RGB_COLOR_MAP:
 */
struct{
        cell colormap
        cell red_max
        cell red_mult
        cell green_max
        cell green_mult
        cell blue_max
        cell blue_mult
        cell base_pixel
        cell visualid           /* added by ICCCM version 1 */
        cell killid             /* added by ICCCM version 1 */
} XStandardColormap

1 Constant ReleaseByFreeingColormap  /* for killid field above */


/*
 * return codes for XReadBitmapFile and XWriteBitmapFile
 */
0 Constant BitmapSuccess
1 Constant BitmapOpenFailed
2 Constant BitmapFileInvalid
3 Constant BitmapNoMemory

/* **************************************************************
 *
 * Context Management
 *
 *************************************************************** */


/* Associative lookup table return codes */

0 Constant XCSUCCESS    /* No error. */
1 Constant XCNOMEM      /* Out of memory */
2 Constant XCNOENT      /* No entry in table */

\ ' XrmUniqueQuark Alias XUniqueContext
\ ' XrmStringToQuark Alias XStringToContext

0 Constant XrmBindTightly
1 Constant XrmBindLoosely

0 Constant XrmEnumAllLevels
1 Constant XrmEnumOneLevel

0 Constant XrmoptionNoArg     /* Value is specified in OptionDescRec.value          */
1 Constant XrmoptionIsArg     /* Value is the option string itself                  */
2 Constant XrmoptionStickyArg /* Value is characters immediately following option */
3 Constant XrmoptionSepArg    /* Value is next argument in argv             */
4 Constant XrmoptionResArg      /* Resource and value in next argument in argv      */
5 Constant XrmoptionSkipArg   /* Ignore this option and the next argument in argv */
6 Constant XrmoptionSkipLine  /* Ignore this option and the rest of argv            */
7 Constant XrmoptionSkipNArgs /* Ignore this option and the next 
                           OptionDescRes.value arguments in argv */

struct{
    cell   option           /* Option abbreviation in argv          */
    cell   specifier        /* Resource specifier                   */
    cell   argKind          /* Which style of option it is          */
    cell   value            /* Value to provide if XrmoptionNoArg   */
} XrmOptionDescRec

/* $XConsortium: cursorfont.h,v 1.4 94/04/17 20:22:00 rws Exp $ */
/*

Copyright (c) 1987  X Consortium

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be included
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE X CONSORTIUM BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

Except as contained in this notice, the name of the X Consortium shall
not be used in advertising or otherwise to promote the sale, use or
other dealings in this Software without prior written authorization
from the X Consortium.

 */

154 Constant XC_num_glyphs
0 Constant XC_X_cursor
2 Constant XC_arrow
4 Constant XC_based_arrow_down
6 Constant XC_based_arrow_up
8 Constant XC_boat
10 Constant XC_bogosity
12 Constant XC_bottom_left_corner
14 Constant XC_bottom_right_corner
16 Constant XC_bottom_side
18 Constant XC_bottom_tee
20 Constant XC_box_spiral
22 Constant XC_center_ptr
24 Constant XC_circle
26 Constant XC_clock
28 Constant XC_coffee_mug
30 Constant XC_cross
32 Constant XC_cross_reverse
34 Constant XC_crosshair
36 Constant XC_diamond_cross
38 Constant XC_dot
40 Constant XC_dotbox
42 Constant XC_double_arrow
44 Constant XC_draft_large
46 Constant XC_draft_small
48 Constant XC_draped_box
50 Constant XC_exchange
52 Constant XC_fleur
54 Constant XC_gobbler
56 Constant XC_gumby
58 Constant XC_hand1
60 Constant XC_hand2
62 Constant XC_heart
64 Constant XC_icon
66 Constant XC_iron_cross
68 Constant XC_left_ptr
70 Constant XC_left_side
72 Constant XC_left_tee
74 Constant XC_leftbutton
76 Constant XC_ll_angle
78 Constant XC_lr_angle
80 Constant XC_man
82 Constant XC_middlebutton
84 Constant XC_mouse
86 Constant XC_pencil
88 Constant XC_pirate
90 Constant XC_plus
92 Constant XC_question_arrow
94 Constant XC_right_ptr
96 Constant XC_right_side
98 Constant XC_right_tee
100 Constant XC_rightbutton
102 Constant XC_rtl_logo
104 Constant XC_sailboat
106 Constant XC_sb_down_arrow
108 Constant XC_sb_h_double_arrow
110 Constant XC_sb_left_arrow
112 Constant XC_sb_right_arrow
114 Constant XC_sb_up_arrow
116 Constant XC_sb_v_double_arrow
118 Constant XC_shuttle
120 Constant XC_sizing
122 Constant XC_spider
124 Constant XC_spraycan
126 Constant XC_star
128 Constant XC_target
130 Constant XC_tcross
132 Constant XC_top_left_arrow
134 Constant XC_top_left_corner
136 Constant XC_top_right_corner
138 Constant XC_top_side
140 Constant XC_top_tee
142 Constant XC_trek
144 Constant XC_ul_angle
146 Constant XC_umbrella
148 Constant XC_ur_angle
150 Constant XC_watch
152 Constant XC_xterm

base !

Module;
