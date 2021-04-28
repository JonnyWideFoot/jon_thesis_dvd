//# maths.cpp
//# all the maths stuff
#include <math.h>
#include <stdio.h>
#include <string.h>
#include "maths.h"

#define __NOGRAPHICS
//# Class vector ------------------------------------------------<

int _3dXoff;              // Screen offsets
int _3dYoff;

int _3d_xtrans;           // Translational constants
int _3d_ytrans;
int _3d_ztrans;

int _3d_yrot;             // rotational constants
int _3d_xrot;

void rmatrix::setTo(rmatrix *mat){
 memcpy((void*)this->r,(void*)mat->r,sizeof(rmatrix));
}

void rmatrix::setToIdentity(){
 r[0][0] = 1;
 r[0][1] = 0;
 r[0][2] = 0;
 r[1][0] = 0;
 r[1][1] = 1;
 r[1][2] = 0;
 r[2][0] = 0;
 r[2][1] = 0;
 r[2][2] = 1;
}
void rmatrix::setTo(float r00, float r01, float r02,
	           float r10, float r11, float r12,
        	   float r20, float r21, float r22){
 r[0][0] = r00;
 r[0][1] = r01;
 r[0][2] = r02;

 r[1][0] = r10;
 r[1][1] = r11;
 r[1][2] = r12;

 r[2][0] = r20;
 r[2][1] = r21;
 r[2][2] = r22;
}

void rmatrix::setToXrot(float angle){
 float c = cos(angle);
 float s = sin(angle);

 r[0][0] = 1;
 r[0][1] = 0;
 r[0][2] = 0;

 r[1][0] = 0;
 r[1][1] = c;
 r[1][2] = -s;

 r[2][0] = 0;
 r[2][1] = s;
 r[2][2] = c;
}

void rmatrix::setToYrot(float angle){
 float c = cos(angle);
 float s = sin(angle);

 r[0][0] = c;
 r[0][1] = 0;
 r[0][2] = s;

 r[1][0] = 0;
 r[1][1] = 1;
 r[1][2] = 0;

 r[2][0] = -s;
 r[2][1] = 0;
 r[2][2] = c;
}

void rmatrix::setToZrot(float angle){
 float c = cos(angle);
 float s = sin(angle);

 r[0][0] = c;                   ///AARRRRG optimise double sin/cos !!!
 r[0][1] = -s;
 r[0][2] = 0;
 
 r[1][0] = s;
 r[1][1] = c;
 r[1][2] = 0;
 
 r[2][0] = 0;
 r[2][1] = 0;
 r[2][2] = 1;
}
 
void rmatrix::setToAxisRot(vector *axis, float angle){

 axis->unify();

 float c = cos(angle);         
 float s = sin(angle);
 float t = 1 - c;
 float tx = t*axis->x;

 float sx = s*axis->x;
 float sy = s*axis->y;
 float sz = s*axis->z;
 float tyz = t*axis->y*axis->z;

 r[0][0] = tx*axis->x + c;                
 r[0][1] = tx*axis->y - sz;
 r[0][2] = tx*axis->z + sy;

 r[1][0] = tx*axis->y + sz;
 r[1][1] = t*sqr(axis->y) + c;
 r[1][2] = tyz - sx;

 r[2][0] = tx*axis->z - sy;
 r[2][1] = tyz + sx;
 r[2][2] = t*sqr(axis->z) + c;

/*
 r[0][0] = t*sqr(axis->x) + c;                /// here's some optimisation possilbe
 r[0][1] = t*axis->x*axis->y - s*axis->z;
 r[0][2] = t*axis->x*axis->z + s*axis->y;

 r[1][0] = t*axis->x*axis->y + s*axis->z;
 r[1][1] = t*sqr(axis->y) + c;
 r[1][2] = t*axis->y*axis->z - s*axis->x;

 r[2][0] = t*axis->x*axis->z - s*axis->y;
 r[2][1] = t*axis->y*axis->z + s*axis->x;;
 r[2][2] = t*sqr(axis->z) + c;

 r[3][0] = 0;
 r[3][1] = 0;
 r[3][2] = 0;
*/
}

