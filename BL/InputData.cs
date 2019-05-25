using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BL.Simplex;

namespace BL
{
    public delegate void DataInfo(object sender, string data);

    public class InputData
    {
        public Function Function { get; private set; }
        private readonly List<double> _dataFunction = new List<double>();
        private readonly List<DConstraint> _constraints = new List<DConstraint>();

        public DConstraint[] Constraints => _constraints.ToArray();

        public void ReadDataFile(string path)
        {
            using (var readFile = new StreamReader(path))
            {
                while (!readFile.EndOfStream)
                {
                    var objValue = readFile.ReadLine();
                    var function = objValue?.Split().FirstOrDefault(x => x == "F(x)");
                    if (!string.IsNullOrEmpty(objValue) && !string.IsNullOrEmpty(function))
                    {
                        SearchObjFunction(objValue);
                        PrintInfo?.Invoke(this, objValue.Trim());
                    }
                    else if (!string.IsNullOrWhiteSpace(objValue) && string.IsNullOrEmpty(function))
                    {
                        ReadRestrictions(objValue);
                        PrintInfo?.Invoke(this, objValue.Trim());
                    }
                }
            }
        }

        public event DataInfo PrintInfo;

        private void SearchObjFunction(string objFunction)
        {
            RegexOptions options = RegexOptions.Multiline;
            string pattern = @"(?!min|max|x|F)\s([-]?[0-9]*[.,]?[0-9])";

            var functionAspiration = objFunction?.Split().FirstOrDefault(x => x == "min" || x == "max");

            foreach (Match m in Regex.Matches(objFunction ?? throw new InvalidOperationException(), pattern, options))
            {
                _dataFunction.Add(double.Parse(m.Value));
            }

            Function = new Function(_dataFunction.ToArray(), functionAspiration == "max" ? Aspiration.max : Aspiration.min);
        }

        private void ReadRestrictions(string input)
        {
            var patternBound = @"(\s[>=<]+)";
            var patternLeft = @"\s([-]?[0-9]*[.,]?[0-9])";
            var patternFull = @"(([-]?[0-9]*[.,]?[0-9])\s*([^>=<]*))";
           
            RegexOptions options = RegexOptions.Multiline;
            var restriction = Regex.Matches(input, patternFull, options);

            var leftRestriction = " "+ restriction[0].Value;
            var rightRestriction =  double.Parse(restriction[1].Value);
            List<double> consValues = new List<double>();

            foreach (Match objectMatch in Regex.Matches(leftRestriction, patternLeft, options))
            {
                 consValues.Add(double.Parse(objectMatch.Value.Trim()));
            }

            var boundMacth = Regex.Matches(input, patternBound, options);
            var bound = 0;
            switch (boundMacth[0].Value.Trim())
            {
                case "<=":
                {
                    bound = AbstractSimplex.LESS_THAN;
                    break;
                }
                case ">=":
                {
                    bound = AbstractSimplex.GREATER_THAN;
                    break;
                }
                case "=":
                {
                    bound = AbstractSimplex.EQUAL_TO;
                    break;
                }
            }

            _constraints.Add(new DConstraint(consValues.ToArray(),bound, rightRestriction));
          
        }
    }
}
