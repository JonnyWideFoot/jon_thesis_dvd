/* ------------------------------------------------------------------------ */
/*  copyright by m.j.sippl   08.02.92 */
/* ------------------------------------------------------------------------ */

/*-------------------------------------------------------------------------*/
/* module coordfun   version 2.00 */
/*-------------------------------------------------------------------------*/

/* ------------------------------------------------------------------------ */
/* IMPORTANT NOTICE:							    */
/* This source code may be used free of charge for non profit purposes.     */
/* If used in scientific research please cite the original work (see below).*/
/* For all other purposes please contact:				    */
/* Manfred J.Sippl / Department of Biochemistry / University of Salzburg    */
/* Hellbrunnerstr.34 / A-5020 Salzburg / Austria			    */
/* Tel.: 0662-8044-5781							    */
/* EAN (electronic mail): 1SIPPL@EDVZ.UNI-SALZBURG.ADA.AT		    */
/* ------------------------------------------------------------------------ */

/* ------------------------------------------------------------------------ */
/* Please report any problems and applications to M.J.Sippl		    */
/* ------------------------------------------------------------------------ */

#include <stdlib.h>

#include "coordfun.h"

/*--------------------------------------------------------------------------*/
/* functions */
/*--------------------------------------------------------------------------*/

/*------------------------------------------------------------------------- */
/* 3x3 matrix utilities */
/*------------------------------------------------------------------------- */

void coo_unit_matrix(double m[3][3])
/* ------------------------------------------------------------------------ */
/*  initializes matrix m as the identity matrix:

       | 1.0  0.0  0.0 |
   m = | 0.0  1.0  0.0 |
       | 0.0  0.0  1.0 |      */
{
  m[0][0] = 1.0;  m[0][1] = 0.0; m[0][2] = 0.0;
  m[1][0] = 0.0;  m[1][1] = 1.0; m[1][2] = 0.0;
  m[2][0] = 0.0;  m[2][1] = 0.0; m[2][2] = 1.0;
}


void coo_zero_matrix(double m[3][3])
/* ------------------------------------------------------------------------ */
/*  initializes matrix m as a zero matrix:

        | 0.0  0.0  0.0 |
    m = | 0.0  0.0  0.0 |
        | 0.0  0.0  0.0 |    */
{
  m[0][0] = 0.0;  m[0][1] = 0.0; m[0][2] = 0.0;
  m[1][0] = 0.0;  m[1][1] = 0.0; m[1][2] = 0.0;
  m[2][0] = 0.0;  m[2][1] = 0.0; m[2][2] = 0.0;
}

void coo_transpose(double m[3][3])
/* ------------------------------------------------------------------------ */
/* transposes m */
{
  double sw;

  sw = m[0][1]; m[0][1] = m[1][0]; m[1][0] = sw;
  sw = m[0][2]; m[0][2] = m[2][0]; m[2][0] = sw;
  sw = m[1][2]; m[1][2] = m[2][1]; m[2][1] = sw;
}

void coo_mat_mul(double a[3][3], double b[3][3], double d[3][3])
/* ------------------------------------------------------------------------ */
/* calculates matrix product c = a x b  and returns result in b */
{
  double c[3][3];
  int i,j,k;

  for (i=0;i<3;i++) for(j=0;j<3;j++) {
    c[i][j] = 0.0;
    for (k=0;k<3;k++) c[i][j] += a[i][k]*b[k][j];
  }
  for (i=0;i<3;i++) for(j=0;j<3;j++) d[i][j] = c[i][j];
}


#define ROTATE(a,i,j,k,l) g=a[i][j];h=a[k][l];a[i][j]=g-s*(h+g*tau);\
	a[k][l]=h+s*(g-h*tau);

