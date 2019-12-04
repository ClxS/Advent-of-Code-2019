using System;
using System.Collections.Generic;
using System.Text;

namespace Day3
{
    public interface ISolver
    {
        (int Distance, int Steps) Solve(Data inputData);
    }
}
