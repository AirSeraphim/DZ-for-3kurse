using System;
using System.IO;                                   // Для работы с файлами и потоками
using System.Text;                                 // Для указания кодировки (UTF-8)
using System.Text.Json;                            // Для сериализации в JSON (.NET 6+)
using System.Text.Json.Serialization;            // Для настроек игнорирования null и других опций
using System.Xml.Serialization;                  // Для XML-сериализации через XmlSerializer
using domaska2.Models;                           // Пространство имён, где находятся модели: SaveFile, User, Item и т.д.
using System.Runtime.Serialization.Formatters.Binary; // Для бинарной сериализации

class Program
{
    static void Main()
    {
        // Создаём объект SaveFile — модель игрового сохранения
        var saveFile = new SaveFile
        {
            CreatedDate = DateTime.Now,                          // Дата создания сохранения
            SaveDate = DateTime.Now.AddMinutes(5),               // Дата последнего сохранения (условно +5 минут)
            FileName = "save_001.dat",                           // Имя файла сохранения
            CurrentLocation = "Тронный зал",                     // Текущее место действия игрока
            User = new User                                     // Информация об игроке
            {
                Name = "Алёша",
                Level = 99,
                Gender = Gender.Male
            },
            Items = new[]                                       // Массив предметов в инвентаре
            {
                new Item { Name = "Меч", Quantity = 1 },
                new Item { Name = "Зелье лечения", Quantity = 5 }
            },
            Coords = (123.45, 67.89)
        };

        try
        {
            // --- СЕРИАЛИЗАЦИЯ В РАЗНЫЕ ФОРМАТЫ ---

            // 1. Бинарная сериализация 
            SerializeBinary(saveFile, "save.bin");
            Console.WriteLine("Binary: Сохранено в save.bin");

            // 2. XML-сериализация 
            SerializeXml(saveFile, "save.xml");
            Console.WriteLine("XML: Сохранено в save.xml");

            // 3. JSON-сериализация 
            SerializeJson(saveFile, "save.json");
            Console.WriteLine("JSON: Сохранено в save.json");

            // --- ПРОВЕРКА ДЕСЕРИАЛИЗАЦИИ ---
            // Загружаем данные обратно из XML, чтобы проверить целостность
            var loaded = DeserializeXml<SaveFile>("save.xml");
            Console.WriteLine($"Загружено: {loaded.User.Name}, Уровень: {loaded.User.Level}, Локация: {loaded.CurrentLocation}");
        }
        catch (Exception ex)
        {
            // Обработка всех возможных ошибок (файл занят, нет прав, ошибка сериализации и т.п.)
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    // МЕТОДЫ ДЛЯ БИНАРНОЙ СЕРИАЛИЗАЦИИ

     
    /// Сериализует объект в бинарный формат и сохраняет в файл.

    static void SerializeBinary<T>(T obj, string path) where T : class
    {
        FileStream fs = File.Create(path);                    // Создаём или перезаписываем файл

        var d = new BinaryFormatter();                        // Устаревший, но рабочий сериализатор
        d.Serialize(fs, obj);                                 // Записываем объект в поток

        fs.Close();                                          
    }

     
    /// Загружает объект из бинарного файла.
     
    static T DeserializeBinary<T>(string path) where T : class
    {
        FileStream fs = File.OpenRead(path);                  // Открываем файл для чтения
        var d = new BinaryFormatter();

        T obj = (T)d.Deserialize(fs);                         // Читаем и преобразуем к нужному типу
        fs.Close();                                           // Закрываем поток
        return obj;
    }

    // МЕТОДЫ ДЛЯ XML СЕРИАЛИЗАЦИИ

     
    /// Сериализует объект в XML-формат с UTF-8 кодировкой.
    /// XmlSerializer требует, чтобы класс имел публичные поля/свойства и конструктор по умолчанию.

    static void SerializeXml<T>(T obj, string path) where T : class
    {
        var serializer = new XmlSerializer(typeof(T));         // Создаём сериализатор для типа T

        // Используем using для автоматического освобождения ресурсов
        using var writer = new StreamWriter(path, false, Encoding.UTF8);
        // false = перезаписать файл, true = добавить в конец
        serializer.Serialize(writer, obj);                   // Выполняем сериализацию
    }
    
     
    /// Десериализует XML-файл обратно в объект.

    static T DeserializeXml<T>(string path) where T : class
    {
        var serializer = new XmlSerializer(typeof(T));
        using var reader = new StreamReader(path, Encoding.UTF8);

        var result = serializer.Deserialize(reader) as T;     // Преобразуем к нужному типу

        if (result is null)
            throw new InvalidDataException($"Не удалось десериализовать XML из файла: {path}");

        return result;
    }

    // МЕТОДЫ ДЛЯ JSON СЕРИАЛИЗАЦИИ
  
    /// Сериализует объект в формат JSON с отступами и UTF-8 кодировкой.
    /// Использует современный System.Text.Json.

    static void SerializeJson<T>(T obj, string path) where T : class
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,  // Красивое форматирование с отступами
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Поддержка кириллицы
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Не записывать null-значения
        };

        string json = JsonSerializer.Serialize(obj, options); // Преобразуем объект в JSON-строку
        File.WriteAllText(path, json, Encoding.UTF8);         // Записываем строку в файл
    }
}