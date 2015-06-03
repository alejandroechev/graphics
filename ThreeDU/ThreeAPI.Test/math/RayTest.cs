using NUnit.Framework;
using System;

using OpenTK;

namespace ThreeAPI.Test
{
  [TestFixture ()]
  public class RayTest
  {
    [Test ()]
    public void TestIntersectPlaneFront ()
    {
      Plane pln = new Plane (Vector3.Zero, Vector3.UnitX);
      Ray ray = new Ray (-Vector3.UnitX, Vector3.UnitX);

      float distance;
      bool expected = true;
      bool actual = Ray.Intersect (ray, pln, out distance);
      Assert.AreEqual (expected, actual);
    }
    [Test ()]
    public void TestIntersectPlaneFrontDistance ()
    {
      Plane pln = new Plane (Vector3.Zero, Vector3.UnitX);
      Ray ray = new Ray (-Vector3.UnitX, Vector3.UnitX);

      float actual;
      float expected = 1.0f;
      Ray.Intersect (ray, pln, out actual);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectPlaneBack ()
    {
      Plane pln = new Plane (Vector3.Zero, Vector3.UnitX);
      Ray ray = new Ray (Vector3.UnitX, Vector3.UnitX);

      float distance;
      bool expected = false;
      bool actual = Ray.Intersect (ray, pln, out distance);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestNoIntersectPlaneParallel ()
    {
      Plane pln = new Plane (Vector3.Zero, Vector3.UnitX);
      Ray ray = new Ray (Vector3.UnitX, Vector3.UnitY);

      float distance;
      bool expected = false;
      bool actual = Ray.Intersect (ray, pln, out distance);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectBoxFromOutsideFront ()
    {
      //
      // .__.
      // |  |  <---
      // |__|
      //

      BoundingBox pln = new BoundingBox (-Vector3.One, Vector3.One);
      Ray ray = new Ray (-2.0f * Vector3.UnitX, Vector3.UnitX);

      float distance;
      bool expected = true;
      bool actual = Ray.Intersect (ray, pln, out distance);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectBoxFromOutsideFrontDistance ()
    {
      //
      // .__.
      // |  |  <---
      // |__|
      //

      BoundingBox bbox = new BoundingBox (-Vector3.One, Vector3.One);
      Ray ray = new Ray (-2.0f * Vector3.UnitX, Vector3.UnitX);

      float actual;
      float expected = 1.0f;
      Ray.Intersect (ray, bbox, out actual);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectBoxFromInside ()
    {
      //
      // ._________.
      // |    <--- |
      // |_________|
      //

      BoundingBox bbox = new BoundingBox (-Vector3.One, Vector3.One);
      Ray ray = new Ray (Vector3.Zero, Vector3.UnitX);

      float distance;
      bool expected = true;
      bool actual = Ray.Intersect (ray, bbox, out distance);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectBoxFromInsideDistance ()
    {
      //
      // ._________.
      // |    <--- |
      // |_________|
      //

      BoundingBox bbox = new BoundingBox (-Vector3.One, Vector3.One);
      Ray ray = new Ray (Vector3.Zero, Vector3.UnitX);

      float actual;
      float expected = 0.0f;
      Ray.Intersect (ray, bbox, out actual);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestNoIntersectBoxFromOutsideBack ()
    {
      //
      //      .__.
      // <--- |  |  
      //      |__|
      //     

      BoundingBox bbox = new BoundingBox (-Vector3.One, Vector3.One);
      Ray ray = new Ray (2.0f * Vector3.UnitX, Vector3.UnitX);

      float distance;
      bool expected = false;
      bool actual = Ray.Intersect (ray, bbox, out distance);
      Console.WriteLine (distance);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestNoIntersectBoxFromOutsideParallel ()
    {
      //      <---
      //      .__.
      //      |  |  
      //      |__|
      //     

      BoundingBox bbox = new BoundingBox (-Vector3.One, Vector3.One);
      Ray ray = new Ray (-2.0f * Vector3.UnitY, Vector3.UnitX);

      float distance;
      bool expected = false;
      bool actual = Ray.Intersect (ray, bbox, out distance);
      Console.WriteLine (distance);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectSphereFromOutsideFront ()
    {
      //   ___
      // ./   \.
      // |     |  <---
      // '\___/'
      //

      BoundingSphere sph = new BoundingSphere (Vector3.Zero, 1.0f);
      Ray ray = new Ray (-2.0f * Vector3.UnitX, Vector3.UnitX);

      float distance;
      bool expected = true;
      bool actual = Ray.Intersect (ray, sph, out distance);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectSphereFromOutsideFrontDistance ()
    {
      //   ___
      // ./   \.
      // |     |  <---
      // '\___/'
      //

      BoundingSphere sph = new BoundingSphere (Vector3.Zero, 1.0f);
      Ray ray = new Ray (-2.0f * Vector3.UnitX, Vector3.UnitX);

      float actual;
      float expected = 1.0f;
      Ray.Intersect (ray, sph, out actual);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectSphereFromInside ()
    {
      //   ___
      // ./   \.
      // | <-- |  
      // '\___/'
      //

      BoundingSphere sph = new BoundingSphere (Vector3.Zero, 1.0f);
      Ray ray = new Ray (Vector3.Zero, Vector3.UnitX);

      float distance;
      bool expected = true;
      bool actual = Ray.Intersect (ray, sph, out distance);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectSphereFromInsideDistance ()
    {
      //   ___
      // ./   \.
      // | <-- |  
      // '\___/'
      //

      BoundingSphere sph = new BoundingSphere (Vector3.Zero, 1.0f);
      Ray ray = new Ray (Vector3.Zero, Vector3.UnitX);

      float actual;
      float expected = 0.0f;
      Ray.Intersect (ray, sph, out actual);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectSphereFromOutsideBack ()
    {
      //        ___
      //      ./   \.
      // <--  |     |  
      //      '\___/'
      //      

      BoundingSphere sph = new BoundingSphere (Vector3.Zero, 1.0f);
      Ray ray = new Ray (2.0f*Vector3.UnitX, Vector3.UnitX);

      float distance;
      bool expected = false;
      bool actual = Ray.Intersect (ray, sph, out distance);
      Assert.AreEqual (expected, actual);
    }

    [Test ()]
    public void TestIntersectSphereFromOutsideParallel ()
    {
      //    ___
      //  ./   \.
      //  |     |  
      //  '\___/'
      //    <---

      BoundingSphere sph = new BoundingSphere (Vector3.Zero, 1.0f);
      Ray ray = new Ray (-2.0f*Vector3.UnitY, Vector3.UnitX);

      float distance;
      bool expected = false;
      bool actual = Ray.Intersect (ray, sph, out distance);
      Assert.AreEqual (expected, actual);
    }
  }

}

