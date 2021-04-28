// This is the main project file for VC++ application project 
// generated using an Application Wizard.

#include "stdafx.h"
#include "maths.h"
#include "sasa.h"
#include "stdlib.h"
#include <iostream>
#include <fstream>
using namespace std;




int main(int argc, char **argv)
{
	vector* atom;

	//atom[0].setTo(0.0f,0.0f,0.0f);
	//atom[1].setTo(3.74f,0.0f,0.0f);
	////atom[2].setTo(1.5f,1.5f,1.5f);

	//radius[0] = 1.87f;
	//radius[1] = 1.87f;
	////radius[2] = 1.87f;



	//ATOM   3055  CA  VAL H 181       1.703 -18.387  19.496  1.00 12.47           C  
	//ATOM   3058  CB  VAL H 181       0.178 -18.617  19.366  1.00 12.68           C  
	//ATOM   3059  CG1 VAL H 181      -0.473 -18.555  20.734  1.00 11.28           C  
	//ATOM   3060  CG2 VAL H 181      -0.434 -17.578  18.430  1.00 13.48           C  
	//atom = new vector [4];
 //   float radius[4];
 //   float sasa[4];
	//int nAtoms = 4;
	//atom[0].setTo(1.703f,-18.387f,19.496f);
	//atom[1].setTo(0.178f,-18.617f,19.366f);
	//atom[2].setTo(-0.473f,-18.555f,20.734f);
	//atom[3].setTo(-0.434f,-17.578f,18.430f);
	//radius[0] = 1.87f;
	//radius[1] = 1.87f;
	//radius[2] = 1.87f;
	//radius[3] = 1.87f;


	/*ATOM   3201  N   LYS H 201      13.616  -7.018  29.993  1.00 20.75           N  
	ATOM   3202  CA  LYS H 201      14.731  -6.163  30.372  1.00 24.76           C  
	ATOM   3203  C   LYS H 201      14.572  -5.444  31.710  1.00 24.83           C  
	ATOM   3204  O   LYS H 201      15.511  -5.405  32.508  1.00 24.80           O  
	ATOM   3205  CB  LYS H 201      15.001  -5.149  29.257  1.00 27.71           C  
	ATOM   3206  CG  LYS H 201      15.355  -5.790  27.924  1.00 31.07           C  
	ATOM   3207  CD  LYS H 201      15.613  -4.741  26.852  1.00 33.73           C  
	ATOM   3208  CE  LYS H 201      16.012  -5.387  25.536  1.00 35.88           C  
	ATOM   3209  NZ  LYS H 201      16.284  -4.369  24.478  1.00 37.27           N  */
	int nAtoms = 9;
	atom = new vector [9];
    float radius[9];
    float sasa[9];
	atom[0].setTo(13.616,  -7.018,  29.993);
	atom[1].setTo(14.731,  -6.163,  30.372);
	atom[2].setTo(14.572,  -5.444,  31.710);
	atom[3].setTo(15.511,  -5.405,  32.508);
	atom[4].setTo(15.001,  -5.149,  29.257);
	atom[5].setTo(15.355,  -5.790,  27.924);
	atom[6].setTo(15.613,  -4.741,  26.852);
	atom[7].setTo(16.012,  -5.387,  25.536);
	atom[8].setTo(16.284,  -4.369,  24.478);
	radius[0] = 1.65f;//N
	radius[1] = 1.87f;//CA
	radius[2] = 1.76f;//C
	radius[3] = 1.40f;//O
	radius[4] = 1.87f;//CB
	radius[5] = 1.87f;//CG
	radius[6] = 1.87f;//CD
	radius[7] = 1.87f;//CE
	radius[8] = 1.50f;//NZ
	printf("%f \n", atom[0].distanceTo( &atom[1]) );
	printf("%f \n", atom[1].distanceTo( &atom[4]) );
	printf("%f \n", atom[4].distanceTo( &atom[5]) );
	printf("%f \n", atom[5].distanceTo( &atom[6]) );
	printf("%f \n", atom[6].distanceTo( &atom[7]) );
	printf("%f \n", atom[7].distanceTo( &atom[8]) );

			float totalsasa = calcNumericalSASA(atom,&radius[0],nAtoms,
									1.4,"lib\\sph1000.dat",1000,&sasa[0]);

	// FILE *file;

	// file = fopen("c:\\solv.csv","w");

	//for ( float i = 0.0; i < 10; i += 0.385f)
	//{
	//	printf("\nCalc for i = %f \n", i);

	//	int nAtoms = 2;
	//	atom = new vector [2];
	//	float radius[2];
	//	float sasa[2];

	//	atom[0].setTo(0.0f, 0.0f, 0.0f);
	//	atom[1].setTo(0.0f, 0.0f, i   );
	//	radius[0] = 1.87f;
	//	radius[1] = 1.87f;

	//	float totalsasa = calcNumericalSASA(atom,&radius[0],nAtoms,
	//								0,"lib\\sph1000.dat",1000,&sasa[0]);

	//	fprintf(file,"%f, %f, %f, %f, %f \n", 
	//		i, 
	//		sasa[0] / sphereSurfaceArea(1.87f),
	//		sasa[1] / sphereSurfaceArea(1.87f),
	//		sasa[0],
	//		sasa[1]
	//	);

	//}

	//fclose(file);

	//for ( int i = 0; i < nAtoms; i++ )
	//{
	//	printf("%f \n", sasa[i]);
	//}





	//int nAtoms = 7;
	//atom = new vector [7];
 //   float radius[7];
 //   float sasa[7];
	//atom[0].setTo(58.427,  -0.009,  -3.695);
	//atom[1].setTo(59.351,  -1.117,  -3.285);
	//atom[2].setTo(59.714,  -2.107,  -4.191);
	//atom[3].setTo(59.867,  -1.201,  -2.015);
	//atom[4].setTo(60.562,  -3.131,  -3.820);
	//atom[5].setTo(60.717,  -2.216,  -1.628);
	//atom[6].setTo(61.066,  -3.183,  -2.533);

	//radius[0] = 1.87f; //CB
	//radius[1] = 1.76f; //CG
	//radius[2] = 1.76f; //CD1
	//radius[3] = 1.76f; //CD2
	//radius[4] = 1.76f; //CE1
	//radius[5] = 1.76f; //CE2
	//radius[6] = 1.76f; //CZ

	//	float totalsasa = calcNumericalSASA(atom,&radius[0],nAtoms,
	//								0,"lib\\sph1000.dat",1000,&sasa[0]);


	int bla = 0;
	cin >> bla; // used to pause program

	delete [] atom;
	return 0;
}

