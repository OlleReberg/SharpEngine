using System.IO;
using System.Net;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GLFW;
using static OpenGL.Gl;

namespace SharpEngine
{
    class Program
    {
        static Triangle triangle = new Triangle
        (
            new Vertex[] 
            {
                new Vertex(new Vector(.2f, .2f), Color.Red),
                new Vertex(new Vector(.8f, .6f), Color.Green),
                new Vertex(new Vector(.8f, .1f), Color.Blue),
                
                new Vertex(new Vector(.2f, .0f), Color.Red),
                new Vertex(new Vector(.5f, .0f), Color.Green),
                new Vertex(new Vector(.5f, .1f), Color.Blue)
            }
        );
        static void Main(string[] args)
        {
            //initialize and configure
            var window = CreateWindow();
            LoadTriangleIntoBuffer();
            CreateShaderProgram();
            
            var direction = new Vector(0.0002f, 0.0002f);
            var multiplier = 0.9999f;
            //Engine rendering loop
            while (!Glfw.WindowShouldClose(window))
            {
                Glfw.PollEvents(); //reacts to window changes (position etc.)
                ClearScreen();
                Render(window);
                triangle.Scale(multiplier);
                
                // 2. Keep track of the Scale, so we can reverse it
                if (triangle.CurrentScale <= 0.5f) {
                    multiplier = 1.001f;
                }
                if (triangle.CurrentScale >= 1f) {
                    multiplier = 0.999f;
                }
                //3. Move Triangle by its direction
               triangle.Move(direction);
               
                // 4. Check the X-Bounds of the Screen
                if(triangle.GetMaxBounds().x >= 1 && direction.x > 0  || triangle.GetMinBounds().x <= -1 && direction.x < 0)
                {
                    direction.x *= -1;
                }
            
                //5. Same as 4 but for y
                if(triangle.GetMaxBounds().y >= 1 && direction.y > 0  || triangle.GetMinBounds().y <= -1 && direction.y < 0)
                {
                    direction.y *= -1;
                }
            }
        }
    
        private static void Render(Window window)
        {
            triangle.Render();
            Glfw.SwapBuffers(window);
            //glFlush();
        }
        private static void ClearScreen()
        {
            glClearColor(.1f, .1f, .2f, alpha:1 );
            glClear(GL_COLOR_BUFFER_BIT);
        }
        private static void CreateShaderProgram()
        {
            //create vertex shader
            var vertexShader = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertexShader, File.ReadAllText("shaders/position-color.vert"));
            glCompileShader(vertexShader);

            //create fragment shader
            var fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fragmentShader, File.ReadAllText("shaders/vertex-color.frag"));
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
            glVertexAttribPointer(0, 3, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), nameof(Vertex.position)));
            glVertexAttribPointer(1, 4, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), nameof(Vertex.color)));
            glEnableVertexAttribArray(0);
            glEnableVertexAttribArray(1);
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
