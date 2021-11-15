using System;
using System.IO;
using System.Net;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GLFW;
using static OpenGL.Gl;

namespace SharpEngine
{
       class Program {
        static float Lerp(float from, float to, float t) {
            return from + (to - from) * t;
        }

        static float GetRandomFloat(Random random, float min = 0, float max = 1) {
            return Lerp(min, max, (float)random.Next() / int.MaxValue);
        }
        
        static void FillSceneWithTriangles(Scene scene, Material material) {
            var newTriangle = new Shape(new Vertex[] {
                new Vertex(new Vector(-.1f, 0f), Color.Red),
                new Vertex(new Vector(.1f, 0f), Color.Green),
                new Vertex(new Vector(0f, .133f), Color.Blue)
            }, material);
            scene.Add(newTriangle);
        }
        
        static void Main(string[] args) {
            
            var window = new Window();
            var material = new Material("shaders/position-color.vert", "shaders/vertex-color.frag");
            var scene = new Scene();
            window.Load(scene);

            FillSceneWithTriangles(scene, material);
            
            // engine rendering loop
            var direction = new Vector(0.0003f, 0.0003f);
            var multiplier = 0.999f;
            var rotation = 0.0005f;
            while (window.IsOpen()) {

                // Update Triangles
                for (var i = 0; i < scene.triangles.Count; i++) {
                    var triangle = scene.triangles[i];
                
                    // 2. Keep track of the Scale, so we can reverse it
                    if (triangle.CurrentScale <= 0.5f) {
                        multiplier = 1.001f;
                    }
                    if (triangle.CurrentScale >= 1f) {
                        multiplier = 0.999f;
                    }
                    
                    triangle.Scale(multiplier);
                    triangle.Rotate(rotation);
                
                    // 4. Check the X-Bounds of the Screen
                    if (triangle.GetMaxBounds().x >= 1 && direction.x > 0 || triangle.GetMinBounds().x <= -1 && direction.x < 0) {
                        direction.x *= -1;
                    }
                
                    // 5. Check the Y-Bounds of the Screen
                    if (triangle.GetMaxBounds().y >= 1 && direction.y > 0 || triangle.GetMinBounds().y <= -1 && direction.y < 0) {
                        direction.y *= -1;
                    }
                    
                    
                    triangle.Move(direction);
                }
                
                window.Render();
            }
        }
    }
}
