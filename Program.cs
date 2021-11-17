using System;
using GLFW;

using System;
using GLFW;

namespace SharpEngine
{
    class Program
    {
        static float Lerp(float from, float to, float t)
        {
            return from + (to - from) * t;
        }

        static float GetRandomFloat(Random random, float min = 0, float max = 1)
        {
            return Lerp(min, max, (float) random.Next() / int.MaxValue);
        }

        static void Main(string[] args)
        {
            var window = new Window();
            var material = new Material("shaders/world-position-color.vert", "shaders/vertex-color.frag");
            var scene = new Scene();
            window.Load(scene);

            var triangle = new Triangle(material);
            triangle.Transform.CurrentScale = new Vector(0.5f, 1f, 1f);
            scene.Add(triangle);

            var rectangle = new Rectangle(material);
            rectangle.Transform.CurrentScale = new Vector(1f, 0.5f, 1f);
            rectangle.Transform.Position = new Vector(0f, -0.5f, 1f);
            scene.Add(rectangle);

            var circle = new Circle(material);
            circle.Transform.CurrentScale = new Vector(3f, 3f, 3f);
            circle.Transform.Position = new Vector(-0.5f, 0.5f, 1f);
            scene.Add(circle);

            var ground = new Rectangle(material);
            ground.Transform.CurrentScale = new Vector(10f, 1f, 1f);
            ground.Transform.Position = new Vector(0f, -1f);

            scene.Add(ground);

            // engine rendering loop
            const int fixedStepNumberPerSecond = 30;
            const float fixedDeltaTime = 1.0f / fixedStepNumberPerSecond;
            const float movementSpeed = 0.5f;
            double previousFixedStep = 0.0;
            while (window.IsOpen())
            {
                while (Glfw.Time > previousFixedStep + fixedDeltaTime)
                {
                    previousFixedStep += fixedDeltaTime;
                    var walkDirection = new Vector();

                    if (window.GetKey(Keys.W))
                    {
                        walkDirection += triangle.Transform.Forward;
                    }

                    if (window.GetKey(Keys.A))
                    {
                        walkDirection += triangle.Transform.Left;
                    }

                    if (window.GetKey(Keys.S))
                    {
                        walkDirection += triangle.Transform.Backward;
                    }

                    if (window.GetKey(Keys.D))
                    {
                        walkDirection += triangle.Transform.Right;
                    }

                    if (window.GetKey(Keys.Q))
                    {
                        var rotation = triangle.Transform.Rotation;
                        rotation.z += 2 * MathF.PI * fixedDeltaTime;
                        triangle.Transform.Rotation = rotation;
                    }

                    if (window.GetKey(Keys.E))
                    {
                        var rotation = triangle.Transform.Rotation;
                        rotation.z -= 2 * MathF.PI * fixedDeltaTime;
                        triangle.Transform.Rotation = rotation;
                    }

                    walkDirection = walkDirection.Normalize();
                    triangle.Transform.Position += walkDirection * movementSpeed * fixedDeltaTime;
                    float facing = Vector.Dot((rectangle.GetCenter() - triangle.GetCenter()).Normalize(), triangle.Transform.Forward);
                    float dotProduct = Vector.Dot((circle.GetCenter() - triangle.GetCenter()).Normalize(), triangle.Transform.Forward);
                    float angle = MathF.Acos(dotProduct);
                    float factor = angle / MathF.PI;
                    circle.SetColor(new Color(factor, factor, factor, 1));
                    bool isTriangleFacing = facing > 0;

                    if (isTriangleFacing)
                    {
                        rectangle.SetColor(Color.Red);
                    }
                    else
                    {
                        rectangle.SetColor(Color.Blue);
                    }
                }
                window.Render();
            }
        }
    }
}