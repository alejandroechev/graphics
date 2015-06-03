using NUnit.Framework;
using System;

using OpenTK;

using ThreeAPI;

namespace ThreeAPI.Test
{
  [TestFixture ()]
  public class BoundingBoxTest
  {
    [Test ()]
    public void TestContainsPointInside ()
    {
      Vector3 vec1 = Vector3.One;
      BoundingBox bbox = new BoundingBox (-vec1, vec1);

      bool expected = true;
      bool actual = BoundingBox.Contains (bbox, Vector3.Zero);

      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestContainsPointOutside ()
    {
      Vector3 vec1 = Vector3.One;
      BoundingBox bbox = new BoundingBox (-vec1, vec1);

      bool expected = false;
      bool actual = BoundingBox.Contains (bbox, 2.0f * Vector3.UnitX);

      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestContainsPointBoundary ()
    {
      Vector3 vec1 = Vector3.One;
      BoundingBox bbox = new BoundingBox (-vec1, vec1);

      bool expected = true;
      bool actual = BoundingBox.Contains (bbox, Vector3.UnitX);

      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectBoxInside ()
    {
      Vector3 vec1 = Vector3.One;
      Vector3 vec2 = 2.0f * Vector3.One;
      // box of size 2 centered at 0,0,0
      BoundingBox bbox1 = new BoundingBox (-vec1, vec1);
      // box of size 4 centered at 0,0,0
      BoundingBox bbox2 = new BoundingBox (-vec2, vec2);

      bool expected = true;
      bool actual = BoundingBox.Intersects (bbox1, bbox2);

      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectBoxOutside ()
    {
      Vector3 vec1 = Vector3.One;
      // box of size 2 centerd at 0,0,0
      BoundingBox bbox1 = new BoundingBox (-vec1, vec1);

      // box of size 2 centerd at 3,3,3
      BoundingBox bbox2 = new BoundingBox (3 * vec1 - vec1, 3 * vec1 + vec1);

      bool expected = false;
      bool actual = BoundingBox.Intersects (bbox1, bbox2);

      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectBoxPartialIntersect ()
    {
      Vector3 vec1 = Vector3.One;
      // box of size 2 centerd at 0,0,0
      BoundingBox bbox1 = new BoundingBox (-vec1, vec1);

      // box of size 2 centerd at 1,1,1
      BoundingBox bbox2 = new BoundingBox (vec1 - vec1, vec1 + vec1);

      bool expected = true;
      bool actual = BoundingBox.Intersects (bbox1, bbox2);

      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectSphereInside ()
    {
      Vector3 vec0 = Vector3.Zero;
      Vector3 vec1 = Vector3.One;

      // box of size 2 centered at 0,0,0
      BoundingBox bbox = new BoundingBox (-vec1, vec1);
      // sphere of size 4 centered at 0,0,0
      BoundingSphere bsph = new BoundingSphere (vec0, 2.0f);

      bool expected = true;
      bool actual = BoundingBox.Intersects (bbox, bsph);

      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectSphereOutside ()
    {
      Vector3 vec1 = Vector3.One;

      // box of size 2 centered at 0,0,0
      BoundingBox bbox = new BoundingBox (-vec1, vec1);
      // sphere of size 2 centered at 3,3,3
      BoundingSphere bsph = new BoundingSphere (3.0f*vec1, 1.0f);

      bool expected = false;
      bool actual = BoundingBox.Intersects (bbox, bsph);

      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectSpherePartialIntersect ()
    {
      Vector3 vec1 = Vector3.One;
      // box of size 2 centerd at 0,0,0
      BoundingBox bbox = new BoundingBox (-vec1, vec1);

      // sphere of size 2 centerd at 1,1,1
      BoundingSphere bsph = new BoundingSphere (Vector3.One, 1.0f);

      bool expected = true;
      bool actual = BoundingBox.Intersects (bbox, bsph);

      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectSphereDiffBox ()
    {
      /// Test limit case for sphere vs box
      /// Both of size s0 and centered at p0
      /// give different results when intersected with bbox1.
      ///
      Vector3 vec1 = Vector3.One;

      // box of size 2 centerd at 0,0,0
      BoundingBox bbox1 = new BoundingBox (-vec1, vec1);

      float radius = 1.0f;
      Vector3 center = Vector3.One +  (radius + 0.1f) * Vector3.One.Normalized();
      BoundingSphere bsph = new BoundingSphere (center, radius);
      BoundingBox bbox2 = new BoundingBox (center - vec1, center + vec1);


      bool sphActual = BoundingBox.Intersects (bbox1, bsph);
      bool boxActual = BoundingBox.Intersects (bbox1, bbox2);

      Assert.AreEqual (false, sphActual);
      Assert.AreEqual (true, boxActual);
    }

  }
}

