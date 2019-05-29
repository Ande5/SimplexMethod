using System;
using System.Collections.Generic;
using System.Linq;
using BL.Simplex;

namespace BL
{
    public class StabilityInterval
    {
        private readonly double [] _rhs;
        private readonly List<Interval> _intervals = new List<Interval>();
        private readonly List<DConstraint> _matrixCoefficients;

        public StabilityInterval(List<DConstraint> matrixCoefficients, double [] rhs)
        {
            _rhs = rhs;
            _matrixCoefficients = matrixCoefficients;
          
        }

        /// <summary>
        /// Нахождение интервала устойчиовтси
        /// </summary>
        public void FindingInterval()
        {
           var lowerInterval = LowerInterval();
           var upperInterval = UpperInterval();

            for (int i = 0; i < _rhs.Length; i++)
            {
                var a = _rhs[i] - Math.Abs(lowerInterval[i]);
                var b = _rhs[i] + Math.Abs(upperInterval[i]);
                _intervals.Add(new Interval(a,b));
                IntervalInfo?.Invoke(this, $" [{a:f2} - {b,6:f2}] => {_rhs[i]}");
            }
        }

        public event DataInfo IntervalInfo;

        /// <summary>
        /// Нижние границы устойчивости
        /// </summary>
        /// <returns>Список границ</returns>
        private List<double> LowerInterval()
        {
            var lowerInterval = new List<double>();

            foreach (var coefficient in _matrixCoefficients)
            {
                var deltaB = new double[coefficient.GetCoefficients().Length];
                for (int k = 0; k < coefficient.GetCoefficients().Length; k++)
                {
                    if (coefficient.GetCoefficients()[k] > 0)
                    { 
                        var deltaBl = _matrixCoefficients[k].GetRhs() / coefficient.GetCoefficients()[k];
                        deltaB[k] = deltaBl;
                    }
                }

                lowerInterval.Add(deltaB.Where(min => min > 0).Min());
            }

            return lowerInterval;
        }

        /// <summary>
        /// Вверхние границы устойчивости
        /// </summary>
        /// <returns>Список границ</returns>
        private List<double> UpperInterval()
        {
            var upperInterval = new List<double>();

            foreach (var coefficient in _matrixCoefficients)
            {
                var deltaB = new double[coefficient.GetCoefficients().Length];
                for (int k = 0; k < coefficient.GetCoefficients().Length; k++)
                {
                    if (coefficient.GetCoefficients()[k] < 0)
                    {
                        var deltaBl = _matrixCoefficients[k].GetRhs() / coefficient.GetCoefficients()[k];
                        deltaB[k] = deltaBl;
                    }
                }

                upperInterval.Add(deltaB.Where(min => min < 0).Max());
            }

            return upperInterval;
        }


        public string PrintIvertMatrix()
        {
            var invertMatrix = string.Empty;
            foreach (var coefficient in _matrixCoefficients)
            {

                for (int k = 0; k < coefficient.GetCoefficients().Length; k++)
                {
                    invertMatrix += $"  {coefficient.GetCoefficients()[k]:f2}";
                }

                invertMatrix += Environment.NewLine;
            }

            return invertMatrix;
        }
    }
}
