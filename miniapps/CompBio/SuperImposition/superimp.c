/* ------------------------------------------------------------------------ */
/*  Copyright by Manfred J.Sippl             25/01/93                       */
/* ------------------------------------------------------------------------ */
/* ------------------------------------------------------------------------ */
/*			  superimpose module                                */
/* ------------------------------------------------------------------------ */

/*
-------------------------------------------------------------------------
 Code may be used free of charge for non profit purposes.
 Reports of applications or extensions are very wellcome.
 Please report problems to
 Manfred J.Sippl
 Center for Applied Molecular Engineering /
 University of Salzburg
 Salzburger TechnoZ
 Jakob Haringer Str.1 / A-5020 Salzburg / Austria
 Tel.: (043) 662-8044-5797
 Fax.: (043) 662-454889
 INTERNET: SIPPL@DSB835.EDVZ.SBG.AC.AT
-------------------------------------------------------------------------

/* ------------------------------------------------------------------------ */
/* Literature:								    */
/*                    Manfred J.Sippl, Hans Stegbuchner			    */
/*         Superposition of three-dimensional objects: A fast and           */
/*         numerically stable algorithm for the calculation of the	    */
/*                        matrix of optimal rotation			    */
/*         Computers and Chemistry 15:73-78, 1991			    */
/* ------------------------------------------------------------------------ */

/*
=========================================================================
IMPLEMENTATION NOTES:

The module contains three functions which calculate rms-errors of
optimal superimpositions:

(1) spp_superpose_rot()
    Function returns the square of the rms-error. The coordinates of
    X remain unchanged. Y is optimally superimposed on X.
    The coordinates of X and Y are not translated to their geometric centers,
    i.e. no minimization with respect to translation is performed.
(2) spp_superpose_full()
    Function returns the square of the rms-error. The coordinates of
    X remain unchanged. Y is optimally superimposed on X.
(3) spp_superperr_rot()
    Function returns the square of the rms-error. The coordinataes of X
    and Y remain unchanged. The coordinates of X and Y are not translated
    to their geometric centers, i.e. no minimization with respect to
    translation is performed.
(4) spp_superperr_full()
    Function returns the square of the rms-error. The coordinates of
    X and Y remain unchanged.

Note on failure of convergence:
If the algorithm fails to converge the iteration is restarted by rotating
the coordinates of X; this sometimes helps.
If the algorithm fails again the iteration is restarted by rotating X
to the eigensystem defined by the tensor of X; this sometimes helps too.
if all fails the algorithm terminates and returns the rms-error using
the current result. In this case the rms-error and superimposition are
unreliable, although the superimposition may still be useful.
- if you find a better way please let me know -

You may retry by changing the maximum number of iterations performed.

Note that this version of the algorithm contains a few additions and
corrections with respect to Sippl & Stegbuchner (1991).

==========================================================================
*/

#include "superimp.h"

#define FRADIANS 0.017453292

/* ------------------------------------------------------------------------ */
/* exported variables */
/* ------------------------------------------------------------------------ */

COORDS spp_cx;                          /* center coordinates of X */
COORDS spp_cy;                          /* center coordinates of Y */
double spp_brm[3][3];                   /* matrix of optimal rotation */
int    spp_maxiter = 300;               /* maximum number of iterations */
int    spp_iter;                        /* number of iterations */

/* ------------------------------------------------------------------------ */
/* private functions */
/* ------------------------------------------------------------------------ */

static void   rot_x(double cose, double sine, double mat[3][3]);
static void   rot_y(double cose, double sine, double mat[3][3]);
static void   rot_z(double cose, double sine, double mat[3][3]);
static void   spp_reorient_x(COORDS *x, COORDS *y, int len);
static void   spp_eigensys_x(COORDS *x, COORDS *y, int len);

/* ------------------------------------------------------------------------ */
/* private variables */
/* ------------------------------------------------------------------------ */

static double spp_tolerance = FRADIANS;  /* default = 1 degree */
static double spp_epsilon   = 0.0174524; /* sin(tolerance) */
static double spp_no_rot    = 0.0043634; /* sin(tolerance/4) */
static double spp_ssum;

static double u[3][3];        /* cross tensor */
static double xm[3][3];       /* rotation matrix for reorienting x */
static double xt[3][3];       /* tensor of X */
static double sine;           /* sine   of current angle */
static double cose;           /* cosine of current angle */

