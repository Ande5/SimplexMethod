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
             initializeSimplex.AssertResult();
            
            Console.ReadKey();
        }

        private static void InputData_PrintInfo(object sender, string data) => Console.WriteLine(data);
    }
}
