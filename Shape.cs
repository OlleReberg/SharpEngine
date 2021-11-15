using System;
using System.Runtime.InteropServices;
using OpenGL;
using static OpenGL.Gl;


namespace SharpEngine
{
    public class Shape
    {
        uint vertexArray;
        uint vertexBuffer;
        protected Vertex[] vertices;
        public Material material;
        public Shape(Vertex[] vertices, Material material)
        {
            this.vertices = vertices;
            this.material = material;
            CurrentScale = 1f;
            LoadTriangleIntoBuffer();
        }
        
        public float CurrentScale { get; private set;}
        //public float 
        
        public Vector GetMinBounds()
        {
            var min = vertices[0].position;
            for (var i = 1; i < vertices.Length; i++) {
                min = Vector.Min(min, vertices[i].position);
            }
            return min;
        }

        public Vector GetMaxBounds()
        {
            var max = vertices[0].position;
            for (var i = 1; i < vertices.Length; i++) 
            {
                max = Vector.Max(max, vertices[i].position);
            }
            return max;
        }

        public Vector GetCenter()
        {
            return (GetMinBounds() + GetMaxBounds()) / 2;
        }

        public void Scale(float multiplier)
        {
            var offset = GetCenter();
            Move(-offset);
            Matrix matrix = Matrix.Scale(new Vector(multiplier, multiplier));
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i].position = matrix * vertices[i].position;
            }
            Move(offset);
            CurrentScale *= multiplier;
        }
        public void Move(Vector direction)
        {
            Matrix matrix = Matrix.Translation(direction);
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i].position = matrix * vertices[i].position;
            }

            
        }

        public void Rotate(float rotation)
        {

        
        }
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

            public static Matrix Identity => new Matrix(1, 0, 0, 0,
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

        private float GetRadians(float angle)
        {
            return angle * (MathF.PI / 180f);
        }

        public Vector MoveDirection(Vector direction)
        {
            //3. Move Triangle by its direction
            Move(direction);
               
            // 4. Check the X-Bounds of the Screen
            if(GetMaxBounds().x >= 1 && direction.x > 0  || GetMinBounds().x <= -1 && direction.x < 0)
            {
                direction.x *= -1;
            }
            
            //5. Same as 4 but for y
            if(GetMaxBounds().y >= 1 && direction.y > 0  || GetMinBounds().y <= -1 && direction.y < 0)
            {
                direction.y *= -1;
            }

            return direction;
        }
        public float CurrentScalar(float multiplier)
        {
            // 2. Keep track of the Scale, so we can reverse it
            if (CurrentScale <= 0.5f) 
            {
                multiplier = 1.001f;
            }
            if (CurrentScale >= 1f) 
            {
                multiplier = 0.999f;
            }

            return multiplier;
        }

        public unsafe void Render() 
        {
            material.Use();
            glBindVertexArray(vertexArray);
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            fixed (Vertex* vertex = &vertices[0]) {
                glBufferData(GL_ARRAY_BUFFER, sizeof(Vertex) * vertices.Length, vertex, GL_DYNAMIC_DRAW);
            }
            glDrawArrays(GL_TRIANGLES, 0, vertices.Length);
            glBindVertexArray(0);
        }
        //2. Fix your Code to make it work with two separate Triangle instances.
        //Hint: You need to save the return value of glGenVertexArray and then always call glBindVertexArray
        //before Rendering the Triangle in its Render-Method. Important, you need to do that as the first thing in the Render-Method.
        public unsafe uint LoadTriangleIntoBuffer()
        {
            vertexArray = glGenVertexArray();
            vertexBuffer = glGenBuffer();
            glBindVertexArray(vertexArray);
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            glVertexAttribPointer(0, 3, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), nameof(Vertex.position)));
            glVertexAttribPointer(1, 4, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), nameof(Vertex.color)));
            glEnableVertexAttribArray(0);
            glEnableVertexAttribArray(1);
            glBindVertexArray(0);
            return vertexArray;
        }
    }

    
}