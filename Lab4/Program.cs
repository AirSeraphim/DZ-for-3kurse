using System;
using System.Collections.Generic;           // Для List<T>, массивов и других коллекций
using System.Diagnostics;                   // Для Stopwatch — измерения времени выполнения
using System.IO;                            // Для работы с файлами: File, Directory, FileInfo
using System.Linq;                          // Для методов расширений: Count, Sum, Where и др.
using System.Threading.Tasks;               // Для поддержки async/await и Task

class Program
{
    // Точка входа в программу
    static async Task Main(string[] args)
    {
        // Путь к папке с файлами для тестирования
        string folderPath = @"TestFolder/";

        // Проверяем, существует ли указанная папка
        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine($"Ошибка: папка не найдена по пути '{folderPath}'.");
            Console.WriteLine("Создайте папку и поместите в неё текстовые файлы для теста.");
            return;
        }

        Console.WriteLine($"Анализируем папку: {folderPath}\n");

        //ВАРИАНТ 1: Один файл — одна задача (Task)
        Console.WriteLine("Запуск Варианта 1: каждый файл обрабатывается в отдельной задаче");
        var stopwatch1 = Stopwatch.StartNew(); // Запускаем таймер

        // Вызываем метод подсчёта пробелов (вариант 1)
        long totalSpacesV1 = await CountSpacesInFolderV1(folderPath);

        stopwatch1.Stop(); // Останавливаем таймер
        Console.WriteLine($"Вариант 1 завершён.");
        Console.WriteLine($"   Общее количество пробелов: {totalSpacesV1}");
        Console.WriteLine($"   Время выполнения: {stopwatch1.ElapsedMilliseconds} мс\n");

        //ВАРИАНТ 2: Одна строка — одна задача (Task)
        Console.WriteLine("Запуск Варианта 2: каждая строка во всех файлах — отдельная задача");
        var stopwatch2 = Stopwatch.StartNew(); // Запускаем таймер

        // Вызываем метод подсчёта пробелов (вариант 2)
        long totalSpacesV2 = await CountSpacesInFolderV2(folderPath);

        stopwatch2.Stop(); // Останавливаем таймер
        Console.WriteLine($"Вариант 2 завершён.");
        Console.WriteLine($"   Общее количество пробелов: {totalSpacesV2}");
        Console.WriteLine($"   Время выполнения: {stopwatch2.ElapsedMilliseconds} мс\n");

        // Сравниваем результаты
        if (totalSpacesV1 == totalSpacesV2)
        {
            Console.WriteLine("Результаты подсчета пробелов обоих методов совпадают!");
        }
        else
        {
            Console.WriteLine("Внимание: результаты НЕ совпадают! Возможна ошибка в реализации.");
        }

        Console.WriteLine("\nНажмите любую клавишу для завершения...");
        Console.Read();
    }

     
    /// Считает количество пробелов (' ') в заданной строке.
    /// Использует LINQ-метод Count с условием c == ' '.
    ///
    private static int CountSpacesInString(string text)
    {
        return text.Count(c => c == ' ');
    }

     
    /// Параллельно читает все файлы в папке, где каждый файл обрабатывается в отдельной задаче (Task).
    /// Асинхронное чтение + параллельный подсчёт через Task.Run.

    public static async Task<long> CountSpacesInFolderV1(string folderPath)
    {
        // Получаем список всех файлов в указанной папке
        var files = Directory.GetFiles(folderPath, "*", SearchOption.TopDirectoryOnly)
                             .Where(f => !new FileInfo(f).Attributes.HasFlag(FileAttributes.Directory)) // Фильтр: только файлы
                             .ToArray();

        // Создаём список задач, каждая из которых будет считать пробелы в одном файле
        var tasks = new List<Task<long>>();

        // Перебираем каждый файл
        foreach (string file in files)
        {
            // Добавляем новую задачу: асинхронно читаем содержимое файла и считаем пробелы
            tasks.Add(Task.Run(async () =>
            {
                // Асинхронно читаем весь текст файла (целиком)
                string content = await File.ReadAllTextAsync(file);
                // Подсчитываем количество пробелов в этом тексте
                return (long)CountSpacesInString(content);
            }));
        }

        // Ожидаем завершения всех задач и получаем массив результатов (по одному на файл)
        long[] results = await Task.WhenAll(tasks);

        // Суммируем все частичные результаты и возвращаем общее количество пробелов
        return results.Sum();
    }


     
    /// Читает все файлы построчно и создаёт отдельную задачу (Task) для КАЖДОЙ строки,
    /// чтобы подсчитать количество пробелов. После завершения всех задач — суммирует результаты.
    public static async Task<long> CountSpacesInFolderV2(string folderPath)
    {
        // Получаем список файлов в папке (только верхний уровень)
        var files = Directory.GetFiles(folderPath, "*", SearchOption.TopDirectoryOnly)
                             .Where(f => !new FileInfo(f).Attributes.HasFlag(FileAttributes.Directory))
                             .ToArray();

        // Список задач: по одной на каждую строку во всех файлах
        var lineTasks = new List<Task<int>>();

        // Перебираем каждый файл
        foreach (string file in files)
        {
            // Асинхронно читаем все строки файла в виде массива строк
            string[] lines = await File.ReadAllLinesAsync(file);

            // Перебираем каждую строку
            foreach (string line in lines)
            {
                // Создаём задачу для подсчёта пробелов в этой строке
                // Task.Run запускает обработку строки в пуле потоков
                lineTasks.Add(Task.Run(() => CountSpacesInString(line)));
            }
        }

        // Ждём завершения всех задач (по каждой строке)
        int[] results = await Task.WhenAll(lineTasks);

        // Суммируем все значения (приводим к long, чтобы избежать переполнения при большом количестве строк)
        return results.Sum(x => (long)x);
    }
}