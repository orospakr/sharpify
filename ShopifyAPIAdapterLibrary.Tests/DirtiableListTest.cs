using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FakeItEasy;
using ShopifyAPIAdapterLibrary.Models;

namespace ShopifyAPIAdapterLibrary.Tests
{
    [TestFixture]
    public class DirtiableListTest
    {
        private IDirtiable MakeDirtiable(bool dirtiness)
        {
            var frag = A.Fake<IDirtiable>();
            A.CallTo(() => frag.IsClean()).Returns(!dirtiness);
            return frag;
        }

        [Test]
        public void ShouldBeDirtyAfterInstantiation()
        {
            var l = new DirtiableList<IDirtiable>();
            Assert.IsFalse(l.IsClean());
        }

        [Test]
        public void ShouldBeCleanAfterResetWhenEmpty()
        {
            var l = new DirtiableList<IDirtiable>();
            l.Reset();
            Assert.IsTrue(l.IsClean());
        }

        [Test]
        public void ShouldBeCleanAfterResetWhenContainingCleanObject()
        {
            var l = new DirtiableList<IDirtiable>();
            l.Add(MakeDirtiable(false));
            l.Reset();
            Assert.IsTrue(l.IsClean());
        }

        [Test]
        public void ShouldBeDirtyAfterInsertingObject()
        {
            var l = new DirtiableList<IDirtiable>();
            l.Reset();

            l.Add(MakeDirtiable(false));
            Assert.IsFalse(l.IsClean());
        }

        [Test]
        public void ShouldBeDirtyAfterResetWhenContainingDirtyObject()
        {
            var l = new DirtiableList<IDirtiable>();
            l.Reset();

            l.Add(MakeDirtiable(true));
            l.Reset();

            Assert.IsFalse(l.IsClean(), "should always be dirty when containing a dirty object");
        }
    }
}
