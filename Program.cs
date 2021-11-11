using System.IO;
using System.Net;
using System.Numerics;
using GLFW;
using static OpenGL.Gl;

namespace SharpEngine
{
    public struct Vertex
    {
        public Vector position;

        public Vertex(Vector position)
        {
            this.position = position;
        }
    }
    class Program
    {
        
        static Vertex[] vertices = new Vertex[]
        {
            new Vertex(new Vector(.4f, .4f)),
            new Vertex(new Vector(.6f, .4f)),
            new Vertex(new Vector(.5f, .6f)),
        };
        
        static void Main(string[] args)
        {
            //initialize and configure
            var window = CreateWindow();
            LoadTriangleIntoBuffer();
            CreateShaderProgram();
            
            var direction = new Vector(0.0002f, 0.0002f);
            var multiplier = 0.9999f;
            var scale = 1f;
            while (!Glfw.WindowShouldClose(window))
            {
                Glfw.PollEvents(); //reacts to window changes (position etc.)
                ClearScreen();
                Render(window);
                for (var i = 0; i < vertices.Length; i ++)
                {
                    vertices[i].position += direction;
                }
                //Move it to center, scale it, move it back. Quick in and out job no one will even notice!
                var min = vertices[0].position;
                for (var i = 1; i < vertices.Length; i++) {
                    min = Vector.Min(min, vertices[i].position);
                }
                var max = vertices[0].position;
                for (var i = 1; i < vertices.Length; i++) {
                    max = Vector.Max(max, vertices[i].position);
                }
                // 1.1.1.2 Average out the Minimum and Maximum to get the Center
                var center = (min + max) / 2;
                // 1.1.2 Move the Triangle the Center
                for (var i = 0; i < vertices.Length; i++) {
                    vertices[i].position -= center;
                }
                // 1.2 Scale the Triangle
                for (var i = 0; i < vertices.Length; i++) {
                    vertices[i].position *= multiplier;
                }
                // 1.3 Move the Triangle Back to where it was before
                for (var i = 0; i < vertices.Length; i++) {
                    vertices[i].position += center;
                }
                // 2. Keep track of the Scale, so we can reverse it
                scale *= multiplier;
                if (scale <= 0.5f) {
                    multiplier = 1.001f;
                }
                if (scale >= 1f) {
                    multiplier = 0.999f;
                }
                
                scale *= multiplier;
                
                if (scale <= 0.5f)
                {
                    multiplier = 1.0001f;
                }
                if (scale >= 1f)
                {
                    multiplier = .9999f;
                }
                for (var i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i].position.x >= 1 && direction.x > 0  || vertices[i].position.x <= -1 && direction.x < 0 )
                    {
                        direction.x *= -1;
                        break;
                    }
                }
                for (var i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i].position.y >= 1 && direction.y > 0 || vertices[i].position.y <= -1 && direction.y < 0 )
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
            fixed (Vertex* vertex = &vertices[0])
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