void rmatrix::mul(rmatrix *mmat){
 int i,j,k;
 rmatrix re;

 re.r[0][0] = r[0][0] * mmat->r[0][0] + 
              r[0][1] * mmat->r[1][0] + 
              r[0][2] * mmat->r[2][0];
 re.r[1][0] = r[1][0] * mmat->r[0][0] + 
              r[1][1] * mmat->r[1][0] + 
              r[1][2] * mmat->r[2][0];
 re.r[2][0] = r[2][0] * mmat->r[0][0] + 
              r[2][1] * mmat->r[1][0] + 
              r[2][2] * mmat->r[2][0];
 
 re.r[0][1] = r[0][0] * mmat->r[0][1] + 
              r[0][1] * mmat->r[1][1] + 
              r[0][2] * mmat->r[2][1];
 re.r[1][1] = r[1][0] * mmat->r[0][1] + 
              r[1][1] * mmat->r[1][1] + 
              r[1][2] * mmat->r[2][1];
 re.r[2][1] = r[2][0] * mmat->r[0][1] + 
              r[2][1] * mmat->r[1][1] + 
              r[2][2] * mmat->r[2][1];

 re.r[0][2] = r[0][0] * mmat->r[0][2] + 
              r[0][1] * mmat->r[1][2] + 
              r[0][2] * mmat->r[2][2];
 re.r[1][2] = r[1][0] * mmat->r[0][2] + 
              r[1][1] * mmat->r[1][2] + 
              r[1][2] * mmat->r[2][2];
 re.r[2][2] = r[2][0] * mmat->r[0][2] + 
              r[2][1] * mmat->r[1][2] + 
              r[2][2] * mmat->r[2][2];
/* 
for(i=0;i<3;i++){
  for(j=0;j<3;j++){
   re.r[i][j] = 0;
   for(k=0;k<3;k++) re.r[i][j] += r[i][k] * mmat->r[k][j];
 }}

*/
 this->setTo(&re);
}

signed int _sin[TRIG_RESOLUTION];           // sin & cos tables;
signed int _cos[TRIG_RESOLUTION];

int initmaths(){          // load precalculated tables
  for(int i=0;i<TRIG_RESOLUTION;i++){
   _sin[i] = (int)((float)sin((float)i * _2pi / TRIG_RESOLUTION)*TRIG_COEFF);
   _cos[i] = (int)((float)cos((float)i * _2pi / TRIG_RESOLUTION)*TRIG_COEFF);
  }

  _3dXoff=0;
  _3dYoff=0;

  _3d_yrot=0;
  _3d_xrot=0;

  return 0;
}


long factorial(long i){
 long result=1;
 int a;
 for(a=2;a<=i;i++) result*=a;
 return result;
}


int vector::print(){
 printf("x:% 7.3f y:% 7.3f z:% 7.3f mag:%7.3f\n",x,y,z,magnitude);
 return 0;
};

vector::vector(float px,float py,float pz)
{x=px;y=py;z=pz;
 mag();
}

vector::vector()
{x=0;y=0;z=0;
 mag();
}

vector::vector(vector *v1)
{x=v1->x;y=v1->y;z=v1->z;
 mag();
}

float vector::scalarProduct(vector *v2){
 return  (x*v2->x + y*v2->y + z*v2->z);             // to simple to comment
}

float dotProduct(vector *v1,vector *v2){
 return  (v1->x*v2->x + v1->y*v2->y + v1->z*v2->z);             // to simple to comment
}

void vector::crossProduct(vector *v1, vector *v2){
 x = (v1->y*v2->z - v1->z*v2->y);
 y = (v1->z*v2->x - v1->x*v2->z);      
 z = (v1->x*v2->y - v1->y*v2->x);      
}

void vector::crossProduct(vector *v1, vector *v2, float scalar){
 x = (v1->y*v2->z - v1->z*v2->y) * scalar;
 y = (v1->z*v2->x - v1->x*v2->z) * scalar;      
 z = (v1->x*v2->y - v1->y*v2->x) * scalar;      
}


void vector::vectorProduct(vector *v2, vector *product){
 product->x = (y*v2->z - z*v2->y);      // finding the vector product
 product->y = (z*v2->x - x*v2->z);      // the quotient makes it a unit vector
 product->z = (x*v2->y - y*v2->x);      // so might be taken away for economy reasons
}

float vector::angleWith(vector *v2){
 float scalar = scalarProduct(v2) / (mag() * v2->mag());  //simple scalar product calculation
 if(scalar>1)return 0;
 if(scalar<-1)return pi;
 return acos(scalar);
}

float vector::distanceTo(vector *v2){
/*
 vector temp;
 temp.setTo(v2);
 temp.sub(this);
 return temp.mag();
*/
// optimize: x1.9
 float g=(v2->x-x);
 float h=(v2->y-y);
 float j=(v2->z-z);
 float temp = sqrt(g*g+h*h+j*j);
 return temp;
}

