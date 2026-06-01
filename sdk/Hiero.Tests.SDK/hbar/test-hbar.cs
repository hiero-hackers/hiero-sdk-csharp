// SPDX-License-Identifier: Apache-2.0
using Hiero.Reference;
using Hiero.SDK;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Hiero.Tests.SDK.HBar
{
    /// <include file="test-hbar.cs.xml" path='docs/member[@name="T:Hiero.Tests.SDK.HBar.HbarTest"]' />
    public class HbarTest
    {
        private readonly Hbar hundredHbar = new(100);
        private readonly Hbar negativeFiftyHbar = new(-50);
        private readonly Hbar fiftyHbar = Hbar.FromTinybars(fiftyGTinybar);
        private static readonly long fiftyGTinybar = 5000000000;

        [Fact]
        /// <include file="test-hbar.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.HBar.HbarTest.ShouldConstruct"]' />
        public virtual void ShouldConstruct()
        {
            Assert.Equal(fiftyHbar.ToTinybars(), fiftyGTinybar);
            Assert.Equal(fiftyHbar.To(HbarUnit.HBAR), new BigDecimal(50));
            Assert.Equal(new Hbar(50).ToTinybars(), fiftyGTinybar);
            Assert.Equal(Hbar.FromTinybars(fiftyGTinybar).ToTinybars(), fiftyGTinybar);
        }
        [Fact]
        /// <include file="test-hbar.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.HBar.HbarTest.ShouldNotConstruct"]' />
        public virtual void ShouldNotConstruct()
        {
            Assert.Throws<ArgumentException>(() => new Hbar(BigDecimal.Parse("0.1"), HbarUnit.TINYBAR));
        }
        [Fact]
        /// <include file="test-hbar.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.HBar.HbarTest.ShouldDisplay"]' />
        public virtual void ShouldDisplay()
        {
            Assert.Equal("50 ℏ", fiftyHbar.ToString());
            Assert.Equal("-50 ℏ", negativeFiftyHbar.ToString());
            Assert.Equal("1 tℏ", Hbar.FromTinybars(1).ToString());
            Assert.Equal("-1 tℏ", Hbar.FromTinybars(1).Negated().ToString());
            Assert.Equal("1000 tℏ", Hbar.FromTinybars(1000).ToString());
            Assert.Equal("-1000 tℏ", Hbar.FromTinybars(1000).Negated().ToString());
        }
        [Fact]
        /// <include file="test-hbar.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.HBar.HbarTest.ShouldConvert(BigDecimal,HbarUnit)"]' />
        public virtual void ShouldConvert()
        {
            BigDecimal 
                value_microbar = new (50000000),
                value_millibar = new (50000),
                value_hbar = new (50),
                value_kilobar = BigDecimal.Parse("0.05"),
                value_megabar = BigDecimal.Parse("0.00005"),
                value_gigabar = BigDecimal.Parse("0.00000005");
            HbarUnit
                unit_microbar = HbarUnit.MICROBAR,
                unit_millibar = HbarUnit.MILLIBAR,
                unit_hbar = HbarUnit.HBAR,
                unit_kilobar = HbarUnit.KILOBAR,
                unit_megabar = HbarUnit.MEGABAR,
                unit_gigabar = HbarUnit.GIGABAR;

            Assert.Equal(fiftyHbar, Hbar.From(value_microbar, HbarUnit.MICROBAR)); 
            Assert.Equal(fiftyHbar, Hbar.From(value_millibar, HbarUnit.MILLIBAR)); 
            Assert.Equal(fiftyHbar, Hbar.From(value_hbar, HbarUnit.HBAR)); 
            Assert.Equal(fiftyHbar, Hbar.From(value_kilobar, HbarUnit.KILOBAR)); 
            Assert.Equal(fiftyHbar, Hbar.From(value_megabar, HbarUnit.MEGABAR)); 
            Assert.Equal(fiftyHbar, Hbar.From(value_gigabar, HbarUnit.GIGABAR));

            Assert.Equal(fiftyHbar.To(unit_microbar), value_microbar);
            Assert.Equal(fiftyHbar.To(unit_millibar), value_millibar);
            Assert.Equal(fiftyHbar.To(unit_hbar), value_hbar);
            Assert.Equal(fiftyHbar.To(unit_kilobar), value_kilobar);
            Assert.Equal(fiftyHbar.To(unit_megabar), value_megabar);
            Assert.Equal(fiftyHbar.To(unit_gigabar), value_gigabar);
        }
        public static IEnumerable<object?[]> ShouldConvert_Data() { yield return [BigDecimal.ValueOf(0), HbarUnit.HBAR]; }

        [Fact]
        /// <include file="test-hbar.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.HBar.HbarTest.ShouldCompare"]' />
        public virtual void ShouldCompare()
        {
            Assert.NotEqual(fiftyHbar, hundredHbar);
            Assert.Equal(fiftyHbar, fiftyHbar);
            Assert.Equal(0, fiftyHbar.CompareTo(new Hbar(50)));
            Assert.True(fiftyHbar.CompareTo(hundredHbar) < 0);
            Assert.True(hundredHbar.CompareTo(fiftyHbar) > 0);
            Assert.True(fiftyHbar.CompareTo(negativeFiftyHbar) > 0);
        }
        [Fact]
        /// <include file="test-hbar.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.HBar.HbarTest.ConstructorWorks"]' />
        public virtual void ConstructorWorks()
        {
            _ = new Hbar(1);
        }
        [Fact]
        /// <include file="test-hbar.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.HBar.HbarTest.FromString"]' />
        public virtual void FromString()
        {
            Assert.Equal(100000000, Hbar.FromString("1").ToTinybars());
            Assert.Equal( 100000000, Hbar.FromString("1 ℏ").ToTinybars());
            Assert.Equal( 150000, Hbar.FromString("1.5 mℏ").ToTinybars());
            Assert.Equal( 150000, Hbar.FromString("+1.5 mℏ").ToTinybars());
            Assert.Equal(-150000, Hbar.FromString("-1.5 mℏ").ToTinybars());
            Assert.Equal( 300000000, Hbar.FromString("+3").ToTinybars());
            Assert.Equal(-300000000, Hbar.FromString("-3").ToTinybars());
            Assert.Throws<ArgumentException>(() =>
            {
                Hbar.FromString("1 h");
            });
            Assert.Throws<ArgumentException>(() =>
            {
                Hbar.FromString("1ℏ");
            });
        }
        [Fact]
        /// <include file="test-hbar.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.HBar.HbarTest.FromStringUnit"]' />
        public virtual void FromStringUnit()
        {
            Assert.Equal(1, Hbar.FromString("1", HbarUnit.TINYBAR).ToTinybars());
        }
        [Fact]
        public virtual void From()
        {
            Assert.Equal(100000000, Hbar.From(1).ToTinybars());
        }
        [Fact]
        /// <include file="test-hbar.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.HBar.HbarTest.FromUnit"]' />
        public virtual void FromUnit()
        {
            Assert.Equal(1, Hbar.From(1, HbarUnit.TINYBAR).ToTinybars());
        }
        [Fact]
        /// <include file="test-hbar.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.HBar.HbarTest.GetValue"]' />
        public virtual void GetValue()
        {
            Assert.Equal(new Hbar(1).GetValue(), BigDecimal.ValueOf(1));
        }
        [Fact]
        /// <include file="test-hbar.cs.xml" path='docs/member[@name="M:Hiero.Tests.SDK.HBar.HbarTest.HasHashCode"]' />
        public virtual void HasHashCode()
        {
            Assert.Equal(100000000, new Hbar(1).GetHashCode());
        }
    }
}
