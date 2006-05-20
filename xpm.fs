\ Xpm interface

Module Xpm

also DOS

legacy on

also X11

library libXpm libXpm.so.4  depends libX11

previous
previous

base @ hex

0 libXpm XpmAttributesSize              XpmAttributesSize               ( -- n )
5 libXpm XpmCreateBufferFromImage       XpmCreateBufferFromImage        ( attribs shapeimg img buffer_r dpy -- n )
5 libXpm XpmCreateBufferFromPixmap      XpmCreateBufferFromPixmap       ( attribs shapemask pixm buffer_r dpy -- n )
3 libXpm XpmCreateBufferFromXpmImage    XpmCreateBufferFromXpmImage     ( info image buffer_r -- n )
5 libXpm XpmCreateDataFromImage         XpmCreateDataFromImage          ( attribs shapeimg img data_r dpy -- n )
5 libXpm XpmCreateDataFromPixmap        XpmCreateDataFromPixmap         ( attribs shapemask pixm data_r dpy -- n )
3 libXpm XpmCreateDataFromXpmImage      XpmCreateDataFromXpmImage       ( info image data_r -- n )
5 libXpm XpmCreateImageFromBuffer       XpmCreateImageFromBuffer        ( attribs shapemask_r img_r buffer dpy -- n )
5 libXpm XpmCreateImageFromData         XpmCreateImageFromData          ( attribs shapemask_r img_r data dpy -- n )
5 libXpm XpmCreateImageFromXpmImage     XpmCreateImageFromXpmImage      ( attribs shapemask_r img_r image dpy -- n )
6 libXpm XpmCreatePixmapFromBuffer      XpmCreatePixmapFromBuffer       ( attribs shapemask_r pixmap_r buffer d dpy -- n )
6 libXpm XpmCreatePixmapFromData        XpmCreatePixmapFromData         ( attribs shapemask_r pixmap_r data d dpy -- n )
6 libXpm XpmCreatePixmapFromXpmImage    XpmCreatePixmapFromXpmImage     ( attribs shapemask_r pixmap_r image d dpy -- n )
3 libXpm XpmCreateXpmImageFromBuffer    XpmCreateXpmImageFromBuffer     ( info image buffer -- n )
3 libXpm XpmCreateXpmImageFromData      XpmCreateXpmImageFromData       ( info image data -- n )
5 libXpm XpmCreateXpmImageFromImage     XpmCreateXpmImageFromImage      ( attribs xpmimage shapeimg img dpy -- n )
5 libXpm XpmCreateXpmImageFromPixmap    XpmCreateXpmImageFromPixmap     ( attribs xpmimage shapemask pixmap dpy -- n )
1 libXpm XpmFreeAttributes              XpmFreeAttributes               ( attribs -- r )
2 libXpm XpmFreeExtensions              XpmFreeExtensions               ( nextensions extensions -- r )
1 libXpm XpmFreeXpmImage                XpmFreeXpmImage                 ( image -- r )
1 libXpm XpmFreeXpmInfo                 XpmFreeXpmInfo                  ( info -- r )
1 libXpm XpmGetErrorString              XpmGetErrorString               ( errcode -- string )
0 libXpm XpmLibraryVersion              XpmLibraryVersion               ( -- n )
2 libXpm XpmReadFileToBuffer            XpmReadFileToBuffer             ( buffer_r filename -- n )
2 libXpm XpmReadFileToData              XpmReadFileToData               ( data_r filename -- n )
5 libXpm XpmReadFileToImage             XpmReadFileToImage              ( attribs shapeimage_r image_r filename dpy -- n )
6 libXpm XpmReadFileToPixmap            XpmReadFileToPixmap             ( attribs shapemask_r pixmap_r filename d dpy -- n )
3 libXpm XpmReadFileToXpmImage          XpmReadFileToXpmImage           ( info image filename -- n )
2 libXpm XpmWriteFileFromBuffer         XpmWriteFileFromBuffer          ( buffer_r filename -- n )
2 libXpm XpmWriteFileFromData           XpmWriteFileFromData            ( data_r filename -- n )
5 libXpm XpmWriteFileFromImage          XpmWriteFileFromImage           ( attribs shapeimage_r image_r filename dpy -- n )
5 libXpm XpmWriteFileFromPixmap         XpmWriteFileFromPixmap          ( attribs shapemask_r pixmap_r filename dpy -- n )
3 libXpm XpmWriteFileFromXpmImage       XpmWriteFileFromXpmImage        ( info image filename -- n )

