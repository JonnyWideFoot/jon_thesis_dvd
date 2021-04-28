/* ------------------------------------------------------------------------ */
/*  Copyright by Manfred J.Sippl                          3/4/91            */
/* ------------------------------------------------------------------------ */

#ifndef COORDFUN_H
#define COORDFUN_H

#include <stdio.h>
#include <math.h>

typedef struct coords COORDS;
typedef struct carray CARRAY;

struct coords {
  float x,y,z;
};

struct carray {
  COORDS *coo;
  int     n;
};


/* ------------------------------------------------------------------------ */
/*			functions in module coordfun			    */
/* ------------------------------------------------------------------------ */

extern void    coo_unit_matrix(double m[3][3]);
extern void    coo_zero_matrix(double m[3][3]);
extern void    coo_transpose(double m[3][3]);
extern void    coo_mat_mul(double a[3][3], double b[3][3], double d[3][3]);
extern int     coo_jacobi(double a[3][3], double v[3][3], double d[3]);
extern void    coo_print_mat(double m[3][3]);
extern void    coo_copy_matrix(double old[3][3], double newm[3][3]);
extern void    coo_copy_point(COORDS *c1, COORDS *c2);
extern void    coo_copy_coords(COORDS *c1, COORDS *c2, int n);
extern COORDS *coo_acopy_coords(COORDS *old, COORDS *newa, int n);
extern CARRAY *coo_acopy_carray(CARRAY *old, CARRAY *newa);
extern void    coo_shiftm_coords(COORDS *coo, COORDS *point, int n);
extern void    coo_shiftp_coords(COORDS *coo, COORDS *point, int n);
extern void    coo_geo_center(COORDS *c,COORDS *s,int n);
extern void    coo_center_coords(COORDS *coo, int n);
extern void    coo_rot_point(double mat[3][3], COORDS *coo);
extern void    coo_invrot_point(double mat[3][3], COORDS *coo);
extern void    coo_rot_coo(COORDS *coo, int n, double mat[3][3]);

extern CARRAY *coo_alloc_carray(CARRAY *car, int n);
extern CARRAY *coo_free_carray(CARRAY *obj);
extern CARRAY *coo_read_carray(CARRAY *obj, char *file);

extern void    coo_print_coords(COORDS *c, int n);


#endif
