// BIOLIB header file

#ifndef __SASA_H
#define __SASA_H

#define sqrmat(y,x,a) ((y)*(a) + (x))

#include "maths.h"

int calcDistanceMatrix(float *dmatrix, vector *atom, int atoms);
int calcDistanceMatrix(float *dmatrix, vector *atom, int atoms, float cutoff);
void printSphFile(int number);
void createVertexSphere(vector *vertex,float radius, int number);
void createEvenVertexSphere(vector *vertex,float radius, int number);
int loadXYZcoords(char *filename, vector *point, int number);

/* Approximate atomic vdw/SASA surface area calculation according to
   Wodak, Shenkin and Still (LCPO) (1990) */

typedef struct{
  float P1,P2,P3,P4;		
  float L1,L2,L3,L4;
  float diff;
  float lastdiff;
  float radius;
  signed char  neighbors;
  int          attype;
  char         use;
  int   isum;
} SASAparam;

float calcNumericalSASA(vector *atom, float *radius,int atoms, float probeRadius,
                        char *SPfile,int SurfacePoints,  float *SASA);

// THis function is required for numerical FDPB solution to find the
// effective born radius of an atom
float  calcNumericalSASAShell(vector   *atom,                // atom vectors
                              float    *radius,              // radii
                              int       atoms,               // number of atoms
							  float    *dmatrix,
                              int       shellatom,           // atom number to be shelled
                              float     shellradius,         // radius to be calculated
                              vector    *stdpoint, 
                              int       SurfacePoints);     // unit sphere surface point file







float LCPOcalcTotalSASA(vector *atom, float *radius, SASAparam *sasaparam, 
						   int atoms, float probeRadius, float *SASA);


#endif