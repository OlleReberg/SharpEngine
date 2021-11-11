using System;
using System.IO;
using System.Net;
using GLFW;
using static OpenGL.Gl;

namespace SharpEngine
{
    struct Vector
    {
        public float x, y, z;

        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public static Vector operator *(Vector v, float f)
        {
            return new Vector(v.x * f, v.y * f, v.z * f);
        }

        public static Vector operator +(Vector lhs, Vector rhs)
        {
            return new Vector(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }
        public static Vector operator -(Vector v, float f)
        {
            return new Vector(v.x - f, v.y - f, v.z - f);
        }
        public static Vector operator /(Vector v, float f)
        {
            return new Vector(v.x / f, v.y / f, v.z / f);
        }
        // +
        // -
        // /
         // Vectors = Direction and Magnitude
         // Position Vectors = Origin + Vector = Vector
         // Vector Addition and Subtraction: (3   2)  (6   5)   = (3+6     2+5) = (9    7)
         // Vector Magnitude: sqrt(x^2 + y^2 + z^2)
         // Vector Scalar Multiplication: (3    2) * 2 = (3 * 2     2 * 2) = (6    4)
    }
    class Program
    {
        static Vector[] vertices = new Vector[]
        {
            new (-.1f, -.1f),
            new (.1f, -.1f),
            new (0f, .1f),
           // new (.4f, .4f),
           // new (.6f, .4f),
           // new (.5f, .6f),
        };
        
        
        static void Main(string[] args)
        {
            //initialize and configure
            var window = CreateWindow();

            LoadTriangleIntoBuffer();


            CreateShaderProgram();

            //engine rendering loop
            var direction = new Vector(0.0002f, 0.0002f);
            while (!Glfw.WindowShouldClose(window))
            {
                Glfw.PollEvents(); //reacts to window changes (position etc.)
                ClearScreen();
                Render(window);
                for (var i = 0; i < vertices.Length; i ++)
                {
                    vertices[i] += direction;
                }

                for (var i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i].x >= 1)
                    {
                        direction.x *= -1;
                    }
                }
                for (var i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i].y >= 1)
                    {
                        direction.y *= -1;
                    }
                }
                for (var i = 0; i < vertices.Length; i ++)
                {
                    vertices[i] += direction;
                }
                for (var i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i].x <= -1)
                    {
                        direction.x *= -1;
                    }
                }

                for (var i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i].y <= -1)
                    {
                        direction.y *= -1;
                        break;
                    }
                }
                UpdateTriangleBuffer();
            }
        }

        
        private static void Render(Window window)
        {
            glDrawArrays(GL_TRIANGLES, 0, vertices.Length);
            Glfw.SwapBuffers(window);
            //glFlush();
        }

        private static void ClearScreen()
        {
            glClearColor(.2f, .4f, .2f, alpha:1 );
            glClear(GL_COLOR_BUFFER_BIT);
        }

        private static void CreateShaderProgram()
        {
            //create vertex shader
            var vertexShader = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertexShader, File.ReadAllText("shaders/screen-coordinates.vert"));
            glCompileShader(vertexShader);

            //create fragment shader
            var fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fragmentShader, File.ReadAllText("shaders/blue.frag"));
            glCompileShader(fragmentShader);

            //create shader program - rendering pipeline
            var program = glCreateProgram();
            glAttachShader(program, vertexShader);
            glAttachShader(program, fragmentShader);
            glLinkProgram(program);
            glUseProgram(program);
        }

        private static unsafe void LoadTriangleIntoBuffer()
        {
            var vertexArray = glGenVertexArray();
            var vertexBuffer = glGenBuffer();
            glBindVertexArray(vertexArray);
            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            UpdateTriangleBuffer();
            glVertexAttribPointer(0, 3, GL_FLOAT, false, sizeof(Vector), NULL);
            glEnableVertexAttribArray(0);
        }

        static unsafe void UpdateTriangleBuffer()
        {
            fixed (Vector* vertex = &vertices[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(Vector) * vertices.Length, vertex, GL_DYNAMIC_DRAW);
            }
        }

        private static Window CreateWindow()
        {
            Glfw.Init();
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.Decorated, true);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.OpenglForwardCompatible, Constants.True);
            Glfw.WindowHint(Hint.Doublebuffer, Constants.True);

            //create and launch a window
            var window = Glfw.CreateWindow(1024, 768, "SharpEngine", Monitor.None, GLFW.Window.None);
            Glfw.MakeContextCurrent(window);
            Import(Glfw.GetProcAddress);
            return window;
        }
    }
}