void vector::mulmat(rmatrix *mmat){
 float nx,ny,nz;
 nx = mmat->r[0][0]*x + mmat->r[0][1]*y + mmat->r[0][2]*z;
 ny = mmat->r[1][0]*x + mmat->r[1][1]*y + mmat->r[1][2]*z;
 nz = mmat->r[2][0]*x + mmat->r[2][1]*y + mmat->r[2][2]*z;
 x = nx;
 y = ny;
 z = nz;
}

void vector::rotateY(float angle){
 float cangle = cos(angle);
 float sangle = sin(angle);
 float tx = x*cangle + z*sangle;
 float tz =-x*sangle + z*cangle;
 x = tx;
 z = tz;
}

void vector::rotateX(float angle){
 float cangle = cos(angle);
 float sangle = sin(angle);
 float ty = y*cangle + z*sangle;
 float tz =-y*sangle + z*cangle;
 y = ty;
 z = tz;
}

void vector::rotateZ(float angle){
 float cangle = cos(angle);
 float sangle = sin(angle);
 float tx = x*cangle + y*sangle;
 float ty =-x*sangle + y*cangle;
 x = tx;
 y = ty;
}


void vector::rotateAxis(float angle,vector *offset,vector *aaxis){
 sub(offset);                              //translate so offset becomes 0

 vector axis(aaxis);                       //make a copy of the axis

 vector xaxis(1,0,0);                      //create an xaxis

 vector xzplane(axis); xzplane.y = 0;      //obtain angle in the xzplane
 float xzangle = xaxis.angleWith(&xzplane);
 if(xzplane.z<0)xzangle*=-1;

      rotateY(xzangle);                    //rotate the point and the axis
 axis.rotateY(xzangle);

 vector xyplane(axis); xyplane.z = 0;      //repeat with xyplane
 float xyangle = xaxis.angleWith(&xyplane);
 if(xyplane.y<0)xyangle*=-1;

 rotateZ(xyangle);

 rotateX(angle);                           //now rotate round axis (X)
 rotateZ(-xyangle);                        //rotate back in backwards direction
 rotateY(-xzangle);                        //

 add(offset);                              //translate back

// printf("%lf %lf %lf\n",xzangle,xyangle,angle);
}



void vector::rotateYat(float angle, vector *rotCenter){
 sub(rotCenter);

 float tx = x*cos(angle) + z*sin(angle);
 float tz =-x*sin(angle) + z*cos(angle);

 x = tx;
 z = tz;

 add(rotCenter);
}

//---< Plane >--------------------------------------------------------------<<o>>

plane::plane(){
   a = new vector();            // initialize memory
   n = new vector();
}

plane::plane(vector *_a, vector *_n){
   a = new vector(_a);           // initialize memory
   n = new vector(_n);           // copy init. values
   if(n->z<0)n->inv();
}

plane::plane(vector *_a, vector *_v1, vector *_v2){
   a = new vector(_a);           // initialize memory
   n = new vector();
   _v1->vectorProduct(_v2,n);    // find the normal
  // if(n->z<0)n->inv();
}

plane::~plane(){
   delete(a);                  // Clean up
   delete(n);
}

float plane::distanceTo(vector *point){
 float d = -a->scalarProduct(n);     // Offset point for the cartesian plane equation
 return (n->x*point->x +             // Don't ask me how this works, it does
         n->y*point->y +             // the Green book is magic, just great
         n->z*point->z + d)/
		(n->mag());
}

int plane::intersectLine(vector *la,vector *ld,vector *intersect){
  float t,d;

  d = -a->scalarProduct(n);     // Offset point for the cartesian plane equation
  t = (-d - la->scalarProduct(n))/(n->scalarProduct(ld));
  intersect->setTo(t*ld->x,
		   t*ld->y,
		   t*ld->z);
  intersect->add(la);
  return 0;
}

//----<point>----------------------------------


void point::draw(unsigned char color){
#ifndef __NT__
#ifndef __NOGRAPHICS
 wsetcolor(color);
 wvesa_putpixel(x,y);    //displaying information
#endif
#endif
}

void point::drawTo(point *p2,unsigned char color)
{
#ifndef __NT__
#ifndef __NOGRAPHICS
 wsetcolor(color);
 wvesa_line(x,y,p2->x,p2->y);
#endif
#endif

}

