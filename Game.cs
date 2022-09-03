
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace PrimerEsenario
{
    public class Game : GameWindow
    {
        int VertexBufferObject; //valor entero para nuestro buffer-> identificador de buffer de vertices
       
         int shaderProgramHandle;
         int vertixArrayHandle;
        public Game() : base(GameWindowSettings.Default,NativeWindowSettings.Default)
        {
            this.CenterWindow(new Vector2i(1200,768)); //tamaño de la pantalla
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height); //para poner  el triangulo en la pantalla
            base.OnResize(e);
        }


        /* Cuando inicialize el programa
         * 
         * 1.- primero generamos el buffer para que el objetivo sea diganle 
         * a opnegl que tipo de buffer esta bien
         * asi que lo que queremos es un buffer de matriz que es el primero que existe
         * para luego pasar los datos a la tarjeta grafica
         * 
         * para las sombras
         *declaramos todos los atributos de vértice de entrada en el sombreador de vértices con la palabra clave in
         *Como cada vértice tiene una coordenada 3D, creamos una variable de entrada vec3 con el nombre aPosition
         *establecemos específicamente la ubicación de la variable de entrada a través del diseño (ubicación = 0) y luego verá por qué vamos a necesitar esa ubicación.
         */
        protected override void OnLoad()
        { 
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);  //definiendo color del trinagulo

             float[] vertices = {
            -0.5f, -0.5f, 0.0f, //Bottom-left vertex
             0.5f, -0.5f, 0.0f, //Bottom-right vertex
             0.0f,  0.5f, 0.0f  //Top vertex
             };

            this.VertexBufferObject = GL.GenBuffer();   //
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject); //unirnos a varios buffer que tengan un tipo diferente
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);//vusara para configurar el buffer enlasado actualmente es vertexObject
           //buffedara copia los datos de vertice previamente adefinidos a la memoria del buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer,0);
           


            this.vertixArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(this.vertixArrayHandle);

            //dibujar shander
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexBufferObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
            //sombreado de vertices
            //cada shander comienza una declaracion  a su version
            string verticeShaderCode =
                @"
                #version 330 core
                    layout (location = 0) in vec3 aPosition;  
                  

                    void main()
                    {
                        gl_Position = vec4(aPosition, 1.0);
                      
                    }
                ";

            string pixelShaderCode =
              @"
                #version 330 core
                    out vec4 FragColor;

                    void main()
                {
                  FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
                }
                ";

            //generamos nuestros sombreadores y vinculamos el código fuente a los sombreadores.
            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle,verticeShaderCode);
            GL.CompileShader(vertexShaderHandle);

            //

            int pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShaderHandle, pixelShaderCode);
            GL.CompileShader(pixelShaderHandle);

            this.shaderProgramHandle = GL.CreateProgram();

            //nos vinculamos a un programa donde que se pueda ejecutar en la GPU
            GL.AttachShader(this.shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(this.shaderProgramHandle, pixelShaderHandle);

            GL.LinkProgram(this.shaderProgramHandle);

            //pequeña limpieza. los datos compilados se copian en el programa de sombreado cuando lo vincula
            GL.DetachShader(this.shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(this.shaderProgramHandle, pixelShaderHandle);
            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader( pixelShaderHandle);

            base.OnLoad();
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
              {
           
                  base.OnUpdateFrame(e);
              }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // sombreador válido
            GL.UseProgram(this.shaderProgramHandle);

            GL.BindVertexArray(this.vertixArrayHandle);
            GL.DrawArrays(PrimitiveType.Triangles,0,3);
            
            this.Context.SwapBuffers();
            base.OnRenderFrame(e);
        }
       
           
         

        /* Cuando Finalize el programa
 * aqui tenes que limpiar manualmente nuestro buffer 
 */
        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //cuando esta en 0 es nulo 
            GL.DeleteBuffer(this.VertexBufferObject);

            GL.UseProgram(0);
            // limpiar el identificador después de que muera esta clase
            GL.DeleteProgram(this.shaderProgramHandle);


            base.OnUnload();
        }
    }
}
