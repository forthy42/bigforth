import float float also

Module GSL

also DOS

library libgslcblas libgslcblas.so.0
library libgsl libgsl.so.0 depends libgslcblas

legacy off

libgsl gsl_matrix_alloc int int (int) gsl_matrix_alloc 
( n m -- *gsl_matrix )
libgsl gsl_matrix_calloc int int (int) gsl_matrix_calloc 
( n m -- *gsl_matrix )
libgsl gsl_matrix_free int (void) gsl_matrix_free 
( *gsl_matrix -- )
libgsl gsl_matrix_get int int int (fp) gsl_matrix_get
( *gsl_matrix i j  -- df )
\ ******* this is the function that gives trouble **********
libgsl gsl_matrix_set df int int int (void/fp) gsl_matrix_set 
( df *gsl_matrix i j  -- )
libgsl gsl_matrix_set_all df int (void/fp) gsl_matrix_set_all 
( df *gsl_matrix i j  -- )
libgsl gsl_blas_dgemm int df int int df int int (int/fp) gsl_blas_dgemm
( alpha beta int int *gsl_matrix *gsl_matrix *gsl_matrix -- n )

struct{ cell size1 cell size2 } gsl_matrix

111 Constant CblasNoTrans

: ]]* (  *gsl_matrix *gsl_matrix -- *gsl_matrix )
    !1 !0 CblasNoTrans CblasNoTrans 2swap 2dup
    gsl_matrix size2 @ swap gsl_matrix size1 @ swap
    gsl_matrix_alloc dup >r
    gsl_blas_dgemm drop r> ;

legacy on
previous

Module;

gsl also
3 3 gsl_matrix_alloc constant a[[
3 3 gsl_matrix_alloc constant b[[
3e a[[ gsl_matrix_set_all
2e b[[ gsl_matrix_set_all
a[[ b[[ ]]* constant c[[
9 0 [dO] [i] 3 mod 0= [if] cr [thEN] c[[ [i] 3 /mod gsl_matrix_get f. [LOOP]
