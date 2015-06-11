using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OpenTK;

namespace ThreeAPI.scene
{
  public class ObjMeshLoader : IMeshLoader
  {
    private readonly char[] _splitCharacters = { ' ' };
    private readonly char[] _faceParamaterSplitter = { '/' };

    private readonly IVertexFactory _vertexFactory;

    public ObjMeshLoader(IVertexFactory vertexFactory)
    {
      _vertexFactory = vertexFactory;
    }

    public void Load(IMesh mesh, string filePath)
    {
      using (var streamReader = new StreamReader(filePath))
      {
        Load(streamReader, mesh);
      }
    }

    private void Load(TextReader textReader, IMesh mesh)
    {
      var vertices = new List<Vector3>();
      var normals = new List<Vector3>();
      var texCoords = new List<Vector2>();
      var verticesMap = new Dictionary<IVertex, Vector3>();
      var vertexToNormal = new Dictionary<Vector3, List<Vector3>>();

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
            vertices.Add(new Vector3(x, y, z));
            break;

          case "vt": // TexCoord
            var u = float.Parse(parameters[1], CultureInfo.InvariantCulture);
            var v = float.Parse(parameters[2], CultureInfo.InvariantCulture);
            texCoords.Add(new Vector2(u, v));
            break;

          case "vn": // Normal
            var nx = float.Parse(parameters[1], CultureInfo.InvariantCulture);
            var ny = float.Parse(parameters[2], CultureInfo.InvariantCulture);
            var nz = float.Parse(parameters[3], CultureInfo.InvariantCulture);
            normals.Add(new Vector3(nx, ny, nz));
            break;

          case "f":
            switch (parameters.Length)
            {
              case 4:

                var triangleVertices = new List<IVertex>();
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
                    vertexToNormal.Add(vertex.Position, new List<Vector3>());
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

    private Vector3 GetAverageNormal(IEnumerable<Vector3> normals)
    {
      var average = new Vector3();
      foreach (var normal in normals)
      {
        average += normal;
      }
      average.Normalize();
      return average;
    }

    private IVertex GetVertex(string faceParameter, List<Vector3> vertices, List<Vector3> normals, List<Vector2> texCoords)
    {
      var position = new Vector3();
      var texCoord = new Vector2();
      var normal = new Vector3();

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
      var vertex = _vertexFactory.CreateVertex(position, normal, texCoord);
      return vertex;
    }

  }
}