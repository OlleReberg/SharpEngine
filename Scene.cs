using System.Collections.Generic;

namespace SharpEngine {
    public class Scene {

        public List<Shape> triangles;

        public Scene() {
            triangles = new List<Shape>();
        }
		
        public void Add(Shape triangle) {
            triangles.Add(triangle);
        }

        public void Render() {
            for (int i = 0; i < triangles.Count; i++) {
                triangles[i].Render();
            }
        }
    }
}