/* ------------------------------------------------------------------------ */
/*  copyright by m.j.sippl   08.02.92 */
/* ------------------------------------------------------------------------ */

/*-------------------------------------------------------------------------*/
/* module superpos  version 2.00 */
/*-------------------------------------------------------------------------*/

#ifndef SUPERIMP_H
#define SUPERIMP_H

#include "coordfun.h"

/* ------------------------------------------------------------------------ */
/* exported functions */
/* ------------------------------------------------------------------------ */

extern double spp_brm[3][3];
extern int    spp_maxiter;
extern int    spp_iter;
extern int    spp_max_retries;
extern int    spp_retries;

/* ------------------------------------------------------------------------ */
/* exported functions */
/* ------------------------------------------------------------------------ */

extern void   spp_set_precision(double prec);
extern double spp_superpose_rot(COORDS *x, COORDS *y, int len);
extern double spp_superpose_full(COORDS *x, COORDS *y, int len);
extern double spp_superperr_rot(COORDS *x, COORDS *y, int len);
extern double spp_superperr_full(COORDS *x, COORDS *y, int len);
extern void   spp_fast(COORDS *x, COORDS *y, int len);
extern void   spp_cycle(void);
extern int    spp_optimat(void);
extern double spp_error(COORDS *x, COORDS *y, int len);
extern double spp_error_fix(COORDS *x, COORDS *y, int len);
extern void   spp_centers(COORDS *x, COORDS *y, int len);
extern void   spp_shift_xy(COORDS *x, COORDS *y, int len);
extern double spp_sin_angle(double arg1, double arg2);
extern void   spp_restore(COORDS *x, COORDS *y, int len);
extern void   spp_xxtensor(COORDS *x, int len);
extern void   spp_xytensor(COORDS *x, COORDS *y, int len);
extern void   spp_get_centers(COORDS *x, COORDS *y, int len);

extern void   spp_cycle_slow(void);
extern double spp_sin_angle_slow(double arg1, double arg2);

#endif

