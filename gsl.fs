\ GNU Scientific Library interface              Mon Sep 12 14:40:15 MDT 2005
\ Copyright (C) 2007, Sergey Plis
\
\ This program is free software; you can redistribute it and/or modify
\ it under the terms of the GNU General Public License as published by
\ the Free Software Foundation; either version 2 of the License, or
\ (at your option) any later version.
\
\ This program is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\ GNU General Public License for more details.

\needs locals| | include locals.fs
\needs atlas | include atlas.fs

Module GSL

also float also DOS

library libblas libblas.so.3

library libgsl libgsl.so.0 depends libblas \ libgslcblas

legacy off

\ some functions
libgsl gsl_log1p df (fp) gsl_log1p ( df -- df )
libgsl gsl_acosh df (fp) gsl_acosh ( df -- df )

\ error handling                         Wed Sep 21 23:04:06 MDT 2005
libgsl gsl_set_error_handler ptr (int) gsl_set_error_handler
( function -- function )
libgsl gsl_strerror int (ptr) gsl_strerror


include callback.fs
also dos
callback 4:0 (void) int int int int callback;
: cstr-fstr ( addr -- addr len )
    0
    begin 2dup + c@ 0 = not while
	    1+
    repeat ;

: .bold-red ." [1;31;40m" ;
: .red ." [2;31;40m" ;
: .reset    ." [0;37;40m" ;
: cb-test
    cr
    .bold-red ." GSL ERROR: " .reset cr
    10 spaces gsl_strerror cstr-fstr type cr 
    drop \    ." at line: " . cr
    drop \    ." of file: " cstr-fstr type cr
    10 spaces cstr-fstr type cr
    .red
    -1 abort" failed at[0;37;40m" ;
' cb-test 4:0 c_plus

\ 1 2 c_plus 2:1call .
variable old_handler
c_plus gsl_set_error_handler old_handler !

\ random number generation               Mon Sep 12 22:06:01 MDT 2005

libgsl gsl_rng_types_setup (ptr) gsl_rng_types_setup ( -- *gsl_rng_type)
libgsl gsl_rng_env_setup (ptr) gsl_rng_env_setup ( -- *gsl_rng)
libgsl gsl_rng_alloc int (int) gsl_rng_alloc ( *gsl_rng_type -- *gsl_rng )
libgsl gsl_rng_name int (int) gsl_rng_name ( *gsl_rng -- string )
libgsl gsl_rng_set int int (void) gsl_rng_set ( *gsl_rng int -- )
libgsl gsl_rng_uniform int (fp) gsl_rng_uniform ( *gsl_rng -- df )
libgsl gsl_rng_uniform_pos int (fp) gsl_rng_uniform_pos ( *gsl_rng -- df )
libgsl gsl_rng_uniform_int int int (int) gsl_rng_uniform_int ( *gsl_rng n -- n )
libgsl gsl_rng_get int (int) gsl_rng_get ( *gsl_rng -- int )
libgsl gsl_rng_max int (int) gsl_rng_max ( *gsl_rng -- int )
libgsl gsl_rng_min int (int) gsl_rng_min ( *gsl_rng -- int )
libgsl gsl_rng_clone int (int) gsl_rng_clone ( *gsl_rng -- *gsl_rng )
libgsl gsl_rng_free int (int) gsl_rng_free ( *gsl_rng -- )



\ random number distributions                     Tue Sep 13 00:44:35 MDT 2005
\ Gaussian
libgsl gsl_ran_gaussian df int (fp) gsl_ran_gaussian ( *gsl_rng df -- df )
libgsl gsl_ran_gaussian_ratio_method df int (fp) gsl_ran_gaussian_ratio_method ( *gsl_rng df -- df )
libgsl gsl_ran_gaussian_pdf df df (fp) gsl_ran_gaussian_pdf ( df df -- df )
\ sigma = 1
libgsl gsl_ran_ugaussian int (fp) gsl_ran_ugaussian ( *gsl_rng -- df )
libgsl gsl_ran_ugaussian_ratio_method int (fp) gsl_ran_ugaussian_ratio_method ( *gsl_rng -- df )
libgsl gsl_ran_ugaussian_pdf df (fp) gsl_ran_ugaussian_pdf ( df df -- df )
\ cdf P(x) = \int_{-\infty}^{x} p(x)dx  Q(x) = \int_{x}^{\infty} p(x)dx
libgsl gsl_cdf_gaussian_P df df (fp) gsl_cdf_gaussian_P ( df df -- df )
libgsl gsl_cdf_gaussian_Q df df (fp) gsl_cdf_gaussian_Q ( df df -- df )
libgsl gsl_cdf_gaussian_Pinv df df (fp) gsl_cdf_gaussian_Pinv ( df df -- df )
libgsl gsl_cdf_gaussian_Qinv df df (fp) gsl_cdf_gaussian_Qinv ( df df -- df )
\ sigma = 1 cdf
libgsl gsl_cdf_ugaussian_P df (fp) gsl_cdf_ugaussian_P ( df -- df )
libgsl gsl_cdf_ugaussian_Q df (fp) gsl_cdf_ugaussian_Q ( df -- df )
libgsl gsl_cdf_ugaussian_Pinv df (fp) gsl_cdf_ugaussian_Pinv ( df -- df )
libgsl gsl_cdf_ugaussian_Qinv df (fp) gsl_cdf_ugaussian_Qinv ( df -- df )


