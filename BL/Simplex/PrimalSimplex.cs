namespace BL.Simplex
{
    internal class PrimalSimplex : AbstractSimplex
    {
        public virtual int Iterate()
        {
            double quotient;
            // Select pivot column
            int pc = -1;
            var min = double.PositiveInfinity;
            for (int i = 0; i < m[m.Length - 1].Length - 1; ++i)
            {
                if (!(m[m.Length - 1][i] < 0) || !(m[m.Length - 1][i] < min) ||
                    i >= objective.Length && locked[i - objective.Length]) continue;
                pc = i;
                min = m[m.Length - 1][i];
            }

            if (pc < 0) return OPTIMAL;

            // Select pivot row
            int pr = -1;
            min = double.PositiveInfinity;
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

            if (pr < 0) return UNBOUNDED;

            Pivot(pr, pc);
            OnSimplexInfo(ToString());
            return CONTINUE;
        }
    }
}
