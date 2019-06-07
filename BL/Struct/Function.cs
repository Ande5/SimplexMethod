namespace BL.Struct
{
    public enum Aspiration { min, max };
    public struct Function
    {
        public double[] DataFunction { get; private set; }
        public Aspiration Aspiration { get; private set; }

        public Function(double [] dataFunction, Aspiration aspiration)
        {
            DataFunction = dataFunction;
            Aspiration = aspiration;
        }
    }
}