\ statistics                                      Tue Sep 13 01:17:35 MDT 2005
libgsl gsl_stats_mean int int int (fp) gsl_stats_mean ( array{ step size -- df )
libgsl gsl_stats_variance int int int (fp) gsl_stats_variance ( array{ step size -- df )
libgsl gsl_stats_variance_m df int int int (fp) gsl_stats_variance_m ( df array{ step size -- df )
libgsl gsl_stats_sd int int int (fp) gsl_stats_sd ( array{ step size -- df )
libgsl gsl_stats_sd_m df int int int (fp) gsl_stats_sd_m ( df array{ step size -- df )
libgsl gsl_stats_skew int int int (fp) gsl_stats_skew ( array{ step size -- df )
libgsl gsl_stats_kurtosis int int int (fp) gsl_stats_kurtosis ( array{ step size -- df )
libgsl gsl_stats_lag1_autocorrelation int int int (fp) gsl_stats_lag1_autocorrelation
( array{ step size -- df )
libgsl gsl_stats_max int int int (fp) gsl_stats_max ( array{ step size -- df )
libgsl gsl_stats_min int int int (fp) gsl_stats_min ( array{ step size -- df )
libgsl gsl_stats_max_index int int int (int) gsl_stats_max_index ( array{ step size -- n )
libgsl gsl_stats_min_index int int int (int) gsl_stats_min_index ( array{ step size -- n )

\ vectors and matrices                           Wed Sep 14 00:15:36 MDT 2005

libgsl gsl_block_alloc int (int) gsl_block_alloc ( n -- addr )
libgsl gsl_block_calloc int (int) gsl_block_calloc ( n -- addr )
libgsl gsl_block_free int (int) gsl_block_free ( n -- addr )

libgsl gsl_vector_alloc int (int) gsl_vector_alloc ( n -- addr )
libgsl gsl_vector_calloc int (int) gsl_vector_calloc ( n -- addr )
libgsl gsl_vector_free int (void) gsl_vector_free ( addr -- )
libgsl gsl_vector_get int int (fp) gsl_vector_get ( addr i -- df )
libgsl gsl_vector_set df int int (void/fp) gsl_vector_set ( df addr i --  )
libgsl gsl_vector_set_all df int (void) gsl_vector_set_all ( df addr -- )
libgsl gsl_vector_set_zero int (void) gsl_vector_set_zero ( addr -- )
libgsl gsl_vector_memcpy int int (int) gsl_vector_memcpy ( dest_addr src_addr -- n )

libgsl gsl_vector_add int int (int) gsl_vector_add ( addr addr -- n )
libgsl gsl_vector_sub int int (int) gsl_vector_sub ( addr addr -- n )
libgsl gsl_vector_mul int int (int) gsl_vector_mul ( addr addr -- n )
libgsl gsl_vector_div int int (int) gsl_vector_div ( addr addr -- n )
libgsl gsl_vector_scale df int (int) gsl_vector_scale ( df addr -- n )
libgsl gsl_vector_add_constant df int (int) gsl_vector_add_constant ( df addr -- n )
libgsl gsl_vector_max int (fp) gsl_vector_max ( addr -- df )
libgsl gsl_vector_min int (fp) gsl_vector_min ( addr -- df )
libgsl gsl_vector_max_index int (fp) gsl_vector_max_index ( addr -- df )
libgsl gsl_vector_min_index int (fp) gsl_vector_min_index ( addr -- df )

\ premutations

libgsl gsl_permutation_alloc int (int) gsl_permutation_alloc ( n -- *gsl_permutation )
libgsl gsl_permutation_calloc int (int) gsl_permutation_calloc ( n -- *gsl_permutation )
libgsl gsl_permutation_init int (void) gsl_permutation_init ( *gsl_permutation -- )
libgsl gsl_permutation_free int (void) gsl_permutation_free ( *gsl_permutation -- )
libgsl gsl_permutation_get int int (int) gsl_permutation_get ( *gsl_permutation i -- n )

libgsl gsl_matrix_scale df int (int/fp)  gsl_matrix_scale ( *gsl_matrix df -- n )
libgsl gsl_matrix_alloc int int (int) gsl_matrix_alloc ( n m -- *gsl_matrix )
libgsl gsl_matrix_calloc int int (int) gsl_matrix_calloc ( n m -- *gsl_matrix )
libgsl gsl_matrix_free int (void) gsl_matrix_free ( *gsl_matrix -- )
libgsl gsl_matrix_get int int int (fp) gsl_matrix_get ( *gsl_matrix i j  -- df )
libgsl gsl_matrix_ptr int int int (int) gsl_matrix_ptr ( *gsl_matrix i j  -- *[i,j] )
libgsl gsl_matrix_set df int int int (void/fp) gsl_matrix_set ( df *gsl_matrix i j  -- )
libgsl gsl_matrix_add int int (int/fp) gsl_matrix_add ( *gsl_matrix *gsl_matrix -- n )
libgsl gsl_matrix_sub int int (int/fp) gsl_matrix_sub ( *gsl_matrix *gsl_matrix -- n )
libgsl gsl_matrix_mul_elements int int (int/fp) gsl_matrix_mul_elements
( *gsl_matrix *gsl_matrix -- n )
libgsl gsl_matrix_set_all df int (void/fp) gsl_matrix_set_all ( *gsl_matrix df -- n )
libgsl gsl_matrix_memcpy int int (int) gsl_matrix_memcpy
( *gsl_matrix *gsl_matrix -- n )
libgsl gsl_matrix_max int (fp) gsl_matrix_max ( *gsl_matrix -- df )
libgsl gsl_matrix_min int (fp) gsl_matrix_min ( *gsl_matrix -- df )
libgsl gsl_matrix_transpose_memcpy int int (int) gsl_matrix_transpose_memcpy
( *gsl_matrix *gsl_matrix -- n )
libgsl gsl_matrix_transpose int (int) gsl_matrix_transpose
( *gsl_matrix *gsl_matrix -- n )

libgsl gsl_matrix_submatrix int int int int int (int) gsl_matrix_submatrix
( *gsl_matrix k1 k2 n1 n2 -- n )

libgsl gsl_matrix_row int int (int) gsl_matrix_row ( *gsl_matrix idx -- *gsl_vector )
libgsl gsl_matrix_column int int (int) gsl_matrix_column ( *gsl_matrix idx -- *gsl_vector )
libgsl gsl_matrix_diagonal int (int) gsl_matrix_diagonal ( *gsl_matrix -- *gsl_vector )

libgsl gsl_vector_subvector int int int (int) gsl_vector_subvector

\ BLAS                                      Wed Sep 14 16:10:34 MDT 2005
\ libblas cblas_dgemm int int df int int int
\ int df int int int int int int (void/fp) cblas_dgemm
libblas cblas_dgemv int int int int df int
int df int int int int (void/fp) cblas_dgemv
libgsl gsl_blas_ddot int int int (int) gsl_blas_ddot
( *gsl_vector *gsl_vector df -- n )
libgsl gsl_blas_dgemm int df int int df int int (int/fp) gsl_blas_dgemm
libgsl gsl_blas_dger int int int df (int/fp) gsl_blas_dger
( alpha *gsl_vector *gsl_vector *gsl_matrix -- n ) ( A=\alpha x y^T+A )
libgsl gsl_blas_dgemv int df int int df int (int/fp) gsl_blas_dgemv
( n alpha *gsl_matrix *gsl_vector beta *gsl_vector -- n )

\ Linear ALgebra                            Wed Sep 14 13:39:22 MDT 2005

libgsl gsl_linalg_LU_decomp int int int (int) gsl_linalg_LU_decomp
( *gsl_matrix *gsl_permutation *variable -- n )
libgsl gsl_linalg_LU_invert int int int (int) gsl_linalg_LU_invert
( *gsl_matrix *gsl_permutation *gsl_matrix -- n )
libgsl gsl_linalg_SV_decomp int int int int (int) gsl_linalg_SV_decomp
( *gsl_matrix *gsl_matrix *gsl_vector *gsl_vector -- n )
libgsl gsl_linalg_SV_decomp_mod int int int int int (int) gsl_linalg_SV_decomp_mod
( *gsl_matrix *gsl_matrix *gsl_matrix *gsl_vector *gsl_vector -- n )

legacy on
previous

\ Structures

struct{
    cell name
    cell max
    cell min
    cell size
    cell set
    cell get
    cell get_double
} gsl_rng_type

struct{
    cell type
    cell state
} gsl_rng

struct{
    cell size
    cell data
} gsl_block

struct{
    cell size
    cell stride
    cell data
    cell block
    cell owner
} gsl_vector

' gsl_block alias gsl_permutation

struct{
    cell size1
    cell size2
    cell tda
    cell data
    cell block
    cell owner
} gsl_matrix

\ random number generation functions
: 0-len dup 1- 0 begin 1+ 2dup + c@ 0= until nip ;
: )gsl-rng ( addr i -- *gsl_rng_type )
    cells + @ ;

\ setting up all available random number generators
gsl_rng_types_setup  value gsl_rng_array(
0 value gsl_rng_default
: gsl-free ( -- )
    gsl_rng_default gsl_rng_free ;

: borosh13 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 0 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: cmrg ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 1 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: coveyou ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 2 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: fishman18 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 3 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: fishman20 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 4 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: fishman2x ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 5 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: gfsr4 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 6 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: knuthran ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 7 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: knuthran2 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 8 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: lecuyer21 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 9 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: minstd ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 10 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: mrg ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 11 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: mt19937 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 12 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: mt19937_1999 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 13 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: mt19937_1998 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 14 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: r250 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 15 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: ran0 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 16 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: ran1 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 17 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: ran2 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 18 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: ran3 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 19 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: rand ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 20 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: rand48 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 21 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random128-bsd ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 22 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random128-glibc2 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 23 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random128-libc5 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 24 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random256-bsd ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 25 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random256-glibc2 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 26 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random256-libc5 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 27 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random32-bsd ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 28 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random32-glibc2 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 29 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random32-libc5 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 30 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random64-bsd ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 31 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random64-glibc2 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 32 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random64-libc5 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 33 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random8-bsd ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 34 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random8-glibc2 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 35 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random8-libc5 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 36 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random-bsd ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 37 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random-glibc2 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 38 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: random-libc5 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 39 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: randu ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 40 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: ranf ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 41 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: ranlux ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 42 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: ranlux389 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 43 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: ranlxd1 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 44 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: ranlxd2 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 45 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: ranlxs0 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 46 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: ranlxs1 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 47 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: ranlxs2 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 48 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: ranmar ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 49 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: slatec ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 50 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: taus ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 51 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: taus2 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 52 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: taus113 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 53 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: transputer ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 54 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: tt800 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 55 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: uni ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 56 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: uni32 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 57 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: vax ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 58 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: waterman14 ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 59 )gsl-rng gsl_rng_alloc to gsl_rng_default ;
: zuf ( -- *gsl_rng ) gsl_rng_default 0<> if gsl-free then 
    gsl_rng_array( 60 )gsl-rng gsl_rng_alloc to gsl_rng_default ;

\ words for actual generation of random numbers
: gsl-randomg ( -- n )
    gsl_rng_default gsl_rng_get ;
: gsl-randomu /* -- f \in [0,1) */
    gsl_rng_default gsl_rng_uniform ;
: gsl-randomu+ /* -- f \in (0,1) */
    gsl_rng_default gsl_rng_uniform_pos ;
: gsl-random-up ( n -- f \in [0,n] )
    gsl_rng_default swap gsl_rng_uniform_int ;
: gsl-set-seed ( n -- )
    gsl_rng_default swap gsl_rng_set ; 
: gsl-clone ( -- *gsl_rng )
    gsl_rng_default gsl_rng_clone ;
: gsl-gaussian ( -- df )
    gsl_rng_default !1 gsl_ran_gaussian ;

\ vectors and matrices

: fvector ( n -- | -- id addr )
    create
    gsl_vector_calloc ,
  does> @ ;

: ]@ ( addr i -- df ) gsl_vector_get ;
: ]! ( df addr i -- ) gsl_vector_set ;
: ]data ( addr -- *data ) gsl_vector data @ ;
: ]fill ( df addr -- ) gsl_vector_set_all ;
: ]erase ( addr -- ) gsl_vector_set_zero ;
: ]+ ( *gsl_vector *gsl_vector -- ) gsl_vector_add drop ;
: ]- ( *gsl_vector *gsl_vector -- ) gsl_vector_sub drop ;
: ]e*! ( *gsl_vector *gsl_vector -- ) gsl_vector_mul drop ;
: ]size ( *gsl_vector -- n ) gsl_vector size @ ;
: ]outer* ( *gsl_vector *gsl_vector -- *gsl_matrix )
    over ]size over ]size gsl_matrix_calloc dup >r !1
    gsl_blas_dger drop r> ;
