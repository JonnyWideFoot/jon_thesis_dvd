//# maths.h
//# header to maths.cpp

#ifndef __MATHS_H
#define __MATHS_H

#include <math.h>

extern int _3dXoff;
extern int _3dYoff;

extern int _3d_xtrans;
extern int _3d_ytrans;
extern int _3d_ztrans;

extern int _3d_yrot;
extern int _3d_xrot;

extern signed int _sin[];           // sin & cos tables;
extern signed int _cos[];

#define VERSION 0.0.7

#define _infinity 10000000000       // just pretty big
#define pi   ((double)3.1415926535897932385)
#define _2pi ((double)6.283185307)
#define _pid180 ((double)0.017453292)  // pi / 180 (deg/rad conv)

#define TRIG_RESOLUTION 4096
#define TRIG_COEFF      1024
#define COORD_COEFF     1024

#ifdef sqr
 #undef sqr
#endif
#define sqr(x) ((x)*(x))


#define sphereSurfaceArea(r) (4*pi*sqr((r)))
#define sphereSurfaceOverlap(ri,rj,dij) (2*pi*(ri) * ((ri) - (dij)/2 - ((ri)*(ri) - (rj)*(rj))/(2*dij)))


int initmaths();            // load precalculated tables
long factorial(long i);

class rmatrix;
class vector;
class plane;

class rmatrix{
 public:
    float r[3][3];  // full matrix for comp. but could red to 3x3  
                    // for speed issues

    void setTo(rmatrix *mat);
    void setTo(float r00, float r01, float r02,
               float r10, float r11, float r12,
               float r20, float r21, float r22);
    void setToIdentity();
    void setToXrot(float angle);
    void setToYrot(float angle);
    void setToZrot(float angle);
    void setToAxisRot(vector *axis, float angle);
    void mul(rmatrix *mmat);
};

class vector                // Mother class, the vector, basal to everything
{
 public:
    float x,y,z,magnitude;                      // vector parameters (x,y,z) & magnitude
	                                            // of the vector
          vector();
          vector(float,float,float);            // basic constructor
		  vector(vector *v1);                   // copy constructor
		  ~vector(){};                          // destructor (just for the sake of completeness
    int   print();                              // -- temporary print routine
	float mag()                                 // computes & returns magnitude
	  {magnitude = (float)sqrt(x*x+y*y+z*z);
	   return magnitude;}

	float scalarProduct(vector *v2);            // calculates scalar product
    void vectorProduct(vector *v2, vector *product);    // does the same for vector product
	float angleWith(vector *v2);                // outputs an acute angle (rad)
    void add(vector *v2){x+=v2->x;y+=v2->y;z+=v2->z;};
	void sub(vector *v2){x-=v2->x;y-=v2->y;z-=v2->z;};

	void diff(vector *v1,vector *v2){x = v1->x - v2->x; 
								     y = v1->y - v2->y;
								     z = v1->z - v2->z;};
    void  sum(vector *v1,vector *v2){x = v1->x + v2->x; 
								     y = v1->y + v2->y;
								     z = v1->z + v2->z;};
	void crossProduct(vector *v1,vector *v2);
    void crossProduct(vector *v1, vector *v2, float scalar);

	void mul(float f){x*=f;y*=f;z*=f;};
	void div(float f){x/=f;y/=f;z/=f;};

    void mulmat(rmatrix *mmat);

    void setTo(vector *v2){x=v2->x;y=v2->y;z=v2->z;};
    void setTo(float _x,float _y,float _z){x=_x;y=_y;z=_z;};
    void inv(){x=-x;y=-y;z=-z;};
    float distanceTo(vector *v2);
    //APPLY TO **DIRECTION** VECTORS ONLY!!!!!--------------
    void unify(){if(mag()!=0){x/=magnitude;y/=magnitude;z/=magnitude;magnitude=1;}};
    //--------------------------------------------------
    void rotateX(float angle);
    void rotateY(float angle);
    void rotateZ(float angle);
    void rotateAxis(float angle,vector *offset,vector *axis);
    void rotateYat(float angle, vector *rotCenter);

};

float dotProduct(vector *v1,vector *v2); 


class plane{
 public:
  vector *a,*n;                // position & normal vector


  plane();                     // Basic constructor
  plane(vector *_a, vector *_n); // Simple copy constructor
  plane(vector *_a, vector *_v1, vector *_v2);  // A bit more subtle ? No just the same in blue.
  ~plane();                    // Destructor

  float distanceTo(vector *point);
  int   intersectLine(vector *la,vector *ld,vector *intersect);

};

class point{
  public:
   int x,y;
   point(){x=0;y=0;}                 // constructors
   point(point *p2){setTo(p2);}      // copy constructors
   point(int x2,int y2){x=x2;y=y2;}
   ~point(){}                        //nop

   void draw(unsigned char color);    //displaying information
   void drawTo(point *p2,unsigned char color);
   void setTo(point *p2){x=p2->x;y=p2->x;}
   void setTo(int x2,int y2){x=x2;y=y2;}
   int fromVector(vector *v2);
};

class face{
public:
 vector v[3];
 vector n;
 float d;

 face();
 face(float x0,float y0,float z0,
      float x1,float y1,float z1,
      float x2,float y2,float z2);
 void setTo(float x0,float y0,float z0,
      float x1,float y1,float z1,
      float x2,float y2,float z2);
 void face::draw(char color);
 void face::drawSh(char color);

float circumCircle(vector *center);  // returns radius&center of circumCircle
float intersectVector(vector *a, vector *u);

};

int line2lineDistance(vector *p1,vector *d1,
		      vector *p2,vector *d2,
		      float *dist,vector *ap1,vector *ap2);

void drawPointer(vector *a,vector *d, float length, char color);
int test4PathCollision(vector *a, vector *b, vector *p, float minDist);
int test4PathCollision(vector *a, vector *b, float ab, vector *p, float minDist);


#endif
