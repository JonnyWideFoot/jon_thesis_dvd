#include <stdio.h>
#include <string.h>
#include <math.h>
#include <stdlib.h>
#include <stdarg.h>

#include "ffparam.h"
#include "biolib.h"





/// --------------------------------------------------------------
//  BondTypeParameter Class Function Definitions

int BondTypeParameter::readDefinitionLine(char *linestr, int line, ForceFieldParameterSet *ffps){
 // Reads out a line like this and saves it in itself
 // BOND              BB_C      BB_O        1.2330       9.2000

 int n;
 char itypename[MAX_TYPENAME_LENGTH],jtypename[MAX_TYPENAME_LENGTH];

 n=sscanf(linestr,"BOND    %s %s %f %f\n",
	      &itypename[0],&jtypename[0],&length,&forceconstant);

 if(n<4){
	 printf("Insufficient BOND parameters (line %d)\n",line);
	 return -1;
 }

 used = 1;  // set used flag to 'yes'
 i = ffps->AtomTypeFind(&itypename[0]);
 if(i<-1){ 
	 printf("Type identifier %s unknown (line %d)\n",&itypename[0],line);
	 return -1;
 }
 j = ffps->AtomTypeFind(&jtypename[0]);
 if(j<-1){ 
	 printf("Type identifier %s unknown (line %d)\n",&jtypename[0],line);
	 return -1;
 }

 //Do any unit conversions
 forceconstant *= __kcal2J / __Na;		 // kcal/mol to J (SI)



 //printf("Bond %s(%d) %s(%d) %f %f\n",
 //	 	      &itypename[0],i,&jtypename[0],j,length,forceconstant);


 
 return 0;
}