\ no control for divizion by zero (I get segfaults)
: ]/ ( *gsl_vector *gsl_vector -- ) gsl_vector_div throw ;
: ]clone ( *gsl_vector -- *gsl_vector )
    dup gsl_vector size @ gsl_vector_alloc
    dup -rot swap gsl_vector_memcpy drop ;

: ]add ( *gsl_vector *gsl_vector -- *gsl_vector )
    ]clone dup -rot swap ]+ ;
: ]sub ( *gsl_vector *gsl_vector -- *gsl_vector )
    swap ]clone dup -rot swap ]- ;
: ]mul ( *gsl_vector *gsl_vector -- *gsl_vector )
    swap ]clone dup -rot swap ]e*! ;
: ]div ( *gsl_vector *gsl_vector -- *gsl_vector )
    swap ]clone dup -rot swap ]/ ;

: ]*c ( df *gsl_vector -- ) gsl_vector_scale drop ;
: ].+ ( df *gsl_vector -- ) gsl_vector_add_constant drop ;
: ]max ( *gsl_vector -- ) gsl_vector_max ;
: ]min ( *gsl_vector -- ) gsl_vector_min ;
: ]imax ( *gsl_vector -- ) gsl_vector_max_index ;
: ]imin ( *gsl_vector -- ) gsl_vector_min_index ;
: ]copy] ( *gsl_vector_dest *gsl_vector_src -- ) gsl_vector_memcpy drop ;
: ]negate !-1.0 ]*c ;
    
