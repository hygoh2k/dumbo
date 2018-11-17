using System;
using System.Collections.Generic;
using Xunit;
using Dombo.Extension;

namespace ExtensionTest
{
    public class CollectionTest
    {
        [Fact]
        public void AddRange()
        {
            IList<int> list = new List<int>();
            list.Add(1);
            list.AddRangeEx(new int[] { 2, 3, 4 });

            Assert.Equal(4, list.Count);
            Assert.Equal(1, list[0]);
            Assert.Equal(2, list[1]);
            Assert.Equal(3, list[2]);
            Assert.Equal(4, list[3]);
        }
    }
}