int point::fromVector(vector *v2){             //yrot for rotation through y axis
   if(_3d_yrot < 0) _3d_yrot += TRIG_RESOLUTION * 10;
   if(_3d_xrot < 0) _3d_xrot += TRIG_RESOLUTION * 10;

   int yrot = (int)(_3d_yrot % TRIG_RESOLUTION);
   int xrot = (int)(_3d_xrot % TRIG_RESOLUTION);

   int ox = (int)(v2->x * COORD_COEFF);
   int oy = (int)(v2->y * COORD_COEFF);
   int oz = (int)(v2->z * COORD_COEFF);

   int tx,ty;

   tx =  (ox*_cos[yrot] + oz*_sin[yrot]) / TRIG_COEFF;
   oz = (-ox*_sin[yrot] + oz*_cos[yrot]) / TRIG_COEFF;
   ox=tx;

   ty =  (oy*_cos[xrot] + oz*_sin[xrot]) / TRIG_COEFF;
   oz = (-oy*_sin[xrot] + oz*_cos[xrot]) / TRIG_COEFF;
   oy = ty;

   ox += _3d_xtrans;           // Translational constants
   oy += _3d_ytrans;
   oz += _3d_ztrans;

   x = ((ox*600)/oz)+ _3dXoff;
   y = ((oy*600)/oz)+ _3dYoff;

   return (int)oz;
}

//---<face>------------------------------------------------------------

face::face(){
 v[0].setTo(0,0,0);
 v[1].setTo(0,0,0);
 v[2].setTo(0,0,0);
}

face::face(float x0,float y0,float z0,
           float x1,float y1,float z1,
           float x2,float y2,float z2){
 v[0].setTo(x0,y0,z0);                        //initialize vectors as given
 v[1].setTo(x1,y1,z1);
 v[2].setTo(x2,y2,z2);
 v[1].sub(&v[0]);                             //convert 1&2 into plane vectors
 v[2].sub(&v[0]);
 v[1].vectorProduct(&v[2],&n);                // find the normal
 d = -v[0].scalarProduct(&n);
}
void face::setTo(float x0,float y0,float z0,
           float x1,float y1,float z1,
           float x2,float y2,float z2){
 v[0].setTo(x0,y0,z0);                        //initialize vectors as given
 v[1].setTo(x1,y1,z1);
 v[2].setTo(x2,y2,z2);
 v[1].sub(&v[0]);                             //convert 1&2 into plane vectors
 v[2].sub(&v[0]);
 v[1].vectorProduct(&v[2],&n);                // find the normal
 d = -v[0].scalarProduct(&n);
}

void face::draw(char color){
 point c1,c2,c3;

 vector v1; v1.setTo(&v[0]); v1.add(&v[1]);
 vector v2; v2.setTo(&v[0]); v2.add(&v[2]);
 c1.fromVector(&v[0]);
 c2.fromVector(&v1);
 c3.fromVector(&v2);

/*
 gpoly3(c1.x,c1.y,v[0].z*6+127,
        c2.x,c2.y,v[1].z*6+127,
        c3.x,c3.y,v[2].z*6+127);
*/
 c1.drawTo(&c2,color);
 c1.drawTo(&c3,color);
 c3.drawTo(&c2,color);




}
void face::drawSh(char color){
 point c1,c2,c3;
 int z1,z2,z3;

 color=color;
 vector v1; v1.setTo(&v[0]); v1.add(&v[1]);
 vector v2; v2.setTo(&v[0]); v2.add(&v[2]);
 z1 = c1.fromVector(&v[0]) / -100 + 130;
 z2 = c2.fromVector(&v1) / -100 + 130;
 z3 = c3.fromVector(&v2) / -100 + 130;

 c1.drawTo(&c2,z2);
 c1.drawTo(&c3,z1);
 c3.drawTo(&c2,z3);
}