: pvector ( *data n -- *gsl_vector )
    sizeof gsl_vector allocate throw
    dup >r dup 1 swap gsl_vector stride !
    gsl_vector size ! r@
    gsl_vector data ! r> ;
: pmatrix! ( *data tda n m *pmatrix -- *gsl_matrix )
    dup >r gsl_matrix size2 !
    r@ gsl_matrix size1 !
    r@ gsl_matrix tda !
    r@ gsl_matrix data !
    r> ;

: pmatrix ( *data tda n m -- *gsl_matrix )
    sizeof gsl_matrix allocate throw
    pmatrix! ;

\ permutations

: fpermutation ( n -- | -- id addr )
    create
    gsl_permutation_calloc ,
  does> @ ;
: }@ ( *gsl_permutation i -- n ) gsl_permutation_get ;
: }data ( *gsl_permutation -- *data ) gsl_block data @ ;
: }free ( *gsl_permutation -- ) gsl_permutation_free ;

\ matrices

: fmatrix ( n m -- | -- id addr )
    create
    gsl_matrix_calloc ,
  does> @ ;

: free_pseudomatrix ( pmatrix/pvector -- )
    free throw ;
create free_matrix ' free_pseudomatrix , ' gsl_matrix_free ,
create free_vector ' free_pseudomatrix , ' gsl_vector_free ,

