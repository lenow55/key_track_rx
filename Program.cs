class Program
{
    static async Task Main(string[] args)
    {
        // Создаем токен для обработки SIGTERM
        using var cts = new CancellationTokenSource();

        // Обработка SIGTERM (или Ctrl+C)
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            Console.WriteLine("Получен сигнал завершения (SIGTERM).");
            cts.Cancel();
            eventArgs.Cancel = true; // Не завершать процесс сразу
        };

        KeyTracker tracker = new KeyTracker();

        // Подписчики
        var fileLogger1 = new FileLogger("key_events1.log");
        var fileLogger2 = new FileLogger("key_events2.log");
        IObserver<ConsoleKeyInfo> consoleLogger = new ConsoleLogger();

        tracker.Subscribe(fileLogger1);
        tracker.Subscribe(fileLogger2);
        tracker.Subscribe(consoleLogger);
        Task trackingTask = tracker.StartTracking(cts.Token);

        try
        {
            // Ожидание завершения задач или сигнала SIGTERM
            Console.WriteLine("Нажмите ESC для выхода...");
            await Task.WhenAny(trackingTask, WaitForCancellation(cts.Token));
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Операция отменена.");
        }
        Console.WriteLine("Программа завершена.");
    }

    static Task WaitForCancellation(CancellationToken token)
    {
        var tcs = new TaskCompletionSource<object?>();
        token.Register(() => tcs.TrySetResult(null));
        return tcs.Task;
    }
}