int coo_jacobi(double a[3][3], double v[3][3], double d[3])
/* ------------------------------------------------------------------------ */
/* calculates eigenvectors and eigenvalues for a[3][3]
   eigenvectors are returned in v[3][3] as columns
   eigenvalues  are returned in d[3]
   elements of a above diagonal are destroyed
   function returns number of Jacabi rotations   nrot */
{
   int    n=3;
   int    nrot=0;
   int    maxiter=100;
   int    j,iq,ip,i;
   double tresh,theta,tau,t,sm,s,h,g,c,b[3],z[3],*nrvector();

   coo_unit_matrix(v);
   for (ip=0;ip<n;ip++) {
     b[ip]=d[ip]=a[ip][ip];
     z[ip]=0.0;
   }
   for (i=1;i<=maxiter;i++) {
     sm=0.0;
     for (ip=0;ip<n-1;ip++) for (iq=ip+1;iq<n;iq++) sm += fabs(a[ip][iq]);
     if (sm < 1.0e-30) return i;
     if (i < 4) tresh=0.2*sm/(n*n);
     else       tresh=0.0;
     for (ip=0;ip<n-1;ip++) {
       for (iq=ip+1;iq<n;iq++) {
         g=100.0*fabs(a[ip][iq]);
         if (i > 4 && fabs(d[ip])+g == fabs(d[ip])
                   && fabs(d[iq])+g == fabs(d[iq])) a[ip][iq]=0.0;
         else if (fabs(a[ip][iq]) > tresh) {
           h=d[iq]-d[ip];
           if (fabs(h)+g == fabs(h))  t=(a[ip][iq])/h;
           else {
             theta=0.5*h/(a[ip][iq]);
             t=1.0/(fabs(theta)+sqrt(1.0+theta*theta));
             if (theta < 0.0) t = -t;
           }
           c=1.0/sqrt(1+t*t);
           s=t*c;
           tau=s/(1.0+c);
           h=t*a[ip][iq];
           z[ip] -= h;
           z[iq] += h;
           d[ip] -= h;
           d[iq] += h;
           a[ip][iq]=0.0;
           for (j=0;j<=ip-1;j++) {ROTATE(a,j,ip,j,iq)}
           for (j=ip+1;j<=iq-1;j++) {ROTATE(a,ip,j,j,iq)}
           for (j=iq+1;j<n;j++) {ROTATE(a,ip,j,iq,j)}
           for (j=0;j<n;j++) {ROTATE(v,j,ip,j,iq)}
           ++nrot;
         }
       }
     }
     for (ip=0;ip<n;ip++) {
       b[ip] += z[ip];
       d[ip]=b[ip];
       z[ip]=0.0;
     }
   }
   printf("Too many iterations in routine JACOBI\n");
   return 0;
}
#undef ROTATE

void coo_print_mat(double m[3][3])
/* ------------------------------------------------------------------------ */
/* transposes m */
{
   int i,j;

   printf("\n");
   for (i=0;i<3;i++) {
     for (j=0;j<3;j++) printf(" %10.3f",m[i][j]);
     printf("\n");
   }
}

void coo_copy_matrix(double old[3][3], double new[3][3])
/* ------------------------------------------------------------------------ */
/* copies old to new */
{
  int i,j;

  for (i=0; i<3; i++) for (j=0; j<3; j++) new[i][j] = old[i][j];
}

/*------------------------------------------------------------------------- */
/* coordinates utilities */
/*------------------------------------------------------------------------- */

void coo_copy_point(COORDS *c1, COORDS *c2)
/*-----------------------------------------------------------------------*/
/* copies coordinates from c1 to c2. */
{
    c2->x = c1->x;
    c2->y = c1->y;
    c2->z = c1->z;
}

void coo_copy_coords(COORDS *c1, COORDS *c2, int n)
/*--------------------------------------------------------------------------*/
/*  copies n coordinates from array c1 to array c2.
    NOTE: ensure that c2 is large enough the hold copy. */
{
    int			 i;

    for (i = 0; i < n; i++) {
	c2[i].x = c1[i].x;
	c2[i].y = c1[i].y;
	c2[i].z = c1[i].z;
    }
}

COORDS *coo_acopy_coords(COORDS *old, COORDS *new, int n)
/*--------------------------------------------------------------------------*/
/*  allocates and copies n coordinates from array c1 to array c2. */
{
    int i;

    if((new = (COORDS *) calloc(n,sizeof(COORDS))) == 0) {
      printf("alloc failed unable to copy COORDS\n");
      return 0;
    }
    for (i = 0; i < n; i++) {
	new[i].x = old[i].x;
	new[i].y = old[i].y;
	new[i].z = old[i].z;
    }
  return new;
}

CARRAY *coo_acopy_carray(CARRAY *old, CARRAY *new)
/*--------------------------------------------------------------------------*/
/* allocates new and copies old to new */
{
  int i;

  if (old == 0) {
	printf("old empty, unable to copy carray\n");
	return 0;
  }
  if ((new = coo_alloc_carray(new,old->n)) == 0) return 0;
  for (i=0; i<old->n; i++) {
    new->coo[i].x = old->coo[i].x;
    new->coo[i].y = old->coo[i].y;
	new->coo[i].z = old->coo[i].z;
  }
  return new;
}

/*------------------------------------------------------------------------- */
/* translation & geometric center */
/*------------------------------------------------------------------------- */

void coo_shiftm_coords(COORDS *coo, COORDS *point, int n)
/* ------------------------------------------------------------------------ */
/* translates coordinate array coo of length n by "point".
   coo[i] -> coo[i] - point (subtracting) */
{
  int i;

  for (i=0; i<n; i++) {
	coo[i].x -= point->x;
	coo[i].y -= point->y;
	coo[i].z -= point->z;
  }
}

void coo_shiftp_coords(COORDS *coo, COORDS *point, int n)
/* ------------------------------------------------------------------------ */
/* translates coordinate array coo of length n by "point".
   coo[i] -> coo[i] + point (adding) */
{
  int i;

  for (i=0; i<n; i++) {
	coo[i].x += point->x;
	coo[i].y += point->y;
	coo[i].z += point->z;
  }
}

