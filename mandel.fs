include complex.fb
float also complex also
: >2? ( z -- z f )  zdup zsqabs 4e f> ;
: znext ( z1 z2 -- z1 z2' )  zdup z* zover z+ ;
: pixel 0e 0e 100 50 do znext >2? if i unloop zdrop exit then loop 0 zdrop ;
: left->right -1.5e 400 0 do fswap pixel c, fswap 0.005e f+ loop fdrop ;
: top->bottom -1e 400 0 do left->right 0.005e f+ loop fdrop
    400 dup * dup negate allot here swap type ;
.( P5) cr .( 400 400) cr .( 255) cr top->bottom bye