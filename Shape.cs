using System;
using System.Runtime.InteropServices;
using OpenGL;
using static OpenGL.Gl;


namespace SharpEngine
{
    public class Shape
    {
        Vertex[] vertices;
        uint vertexArray;
        uint vertexBuffer;
        public Material material;
        
        public Transform Transform { get; }
        
        public Shape(Vertex[] vertices, Material material)
        {
            this.vertices = vertices;
            this.material = material;
            LoadTriangleIntoBuffer();
            Transform = new Transform();
        }
        
        public Vector GetMinBounds()
        {
            var min = Transform.Matrix * vertices[0].position;
            for (var i = 1; i < vertices.Length; i++) {
                min = Vector.Min(min, Transform.Matrix * vertices[i].position);
            }
            return min;
        }
        public Vector GetMaxBounds()
        {
            var max = Transform.Matrix * vertices[0].position;
            for (var i = 1; i < vertices.Length; i++) 
            {
                max = Vector.Max(max, Transform.Matrix * vertices[i].position);
            }
            return max;
        }
        public Vector GetCenter()
        {
            return (GetMinBounds() + GetMaxBounds()) / 2;
        }
       
        public unsafe void Render() 
        {
            material.Use();
            material.SetTransform(Transform.Matrix);
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
        public void SetColor(Color color)
        {
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i].color = color;
            }
        }

    }
}