void spp_set_precision(double prec)
/* ------------------------------------------------------------------------ */
/* set internal values for accuracy of superimposition (prec in degrees) */
{
  spp_tolerance = FRADIANS*prec;
  spp_epsilon = sin(spp_tolerance);
  /* if sum of angles in one cycle  < epsilon algorithm converged */
  spp_no_rot  = sin(spp_tolerance*0.25);
  /* no rotation is performed if angle < prec/4 */
}

double spp_superpose_rot(COORDS *x, COORDS *y, int len)
/* ------------------------------------------------------------------------ */
/* calculate root mean square error of superimposition
   X remains unchanged
   Y is optimally oriented
   No translations are performed */
{
  spp_fast(x,y,len);
  coo_rot_coo(y,len,spp_brm);
  return spp_error(x,y,len);
}

double spp_superpose_full(COORDS *x, COORDS *y, int len)
/* ------------------------------------------------------------------------ */
/* calculate root mean square error of superimposition
   X remains unchanged
   Y is optimally oriented */
{
  spp_centers(x,y,len);
  spp_fast(x,y,len);
  coo_rot_coo(y,len,spp_brm);
  spp_shift_xy(x,y,len);
  return spp_error(x,y,len);
}

double spp_superperr_rot(COORDS *x, COORDS *y, int len)
/* ------------------------------------------------------------------------ */
/* calculate root mean square error of superimposition
   X and Y remain unchanged
   No translations are performed */
{
  spp_fast(x,y,len);
  return spp_error_fix(x,y,len);
}

double spp_superperr_full(COORDS *x, COORDS *y, int len)
/* ------------------------------------------------------------------------ */
/* calculate root mean square error of superimposition
   X and Y remain unchanged */
{
  double r;

  spp_centers(x,y,len);
  spp_fast(x,y,len);
  r = spp_error_fix(x,y,len);
  spp_restore(x,y,len);
  return r;
}

void spp_fast(COORDS *x, COORDS *y, int len)
/* ------------------------------------------------------------------------ */
/* root mean square error of superimposition kernel routine
   X and Y remain unchanged
   No translations are performed */
{
  spp_xytensor(x,y,len);
  while (1) {
    /* Fast rms calculation */
    if (spp_optimat() == 1) {
      break;
    }
    else printf("Fast rms-calculation failed\n");
    /* restart from new orientation of X */
    spp_reorient_x(x,y,len);
    if (spp_optimat() == 1) {           /* find optimal rotation */
      coo_mat_mul(xm,spp_brm,spp_brm);  /* correct for rotation of X */
      break;
    }
    else printf("Fast rms-calculation failed after reorientation\n");
    /* restart from Eigensystem of X */
    spp_eigensys_x(x,y,len);
    if (spp_optimat() == 1) {
      coo_mat_mul(xm,spp_brm,spp_brm);
      break;
    }
    else {
      printf("Fast rms-calculation failed in eigensystem\n");
      printf("Warning: rms-value and optimal rotation are unrealiable\n");
    }
    break;
  }
}

int spp_optimat(void)
/* ------------------------------------------------------------------------ */
/* calculates the matrix "spp_brm" of optimal superposition.
   returns 1 if iteration converged
   returns 0 if iteration did not converge
   iteration converged if spp_angle_sum < spp_tolerance
   maximum number of iteration cycles is spp_maxiter */
{
  spp_iter = 0;
  spp_ssum = 0.0;

  coo_unit_matrix(spp_brm);

  while(spp_iter <= spp_maxiter) {
    spp_cycle();
    spp_iter++;
    if (spp_ssum < spp_epsilon) 
		return 1;
  }
  return 0;
}

double spp_error(COORDS *s1, COORDS *s2, int len)
/* ------------------------------------------------------------------------ */
/* calculates the square of the root-mean-square distance of s1 and s2 */
{
  COORDS *c1, *c2;
  register double rms = 0.0, d;
  register int   i;

  for (i = 0; i < len; i++) {
    c1 = s1+i;
    c2 = s2+i;

    d = c1->x - c2->x;  rms += d*d;
    d = c1->y - c2->y;  rms += d*d;
    d = c1->z - c2->z;  rms += d*d;
  }
  return rms / len;      /* returns squared rms error */
}