: ]]free ( *gsl_matrix -- )
    dup gsl_matrix owner @
    cells free_matrix + @ execute ;
: ]free ( addr -- )
    dup gsl_vector owner @
    cells free_vector + @ execute ;
: ]]@ ( *gsl_matrix i j -- df ) gsl_matrix_get ;
: ]]*@ ( *gsl_matrix i j -- *[i,j] ) gsl_matrix_ptr ;
: ]]! ( *gsl_matrix i j df -- ) gsl_matrix_set ;
: ]]fill ( addr df -- ) gsl_matrix_set_all ;
: ]]size1 gsl_matrix size1 @ ;
: ]]size2 gsl_matrix size2 @ ;
: ]]dim ( *gsl_matrix -- m n ) dup ]]size1 swap ]]size2 ;
: ]]dim. ( *gsl_matrix -- ) ]]dim swap . 8 emit ." x" . cr ;
: ]]data ( *gsl_matrix -- addr) gsl_matrix data @ ;
: ]]tda gsl_matrix tda @ ;
: ]]block gsl_matrix block @ ;
: ]]copy]] ( *gsl_matrix_dest *gsl_matrix_src -- ) gsl_matrix_memcpy drop ;
\ : ]]row ( *gsl_matrix idx -- *gsl_vector ) gsl_matrix_row ;
\ : ]]col ( *gsl_matrix idx -- *gsl_vector ) gsl_matrix_column ;

: ]]max gsl_matrix_max ;
: ]]min gsl_matrix_min ;
: ]]add! ( *gsl_matrix *gsl_matrix -- )
    gsl_matrix_add drop ;
: ]]sub! ( *gsl_matrix *gsl_matrix -- )
    gsl_matrix_sub drop ;
: ]]e*! ( *gsl_matrix *gsl_matrix -- )
    gsl_matrix_mul_elements drop ;
: ]]*c ( *gsl_matrix df -- )
    gsl_matrix_scale drop ;
: ]]clone ( *gsl_matrix -- *gsl_matrix )
    dup dup gsl_matrix size1 @ swap gsl_matrix size2 @
    gsl_matrix_alloc
    dup -rot swap gsl_matrix_memcpy drop ;
: ]]negate !-1.0 ]]*c ;

: ]]+ ( *gsl_matrix *gsl_matrix -- *gsl_matrix )
    ]]clone dup -rot swap ]]add! ;

: ]]- ( *gsl_matrix *gsl_matrix -- *gsl_matrix )
    swap ]]clone dup -rot swap ]]sub! ;

\ blas

\ constants
101 Constant CblasRowMajor
102 Constant CblasColMajor
111 Constant CblasNoTrans
112 Constant CblasTrans
113 Constant CblasConjTrans
121 Constant CblasUpper
122 Constant CblasLower
131 Constant CblasNonUnit
132 Constant CblasUnit
141 Constant CblasLeft
142 Constant CblasRight

: action? (  *gsl_matrix *gsl_matrix n n n -- )
    dup 0= if
	drop
	2swap 2dup
	]]size2 swap ]]size1 swap
	exit
    then
    dup 1 = if
	drop
	2swap 2dup
	]]size2 swap ]]size2 swap
	exit
    then
    2 = if
	2swap 2dup
	]]size1 swap ]]size1 swap
	exit
    then
    3 = if
	2swap 2dup
	]]size1 swap ]]size2 swap
	exit
    then ;    
: ]]mul (  *gsl_matrix *gsl_matrix n n n -- *gsl_matrix )
    !1 !0 action?
    gsl_matrix_alloc dup >r
    gsl_blas_dgemm drop r> ;
: ]]* (  *gsl_matrix *gsl_matrix -- *gsl_matrix )
    CblasNoTrans CblasNoTrans 0 ]]mul ;
: ]]'* (  *gsl_matrix *gsl_matrix -- *gsl_matrix )
    CblasTrans CblasNoTrans 1 ]]mul ;
: ]]*' (  *gsl_matrix *gsl_matrix -- *gsl_matrix )
    CblasNoTrans CblasTrans 2 ]]mul ;
: ]]'*' (  *gsl_matrix *gsl_matrix -- *gsl_matrix )
    CblasTrans CblasTrans 3 ]]mul ;

: ]]mul! (  n n *gsl_matrix *gsl_matrix *gsl_matrix -- )
    !1 !0 gsl_blas_dgemm drop ;
: ]]*! (  *gsl_matrix *gsl_matrix *gsl_matrix -- )
    >r CblasNoTrans CblasNoTrans 2swap r> ]]mul! ;
: ]]'*! (  *gsl_matrix *gsl_matrix *gsl_matrix --  )
    >r CblasTrans CblasNoTrans 2swap r> ]]mul! ;
: ]]*'! (  *gsl_matrix *gsl_matrix *gsl_matrix -- )
    >r CblasNoTrans CblasTrans 2swap r> ]]mul! ;


: ]]*] ( *gsl_matrix *gsl_vector -- *gsl_vector )
    over ]]size1 gsl_vector_calloc >r
    CblasNoTrans -rot r@ !1 !0 gsl_blas_dgemv drop r> ;
