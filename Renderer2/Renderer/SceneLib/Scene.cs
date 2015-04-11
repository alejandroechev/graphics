using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Drawing;

namespace SceneLib
{
  /// <summary>
  /// Represents a Scene to be rendered
  /// </summary>
  public class Scene
  {
    private readonly IRenderObjectFactory _renderObjectFactory;
  
    private readonly int _width;
    private readonly int _height;

    private readonly RenderingParameters _parameters = RenderingParameters.Instance;
    private readonly Dictionary<string, Material> _materialsTable = new Dictionary<string, Material>();
    private readonly List<RenderObject> _objects = new List<RenderObject>();
    private readonly List<Light> _lights = new List<Light>();
    private readonly List<Camera> _cameras = new List<Camera>();
    private Camera _camera;

    private Vector _backgroundColor;
    private Vector _ambientLight;

    public int Width
    {
      get { return _width; }
    }

    public int Height
    {
      get { return _height; }
    }

    public List<RenderObject> Objects
    {
      get { return _objects; }
    }

    public List<Light> Lights
    {
      get { return _lights; }
    }

    public List<Camera> Cameras
    {
      get { return _cameras; }
    }

    public Camera Camera
    {
      get { return _camera; }
    }

    public Vector BackgroundColor
    {
      get { return _backgroundColor; }
    }

    public Vector AmbientLight
    {
      get { return _ambientLight; }
    }

    public RenderingParameters Parameters
    {
      get { return _parameters; }
    }


    public Scene(int width, int height, IRenderObjectFactory renderObjectFactory)
    {
      if (renderObjectFactory == null) throw new ArgumentNullException("renderObjectFactory");
      _width = width;
      _height = height;
      _renderObjectFactory = renderObjectFactory;
    }

    public void NextCamera()
    {
      var index = _cameras.IndexOf(_camera);
      var nextIndex = (index + 1) % _cameras.Count;
      _camera = _cameras[nextIndex];
    }

    //Loads a scene from an XML file
    public void Load(string file)
    {
      var xmlDoc = XDocument.Load(file);
      var xmlScene = xmlDoc.Elements("scene").First();

      LoadBackground(xmlScene);
      LoadCameras(xmlScene);
      LoadLights(xmlScene);
      LoadMaterials(xmlScene);
      LoadObjects(xmlScene);
    }

    private void LoadObjects(XElement xmlScene)
    {
      var xmlObjects = xmlScene.Elements("object_list").First();
      LoadModels(xmlObjects);
      LoadTriangles(xmlObjects);
      LoadSpheres(xmlObjects);
    }

    private void LoadSpheres(XElement xmlObjects)
    {
      foreach (var sphereNode in xmlObjects.Elements("sphere"))
      {
        var sphere = CreateSphere(sphereNode);
        _objects.Add(sphere);
      }
    }

    private SphereBase CreateSphere(XElement sphereNode)
    {
      var sphere = _renderObjectFactory.CreateSphere();
      sphere.Radius = XMLHelper.LoadFloat(sphereNode, "radius");
      sphere.Material = _materialsTable[sphereNode.Attribute("material").Value];
      sphere.Scale = XMLHelper.LoadXYZ(sphereNode.Elements("scale").First());
      sphere.Position = XMLHelper.LoadXYZ(sphereNode.Elements("position").First());
      sphere.Rotation = XMLHelper.LoadXYZ(sphereNode.Elements("rotation").First());
      sphere.Center = XMLHelper.LoadXYZ(sphereNode.Elements("center").First());
      if ((sphereNode.Elements("velocity").Any()))
        sphere.Velocity = XMLHelper.LoadXYZ(sphereNode.Elements("velocity").First());
      sphere.Load();
      return sphere;
    }

    private void LoadTriangles(XElement xmlObjects)
    {
      foreach (var triangleNode in xmlObjects.Elements("triangle"))
      {
        var triangle = CreateTriangle(triangleNode);
        _objects.Add(triangle);
      }
    }

    private void LoadModels(XElement xmlObjects)
    {
      foreach (var modelNode in xmlObjects.Elements("mesh"))
      {
        var model =_renderObjectFactory.CreateMesh(modelNode.Attribute("name").Value, modelNode.Attribute("path").Value);
        model.Material = _materialsTable[modelNode.Attribute("material").Value];
        model.Scale = XMLHelper.LoadXYZ(modelNode.Elements("scale").First());
        model.Position = XMLHelper.LoadXYZ(modelNode.Elements("position").First());
        model.Rotation = XMLHelper.LoadXYZ(modelNode.Elements("rotation").First());
        model.Load();
        _objects.Add(model);
      }
    }

