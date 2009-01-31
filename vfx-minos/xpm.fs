\ Xpm interface

Vocabulary Xpm

also Xpm definitions

library: libXpm.so.4 \ depends libX11

base @ hex

LocalExtern: XpmReadFileToPixmap int XpmReadFileToPixmap( int , int , int , int , int , int ); ( dpy d filename pixmap_r shapemask_r attribs -- n )

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

previous definitions

