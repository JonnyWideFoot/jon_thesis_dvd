/* ------------------------------------------------------------------------
               Macromolecular Surface Area Calculations
   ------------------------------------------------------------------------ */

#include <stdio.h>
#include <string.h>
#include <math.h>
#include <stdlib.h>
#include <stdarg.h>

#include "sasa.h"
#include "biolib.h"





// very plain straight forward distance matrix calculation
int calcDistanceMatrix(float *dmatrix, vector *atom, int atoms){
 int i,j;
 float x1,x2,y1,y2,z1,z2;
 float distij;
 
 for(i=0;i<atoms;i++){
  for(j=i+1;j<atoms;j++){  
           distij=sqrt( sqr((atom[i].x - atom[j].x)) +
                        sqr((atom[i].y - atom[j].y)) +
                        sqr((atom[i].z - atom[j].z)) );

           dmatrix[sqrmat(i,j,atoms)] = distij; 
           dmatrix[sqrmat(j,i,atoms)] = distij;  
  }
  dmatrix[sqrmat(i,i,atoms)] = 0;
 }

 return 0;
}

// very plain straight forward distance matrix calculation
int calcDistanceMatrix(float *dmatrix, vector *atom, int atoms, float cutoff){
 int i,j;
 float x1,x2,y1,y2,z1,z2;
 float distij, sqrdistij;
 float sqrcutoff = sqr(cutoff);
 
 for(i=0;i<atoms;i++){
 
  for(j=i+1;j<atoms;j++){  
       
           sqrdistij =  sqr((atom[i].x - atom[j].x)) +
                        sqr((atom[i].y - atom[j].y)) +
                        sqr((atom[i].z - atom[j].z)) ;
           if(sqrdistij < sqrcutoff){
             distij = sqrt(sqrdistij);
             dmatrix[sqrmat(i,j,atoms)] = distij; 
             dmatrix[sqrmat(j,i,atoms)] = distij;  
           }else{
             dmatrix[sqrmat(i,j,atoms)] = cutoff; // these are invalid
             dmatrix[sqrmat(j,i,atoms)] = cutoff; // these are invalid          
           }
  }
  dmatrix[sqrmat(i,i,atoms)] = 0;
 }

 return 0;
}





void printSphFile(int number){
 vector *stdpoint;
 
 stdpoint = new vector[number];
 
 createEvenVertexSphere(&stdpoint[0],1,number);
 for(int i=0;i<number;i++)
   printf("% 6.4f  % 6.4f  % 6.4f \n",
         stdpoint[i].x,
         stdpoint[i].y,
         stdpoint[i].z);


 delete [] stdpoint;
}


void createVertexSphere(vector *vertex,float radius, int number){
 int i;
 float x,y,z,t,r;

 for(i=0;i<number;i++){
  z = ((float)(rand()%100000)-50000)/50000;
  t = ((float)(rand()%100000)/50000) * _2pi;

  r = sqrt(1 - sqr(z));
  x = r * cos(t);
  y = r * sin(t);

  vertex[i].setTo(x*radius,y*radius,z*radius);  
  vertex[i].magnitude = 1;
 }

}

void createEvenVertexSphere(vector *vertex,float radius, int number){
 int i;
 float x,y,z,t,r;
 vector *nvert = new vector [number*10]; // reserve space for 10 times more

 for(i=0;i<(number*10);i++){
  z = ((float)(rand()%100000)-50000)/50000;
  t = ((float)(rand()%100000)/50000) * _2pi;

  r = sqrt(1 - sqr(z));
  x = r * cos(t);
  y = r * sin(t);

  nvert[i].setTo(x*radius,y*radius,z*radius);  
  nvert[i].magnitude = 1;
 }

 float ilimit= 2 * r / sqrt((float)number);
 float limit = ilimit*0.9;

int nvertex = number*10;
 int done=1,j,k;

 printf("ilimit is %f\n",ilimit);


 for(i=0;i<100;i++){
 
  for(j=0;j<(number * 10);j++){
   if(nvert[j].magnitude < 0) continue;
   for(k=0;k<(number * 10);k++){
    if(nvert[k].magnitude < 0) continue;
    if(k==j) continue;
    if(nvert[j].distanceTo(&nvert[k]) < limit){
     nvert[j].magnitude = -1;  // mark vertex invalid
     nvertex--; // decrease number of vertexes left by 1
     break;
    }
    if(nvertex<=number){ done = 0; break; }
   }
   if(nvertex<=number){ done = 0; break; }
  }
  printf("REMARK limit %f nvertex %d \n",limit,nvertex);
  if(done==0) break;
  limit *= 1.221;
 }


 // now put points back into original array

 int pos=0;
 for(i=0;i<number*10;i++){
  if(nvert[i].magnitude > 0){ vertex[pos].setTo(&nvert[i]);
 
   printf("% 6.4f  % 6.4f  % 6.4f \n",
         vertex[pos].x,
         vertex[pos].y,
         vertex[pos].z);
  pos++;
  if(pos>=number) break;
  }
 }
 
 delete [] nvert;
}

