using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using SceneLib;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace Renderer.Base
{
  public class CPUTextureRenderOpenGlManager : BaseTextureRenderOpenGLManager
  {
    private readonly BaseRenderer _renderer;

    private bool _needsRefresh = true;

    private int _textureHandler;
    private int _textureId;
    private bool _textureIsCreated = false;
    private readonly Bitmap _renderTarget;

    public CPUTextureRenderOpenGlManager(Scene scene, IPixelSampler pixelSampler, BaseRenderer renderer) : base(scene)
    {
      if (pixelSampler == null) throw new ArgumentNullException("pixelSampler");
      if (renderer == null) throw new ArgumentNullException("renderer");
      _renderTarget = new Bitmap(_scene.Width, _scene.Height);
      _renderer = renderer;
    }


    public override void Load()
    {
      base.Load();
      _needsRefresh = true;
      if (!_textureIsCreated)
      {
        CreateTexture();
        _textureIsCreated = true;
      }
    }

    public override void Update(double time)
    {
      _renderer.Update(time);
    }

    public override void OnKeyPress(KeyPressEventArgs e)
    {
      base.OnKeyPress(e);
      _needsRefresh = true;
      _renderer.OnKeyPress(e);
      if (e.KeyChar == 'g')
      {
        InvertImage(_renderTarget).Save("render.png", ImageFormat.Png);
        _needsRefresh = false;
      }
     
    }

    public override void OnMouseMoved(MouseMoveEventArgs mouseMoveEventArgs)
    {

    }


    protected override void LoadShaders()
    {
      _fragmentShaderSource = @"
      #version 130
      
      precision highp float;

      uniform sampler2D texture1; 

      in vec2 pixelCoords;
      out vec4 pixelColor;
      
      void main(void)
      {
        vec4 samplerColor = texture(texture1, vec2(pixelCoords));
        pixelColor = samplerColor;
      }";

    }

    protected override void CreateShaders()
    {
      base.CreateShaders();
      _textureHandler = GL.GetUniformLocation(_shaderProgramHandle, "imageTexture");
    }

    protected override void InnerRender(double time)
    {
      if (_needsRefresh)
      {
        _renderer.Render();
        UpdateRenderTarget();
        _needsRefresh = false;
      }
      GL.ActiveTexture(TextureUnit.Texture0);
      GL.BindTexture(TextureTarget.Texture2D, _textureId);
      GL.Uniform1(_textureHandler, 0);
    }


    private void CreateTexture()
    {
      _textureId = GL.GenTexture();
      GL.ActiveTexture(TextureUnit.Texture0);
      GL.BindTexture(TextureTarget.Texture2D, _textureId);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _scene.Width, _scene.Height, 0,
        PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
      GL.BindTexture(TextureTarget.Texture2D, 0);
    }


    private void UpdateRenderTarget()
    {
      GL.BindTexture(TextureTarget.Texture2D, _textureId);

      CopyRenderImageToTarget();
      var bmpData = _renderTarget.LockBits(new Rectangle(0, 0, _renderTarget.Width, _renderTarget.Height),
        ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

      GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, _renderTarget.Width, _renderTarget.Height, PixelFormat.Bgra,
        PixelType.UnsignedByte, bmpData.Scan0);

      _renderTarget.UnlockBits(bmpData);

      GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    private void CopyRenderImageToTarget()
    {
      for (int i = 0; i < _scene.Width; i++)
      {
        for (int j = 0; j < _scene.Height; j++)
        {
          int r = Math.Max(0, Math.Min(255, (int) (255*_renderer.Image[i, j].R)));
          int g = Math.Max(0, Math.Min(255, (int) (255*_renderer.Image[i, j].G)));
          int b = Math.Max(0, Math.Min(255, (int) (255*_renderer.Image[i, j].B)));
          _renderTarget.SetPixel(i, j, Color.FromArgb(r, g, b));
        }
      }

    }

    private Bitmap InvertImage(Bitmap image)
    {
      var inverted = new Bitmap(image.Width, image.Height);
      for (int i = 0; i < image.Width; i++)
      {
        for (int j = 0; j < image.Height; j++)
        {
          inverted.SetPixel(i, image.Height - j - 1, image.GetPixel(i, j));
        }

      }
      return inverted;
    }
  }
}