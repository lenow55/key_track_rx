public class FileLogger : IObserver<ConsoleKeyInfo>
{
    private readonly string _filePath;
    private StreamWriter? _writer;

    public FileLogger(string filePath)
    {
        _filePath = filePath;
        _writer = new StreamWriter(_filePath, append: true) { AutoFlush = true };
    }

    public async Task WriteToFileAsync(string content)
    {
        if (_writer != null)
        {
            await _writer.WriteLineAsync(content);
        }
    }

    public void OnNext(ConsoleKeyInfo keyInfo)
    {
        // Асинхронная запись в файл
        Task.Run(
            () =>
                WriteToFileAsync(
                    $"{DateTime.Now}: [LOG]: Клавиша: {keyInfo.Key}, Модификаторы: {keyInfo.Modifiers}"
                )
        );
    }

    public void OnError(Exception error)
    {
        // Асинхронная запись ошибки в файл
        Task.Run(() => WriteToFileAsync($"{DateTime.Now}: [ERROR]: {error.Message}"));
    }

    public void OnCompleted()
    {
        // Асинхронно записываем сообщение и закрываем файл
        var task = Task.Run(() =>
        {
            if (_writer != null)
            {
                WriteToFileAsync($"{DateTime.Now}: [Log]: Трекер завершил работу.").Wait(); // Подождем завершения записи
                _writer.Close();
            }
        });
        task.Wait(100);
    }
}
