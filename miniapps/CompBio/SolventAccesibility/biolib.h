// BIOLIB header file

#ifndef __BIOLIB_H
#define __BIOLIB_H

#define BIOLIBversion  0.01

#include "maths.h"
#include "ffparam.h"
#include <stdlib.h>


// Convertion tables & macros from aminoacid numbers to letter
// and vice versa

extern const char *aalibfull[];
extern const char aalib[];
extern const char aalibnums[];

#define getAANameFull(x) (aalibfull[(x)])
#define getAALetter(x)   (aalib[(x)])
#define getAANumber(x)   (aalibnums[(x - 'A')])
char    getAALetterFromFullName(char *resname);
char    getAANumberFromFullName(char *resname);

// Units 'n stuff
#define __kcal2J 4186.8
#define __J2kcal 2.39E-4

#define __Na        6.002E23                // particles per mol (mol-1)
#define __kB        1.38066E-23             // JK-1

#define __e0        8.854187817E-12         // C^2/J m 
#define __4pi_e0    (4*__e0*pi)      

#define __e_charge  1.60217E-19             // C ,eletronic charge
#define __sqr_e_charge  2.5669487089E-38    // C2,square of electronic charge

#define __Angstrom     1E-10
#define __invAngstrom  1E10

#define __amu       1.66057E-27    // atomic mass unit




typedef struct _PDB
{
   float x,y,z;
   int  resnum;
   char atnam[8];
   char resnam[8];
}  PDB;

class PDBfile {
 public:
   PDBfile();
   ~PDBfile();

   
   int readPDBfile(char *filename, unsigned char options);
   int readPDBfile(char *filename, unsigned int model, unsigned char options);
   int print();
   int print(FILE *file);
   int findAtom(char *name,int nres); 

   int cmpAtom(int atnum, int resnum, char *resnam, char *atnam);

   int getAtomVectors(vector *vatom);

   int atoms;
   char filename[256];
   PDB *atom;
   char *sequence;
   int residues,firstres, lastres;
};


#define PDB_ignoreHydrogens 0x01
#define PDB_readHydrogens   0x00 
#define PDB_print           0x02
#define PDB_errormsg        0x04

#define PDB_maxatoms        10000
#define PDB_maxresidues     1000

/*     Particle/Atom structures    */
/* ------------------------------  */

/* ------------------------------------------------------------------------
               Macromolecular Surface Area Calculations
   ------------------------------------------------------------------------ */

#define sqrmat(y,x,a) ((y)*(a) + (x))
int calcDistanceMatrix(float *dmatrix, vector *atom, int atoms);
int calcDistanceMatrix(float *dmatrix, vector *atom, int atoms, float cutoff);

#endif
