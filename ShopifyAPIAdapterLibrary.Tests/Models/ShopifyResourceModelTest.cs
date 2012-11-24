using NUnit.Framework;
using ShopifyAPIAdapterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Tests.Models
{
    public class Mob : ShopifyResourceModel
    {
        private int? _Level;
        public int? Level
        {
            get { return _Level; }
            set
            {
                SetProperty(ref _Level, value);
            }
        }
    }

    [TestFixture]
    class ShopifyResourceModelTest
    {
        [Test]
        public void ShouldMarkModifiedFieldsAsChanged()
        {
            var mob = new Mob();
            Assert.IsTrue(mob.IsClean());
            Assert.IsFalse(mob.IsFieldDirty("Level"));
            mob.Level = 34;
            Assert.IsTrue(mob.IsFieldDirty("Level"));
            Assert.IsFalse(mob.IsClean());
            mob.Reset();
            Assert.IsFalse(mob.IsFieldDirty("Level"));
        }
    }
}
