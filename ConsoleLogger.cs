public class ConsoleLogger : IObserver<ConsoleKeyInfo>
{
    public void OnNext(ConsoleKeyInfo keyInfo)
    {
        Console.WriteLine($"[LOG]: Клавиша: {keyInfo.Key}, Модификаторы: {keyInfo.Modifiers}");
    }

    public void OnError(Exception error)
    {
        Console.WriteLine($"[ERROR]: {error.Message}");
    }

    public void OnCompleted()
    {
        Console.WriteLine("[LOG]: Трекер завершил работу.");
    }
}
