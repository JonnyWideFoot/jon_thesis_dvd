// ffparam.h
// * ------------------------------------------------------------------------
//                      Atomic Parameter Loading
//    ------------------------------------------------------------------------ */

//
//   WORK in PRogress !!! This set of function ouht to be united into a single
//   Class structure for neatness & allowing several parameter sets to be invoked at once
//
//



#ifndef __FFPARAM_H
#define __FFPARAM_H

#define MAX_ATOMS_PER_MOLECULE  30
#define MAX_ATOMTYPES           30
#define MAX_RESTRAINTS 20
#define MAX_TYPES 400


class ForceFieldParameterSet;      // the main class structure, defined/declared later



typedef struct __RestraintList{
    int   n;                       // number of restraints
    int   i[MAX_RESTRAINTS];       // partner indices
} RestraintList;


typedef struct _AtomParameter{
// constitutive members
  char  united;				  // united (1) or not (0) ?	
  char  num;                  // ?
  char  valid;                // 0 for invalid (to be removed/ignored) !=0 for valid
  
  char  aacid;                // which aa does it belong to ?
  char  pdbname[6];           // PDB name
  char  type;                 // type code

  float x,y,z;                // initial coordinates

  int   Z;					  // atomic number
  float mass;                 // mass in kg
  float radius;               // VdW radius
  float epsilon;              // Epsilon (VdW well depth)

  float charge;               // Formal/Partial charge

  // additional members
  float solv;                 // solvation parameter
  int   hbond;                // hbond code;

  int   iparam1,iparam2,iparam3; // extra parameters
  float fparam1,fparam2,fparam3; 

  RestraintList r_covalent;   // intra amino acid restraints (bonds);
} AtomParameter;

class MoleculeDefinition{
 public: 
  int atoms;
  char letter;
  char name[40];
  AtomParameter atom[MAX_ATOMS_PER_MOLECULE];    // stores Atom information

  MoleculeDefinition(){
	  atoms=0;
	  letter=0;
	  name[0]=0;  
  };

  int findAtom(char *atom_name);
  int checkRestraintList();
  int decodeRestraintList(char *data,int atnum);
  int readMoleculeBlock(FILE *file, int *line, ForceFieldParameterSet *ffps);
};

#define MAX_TYPENAME_LENGTH 256

class AtomTypeParameter{           // used to save atom type definitions
 public:
  char name[MAX_TYPENAME_LENGTH];
  char used;
  AtomParameter p;

  AtomTypeParameter(){
	  name[0]=0;
  };
};

class BondTypeParameter{
public:
  char  used;
  int   i,j;						// type identifier (in AtomType[])
  float length;
  float forceconstant;

  int   readDefinitionLine(char *linestr,int line, 
						   ForceFieldParameterSet *ffps);
  int   cmpBond(int ti, int tj);    // compares if the bond type matches t{i,j}
};

class AngleTypeParameter{
public:
  char  used;
  int   i,a,j;						// type identifier (in AtomType[])
  float angle;
  float forceconstant;

  int   readDefinitionLine(char *linestr,int line,
	                       ForceFieldParameterSet *ffps);
  int   cmpAngle(int ti,int ta, int tj);
};

class TorsionTypeParameter{
public:
  char  used;
  int   i,a,b,j;						// type identifier (in AtomType[])

  int   terms;                        // how many fourier terms
  float Vn[4];                        // the individual parameters for each term
  float n[4];
  float gamma[4];

  int   readDefinitionLine(TorsionTypeParameter *list,
	                       int list_length,char *linestr,
						   int line,
						   ForceFieldParameterSet *ffps);
  int   addTerm(float newVn,float newn,float newgamma);
  int   cmpTorsion(int ti,int ta,int tb,int tj);
  void  zero();
  void  print();

};







class ForceFieldParameterSet{
 private:

   void printAtomParameters(AtomParameter *apar);
   int AtomTypeReadLine(char *linestr, int line);
 public:
 	 
   char *fffilename;     // Filename of parameter set

   MoleculeDefinition    aaparam[60]; 

   MoleculeDefinition    molecule[100]; 
   int                   nMolecules;

   AtomTypeParameter     AtomType[MAX_TYPES];
   BondTypeParameter     BondType[MAX_TYPES];
   AngleTypeParameter    AngleType[MAX_TYPES];
   TorsionTypeParameter  TorsionType[MAX_TYPES];

   int                   nAtomTypes;
   int                   nBondTypes;
   int                   nAngleTypes;
   int                   nTorsionTypes;
 
   ForceFieldParameterSet(){ fffilename=NULL; 
                             nMolecules = 0; };
   ~ForceFieldParameterSet(){ };
   
   int readExplicitAtomTypeFile(char *filename);
   int writeForceFieldParameterFile(char *filename);
   int AtomTypeFind(char *name);

   int  unifyForcefield();
   void checkUsedTypes();

   int findBondType(int ti, int tj);
   int findAngleType(int ti,int ta, int tj);
   int findTorsionType(int ti,int ta, int tb,int tj);

   int findMoleculeType(char *molname);
};





#endif