int loadXYZcoords(char *filename, vector *point, int number){
 FILE *file;
 int i;
 char buffer[100];
 float x,y,z;

 file = fopen(filename,"r");
 if(file==NULL){
    printf("ERROR - file not found \n");
	return -1;
 }

 for(i=0;(i<number)&&(!feof(file));i++){
  fgets(&buffer[0],100,file);
  sscanf(&buffer[0],"%f %f %f",&x,&y,&z);
  point[i].setTo(x,y,z);
 }
 printf("Read %d coordinates from %s \n",i,filename);
 fclose(file);
 return 0;
}


float  calcNumericalSASA(vector   *atom,
                         float    *radius,
                         int      atoms,
                         float    probeRadius, // standard point file
                         char    *SPfile, 
                         int       SurfacePoints,       // unit sphere surface point file
                         float    *SASA){

  vector *stdpoint;
  vector point;
  float  *dmatrix;
  int    validpoints,valid;
  float  distjp;
  int    i,j,p;
  float  SAi;
  float  SASAtot=0;

  stdpoint = new vector[SurfacePoints];
  dmatrix  = new float[sqr(atoms)];

  if(SPfile == NULL)  
    createVertexSphere(&stdpoint[0],1,SurfacePoints);
  else
    loadXYZcoords(SPfile, &stdpoint[0], SurfacePoints); // load coords
  
  calcDistanceMatrix(&dmatrix[0],atom, atoms);

  for(i=0;i<atoms;i++){
   if(radius[i] <= 0.0001){;
     validpoints=0;
   }else{

   validpoints = SurfacePoints; 
   for(p=0;p<SurfacePoints;p++){
    point = stdpoint[p];  // get point
    point.mul((radius[i]+probeRadius)); // multiply by relevant radius + proberadius
    point.add(&atom[i]);  // move to coordinate of respective atom

    valid = 1;
    for(j=0;j<atoms;j++){
     // check if atom is out of reach anyway
     if(j==i) continue;
	 if(radius[j] <= 0.0001) continue;
     if(dmatrix[sqrmat(i,j,atoms)] > (radius[i] + radius[j] + 2 * probeRadius)) continue; 
     // if not, see if it occludes the point of interest
     distjp = atom[j].distanceTo(&point);
     if(distjp < (radius[j] + probeRadius)) {
       valid = 0;
       break;
     }
	}
    if(valid != 1) validpoints--;
   }

   }

   printf("SASA %d  %f\n",i,((float)validpoints/(float)SurfacePoints) );

   float totalSA = sphereSurfaceArea(radius[i]+probeRadius);
   SAi = ((float)validpoints/(float)SurfacePoints) * 
           totalSA ;
   SASA[i] = SAi;
   SASAtot += SAi;
   //printf("Atom %d: Surface Area: %f %c %f\n",i,SAi,'/', totalSA);
  }

  delete [] stdpoint;
  delete [] dmatrix;
  return  SASAtot;
}





// THis function is required for numerical FDPB solution to find the
// effective born radius of an atom
float  calcNumericalSASAShell(vector   *atom,                // atom vectors
                              float    *radius,              // radii
                              int       atoms,               // number of atoms
							  float    *dmatrix,
                              int       shellatom,           // atom number to be shelled
                              float     shellradius,         // radius to be calculated
                              vector    *stdpoint, 
                              int       SurfacePoints){     // unit sphere surface point file
                       

  vector point;
  int    validpoints,valid;
  float  distjp;
  int    j,p;
  float  SAi;
  float  SASAtot=0;

  float  savradius;

  // all this is done once outside this function to enhance speed
  //stdpoint = new vector[SurfacePoints];
  //dmatrix  = new float[sqr(atoms)];
/*
  if(SPfile == NULL)  
    createVertexSphere(&stdpoint[0],1,SurfacePoints);
  else
    loadXYZcoords(SPfile, &stdpoint[0], SurfacePoints); // load coords
  */
 // calcDistanceMatrix(&dmatrix[0],atom, atoms);

   validpoints = SurfacePoints; 
   for(p=0;p<SurfacePoints;p++){
    point = stdpoint[p];  // get point
    point.mul(shellradius); // multiply by relevant radius + proberadius
    point.add(&atom[shellatom]);  // move to coordinate of respective atom

    valid = 1;
    for(j=0;j<atoms;j++){
     // check if atom is out of reach anyway
     if(j==shellatom) continue;  // no self-occlusion
	 if(dmatrix[sqrmat(shellatom,j,atoms)] > (shellradius + radius[j] + 0.05)) continue; 
     if(dmatrix[sqrmat(shellatom,j,atoms)] < (shellradius - radius[j] - 0.05)) continue; 

     // if not, see if it occludes the point of interest
     distjp = atom[j].distanceTo(&point);
     if(distjp < (radius[j])) {
       valid = 0;
       break;
     }
    }
    if(valid != 1) validpoints--;
   }

   SASAtot = ((float)validpoints/(float)SurfacePoints);
            
 
   return  SASAtot;
}


















