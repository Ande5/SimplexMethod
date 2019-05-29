using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BL.Simplex;

namespace BL
{
    public class InitializeSimplex: MathematicalModel
    {
        private DualSimplex _dualSimplex;

        public int Variables { get; set; }

        public new int Constraints { get; set; }

        public double[] ShadowEstimates { get; private set; }

        public List<DConstraint> MatrixCoefficients { get; private set; }

        public InitializeSimplex(Function function, DConstraint[] constraintsValue, bool autoVariableConstraints = true)
            : base(function, constraintsValue) => Initialize(autoVariableConstraints);

        private void Initialize(bool autoVariableConstraints)
        {
            if (!autoVariableConstraints) return;
            Constraints = _constraintsValue.Length;
            Variables = _function.DataFunction.Length;

            InitDualSimplex();
            IterationSolution();
        }

        /// <summary>
        /// Инициализация двойственого симплекс метода
        /// </summary>     
        private void InitDualSimplex()
        {
            _dualSimplex = new DualSimplex();
            _dualSimplex.SimplexInfo += SimplexInfo;
            _dualSimplex.SetObjective(_function);

            var constraintArray = new double[_constraintsValue.Length][];
            for (int i = 0; i < _constraintsValue.Length; i++)
            {
                constraintArray[i] = new double[Variables];
            }

            var rhs = new double[_constraintsValue.Length];
            var equations = new int[_constraintsValue.Length];
       
            for (int i = 0; i < _constraintsValue.Length; ++i)
            {
                constraintArray[i] = _constraintsValue[i].GetCoefficients();
                equations[i] = _constraintsValue[i].GetEquations();
                rhs[i] = _constraintsValue[i].GetRhs();
            }

            _dualSimplex.SetConstraints(constraintArray, equations, rhs);
            _dualSimplex.Init();
        }

        public string SimplexTabel { get; private set; }

        public string ResultSimplexTabel { get; private set; }

        private void SimplexInfo(object sender, string data)
        {
            SimplexTabel += '\n' + data;
            ResultSimplexTabel = '\n' + data;
        } 

        private void IterationSolution()
        {
            while (true)
            {
                if (_dualSimplex.Iterate() == AbstractSimplex.CONTINUE) continue;
                MatrixCoefficients = _dualSimplex.GetConstraint();
                ShadowEstimates = _dualSimplex.GetShadowEstimates();
                break;
            }
        }

        public event DataInfo PrintInfo;

        public string PrintShadowEstimates() 
            => ShadowEstimates.Aggregate("y = (", (current, shadowEstimate) => current + $" {shadowEstimate:f2};") + ")\n";

        /// <summary>
        /// Вывод результата
        /// </summary>
        public void AssertResult()
        {
            // Get coefficients of objective function from simplex
            var targetCoefficientValues = _dualSimplex.GetCoefficients();

            var str = "///Результат решения симплекс-методом///\n";

            // We do some formatting data here
            str += "\nx = (";
            for (var i = 0; i < Variables; ++i)
            {
              //  str += $"x{i+1}: {targetCoefficientValues[i]:f2}\n";
                str += $" {targetCoefficientValues[i]:f2};";
                // PrintInfo?.Invoke(this, $" x{i} : {targetCoefficientValues[i]:f3} ");
            }

            str += ") ";

            // Get answer function
            var res = "F(x) = " + _dualSimplex.GetObjectiveResult();
            str += $"F(x) = {_dualSimplex.GetObjectiveResult():f2}" + Environment.NewLine;
            PrintInfo?.Invoke(this, str);
        }
    }
}