void coo_geo_center(COORDS *c, COORDS *s, int n)
/*-----------------------------------------------------------------------*/
/*  calculates geometric center s from coordinate array c of length n.
    returns coordinates of center in s. */
{
	int			 i;
	double		   n1;

	n1 = 1.0 /  n;
    s->x = s->y = s->z = 0.0;

    for (i = 0; i < n; i++) {
	  s->x += c[i].x;
	  s->y += c[i].y;
	  s->z += c[i].z;
    }
    s->x = s->x * n1;
    s->y = s->y * n1;
    s->z = s->z * n1;

    return;
}

void coo_center_coords(COORDS *coo, int n)
/* ------------------------------------------------------------------------ */
/* calculates geometric center from coordinate array coo of length n
   and translates coordinates to center. */
{
  double cx,cy,cz;
  int    i;

  cx = cy = cz = 0.0;
  for (i=0; i < n; i++) {
    cx += coo[i].x;
    cy += coo[i].y;
    cz += coo[i].z;
  }
  cx = cx / n;
  cy = cy / n;
  cz = cz / n;

  for (i=0; i < n; i++) {
    coo[i].x -= cx;
    coo[i].y -= cy;
    coo[i].z -= cz;
  }
  return;
}



/* ------------------------------------------------------------------------ */
/*	              coordinate rotating functions			    */
/* ------------------------------------------------------------------------ */

void coo_rot_point(double mat[3][3], COORDS *coo)
/* ------------------------------------------------------------------------ */
/* rotates coordinates coo by mat. */
{
  double x,y,z;

  x = coo->x; y = coo->y; z = coo->z;

  coo->x =  mat[0][0] * x + mat[0][1] * y + mat[0][2] * z;
  coo->y =  mat[1][0] * x + mat[1][1] * y + mat[1][2] * z;
  coo->z =  mat[2][0] * x + mat[2][1] * y + mat[2][2] * z;
}

void coo_invrot_point(double mat[3][3], COORDS *coo)
/* ------------------------------------------------------------------------ */
/* rotates coordinates coo by inverse mat. */
{
  double x,y,z;

  x = coo->x; y = coo->y; z = coo->z;

  coo->x =  mat[0][0] * x + mat[1][0] * y + mat[2][0] * z;
  coo->y =  mat[0][1] * x + mat[1][1] * y + mat[2][1] * z;
  coo->z =  mat[0][2] * x + mat[1][2] * y + mat[2][2] * z;
}

void coo_rot_coo(COORDS *coo, int n, double mat[3][3])
/* ------------------------------------------------------------------------ */
/* rotates the n coordinates coo by rotation matrix mat. */
{
  int i;
  double x,y,z;

  for (i=0; i < n; i++) {
    x = coo[i].x;
    y = coo[i].y;
    z = coo[i].z;

    coo[i].x =  mat[0][0] * x + mat[0][1] * y + mat[0][2] * z;
    coo[i].y =  mat[1][0] * x + mat[1][1] * y + mat[1][2] * z;
    coo[i].z =  mat[2][0] * x + mat[2][1] * y + mat[2][2] * z;
  }
  return;
}


/* ------------------------------------------------------------------------ */
/* memory allocation and io */
/* ------------------------------------------------------------------------ */

CARRAY *coo_alloc_carray(CARRAY *obj, int n)
/* ------------------------------------------------------------------------ */
/* alloc CARRAY */
{
	obj = (CARRAY *) calloc(1, sizeof(CARRAY));
	obj->n   = n;
    obj->coo = (COORDS *) calloc(n, sizeof(COORDS));
    return obj;
}

CARRAY *coo_free_carray(obj)
/* ------------------------------------------------------------------------ */
CARRAY *obj;
/* free CARRAY */
{
    if (obj != 0) {
	  if (obj->coo != 0) free(obj->coo);
	  free(obj);
    }
    return 0;
}

CARRAY *coo_read_carray(CARRAY *obj, char *file)
/* ------------------------------------------------------------------------ */
/*  reads coordinates from ASCII "file"
	file format is:
	x1 y1 z1
	x2 y2 z2
	.
	.
	xn yn zn */
{
	COORDS         *c;
    FILE           *fp;
    char            buffer[256];
    int             i, n;

	if (obj != 0) obj = coo_free_carray(obj);
    if ((fp = fopen(file, "r")) == 0) {
	printf("Unable to open %s\n", file);
	return 0;
    }
    /* count number of points in object */
    n = 0;
    while (fgets(buffer, 256, fp) != 0)
	n++;
	obj = coo_alloc_carray(obj,n);

    /* read coordinates of object */
    rewind(fp);
	c = obj->coo;
    i = 0;
    while (fgets(buffer, 256, fp) != 0) {
	sscanf(buffer, "%f %f %f", &c[i].x, &c[i].y, &c[i].z);
	/*
	 * printf("%3d  %6.2f %6.2f %6.2f\n",i,c[i].x,c[i].y,c[i].z); 
	 */
	i++;
    }
    fclose(fp);
    return obj;
}

void coo_print_coords(COORDS *c, int n)
/* ------------------------------------------------------------------------ */
/* print coordinates on stdout */
{
  int i;

  for (i=0; i<n; i++) 
	printf("%3d %10.3f %10.3f %10.3f\n",i,c[i].x,c[i].y,c[i].z);
}
