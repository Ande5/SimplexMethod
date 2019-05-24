using System;

namespace BL.Simplex
{
    public class PrimalSimplex:AbstractSimplex
    {
        public int Iterate()
        {
            double quotient;
            // Select pivot column
            int pc = -1;
            double min = Double.PositiveInfinity;
            for (int i = 0; i < m[m.Length - 1].Length - 1; ++i)
            {
                if (
                    m[m.Length - 1][i] < 0 &&
                    m[m.Length - 1][i] < min &&
                    (i < objective.Length || !locked[i - objective.Length]))
                {

                    pc = i;
                    min = m[m.Length - 1][i];
                }
            }
            if (pc < 0)
            {
                return OPTIMAL;
            }

            // Select pivot row
            int pr = -1;
            min = Double.PositiveInfinity;
            for (int i = 0; i < m.Length - 1; ++i)
            {
                if (m[i][pc] > 0)
                {
                    quotient = m[i][m[i].Length - 1] / m[i][pc];
                    if (quotient < min)
                    {
                        min = quotient;
                        pr = i;
                    }
                }
            }
            if (pr < 0)
            {
                return UNBOUNDED;
            }

            // Pivot
            Console.WriteLine("Pivo: row=" + (pr + 1) + ", column=" + (pc + 1));
            Pivot(pr, pc);

            return CONTINUE;
        }
    }
}
