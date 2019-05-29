using BL;
using System;
using System.Linq;

namespace UI
{
    static class Program
    {
        static void Main(string[] args)
        {
            var inputData = new InputData();
            inputData.ReadDataFile("Simplex Variant_1.txt");

            //Прямой симплекс метод
            var simplex = new InitializeSimplex(inputData.Function, inputData.Constraints);
            simplex.PrintInfo += PrintInfo;
            Console.WriteLine("///Прямая задача///\n");
            Console.WriteLine(simplex.ToString());
            Console.WriteLine("///Симпликс-таблица///");
            Console.WriteLine(simplex.ResultSimplexTabel);
            simplex.AssertResult();

            //Получение интервалов устойчивости в прямой задаче
            Console.WriteLine("///Интервалы устойчивости///\n");
            var rhsList = inputData.Constraints.Select(x => x.GetRhs());
            var stabilityInterval = new StabilityInterval(simplex.MatrixCoefficients, rhsList.ToArray());
            stabilityInterval.IntervalInfo += PrintInfo;
            Console.WriteLine(stabilityInterval.PrintIvertMatrix());
            stabilityInterval.FindingInterval();

            //Формирование двойственной задачи
            var compilingDualTasks = new CompilingDualTasks();
            compilingDualTasks.CompilingTasks(inputData.Function, inputData.Constraints);
            Console.WriteLine("\n///Двойственная задача///\n");
            Console.WriteLine(compilingDualTasks.ToString());

            var dualSimplex = new InitializeSimplex(compilingDualTasks.Function, compilingDualTasks.Constraints);
            dualSimplex.PrintInfo += PrintInfo;
            Console.WriteLine(dualSimplex.ResultSimplexTabel);
            dualSimplex.AssertResult();

            Console.ReadKey();
        }

        private static void PrintInfo(object sender, string data) => Console.WriteLine(data);
    }
}
