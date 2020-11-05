using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using static System.Console;

namespace ImplementingMultitaskging
{

    public class UsingPLinq
    {
        protected internal long limit = 1000000000;
        public void PLinq()
        {
            // PLINQ bad perfomance: https://bit.ly/3k4ubTz
            // Open Task Manager to view the memory and processor utilizations
            // Create a System.Diagnostics.Stopwatch.
            var sw = new Stopwatch();
            WriteLine("Watch starting...");
            sw.Start();
            IEnumerable<long> serie = Fill();
            serie.ToList();
            sw.Stop();
            WriteLine($"Time elapse, generating a secuence of {limit:N} agains a IEnumerable list:{sw.ElapsedMilliseconds:N} milliseconds");
            sw.Start();
            var evenNums1 = (from num in serie
                             where num % 2 == 0
                             select num).ToList();
            sw.Stop();
            WriteLine($"Time elapse, querying a {limit:N} numbers finding only evens :{sw.ElapsedMilliseconds:N} milliseconds");
            WriteLine("In serie:{0:N} even numbers out of {1:N} total", evenNums1.Count(), serie.Count());

            //sw.Start();
            //var source = Enumerable.Range(1, (int) limit);
            //sw.Stop();
            //WriteLine($"Time elapse, generating a secuence of {limit} with Enumerable Range:{sw.Elapsed.ToString()}");
            sw.Start();
            // Opt in to PLINQ with AsParallel.
            var evenNums = (from num in serie.AsParallel()    //source.AsParallel()
                            where num % 2 == 0
                            select num).ToList();
            sw.Stop();
            WriteLine($"Time elapse, quering a secuence of {limit:N} finding evens in Linq Parallel :{sw.ElapsedMilliseconds:N} milliseconds");
            WriteLine("In serie:{0:N} even numbers out of {1:N} total", evenNums.Count(), serie.Count()); // source.Count());
        }

        static IEnumerable<long> Fill()
        {
            long limit = 1000000000;
            for (long i = 0; i < limit; i++)
            {
                yield return i;
            }
        }


        public void PLinqSecondMeasure()
        {
            Write("Inicio de prueba");
            Stopwatch sw = new Stopwatch();

            int[] vals = Enumerable.Range(0, Int16.MaxValue).ToArray();

            sw.Start();
            int[] x1 = vals.Where(IsValid).ToArray();
            sw.Stop();
            WriteLine("Sequential Execution {0} milliseconds", sw.ElapsedMilliseconds);

            sw.Restart();
            int[] x2 = vals.AsParallel().Where(IsValid).ToArray();
            sw.Stop();
            WriteLine("Parallel #1 Execution {0} milliseconds", sw.ElapsedMilliseconds);

            sw.Restart();
            int[] x3 = vals.AsParallel().Where(IsValid).ToArray();
            sw.Stop();
            WriteLine("Parallel #2 Execution {0} milliseconds", sw.ElapsedMilliseconds);
            Write("Fin de prueba");
        }

        static bool IsValid(int input)
        {
            int result = 0;

            for (int i = 0; i < 50000; i++)
                result = input % 2;

            return result == 0;
        }

    }

}
