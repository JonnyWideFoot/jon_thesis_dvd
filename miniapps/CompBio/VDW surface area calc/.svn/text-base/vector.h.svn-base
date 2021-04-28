#define _infinity 10000000000       // just pretty big
#define pi   ((double)3.1415926535897932385)
#define _2pi ((double)6.283185307)
#define _pid180 ((double)0.017453292)  // pi / 180 (deg/rad conv)




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