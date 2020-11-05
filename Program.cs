using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
namespace ImplementingMultitaskging
{
    class Program
    {
        static void Main(string[] args)
        {
            //new Program().Task1(); // Creating and starting Task, with delegate, lambda expression and Anonymous Method
            //new Program().Task2(); 
            // Creating and starting Task with run methods shortcut of Task.Factory.StartNew
            //new UsingTaskFactory().RunningTaskFactory(); //Task.Factory.StartNew is highly configurable and
            //accepts a wide range of parameters.
            //new Program().WaitingForMultipleTaskToComplete();                                                             
            //new Program().RetrievingAValueFromATask(); //Returning a Value from a Task

            //new Program().CancellingTask(); // Calling Sync
            /*var tasking = new Program().CancellingTask(); // Calling Async
            WriteLine(tasking.Status);
            //try
            //{
                //tasking.ContinueWith(t => WriteLine("waiting for task"));
                tasking.Wait();
            /*}
            catch (SystemException e)
            {
                WriteLine($"Message: {e.Message}");
            }
            finally
            {
                tasking.Dispose();
            }*/

            //new UsingParallel().usingParallelFor();
            //new UsingPLinq().PLinq();
            //new UsingPLinq().PLinqSecondMeasure();

            // Handling Task Exceptions 
            new Program().CachingTaskException();
        }

        private void Task1()
        {
            // Creating a Task by Using an Action Delegate 
            var taskName = "task1";
            Task task1 = new Task(GetTheTime, taskName);
            task1.Start();  // Using the Task.Start Method to Queue a Task 
            task1.Wait();   // Waiting for a Single Task to Complete 

            // Creating a Task by Using an Anonymous Delegate 
            Task task2 = new Task(delegate
            {
                WriteLine("task2:The time now is {0}", DateTime.Now);
            });
            task2.Start(); // Using the Task.Start Method to Queue a Task 
            task2.Wait();  // Waiting for a Single Task to Complete 

            //Using Lambda Expressions to Create Tasks and Invoke a no anonymous Method
            Task task3 = new Task(() => MyMethod("task3"));
            // This is equivalent to: 
            //Task task3 = new Task( delegate(MyMethod) ); 
            task3.Start(); // Using the Task.Start Method to Queue a Task 
            task3.Wait();  // Waiting for a Single Task to Complete 

            //Using a Lambda Expression to Invoke an Anonymous Method 
            Task task4 = new Task(() => { WriteLine("task4:Test"); });
            // This is equivalent to: 
            //Task task4 = new Task( delegate { WriteLine("Test") }
            task4.Start(); // Using the Task.Start Method to Queue a Task 
            task4.Wait();  // Waiting for a Single Task to Complete 

        }
        private static void GetTheTime(object nameTask)
        {
            WriteLine("{0}:The time now is {1}", nameTask, DateTime.Now);
        }
        private void MyMethod(string nameTask)
        {
            WriteLine($"{nameTask}:Test");
        }
        private void Task2()
        {
            Thread.CurrentThread.Name = "Main";

            // Create a task and supply a user delegate by using a lambda expression.
            Task taskA = new Task(() => WriteLine("Hello from taskA."));
            // Start the task.
            taskA.Start();

            Task taskB = new Task(delegate { WriteLine("Hello from taskB"); });
            // Start the task B
            taskB.Start();

            // Output a message from the calling thread.
            WriteLine("Hello from thread '{0}'.", Thread.CurrentThread.Name);
            taskB.Wait();
            taskA.Wait();
        }
        private void Task3()
        {
            Thread.CurrentThread.Name = "Main";

            // Define and run the task.
            Task taskA = Task.Run(() => WriteLine("Hello from taskA."));

            Task taskB = Task.Run(delegate { WriteLine("Hello from taskB"); });

            // Output a message from the calling thread.
            WriteLine("Hello from thread '{0}'.", Thread.CurrentThread.Name);
            taskB.Wait();
            taskA.Wait();
        }
        #region WaitingForMultipleTask
        private void WaitingForMultipleTaskToComplete()
        {
            Task[] tasks = new Task[3]
            {
            Task.Run( () => LongRunningMethodA()),
            Task.Run( () => LongRunningMethodB()),
            Task.Run( () => LongRunningMethodC())
            };
            // Wait for any of the tasks to complete.
            //Task.WaitAny(tasks);
            // Alternatively, wait for all of the tasks to complete.
            Task.WaitAll(tasks);
            // Continue with execution. 
            WriteLine("End of method");
        }
        private void LongRunningMethodA()
        {
            for (int i = 0; i < Int32.MaxValue;) //WaitAny:22222000 //WaitAll:8888000
            {
                i++;
            }
            WriteLine("\nAtEnd of Method A");
        }
        private void LongRunningMethodB()
        {
            for (int i = 2000; i < 3000;)
            {
                i++;
            }
            WriteLine("\nAtEnd of Method B");
        }
        private void LongRunningMethodC()
        {
            for (int i = 4000; i < 60000;) //WaitAny: 55555000 //WaitAll: 60000
            {
                i++;
            }
            WriteLine("\nAtEnd of Method C");
        }
        #endregion
        private void RetrievingAValueFromATask()
        {
            // Create and queue a task that returns the day of the week as a string.
            Task<string> task1 = Task.Run<string>(() => DateTime.Now.DayOfWeek.ToString());
            // Retrieve and display the task result.
            WriteLine(task1.Result);
        }

        //private async Task CancellingTask()
        private void CancellingTask()
        {
            var tokenSource2 = new CancellationTokenSource();
            CancellationToken ct = tokenSource2.Token;
            var task = Task.Run(() =>
            {
                // Were we already canceled?
                ct.ThrowIfCancellationRequested();

                bool moreToDo = true;
                while (moreToDo)
                {
                    // Poll on this property if you have to do
                    // other cleanup before throwing.
                    if (ct.IsCancellationRequested)
                    {
                        // Clean up here, then...
                        ct.ThrowIfCancellationRequested();
                    }
                }
            }, tokenSource2.Token); // Pass same token to Task.Run.

            tokenSource2.Cancel();

            // Just continue on this thread//, or await with try-catch:
            try
            {
                //await task;
                task.Wait();
            }
            catch (OperationCanceledException e) // Cuando sea asyncrono
            {
                WriteLine($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
            }
            catch (AggregateException e)  //Syncrono -> Será generado si el método no es definido como async wait
            {
                WriteLine($"Message: {e.Message}");
            }
            catch (SystemException e)
            {
                WriteLine($"Message: {e.Message}");
            }
            finally
            {
                tokenSource2.Dispose();
            }
            WriteLine("Press Any Key to continue...");
            ReadKey();
        }

        private void CachingTaskException()
        {
            // Create a cancellation token source and obtain a cancellation token.
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;
            // Create and start a long-running task.
            var task1 = Task.Run(() => doWork(ct), ct);
            // Cancel the task.
            cts.Cancel();
            // Handle the TaskCanceledException.
            try
            {
                task1.Wait();
            }
            catch (AggregateException ae)
            {
                foreach (var inner in ae.InnerExceptions)
                {
                    if (inner is TaskCanceledException)
                    {
                        WriteLine($"The task was cancelled.\n{inner.Message}");
                        ReadLine();
                    }
                    else
                    {
                        // If it's any other kind of exception, re-throw it.
                        throw;
                    }
                }
            }
        }
        // Method run by the task.
        private void doWork(CancellationToken token)
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.SpinWait(500000);
                // Throw an OperationCanceledException if cancellation was requested.
                token.ThrowIfCancellationRequested();
            }
        }
    }
}
