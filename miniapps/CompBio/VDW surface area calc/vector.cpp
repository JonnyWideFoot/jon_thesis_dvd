#include "vector.h"



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
 return sqrt(g*g+h*h+j*j);
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