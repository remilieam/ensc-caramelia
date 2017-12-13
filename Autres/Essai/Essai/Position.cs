using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Essai
{
    public class Position
    {
        public int Row { get; set; }

        public Position(int x)
        {
            this.Row = x;
        }

        public bool SamePosition(Position position)
        {
            if (this.Row == position.Row)
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return "Ligne : " + (this.Row + 1);
        }
    }
}
