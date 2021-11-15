
namespace SharpEngine
{
    public struct Matrix
        {
            public float m11, m12, m13, m14;
            public float m21, m22, m23, m24;
            public float m31, m32, m33, m34;
            public float m41, m42, m43, m44;

            // generated using ctorf + enter
            public Matrix(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24, 
                float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
            {
                this.m11 = m11; this.m12 = m12;this.m13 = m13; this.m14 = m14;
                this.m21 = m21; this.m22 = m22; this.m23 = m23; this.m24 = m24;
                this.m31 = m31; this.m32 = m32; this.m33 = m33; this.m34 = m34;
                this.m41 = m41; this.m42 = m42; this.m43 = m43; this.m44 = m44;
                
            }

            public static Matrix Identity => new (1, 0, 0, 0,
                                                 0, 1, 0, 0,
                                                 0, 0, 1, 0,
                                                 0, 0, 0, 1);

            public static Vector operator *(Matrix m, Vector v)
            {
                return new Vector(m.m11 * v.x + m.m12 * v.y + m.m13 * v.z + m.m14 * 1,
                                  m.m21 * v.x + m.m22 * v.y + m.m23 * v.z + m.m24 * 1,
                                  m.m31 * v.x + m.m32 * v.y + m.m33 * v.z + m.m34 * 1);
            }
            
            public static Matrix Translation(Vector translation)
            {
                var result = Identity;
                result.m14 = translation.x;
                result.m24 = translation.y;
                result.m34 = translation.z;
                return result;
            }

            public static Matrix Scale(Vector scale)
            {
                var result = Identity;
                result.m11 = scale.x;
                result.m22 = scale.y;
                result.m33 = scale.z;
                return result;
            }
        }
   
}

 

    
    