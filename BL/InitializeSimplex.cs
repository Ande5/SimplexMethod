using System.Collections.Generic;
using BL.Simplex;

namespace BL
{
    public class InitializeSimplex: MathematicalModel
    {
        private DualSimplex _dualSimplex;

        public int Variables { get; set; }
        public new int Constraints { get; set; }
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
            _dualSimplex.SimplexInfo += DualSimplex_SimplexInfo;
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

        private void DualSimplex_SimplexInfo(object sender, string data) => SimplexTabel += '\n'+ data;

        private void IterationSolution()
        {
            while (true)
            {
                if (_dualSimplex.Iterate() == AbstractSimplex.CONTINUE) continue;
                MatrixCoefficients = _dualSimplex.GetConstraint();
                break;
            }
        }

        public event DataInfo PrintInfo;

        /// <summary>
        /// Вывод результата
        /// </summary>
        public void AssertResult()
        {
            // Get coefficients of objective function from simplex
            var targetCoefficientValues = _dualSimplex.GetCoefficients();
           
            // We do some formatting data here
            for (var i = 0; i < Variables; ++i)
            {
                PrintInfo?.Invoke(this, $"x{i} : {targetCoefficientValues[i]:f3} ");
            }

            // Get answer function
            var res = "F(x) = " + _dualSimplex.GetObjectiveResult();
            PrintInfo?.Invoke(this, res);
        }
    }
}