: ]]'*] ( *gsl_matrix *gsl_vector -- *gsl_vector )
    over ]]size1 gsl_vector_calloc >r
    CblasTrans -rot r@ !1 !0 gsl_blas_dgemv drop r> ;

: ]]i ( *gsl_matrix -- )
    dup dup ]]size1 swap ]]size2 <> if
	abort" ERROR: Not a square matrix!"
    then
    dup ]]size1 0 do
	dup i i !1 ]]! 
    loop drop ;
: identity ( n -- *gsl_matrix )
    dup gsl_matrix_calloc dup ]]i ;
: min-identity ( *gsl_matrix -- *gsl_matrix )
    dup ]]size1 swap ]]size2 min identity ;
: left/right' ( *gsl_matrix *gsl_matrix -- *gsl_matrix )
    over ]]size1 over ]]size1 > if
	swap ]]*' exit
    else
	]]'* exit
    then ;

\ original matrix remains intact
: ]]' ( *gsl_matrix -- *gsl_matrix )
    dup min-identity dup >r
    left/right'
    r> ]]free ;
: ]]T! ( *gsl_matrix -- )
    gsl_matrix_transpose drop ;

: ]]T ( *gsl_matrix -- *gsl_matrix )
    dup ]]dim swap gsl_matrix_alloc dup rot gsl_matrix_transpose_memcpy drop ;

: ]]2T ( *gsl_matr *gsl_matrix -- )
    gsl_matrix_transpose_memcpy drop ;


: ]]+! ( *gsl_matrix i j df -- ) >r 2dup r@ ]]@ f+ r> ]]! ;
: ]]scale! ( *gsl_matrix i j df -- ) >r 2dup r@ ]]@ f* r> ]]! ;
: ]]data_ij ( *gsl_matrix i j -- addr)
    rot >r swap r@ ]]tda dfloats * swap dfloats + r> ]]data + ;
: ]x ( *gsl_vector *gsl_vector -- *gsl_vector )
    3 gsl_vector_alloc
    { x1[ x2[ x3[ |
    x1[ 2 ]@ fnegate x2[ 1 ]@ f* x1[ 1 ]@ x2[ 2 ]@ f* f+ x3[ 0 ]! 
    x1[ 2 ]@ x2[ 0 ]@ f* x1[ 0 ]@ fnegate x2[ 2 ]@ f* f+ x3[ 1 ]!
    x1[ 1 ]@ fnegate x2[ 0 ]@ f* x1[ 0 ]@ x2[ 1 ]@ f* f+ x3[ 2 ]!
    x3[ } ;
: ]. ( *gsl_vector *gsl_vector -- f:dot_product )
    { x1[ x2[ |
    0 0 sp@ x1[ x2[ rot gsl_blas_ddot drop fd>f } ;
: ]total ( *gsl_vector -- f:sum )
    dup ]size gsl_vector_alloc dup 1e0 ]fill dup rot ]. ]free ;
\ probability normalize - assures sum is unity
: ]pnormalize ( *gsl_vector - )
    dup ]total 1/f ]*c ;
: |]| ( *gsl_vector -- f:norm ) dup ].  fsqrt ;
\ assures vector norm is unity
: ]normalize ( *gsl_vector - )
    dup |]| 1/f ]*c ;
: ]distance ( *gsl-vector *gsl-vector -- f )
    ]sub dup |]| ]free ;
: ]+! ( *gsl_vector i df -- )
    2dup ]@ f+ ]! ;
: ]*! ( *gsl_vector i df -- )
    2dup ]@ f* ]! ;

: ]]*]m ( *gsl_matrix *gsl_vector -- *gsl_vector )
    over ]]size1 gsl_vector_calloc 
    { m[[ x[ y[ |
    m[[ ]]size1 0 do
	m[[ ]]size2 0 do
	    m[[ j i ]]@ x[ i ]@ f* y[ j ]+! 
	loop
    loop y[ } ;

: >#rows ( -- )
    swap ]]size1 >= abort" number of rows is bigger than available!" ;
: >#cols ( -- )
    swap ]]size2 >= abort" number of columns is bigger than available!" ;

: ]]row ( *gsl_matrix n -- *gsl_vector )
    2dup >#rows    
    sizeof gsl_vector allocate throw
    dup 1 swap gsl_vector stride ! >r
    over ]]size2 r@ gsl_vector size !
    0 ]]data_ij r@ gsl_vector data !
    0 r@ gsl_vector owner ! r> ;
: ]]col ( *gsl_matrix n -- *gsl_vector )
    2dup >#cols
    sizeof gsl_vector allocate throw >r
    over ]]tda r@ gsl_vector stride ! 
    over ]]size1 r@ gsl_vector size !
    over ]]block r@ gsl_vector block !    
    0 swap ]]data_ij r@ gsl_vector data !
    0 r@ gsl_vector owner ! r> ;
