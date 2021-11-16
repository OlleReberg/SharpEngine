using System.Collections.Generic;

namespace SharpEngine 
{
    public class Scene 
    {
        public List<Shape> shapes;

        public Scene() 
        {
            shapes = new List<Shape>();
        }
        public void Add(Shape shape) 
        {
            shapes.Add(shape);
        }

        public void Render() 
        {
            for (int i = 0; i < shapes.Count; i++) 
            {
                shapes[i].Render();
            }
        }
    }
}