double spp_error_fix(COORDS *s1, COORDS *s2, int len)
/* ------------------------------------------------------------------------ */
/* calculates the square of the root-mean-square distance of s1 and s2 */
{
  double x1,y1,z1,x2,y2,z2;
  register double rms = 0.0, d;
  register int   i;

  for (i = 0; i < len; i++) {
    x1 =  s1[i].x; y1 =  s1[i].y; z1 =  s1[i].z;
    x2 =  s2[i].x; y2 =  s2[i].y; z2 =  s2[i].z;

    d =  x1 - (spp_brm[0][0]*x2 + spp_brm[0][1]*y2 + spp_brm[0][2]*z2);
    rms += d * d;
    d =  y1 - (spp_brm[1][0]*x2 + spp_brm[1][1]*y2 + spp_brm[1][2]*z2);
    rms += d * d;
    d =  z1 - (spp_brm[2][0]*x2 + spp_brm[2][1]*y2 + spp_brm[2][2]*z2);
    rms += d * d;
  }
  rms = rms / len;      /* returns squared rms error */
  return rms;
}

void spp_centers(COORDS *x, COORDS *y, int len)
/* ------------------------------------------------------------------------ */
/* calculates centers of coordinates of X and Y and shifts coordinates */
{
  COORDS *xx, *yy;
  int i;

  spp_cx.x = spp_cx.y = spp_cx.z = 0.0;
  spp_cy.x = spp_cy.y = spp_cy.z = 0.0;

  for (i=0; i<len; i++) {
    xx = x+i; yy = y+i;
    spp_cx.x += xx->x; spp_cx.y += xx->y; spp_cx.z += xx->z;
    spp_cy.x += yy->x; spp_cy.y += yy->y; spp_cy.z += yy->z;
  }
  spp_cx.x /= len; spp_cx.y /= len; spp_cx.z /= len;
  spp_cy.x /= len; spp_cy.y /= len; spp_cy.z /= len;
  for (i=0; i<len; i++) {
    xx = x+i; yy = y+i;
    xx->x -= spp_cx.x; xx->y -= spp_cx.y; xx->z -= spp_cx.z;
    yy->x -= spp_cy.x; yy->y -= spp_cy.y; yy->z -= spp_cy.z;
  }
}

void spp_shift_xy(COORDS *x, COORDS *y, int len)
/* ------------------------------------------------------------------------ */
/* shifts coordinates of X and Y to original location of X */
{
  COORDS *xx, *yy;
  int i;

  for (i=0; i<len; i++) {
    xx = x+i; yy = y+i;
    xx->x += spp_cx.x; xx->y += spp_cx.y; xx->z += spp_cx.z;
    yy->x += spp_cx.x; yy->y += spp_cx.y; yy->z += spp_cx.z;
  }
}

void spp_restore(COORDS *x, COORDS *y, int len)
/* ------------------------------------------------------------------------ */
/* shifts coordinates of X and Y back to original location */
{
  COORDS *xx, *yy;
  int i;

  for (i=0; i<len; i++) {
    xx = x+i; yy = y+i;
    xx->x += spp_cx.x; xx->y += spp_cx.y; xx->z += spp_cx.z;
    yy->x += spp_cy.x; yy->y += spp_cy.y; yy->z += spp_cy.z;
  }
}

void spp_xxtensor(COORDS *x, int len)
/* ------------------------------------------------------------------------ */
/* calculates tensor of COORDS object x. */
{
  double xx,xy,xz;
  int i;

  coo_zero_matrix(xt);
  for (i=0; i<len; i++) {
    xx = x[i].z; xy = x[i].y; xz = x[i].z;
    xt[0][0] += xx * xx; xt[1][1] += xy * xy; xt[2][2] += xz * xz;
    xt[0][1] += xx * xy; xt[0][2] += xx * xz; xt[1][2] += xy * xz;
  }
  xt[1][0] = xt[0][1]; xt[2][0] = xt[0][2]; xt[2][1] = xt[1][2];
}

void spp_xytensor(COORDS *x, COORDS *y, int len)
/* ------------------------------------------------------------------------ */
/* calculates mixed tensor of COORDS objects x and y. */
{
  double xx,xy,xz,yx,yy,yz;
  int k;

  coo_zero_matrix(u);

  for (k=0; k < len; k++) {
    xx = x[k].x; xy = x[k].y; xz = x[k].z;
    yx = y[k].x; yy = y[k].y; yz = y[k].z;

    u[0][0] += xx * yx; u[0][1] += xy * yx; u[0][2] += xz * yx;
    u[1][0] += xx * yy; u[1][1] += xy * yy; u[1][2] += xz * yy;
    u[2][0] += xx * yz; u[2][1] += xy * yz; u[2][2] += xz * yz;
  }

}

