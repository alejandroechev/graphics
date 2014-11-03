using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SceneLib;

namespace Renderer.Base
{
  public abstract class BaseTextureRenderOpenGLManager : BaseOpenGLManager
  {
    protected string _vertexShaderSource = @"
      #version 130
      
      precision highp float;

      in vec3 inPosition;
      out vec2 pixelCoords;
      void main(void)
      {
        gl_Position = vec4(inPosition, 1);
        pixelCoords = vec2((inPosition.x + 1.0)/2.0, (inPosition.y + 1.0)/2.0);
      }";

    protected string _fragmentShaderSource;

    private int _vertexShaderHandle;
    private int _fragmentShaderHandle;
    protected int _shaderProgramHandle;
    private int _vaoHandle;
    private int _positionVboHandle;
    private int _eboHandle;
    
    //private readonly Vector3[] _positionVboData =
    //{
    //  new Vector3(-1.0f, -1.0f,  -1.0f),
    //  new Vector3( 1.0f, -1.0f,  -1.0f),
    //  new Vector3( 1.0f,  1.0f,  -1.0f),
    //  new Vector3(-1.0f,  1.0f,  -1.0f)
    //};
    //
    //private readonly int[] _indicesVboData =
    //{
    //  // front face
    //  0, 1, 2, 2, 3, 0
    //};

    private readonly Vector3[] _positionVboData = new Vector3[]{
            new Vector3(-1.0f, -1.0f,  1.0f),
            new Vector3( 1.0f, -1.0f,  1.0f),
            new Vector3( 1.0f,  1.0f,  1.0f),
            new Vector3(-1.0f,  1.0f,  1.0f),
            new Vector3(-1.0f, -1.0f, -1.0f),
            new Vector3( 1.0f, -1.0f, -1.0f), 
            new Vector3( 1.0f,  1.0f, -1.0f),
            new Vector3(-1.0f,  1.0f, -1.0f) };

    private readonly int[] _indicesVboData = new int[]{
             // front face
                0, 1, 2, 2, 3, 0,
                // top face
                3, 2, 6, 6, 7, 3,
                // back face
                7, 6, 5, 5, 4, 7,
                // left face
                4, 0, 3, 3, 7, 4,
                // bottom face
                0, 1, 5, 5, 4, 0,
                // right face
                1, 5, 6, 6, 2, 1, };


    protected BaseTextureRenderOpenGLManager(Scene scene)
      : base(scene)
    {

    }

    protected abstract void LoadShaders();

    protected abstract void InnerRender(double time);
    

    public override void Load()
    {
      LoadShaders();
      CreateShaders();
      CreateVBOs();
      CreateVAOs();
      
    }

    public override void Render(double time)
    {
      GL.Viewport(0, 0, _scene.Width, _scene.Height);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      InnerRender(time);

      GL.BindVertexArray(_vaoHandle);
      GL.DrawElements(BeginMode.Triangles, _indicesVboData.Length,
          DrawElementsType.UnsignedInt, IntPtr.Zero);
    }


    protected virtual void CreateShaders()
    {
      GL.UseProgram(0);
      _vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
      _fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

      GL.ShaderSource(_vertexShaderHandle, _vertexShaderSource);
      GL.ShaderSource(_fragmentShaderHandle, _fragmentShaderSource);

      GL.CompileShader(_vertexShaderHandle);
      GL.CompileShader(_fragmentShaderHandle);

      // Create program
      _shaderProgramHandle = GL.CreateProgram();

      GL.AttachShader(_shaderProgramHandle, _vertexShaderHandle);
      GL.AttachShader(_shaderProgramHandle, _fragmentShaderHandle);

      GL.LinkProgram(_shaderProgramHandle);
      GL.UseProgram(_shaderProgramHandle);

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

   
   
  }
}