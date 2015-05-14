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
    public int NumberOfRecursiveRays { get; set; }
    public int NumberOfSamplesPerPixel { get; set; }
    
    private RenderingParameters()
    {
      ScenePath = "Scenes/cornellBox.xml";
      ImageWidth = 256;
      ImageHeight = 256;
      NumberOfRecursiveRays = 3;
      NumberOfSamplesPerPixel = 1;
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
      if (xmlConfig.Elements("parameters").Any())
        NumberOfSamplesPerPixel = XMLHelper.LoadInt(xmlConfig.Elements("parameters").First(), "samplesPerPixel");

    }

  }
}