float face::circumCircle(vector *center){  // returns radius of Circumcircle
  vector nrml;
  float denom, v1sqlen, v2sqlen;

  // Squares of magnitudes of v[1] and v[2]
  v1sqlen = v[1].x * v[1].x + v[1].y * v[1].y + v[1].z * v[1].z;
  v2sqlen = v[2].x * v[2].x + v[2].y * v[2].y + v[2].z * v[2].z;

  // find right angle vector on v[1] & v[2]
  v[1].vectorProduct(&v[2],&nrml);

  // precompute denominator
  denom = 0.5 / (nrml.x * nrml.x + nrml.y * nrml.y +
                       nrml.z * nrml.z);
  // find circumcenter
  center->x = ((v1sqlen * v[2].y - v2sqlen * v[1].y) * nrml.z -
               (v1sqlen * v[2].z - v2sqlen * v[1].z) * nrml.y) * denom;
  center->y = ((v1sqlen * v[2].z - v2sqlen * v[1].z) * nrml.x -
               (v1sqlen * v[2].x - v2sqlen * v[1].x) * nrml.z) * denom;
  center->z = ((v1sqlen * v[2].x - v2sqlen * v[1].x) * nrml.y -
               (v1sqlen * v[2].y - v2sqlen * v[1].y) * nrml.x) * denom;
  float radius = center->mag();
  center->add(&v[0]);  // center is relative to v[0], make it absolute
  return radius;
}


float face::intersectVector(vector *a, vector *u){
 float t = (-d - a->scalarProduct(&n))/(n.scalarProduct(u));
 float dist=t*u->mag();
 vector intersect(t*u->x,
                 t*u->y,
				 t*u->z);
 intersect.add(a);

 vector v0p(&intersect);
 v0p.sub(&v[0]);

 float angle1 = v0p.angleWith(&v[1]);
 float angle2 = v0p.angleWith(&v[2]);
 float angle3 = v[1].angleWith(&v[2]);

 if(angle1 > angle3) return 0;
 if(angle2 > angle3) return 0;
//----------------------------------------------
 v0p.setTo(&intersect);
 v0p.sub(&v[0]);
 v0p.sub(&v[1]);

 vector vec1(&v[2]);
 vec1.sub(&v[1]);
 vector vec2(&v[1]);
 vec2.inv();

 angle1 = v0p.angleWith(&vec1);
 angle2 = v0p.angleWith(&vec2);
 angle3 = vec1.angleWith(&vec2);

 if(angle1 > angle3) return 0;
 if(angle2 > angle3) return 0;

 return dist;
}


void drawPointer(vector *a,vector *d, float length, char color){
 point p1,p2;

 p1.fromVector(a);
 vector temp(d);
 temp.unify();
 temp.x *= length;
 temp.y *= length;
 temp.z *= length;
 temp.add(a);
 p2.fromVector(&temp);
 p1.drawTo(&p2,color);
}

int line2lineDistance(vector *p1,vector *d1,
		      vector *p2,vector *d2,
		      float *dist,vector *ap1,vector *ap2){

  vector d(d1);
  d.sub(d2);
  if(d.mag()==0) return 0;

  vector c;
  d1->vectorProduct(d2, &c);


  plane pl1(p1,d1,&c);
  plane pl2(p2,d2,&c);

  pl2.intersectLine(p1,d1,ap1);
  pl1.intersectLine(p2,d2,ap2);

  *dist = ap1->distanceTo(ap2);

  return 1;
}

int test4PathCollision(vector *a, vector *b, vector *p, float minDist){
   float g=(a->x-b->x);
   float h=(a->y-b->y);
   float j=(a->z-b->z);
   float ab = sqrt(g*g+h*h+j*j);

   g=(a->x-p->x);
   h=(a->y-p->y);
   j=(a->z-p->z);
   float ap = sqrt(g*g+h*h+j*j);

   g=(b->x-p->x);
   h=(b->y-p->y);
   j=(b->z-p->z);
   float bp = sqrt(g*g+h*h+j*j);

   if((ap+bp) > (ab + minDist)) return 0;

   float s = (ab+ap+bp)*0.5;
   float l = 2*sqrt(s*(s-ab)*(s-bp)*(s-ap))/ab;

   if(l>minDist) return 0;

   return 1;
}

int test4PathCollision(vector *a, vector *b, float ab, vector *p, float minDist){
   float g=(a->x-p->x);
   float h=(a->y-p->y);
   float j=(a->z-p->z);
   float ap = sqrt(g*g+h*h+j*j);

   if(ap > (ab*2 + minDist)) return 0;

   g=(b->x-p->x);
   h=(b->y-p->y);
   j=(b->z-p->z);
   float bp = sqrt(g*g+h*h+j*j);

   if((ap+bp) > (ab + minDist)) return 0;

   float s = (ab+ap+bp)*0.5;
   float l = 2*sqrt(s*(s-ab)*(s-bp)*(s-ap))/ab;

   if(l>minDist) return 0;

   return 1;
}
