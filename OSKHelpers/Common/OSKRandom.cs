using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSKHelpers.Common
{
    public class OSKRandom : Random
    {
        #region Proprietà

        public static int Seed => Math.Abs((int)DateTime.Now.Ticks);

        #endregion

        #region Costruttori

        public OSKRandom() : base(Seed) { }

        public OSKRandom(int seed) : base(seed) { }

        #endregion
    }
}
