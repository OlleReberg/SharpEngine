using OpenGL;

namespace SharpEngine
{
    public class Triangle
    {
        private Vertex[] vertices;
        public Triangle(Vertex[] vertices)
        {
            this.vertices = vertices;
            CurrentScale = 1f;
        }

        public float CurrentScale { get; private set;}
        
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

        public unsafe void Render() 
        {
            fixed (Vertex* vertex = &vertices[0]) {
                Gl.glBufferData(Gl.GL_ARRAY_BUFFER, sizeof(Vertex) * vertices.Length, vertex, Gl.GL_DYNAMIC_DRAW);
            }
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, vertices.Length);
        }
    }
}