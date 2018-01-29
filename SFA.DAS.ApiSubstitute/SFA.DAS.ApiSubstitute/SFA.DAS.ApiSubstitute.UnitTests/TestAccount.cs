using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ApiSubstitute.UnitTests
{
    public class TestAccount
    {
        public int Accountid { get; protected set; }
        public TestAccount(int accountid)
        {
            Accountid = accountid;
        }
    }
}
