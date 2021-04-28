#include <stdio.h>
#include <string.h>
#include <math.h>
#include <stdlib.h>
#include <stdarg.h>

#include "biolib.h"
#include "ffparam.h"
#include "sasa.h"

/* ------------------------------------------------------------------------
                       Global variable initiation 
   ------------------------------------------------------------------------ */
const char *aalibfull[] = {
	                    "ALA","GLY","PRO","ARG",
                        "ASN","ASP","CYS","GLN",
                        "GLU","HIS","ILE","LEU",
                        "LYS","MET","PHE","SER",
                        "THR","TRP","TYR","VAL",

	                    "NALA","NGLY","NPRO","NARG",
                        "NASN","NASP","NCYS","NGLN",
                        "NGLU","NHIS","NILE","NLEU",
                        "NLYS","NMET","NPHE","NSER",
                        "NTHR","NTRP","NTYR","NVAL",

	                    "CALA","CGLY","CPRO","CARG",
                        "CASN","CASP","CCYS","CGLN",
                        "CGLU","CHIS","CILE","CLEU",
                        "CLYS","CMET","CPHE","CSER",
                        "CTHR","CTRP","CTYR","CVAL"
};
const char aalib[] = {
	                  'A','G','P','R',
                      'N','D','C','Q',
                      'E','H','I','L',
                      'K','M','F','S',
                      'T','W','Y','V',
	                  'A','G','P','R',
                      'N','D','C','Q',
                      'E','H','I','L',
                      'K','M','F','S',
                      'T','W','Y','V',
	                  'A','G','P','R',
                      'N','D','C','Q',
                      'E','H','I','L',
                      'K','M','F','S',
                      'T','W','Y','V'};
const char aalibnums[] = {
                        0,-1, 6, 5, 8,14,
                        1, 9,10,-1,12,11,
                       13, 4,-1, 2, 7,
                        3,15,16,-1,19,
                       17,-1,18};


char getAALetterFromFullName(char *resname){   //This function
												  //returns a one letter code from a 3 letter code
	int i;
	for(i=0;i<60;i++){
      if(strcmp(aalibfull[i],resname)==0) return aalib[i];
	}
    return -1;  
}

char getAANumberFromFullName(char *resname){   //This function returns a number 
	int i;
	for(i=0;i<60;i++){
      if(strcmp(aalibfull[i],resname)==0) return i;
	}
    return -1;  
}



/* ------------------------------------------------------------------------
                       PDB Input / Output functions 
   ------------------------------------------------------------------------ */


PDBfile::PDBfile(){
 filename[0]=0;
 sequence=NULL;
 atom = NULL;
 atoms=0;
}

PDBfile::~PDBfile(){
 delete [] atom;
 delete [] sequence;
}

int PDBfile::readPDBfile(char *filename, unsigned char options){
 return readPDBfile(filename, 0, options);
}