int BondTypeParameter::cmpBond(int ti, int tj){  // compares if the bond type matches t{i,j}
 int result;
 int wildcards;
	     
 result = 2;
 wildcards=0;


 if (i == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (i == ti) result-=1;          // otherwise look for a match
 if (j == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (j == tj) result-=1;          // otherwise look for a match

 if(result == 0)                       // if all 2 matched, then result is 0 by now
	 return wildcards;                 // return number of wildcards

 // if no forward match is achieved, try backwards

 result = 2;
 wildcards=0;

 if (i == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (i == tj) result-=1;          // otherwise look for a match
 if (j == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (j == ti) result-=1;          // otherwise look for a match

 if(result == 0)                       // if all 2 matched, then result is 0 by now
	 return wildcards;                 // return number of wildcards

 return -1;                       // otherwise return -1 indicating no match
}














/// --------------------------------------------------------------
//  AngleTypeParameter Class Function Definitions


int AngleTypeParameter::readDefinitionLine(char *linestr, int line, ForceFieldParameterSet *ffps){
 // Reads out a line like this and saves it in itself
 // ANGLE       BB_N    BB_C    BB_O     1.2330   9.2000

 int n;
 char itypename[MAX_TYPENAME_LENGTH],
	  atypename[MAX_TYPENAME_LENGTH],
	  jtypename[MAX_TYPENAME_LENGTH];

 n=sscanf(linestr,"ANGLE   %s %s %s %f %f\n",
	      &itypename[0],&atypename[0],&jtypename[0],&angle,&forceconstant);
 if(n<5){
	 printf("Insufficient ANGLE parameters (line %d)\n",line);
	 return -1;
 }

 used = 1;  // set used flag to 'yes'
 i = ffps->AtomTypeFind(&itypename[0]);
 if(i<-1){ 
	 printf("Type identifier %s unknown (line %d)\n",&itypename[0],line);
	 return -1;
 }
 a = ffps->AtomTypeFind(&atypename[0]);
 if(a<-1){ 
	 printf("Type identifier %s unknown (line %d)\n",&atypename[0],line);
	 return -1;
 }
 j = ffps->AtomTypeFind(&jtypename[0]);
 if(j<-1){ 
	 printf("Type identifier %s unknown (line %d)\n",&jtypename[0],line);
	 return -1;
 }

 //printf("Angle %s(%d) %s(%d) %s(%d) %f %f\n",
 //	 	      &itypename[0],i,&atypename[0],a,&jtypename[0],j,angle,forceconstant);

 // Do any unit conversion

 angle *= pi/180;            // degrees to radians
 forceconstant *=  __kcal2J / __Na; // kcal/mol/rad to J/rad (SI)

 return 0;
}


int AngleTypeParameter::cmpAngle(int ti,int ta,int tj){  // compares if the angle type matches t{i,a,j}
 int result;
 int wildcards;
	     
 result = 3;
 wildcards=0;

 if (i == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (i == ti) result-=1;          // otherwise look for a match
 if (a == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (a == ta) result-=1;          // otherwise look for a match
 if (j == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (j == tj) result-=1;          // otherwise look for a match

 if(result == 0)                       // if all 4 matched, then result is 0 by now
	 return wildcards;                 // return number of wildcards

 // if no forward match is achieved, try backwards

 result = 3;
 wildcards=0;

 if (i == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (i == tj) result-=1;          // otherwise look for a match
 if (a == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (a == ta) result-=1;          // otherwise look for a match
 if (j == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (j == ti) result-=1;          // otherwise look for a match

 if(result == 0)                       // if all 4 matched, then result is 0 by now
	 return wildcards;                 // return number of wildcards

 return -1;                       // otherwise return -1 indicating no match
}








/// --------------------------------------------------------------
//  TorsionTypeParameter Class Function Definitions

int TorsionTypeParameter::cmpTorsion(int ti,int ta,int tb,int tj){  // compares if the torsion type matches t{i,a,b,j}
 int result;
 int wildcards;
	     
 result = 4;
 wildcards=0;

 if (i == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (i == ti) result-=1;          // otherwise look for a match
 if (a == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (a == ta) result-=1;          // otherwise look for a match
 if (b == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (b == tb) result-=1;          // otherwise look for a match
 if (j == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (j == tj) result-=1;          // otherwise look for a match

 if(result == 0)                       // if all 4 matched, then result is 0 by now
	 return wildcards;                 // return number of wildcards

 // if no forward match is achieved, try backwards

 result = 4;
 wildcards=0;

 if (i == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (i == tj) result-=1;          // otherwise look for a match
 if (a == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (a == tb) result-=1;          // otherwise look for a match
 if (b == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (b == ta) result-=1;          // otherwise look for a match
 if (j == -1){ result-=1; wildcards++;} // if its a wildcard, its always a match
 else if (j == ti) result-=1;          // otherwise look for a match

 if(result == 0)                       // if all 4 matched, then result is 0 by now
	 return wildcards;                 // return number of wildcards

 return -1;                       // otherwise return -1 indicating no match
}

void TorsionTypeParameter::zero(){
  terms=0;
  a = -2;
  i = -2;
  j = -2;
  b = -2;
}

int TorsionTypeParameter::addTerm(float newVn,float newn,float newgamma){
 if(terms>=4) {
		 printf("Maximum of 4 fourier terms supported\n");
		 return -1;
 }

 Vn[terms] = newVn;
 n[terms] = newn;
 gamma[terms] = newgamma;
 terms++;

 return 0;
}

int TorsionTypeParameter::readDefinitionLine(TorsionTypeParameter *list,
											 int list_length, char *linestr, 
											 int line, ForceFieldParameterSet *ffps){
 // Reads out a line like this and adds it to itself
 // TORSION       BB_CA   BB_N    BB_C    BB_O     ???    ???    ???
 // 
 // list is a pointer to an array, of length list_length
	
 int n,prevtors;
 char itypename[MAX_TYPENAME_LENGTH],
	  atypename[MAX_TYPENAME_LENGTH],
	  btypename[MAX_TYPENAME_LENGTH],
	  jtypename[MAX_TYPENAME_LENGTH];
 float newVn,newn,newgamma;

 n=sscanf(linestr,"TORSION   %s %s %s %s  %f %f %f\n",
	      &itypename[0],&atypename[0],&btypename[0],&jtypename[0],
		  &newVn,&newgamma,&newn);
 if(n<7){
	 printf("Insufficient TORSION parameters (line %d)\n",line);
	 return -1;
 }

 used = 1;  // set used flag to 'yes'
 i = ffps->AtomTypeFind(&itypename[0]);
 if(i<-1){ 
	 printf("Type identifier %s unknown (line %d)\n",&itypename[0],line);
	 return -1;
 }
 a = ffps->AtomTypeFind(&atypename[0]);
 if(a<-1){ 
	 printf("Type identifier %s unknown (line %d)\n",&atypename[0],line);
	 return -1;
 }
 b = ffps->AtomTypeFind(&btypename[0]);
 if(b<-1){ 
	 printf("Type identifier %s unknown (line %d)\n",&btypename[0],line);
	 return -1;
 }
 j = ffps->AtomTypeFind(&jtypename[0]);
 if(j<-1){ 
	 printf("Type identifier %s unknown (line %d)\n",&jtypename[0],line);
	 return -1;
 }

 // Do any unit conversion
 newVn     *= __kcal2J/__Na;           // kcal/mol --> J / atom
 newgamma  *= pi/180;             //deg --> rad
 
 //now find out if there was a previous torsion of the same type (i.e. 
 //we need to add a new fourier term, not an entirely new torsion type
 for(prevtors=0;prevtors<list_length;prevtors++){
  if( (list[prevtors].i == i) &&
	  (list[prevtors].a == a) &&
	  (list[prevtors].b == b) &&
	  (list[prevtors].j == j) ){
      break;
  }
 }

 if(prevtors < list_length){    // yep there was a previous torsion of same type
   list[prevtors].addTerm(newVn,newn,newgamma); // ok, add to previous one
   return -1;                   // signal not increase torsion counter
 }else{                         // no previous torsion of same type found
   addTerm(newVn,newn,newgamma);
   return 0;                    // signal to increase torsion counter
 }

 return 0;
}

void  TorsionTypeParameter::print(){
 int h;

 printf("TORSION   %3d %3d %3d %3d  %6.3f %6.1f %2.f\n",
	     i,a,b,j,Vn[0]*__J2kcal*__Na,gamma[0]*180/pi,n[0]);
 for(h=1;h<terms;h++){
  printf("                  %6.3f %6.1f %2.f\n",
	      Vn[h]*__J2kcal*__Na,gamma[h]*180/pi,n[h]);
 }

}












/// --------------------------------------------------------------
//  MoleculeDefinition Class Function Definitions


int MoleculeDefinition::findAtom(char *atom_name){
 int index=-1;                 // sift through all atoms of that aacid to find the one referenced
 for(int i=0;i<atoms;i++){
	if( strcmp(&atom[i].pdbname[0],atom_name)==0 ){
        index=i;
	    break;
	}
 }
 return index;
}



int MoleculeDefinition::decodeRestraintList(char *data,int atnum){
 int slen = strlen(data);
 int i,pos=0,rpos=0,rest_index,n;
 char rstring[6];
#define RL_DIVIDER ','

 AtomParameter *aparam = &atom[atnum];

 aparam->r_covalent.n = 0;						// no restraints to start off with

 while(pos<slen){
 	 
     // Find next restraint (all characters up to next komma)	 
	 // & write them into the string rstring[] 
	 rpos=0;
	 while( ( pos < slen) &&
		    (rpos < 6)   &&
		    ( data[pos] != RL_DIVIDER ) ){
      rstring[rpos] = data[pos];
      rpos ++;
      pos ++ ;
	 }
     rstring[rpos]=0;               // terminate string
 	 pos++;							// skip past the komma (',')

	 //check the string is sensible // this may have to go !
	 if( (strlen(&rstring[0]) > 6) ||
		 (strlen(&rstring[0]) < 1) ){
      printf("SYNTAX ERROR: Restraint maldefined (wrong length)\n");
	  continue;
	 }
	 

	 // now find rest index

     rest_index = findAtom(&rstring[0]);
	 if(rest_index==-1){			// if atom name unknown, warn & skip
          printf("ERROR: Unknown atom identifier in amino acid %s, atom %s \n",
			     &name[0],&aparam->pdbname[0]);
		  continue;
	 }
	 
     if(aparam->r_covalent.n >= MAX_RESTRAINTS){
      printf("OVERFLOW ERROR: Too many covalent restraints (MAX_RESTRAINTS = %d) \n",MAX_RESTRAINTS);
      break;
	 }
     // save it down & count up
	 aparam->r_covalent.i[aparam->r_covalent.n] = rest_index ; // save down restraint & convert to zero counting
     aparam->r_covalent.n++;						             // restraint added
 }
 return 0;
}


int MoleculeDefinition::checkRestraintList(){
 int j,k,resti,ok,l;
 int return_value = 0;

 for(j=0;j<atoms;j++){   // for each atom in that amino acid
		 for(k=0;k<atom[j].r_covalent.n;k++){  // for every covalent restraint
          
          resti = atom[j].r_covalent.i[k];     // get the atom reference number for this restraint

		  // Now check if the atom linked to, also links back to this atom.
		  ok = 0; // (false)
		  for(l=0;l<atom[resti].r_covalent.n;l++){
		         if(atom[resti].r_covalent.i[l] == j){  // is atom j part of atom resti's 
																// restraint list ? Yes ?
				  ok = 1; // (true)
                  break;  // break outermost for loop (l)
				 }
			   
		  }
 
		  if((ok==0)){
		   printf("ERROR:  Missing back link for restraint %d, atom %s, molecule %s \n",k+1,&atom[j].pdbname[0],&name[0]);
           return_value = -1;  // error found
		  }
		 }
	 }
 return return_value;
}



int MoleculeDefinition::readMoleculeBlock(FILE *file, int *line, ForceFieldParameterSet *ffps){
  char  buffer[200],command[200];
  char  newname[100],restraint[MAX_ATOMS_PER_MOLECULE][100];
  float x,y,z,charge,mass;
  char  atname[5],resname[5];
  int   atnum,n,k;

  atoms=0;
  while(!feof(file)){                                 // Read line for line  
     buffer[0]=0;
	  fgets(&buffer[0],200,file);
//  printf("%s",&buffer[0]);
     *line+=1;

     if (buffer[0]=='#') continue;           //comment
     if (buffer[0]==0) continue;             //empty line
  
     command[0]=0;
     n = sscanf(&buffer[0],"%s",&command[0]);
     if(n!=1) continue;                     // weired, but wrong
     if(command[0]=='#') continue;          // comment
     
	 if(strcmp(&command[0],"ENDAMINOACID")==0) break;;
	 if(strcmp(&command[0],"ENDMOLECULE")==0) break;;

	 if(strcmp(&command[0],"ATOM")==0){

         //Null all the previous contents of strings
         atname[0]=0;
         newname[0]=0;
         restraint[atoms][0]=0;

         n=sscanf(&buffer[0],"%s %s %f %f %f %s %s",
              &command[0],&atname[0],&x,&y,&z,&newname[0],&restraint[atoms][0]);

		 //printf("%d %s %s %f %f %f %s %s\n",
		 //	     atnum, &command[0],&atname[0],x,y,z,&newname[0],&restraint[atnum][0]);

		 //save restraint string for later analysis
         if(n<6){
           printf("SYNTAX ERROR: Insufficient number of parameters (%d) (line %d)\n",n,*line);
           continue;
		 }
         if(n<7){
	       printf("SYNTAX WARNING: No restraints defined - Atom free (line %d)\n",*line);
		 }
   
         k=ffps->AtomTypeFind(&newname[0]);
   
         if(k<0){                          // name2 was not found
          printf("SYNTAX ERROR: Identifier %s unknown (line %d)\n",&newname[0],*line);
          continue;
		 }

		 //Now assign type to atom parameter

		 memcpy(&atom[atoms],&ffps->AtomType[k].p,sizeof(AtomParameter));
         
		 // Add additional properties unique to the individual atom

         atom[atoms].num = atoms;          // Load in all the atom parameters
         strcpy(&atom[atoms].pdbname[0],&atname[0]);
         atom[atoms].x = x;   
         atom[atoms].y = y;   
         atom[atoms].z = z;   
  
         atoms++;
	 }else
	 if(strcmp(&command[0],"CHARGE")==0){
         atname[0]=0;
         n=sscanf(&buffer[0],"%s %s %f",
              &command[0],&atname[0],&charge);
		 if(n<3){
              printf("ERROR: Insufficient number of parameters (line %d)\n",*line);
			  continue;
		 }
         k=findAtom(&atname[0]);

		 if(k<0){
              printf("ERROR: Atom name \"%s\" unknown (line %d)\n",&atname[0],*line);
			  continue;
		 }
         
		 atom[k].charge = charge;          // assign charge
	 }else
	 if(strcmp(&command[0],"MASS")==0){
         atname[0]=0;
         n=sscanf(&buffer[0],"%s %s %f",
              &command[0],&atname[0],&mass);
		 if(n<3){
              printf("ERROR: Insufficient number of parameters (line %d)\n",*line);
			  continue;
		 }
         k=findAtom(&atname[0]);

		 if(k<0){
              printf("ERROR: Atom name \"%s\" unknown (line %d)\n",&atname[0],*line);
			  continue;
		 }
         
		 atom[k].mass = mass * __amu;          // assign charge
	 }else{
         printf("ERROR: Identifier \"%s\" unknown (line %d)\n",&command[0],*line); //ignore 

	 }


  }


  // Finally decode the restraint list(s) for all atoms
  for(atnum=0;atnum<atoms;atnum++){        
      decodeRestraintList(&restraint[atnum][0],atnum);
  }

  // Now do a check of the restraint information looking for errors
  // in the definition file
  if(checkRestraintList()!=0){
   printf("ERROR: Errors found in restraint lists - check definition files \n");	 
   return -1;
  };

  return 0;  //ok , error free
}












/// --------------------------------------------------------------
//  ForceFieldParameterSet Class Function Definitions

void ForceFieldParameterSet::printAtomParameters(AtomParameter *apar){
  printf(" Z:%2d %4.1famu %4.1fA(rad) %5.2f % 5.2f  ",
     apar->Z,
     apar->mass / __amu,
     apar->radius,
     apar->epsilon * __J2kcal * __Na,
     apar->charge);
    
}



int ForceFieldParameterSet::AtomTypeFind(char *name){
 int i;
 if(strcmp(name,"X")==0)//Found it
     return -1;           //-1 means any atom

 for(i=0;i<nAtomTypes;i++){
   if(strcmp(name,&AtomType[i].name[0])==0)//Found it
     return i;           //break out of for loop & return
 } 
 return -2;               //-2 is an error signal
}



int ForceFieldParameterSet::AtomTypeReadLine(char *linestr, int line){
  char name[256],name2[256];
  float p1=0,p2=0,p3=0,p4=0,p5=0,p6=0,p7=0;
  int n,k;

  n=sscanf(linestr,"TYPE %s %f %f %f %f %f %f %f",
              &name[0],&p1,&p2,&p3,&p4,&p5,&p6,&p7);

  
  if(n<1){
    printf("SYNTAX ERROR: Identifier expected after TYPE (line %d\n",line);
    return -1;
  }

  // ----------------------------------
  // Copy definitions of TYPE

  if(n<2){ //this may be a copy-definition

	n=sscanf(linestr,"TYPE %s %s",
             &name[0],&name2[0]);
    if(n<2){
     printf("SYNTAX WARNING: No parameters defined for %s (line %d)\n",&name[0],line);
	 return -1;
    }

    k=AtomTypeFind(&name2[0]);           //Find already defined &name2[] and save its index
    if(k<0){                             // name2 was not found
      printf("SYNTAX ERROR: Identifier %s unknown (line %d)\n",&name2[0],line);
      return -1;
    }
                     // otherwise copy properties of name2 onto name
   
	memcpy(&AtomType[nAtomTypes],&AtomType[k],sizeof(AtomTypeParameter));
	strcpy(&AtomType[nAtomTypes].name[0],&name[0]);  //save name
    nAtomTypes++;     
    return 0;    
  }
   

  // --------------------------------
  // Normal case (individual parameters given)

   // Warn if parameter list is incomplete
   if(n<6){
    printf("SYNTAX WARNING: Definition incomplete (%d/3 parameters given) (line %d)\n",
           n-1,line); 
   }   

   strcpy(&AtomType[nAtomTypes].name[0],&name[0]);     // save name

   AtomType[nAtomTypes].used      = 1;				   // set used to be true;
   AtomType[nAtomTypes].p.valid   = 1;                 // set valid to true
   AtomType[nAtomTypes].p.type    = nAtomTypes;        // it knows its own type number
   AtomType[nAtomTypes].p.Z       = (int)p1;
   AtomType[nAtomTypes].p.mass    = p2;
   AtomType[nAtomTypes].p.radius  = p3;                // in angstrom 
   AtomType[nAtomTypes].p.epsilon = p4;
   AtomType[nAtomTypes].p.charge  = p5;
   AtomType[nAtomTypes].p.fparam1 = p6;
   AtomType[nAtomTypes].p.fparam2 = p7;

   // Units and other calculations go here !
    //epsilon is given in kcal/mol; convert it into units of J
    AtomType[nAtomTypes].p.epsilon *= __kcal2J/__Na; 
    //AtomType[nAtomTypes].p.epsilon = sqrt(AtomType[nAtomTypes].p.epsilon);
 

    //The mass is given in atomic mass units (amu) not kg
    AtomType[nAtomTypes].p.mass    *= __amu;  // convert to kg !

	nAtomTypes++;
   return 0;
}



int ForceFieldParameterSet::findBondType(int ti, int tj){
  int curcand=-1;
  int curwildcards=4;
  int result;
  int nbond;

  //printf("Looking for %3d %3d type bond ... \n",ti,tj);

  for(nbond=0;nbond<nBondTypes;nbond++){
     result = BondType[nbond].cmpBond(ti,tj);
	 if(result<0)continue;              // no match, go to next
     
/*	 printf("      Found %3d %3d, with %d wildcards\n",
		     BondType[nbond].i,
		     BondType[nbond].j,result);
*/
	 if(result == curwildcards){
       printf("WARNING: Bond type ambiguity due to wildcards. Ignoring second type\n");
	   continue;
	 }
	 if(result < curwildcards){
       curwildcards = result;
	   curcand = nbond;
	 }
  }
  if(curcand < 0){
     printf("WARNING: No appropriate Bond type (%s %s) found - ignoring bond\n",
		    &AtomType[ti].name[0],
			&AtomType[tj].name[0]);
	 return -1;
  }

 return curcand;

}



int ForceFieldParameterSet::findAngleType(int ti,int ta, int tj){
  int curcand=-1;
  int curwildcards=4;
  int result;
  int nang;

//  printf("Looking for %3d %3d %3d type angle ... \n",ti,ta,tj);

  for(nang=0;nang<nAngleTypes;nang++){
     result = AngleType[nang].cmpAngle(ti,ta,tj);
	 if(result<0)continue;              // no match, go to next
     
/*	 printf("      Found %3d %3d %3d, with %d wildcards\n",
		     AngleType[nang].i,
		     AngleType[nang].a,
		     AngleType[nang].j,result);
*/
	 if(result == curwildcards){
       printf("WARNING: Angle type ambiguity due to wildcards. Ignoring second type\n");
	   continue;
	 }
	 if(result < curwildcards){
       curwildcards = result;
	   curcand = nang;
	 }
  }
  if(curcand < 0){
     printf("WARNING: No appropriate Angle type (%s %s %s) found - ignoring angle\n",
		    &AtomType[ti].name[0],
		    &AtomType[ta].name[0],
			&AtomType[tj].name[0]);
	 return -1;
  }

 return curcand;

}


int ForceFieldParameterSet::findTorsionType(int ti,int ta, int tb,int tj){
  int curcand=-1;
  int curwildcards=4;
  int result;
  int ntor;

//  printf("Looking for %3d %3d %3d %3d type torsion ... \n",ti,ta,tb,tj);

  for(ntor=0;ntor<nTorsionTypes;ntor++){
     result = TorsionType[ntor].cmpTorsion(ti,ta,tb,tj);
	 if(result<0)continue;              // no match, go to next
     
/*	 printf("      Found %3d %3d %3d %3d, with %d wildcards\n",
		     TorsionType[ntor].i,
		     TorsionType[ntor].a,
		     TorsionType[ntor].b,
		     TorsionType[ntor].j,result);
*/
	 if(result == curwildcards){
       printf("WARNING: Torsion type ambiguity due to wildcards. Ignoring second type\n");
	   continue;
	 }
	 if(result < curwildcards){
       curwildcards = result;
	   curcand = ntor;
	 }
  }
  if(curcand < 0){
     printf("WARNING: No appropriate Torsion type (%s %s %s %s)found - ignoring torsion\n",
		    &AtomType[ti].name[0],
		    &AtomType[ta].name[0],
		    &AtomType[tb].name[0],
			&AtomType[tj].name[0]);
	 return -1;
  }

 return curcand;
}


int ForceFieldParameterSet::findMoleculeType(char *molname){
 int i;

 for(i=0;i<nMolecules;i++){
	 if(strcmp(&molecule[i].name[0],&molname[0])==0){
       return i;
	 }
 }
 return -1;
}


int ForceFieldParameterSet::readExplicitAtomTypeFile(char *filename){
 FILE  *file;
 int   i,j,line,resnum,n,k;
 char  buffer[200],command[200],name[100],name2[100],restraint[100];
 int   errorstatus=0;

 int   ignore = 0;
 char *ignore_release="";

 fffilename = filename;

 file = fopen(filename,"r");                         // Open the data file
 if(file==NULL){
  printf("Forcefieldfile '%s' not found\n",filename);
  return -1;
 }


 //Clear the library of types
 nAtomTypes=0;
 nBondTypes=0;
 nAngleTypes=0;
 nTorsionTypes=0;
 for(i=0;i<MAX_TYPES;i++){
   TorsionType[i].zero();
 }

 //Start reading parameter file

 line=-1;
 while(!feof(file)){                                 // Read line for line
  buffer[0]=0;
  fgets(&buffer[0],200,file);
  line++;

  //Sort out reading in of first word & handling of comments & empty lines
  if (buffer[0]=='#') continue;           //comment
  if (buffer[0]==0) continue;             //empty line
  command[0]=0;
  n = sscanf(&buffer[0],"%s",&command[0]);
  if(n!=1) continue;                     // weired, but wrong
  if(command[0]=='#') continue;          // comment

  if(ignore){
    if(strcmp(&command[0],ignore_release)==0) ignore=false;    //end ignore section
	continue;
  }

  if(strcmp(&command[0],"SECTION")==0){       // TYPE syntax reading/checking
      ignore = true;
	  ignore_release = "ENDSECTION";
	   
  }else
  if(strcmp(&command[0],"TYPE")==0){       // TYPE syntax reading/checking
     AtomTypeReadLine(&buffer[0],line);
  }else 
  if(strcmp(&command[0],"BOND")==0){       // BOND syntax reading/checking
	  if(nBondTypes >= MAX_TYPES){
          printf("OVERFLOW ERROR:  Too many bond types defined. Maximum = %d\n",MAX_TYPES);
		  continue;
	  }
	  if(BondType[nBondTypes].readDefinitionLine(&buffer[0],line,this)==0){
		  nBondTypes++;                   // only increment if validly read out, otherwise ignore
	  }

  }else
  if(strcmp(&command[0],"ANGLE")==0){      // ANGLE syntax reading/checking
	  if(nAngleTypes >= MAX_TYPES){
          printf("OVERFLOW ERROR:  Too many angle types defined. Maximum = %d\n",MAX_TYPES);
		  continue;
	  }
	  if(AngleType[nAngleTypes].readDefinitionLine(&buffer[0],line,this)==0){
           nAngleTypes++;                  // only increment if validly read out, otherwise ignore
	  }
  }else
  if(strcmp(&command[0],"TORSION")==0){    // TORSION syntax reading/checking
	  if(nTorsionTypes >= MAX_TYPES){
          printf("ERROR:  Too many torsion types defined. Maximum = %d\n",MAX_TYPES);
		  continue;
	  }
	  if(TorsionType[nTorsionTypes].readDefinitionLine(
		&TorsionType[0],nTorsionTypes,&buffer[0],line,this)==0){
           nTorsionTypes++;                // only increment if validly read out 
		                                   // & actually a new torsion was added, otherwise ignore
	  }
  }else
  if(strcmp(&command[0],"AMINOACID")==0){  //  AMNIOACID syntax reading/checking
    n=sscanf(&buffer[0],"AMINOACID %s %s",&name[0],&name2[0]);

	if((n<2)||
	   (strlen(&name[0]) != 1)){
      printf("SYNTAX ERROR:  Insufficient/Invalid Parameters for aminoacid definition (line %d)\n",line);
	  continue;	
	}
	resnum = getAANumberFromFullName(&name2[0]);
	if(resnum == -1) {
      printf("SYNTAX ERROR:  Unknown aminoacid name (line %d)\n",line);
      continue;
	}
          
	       aaparam[resnum].atoms = 0;						//store amino acid parameters
           aaparam[resnum].letter = name[0];
    strcpy(aaparam[resnum].name,&name2[0]);
    
    if(aaparam[resnum].readMoleculeBlock(file, &line,this)!=0)  // read atom information (ENDAMINOACID terminates)
     errorstatus=-1;
	else{
     strcpy(&aaparam[resnum].name[0],&name2[0]);                 // add name
	}

  
  }else 

	  
  if(strcmp(&command[0],"MOLECULE")==0){  //  MOLECULE syntax reading/checking
    n=sscanf(&buffer[0],"MOLECULE %s",&name[0]);

	if(n<1){
      printf("SYNTAX ERROR:  Name expected after MOLECULE (line %d)\n",line);
	  continue;	
	}

    if(molecule[nMolecules].readMoleculeBlock(file, &line,this)!=0)  // read atom information (ENDMOLECULE terminates)
     errorstatus=-1;
	else{
     strcpy(&molecule[nMolecules].name[0],&name[0]);                 // add name
	 nMolecules++;
	}

  }else{
    printf("SYNTAX ERROR: Unknown identifier '%s'(line %d)\n",&command[0],line);
  }


 }

 if(ignore){
    printf("ERROR: Unexpected end of file - no %s found \n",ignore_release);
	errorstatus=1;
 }

 if(errorstatus == 0 )
 printf("Finished reading Forcefield Definition File - no errors\n");
 else
 printf("Errors occurded during forcefield file readout - check script \n");


 unifyForcefield();
 checkUsedTypes();
 
 // print all gathered information
 char newfilename[256];
 strcpy(&newfilename[0],filename);
 strcat(&newfilename[0],".new");
 writeForceFieldParameterFile(&newfilename[0]);
 
 printf("End of parameter read in\n");

 fclose(file);
 return errorstatus;
}


int ForceFieldParameterSet::unifyForcefield(){
 int i,j,k;

 for(i=0;i<60;i++){                    // go through all aminoacid types
	 for(j=0;j<aaparam[i].atoms;j++){  // an their atoms
		 if( aaparam[i].atom[j].Z == 6 ){   // if carbon
            // find all the hydrogens
			 for(k=0;k<aaparam[i].atom[j].r_covalent.n;k++){ // scan through all the covalently bound atoms
			  
				 if(aaparam[i].atom[aaparam[i].atom[j].r_covalent.i[k]].Z == 1){ // found a hydrogen
                    // now add charge of the bound hydrogens to the carbon atom
                    aaparam[i].atom[j].charge += aaparam[i].atom[aaparam[i].atom[j].r_covalent.i[k]].charge; 
					// also add the masses of the hydrogens to the motehr atom
                    aaparam[i].atom[j].mass += aaparam[i].atom[aaparam[i].atom[j].r_covalent.i[k]].mass; 

                    aaparam[i].atom[aaparam[i].atom[j].r_covalent.i[k]].valid = 0; // invalidate the hydrogen
				 }
			}
		 }
  }
 }


 return 0;
}

//this reduces the forecefield parameter set to the minimum, by analysing whihc
//atom types are actually used in the molecule definitions, and according to that
//cutting out unused atom types and covalent structures (angles/bonds/torsions/impropers)
//that contain them. This is done by marking them as unused (by setting used to 0)
//writeForcefieldParaemterfile(...) then ignores these during print.

void ForceFieldParameterSet::checkUsedTypes(){
  int t,i,j;

  //check filetypes first
  for(t=0;t<nAtomTypes;t++){
    AtomType[t].used = 0;
    // now look through all amino acid definitions
    for(i=0;i<60;i++){                    // go through all aminoacid types
    	 for(j=0;j<aaparam[i].atoms;j++){ // an their atoms
           	 if(aaparam[i].atom[j].valid == 0) continue; // ignore invalid atoms
			 if(aaparam[i].atom[j].type == t){
               AtomType[t].used = 1;      // mark as used
               break;
			 }

		 }
		 if(AtomType[t].used == 1) break; // dont bother looking further if found
	}
    if(AtomType[t].used == 1) continue;   // dont bother looking further if found
    // and all molecule definitions
    for(i=0;i<nMolecules;i++){                    // go through all aminoacid types
    	 for(j=0;j<molecule[i].atoms;j++){ // an their atoms
           	 if(molecule[i].atom[j].valid == 0) continue; // ignore invalid atoms
			 if(molecule[i].atom[j].type == t){
               AtomType[t].used = 1;      // mark as used
               break;
			 }

		 }
		 if(AtomType[t].used == 1) break; // dont bother looking further if found
	}
  }

 for(i=0;i<nBondTypes;i++){
	          if( BondType[i].i!=-1)
     if( AtomType[BondType[i].i].used == 0) BondType[i].used = 0;   // set used flag to 0 if atomtype is unused
	          if( BondType[i].j!=-1)
     if( AtomType[BondType[i].j].used == 0) BondType[i].used = 0;   // set used flag to 0 if atomtype is unused
 }

 for(i=0;i<nAngleTypes;i++){
	          if( AngleType[i].i!=-1)
     if( AtomType[AngleType[i].i].used == 0) AngleType[i].used = 0;   // set used flag to 0 if atomtype is unused
	          if( AngleType[i].a!=-1)
     if( AtomType[AngleType[i].a].used == 0) AngleType[i].used = 0;   // set used flag to 0 if atomtype is unused
	          if( AngleType[i].j!=-1)
     if( AtomType[AngleType[i].j].used == 0) AngleType[i].used = 0;   // set used flag to 0 if atomtype is unused
 }
 for(i=0;i<nTorsionTypes;i++){
	          if( TorsionType[i].i!=-1)
     if( AtomType[TorsionType[i].i].used == 0) TorsionType[i].used = 0;   // set used flag to 0 if atomtype is unused
	          if( TorsionType[i].a!=-1)
     if( AtomType[TorsionType[i].a].used == 0) TorsionType[i].used = 0;   // set used flag to 0 if atomtype is unused
	          if( TorsionType[i].b!=-1)
     if( AtomType[TorsionType[i].b].used == 0) TorsionType[i].used = 0;   // set used flag to 0 if atomtype is unused
	          if( TorsionType[i].j!=-1)
     if( AtomType[TorsionType[i].j].used == 0) TorsionType[i].used = 0;   // set used flag to 0 if atomtype is unused
 }
 return;
} 


int ForceFieldParameterSet::writeForceFieldParameterFile(char *filename){
 FILE  *file;
 int i,h,j,k;
 char *X = "X";
 char *ci,*ca,*cb,*cj;

 file = fopen(filename,"w");                         // Open the data file
 if(file==NULL){
  printf("Cannot open forcefield file for writing\n");
  return -1;
 }

 
 fprintf(file,"#   Atom Types\n#\n");
 fprintf(file,"#   Groups      .---<atom>----.  .-----vdw-------.  .--elec--. \n");
 fprintf(file,"#   Type          (Z)  AtMass     Radius  Epsilon    Charge    \n");
 
 for(i=0;i<nAtomTypes;i++) {
     if(AtomType[i].used == 0) continue;  // skip unused atom types
	 fprintf(file,"TYPE    %-10s%2d  %7.2f   %7.4f   %7.4f  %7.4f\n",
		 &AtomType[i].name[0],
		 AtomType[i].p.Z,
         AtomType[i].p.mass / __amu,
		 AtomType[i].p.radius,
		 AtomType[i].p.epsilon / (__kcal2J/__Na),
		 AtomType[i].p.charge);
 }

 fprintf(file,"\n\n\n\n");

  
 fprintf(file,"# Bond types:\n");
 fprintf(file,"# Atom    A   B    length(A)    force constant(kcal/mol/A^2)\n");
 fprintf(file,"#\n");

 for(i=0;i<nBondTypes;i++){
     if(BondType[i].used == 0) continue;  // skip unused types

	 if(BondType[i].i==-1) ci=X; else ci=&AtomType[BondType[i].i].name[0];
	   if(BondType[i].j==-1) cj=X; else cj=&AtomType[BondType[i].j].name[0];

 	   fprintf(file,"BOND   %3s %3s     %7.4f %7.2f \n",
		 ci,cj, BondType[i].length,
		 BondType[i].forceconstant*__J2kcal*__Na);
 }
 fprintf(file,"\n\n\n\n");


 
 fprintf(file,"# Angle types:\n");
 fprintf(file,"# Atom    A   B   C    angle(deg)  force constant(kcal/mol/A^2)\n");
 fprintf(file,"#\n");

 for(i=0;i<nAngleTypes;i++){
       if(AngleType[i].used == 0) continue;  // skip unused types

	   if(AngleType[i].i==-1) ci=X; else ci=&AtomType[AngleType[i].i].name[0];
	   if(AngleType[i].a==-1) ca=X; else ca=&AtomType[AngleType[i].a].name[0];
	   if(AngleType[i].j==-1) cj=X; else cj=&AtomType[AngleType[i].j].name[0];

 	   fprintf(file,"ANGLE   %3s %3s %3s     %6.2f %7.2f \n",
		 ci,ca,cj, AngleType[i].angle*180/pi,
		 AngleType[i].forceconstant*__J2kcal*__Na);
 }
 fprintf(file,"\n\n\n\n");



 fprintf(file,"# Torsion types:\n");
 fprintf(file,"# Atom     A   B   C   D      Vm(kcal/mol) gamma(deg) n  \n");
 fprintf(file,"#\n");

 for(i=0;i<nTorsionTypes;i++){

   for(h=0;h<TorsionType[i].terms;h++){
       if(TorsionType[i].used == 0) continue;  // skip unused types

	   if(TorsionType[i].i==-1) ci=X; else ci=&AtomType[TorsionType[i].i].name[0];
	   if(TorsionType[i].a==-1) ca=X; else ca=&AtomType[TorsionType[i].a].name[0];
	   if(TorsionType[i].b==-1) cb=X; else cb=&AtomType[TorsionType[i].b].name[0];
	   if(TorsionType[i].j==-1) cj=X; else cj=&AtomType[TorsionType[i].j].name[0];

 	   fprintf(file,"TORSION   %3s %3s %3s %3s     %6.3f %6.1f %4.2f\n",
		 ci,ca,cb,cj,
		 TorsionType[i].Vn[h]*__J2kcal*__Na,
		 TorsionType[i].gamma[h]*180/pi,
		 TorsionType[i].n[h]);
   }
 }
 fprintf(file,"\n\n\n\n");


 fprintf(file,"#Now define amino acid atoms explicitly\n");
 fprintf(file,"#\n");
 fprintf(file,"#              atnam    x       y       z    type      Connectivity\n");

 for(i=0;i<60;i++){
    fprintf(file,"AMINOACID   %c   %s\n",
            aaparam[i].letter,
		   &aaparam[i].name[0]);

  for(j=0;j<aaparam[i].atoms;j++){
	if(aaparam[i].atom[j].valid == 0) continue; // ignore invalid atoms
    fprintf(file,"           ATOM %-4s  %6.3f  %6.3f  %6.3f %-10s ",
            &aaparam[i].atom[j].pdbname[0],
            aaparam[i].atom[j].x,
            aaparam[i].atom[j].y,
            aaparam[i].atom[j].z,
			&AtomType[aaparam[i].atom[j].type].name[0]);

	// new format
	for(k=0;k<aaparam[i].atom[j].r_covalent.n;k++){
	  if(aaparam[i].atom[aaparam[i].atom[j].r_covalent.i[k]].valid == 0) continue; // ignore invalid atoms

	  if(k!=0) fprintf(file,",");
      fprintf(file,"%s", &aaparam[i].atom[aaparam[i].atom[j].r_covalent.i[k]].pdbname[0] );
    }
  	fprintf(file,"\n");
  }

  for(j=0;j<aaparam[i].atoms;j++){
	// if charge of individual atom differs from atom type, print implicit charge
      if(aaparam[i].atom[j].valid == 0) continue; // ignore invalid atoms
	  if(aaparam[i].atom[j].charge != AtomType[aaparam[i].atom[j].type].p.charge){
        fprintf(file,"           CHARGE   %-4s  %8.5f \n",
            &aaparam[i].atom[j].pdbname[0],
            aaparam[i].atom[j].charge);
	  }
  }

  for(j=0;j<aaparam[i].atoms;j++){
	// if mass of individual atom differs from atom type, print implicit mass
      if(aaparam[i].atom[j].valid == 0) continue; // ignore invalid atoms
	  if(aaparam[i].atom[j].mass != AtomType[aaparam[i].atom[j].type].p.mass){
        fprintf(file,"           MASS   %-4s  %8.5f \n",
            &aaparam[i].atom[j].pdbname[0],
            aaparam[i].atom[j].mass / __amu);
	  }
  }

  fprintf(file,"ENDAMINOACID\n\n\n"); 
 }



 fprintf(file,"#Now define molecule types explicitly\n");
 fprintf(file,"#\n");
 fprintf(file,"#              atnam    x       y       z    type      Connectivity\n");


 for(i=0;i<nMolecules;i++){
    fprintf(file,"MOLECULE   %s\n",
		    &molecule[i].name[0]);

  for(j=0;j<molecule[i].atoms;j++){
    fprintf(file,"           ATOM %-4s  %6.3f  %6.3f  %6.3f %-10s ",
            &molecule[i].atom[j].pdbname[0],
            molecule[i].atom[j].x,
            molecule[i].atom[j].y,
            molecule[i].atom[j].z,
			&AtomType[molecule[i].atom[j].type].name[0]);

	for(k=0;k<molecule[i].atom[j].r_covalent.n;k++){
	  if(k!=0) fprintf(file,",");
      fprintf(file,"%s", &molecule[i].atom[molecule[i].atom[j].r_covalent.i[k]].pdbname[0] );
    }
  	fprintf(file,"\n");
  }

  for(j=0;j<molecule[i].atoms;j++){
	// if charge of individual atom differs from atom type, print implicit charge
      if(molecule[i].atom[j].valid == 0) continue; // ignore invalid atoms
	  if(molecule[i].atom[j].charge != AtomType[molecule[i].atom[j].type].p.charge){
        fprintf(file,"           CHARGE   %-4s  %8.5f \n",
            &molecule[i].atom[j].pdbname[0],
            molecule[i].atom[j].charge);
	  }
  }

  for(j=0;j<molecule[i].atoms;j++){
	// if charge of individual atom differs from atom type, print implicit charge
      if(molecule[i].atom[j].valid == 0) continue; // ignore invalid atoms
      if(molecule[i].atom[j].mass != AtomType[molecule[i].atom[j].type].p.mass){
        fprintf(file,"           MASS   %-4s  %8.5f \n",
            &molecule[i].atom[j].pdbname[0],
            molecule[i].atom[j].mass / __amu);
	  }
  }

  fprintf(file,"ENDMOLECULE\n\n\n"); 
 }




 fclose(file);

 return 0;
}


