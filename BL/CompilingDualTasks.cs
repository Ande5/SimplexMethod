using System.Collections.Generic;
using BL.Simplex;

namespace BL
{
    public class CompilingDualTasks:MathematicalModel
    {
        /// <summary>
        /// Формирование двойственной задачи
        /// </summary>
        /// <param name="function">Исходная функция прямой задачи</param>
        /// <param name="constraintsValue">Ограничения прямой задачи</param>
        public void CompilingTasks(Function function, DConstraint[] constraintsValue)
        {
            var sizeMatrix = constraintsValue[0].GetCoefficients().Length;
            var equations = new int[sizeMatrix];
            var matrix = new double[sizeMatrix,sizeMatrix];
           
            for (int i = 0; i < constraintsValue.Length; i++)
            {
                for (int j = 0; j < sizeMatrix; j++)
                {
                    matrix[i, j] = constraintsValue[i].GetCoefficients()[j];
                }

                equations[i] = constraintsValue[i].GetEquations();
            }

            Function = new Function(ConvertRhsToFunction(constraintsValue),
                function.Aspiration == Aspiration.max ? Aspiration.min : Aspiration.max);

            var newRhs = function.DataFunction;
            
            MatrixTransposition(ref matrix);

            CompilingConstraints(newRhs, equations, matrix);
        }

        private int InvertEquations(int equations)
        {
            switch (equations)
            {
                case AbstractSimplex.LESS_THAN:
                    return AbstractSimplex.GREATER_THAN;
                case AbstractSimplex.GREATER_THAN:
                    return AbstractSimplex.LESS_THAN;
                case AbstractSimplex.EQUAL_TO:
                    return AbstractSimplex.EQUAL_TO;
                default: return AbstractSimplex.EQUAL_TO;
            }
        }

        /// <summary>
        /// Формирование ограничений для двойственной задачи
        /// </summary>
        /// <param name="rhs"></param>
        /// <param name="matrix"></param>
        private void CompilingConstraints(double[] rhs, int[] equations, double [,] matrix)
        {
            var constraints = new List<DConstraint>();
            for (int i = 0; i <= matrix.GetUpperBound(0); i++)
            {
                var coefficients = new double[matrix.GetUpperBound(0)+1];
                for (int j = 0; j <= matrix.GetUpperBound(1); j++)
                {
                    coefficients[j] = matrix[i, j];
                }

                constraints.Add(new DConstraint(coefficients, InvertEquations(equations[i]), rhs[i]));
            }

            Constraints = constraints.ToArray();
        }

        /// <summary>
        /// Конвертация границ в двойственную функцию
        /// </summary>
        /// <param name="constraintsValue"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Транспонирование матрицы
        /// </summary>
        /// <param name="matrix"></param>
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