struct{
    cell valuemask            /* Specifies which attributes are
                                         * defined */

    ptr visual                     /* Specifies the visual to use */
    ptr colormap                  /* Specifies the colormap to use */
    cell depth                 /* Specifies the depth */
    cell width                 /* Returns the width of the created
                                         * pixmap */
    cell height                /* Returns the height of the created
                                         * pixmap */
    cell x_hotspot             /* Returns the x hotspot's
                                         * coordinate */
    cell y_hotspot             /* Returns the y hotspot's
                                         * coordinate */
    cell cpp                   /* Specifies the number of char per
                                         * pixel */
    ptr pixels                      /* List of used color pixels */
    cell npixels               /* Number of pixels */
    ptr colorsymbols       /* Array of color symbols to
                                         * override */
    cell numsymbols            /* Number of symbols */
    ptr rgb_fname                    /* RGB text file name */
    cell nextensions           /* number of extensions */
    ptr extensions           /* pointer to array of extensions */

    cell ncolors               /* Number of colors */
    ptr colorTable               /* Color table pointer */
/* 3.2 backward compatibility code */
    ptr hints_cmt                    /* Comment of the hints section */
    ptr colors_cmt                   /* Comment of the colors section */
    ptr pixels_cmt                   /* Comment of the pixels section */
/* end 3.2 bc */
    cell mask_pixel            /* Transparent pixel's color table
                                         * index */

    /* Color Allocation Directives */
    cell exactColors           /* Only use exact colors for visual */
    cell closeness             /* Allowable RGB deviation */
    cell red_closeness         /* Allowable red deviation */
    cell green_closeness       /* Allowable green deviation */
    cell blue_closeness        /* Allowable blue deviation */
    cell color_key             /* Use colors from this color set */

    ptr alloc_pixels           /* Returns the list of alloc'ed color
                                  pixels */
    cell nalloc_pixels         /* Returns the number of alloc'ed
                                  color pixels */

    cell alloc_close_colors    /* Specify whether close colors should
                                  be allocated using XAllocColor
                                  or not */
    cell bitmap_format         /* Specify the format of 1bit depth
                                  images: ZPixmap or XYBitmap */

    /* Color functions */
    ptr alloc_color            /* Application color allocator */
    ptr free_colors            /* Application color de-allocator */
    ptr color_closure          /* Application private data to pass to
                                  alloc_color and free_colors */

} XpmAttributes

decimal

/* XpmAttributes value masks bits */
1 0 << Constant XpmVisual
1 1 << Constant XpmColormap
1 2 << Constant XpmDepth
1 3 << Constant XpmSize         /* width & height */
1 4 << Constant XpmHotspot      /* x_hotspot & y_hotspot */
1 5 << Constant XpmCharsPerPixel
1 6 << Constant XpmColorSymbols
1 7 << Constant XpmRgbFilename
/* 3.2 backward compatibility code */
1 8 << Constant XpmInfos
XpmInfos Constant XpmReturnInfos
/* end 3.2 bc */
1 9 << Constant XpmReturnPixels
1 10 << Constant XpmExtensions
XpmExtensions Constant XpmReturnExtensions

1 11 << Constant XpmExactColors
1 12 << Constant XpmCloseness
1 13 << Constant XpmRGBCloseness
1 14 << Constant XpmColorKey

1 15 << Constant XpmColorTable
XpmColorTable Constant XpmReturnColorTable

1 16 << Constant XpmReturnAllocPixels
1 17 << Constant XpmAllocCloseColors
1 18 << Constant XpmBitmapFormat

1 19 << Constant XpmAllocColor
1 20 << Constant XpmFreeColors
1 21 << Constant XpmColorClosure

base !

Module;
