using System;
using System.IO; // Подключаем для работы с файлами и директориями (Directory, FileInfo и др.)

namespace domashka3
{  
    /// Класс для поиска файлов в указанной директории с возможностью уведомления через событие.
    /// При нахождении каждого файла генерируется событие FileFound.
    /// Поиск можно отменить из обработчика события.
    public class FileSearcher
    {
        /// Событие, которое вызывается при нахождении файла.
        /// Тип события — EventHandler с аргументами FileEventArgs.
        /// Может быть null
        public event EventHandler<FileEventArgs>? FileFound;

         
        /// Выполняет рекурсивный поиск всех файлов в указанной директории и её подкаталогах.
        /// Для каждого найденного файла вызывается событие FileFound.
        /// Если в аргументах события установлен флаг Cancel, поиск прекращается.
        public void SearchDirectory(string path)
        {
            // Проверяем, существует ли указанная директория
            if (!Directory.Exists(path))
                // Если нет — выбрасываем исключение с информативным сообщением
                throw new DirectoryNotFoundException($"Директория не найдена: {path}");

            // Перебираем все файлы в указанной папке и во всех вложенных подпапках
            foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            {
                // Создаём объект аргументов события, содержащий путь к найденному файлу
                var args = new FileEventArgs(file);

                // Вызываем виртуальный метод OnFileFound
                OnFileFound(args);

                // Проверяем, был ли запрос на отмену поиска из обработчика события
                if (args.Cancel)
                {
                    // Сообщаем в консоль, что поиск остановлен
                    Console.WriteLine("Поиск отменён обработчиком события.");
                    return; // Прекращаем дальнейший поиск
                }
            }
        }

         
        /// Защищённый виртуальный метод для вызова события FileFound..
        protected virtual void OnFileFound(FileEventArgs e)
        {
            // Проверяем, есть ли подписчики на событие, и если да — вызываем его
            FileFound?.Invoke(this, e);
        }
    }
}