/* ------------------------------------------------------------------------ */
/*  Copyright by Manfred J.Sippl             25/01/93                       */
/* ------------------------------------------------------------------------ */
/* ------------------------------------------------------------------------ */
/*			  superimpose example driver                        */
/* ------------------------------------------------------------------------ */

/* ------------------------------------------------------------------------
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
 ------------------------------------------------------------------------ */

#include <stdlib.h>
#include <stdio.h>
#include <math.h>

#include "coordfun.h"
#include "superimp.h"

#include <malloc.h>

main()
{
  CARRAY *o1=0, *o2=0, *o3=0;
  COORDS *c1, *c2, *c3;
  double  squared_rms;
  int i,j;

  printf("\n\n");
  printf("Several examples for the usage of the superpos-module\n");
  printf("=====================================================\n\n");
  /* read coordinates from files */
  o1 = coo_read_carray(o1,"c:\\2mhr.xyz");
  o2 = coo_read_carray(o2,"c:\\4pti.xyz");
  o3 = coo_read_carray(o3,"c:\\4pti.xyz");
  if (o1 == 0 || o2 == 0 || o3 == 0) exit(1);
  spp_set_precision(1.0e-8);   /* very high precision */

/* ------------------------------------------------------------------------ */
/* Example 1								    */
/* ------------------------------------------------------------------------ */
  printf("EXAMPLE 1\n");
  printf("---------\n");
  printf("Some rms values (Sippl & Stegbuchner Table 3):\n"); 
  c1 = o1->coo;
  c2 = o2->coo;
  squared_rms = spp_superperr_full(c1,c2,(int)5);
  printf("2mhr[1-10] vs 4pti[1-10]: %15.8f\n",sqrt(squared_rms));
  squared_rms = spp_superperr_full(c1,c2,(int)30);
  printf("2mhr[1-30] vs 4pti[1-30]: %15.8f\n",sqrt(squared_rms));
  squared_rms = spp_superperr_full(c1,c2,(int)58);
  printf("2mhr[1-58] vs 4pti[1-58]: %15.8f\n\n",sqrt(squared_rms));

/* ------------------------------------------------------------------------ */
/* Example 2								    */
/* ------------------------------------------------------------------------ */
  printf("EXAMPLE 2\n");
  printf("---------\n");
  printf("Example for two arbitrary fragments of length 20;\n");
  squared_rms = spp_superperr_full(c1+4,c2+12,(int)20);
  printf("2mhr[5-24] vs 4pti[13-32]: %15.8f\n\n",sqrt(squared_rms));

/* ------------------------------------------------------------------------ */
/* Example 3								    */
/* ------------------------------------------------------------------------ */
  printf("EXAMPLE 3\n");
  printf("---------\n");
  printf("Image vs mirror image of pti:");
  for (i=0;i<o3->n;i++) o3->coo[i].x = -o3->coo[i].x;
  c1 = o2->coo;
  c2 = o3->coo;
  squared_rms = spp_superperr_full(c1,c2,o2->n-1);
  printf(" %15.8f\n\n",sqrt(squared_rms));

/* ------------------------------------------------------------------------ */
/* Example 4								    */
/* ------------------------------------------------------------------------ */
  printf("EXAMPLE 4\n");
  printf("---------\n");
  printf("Comparison of fragments of proteins (small window)\n");
  printf("fragments 20-30 of 2mhr versus fragments 45-51 of 4pti.\n");
  printf("fragment size 8 residues.\n\n");

  printf("        ");
  for (i=44; i<50; i++) printf(" 4pti-%2d",i+1);
  printf("\n");
  for (j=19; j<29; j++) {
    printf("2mhr-%2d ",j+1);
    for (i=44; i<50; i++) {
	  squared_rms = spp_superperr_full(o1->coo+j,o2->coo+i,(int)8);
	  printf(" %7.2f",sqrt(squared_rms));
    }
    printf("\n");
  }
  printf("\n\n");


/* ------------------------------------------------------------------------ */
/* Example 5								    */
/* ------------------------------------------------------------------------ */
  printf("EXAMPLE 5\n");
  printf("---------\n");
  printf("Example for optimal superposition by matrix rotation\n");
  printf("Fragment 4pti-47 is superimposed on fragment 2mhr-20\n");
  printf("Fragment size 8 residues\n");
  /* copy o2 to o3 (coordinates of o3 will be changed by rotation) */
  for (i=0; i<o2->n; i++) {
    o3->coo[i].x = o2->coo[i].x;
    o3->coo[i].y = o2->coo[i].y;
    o3->coo[i].z = o2->coo[i].z;
  }
  /* define fragments */
  c1 = o1->coo+19;
  c2 = o2->coo+46;
  c3 = o3->coo+46;

  /* superimpose */
  printf("r.m.s error: %6.2f\n",sqrt(spp_superpose_full(c1,c3,8)));
  /* show original and rotated coordinates */
  printf("    2mhr (target)     |      4pti (rotated)   |     4pti (original)\n");
  for (i=0; i<8; i++) {
    printf("%6.2f %6.2f %6.2f  | ",c1[i].x,c1[i].y,c1[i].z);
    printf("%6.2f %6.2f %6.2f  | ",c3[i].x,c3[i].y,c3[i].z);
    printf("%6.2f %6.2f %6.2f\n",c2[i].x,c2[i].y,c2[i].z);
  }
  o1 = coo_free_carray(o1);
  o2 = coo_free_carray(o2);
  o3 = coo_free_carray(o3);
  return 0;
}