: ]]submat ( *gsl_matrix n1 n2 m1 m2 -- *gsl_matrix )
    { m[[ n1 n2 m1 m2 |
    sizeof gsl_matrix allocate throw >r
    n2 n1 - 1+ r@ gsl_matrix size1 !
    m2 m1 - 1+ r@ gsl_matrix size2 !
    m[[ n1 m1 ]]data_ij r@ gsl_matrix data !
    m[[ ]]tda r@ gsl_matrix tda !    
    0 r@ gsl_matrix owner ! r> } ;
: ?square ( *gsl_matrix -- )
    dup ]]size1 swap ]]size2 <> abort" ERROR: Not a square matrix!" ;
: ]]diag ( *gsl_matrix n1 n2 -- *gsl_vector )
    rot dup ?square -rot
    sizeof gsl_vector allocate throw { d[ |
    over - d[ gsl_vector size !
    2dup dup ]]data_ij d[ gsl_vector data ! drop
    dup ]]tda d[ gsl_vector stride ! 
        ]]block d[ gsl_vector block !    
    0 d[ gsl_vector owner !
    d[ } ;
    
previous

\ with input matrix replaced by the result
: ]]gsl-svd ( *gsl_matrix -- *gsl_matrix *gsl_vector )
    dup ]]size2 dup dup gsl_matrix_calloc
    swap dup gsl_vector_calloc swap
    gsl_vector_calloc
    { mV vS vW |
    mV vS vW gsl_linalg_SV_decomp drop
    vW ]free
    mV vS } ;
\ seems to be 30% faster
: ]]gsl-svdm ( *gsl_matrix -- *gsl_matrix *gsl_vector )
    dup ]]size2 dup ( a n n -- )
    dup dup gsl_matrix_calloc swap ( a n a n -- )
    dup gsl_matrix_calloc rot dup ( a a a n n -- )
    gsl_vector_calloc swap
    gsl_vector_calloc
    { mX mV vS vW |
    mX mV vS vW gsl_linalg_SV_decomp_mod drop
    vW ]free mX ]]free
    mV vS } ;

also atlas
: ]]alu ( *gsl_matrix -- *gsl_permutation ) ( matrix replaced with its lu )
    { a[[ |
    CblasRowMajor a[[ ]]size1 a[[ ]]size2 a[[ ]]data a[[ ]]size1 dup
    gsl_permutation_alloc dup >r }data
    clapack_dgetrf throw r> } ;
: ]]ainv ( *gsl_matrix *gsl_permutation -- )
    ( LU of a matrix replaced with its inverse )
    { a[[ t{ |
    CblasRowMajor a[[ ]]size2 a[[ ]]data a[[ ]]size1 t{ }data
    clapack_dgetri throw } ;
: ]]ainvert ( *gsl_matrix -- *gsl_matrix )
    ]]clone dup dup >r ]]alu dup >r ]]ainv r> }free r> ;
\ calculates the work needed for dgesvd_ ( see man dgesvd )
: lwork ( m n -- c )
    2dup max -rot min 3 * over + swap 5 * max ;
\ this svd returns U MxM so eats a lot of memory
: ]]asvda ( *gsl_matrix -- *gsl_matrix *gsl_matrix *gsl_vector )
    ]]clone { A[[ |
    A[[ ]]size1 dup gsl_matrix_alloc
    A[[ ]]size2 dup gsl_matrix_alloc
    A[[ ]]size1 A[[ ]]size2 min gsl_vector_alloc
    8 cells allocate throw
    { U[[ V[[ W[ p[ |
    ascii A p[ 0 cells + ! p[ 0 cells + 
    ascii A p[ 1 cells + ! p[ 1 cells + 
    A[[ ]]size1 p[ 2 cells + ! p[ 2 cells +
    A[[ ]]size2 p[ 3 cells + ! p[ 3 cells +
    A[[ ]]data
    p[ 2 cells +
    W[ ]data
    U[[ ]]data
    U[[ ]]size1 p[ 4 cells + ! p[ 4 cells +
    V[[ ]]data
    V[[ ]]size1 p[ 5 cells + ! p[ 5 cells +
    A[[ ]]size1 A[[ ]]size2 lwork
    dup gsl_vector_alloc dup >r
    ]data swap p[ 6 cells + ! p[ 6 cells +
    p[ 7 cells +
    dgesvd_
    r> ]free p[ free throw A[[ ]]free
    U[[ V[[ W[ } } ;

\ performs A=U*S*V^T
\ A = MxN, where M>N, pass it A^T
\ returns U^T (MxN), V(NxN) and vector of N eigenvalues
: ]]asvdO ( *gsl_matrix -- *gsl_matrix *gsl_matrix *gsl_vector )
    { A[[ |
    A[[ ]]size2 A[[ ]]size1 min dup gsl_matrix_alloc    
    A[[ ]]size1 A[[ ]]size2 min gsl_vector_alloc
    8 cells allocate throw
    { V[[ W[ p[ |
    ascii O p[ 0 cells + ! p[ 0 cells + 
    ascii S p[ 1 cells + ! p[ 1 cells + 
    A[[ ]]size2 p[ 2 cells + ! p[ 2 cells +
    A[[ ]]size1 p[ 3 cells + ! p[ 3 cells +
    A[[ ]]data
    p[ 2 cells +
    W[ ]data
    0
    p[ 2 cells +
    V[[ ]]data
    V[[ ]]size2 p[ 5 cells + ! p[ 5 cells +
    A[[ ]]size2 A[[ ]]size1 lwork
    dup gsl_vector_alloc dup >r
    ]data swap p[ 6 cells + ! p[ 6 cells +
    p[ 7 cells +
    dgesvd_
    r> ]free p[ free throw
    A[[ V[[ W[ } } ;
previous


also float

: ]diag[[ ( *gsl_vector -- *gsl_matrix )
    dup ]size dup dup gsl_matrix_calloc swap
    0 do
	2dup swap i ]@ i i ]]!
    loop nip ;

: ]print ( *gsl_vector -- )
    dup ]size 0 do dup i ]@ f. loop drop cr ;
: ]]print ( *gsl_matrix -- )
    cr
    dup ]]size1 0 do
	\ i . ." :  "
	dup ]]size2 0 do
	    dup
	    j i ]]@ f.
	loop
	cr
    loop
    drop ;
: ]]row-print ( *gsl_matrix i -- )
    cr
    over gsl_matrix size2 @ 0 do
	2dup
	 i ]]@ f.
    loop
    cr 2drop ;

: ]]col-print ( *gsl_matrix i -- )
    cr
    over gsl_matrix size1 @ 0 do
	2dup
	i swap ]]@ f.
    loop
    cr 2drop ;

: ]]nthrow ( *gsl_matrix n -- addr )
    over ]]tda * dfloats swap ]]data + ;

: ]]randomize ( *gsl_matrix -- )
    dup dup ]]size1 swap ]]size2 * 0 do
	    dup
	    gsl-randomu
	    ]]data i dfloats + df!
    loop drop ;
: ]randomize ( *gsl_vector -- )
    dup ]size 0 do
	    dup
	    gsl-randomu
	    i ]!
    loop drop ;
: ]mean ( *gsl_vector -- f )
    dup ]size 1 swap gsl_stats_mean ;

: ]variance ( *gsl_vector -- f )
    dup ]size 1 swap gsl_stats_variance ;

: ]sd ( *gsl_vector -- f )
    dup ]size 1 swap gsl_stats_sd ;

: ]skew ( *gsl_vector -- f )
    dup ]size 1 swap gsl_stats_skew ;

: ]kurtosis ( *gsl_vector -- f )
    dup ]size 1 swap gsl_stats_kurtosis ;

: ]]gsl-lu ( *gsl_matrix -- *gsl_matrix *gsl_permutation )
    1 sp@ rot ]]clone dup >r dup ]]size1 gsl_permutation_calloc dup >r rot
    gsl_linalg_LU_decomp drop r> r> swap rot drop ;

