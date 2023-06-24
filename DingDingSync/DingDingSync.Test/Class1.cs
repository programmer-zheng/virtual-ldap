using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DingDingSync.Test
{
    public  class Class1:DingDingSyncTestBase
    {
        [Fact]
        public void asdf()
        {
            var x = 1 / 1;
            Assert.Equal(1, x);
        }
    }
}