int PDBfile::readPDBfile(char *filename, unsigned int model, unsigned char options){
 FILE *file;
 char buffer[256];
 int curatom=0;

 PDB tatom[PDB_maxatoms]; 
 char atype[20],rtype[20],crap[20];
 int  anum,rnum;
 float x,y,z;
 int  canum=0,i,j;
 int  lastresnum;

 char pdbsequence[PDB_maxresidues];
 int  pdbsequence_cnt = 0;
 char pdbresletter;

 if((options & PDB_errormsg) != 0)
  printf("Reading PDB file <%s> ...",filename);
 
 file = fopen(filename,"r");
 if(file==NULL){
  if((options & PDB_errormsg) != 0)
   printf(". ERROR: file not found\n",filename);
  return -1;
 }
 
 strcpy(&this->filename[0],filename);

 if(model > 0){                                      // fastforward to model of choice
                                                     // not yet implemented, alpha version 

 }
 
 lastresnum=-1;
 
 pdbsequence[0]=0;
 pdbsequence_cnt=0;

 do{
   fgets(&buffer[0],256,file);
   if(curatom >= PDB_maxatoms) break;
   if(strncmp(&buffer[0],"ENDMDL",6)==0) break;      // finished reading model
   if(strncmp(&buffer[0],"ATOM",4)!=0) continue;     // ignore everything but atom lines

   if(
   sscanf(&buffer[0],"ATOM %d %s %s %d %f %f %f",
          &anum,
          &tatom[curatom].atnam[0],
          &tatom[curatom].resnam[0],
          &tatom[curatom].resnum,
          &x, &y, &z)
   !=7) {
     // this format was unsuccessfull in extracting all 7 items, thus try again
	   //
     sscanf(&buffer[0],"ATOM %d %s %s %s %d %f %f %f",
            &anum,
            &tatom[curatom].atnam[0],
            &tatom[curatom].resnam[0],
	    &crap[0],
            &tatom[curatom].resnum,
            &x, &y, &z);
   }
   
   if( (options & PDB_ignoreHydrogens) != 0){        // ignore if its a hydrogen
      if((tatom[curatom].atnam[0] == 'H')||
         (tatom[curatom].atnam[1] == 'H')) continue;
   }

   if(tatom[curatom].resnum != lastresnum){          // are we starting a new residue ?
     pdbresletter = getAALetterFromFullName(&tatom[curatom].resnam[0]);  // determine residue letter          
	 if(pdbresletter == -1) { 
        pdbsequence[pdbsequence_cnt] = '-';
	 }else{
	    pdbsequence[pdbsequence_cnt] = pdbresletter;    // take note of the new residue name    	 
	 }
     pdbsequence_cnt ++;							 // increase counter    
     lastresnum = tatom[curatom].resnum;             // remember new residue number
   }

   tatom[curatom].x = x;                             // save positions
   tatom[curatom].y = y;
   tatom[curatom].z = z;

   if( (options & PDB_print) != 0){
     printf("ATOM %d %s %s %d %f %f %f \n",
     anum,
     &tatom[curatom].atnam[0],
     &tatom[curatom].resnam[0],
     tatom[curatom].resnum,
     x,y,z);
   }
 
   curatom++;
   
 }while(!feof(file));                                // quit if reached end of file

 // Deal with the atoms
 atoms = curatom;
 if(atom != NULL) delete [] atom;                       // clear previous memory
 atom = new PDB[atoms+1];                            // allocate new memory
 memcpy(atom, &tatom[0], sizeof(PDB) * atoms);       // copy atom info into new space
 firstres = atom[0].resnum;                          // assign some number for orientation 
 lastres  = atom[atoms-1].resnum;
 
 
 //Deal with the sequence
 pdbsequence[pdbsequence_cnt] = 0;                   // zero terminate the sequence string
 if(sequence != NULL) delete [] sequence;
 sequence = new char[ strlen(&pdbsequence[0]) + 2 ];
 strcpy(sequence,&pdbsequence[0]);
 residues = strlen(sequence);


 printf("The sequence is: %s ",&pdbsequence[0]);
 
 if((options & PDB_errormsg) != 0)
    printf(".. ok .. %d atoms recorded..\n",atoms);
 
 fclose(file);	
 return 0;
}


int PDBfile::print(){
 return print(stdout);
}

int PDBfile::print(FILE *file){
 for(int i=0;i<atoms;i++){

   fprintf(file,"ATOM %d %s %s %d %f %f %f \n",
     i,
     &atom[i].atnam[0],
     &atom[i].resnam[0],
     atom[i].resnum,
     atom[i].x,atom[i].y,atom[i].z);
 
 }
 return 0;
}

int PDBfile::findAtom(char *name,int nres){
 int index=-1;
 int i;

 for(i=0;i<atoms;i++){
  if((strcmp(&atom[i].atnam[0],name)==0)&&
             (atom[i].resnum == nres)){
   return i;
  }
 }

 return -1;
}


void convertHydrogenName(char *tgt,char *src){  // converts names like HD11 into 1HD1
 int i;
 tgt[0] = src[strlen(src)-1];
 for(i=1;i<strlen(src);i++){
   tgt[i] = src[i-1];
 }
 tgt[strlen(src)] = 0;
}

int PDBfile::cmpAtom(int atnum, int resnum, char *resnam, char *atnam){
 char nbuf[25];
 char *nstr;
 
	  

 if( (atom[atnum].resnum - firstres) != resnum) return false;   //same residue number
 
 if( strcmp(&atom[atnum].resnam[0],resnam) != 0) return false;  //same residue name
 
 
 if( strcmp(&atom[atnum].atnam[0],atnam) == 0) return true;      // same atom name

 if(( atnam[0] == 'H' )){   // this may be 4char hydrogen atom
    convertHydrogenName(&nbuf[0],atnam);
    // try another comparison
    if( strcmp(&atom[atnum].atnam[0],&nbuf[0]) == 0) return true;      // same atom name

 }

 nstr="";
 if(resnum == 0){   //  in the case of the first residue
  if( strcmp(atnam,"HT1")==0) nstr="1H";
  if( strcmp(atnam,"HT2")==0) nstr="2H";
  if( strcmp(atnam,"HT3")==0) nstr="3H";
  if( strcmp(atnam,"HN1")==0) nstr="1H";
  if( strcmp(atnam,"HN2")==0) nstr="2H";
 }
 if( strcmp(atnam,"OT1")==0) nstr="O";
 if( strcmp(atnam,"OT2")==0){
	 nstr="OXT";
 }

 if( strcmp(&atom[atnum].atnam[0],nstr) == 0) return true;      // same atom name

 return false;
}

					 
					 
	
int PDBfile::getAtomVectors(vector *vatom){ // assumes there is enough space in array !
 for(int i=0;i<atoms;i++){
  vatom[i].x = atom[i].x;  
  vatom[i].y = atom[i].y;  
  vatom[i].z = atom[i].z;  
 }
 return atoms;
}



