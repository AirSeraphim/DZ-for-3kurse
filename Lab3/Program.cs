using System;
using System.Collections.Generic;           // Для использования List<T>
using System.IO;                            // Для работы с FileInfo и Directory
using System.Linq;                          // Для методов расширений LINQ, таких как MaxBy (в GetMax)

namespace domashka3
{

    class Program
    {
        
        static void Main(string[] args)
        {
            //Тест метода GetMax
            // Создаём список объектов FileInfo с разными именами файлов
            var files = new List<FileInfo>
            {
                new("file1.txt"),
                new("very_long_file_name.txt"),
                new("a.txt")
            };

            // Вызываем метод расширения GetMax, который находит элемент с максимальным значением длины имени файла
            var longestFile = files.GetMax(f => f.Name.Length);

            // Выводим имя найденного файла. Используем ?. чтобы избежать ошибки, если список пуст
            Console.WriteLine($"Файл с самым длинним именем: {longestFile?.Name}");

            //Поиск файлов с использованием события
            // Создаём экземпляр класса для поиска файлов
            var searcher = new FileSearcher();

            // Счётчик найденных файлов
            int fileCount = 0;

            // Подписываемся на событие FileFound
            searcher.FileFound += (sender, e) =>
            {
                fileCount++;  // Увеличиваем счётчик при каждом найденном файле
                Console.WriteLine($"Найден файл #{fileCount}: {e.FileName}");  // Выводим информацию о файле

                // Пример условия для отмены дальнейшего поиска — после нахождения 3 файлов
                if (fileCount >= 3)
                {
                    e.Cancel = true;  // Устанавливаем флаг отмены в аргументах события
                    Console.WriteLine("Установлен флаг отмены.");  // Сообщаем, что поиск будет остановлен
                }
            };

            // Обрабатываем возможные исключения при выполнении поиска (например, доступ запрещён)
            try
            {
                // Запускаем поиск файлов в текущей директории
                searcher.SearchDirectory(Directory.GetCurrentDirectory());
            }
            catch (Exception ex)
            {
                // Выводим сообщение об ошибке, если возникло исключение
                Console.WriteLine($"Ошибка при поиске: {ex.Message}");
            }

            // Сообщаем, что программа завершила основную работу
            Console.WriteLine("Работа завершена.");
        }

    }
}