: ]]gsl-invert ( *gsl_matrix -- *gsl_matrix )
    ]]clone dup dup ]]gsl-lu 2dup >r >r rot
    gsl_linalg_LU_invert drop r> ]]free r> }free ;

' ]]ainvert alias ]]invert
' ]]asvdO alias ]]svd

: ]]save ( *gsl_matrix *gsl_matrix_cfa fid -- )
    -rot { m[[ name[[ |
    >r
    name[[ >name count 1+ nip 0 m[[ ]]size2 m[[ ]]size1 0
    sp@ 5 cells r@ write-file throw
    2drop 2drop drop
    name[[ >name count 1+ r@ write-file throw
    m[[ ]]size1 m[[ ]]size2 * dfloats  m[[ ]]T dup s>f ]]data swap
    r> write-file throw  f>s ]]free } ;

\ saving and restoring floating point stack
0 value tempfloat 0 value tempfloat2
: pushfstack
    fdepth dup gsl_vector_alloc to tempfloat
    0 ?do
	tempfloat i ]!
    loop ;
: savefloat2
    fdepth dup gsl_vector_alloc to tempfloat2 0
    ?do
	tempfloat2 i ]!
    loop ;

: restorefloat
    0 tempfloat ]size
    ?do
	i 0= if leave then
	tempfloat i 1- ]@ -1
    +loop tempfloat ]free ;

: restorefloat2
    0 tempfloat2 ]size
    ?do
	i 0= if leave then	
	tempfloat2 i 1- ]@ -1
    +loop tempfloat2 ]free ;

: popfstack
    fdepth 0<> if
    	savefloat2
	restorefloat
	restorefloat2
	exit
    then
    restorefloat ;   
\ allocate a nameless vector
: :] ( # -- addr ) gsl_vector_calloc ;

\ these words do not work with float matrices but are needed for
\ scientific calculations, that's why they are in this module

: _hmatrix ( n m size -- addr )
    rot over * 2 pick * [ 2 cells ] literal +
    allocate throw dup [ 2 cells ] literal + >r
    rot over ! [ 1 cells ] literal + ! r> ;
: hmatrix ( n m size -- )
    create
    rot over * 2 pick * [ 2 cells ] literal + allocate throw dup ,
    rot over ! [ 1 cells ] literal + !
  does> @ [ 2 cells ] literal + ;
: }}row-size ( hmatrix -- ) [ 2 cells ] literal - @ ;
: freeHmatrix ( hmatrix -- ) [ 2 cells ] literal - free throw ;
: }} ( addr i j -- addr[i][j] )    \ word to fetch 2-D array addresses
    >R >R                          \ indices to return stack temporarily
    DUP CELL- CELL- 2@             \ &a[0][0] size m
    R> * R> + *
    +
    ALIGNED ;

previous previous

Module;
