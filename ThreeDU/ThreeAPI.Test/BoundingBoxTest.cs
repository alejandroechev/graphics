using NUnit.Framework;
using System;
using ThreeAPI;

namespace ThreeAPI.Test
{
  [TestFixture ()]
  public class BoundingBoxTest
  {
    [Test ()]
    public void TestCase ()
    {
      int a = BoundingBox.dummy ();
      Assert.AreEqual (a, 0);
    }
  }
}