void spp_cycle(void)
/* ------------------------------------------------------------------------ */
/* successively calculates optimal angles alpha, beta and gamma
   and performs the associate rotations */
{
  double abs_v;
  double uNum;

  spp_ssum = 0.0;

  /* Find optimal rotation around x-axis (angle alpha) */
  abs_v = spp_sin_angle(u[1][2]-u[2][1],u[1][1]+u[2][2]);
  if (abs_v > spp_no_rot) {rot_x(cose,sine,u); rot_x(cose,sine,spp_brm);}
  spp_ssum += abs_v;

  uNum = u[0][0];
  uNum = u[0][1];
  uNum = u[0][2];
  uNum = u[1][0];
  uNum = u[1][1];
  uNum = u[1][2];
  uNum = u[2][0];
  uNum = u[2][1];
  uNum = u[2][2];


  /* Find optimal rotation around y-axis (angle gamma) */
  abs_v = spp_sin_angle(u[0][2]-u[2][0],u[0][0]+u[2][2]);
  if (abs_v > spp_no_rot) {rot_y(cose,sine,u); rot_y(cose,sine,spp_brm);}
  spp_ssum += abs_v;

    uNum = u[0][0];
  uNum = u[0][1];
  uNum = u[0][2];
  uNum = u[1][0];
  uNum = u[1][1];
  uNum = u[1][2];
  uNum = u[2][0];
  uNum = u[2][1];
  uNum = u[2][2];

  /* Find optimal rotation around z-axis (angle gamma) */
  abs_v = spp_sin_angle(u[0][1]-u[1][0],u[0][0]+u[1][1]);
  if (abs_v > spp_no_rot) {rot_z(cose,sine,u); rot_z(cose,sine,spp_brm);}
  spp_ssum += abs_v;

    uNum = u[0][0];
  uNum = u[0][1];
  uNum = u[0][2];
  uNum = u[1][0];
  uNum = u[1][1];
  uNum = u[1][2];
  uNum = u[2][0];
  uNum = u[2][1];
  uNum = u[2][2];
}

double spp_sin_angle(double arg1, double arg2)
/* ------------------------------------------------------------------------ */
/* calculates angle and determines second derivative
   since tan(a) = +/- sin(a) / cos(a)
   a = +/- atan(sin(a)/cos(a)) = atan(+/-sin(a)/cos(a))
   if cos(a) * arg2 + sin(a) * arg1 > 0  solution is a minimum
   returns absolute value of sinus of angle.
   on 386 with coprocessor this function is approximately three times
   faster as compared with spp_sin_angle_slow using atan2, sin and cos.
   */
 {
  double tang;

  if (arg1 == 0.0) return 0.0;
  /* sin(x) = tan(x)/sqrt(1.0+tan(x)**2); cos(x) = 1.0/sqrt(1.0+tan(x)**2) */
  tang = arg1/arg2;
  cose = 1.0/sqrt(1.0+tang*tang);
  if (arg2 < 0.0) cose = -cose;
  sine = tang*cose;
  if (cose*arg2 + sine*arg1 < 0.0) sine = -sine;  /* second solution */
  if (sine < 0.0) return -sine;
  else            return  sine;
}

static void rot_x(double cose, double sine, double mat[3][3])
/* ------------------------------------------------------------------------ */
/* calculates the matrix product c = a * mat where a is a rotation
   around axis X. */
{
  double arg1, arg2;

  arg1 = cose * mat[1][0] - sine * mat[2][0];
  arg2 = sine * mat[1][0] + cose * mat[2][0];
  mat[1][0] = arg1;
  mat[2][0] = arg2;

  arg1 = cose * mat[1][1] - sine * mat[2][1];
  arg2 = sine * mat[1][1] + cose * mat[2][1];
  mat[1][1] = arg1;
  mat[2][1] = arg2;

  arg1 = cose * mat[1][2] - sine * mat[2][2];
  arg2 = sine * mat[1][2] + cose * mat[2][2];
  mat[1][2] = arg1;
  mat[2][2] = arg2;
  return;
}

static void rot_y(double cose, double sine, double mat[3][3])
/* ------------------------------------------------------------------------ */
/* calculates the matrix product c = a * mat where a is a rotation
   around axis Y. This is a clockwise rotation. */
{
  double arg1, arg2;

  arg1 =  cose * mat[0][0] - sine * mat[2][0];
  arg2 =  sine * mat[0][0] + cose * mat[2][0];
  mat[0][0] = arg1;
  mat[2][0] = arg2;

  arg1 =  cose * mat[0][1] - sine * mat[2][1];
  arg2 =  sine * mat[0][1] + cose * mat[2][1];
  mat[0][1] = arg1;
  mat[2][1] = arg2;

  arg1 =  cose * mat[0][2] - sine * mat[2][2];
  arg2 =  sine * mat[0][2] + cose * mat[2][2];
  mat[0][2] = arg1;
  mat[2][2] = arg2;
  return;
}

