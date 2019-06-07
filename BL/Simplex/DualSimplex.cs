namespace BL.Simplex
{
    internal class DualSimplex : PrimalSimplex
    {
        public override void Init()
        {
            base.Init();
            _primal = false;
        }

        public override int Iterate()
        {
            if (_primal) return base.Iterate();

            // Select pivot row
            int pr = -1;
            var min = double.PositiveInfinity;
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
                    OnSimplexInfo("///Решение прямым симплекс методом///\n");
                    _primal = true;
                    return CONTINUE;
                }
                return OPTIMAL;
            }

            // Select pivot column
            int pc = -1;
            var max = double.NegativeInfinity;
            if (pr > -1)
            {
                for (int i = 0; i < m[pr].Length - 1; ++i)
                {
                    if (!(m[pr][i] < 0) || i >= objective.Length && locked[i - objective.Length]) continue;
                    var quotient = m[m.Length - 1][i] / m[pr][i];
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

            Pivot(pr, pc);
            OnSimplexInfo(ToString());
            OnClassicSimplexInfo(ClassicMatrix());
            return CONTINUE;
        }
    }
}
