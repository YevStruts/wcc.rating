using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wcc.rating.Enums
{
    public enum WinRule
    {
        [Description("Best of 1")]
        bo1,

        [Description("Best of 3")]
        bo3,

        [Description("Best of 5")]
        bo5,
    }
}