    private TriangleBase CreateTriangle(XElement triangleNode)
    {
      var triangle = _renderObjectFactory.CreateTriangle();
      triangle.Scale = XMLHelper.LoadXYZ(triangleNode.Elements("scale").First());
      triangle.Position = XMLHelper.LoadXYZ(triangleNode.Elements("position").First());
      triangle.Rotation = XMLHelper.LoadXYZ(triangleNode.Elements("rotation").First());

      foreach (var vertexNode in triangleNode.Elements("vertex"))
      {
        var vertex = new Vertex();
        vertex.Material = _materialsTable[vertexNode.Attribute("material").Value];
        vertex.Position = XMLHelper.LoadXYZ(vertexNode.Elements("position").First());
        vertex.Normal = XMLHelper.LoadXYZ(vertexNode.Elements("normal").First());
        var textNode = vertexNode.Elements("texture").First();
        vertex.TextureCoordinates.U = XMLHelper.LoadFloat(textNode, "u");
        vertex.TextureCoordinates.V = XMLHelper.LoadFloat(textNode, "v");
        triangle.AddVertex(vertex);
      }
      return triangle;
    }

    private void LoadMaterials(XElement xmlScene)
    {
      var xmlMaterials = xmlScene.Elements("material_list").First();
      foreach (var materialNode in xmlMaterials.Elements("material"))
      {
        AddMaterial(materialNode);
      }
    }

    private void LoadLights(XElement xmlScene)
    {
      var xmlLights = xmlScene.Elements("light_list").First();
      foreach (var lightNode in xmlLights.Elements("light"))
      {
        AddLight(lightNode);
      }
    }

    private void LoadCameras(XElement xmlScene)
    {
      var xmlCameras = xmlScene.Elements("camera_list").First();
      foreach (var xmlCamera in xmlCameras.Elements("camera"))
      {
        AddCamera(xmlCamera);
      }
      _camera = _cameras.First();
    }

    private void LoadBackground(XElement xmlScene)
    {
      var xmlBackground = xmlScene.Elements("background").First();
      _ambientLight = XMLHelper.LoadColor(xmlBackground.Elements("ambientLight").First());
      _backgroundColor = XMLHelper.LoadColor(xmlBackground.Elements("color").First());
    }

    private void AddMaterial(XElement materialNode)
    {
      var name = materialNode.Attribute("name").Value;
      var material = new Material();
      if (materialNode.Elements("diffuseTexture").Any())
      {
        var diffuseTextureFile = materialNode.Elements("diffuseTexture").First().Attribute("filename").Value;
        if (string.IsNullOrEmpty(diffuseTextureFile) || !File.Exists(diffuseTextureFile)) throw new ApplicationException("diffuse texture of material " + name + " is invalid");
        material.DiffuseTexture = (Bitmap)Image.FromFile(diffuseTextureFile);
      }
      if (materialNode.Elements("specular").Any())
      {
        material.Specular = XMLHelper.LoadSpecular(materialNode.Elements("specular").First());
        material.Shininess = material.Specular.W;
        material.Specular.W = 1;
      }
      if (materialNode.Elements("diffuse").Any())
        material.Diffuse = XMLHelper.LoadColor(materialNode.Elements("diffuse").First());
      if (materialNode.Elements("reflective").Any())
        material.ReflectivityAttenuation = XMLHelper.LoadFloat(materialNode.Elements("reflective").First(), "attenuation");
      if (materialNode.Elements("refractive").Any())
        material.RefractiveIndex = XMLHelper.LoadFloat(materialNode.Elements("refractive").First(), "index");
      if (materialNode.Elements("refractive").Any())
        material.RefractiveAttenuation = XMLHelper.LoadFloat(materialNode.Elements("refractive").First(), "attenuation");
      _materialsTable.Add(name, material);
    }

    private void AddLight(XElement lightNode)
    {
      var lightObj = new Light();
      lightObj.Position = XMLHelper.LoadXYZ(lightNode.Elements("position").First());
      lightObj.Color = XMLHelper.LoadColor(lightNode.Elements("color").First());
      lightObj.Size = XMLHelper.LoadFloat(lightNode.Elements("position").First(), "size");
      _lights.Add(lightObj);
    }

    private void AddCamera(XElement xmlCamera)
    {
      var camera = new Camera();
      camera.FieldOfView = XMLHelper.LoadFloat(xmlCamera, "fieldOfView");
      camera.NearClip = XMLHelper.LoadFloat(xmlCamera, "nearClip");
      camera.FarClip = XMLHelper.LoadFloat(xmlCamera, "farClip");
      camera.LensSize = XMLHelper.LoadFloat(xmlCamera, "lensSize");
      camera.Position = XMLHelper.LoadXYZ(xmlCamera.Elements("position").First());
      camera.Target = XMLHelper.LoadXYZ(xmlCamera.Elements("target").First());
      camera.Up = XMLHelper.LoadXYZ(xmlCamera.Elements("up").First());
      _cameras.Add(camera);
    }

   
  }
}
