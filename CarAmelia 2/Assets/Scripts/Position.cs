using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Position
{
    public int Number { get; set; }

    public Position(int x)
    {
        this.Number = x;
    }

    public bool SamePosition(Position position)
    {
        if (this.Number == position.Number)
        {
            return true;
        }

        return false;
    }

    public override string ToString()
    {
        return this.Number.ToString();
    }
}

