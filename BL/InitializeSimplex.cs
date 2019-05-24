using BL.Simplex;

namespace BL
{
    public class InitializeSimplex
    {
        private DualSimplex _dualSimplex;
        private DConstraint[] _constraintsValue;
        private double[] _targetCoefficients;
        private static int status;

        public InitializeSimplex(int variables, int constraints, DConstraint[] constraintsValue)
        {
            Variables = variables;
            Constraints = constraints;
            _constraintsValue = constraintsValue;
        }

        public int Variables { get; private set; }
        public int Constraints { get; private set; }


        private void Initialize()
        {
            _targetCoefficients = new double[Variables];
            _constraintsValue = new DConstraint[Constraints];

            //TODO:Добавить считывание из файла
            for (int i = 0; i < Variables; i++)
            {
                
               // _targetCoefficients[i] = double.Parse(objFunction[i].Text);
            }

            for (int i = 0; i < Constraints; i++)
            {
                // Stores constraint values at given index
                double[] consValues = new double[Variables];
               
                //TODO:Take right side of constraint
               // double right = double.Parse(constraintFunctionRight[i].Text);

                //TODO:Take all fields and parse to double
                //for (int j = 0; j < Variables; j++)
                //{
                //    consValues[j] = double.Parse(constraintFunction[i][j].Text);
                //}
                // Represents sign of constraint
                int bound = 0;

                //if (constraintSigns[i].SelectedIndex == 0) bound = AbstractSimplex.LESS_THAN;
                //else if (constraintSigns[i].SelectedIndex == 1) bound = AbstractSimplex.EQUAL_TO;
                //else bound = AbstractSimplex.GREATER_THAN;

                // Create object of DContraint passing arguments of variables, sign and right side, and store it to array
                //_constraintsValue[i] = new DConstraint(consValues, bound, right);
            }

            InitDualSimplex(true);
            Solve();
            AssertResult();
        }

        /// <summary>
        /// Инициализация двойственого симплекс метода
        /// </summary>
        /// <param name="targetCoefficients"></param>
        /// <param name="minimize"></param>
        private void InitDualSimplex(bool minimize)
        {
            _dualSimplex = new DualSimplex();
            _dualSimplex.SetObjective(_targetCoefficients, minimize);

            var constraintArray = new double[_constraintsValue.Length][];
            for (int i = 0; i < _constraintsValue.Length; i++)
            {
                constraintArray[i] = new double[_targetCoefficients.Length];
            }

            var rhs = new double[_constraintsValue.Length];
            var equations = new int[_constraintsValue.Length];
       
            for (int i = 0; i < _constraintsValue.Length; ++i)
            {
                constraintArray[i] = _constraintsValue[i].GetCoefficients();
                equations[i] = _constraintsValue[i].GetEquations();
                rhs[i] = _constraintsValue[i].GetRhs();
            }
        }

        private void Solve()
        {
            while ((status = _dualSimplex.Iterate()) == AbstractSimplex.CONTINUE)
            {

            }
        }

        /// <summary>
        /// Вывод результата
        /// </summary>
        private void AssertResult()
        {
            // Get coefficients of objective function from simplex
            double[] targetCoefficientValues = _dualSimplex.GetCoefficients();
            // Get answer function
            string res = "Z = " + _dualSimplex.GetObjectiveResult() + "\n";
            // We do some formatting data here
            for (int i = 0; i < _targetCoefficients.Length; ++i)
            {
                res = res + " Target Coefficient#" + i + " : " + targetCoefficientValues[i] + "\n";
            }
        }
    }
}
