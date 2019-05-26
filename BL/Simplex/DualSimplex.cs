using System;

namespace BL.Simplex
{
    internal class DualSimplex : PrimalSimplex
    {
        private bool primal;

        public void Init()
        {
            base.Init();
            primal = false;
        }

        public int Iterate()
        {
            if (primal) return base.Iterate();

            double quotient;

            // Select pivot row
            int pr = -1;
            double min = double.PositiveInfinity;
            for (int i = 0; i < m.Length - 1; ++i)
            {
                if (!(m[i][m[i].Length - 1] < 0) || !(m[i][m[i].Length - 1] < min)) continue;
                pr = i;
                min = m[i][m[i].Length - 1];
            }
            if (pr < 0)
            {
                for (int i = 0; i < m[m.Length - 1].Length - 1; ++i)
                {
                    if (!(m[m.Length - 1][i] < 0)) continue;
                    // Start primal
                    Console.WriteLine("Continue with primal simplex");
                    primal = true;
                    return CONTINUE;
                }
                return OPTIMAL;
            }

            // Select pivot column
            int pc = -1;
            double max = double.NegativeInfinity;
            if (pr > -1)
            {
                for (int i = 0; i < m[pr].Length - 1; ++i)
                {
                    if (!(m[pr][i] < 0) || i >= objective.Length && locked[i - objective.Length]) continue;
                    quotient = m[m.Length - 1][i] / m[pr][i];
                    if (quotient > max)
                    {
                        max = quotient;
                        pc = i;
                    }
                }
                if (pc < 0)
                {
                    return UNBOUNDED;
                }
            }

            // Pivot
            Console.WriteLine("Pivo: row=" + (pr + 1) + ", column=" + (pc + 1));
            Pivot(pr, pc);

            return CONTINUE;
        }
    }
}
