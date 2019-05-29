using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BL.Extensions;
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

        /// <summary>
        /// Чтение данных из файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
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
                        SearchObjConstraints(objValue);
                        PrintInfo?.Invoke(this, objValue.Trim());
                    }
                }
            }
        }

        public event DataInfo PrintInfo;

        /// <summary>
        /// Поиск объектов функции
        /// </summary>
        /// <param name="objFunction">Исходня функция</param>
        private void SearchObjFunction(string objFunction)
        {
            const RegexOptions options = RegexOptions.Multiline;
            const string pattern = @"(?!min|max|x|F)([+-]?\s[+-]?[0-9]*[.,]?[0-9])";

            var functionAspiration = objFunction?.Split().FirstOrDefault(x => x == "min" || x == "max");

            foreach (Match m in Regex.Matches(objFunction ?? throw new InvalidOperationException(), pattern, options))
            {
                _dataFunction.Add(double.Parse(m.Value.ReplacePointToComma()));
            }

            Function = new Function(_dataFunction.ToArray(), functionAspiration == "max" ? Aspiration.max : Aspiration.min);
        }

        /// <summary>
        /// Поиск объектов ограничений
        /// </summary>
        /// <param name="input">Ограничения</param>
        private void SearchObjConstraints(string input)
        {
            const string patternBound = @"(\s[>=<]+)";
            const string patternLeft = @"([+-]?\s[+-]?[0-9]*[.,]?[0-9])";
            const string patternFull = @"(([-]?[0-9]*[.,]?[0-9])\s*([^>=<]*))";
           
            const RegexOptions options = RegexOptions.Multiline;
            var restriction = Regex.Matches(input, patternFull, options);

            var leftRestriction = " "+ restriction[0].Value;
            var rightRestriction =  double.Parse(restriction[1].Value);

            var consValues = new List<double>();

            foreach (Match objectMatch in Regex.Matches(leftRestriction, patternLeft, options))
                consValues.Add(double.Parse(objectMatch.Value.ReplacePointToComma()));

            var boundMatch = Regex.Matches(input, patternBound, options);
            var bound = DefineBound(boundMatch[0].Value.Trim());

            _constraints.Add(new DConstraint(consValues.ToArray(),bound, rightRestriction));
          
        }

        /// <summary>
        /// Определение знака неравенства ограничений
        /// </summary>
        /// <param name="boundMatch">Знака неравенства</param>
        /// <returns>Значение неравенства</returns>
        private static int DefineBound(string boundMatch)
        {
            switch (boundMatch)
            {
                case "<=":
                    return AbstractSimplex.LESS_THAN;
                case ">=":
                    return AbstractSimplex.GREATER_THAN;
                case "=":
                    return AbstractSimplex.EQUAL_TO;
                default:
                    return AbstractSimplex.EQUAL_TO;
            }
        }

    }
}
