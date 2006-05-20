\ Xext interface

DOS also

Module Xext

lib: libXext libXext.so.6

base @ hex

1 libXext XMITMiscGetBugMode	XMITMiscGetBugMode	( dpy -- flag )
3 libXext XMITMiscQueryExtension	XMITMiscQueryExtension	( event error dpy -- flag )
2 libXext XMITMiscSetBugMode	XMITMiscSetBugMode	( onOff dpy -- Status )

2 libXext XMissingExtension	XMissingExtension	( ext_name dpy -- n )
1 libXext XSetExtensionErrorHandler	XSetExtensionErrorHandler	( handler -- n )

7 libXext XShapeCombineMask	XShapeCombineMask	( op src y x dest_kind dest dpy -- r )
9 libXext XShapeCombineRectangles	XShapeCombineRectangles	( ordering op n rects y x dest_kind dest dpy -- r )
7 libXext XShapeCombineRegion	XShapeCombineRegion	( op region y x dest_kind dest dpy -- r )
8 libXext XShapeCombineShape	XShapeCombineShape	( op src_kind src y x dest_kind dest dpy -- r )
5 libXext XShapeGetRectangles	XShapeGetRectangles	( &ordering &count kind win dpy -- rects )
2 libXext XShapeInputSelected	XShapeInputSelected	( win dpy -- n )
5 libXext XShapeOffsetShape	XShapeOffsetShape	( y x dest_kind dest dpy -- r )
3 libXext XShapeQueryExtension	XShapeQueryExtension	( error event dpy -- flag )
$C libXext XShapeQueryExtents	XShapeQueryExtents	( &hc &wc &yc &xc clip_shaped &h &w &y &x bounding-shaped win dpy -- Status )
3 libXext XShapeQueryVersion	XShapeQueryVersion	( &minor &mayor dpy -- Status )
3 libXext XShapeSelectInput	XShapeSelectInput	( mask win dpy -- r )

2 libXext XShmAttach	XShmAttach	( shminfo dpy -- status )
8 libXext XShmCreateImage	XShmCreateImage	( h w shminfo data format depth vidual dpy -- ximage )
7 libXext XShmCreatePixmap	XShmCreatePixmap	( depth h w shminfo data d dpy -- pixmap )
2 libXext XShmDetach	XShmDetach	( shminfo dpy -- status )
\ libXext XShmGetEventBase	XShmGetEventBase	( -- )
6 libXext XShmGetImage	XShmGetImage	( plane_mask y x image d dpy -- status )
1 libXext XShmPixmapFormat	XShmPixmapFormat	( dpy -- n )
$B libXext XShmPutImage	XShmPutImage	( send_e sh sw dy dx sy sx image gc d dpy -- status )
3 libXext XShmQueryExtension	XShmQueryExtension	( error event dpy -- flag )
4 libXext XShmQueryVersion	XShmQueryVersion	( &sharedpm &minor &mayor dpy -- flag )

3 libXext XSyncAwait	XSyncAwait	( n_conds wait_list dpy -- status )
4 libXext XSyncChangeAlarm	XSyncChangeAlarm	( &values mask alarm dpy -- status )
3 libXext XSyncChangeCounter	XSyncChangeCounter	( value counter dpy -- status )
3 libXext XSyncCreateAlarm	XSyncCreateAlarm	( values mask dpy -- alarm )
2 libXext XSyncCreateCounter	XSyncCreateCounter	( init dpy -- counter )
2 libXext XSyncDestroyAlarm	XSyncDestroyAlarm	( alarm dpy -- status )
2 libXext XSyncDestroyCounter	XSyncDestroyCounter	( counter dpy -- status )
1 libXext XSyncFreeSystemCounterList	XSyncFreeSystemCounterList	( list -- r )
3 libXext XSyncGetPriority	XSyncGetPriority	( priority client_rc_id dpy -- status )
3 libXext XSyncInitialize	XSyncInitialize	( &minor &mayor dpy -- statis )
2 libXext XSyncIntToValue	XSyncIntToValue	( i pv -- r )
3 libXext XSyncIntsToValue	XSyncIntsToValue	( h l pv -- r )
2 libXext XSyncListSystemCounters	XSyncListSystemCounters	( &n dpy -- list )
1 libXext XSyncMaxValue	XSyncMaxValue	( &v -- r )
1 libXext XSyncMinValue	XSyncMinValue	( &v -- r )
3 libXext XSyncQueryAlarm	XSyncQueryAlarm	( &values alarm dpy -- status )
3 libXext XSyncQueryCounter	XSyncQueryCounter	( &value counter dpy -- status )
3 libXext XSyncQueryExtension	XSyncQueryExtension	( eror event dpy -- status )
3 libXext XSyncSetCounter	XSyncSetCounter	( value counter dpy -- status )
3 libXext XSyncSetPriority	XSyncSetPriority	( prior client_rid dpy -- status )
4 libXext XSyncValueAdd	XSyncValueAdd	( &ov b a &r -- r )
2 libXext XSyncValueEqual	XSyncValueEqual	( b a -- flag )
2 libXext XSyncValueGreaterOrEqual	XSyncValueGreaterOrEqual	( b a -- flag )
2 libXext XSyncValueGreaterThan	XSyncValueGreaterThan	( b a -- flag )
1 libXext XSyncValueHigh32	XSyncValueHigh32	( v -- n )
1 libXext XSyncValueIsNegative	XSyncValueIsNegative	( v -- flag )
1 libXext XSyncValueIsPositive	XSyncValueIsPositive	( v -- flag )
1 libXext XSyncValueIsZero	XSyncValueIsZero	( v -- flag )
2 libXext XSyncValueLessOrEqual	XSyncValueLessOrEqual	( b a -- flag )
2 libXext XSyncValueLessThan	XSyncValueLessThan	( b a -- flag )
1 libXext XSyncValueLow32	XSyncValueLow32	( v -- u )
4 libXext XSyncValueSubtract	XSyncValueSubtract	( &ov b a &r -- r )

base !

Module;
