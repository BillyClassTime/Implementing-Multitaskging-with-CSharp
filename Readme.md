# Implementing Multitasking in CSharp

**Index** 

- **Creating and starting Task, with delegate, lambda expression and Anonymous Method**
- **Creating and starting Task with run methods shortcut of Task.Factory.StartNew**
- **Waiting for multiple task complete**
- **Returning a Value from a Task**
- **Canceling Task**
- **Using Parallel**
- **Using PLinq**
- **Caching Task Exception**

## Creating and starting Task, with delegate, lambda expression and Anonymous Method

```csharp
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
```

## Creating and starting Task with run methods shortcut of Task.Factory.StartNew

```csharp
public void RunningTaskFactory()
{
    Task[] taskArray = new Task[10]; // Matriz de 10 tareas
    for (int i = 0; i < taskArray.Length; i++)
    {
    	taskArray[i] = Task.Factory.StartNew(
        (Object obj) =>
        {
            CustomData data = obj as CustomData;
            if (data == null)
                return;

  			data.ThreadNum = Thread.CurrentThread.ManagedThreadId;
        },
    	new CustomData() { Name = i, CreationTime = DateTime.Now.Ticks });
    }
    Task.WaitAll(taskArray);
    foreach (var task in taskArray)
    {
        var data = task.AsyncState as CustomData;
        if (data != null)
            WriteLine("Task #{0} created at {1}, ran on thread #{2}.",
            data.Name, data.CreationTime, data.ThreadNum);
    }
}
```

## Waiting for multiple task complete

```csharp
// ..
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
//..
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
```

## Returning a Value from a Task

```csharp
private void RetrievingAValueFromATask()
{
    // Create and queue a task that returns the day of the week as a string.
    Task<string> task1 = Task.Run<string>(() => DateTime.Now.DayOfWeek.ToString());
    // Retrieve and display the task result.
    WriteLine(task1.Result);
}
```

##  Canceling Task

```csharp
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
        WriteLine($"{nameof(OperationCanceledException)} 
        thrown with message: {e.Message}");
    }
    catch (AggregateException e)  
    //Syncrono -> Será generado si el método no es definido como async wait
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
```

## Using Parallel

```Csharp
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
```

## Using PLinq

``` csharp
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
    WriteLine($"Time elapse, generating a secuence of {limit:N}" +
              "agains a IEnumerable list:{sw.ElapsedMilliseconds:N} milliseconds");
    sw.Start();
    var evenNums1 = (from num in serie
                     where num % 2 == 0
                     select num).ToList();
    sw.Stop();
    WriteLine($"Time elapse, querying a {limit:N} numbers finding only evens :" +
              "{sw.ElapsedMilliseconds:N} milliseconds");
    WriteLine("In serie:{0:N} even numbers out of {1:N} total", 
              evenNums1.Count(), serie.Count());

    //sw.Start();
    //var source = Enumerable.Range(1, (int) limit);
    //sw.Stop();
    //WriteLine($"Time elapse, generating a secuence of {limit}" +
    //"with Enumerable Range:{sw.Elapsed.ToString()}");
    sw.Start();
    // Opt in to PLINQ with AsParallel.
    var evenNums = (from num in serie.AsParallel()    //source.AsParallel()
                    where num % 2 == 0
                    select num).ToList();
    sw.Stop();
    WriteLine($"Time elapse, quering a secuence of {limit:N} " + 
              "finding evens in Linq Parallel :{sw.ElapsedMilliseconds:N} milliseconds");
    WriteLine("In serie:{0:N} even numbers out of {1:N} total", evenNums.Count(), 
              serie.Count()); // source.Count());
}
```

## Caching Task Exception

```csharp
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
```

