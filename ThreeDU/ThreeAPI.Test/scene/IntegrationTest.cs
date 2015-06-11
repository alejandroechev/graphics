using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using ThreeAPI.scene;

namespace ThreeAPI.Test.scene
{
  [TestFixture()]
  public class IntegrationTest
  {
    [Test()]
    public void TestSceneGraph()
    {
      var testXml = @"<Scene>
                        <Scale x='2' y='2' z='2'>
                           <Rotate x='1' y='0' z='0' angle='90'>
                            <Translate x='10' y='10' z='10'>
                            </Translate>
                           </Rotate>                        
                        </Scale>                          
                      </Scene>";
      File.WriteAllText("test.xml", testXml);

      var xmlReader = CreateXMLReader();
      var scene = xmlReader.Read("test.xml");

      Assert.IsInstanceOf(typeof(BaseSceneNode), scene);
      Assert.IsInstanceOf(typeof(ScaleNode), scene.Children.First());
      Assert.IsInstanceOf(typeof(RotationNode), scene.Children.First().Children.First());
      Assert.IsInstanceOf(typeof(TranslationNode), scene.Children.First().Children.First().Children.First());
    }

    [Test()]
    public void TestScaleParameters()
    {
      var testXml = @"<Scene>
                        <Scale x='2' y='3' z='4'>                                                   
                        </Scale>                          
                      </Scene>";
      File.WriteAllText("test.xml", testXml);

      var xmlReader = CreateXMLReader();
      var scene = xmlReader.Read("test.xml");

      var scaleNode = (ScaleNode) scene.Children.First();
      Assert.AreEqual(2.0, scaleNode.X);
      Assert.AreEqual(3.0, scaleNode.Y);
      Assert.AreEqual(4.0, scaleNode.Z);
    }

    [Test()]
    public void TestRotateParameters()
    {
      var testXml = @"<Scene>
                        <Rotate x='1' y='0' z='4' angle='90'>                                           
                        </Rotate>                      
                      </Scene>";
      File.WriteAllText("test.xml", testXml);

      var xmlReader = CreateXMLReader();
      var scene = xmlReader.Read("test.xml");

      var rotateNode = (RotationNode)scene.Children.First();
      Assert.AreEqual(1.0, rotateNode.X);
      Assert.AreEqual(0.0, rotateNode.Y);
      Assert.AreEqual(4.0, rotateNode.Z);
      Assert.AreEqual((float)Math.PI / 2.0f, rotateNode.Angle);
    }

    [Test()]
    public void TestTranslateParameters()
    {
      var testXml = @"<Scene>
                        <Translate x='20' y='1' z='400'>                                                   
                        </Translate>                          
                      </Scene>";
      File.WriteAllText("test.xml", testXml);

      var xmlReader = CreateXMLReader();
      var scene = xmlReader.Read("test.xml");

      var translationNode = (TranslationNode)scene.Children.First();
      Assert.AreEqual(20.0, translationNode.X);
      Assert.AreEqual(1.0, translationNode.Y);
      Assert.AreEqual(400.0, translationNode.Z);
    }

    private static XMLDataNodeReader CreateXMLReader()
    {
      var vertexFactory = new VertexFactory();
      var meshLoader = new ObjMeshLoader(vertexFactory);
      var shapeFactory = new ShapeFactory(meshLoader);
      var sceneNodeFactory = new SceneNodeFactory(shapeFactory);
      var dataNodeFactory = new DataNodeFactory(sceneNodeFactory);
      var xmlReader = new XMLDataNodeReader(dataNodeFactory);
      return xmlReader;
    }
  }
}