static void rot_z(double cose, double sine, double mat[3][3])
/* ------------------------------------------------------------------------ */
/* calculates the matrix product c = a * mat where a is a rotation
   around axis Z. */
{
  double arg1, arg2;

  arg1 = cose * mat[0][0] - sine * mat[1][0];
  arg2 = sine * mat[0][0] + cose * mat[1][0];
  mat[0][0] = arg1;
  mat[1][0] = arg2;

  arg1 = cose * mat[0][1] - sine * mat[1][1];
  arg2 = sine * mat[0][1] + cose * mat[1][1];
  mat[0][1] = arg1;
  mat[1][1] = arg2;

  arg1 = cose * mat[0][2] - sine * mat[1][2];
  arg2 = sine * mat[0][2] + cose * mat[1][2];
  mat[0][2] = arg1;
  mat[1][2] = arg2;
  return;
}

static void spp_reorient_x(COORDS *x, COORDS *y, int len)
/* ------------------------------------------------------------------------ */
/* rotate x and set up for new orientation */
{
  coo_unit_matrix(xm);
  cose = sine = 0.707106781;  /* 45 degrees */
  rot_x(cose,sine,xm);        /* build rotation matrix */
  rot_y(cose,sine,xm);        /* build rotation matrix */
  rot_z(cose,sine,xm);        /* build rotation matrix */
  coo_rot_coo(x,len,xm);      /* rotate x */
  spp_xytensor(x,y,len);      /* calculate cross tensor */
  coo_transpose(xm);          /* get inverse rotation */
  coo_rot_coo(x,len,xm);      /* restore X */
}

static void spp_eigensys_x(COORDS *x, COORDS *y, int len)
/* ------------------------------------------------------------------------ */
/* transform x to eigensystem and set up for new orientation */
{
  double x_eig[3];

  spp_xxtensor(x,len);
  /* get eigenvectors */
  if (coo_jacobi(xt,xm,x_eig) == 0) {
    printf("Jacobi iteration failed (superpos)\n");
  }
  coo_rot_coo(x,len,xm);
  spp_xytensor(x,y,len);
  coo_transpose(xm);
  coo_rot_coo(x,len,xm);   /* restore X */
}

void spp_cycle_slow(void)
/* ------------------------------------------------------------------------ */
/* successively calculates optimal angles alpha, beta and gamma
   and performs the associate rotations
   uses slow angle calculation and is outdated */
{
  double abs_v;

  spp_ssum = 0.0;

  /* Find optimal rotation around x-axis (angle alpha) */
  abs_v = spp_sin_angle_slow(u[1][2]-u[2][1],u[1][1]+u[2][2]);
  if (abs_v > spp_no_rot) {rot_x(cose,sine,u); rot_x(cose,sine,spp_brm);}
  spp_ssum += abs_v;

  /* Find optimal rotation around y-axis (angle gamma) */
  abs_v = spp_sin_angle_slow(u[0][2]-u[2][0],u[0][0]+u[2][2]);
  if (abs_v > spp_no_rot) {rot_y(cose,sine,u); rot_y(cose,sine,spp_brm);}
  spp_ssum += abs_v;

  /* Find optimal rotation around z-axis (angle gamma) */
  abs_v = spp_sin_angle_slow(u[0][1]-u[1][0],u[0][0]+u[1][1]);
  if (abs_v > spp_no_rot) {rot_z(cose,sine,u); rot_z(cose,sine,spp_brm);}
  spp_ssum += abs_v;
}


double spp_sin_angle_slow(double arg1, double arg2)
/* ------------------------------------------------------------------------ */
/* calculates angle and determines second derivative
   tan(a) = +/- sin(a) / cos(a)
   therefore
   a = +/- atan(sin(a)/cos(a)) = atan(+/-sin(a)/cos(a))
   if cos(a) * arg2 + sin(a) * arg1 > 0  solution is a minimum
   returns absolute value of sinus of angle
   slow version due to atan - outdated */
 {
  double angle;

  if (arg1 == 0.0) return 0.0;

  angle = atan2(arg1,arg2);
  cose = cos(angle);
  sine = sin(angle);
  if (cose*arg2 + sine*arg1 < 0.0) sine = -sine; /* second solution */
  if (sine < 0.0) return -sine;
  else            return  sine;
}

