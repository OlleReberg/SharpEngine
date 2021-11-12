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
        private static Shape triangle = new Triangle(.8f, .8f, new Vector(-.4f, 0));
        private static Shape triangle2 = new Triangle(.4f, .4f, new Vector(.4f, 0));
        

        private static Shape[] _triangles = {triangle, triangle2};
        static void Main()
        {
            //initialize and configure
            var window = CreateWindow();
            CreateShaderProgram();
            
            var direction = new Vector(0.0002f, 0.0002f);
            var multiplier = 0.9999f;
            var rotate = .5f;
            //Engine rendering loop
            while (!Glfw.WindowShouldClose(window))
            {
                foreach (var triangle in _triangles)
                {
                    Glfw.PollEvents(); //reacts to window changes (position etc.)
                    ClearScreen();
                    Render(window);
                    triangle.Scale(multiplier);
                    triangle2.Scale(multiplier);
                    triangle.Rotate(rotate);
                    triangle2.Rotate(rotate);

                    multiplier = triangle.CurrentScalar(multiplier);
                    direction = triangle.MoveDirection(direction);
                    multiplier = triangle2.CurrentScalar(multiplier);
                    direction = triangle2.MoveDirection(direction);
                    
                }
                
            }
        }
        
        private static void Render(Window window)
        {
            triangle.Render(triangle.LoadTriangleIntoBuffer());
            triangle2.Render(triangle2.LoadTriangleIntoBuffer());
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
