using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PotStirrersWebAPI.Models
{
    public class TurnPosition
    {
        public TurnPosition(int x, bool y)
        {
            ingPos = x;
            ingCooked = y;
        }
        internal int ingPos;
        internal bool ingCooked;
    }
}