/* Approximate atomic vdw/SASA surface area calculation according to
   Wodak, Shenkin and Still (LCPO) (1990)
*/




#define calcBdash(r1,r2,d) (pi * ((r1) + (rw)) * ((r1) + (r2) + 2*rw - (s) - (d))) * (1 + ((r2) - (s) - (r1))/(d))
#define     calcB(r1,r2,d) (pi * ((r1) + (rw)) * ((r1) + (r2) + 2*rw -       (d))) * (1 + ((r2) -       (r1))/(d))


#define Nlistmax 200
typedef struct _Nlist_member{
  unsigned int size;
  unsigned int i[Nlistmax];
} Nlist_member;

float LCPOcalcTotalSASA(vector  *atom,
                      float     *radius,
					  SASAparam *sasaparam,
                      int       atoms,
                      float     probeRadius,
                      float     *SASA){

 unsigned int i,j,k,ia,ja,ka;
 float SAtot=0,SAi, Si, Si2,Si3,Si4,Si2_t,Si3_t;
 unsigned int   count=0;
 float P1,P2,P3,P4;

 P1 = 1;
 P2 = 1;
 P3 = 0;
 P4 = 0;


 float *dmatrix = new float[sqr(atoms)];
 Nlist_member  *Nlist = new Nlist_member[atoms];
 
 if(Nlist == NULL) return -1;

 calcDistanceMatrix(&dmatrix[0],atom, atoms);

  // create neighbour lists
 count=0;
 for(i=0;i<atoms;i++){
  Nlist[i].size = 0;
  if(sasaparam[i].use==0) continue;
  
  for(j=0;j<atoms;j++){
	if(sasaparam[j].use==0) continue;
    if(dmatrix[sqrmat(i,j,atoms)] < (radius[i]+radius[j]+probeRadius*2)){
     if(i!=j){
      Nlist[i].i[Nlist[i].size] = j;
      Nlist[i].size++;
      if(Nlist[i].size > Nlistmax) printf("AAAAAAAAA");
     }
    }
    count++;
  }
 }

 // printf("Total neighbor list CRC %d \n",totn);

 /*
 // rpint n list
 for(i=0;i<atoms;i++){
  printf("%d: ",i);
  for(j=0;j<Nlist[i].size;j++) printf(" %d",Nlist[i].i[j]); 
  printf("\n");
 }
*/

   
 for(i=0;i<atoms;i++){
	 if(sasaparam[i].use==0){
         SASA[i] = 0;
		 continue; // ignore radius 0 atoms
	 }

  ia = i;
  Si = sphereSurfaceArea(radius[ia]+probeRadius);
 
  Si2 = 0;
  Si3 = 0;
  Si4 = 0;
  
  for(j=0;j<Nlist[i].size;j++){
   ja = Nlist[ia].i[j];           // get atom number
   if(sasaparam[ja].use==0) continue;
 
   Si2_t = sphereSurfaceOverlap(radius[ia]+probeRadius,radius[ja]+probeRadius,dmatrix[sqrmat(ia,ja,atoms)]);
   Si3_t = 0;
   for(k=0;k<Nlist[ja].size;k++){
    ka = Nlist[ja].i[k];
	if(sasaparam[ka].use==0) continue;
    
	//check if those two (ka and ja) actually overlap
    //if( (radius[ja] + radius[ka] + 2*probeRadius) < dmatrix[sqrmat(ja,ka,atoms)]) continue; 
    if( (radius[ia] + radius[ka] + 2*probeRadius) < dmatrix[sqrmat(ia,ka,atoms)]) continue; 
   
    Si3_t += sphereSurfaceOverlap(radius[ja]+probeRadius,radius[ka]+probeRadius,dmatrix[sqrmat(ja,ka,atoms)]);
   }

   Si4 += (Si3_t * Si2_t);
   Si3 += Si3_t;
   Si2 += Si2_t;
  } 


  SAi = sasaparam[i].P1*Si + 
	    sasaparam[i].P2*Si2 + 
		sasaparam[i].P3*Si3 + 
		sasaparam[i].P4*Si4;

  if(SAi < 0) SAi = 0; 
//  printf("SASA = %f  -- Si %6.2f Si2 %6.2f Si3 %6.2f Si4 %6.2f \n",SAi,Si,Si2,Si3,Si4);

  SASA[i] = SAi;
  SAtot += SAi;
 }

// printf("SASA total = %f A^2 \n",  SAtot );

  

  delete [] dmatrix;
  delete [] Nlist;

  

  return SAtot;
}
                      



