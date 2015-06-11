using System.Xml.Linq;

namespace ThreeAPI.scene
{
  public interface IDataNodeFactory
  {
    IDataNode CreateXmlDataNode(XElement element);
  }
}