namespace BL.Simplex
{
    public class DConstraint
    {
        // store here coefficients of contraint
        private double[] coefficients;
        // sign of constraint
        private int equations;
        // right side
        private double rhs;

        // Constructor
        public DConstraint(double[] coefficients, int equations, double rhs)
        {
            this.coefficients = coefficients;
            this.equations = equations;
            this.rhs = rhs;
        }

        // Getters and setters
        public double[] GetCoefficients() => coefficients;

        public int GetEquations() => equations;

        public double GetRhs() => rhs;
    }
}
