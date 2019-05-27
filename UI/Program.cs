using BL;
using System;

namespace UI
{
    static class Program
    {

        static void Main(string[] args)
        {
             var inputData = new InputData();
             inputData.PrintInfo += InputData_PrintInfo;
             inputData.ReadDataFile("Simplex Variant_1.txt");

              var initializeSimplex = new InitializeSimplex(inputData.Function, inputData.Constraints);
              initializeSimplex.PrintInfo += InputData_PrintInfo;
              Console.WriteLine(initializeSimplex.SimplexTabel);
              initializeSimplex.AssertResult();

            var compilingDualTasks = new CompilingDualTasks();
            compilingDualTasks.CompilingTasks(inputData.Function, inputData.Constraints);


            var initializeSimplex1 = new InitializeSimplex(compilingDualTasks.Function, compilingDualTasks.Constraints);
            initializeSimplex1.PrintInfo += InputData_PrintInfo;
            Console.WriteLine(initializeSimplex1.SimplexTabel);
            initializeSimplex1.AssertResult();

            Console.ReadKey();
        }

        private static void InputData_PrintInfo(object sender, string data) => Console.WriteLine(data);
    }
}
