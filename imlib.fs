\ Imlib bindings                                       07feb98py

Module imlib
DOS also Forth also X11 also

\ & libXext @lib

\ library libz libz.so.1
\ library libjpeg libjpeg.so.62
\ library libgif libungif.so.4
\ library libpng libpng.so.2  depends libm  depends libz
\ library libtiff libtiff.so  depends libm
library libImlib libImlib.so.1
\ depends libpng  depends libtiff  depends libjpeg  depends libgif
\ depends libXext

struct{
   cell r /* Red value (0-255) */
   cell g /* Green value (0-255) */
   cell b /* Blue value (0-255) */
   cell pixel
} ImColor

struct{
   cell gamma
   cell brightness
   cell contrast
} ImlibColorModifier


struct{
    cell shmseg        /* resource id */
    cell shmid         /* kernel id */
    ptr shmaddr        /* address in client */
    cell readOnly      /* how the server should attach it */
} XShmSegmentInfo

struct{
   ptr disp
   cell screen
   cell root
   ptr visual
   cell depth
   cell root_cmap
   byte shm
   byte shmp
   cell shm_event
   ptr last_xim
   ptr last_sxim
   struct XShmSegmentInfo last_shminfo
   struct XShmSegmentInfo last_sshminfo
} Xdata

struct{
   cell left
   cell right
   cell top
   cell bottom
} ImlibBorder

struct{
   cell rgb_width
   cell rgb_height
   ptr rgb_data
   ptr alpha_data
   ptr filename
   cell width
   cell height
   struct ImColor shape_color
   struct ImlibBorder border
   cell pixmap
   cell shape_mask
   byte cache
   struct ImlibColorModifier mod
   struct ImlibColorModifier rmod
   struct ImlibColorModifier gmod
   struct ImlibColorModifier bmod
   256 string rmap
   256 string gmap
   256 string bmap
} Image

struct{
   ptr file
   ptr im
   cell refnum
   byte dirty
   ptr prev
   ptr next
} image_cache

struct{
   ptr im
   ptr file
   byte dirty
   cell width
   cell height
   cell pmap
   cell shape_mask
   ptr xim
   ptr sxim
   cell refnum
   ptr prev
   ptr next
} pixmap_cache

\ ImLib functions                                      08feb98py

legacy on
       
1 libImlib ImlibInit Imlib_init ( dpy -- data )
1 libImlib ImlibGetRenderType Imlib_get_render_type ( id -- n )
2 libImlib ImlibSetRenderType Imlib_set_render_type ( rend id -- n )
2 libImlib ImlibLoadColors Imlib_load_colors ( file id -- n )
2 libImlib ImlibLoadImage Imlib_load_image ( file id -- image )
4 libImlib ImlibBestColorMatch Imlib_best_color_match
                                               ( b g r id -- n )
4 libImlib ImlibRender Imlib_render ( h w im id -- n )
2 libImlib ImlibCopyImageToPixmap Imlib_copy_image
                                             ( im id -- pixmap )
2 libImlib ImlibMoveImageToPixmap Imlib_move_image
                                             ( im id -- pixmap )
2 libImlib ImlibCopyMaskToPixmap Imlib_copy_mask
                                             ( im id -- pixmap )
2 libImlib ImlibMoveMaskToPixmap Imlib_move_mask
                                             ( im id -- pixmap )
2 libImlib ImlibDestroyImage Imlib_destroy_image ( im id -- x )
2 libImlib ImlibKillImage Imlib_kill_image ( im id -- x )
1 libImlib ImlibFreeColors Imlib_free_colors ( id -- x )
\ 4 libImlib ImlibDestroyPixmaps Imlib_destroy_pixmaps
                                              ( h w im id -- x )

2 libImlib ImlibFreePixmap Imlib_free_pixmap ( pixmap id -- x )
6 libImlib ImlibSetImageBorder Imlib_set_image_border
                                          ( b t r l im id -- x )
6 libImlib ImlibGetImageBorder Imlib_get_image_border
                                          ( b t r l im id -- x )
\ 3 libImlib ImlibGetImageTransparentColor Imlib_get_image_transparent_color
                                          ( icl im id -- x )
\ 3 libImlib ImlibSetImageTransparentColor Imlib_set_image_transparent_color
                                          ( icl im id -- x )
\ 1 libImlib ImlibGetImageCacheSize Imlib_get_image_cache_size ( id -- n )
\ 2 libImlib ImlibSetImageCacheSize Imlib_set_image_cache_size ( size id -- x )
\ 1 libImlib ImlibGetImageCacheStatus Imlib_get_image_cache_status ( id -- n )
\ 2 libImlib ImlibSetImageCacheStatus Imlib_set_image_cache_status ( onoff id )
\ 1 libImlib ImlibGetPixmapCacheSize Imlib_get_pixmap_cache_size ( id -- n )
\ 2 libImlib ImlibSetPixmapCacheSize Imlib_set_pixmap_cache_size ( size id -- x )
\ 1 libImlib ImlibGetPixmapCacheStatus Imlib_get_pixmap_cache_status ( id -- n )
\ 2 libImlib ImlibSetPixmapCacheStatus Imlib_set_pixmap_cache_status ( onoff 
3 libImlib ImlibSaveImageToEIM Imlib_save_image_to_eim ( file im id -- n )
3 libImlib ImlibAddImageToEIM Imlib_add_image_to_eim ( file im id -- n )
3 libImlib ImlibSaveImageToPPM Imlib_save_image_to_ppm ( file im id -- n )

3 libImlib ImlibSetImageModifier Imlib_set_image_modifier ( mod im id -- r )

previous previous previous imlib
Module;
