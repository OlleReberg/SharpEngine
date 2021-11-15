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
        Vertex[] vertices;
        Matrix transform = Matrix.Identity;
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
            
        }
        public void Move(Vector direction)
        {
            transform *= Matrix.Translation(direction);
        }

        public void Rotate(float rotation)
        {

        
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
            material.SetTransform(transform);
            glBindVertexArray(vertexArray);
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            fixed (Vertex* vertex = &vertices[0]) {
                glBufferData(GL_ARRAY_BUFFER, sizeof(Vertex) * vertices.Length, vertex, GL_DYNAMIC_DRAW);
            }
            glDrawArrays(GL_TRIANGLES, 0, vertices.Length);
            glBindVertexArray(0);
        }
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