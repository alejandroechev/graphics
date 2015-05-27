using System;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Input;
using EnableCap = OpenTK.Graphics.OpenGL.EnableCap;
using GL = OpenTK.Graphics.OpenGL.GL;
using OpenTK.Graphics.OpenGL;

namespace ThreeDU
{
  class MainWindow : GameWindow
  {
    public static int WindowWidth = 512;
    public static int WindowHeight = 512;
    public static float FrameRate = 60;

    private string PixelShaderFilePath = Path.Combine("Shaders", "PerVertexLightingFragmentShader.glsl");
    private string _pixelShaderSource;
    private string VertexShaderFilePath = Path.Combine("Shaders", "PerVertexLightingVertexShader.glsl");
    private string _vertexShaderSource;
    
    private int _vertexShaderHandle;
    private int _fragmentShaderHandle;
    private int _shaderProgramHandle;
    private int _vaoHandle;
    private int _positionVboHandle;
    private int _colorVboHandle;
    private int _eboHandle;

    private readonly Vector3[] _positionVboData =
    {
      new Vector3(-1.0f, -1.0f,  -1.0f),
      new Vector3( 1.0f, -1.0f,  -1.0f),
      new Vector3( 1.0f,  1.0f,  -1.0f),
      new Vector3(-1.0f,  1.0f,  -1.0f)
    };

    private readonly Vector3[] _colorVboData =
    {
      new Vector3( 1.0f, 0.0f,  0.0f),
      new Vector3( 0.0f, 1.0f,  0.0f),
      new Vector3( 0.0f, 0.0f,  1.0f),
      new Vector3( 1.0f, 1.0f,  0.0f)
    };

    private readonly int[] _indicesVboData =
    {
      // front face
      0, 1, 2, 2, 3, 0
    };

    private int _timeHandler;
    private int _mouseHandler;
    private readonly float[] _mouse = new float[2];
    private float _time = 0;


    public MainWindow(int width, int height)
      : base(width, height,
        new OpenTK.Graphics.GraphicsMode(), "ThreeDU", GameWindowFlags.Default,
        DisplayDevice.Default, 3, 0,
        OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible | OpenTK.Graphics.GraphicsContextFlags.Debug)
    {
      Mouse.Move += MouseMoved;
    }

    private void MouseMoved(object sender, MouseMoveEventArgs e)
    {
      _mouse[0] = (float)e.X / this.Width;
      _mouse[1] = (WindowHeight - (float)e.Y)/this.Height;
    }

    protected override void OnLoad(EventArgs e)
    {
      VSync = VSyncMode.On;
      GL.Enable(EnableCap.DepthTest);
      GL.Enable(EnableCap.Texture2D);
      GL.ClearColor(Color.AliceBlue);

      LoadShaders();
      CreateShaders();
      CreateVBOs();
      CreateVAOs();
    }

    private void LoadShaders()
    {
      _pixelShaderSource = File.ReadAllText(PixelShaderFilePath);
      _vertexShaderSource = File.ReadAllText(VertexShaderFilePath);
    }

    protected virtual void CreateShaders()
    {
      GL.UseProgram(0);
      _vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
      _fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

      GL.ShaderSource(_vertexShaderHandle, _vertexShaderSource);
      GL.ShaderSource(_fragmentShaderHandle, _pixelShaderSource);

      GL.CompileShader(_vertexShaderHandle);
      GL.CompileShader(_fragmentShaderHandle);

      Console.WriteLine(GL.GetShaderInfoLog(_fragmentShaderHandle));

      // Create program
      _shaderProgramHandle = GL.CreateProgram();

      GL.AttachShader(_shaderProgramHandle, _vertexShaderHandle);
      GL.AttachShader(_shaderProgramHandle, _fragmentShaderHandle);

      GL.BindAttribLocation(_shaderProgramHandle, 0, "inPosition");
      GL.BindAttribLocation(_shaderProgramHandle, 1, "inDiffuseColor");


      GL.LinkProgram(_shaderProgramHandle);
      GL.UseProgram(_shaderProgramHandle);

      //_timeHandler = GL.GetUniformLocation(_shaderProgramHandle, "time");
      //_mouseHandler = GL.GetUniformLocation(_shaderProgramHandle, "mouse");
    }

    private void CreateVBOs()
    {
      GL.GenBuffers(1, out _positionVboHandle);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _positionVboHandle);
      GL.BufferData(BufferTarget.ArrayBuffer,
          new IntPtr(_positionVboData.Length * Vector3.SizeInBytes),
          _positionVboData, BufferUsageHint.StaticDraw);

      GL.GenBuffers(1, out _colorVboHandle);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _colorVboHandle);
      GL.BufferData(BufferTarget.ArrayBuffer,
          new IntPtr(_colorVboData.Length * Vector3.SizeInBytes),
          _colorVboData, BufferUsageHint.StaticDraw);

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

      GL.BindBuffer(BufferTarget.ArrayBuffer, _colorVboHandle);
      GL.EnableVertexAttribArray(1);
      GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
      
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboHandle);

      GL.BindVertexArray(0);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      _time += 1.0f / FrameRate;
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      GL.Viewport(0, 0, this.Width, this.Height);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      //GL.Uniform1(_timeHandler, (float)_time);
      //GL.Uniform2(_mouseHandler, 1, _mouse);

      GL.BindVertexArray(_vaoHandle);
      GL.DrawElements(BeginMode.Triangles, _indicesVboData.Length,
          DrawElementsType.UnsignedInt, IntPtr.Zero);
      SwapBuffers();
    }



    [STAThread]
    public static void Main()
    {
      using (var window = new MainWindow(WindowWidth, WindowHeight))
      {
        window.Run(FrameRate);
      }
    }
  }
}
