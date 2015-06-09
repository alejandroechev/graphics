using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SceneLib
{
  class XMLHelper
  {
    #region LoadHelpers
    public static float LoadFloat(XElement node, string attribute)
    {
      if (node == null || node.Attribute(attribute) == null)
        return 0;
      float value;
      float.TryParse(node.Attribute(attribute).Value, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
      return value;
    }

    public static int LoadInt(XElement node, string attribute)
    {
      if (node == null || node.Attribute(attribute) == null)
        return 0;
      int value;
      int.TryParse(node.Attribute(attribute).Value, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
      return value;
    }

    public static bool LoadBool(XElement node, string attribute)
    {
      if (node == null || node.Attribute(attribute) == null)
        return false;
      bool value = node.Attribute(attribute).Value.ToLowerInvariant() == "true";
      return value;
    }

    public static string LoadString(XElement node, string attribute)
    {
      if (node == null || node.Attribute(attribute) == null)
        return string.Empty;
      return node.Attribute(attribute).Value;
    }

    public static Vector LoadXYZ(XElement node)
    {
      if (node == null)
        return new Vector();
      return new Vector(LoadFloat(node, "x"), LoadFloat(node, "y"), LoadFloat(node, "z"));
    }

    public static Vector LoadColor(XElement node)
    {
      return new Vector(LoadFloat(node, "red"), LoadFloat(node, "green"), LoadFloat(node, "blue"));
    }

    public static Vector LoadSpecular(XElement node)
    {
      return new Vector(LoadFloat(node, "red"), LoadFloat(node, "green"), LoadFloat(node, "blue"), LoadFloat(node, "shininess"));
    }
    #endregion
  }
}
