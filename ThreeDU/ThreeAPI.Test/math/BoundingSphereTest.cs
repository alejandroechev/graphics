using NUnit.Framework;
using System;

using OpenTK;

namespace ThreeAPI.Test
{
  [TestFixture ()]
  public class BoundingSphereTest
  {
    [Test ()]
    public void TestContainsPointInside ()
    {
      BoundingSphere sph = new BoundingSphere (Vector3.Zero, 1.0f);
      bool expected = true;
      bool actual = BoundingSphere.Contains (sph, Vector3.Zero);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestContainsPointOutside ()
    {
      BoundingSphere sph = new BoundingSphere (Vector3.Zero, 1.0f);
      bool expected = false;
      bool actual = BoundingSphere.Contains (sph, 2.0f*Vector3.UnitX);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestContainsPointBoundary ()
    {
      BoundingSphere sph = new BoundingSphere (Vector3.Zero, 1.0f);
      bool expected = true;
      bool actual = BoundingSphere.Contains (sph, 1.0f*Vector3.UnitX);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectsSphereInside ()
    {
      BoundingSphere sph1 = new BoundingSphere (Vector3.Zero, 1.0f);
      BoundingSphere sph2 = new BoundingSphere (Vector3.Zero, 2.0f);
      bool expected = true;
      bool actual = BoundingSphere.Intersects (sph1, sph2);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectsSpherePart ()
    {
      BoundingSphere sph1 = new BoundingSphere (Vector3.Zero, 1.0f);
      BoundingSphere sph2 = new BoundingSphere (1.0f*Vector3.Zero, 1.0f);
      bool expected = true;
      bool actual = BoundingSphere.Intersects (sph1, sph2);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectsSphereOutside ()
    {
      BoundingSphere sph1 = new BoundingSphere (Vector3.Zero, 1.0f);
      BoundingSphere sph2 = new BoundingSphere (3.0f*Vector3.UnitX, 1.0f);
      bool expected = false;
      bool actual = BoundingSphere.Intersects (sph1, sph2);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectsSphereBoundary ()
    {
      BoundingSphere sph1 = new BoundingSphere (Vector3.Zero, 1.0f);
      BoundingSphere sph2 = new BoundingSphere (2.0f*Vector3.UnitX, 1.0f);
      bool expected = true;
      bool actual = BoundingSphere.Intersects (sph1, sph2);
      Assert.AreEqual (expected, actual);
    }
  }
}

