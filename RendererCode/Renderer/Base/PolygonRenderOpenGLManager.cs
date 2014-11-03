using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using SceneLib;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace Renderer.Base
{
  public class PolygonRenderOpenGLManager : BaseOpenGLManager
  {
    protected string _vertexShaderSource, _fragmentShaderSource;

    private const string _vertexShaderFilePath = @"Shaders\GouraudVertexShader.fx";
    private const string _fragmentShaderFilePath = @"Shaders\GouraudFragmentShader.fx";
  

    private int _vertexShaderHandle;
    private int _fragmentShaderHandle;
    private int _shaderProgramHandle;
    private int _modelviewMatrixLocation;
    private int _projectionMatrixLocation;
    private int _vaoHandle;
    private int _positionVboHandle;
    private int _diffuseColorVboHandle;
    private int _specularColorVboHandle;
    private int _normalVboHandle;
    private int _textCoordsVboHandle;
    private int _eboHandle;

    private int _diffuseTextureHandler;
    private int _diffuseTextureId;
    private int _normalMapHandler;
    private int _normalMapId;

    private Vector3[] _positionVboData;
    private Vector3[] _diffuseColorVboData;
    private Vector3[] _normalVboData;
    private Vector3[] _specularColorVboData;
    private Vector3[] _texCoordsColorVboData;

    private int[] _indicesVboData;

    private Matrix4 _projectionMatrix, _viewMatrix;

    private bool _usePerspective = true;
    private bool _showWireframe = false;
    private bool _transformNormals = true;
    private bool _correctPerspective = true;
    private bool _depthTest = true;

    private int _timeHandler;

    public PolygonRenderOpenGLManager(Scene scene)
      : base(scene)
    {
    }

    public override void Load()
    {
      LoadShaders();
      CreateShaders();
      LoadObjects();
      CreateVBOs();
      CreateVAOs();
    }


    public override void Render(double time)
    {
      GL.Viewport(0, 0, _scene.Width, _scene.Height);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      GL.PolygonMode(MaterialFace.FrontAndBack, _showWireframe ? PolygonMode.Line : PolygonMode.Fill);

      GL.Uniform1(_timeHandler, (float)time);
      GL.ActiveTexture(TextureUnit.Texture0);
      GL.BindTexture(TextureTarget.Texture2D, _diffuseTextureId);
      GL.Uniform1(_diffuseTextureHandler, 0);

      GL.ActiveTexture(TextureUnit.Texture1);
      GL.BindTexture(TextureTarget.Texture2D, _normalMapId);
      GL.Uniform1(_normalMapHandler, 1);

      GL.BindVertexArray(_vaoHandle);
      GL.DrawElements(BeginMode.Triangles, _indicesVboData.Length,
          DrawElementsType.UnsignedInt, IntPtr.Zero);


    }

    public override void Update(double time)
    {

    }

    public override void OnKeyPress(KeyPressEventArgs e)
    {
      base.OnKeyPress(e);
      if (e.KeyChar == 'p')
        _usePerspective = !_usePerspective;
      if (e.KeyChar == 'o')
        _showWireframe = !_showWireframe;
      if (e.KeyChar == 'n')
      {
        _transformNormals = !_transformNormals;
        Load();
      }
      UpdateDynamicUniformsInShaders();
    }

    private void LoadShaders()
    {
      _fragmentShaderSource = File.ReadAllText(_fragmentShaderFilePath);
      _vertexShaderSource = File.ReadAllText(_vertexShaderFilePath);
    }

    private void UpdateDynamicUniformsInShaders()
    {
      var aspectRatio = (float)_scene.Width / (float)_scene.Height;
      if (_usePerspective)
      {
        Matrix4.CreatePerspectiveFieldOfView((float) _scene.Camera.FieldOfView.ToRadians(), aspectRatio,
          _scene.Camera.NearClip, _scene.Camera.FarClip, out _projectionMatrix);
      }
      else
      {
        var bounds = _scene.Camera.FrustumBounds(_scene.Width, _scene.Height);
        Matrix4.CreateOrthographic(bounds[Bounds.Right] - bounds[Bounds.Left], bounds[Bounds.Top] - bounds[Bounds.Bottom], //  2* Math.Abs(_scene.Camera.Position.Z), 2*Math.Abs(_scene.Camera.Position.Z),
          _scene.Camera.NearClip, _scene.Camera.FarClip, out _projectionMatrix);
      }
      _projectionMatrixLocation = GL.GetUniformLocation(_shaderProgramHandle, "projectionMatrix");
      GL.UniformMatrix4(_projectionMatrixLocation, false, ref _projectionMatrix);

      _viewMatrix = Matrix4.LookAt(_scene.Camera.Position.ToVector3(), _scene.Camera.Target.ToVector3(), _scene.Camera.Up.ToVector3());
      _modelviewMatrixLocation = GL.GetUniformLocation(_shaderProgramHandle, "viewMatrix");
      GL.UniformMatrix4(_modelviewMatrixLocation, false, ref _viewMatrix);
      var eyeLocation = GL.GetUniformLocation(_shaderProgramHandle, "eye");
      GL.Uniform3(eyeLocation, _scene.Camera.Position.ToVector3());

      for (int index = 0; index < 1; index++)
      {
        var lightPositionLocation = GL.GetUniformLocation(_shaderProgramHandle, "light" + index + ".position");
        var lightColorLocation = GL.GetUniformLocation(_shaderProgramHandle, "light" + index + ".color");
        var lightPosition = index < _scene.Lights.Count ? _scene.Lights[index].Position.ToVector3() : new Vector3(0, 0, 0);
        var lightColor = index < _scene.Lights.Count ? _scene.Lights[index].Color.ToVector3() : new Vector3(0, 0, 0);

        GL.Uniform3(lightPositionLocation, lightPosition);
        GL.Uniform3(lightColorLocation, lightColor);

      }
    }

    private int CreateTexture(Bitmap texture, TextureUnit unit, TextureMinFilter minFilter, TextureMagFilter magFilter)
    {
      int textureId = GL.GenTexture();
      GL.ActiveTexture(unit);
      GL.BindTexture(TextureTarget.Texture2D, textureId);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texture.Width, texture.Height, 0,
        PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);

      var bmpData = texture.LockBits(new Rectangle(0, 0, texture.Width, texture.Height),
        ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

      GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, texture.Width, texture.Height, PixelFormat.Bgra,
        PixelType.UnsignedByte, bmpData.Scan0);

      texture.UnlockBits(bmpData);
     
      GL.BindTexture(TextureTarget.Texture2D, 0);

      return textureId;
    }

    private void LoadObjects()
    {
      var positionList = new List<Vector3>();
      var diffuseColorList = new List<Vector3>();
      var normalList = new List<Vector3>();
      var specularColorList = new List<Vector3>();
      var texCoordsList = new List<Vector3>();
      var indexList = new List<int>();
      var index = 0;
      foreach (var renderObject in _scene.Objects)
      {
        //_diffuseTextureId = CreateTexture((renderObject as Sphere).Material.DiffuseTexture, TextureUnit.Texture0, TextureMinFilter.Linear, TextureMagFilter.Linear);
        //_normalMapId = CreateTexture((renderObject as Sphere).Material.NormalTexture, TextureUnit.Texture1, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
        if (renderObject is IHavePolygons)
        {
          var iHavePolygons = renderObject as IHavePolygons;
          var polygons = iHavePolygons.Polygons;
          foreach (var polygon in polygons)
          {
            var scaleMatrix = Matrix4.Scale(polygon.Scale.ToVector3());
            var rotationMatrixX = Matrix4.CreateRotationX((float)(polygon.Rotation.X * Math.PI / 180.0f));
            var rotationMatrixY = Matrix4.CreateRotationY((float)(polygon.Rotation.Y * Math.PI / 180.0f));
            var rotationMatrixZ = Matrix4.CreateRotationZ((float)(polygon.Rotation.Z * Math.PI / 180.0f));
            var translationMatrix = Matrix4.CreateTranslation(polygon.Position.ToVector3());
            
            var positions = polygon.PositionsToVector3List();
            var transformedPosition = new List<Vector3>();
            foreach (var position in positions)
            {
              var scaledPosition = Vector3.Transform(position, scaleMatrix);
              var rotatedPosition = Vector3.Transform(scaledPosition, rotationMatrixZ);
              rotatedPosition = Vector3.Transform(rotatedPosition, rotationMatrixY);
              rotatedPosition = Vector3.Transform(rotatedPosition, rotationMatrixX);
              var translatedPosition = Vector3.Transform(rotatedPosition, translationMatrix);
              transformedPosition.Add(translatedPosition);
            }

            var normals = polygon.NormalsToVector3List();
            var transformedNormals = new List<Vector3>();
            foreach (var normal in normals)
            {
              if (_transformNormals)
              {
                var scaledNormal = Vector3.Transform(normal, scaleMatrix);
                var rotatedNormal = Vector3.Transform(scaledNormal, rotationMatrixZ);
                rotatedNormal = Vector3.Transform(rotatedNormal, rotationMatrixY);
                rotatedNormal = Vector3.Transform(rotatedNormal, rotationMatrixX);
                //var translatedPosition = Vector3.Transform(rotatedPosition, translationMatrix);
                transformedNormals.Add(rotatedNormal);
              }
              else
              {
                transformedNormals.Add(normal);
              }
            }

            positionList.AddRange(transformedPosition);
            diffuseColorList.AddRange(polygon.DiffuseColorsToVector3List());
            normalList.AddRange(transformedNormals);
            specularColorList.AddRange(polygon.SpecularColorsToVector3List());
            texCoordsList.AddRange(polygon.TextureCoordinatesToVector3List());
            indexList.AddRange(new[] { index, index + 1, index + 2 });
            index += 3;
          }
        }
      }
      _positionVboData = positionList.ToArray();
      _diffuseColorVboData = diffuseColorList.ToArray();
      _normalVboData = normalList.ToArray();
      _specularColorVboData = specularColorList.ToArray();
      _texCoordsColorVboData = texCoordsList.ToArray();
      _indicesVboData = indexList.ToArray();
    }

    private void CreateShaders()
    {
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

      GL.BindAttribLocation(_shaderProgramHandle, 0, "inPosition");
      GL.BindAttribLocation(_shaderProgramHandle, 1, "inDiffuseColor");
      GL.BindAttribLocation(_shaderProgramHandle, 2, "inNormal");
      GL.BindAttribLocation(_shaderProgramHandle, 3, "inSpecularColor");
      GL.BindAttribLocation(_shaderProgramHandle, 4, "inTexCoords");

      GL.LinkProgram(_shaderProgramHandle);
      GL.UseProgram(_shaderProgramHandle);

      // Set uniforms
      
      UpdateDynamicUniformsInShaders();

      var backgroundColorLocation = GL.GetUniformLocation(_shaderProgramHandle, "backgroundColor");
      GL.Uniform3(backgroundColorLocation, _scene.BackgroundColor.ToVector3());
      var ambientColorLocation = GL.GetUniformLocation(_shaderProgramHandle, "ambientColor");
      GL.Uniform3(ambientColorLocation, _scene.AmbientLight.ToVector3());

      _diffuseTextureHandler = GL.GetUniformLocation(_shaderProgramHandle, "diffuseTexture");
      _normalMapHandler = GL.GetUniformLocation(_shaderProgramHandle, "normalMap");
      _timeHandler = GL.GetUniformLocation(_shaderProgramHandle, "time");

    }

    private void CreateVBOs()
    {
      GL.GenBuffers(1, out _positionVboHandle);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _positionVboHandle);
      GL.BufferData(BufferTarget.ArrayBuffer,
          new IntPtr(_positionVboData.Length * Vector3.SizeInBytes),
          _positionVboData, BufferUsageHint.StaticDraw);

      GL.GenBuffers(1, out _diffuseColorVboHandle);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _diffuseColorVboHandle);
      GL.BufferData(BufferTarget.ArrayBuffer,
          new IntPtr(_diffuseColorVboData.Length * Vector3.SizeInBytes),
          _diffuseColorVboData, BufferUsageHint.StaticDraw);

      GL.GenBuffers(1, out _normalVboHandle);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _normalVboHandle);
      GL.BufferData(BufferTarget.ArrayBuffer,
          new IntPtr(_normalVboData.Length * Vector3.SizeInBytes),
          _normalVboData, BufferUsageHint.StaticDraw);

      GL.GenBuffers(1, out _specularColorVboHandle);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _specularColorVboHandle);
      GL.BufferData(BufferTarget.ArrayBuffer,
          new IntPtr(_specularColorVboData.Length * Vector3.SizeInBytes),
          _specularColorVboData, BufferUsageHint.StaticDraw);

      GL.GenBuffers(1, out _textCoordsVboHandle);
      GL.BindBuffer(BufferTarget.ArrayBuffer, _textCoordsVboHandle);
      GL.BufferData(BufferTarget.ArrayBuffer,
          new IntPtr(_texCoordsColorVboData.Length * Vector3.SizeInBytes),
          _texCoordsColorVboData, BufferUsageHint.StaticDraw);

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
      // GL3 allows us to store the vertex layout in a "vertex array object" (VAO).
      // This means we do not have to re-issue VertexAttribPointer calls
      // every time we try to use a different vertex layout - these calls are
      // stored in the VAO so we simply need to bind the correct VAO.
      GL.GenVertexArrays(1, out _vaoHandle);
      GL.BindVertexArray(_vaoHandle);

      GL.BindBuffer(BufferTarget.ArrayBuffer, _positionVboHandle);
      //int pos = GL.GetAttribLocation(_shaderProgramHandle, "inPosition");
      GL.EnableVertexAttribArray(0);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);

      GL.BindBuffer(BufferTarget.ArrayBuffer, _diffuseColorVboHandle);
      //int color = GL.GetAttribLocation(_shaderProgramHandle, "inDiffuseColor");
      GL.EnableVertexAttribArray(1);
      GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);

      GL.BindBuffer(BufferTarget.ArrayBuffer, _normalVboHandle);
      //int normal = GL.GetAttribLocation(_shaderProgramHandle, "inNormal");
      GL.EnableVertexAttribArray(2);
      GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);

      GL.BindBuffer(BufferTarget.ArrayBuffer, _specularColorVboHandle);
      //int specular = GL.GetAttribLocation(_shaderProgramHandle, "inSpecularColor");
      GL.EnableVertexAttribArray(3);
      GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);

      GL.BindBuffer(BufferTarget.ArrayBuffer, _textCoordsVboHandle);
      //int texCoords = GL.GetAttribLocation(_shaderProgramHandle, "inTexCoords");
      GL.EnableVertexAttribArray(4);
      GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);

      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboHandle);

      GL.BindVertexArray(0);
    }
  }
}