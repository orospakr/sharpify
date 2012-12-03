using FakeItEasy;
using NUnit.Framework;
using ShopifyAPIAdapterLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopifyAPIAdapterLibrary.Tests.Models
{
    public class Faction : ShopifyResourceModel
    {
    }

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

        private Faction _Faction;
        public Faction Faction
        {
            get { return _Faction; }
            set
            {
                SetProperty(ref _Faction, value);
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

        [Test]
        public void ShouldBeDirtyIfAnyDirtiablesAreDirty()
        {
            var dirtyResource = A.Fake<Faction>();
            A.CallTo(() => dirtyResource.IsClean()).Returns(false);

            var mob = new Mob();
            mob.Faction = dirtyResource;
            mob.Reset();
            Assert.IsFalse(mob.IsClean());
        }
    }
}
