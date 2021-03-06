﻿using FakeItEasy;
using NUnit.Framework;
using Sharpify.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpify.Tests.Models
{
    // used as a flat inline
    public class Faction : ShopifyResourceModel
    {
    }

    // used as a Has One inline
    public class Tier : ShopifyResourceModel
    {
        
    }

    public class AI : ShopifyResourceModel
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

        // inline flat
        private Faction _Faction;
        public Faction Faction
        {
            get { return _Faction; }
            set
            {
                SetProperty(ref _Faction, value);
            }
        }

        // as by-id has one
        private IHasOne<AI> _AI;
        public IHasOne<AI> AI
        {
            get { return _AI; }
            set
            {
                SetProperty(ref _AI, value);
            }
        }
        
        // inline has one
        private IHasOne<Tier> _Tier;
        [Inlinable]
        public IHasOne<Tier> Tier
        {
            get { return _Tier; }
            set
            {
                SetProperty(ref _Tier, value);
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

        //[Test]
        //public void ShouldUpdateAttributes()
        //{
        //    var mob = new Mob();
        //    var vals = new NameValueCollection();
        //    vals.Add("Level", "78");
        //    mob.UpdateAttributes(vals);
        //    Assert.AreEqual(78, mob.Level);
        //    Assert.IsTrue(mob.IsFieldDirty("Level"));
        //    Assert.IsFalse(mob.IsClean());
        //}
    }
}
