using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using SceneLib;
using EnableCap = OpenTK.Graphics.OpenGL.EnableCap;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace GPURenderer
{
  /// <summary>
  /// Renderer básico en OpenGL que renderea el primer mesh de la escena configurada. 
  /// </summary>
  class MainWindow : GameWindow
  {
    private readonly Scene _scene;

    //El codigo de los shaders debe ser leido de archivo por la CPU y luego enviado a la GPU para ser compilado
    private string _vertexShaderSource, _pixelShaderSource;
    private const string VertexShaderFilePath = @"Shaders\VertexShader.glsl";
    private const string PixelShaderFilePath = @"Shaders\PixelShader.glsl";

    //OpenGL ocupa identificadores enteros (aka handles) para saber ubicaciones de distintos elementos en la GPU
    private int _vertexShaderHandle;
    private int _pixelShaderHandle;
    private int _shaderProgramHandle;
    private int _vaoHandle;
    private int _positionVboHandle;
    private int _normalVboHandle;
    private int _eboHandle;

    //Arreglos para almacenar la información por vértice del mesh (posicion y normal)
    private Vector3[] _positionVboData;
    private Vector3[] _normalVboData;

    //Arreglo de índices usado para indicar la geometría que forman los vértices. Los índices apuntan a ubicaciones en los arreglos de arriba
    //y en el caso de que la geometría sean triángulos (podrían también ser líneas o puntos) se consideran 3 índices secuencuales para referirse a un triángulo
    //La idea de este arreglo de índices es que idealmente uno no quiere repetir vértices que sean comunes a varios triángulos, entonces
    //en los arreglos de información de vértice estos se podrían definir una sola vez, pero ser indexados múltiples veces en este arreglo de índices 
    private int[] _indicesVboData;

    //Matrices de viewing, que serán pasadas como uniform a la GPU
    private Matrix4 _projectionMatrix, _viewMatrix, _modelToWorld;
    
    public MainWindow(int width, int height)
      : base(width, height,
        new OpenTK.Graphics.GraphicsMode(), "GPU Renderer", GameWindowFlags.Default,
        DisplayDevice.Default, 3, 0,
        OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible | OpenTK.Graphics.GraphicsContextFlags.Debug)
    {
      var meshLoader = new ObjMeshLoader();
      var renderObjectFactory = new RenderObjectFactory(meshLoader);
      _scene = new Scene(width, height, renderObjectFactory);
      _scene.Load(RenderingParameters.Instance.ScenePath);
      
      Mouse.Move += OnMouseMoved;
    }

    //En el OnLoad de la ventan haremos el setup necesario para realizar el rendering de un objeto de la escena
    protected override void OnLoad(EventArgs e)
    {
      OpenGLConfiguration(); //Configurar distintos parámetros de las etapas no programables del algoritmo en GPU
      RenderSetup(); //Setup de información necesaria para el rendering: vertices y viewing
      CreateShaders(); //Los shaders deben ser leidos de archivo, enviados a la GPU, compilados y configurados sus uniform
      CreateVertexBuffers(); //Se crean los vertex buffer que contienen la información por vértice
      CreateVertexArrays(); //Se crean los vertex array que agrupan la información de los vertex buffer, asociando a atributos específicos del shader
    }

    //En OpenGL existen una serie de configuraciones de las etapas del algotimo de rendering que no son programablaes 
    //que pueden ser habilitadas (Gl.Enable) o deshabilitadas (Gl.Disable)
    //Además, hay ciertos valores especiales que deben ser configurados directamente, como el color de fondo "ClearColor"
    private void OpenGLConfiguration()
    {
      GL.Enable(EnableCap.DepthTest);
      GL.Enable(EnableCap.Texture2D);
      GL.ClearColor(_scene.BackgroundColor.ToColor4());
    }

    //En este ejemplo estamos solamente renderando el primer mesh de la escena. En este método se guardan las posiciones, normales e índices de triángulos
    //del primer mesh.
    //Además se calculan las matrices de viewing, incluyendo la matriz de objeto a mundo para este mesh
    private void RenderSetup()
    {
      var mesh = _scene.Objects.FirstOrDefault(o => o is IHaveTriangles) as IHaveTriangles;
      var vertices = new List<Vector3>();
      var normals = new List<Vector3>();
      mesh.Triangles.ForEach(t => t.Vertices.ForEach(vt => vertices.Add(vt.Position.ToVector3())));
      mesh.Triangles.ForEach(t => t.Vertices.ForEach(vt => normals.Add(vt.Normal.ToVector3())));
      _positionVboData = vertices.ToArray();
      _normalVboData = normals.ToArray();
      _indicesVboData = Enumerable.Range(0, mesh.Triangles.Count * 3).ToArray();

      ViewingSetup(mesh);
    }

    //Las matrices de viewing son implementadas ocupando clases helpers de la libería OpenTK (el wrapper de C# de OpenGL que estamos ocupando)
    //La API de OpenGL no provee estos helpers
    private void ViewingSetup(IHaveTriangles mesh)
    {
      var scaleMatrix = Matrix4.CreateScale(mesh.Scale.ToVector3());
      var rotationMatrixX = Matrix4.CreateRotationX((float)(mesh.Rotation.X.ToRadians()));
      var rotationMatrixY = Matrix4.CreateRotationY((float)(mesh.Rotation.Y.ToRadians()));
      var rotationMatrixZ = Matrix4.CreateRotationZ((float)(mesh.Rotation.Z.ToRadians()));
      var translationMatrix = Matrix4.CreateTranslation(mesh.Position.ToVector3());
      _modelToWorld = Matrix4.Mult(translationMatrix, Matrix4.Mult(rotationMatrixX, Matrix4.Mult(rotationMatrixY, Matrix4.Mult(rotationMatrixZ, scaleMatrix))));

      var aspectRatio = (float)_scene.Width / (float)_scene.Height;
      Matrix4.CreatePerspectiveFieldOfView((float)_scene.Camera.FieldOfView.ToRadians(), aspectRatio,
          _scene.Camera.NearClip, _scene.Camera.FarClip, out _projectionMatrix);
      _viewMatrix = Matrix4.LookAt(_scene.Camera.Position.ToVector3(), _scene.Camera.Target.ToVector3(), _scene.Camera.Up.ToVector3());
    }

    //Creación, carga, compilación y configuración de los shaders
    private void CreateShaders()
    {
      //Carga de los archivos de shaders
      _pixelShaderSource = File.ReadAllText(PixelShaderFilePath);
      _vertexShaderSource = File.ReadAllText(VertexShaderFilePath);

      //Mediante estos comandos se le está indicando a la GPU que cree lo necesario para almacenar y ocupar estos shaders
      //La GPU entrega un idenitifcador único para estos shaders (aka handles)
      _vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
      _pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

      //De aqui en adelante ocuparemos los handles para indicarle a la GPU lo que querramos hacer con estos shaders
      //El primer paso es enviarle el codigo
      GL.ShaderSource(_vertexShaderHandle, _vertexShaderSource);
      GL.ShaderSource(_pixelShaderHandle, _pixelShaderSource);

      //Una vez enviado el codigo, se debe compilar
      GL.CompileShader(_vertexShaderHandle);
      GL.CompileShader(_pixelShaderHandle);

      //Aca obtenemos la informacion de los errores de compilacion
      Console.WriteLine(@"Vertex Shader Errors:");
      Console.WriteLine(GL.GetShaderInfoLog(_vertexShaderHandle));
      Console.WriteLine(@"Pixel Shader Errors:");
      Console.WriteLine(GL.GetShaderInfoLog(_pixelShaderHandle));

      // Un "program" en terminologia OpenGL es un conjunto de shaders
      _shaderProgramHandle = GL.CreateProgram();

      //Aca agregamos shader al programa
      GL.AttachShader(_shaderProgramHandle, _vertexShaderHandle);
      GL.AttachShader(_shaderProgramHandle, _pixelShaderHandle);

      //Muy importante: más adelante querremos referinos a los atributos del vertex shader con un id, aqui estamos asociando un id a
      //a los nombres de los atributos. Ojo que esto se hace con el handle de programa, no con el del vertex shader
      GL.BindAttribLocation(_shaderProgramHandle, 0, "inPosition");
      GL.BindAttribLocation(_shaderProgramHandle, 1, "inNormal");

      //El linking se encarga de juntar los distintos shaders compilados y hacer los hookups necesarios para coordinar los atributos out del vertex
      //shader con los atributos in del pixel shader
      GL.LinkProgram(_shaderProgramHandle);

      //Con este comando indicamos que para las proximas llamadas de rendering ocuparemos este shader program
      GL.UseProgram(_shaderProgramHandle);

      //Finalmente tenemos que pasar todos los unfiforms que ocuparemos, para lo cual tenemos que obtener la ubicación de estos en el shader
      //y luego copiar la información
      var projectionMatrixLocation = GL.GetUniformLocation(_shaderProgramHandle, "projectionMatrix");
      GL.UniformMatrix4(projectionMatrixLocation, false, ref _projectionMatrix);
      var viewMatrixLocation = GL.GetUniformLocation(_shaderProgramHandle, "viewMatrix");
      GL.UniformMatrix4(viewMatrixLocation, false, ref _viewMatrix);
      var modelToWorldLocation = GL.GetUniformLocation(_shaderProgramHandle, "modelToWorld");
      GL.UniformMatrix4(modelToWorldLocation, false, ref _modelToWorld);
      var cameraPositionLocation = GL.GetUniformLocation(_shaderProgramHandle, "cameraPosition");
      GL.Uniform3(cameraPositionLocation, _scene.Camera.Position.ToVector3());
    }

    //Los vertex buffers corresponde a los espacios de memoria de GPU donde almacenaremos la información por vértice
    //En este caso creamos dos buffers, uno para posiciones y otro para normales y les pasamos sus datos.
    //Necesitamos además un tercer buffer de índices para identificar que vértices conforman los triángulos
    private void CreateVertexBuffers()
    {
      GL.GenBuffers(1, out _positionVboHandle);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _positionVboHandle);
      GL.BufferData(BufferTarget.ArrayBuffer,
          new IntPtr(_positionVboData.Length * Vector3.SizeInBytes),
          _positionVboData, BufferUsageHint.StaticDraw);

      GL.GenBuffers(1, out _normalVboHandle);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _normalVboHandle);
      GL.BufferData(BufferTarget.ArrayBuffer,
          new IntPtr(_normalVboData.Length * Vector3.SizeInBytes),
          _normalVboData, BufferUsageHint.StaticDraw);

      GL.GenBuffers(1, out _eboHandle);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboHandle);
      GL.BufferData(BufferTarget.ElementArrayBuffer,
          new IntPtr(sizeof(uint) * _indicesVboData.Length),
          _indicesVboData, BufferUsageHint.StaticDraw);

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
    }

    //Existe un segundo concepto importante distinto a los vertex buffer que son los vertex array. El vertex array es lo que efectivamente
    //accede la GPU y  sirven de punto de conexion entre vertex buffer y atributos del vertex shader, lo que se configura en este codigo
    private void CreateVertexArrays()
    {
      GL.GenVertexArrays(1, out _vaoHandle);
      GL.BindVertexArray(_vaoHandle);

      GL.BindBuffer(BufferTarget.ArrayBuffer, _positionVboHandle);
      GL.EnableVertexAttribArray(0);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);

      GL.BindBuffer(BufferTarget.ArrayBuffer, _normalVboHandle);
      GL.EnableVertexAttribArray(1);
      GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);

      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboHandle);

      GL.BindVertexArray(0);
    }

    //Este metodo se llama cada vez que la CPU quiere un nuevo rendering 
    protected override void OnRenderFrame(FrameEventArgs e)
    {
      GL.Viewport(0, 0, _scene.Width, _scene.Height); //Indicamos el tamaño y ubicación de la imagen, para la transformacion de Viewport que realizara la GPU
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); //Limpiamos la imagen y el depth buffer (z-buffer)

      GL.BindVertexArray(_vaoHandle); //Indicamos cual es nuestro vertex array
      GL.DrawElements(BeginMode.Triangles, _indicesVboData.Length, DrawElementsType.UnsignedInt, IntPtr.Zero); //Indicamos a la GPU que queremos dibujar los triangulos que configuramos

      //La llamada a DrawElements le indica a la GPU que realice su rendering, pero retorna de inmediato (no espera terminar)
      //Luego de solicitar que se dibuje el proximo frame, la CPU le pide a la GPU que actualice el display con el ultimo frame completado
      SwapBuffers(); 
    }


    private void OnMouseMoved(object sender, MouseMoveEventArgs e)
    {

    }
    
    protected override void OnKeyPress(KeyPressEventArgs e)
    {
     
    }

    [STAThread]
    public static void Main()
    {
      RenderingParameters.Instance.Load();
      using (var window = new MainWindow(RenderingParameters.Instance.ImageWidth, RenderingParameters.Instance.ImageHeight))
      {
        window.Run(RenderingParameters.Instance.FrameRate);
      }
    }
  }
}
