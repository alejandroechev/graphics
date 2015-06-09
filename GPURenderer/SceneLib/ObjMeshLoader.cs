using System.Globalization;
using System.IO;
using System.Collections.Generic;

namespace SceneLib
{
  public class ObjMeshLoader : IMeshLoader
  {
    private readonly char[] _splitCharacters = { ' ' };
    private readonly char[] _faceParamaterSplitter = { '/' };

    public void Load(MeshBase mesh)
    {
      using (var streamReader = new StreamReader(mesh.FilePath))
      {
        Load(streamReader, mesh);

      }
    }

    private void Load(TextReader textReader, MeshBase mesh)
    {
      var vertices = new List<Vector>();
      var normals = new List<Vector>();
      var texCoords = new List<Vector>();
      var verticesMap = new Dictionary<Vertex, Vector>();
      var vertexToNormal = new Dictionary<Vector, List<Vector>>();

      string line;
      while ((line = textReader.ReadLine()) != null)
      {
        line = line.Trim(_splitCharacters);
        line = line.Replace("  ", " ");

        var parameters = line.Split(_splitCharacters);

        switch (parameters[0])
        {
          case "p": // Point
            break;

          case "v": // Vertex
            var x = float.Parse(parameters[1], CultureInfo.InvariantCulture);
            var y = float.Parse(parameters[2], CultureInfo.InvariantCulture);
            var z = float.Parse(parameters[3], CultureInfo.InvariantCulture);
            vertices.Add(new Vector(x, y, z));
            break;

          case "vt": // TexCoord
            var u = float.Parse(parameters[1], CultureInfo.InvariantCulture);
            var v = float.Parse(parameters[2], CultureInfo.InvariantCulture);
            texCoords.Add(new Vector(u, v));
            break;

          case "vn": // Normal
            var nx = float.Parse(parameters[1], CultureInfo.InvariantCulture);
            var ny = float.Parse(parameters[2], CultureInfo.InvariantCulture);
            var nz = float.Parse(parameters[3], CultureInfo.InvariantCulture);
            normals.Add(new Vector(nx, ny, nz));
            break;

          case "f":
            switch (parameters.Length)
            {
              case 4:

                var triangleVertices = new List<Vertex>();
                for (int i = 0; i < 3; i++)
                {
                  var vertex = GetVertex(parameters[i + 1], vertices, normals, texCoords);
                  triangleVertices.Add(vertex);
                  verticesMap.Add(vertex, vertex.Position);

                }
                var polygon = mesh.AddPolygon(triangleVertices);
                foreach (var vertex in polygon.Vertices)
                {
                  if (!vertexToNormal.ContainsKey(vertex.Position))
                    vertexToNormal.Add(vertex.Position, new List<Vector>());
                  vertexToNormal[vertex.Position].Add(polygon.GetFaceNormal());
                }

                break;

              case 5:
                //Quads
                break;
            }
            break;
        }
      }

      if (normals.Count == 0)
      {
        foreach (var keyValue in verticesMap)
        {
          var normal = GetAverageNormal(vertexToNormal[keyValue.Value]);
          keyValue.Key.Normal = normal;
        }
      }


    }

    private Vector GetAverageNormal(IEnumerable<Vector> normals)
    {
      var average = new Vector();
      foreach (var normal in normals)
      {
        average += normal;
      }
      return average.Normalize3();
    }

    private Vertex GetVertex(string faceParameter, List<Vector> vertices, List<Vector> normals, List<Vector> texCoords)
    {
      var vertex = new Vertex();
      var position = new Vector();
      var texCoord = new Vector();
      var normal = new Vector();

      var parameters = faceParameter.Split(_faceParamaterSplitter);

      var vertexIndex = int.Parse(parameters[0]);
      if (vertexIndex < 0) vertexIndex = vertices.Count + vertexIndex;
      else vertexIndex = vertexIndex - 1;
      position = vertices[vertexIndex];

      if (parameters.Length > 1 && texCoords.Count > 0)
      {
        int texCoordIndex = 0;
        if (int.TryParse(parameters[1], out texCoordIndex))
        {
          if (texCoordIndex < 0) texCoordIndex = texCoords.Count + texCoordIndex;
          else texCoordIndex = texCoordIndex - 1;
          texCoord = texCoords[texCoordIndex];
        }
      }

      if (parameters.Length > 2 && normals.Count > 0)
      {
        int normalIndex = 0;
        if (int.TryParse(parameters[2], out normalIndex))
        {
          if (normalIndex < 0) normalIndex = normals.Count + normalIndex;
          else normalIndex = normalIndex - 1;
          normal = normals[normalIndex];
        }
      }
      vertex.Position = position;
      vertex.Normal = normal;
      vertex.TextureCoordinates = texCoord;

      return vertex;
    }



  }
}