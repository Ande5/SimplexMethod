using System.Collections.Generic;
using BL.Simplex;

namespace BL
{
    public class CompilingDualTasks
    {
        public Function Function { get; private set; }
        private readonly List<DConstraint> _constraints = new List<DConstraint>();

        public DConstraint[] Constraints => _constraints.ToArray();

        public void CompilingTasks(Function function, DConstraint[] constraintsValue)
        {
            var sizeMatrix = constraintsValue[0].GetCoefficients().Length;
            var matrix = new double[sizeMatrix,sizeMatrix];

            for(int i = 0; i < constraintsValue.Length; i++)
            {
                for (int j = 0; j < sizeMatrix; j++)
                {
                    matrix[i, j] = constraintsValue[i].GetCoefficients()[j];
                }
            }

            Function = new Function(ConvertRhsToFunction(constraintsValue),
                function.Aspiration == Aspiration.max ? Aspiration.min : Aspiration.max);

            var newRhs = function.DataFunction;

            MatrixTransposition(ref matrix);

            CompilingConstraints(newRhs, matrix);
        }

        private void CompilingConstraints(double[] rhs, double [,] matrix)
        {
            for (int i = 0; i <= matrix.GetUpperBound(0); i++)
            {
                var coefficients = new double[matrix.GetUpperBound(0)+1];
                for (int j = 0; j <= matrix.GetUpperBound(1); j++)
                {
                    coefficients[j] = matrix[i, j];
                }
                _constraints.Add(new DConstraint(coefficients, AbstractSimplex.LESS_THAN,rhs[i]));
            }

        }

        private double[] ConvertRhsToFunction(DConstraint[] constraintsValue)
        {
            var index = 0;
            var dataFunction = new double[constraintsValue.Length];

            foreach (var constraint in constraintsValue)
            {
                dataFunction[index] = constraint.GetRhs();
                index++;
            }

            return dataFunction;
        }

        private void MatrixTransposition(ref double[,] matrix)
        {
            for (int i = 0; i <= matrix.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    var temp = matrix[i, j];
                    matrix[i, j] = matrix[j, i];
                    matrix[j, i] = temp; 
                }
            }
        }

    }
}
