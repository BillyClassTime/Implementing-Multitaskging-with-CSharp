using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using static System.Console;
namespace ImplementingMultitaskging
{

    class UsingParallel
    {
        public void usingParallelFor()
        {
            var sw = new Stopwatch();

            int from = 0;
            int to = 500000;
            double[] array = new double[500000];
            // This is a sequential implementation:
            sw.Start();
            for (int index = 0; index < 500000; index++)
            {
                array[index] = Math.Sqrt(index);
            }
            sw.Stop();
            WriteLine($"Tiempo en milisegundos:{sw.ElapsedMilliseconds}");
            sw.Start();
            // This is the equivalent parallel implementation:
            Parallel.For(from, to, index =>
            {
                array[index] = Math.Sqrt(index);
            });
            sw.Stop();
            WriteLine($"Tiempo en milisegundos:{sw.ElapsedMilliseconds}");
        }


    }

}