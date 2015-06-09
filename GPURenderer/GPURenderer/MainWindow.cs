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
  class MainWindow : GameWindow
  {
    private readonly Scene _scene;

    private string _vertexShaderSource, _pixelShaderSource;
    private const string VertexShaderFilePath = @"Shaders\VertexShader.glsl";
    private const string PixelShaderFilePath = @"Shaders\PixelShader.glsl";

    private int _vertexShaderHandle;
    private int _pixelShaderHandle;
    private int _shaderProgramHandle;
    private int _vaoHandle;
    private int _positionVboHandle;
    private int _normalVboHandle;
    private int _eboHandle;

    private Vector3[] _positionVboData;
    private Vector3[] _normalVboData;

    private int[] _indicesVboData;

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

    protected override void OnLoad(EventArgs e)
    {
      OpenGLConfiguration();
      RenderSetup();
      LoadShaders();
      CreateShaders();
      CreateVBOs();
      CreateVAOs();
    }

    private void OpenGLConfiguration()
    {
      VSync = VSyncMode.On;
      GL.Enable(EnableCap.DepthTest);
      GL.Enable(EnableCap.Texture2D);
      GL.ClearColor(_scene.BackgroundColor.ToColor4());
    }

    private void LoadShaders()
    {
      _pixelShaderSource = File.ReadAllText(PixelShaderFilePath);
      _vertexShaderSource = File.ReadAllText(VertexShaderFilePath);
    }


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

    private void CreateShaders()
    {
      _vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
      _pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

      GL.ShaderSource(_vertexShaderHandle, _vertexShaderSource);
      GL.ShaderSource(_pixelShaderHandle, _pixelShaderSource);

      GL.CompileShader(_vertexShaderHandle);
      GL.CompileShader(_pixelShaderHandle);

      Console.WriteLine(@"Vertex Shader Errors:");
      Console.WriteLine(GL.GetShaderInfoLog(_vertexShaderHandle));
      Console.WriteLine(@"Pixel Shader Errors:");
      Console.WriteLine(GL.GetShaderInfoLog(_pixelShaderHandle));

      // Create program
      _shaderProgramHandle = GL.CreateProgram();

      GL.AttachShader(_shaderProgramHandle, _vertexShaderHandle);
      GL.AttachShader(_shaderProgramHandle, _pixelShaderHandle);

      GL.BindAttribLocation(_shaderProgramHandle, 0, "inPosition");
      GL.BindAttribLocation(_shaderProgramHandle, 1, "inNormal");

      GL.LinkProgram(_shaderProgramHandle);
      GL.UseProgram(_shaderProgramHandle);

      // Update

      var projectionMatrixLocation = GL.GetUniformLocation(_shaderProgramHandle, "projectionMatrix");
      GL.UniformMatrix4(projectionMatrixLocation, false, ref _projectionMatrix);
      var viewMatrixLocation = GL.GetUniformLocation(_shaderProgramHandle, "viewMatrix");
      GL.UniformMatrix4(viewMatrixLocation, false, ref _viewMatrix);
      var modelToWorldLocation = GL.GetUniformLocation(_shaderProgramHandle, "modelToWorld");
      GL.UniformMatrix4(modelToWorldLocation, false, ref _modelToWorld);
      var cameraPositionLocation = GL.GetUniformLocation(_shaderProgramHandle, "cameraPosition");
      GL.Uniform3(cameraPositionLocation, _scene.Camera.Position.ToVector3());
      var ambientColorLocation = GL.GetUniformLocation(_shaderProgramHandle, "ambientColor");
      GL.Uniform3(ambientColorLocation, _scene.AmbientLight.ToVector3());

    }

    private void CreateVBOs()
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

    private void CreateVAOs()
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

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      GL.Viewport(0, 0, _scene.Width, _scene.Height);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

      GL.BindVertexArray(_vaoHandle);
      GL.DrawElements(BeginMode.Triangles, _indicesVboData.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
      
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
