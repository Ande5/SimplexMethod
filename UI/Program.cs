using BL;
using System;
using System.Linq;

namespace UI
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Simplex";

            var inputData = new InputData();
            inputData.ReadDataFile("Simplex Variant.txt");

            //Прямой симплекс метод
            var simplex = new InitializeSimplex(inputData.Function, inputData.Constraints);
            simplex.PrintInfo += PrintInfo;
            Console.WriteLine("///Прямая задача///\n");
            Console.WriteLine(simplex.ToString());
            Console.WriteLine("///Симпликс-таблица///");
            Console.WriteLine(simplex.SimplexTabel);
            simplex.AssertResult();
            Console.WriteLine("///Теневые оценки///\n");
            Console.WriteLine(simplex.PrintShadowEstimates());

            //Получение интервалов устойчивости в прямой задаче
            var rhsList = inputData.Constraints.Select(x => x.GetRhs());
            var stabilityInterval = new StabilityInterval(simplex.MatrixCoefficients, rhsList.ToArray());
            stabilityInterval.IntervalInfo += PrintInfo;
            Console.WriteLine("///Обратная матрица A^-1///\n");
            Console.WriteLine(stabilityInterval.PrintInvertMatrix());
            Console.WriteLine("///Интервалы устойчивости///\n");
            stabilityInterval.FindingInterval();

            //Формирование двойственной задачи
            var compilingDualTasks = new CompilingDualTasks();
            compilingDualTasks.CompilingTasks(inputData.Function, inputData.Constraints);
            Console.WriteLine("\n///Двойственная задача///\n");
            Console.WriteLine(compilingDualTasks.ToString());

            var dualSimplex = new InitializeSimplex(compilingDualTasks.Function, compilingDualTasks.Constraints);
            dualSimplex.PrintInfo += PrintInfo;
            Console.WriteLine("///Симпликс-таблица///");
            Console.WriteLine(dualSimplex.SimplexTabel);
            dualSimplex.AssertResult();

            Console.ReadKey();
        }

        private static void PrintInfo(object sender, string data) => Console.WriteLine(data);
    }
}
