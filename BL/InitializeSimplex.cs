using BL.Simplex;

namespace BL
{
    public class InitializeSimplex
    {
        private DualSimplex _dualSimplex;
        private readonly Function _function;
        private readonly DConstraint[] _constraintsValue;
      
        public int Variables { get; set; }
        public int Constraints { get; set; }

        public InitializeSimplex(Function function, DConstraint[] constraintsValue, bool autoVariableConstraints = true)
        {
            _function = function;
            _constraintsValue = constraintsValue;

            Initialize(autoVariableConstraints);          
        }

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


        private void IterationSolution()
        {
            while (true)
            {
                if (_dualSimplex.Iterate() == AbstractSimplex.CONTINUE) continue;
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
            // Get answer function
            var res = "F(x) = " + _dualSimplex.GetObjectiveResult();
            PrintInfo?.Invoke(this, res);
            // We do some formatting data here
            for (var i = 0; i < Variables; ++i)
            {
                PrintInfo?.Invoke(this, " Target Coefficient#" + i + " : " + targetCoefficientValues[i]);
            }
        }
    }
}
