using System.Collections.Generic;
using System.Text;

namespace BL.Simplex
{
    internal abstract class AbstractSimplex
    {
        public const int LESS_THAN = 0;
        public const int GREATER_THAN = 1;
        public const int EQUAL_TO = 2;
        /*
        public final static int GREATER_EQUAL = 3;
        public final static int LESS_EQUAL = 4;
        */

        public static int CONTINUE = 0;
        protected static int OPTIMAL = 1;
        protected static int UNBOUNDED = 2;

        private bool _minimize;
        protected double[] objective;
        private double[][] _constraints;
        private int[] _equations;
        private double[] _rhs;

        protected double[][] m;
        private int[] _basisVariable;
        private int[] _nonBasisVariable;
        private int[] _slackVariable;
        protected bool[] locked;

        public virtual void Init()
        {
            m = new double[_constraints.Length + 1][];
            for (var i = 0; i < _constraints.Length + 1; i++)
                m[i] = new double[objective.Length + _constraints.Length + 1];

            for (var i = 0; i < _constraints.Length; ++i)
            {
                for (var j = 0; j < _constraints[i].Length; ++j)
                    m[i][j] = _constraints[i][j] * (_equations[i] == GREATER_THAN ? -1 : 1);
                m[i][objective.Length + i] = 1;
                m[i][m[i].Length - 1] = _rhs[i] * (_equations[i] == GREATER_THAN ? -1 : 1);
            }

            for (var i = 0; i < objective.Length; ++i) m[m.Length - 1][i] = objective[i] * (_minimize ? 1 : -1);
            _nonBasisVariable = new int[objective.Length + _constraints.Length];
            _slackVariable = new int[_constraints.Length];
            for (var i = 0; i < _nonBasisVariable.Length; ++i)
            {
                _nonBasisVariable[i] = i;
                if (i >= objective.Length) _slackVariable[i - objective.Length] = i;
            }

            _basisVariable = new int[_constraints.Length];
            for (var i = 0; i < _basisVariable.Length; ++i) _basisVariable[i] = _slackVariable[i];
            locked = new bool[_basisVariable.Length];
        }

        public void SetObjective(Function function)
        {
            objective = function.DataFunction;
            _minimize = function.Aspiration == Aspiration.min;
        }

        public void SetConstraints(double[][] constraints, int[] equations, double[] rhs)
        {
            _constraints = constraints;
            _equations = equations;
            _rhs = rhs;
        }

        protected void Pivot(int pivotRow, int pivotColumn)
        {
            var quotient = m[pivotRow][pivotColumn];
            for (int i = 0; i < m[pivotRow].Length; ++i)
            {
                m[pivotRow][i] = m[pivotRow][i] / quotient;
            }
            for (int i = 0; i < m.Length; ++i)
            {
                if (m[i][pivotColumn] != 0 && i != pivotRow)
                {

                    quotient = m[i][pivotColumn] / m[pivotRow][pivotColumn];

                    for (int j = 0; j < m[i].Length; ++j)
                    {
                        m[i][j] = m[i][j] - quotient * m[pivotRow][j];
                    }
                }
            }
            _basisVariable[pivotRow] = _nonBasisVariable[pivotColumn];
        }

        public double GetObjectiveResult() 
            => m[m.Length - 1][m[m.Length - 1].Length - 1] * (_minimize ? -1 : 1);

        public double[] GetCoefficients()
        {
            var result = new double[objective.Length];

            for (int i = 0; i < result.Length; ++i)
            {
                for (int j = 0; j < _basisVariable.Length; ++j)
                {
                    if (i == _basisVariable[j])
                    {
                        result[i] = m[j][m[j].Length - 1];
                    }
                }
            }
            return result;
        }

        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append('\t');
            for (int i = 0; i < _nonBasisVariable.Length; ++i)
            {
                if (i < _nonBasisVariable.Length - _basisVariable.Length)
                {
                    s.Append('x');
                    s.Append(_nonBasisVariable[i] + 1);
                }
                else
                {
                    s.Append('s');
                    s.Append(_nonBasisVariable[i] - (_nonBasisVariable.Length - _basisVariable.Length) + 1);
                }
                s.Append('\t');
            }
            s.Append('\n');

            for (int i = 0; i < m.Length - 1; ++i)
            {
                if (_basisVariable[i] < _nonBasisVariable.Length - _basisVariable.Length)
                {
                    s.Append('x');
                    s.Append(_basisVariable[i] + 1);
                }
                else
                {
                    s.Append('s');
                    s.Append(_basisVariable[i] - (_nonBasisVariable.Length - _basisVariable.Length) + 1);
                }
                s.Append('\t');
                for (int j = 0; j < m[i].Length; ++j)
                {
                    s.Append($"{m[i][j]:f2}");
                    s.Append('\t');
                }
                s.Append('\n');
            }

            s.Append("F(x)");
            s.Append('\t');
            for (int i = 0; i < m[m.Length - 1].Length; ++i)
            {
                s.Append($"{m[m.Length - 1][i]:f2}");
                s.Append('\t');
            }
            s.Append('\n');

            return s.ToString();
        }

        public List<DConstraint> GetConstraint()
        {
            var constraints = new List<DConstraint>();

            for (int i = 0; i < m.Length - 1; i++)
            {
                var index = 0;
                var coefficients = new List<double>();
                for (int j = 0; j < m[i].Length - 1; j++ , index++)
                {
                    if (j >= _nonBasisVariable.Length - _basisVariable.Length)
                    {
                        coefficients.Add(m[i][j]*-1);
                    }
                }
                constraints.Add(new DConstraint(coefficients.ToArray(), EQUAL_TO, m[i][index]));
            }

            return constraints;
        }


        public event DataInfo SimplexInfo;

        protected void OnSimplexInfo(string data)
        {
            SimplexInfo?.Invoke(this, data);
        }
    }
}
