using System;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Input;
using EnableCap = OpenTK.Graphics.OpenGL.EnableCap;
using GL = OpenTK.Graphics.OpenGL.GL;
using OpenTK.Graphics.OpenGL;

namespace PixelShader
{
  class MainWindow : GameWindow
  {
    public static int SceneWidth = 512;
    public static int SceneHeight = 512;
    public static float FrameRate = 60;


    private const string PixelShaderFilePath = @"Shaders\RaytracerTarea2.glsl";
    private string _pixelShaderSource;
    private const string VertexShaderSource = @"
      #version 130
      
      precision highp float;

      in vec3 inPosition;
      out vec2 pixelCoords;
      void main(void)
      {
        gl_Position = vec4(inPosition, 1);
        pixelCoords = vec2((inPosition.x + 1.0)/2.0, (inPosition.y + 1.0)/2.0);
      }";

    private int _vertexShaderHandle;
    private int _fragmentShaderHandle;
    private int _shaderProgramHandle;
    private int _vaoHandle;
    private int _positionVboHandle;
    private int _eboHandle;

    private readonly Vector3[] _positionVboData =
    {
      new Vector3(-1.0f, -1.0f,  -1.0f),
      new Vector3( 1.0f, -1.0f,  -1.0f),
      new Vector3( 1.0f,  1.0f,  -1.0f),
      new Vector3(-1.0f,  1.0f,  -1.0f)
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
        new OpenTK.Graphics.GraphicsMode(), "Pixel Shader", GameWindowFlags.Default,
        DisplayDevice.Default, 3, 0,
        OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible | OpenTK.Graphics.GraphicsContextFlags.Debug)
    {

      Mouse.Move += MouseMoved;
    }

    private void MouseMoved(object sender, MouseMoveEventArgs e)
    {
      _mouse[0] = (float)e.X / SceneWidth;
      _mouse[1] = (SceneHeight - (float)e.Y)/SceneHeight;
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
    }

    protected virtual void CreateShaders()
    {
      GL.UseProgram(0);
      _vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
      _fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

      GL.ShaderSource(_vertexShaderHandle, VertexShaderSource);
      GL.ShaderSource(_fragmentShaderHandle, _pixelShaderSource);

      GL.CompileShader(_vertexShaderHandle);
      GL.CompileShader(_fragmentShaderHandle);

      Console.WriteLine(GL.GetShaderInfoLog(_fragmentShaderHandle));

      // Create program
      _shaderProgramHandle = GL.CreateProgram();

      GL.AttachShader(_shaderProgramHandle, _vertexShaderHandle);
      GL.AttachShader(_shaderProgramHandle, _fragmentShaderHandle);

      GL.LinkProgram(_shaderProgramHandle);
      GL.UseProgram(_shaderProgramHandle);

      _timeHandler = GL.GetUniformLocation(_shaderProgramHandle, "time");
      _mouseHandler = GL.GetUniformLocation(_shaderProgramHandle, "mouse");
    }

    private void CreateVBOs()
    {
      GL.GenBuffers(1, out _positionVboHandle);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _positionVboHandle);
      GL.BufferData(BufferTarget.ArrayBuffer,
          new IntPtr(_positionVboData.Length * Vector3.SizeInBytes),
          _positionVboData, BufferUsageHint.StaticDraw);

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

      GL.EnableVertexAttribArray(0);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _positionVboHandle);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
      GL.BindAttribLocation(_shaderProgramHandle, 0, "inPosition");

      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboHandle);

      GL.BindVertexArray(0);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      _time += 1.0f / FrameRate;
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      GL.Viewport(0, 0, SceneWidth, SceneHeight);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      GL.Uniform1(_timeHandler, (float)_time);
      GL.Uniform2(_mouseHandler, 1, _mouse);

      GL.BindVertexArray(_vaoHandle);
      GL.DrawElements(BeginMode.Triangles, _indicesVboData.Length,
          DrawElementsType.UnsignedInt, IntPtr.Zero);
      SwapBuffers();
    }



    [STAThread]
    public static void Main()
    {
      using (var window = new MainWindow(SceneWidth, SceneHeight))
      {
        window.Run(FrameRate);
      }
    }
  }
}
