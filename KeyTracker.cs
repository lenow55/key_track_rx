using System.Reactive.Subjects;

public class KeyTracker : IObservable<ConsoleKeyInfo>
{
    private readonly Subject<ConsoleKeyInfo> _subject = new(); // Управляет подписками и передачей событий
    private bool _isTracking = true;

    public IDisposable Subscribe(IObserver<ConsoleKeyInfo> observer)
    {
        // Позволяет подписчикам подключаться
        return _subject.Subscribe(observer);
    }

    public async Task StartTracking(CancellationToken cancellationToken)
    {
        Console.WriteLine("Начало отслеживания клавиш. Нажмите ESC для завершения.");
        try
        {
            while (!cancellationToken.IsCancellationRequested && this._isTracking)
            {
                if (Console.KeyAvailable) // Проверяет, есть ли нажатия клавиш
                {
                    var key = Console.ReadKey(intercept: true); // Получение события клавиатуры

                    if (key.Key == ConsoleKey.Escape) // Завершение работы по клавише ESC
                    {
                        _subject.OnCompleted();
                        break;
                    }
                    else
                    {
                        _subject.OnNext(key);
                    }
                }

                await Task.Delay(10, cancellationToken); // Добавляем небольшую задержку, чтобы не нагружать CPU
            }
        }
        catch (OperationCanceledException)
        {
            // Ловим отмену через CancellationToken
            Console.WriteLine("Отслеживание клавиш было отменено.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            _subject.OnError(ex);
        }
        finally
        {
            _subject.OnCompleted();
        }
    }
}
