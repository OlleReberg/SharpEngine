using System;
using System.Runtime.InteropServices;
using OpenGL;
using static OpenGL.Gl;


namespace SharpEngine
{

    public class Triangle : Shape
    {
        public Triangle(float width, float height, Vector position) : base(new Vertex[3])
        {
            vertices[0] = new Vertex(new Vector(position.x - width / 2, position.y - height / 2), Color.Red);
            vertices[1] = new Vertex(new Vector(position.x + width / 2, position.y - height / 2), Color.Green);
            vertices[2] = new Vertex(new Vector(position.x, position.y + height / 2), Color.Blue);
        }
    }
    public class Shape
    {
        protected Vertex[] vertices;
        public Shape(Vertex[] vertices)
        {
            this.vertices = vertices;
            CurrentScale = 1f;
            Render(LoadTriangleIntoBuffer());
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
            CurrentScale *= multiplier;
            // First moving triangle to center, to fix scaling, then move it back
            var center = GetCenter();
            Move(center*-1);
            for (var i = 0; i < vertices.Length; i++) 
            {
                vertices[i].position *= multiplier;
            }
            Move(center);
        }
        public void Move(Vector direction)
        {
            for (var i = 0; i < vertices.Length; i ++)
            {
                vertices[i].position += direction;
            }
        }

        public void Rotate(float rotation)
        {
            var center = GetCenter();
            Move(center * -1);
            for (int i = 0; i < vertices.Length; i++)
            {
                var newAngle = MathF.Atan2(vertices[i].position.y, vertices[i].position.x);
                var magnitutde = MathF.Sqrt(MathF.Pow(vertices[i].position.x, 2) + MathF.Pow(vertices[i].position.y, 2));
                var rotationx = MathF.Cos(newAngle + GetRadians(rotation)) * magnitutde;
                var rotationy = MathF.Sin(newAngle + GetRadians(rotation)) * magnitutde;
                vertices[i].position = new Vector(rotationx, rotationy); 
            }
            Move(center);
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

        public unsafe void Render(uint vertexArray) 
        {
            fixed (Vertex* vertex = &vertices[0]) {
                glBufferData(GL_ARRAY_BUFFER, sizeof(Vertex) * vertices.Length, vertex, GL_DYNAMIC_DRAW);
            }
            glDrawArrays(GL_TRIANGLES, 0, vertices.Length);
            glBindVertexArray(vertexArray);
        }
        //2. Fix your Code to make it work with two separate Triangle instances.
        //Hint: You need to save the return value of glGenVertexArray and then always call glBindVertexArray
        //before Rendering the Triangle in its Render-Method. Important, you need to do that as the first thing in the Render-Method.
        public unsafe uint LoadTriangleIntoBuffer()
        {
            var vertexArray = glGenVertexArray();
            var vertexBuffer = glGenBuffer();
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            glVertexAttribPointer(0, 3, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), nameof(Vertex.position)));
            glVertexAttribPointer(1, 4, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), nameof(Vertex.color)));
            glEnableVertexAttribArray(0);
            glEnableVertexAttribArray(1);
            return vertexArray;
        }
    }
}