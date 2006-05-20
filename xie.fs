\ Xext interface

DOS also

Module XIE

lib: libXIE libXIE.so.6

base @ hex

\ startup functions
2 libXIE XieInitialize		XieInitialize		( extinfo_r dpy -- status )
4 libXIE XieQueryTechniques	XieQueryTechniques	( tech_r ntech_r tech_group dpy -- status )
2 libXIE XieFreeTechniques	XieFreeTechniques	( count techs -- r )

\ Color List functions
1 libXIE XieCreateColorList	XieCreateColorList	( dpy -- color_list )
2 libXIE XieDestroyColorList	XieDestroyColorList	( color_list dpy -- r )
2 libXIE XiePurgeColorList	XiePurgeColorList	( color_list dpy -- r )
5 libXIE XieQueryColorList	XieQueryColorList	( colors_r ncolors_r colormap_r color_list dpy -- status )

\ LUT functions
1 libXIE XieCreateLUT		XieCreateLUT		( dpy -- lut )
2 libXIE XieDestroyLUT		XieDestroyLUT		( lut dpy -- r )

\ Photomap functions
1 libXIE XieCreatePhotomap	XieCreatePhotomap	( dpy -- photomap )
2 libXIE XieDestroyPhotomap	XieDestroyPhotomap	( photomap dpy -- )
A libXIE XieQueryPhotomap	XieQueryPhotomap	( levels_r height_r width_r decode_t_r class_r datatype_r populated_r photomap dpy -r status )

\ ROI functions
1 libXIE XieCreateROI		XieCreateROI		( dpy -- roi )
2 libXIE XieDestroyROI		XieDestroyROI		( roi dpy -r r )

\ Photospace functions


base !

Module;