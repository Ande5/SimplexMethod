using System.Text;
using BL.Simplex;

namespace BL
{
    public abstract class MathematicalModel
    {
        protected Function _function;
        protected DConstraint[] _constraintsValue;

        public Function Function
        {
            get => _function;
            protected set => _function = value;
        }

        public DConstraint[] Constraints
        {
            get => _constraintsValue;
            protected set => _constraintsValue = value;
        }

        protected MathematicalModel() {}

        protected MathematicalModel(Function function, DConstraint[] constraintsValue)
        {
            _function = function;
            _constraintsValue = constraintsValue;
        }

        public override string ToString()
        {
            var model = new StringBuilder();
            foreach (var constraint in _constraintsValue)
            {
                for (int i = 0; i < constraint.GetCoefficients().Length; i++)
                {
                    model.Append(constraint.GetCoefficients()[i] > 0 && i!=0
                        ? $" + {constraint.GetCoefficients()[i]}*x{i+1}"
                        : $" {constraint.GetCoefficients()[i]}*x{i+1}");
                }

                model.Append(CheckEquations(constraint.GetEquations()));
                model.Append(constraint.GetRhs());
                model.Append('\n');
            }

            model.Append("F(x) =");
            int index = 1;
            foreach (var function in _function.DataFunction)
            {
                if (function > 0 && index != 1) model.Append($" + {function}*x{index}");
                else model.Append($" {function}*x{index}");
                index++;
            }

            model.Append($" -> {_function.Aspiration.ToString()}");

            return model.ToString();
        }

        private string CheckEquations(int equations)
        {
            switch (equations)
            {
                case 0:
                    return " <= ";
                case 1:
                    return " >= ";
                case 2:
                    return " = ";
                default:
                    return " = ";
            }
        }
    }
}
