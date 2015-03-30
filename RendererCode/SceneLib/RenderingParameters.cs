using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SceneLib
{

  public class RenderingParameters
  {
    private const string _configFileName = "config.xml";

    private static readonly RenderingParameters instance = new RenderingParameters();
    static RenderingParameters() { }

    public static RenderingParameters Instance
    {
      get { return instance; }
    }

    public string ScenePath { get; set; }
    public int ImageWidth { get; set; }
    public int ImageHeight { get; set; }
    public int FrameRate { get; set; }
    public int SamplesPerPixel { get; set; }
    public bool MultiSampleTexture { get; set; }
    public bool EnableParallelism { get; set; }
    public bool EnableSpecular { get; set; }
    public bool EnableAmbient { get; set; }
    public bool EnableShadows { get; set; }
    public bool EnableReflections { get; set; }
    public bool EnableRefractions { get; set; }
    public int NumberOfRecursiveRays { get; set; }
    public bool EnableTextureMapping { get; set; }
    public float MinimumTime { get; set; }
    public float MaximumTime { get; set; }
    public float FocalDistance { get; set; }
    public float LensSize { get; set; }
    public bool EnableShading { get; set; }

    private RenderingParameters()
    {
      ScenePath = "Scenes/cornellBox.xml";
      ImageWidth = 256;
      ImageHeight = 256;
      FrameRate = 30;
      SamplesPerPixel = 1;
      EnableParallelism = true;
      MultiSampleTexture = false;

      EnableShadows = false;
      EnableReflections = true;
      EnableRefractions = true;
      NumberOfRecursiveRays = 6;
      EnableTextureMapping = true;
      MinimumTime = 0;
      MaximumTime = 0;
      FocalDistance = 0;
      LensSize = 0;
      EnableShading = false;
      EnableSpecular = true;
      EnableAmbient = true;
    }

    public void Load()
    {
      if (!File.Exists(_configFileName))
        return;

      var xmlDoc = XDocument.Load(_configFileName);
      var xmlConfig = xmlDoc.Elements("config").First();

      ScenePath = XMLHelper.LoadString(xmlConfig.Elements("scene").First(), "path");
      ImageWidth = XMLHelper.LoadInt(xmlConfig.Elements("window").First(), "width");
      ImageHeight = XMLHelper.LoadInt(xmlConfig.Elements("window").First(), "height");
      FrameRate = XMLHelper.LoadInt(xmlConfig.Elements("window").First(), "frameRate");
      SamplesPerPixel = XMLHelper.LoadInt(xmlConfig.Elements("rendering").First(), "samplesPerPixel");
      MultiSampleTexture = XMLHelper.LoadBool(xmlConfig.Elements("rendering").First(), "multiSampleTexture");
      MinimumTime = XMLHelper.LoadFloat(xmlConfig.Elements("rendering").First(), "minimumTime");
      MaximumTime = XMLHelper.LoadFloat(xmlConfig.Elements("rendering").First(), "maximumTime");
      FocalDistance = XMLHelper.LoadFloat(xmlConfig.Elements("rendering").First(), "focalDistance");
      LensSize = XMLHelper.LoadFloat(xmlConfig.Elements("rendering").First(), "lensSize");
      EnableParallelism = XMLHelper.LoadBool(xmlConfig.Elements("optimizations").First(), "parallelism");

      
    }

  }
}
