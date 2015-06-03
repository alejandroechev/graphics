using NUnit.Framework;
using System;

using OpenTK;

using ThreeAPI;

namespace ThreeAPI.Test
{
  [TestFixture ()]
  public class PlaneTest
  {
    [Test ()]
    public void TestIntersectsBox ()
    {
      BoundingBox bbox = new BoundingBox (-Vector3.One, Vector3.One);
      Plane plane = new Plane (Vector3.Zero, Vector3.UnitX);

      bool expected = true;
      bool actual = Plane.Intersect (plane, bbox);

      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestNoIntersectsBoxFront ()
    {
      BoundingBox bbox = new BoundingBox (-Vector3.One, Vector3.One);
      Plane plane = new Plane (-2.0f * Vector3.UnitX, Vector3.UnitX);

      bool expected = false;
      bool actual = Plane.Intersect (plane, bbox);

      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestNoIntersectsBoxBack ()
    {
      BoundingBox bbox = new BoundingBox (-Vector3.One, Vector3.One);
      Plane plane = new Plane (2.0f * Vector3.UnitX, Vector3.UnitX);

      bool expected = false;
      bool actual = Plane.Intersect (plane, bbox);

      Assert.AreEqual (expected, actual);
    }
    [Test ()]
    public void TestIntersectsSphere ()
    {
      BoundingSphere sphere = new BoundingSphere (Vector3.Zero, 1.0f);
      Plane plane = new Plane (Vector3.Zero, Vector3.UnitX);

      bool expected = true;
      bool actual = Plane.Intersect (plane, sphere);

      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestNoIntersectsSphereFront ()
    {
      BoundingSphere sphere = new BoundingSphere (Vector3.Zero, 1.0f);
      Plane plane = new Plane (-2.0f * Vector3.UnitX, Vector3.UnitX);

      bool expected = false;
      bool actual = Plane.Intersect (plane, sphere);

      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestNoIntersectsSphereBack ()
    {
      BoundingSphere sphere = new BoundingSphere (Vector3.Zero, 1.0f);
      Plane plane = new Plane (2.0f * Vector3.UnitX, Vector3.UnitX);

      bool expected = false;
      bool actual = Plane.Intersect (plane, sphere);

      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestDistanceZero ()
    {
      Plane plane = new Plane (Vector3.Zero, Vector3.UnitX);

      float expected = 0.0f;
      float actual = Plane.Distance (plane, Vector3.Zero);

      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestDistanceFront ()
    {
      Plane plane = new Plane (Vector3.Zero, Vector3.UnitX);

      float expected = 1.0f;
      float actual = Plane.Distance (plane, Vector3.UnitX);

      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestDistanceBack ()
    {
      Plane plane = new Plane (Vector3.Zero, Vector3.UnitX);

      float expected = -1.0f;
      float actual = Plane.Distance (plane, -Vector3.UnitX);

      Assert.AreEqual (expected, actual);
    }
  }
}

