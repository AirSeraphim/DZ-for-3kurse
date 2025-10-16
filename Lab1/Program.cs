using System;
using System.Diagnostics; // Для измерения времени выполнения (Stopwatch)
using System.IO;          // Для работы с файлами и потоками
using System.Linq;
using System.Text.Json;   // Для встроенной сериализации в JSON
using System.Runtime.Serialization; // Подключаем для поддержки сериализации исключений (например, при передаче по сети или сохранении)

namespace  lab1
{
    // Простой класс с шестью публичными полями и девятью приватными свойствами.
    class MyClass
    {
        public int num1;
        public int num2;
        public int num3;
        public int num4;
        public int num5;
        public int num6;

        // Приватные автоматические свойства (не доступны извне)
        private int prop1 { get; set; }
        private int prop2 { get; set; }
        private int prop3 { get; set; }
        private int prop4 { get; set; }
        private int prop5 { get; set; }
        private int prop6 { get; set; }
        private int prop7 { get; set; }
        private int prop8 { get; set; }
        private int prop9 { get; set; }

        // Пустой метод (заглушка, не делает ничего)
        public void Method()
        {
        }

        // Конструктор, инициализирующий поля и свойства объекта
        public MyClass(int num1, int num2, int num3, int num4, int num5, int num6)
        {
            this.num1 = num1;
            this.num2 = num2;
            this.num3 = num3;
            this.num4 = num4;
            this.num5 = num5;
            this.num6 = num6;

            // Присваивание значений приватным свойствам
            this.prop1 = num1;
            this.prop2 = num2;
            this.prop3 = num3;
            this.prop4 = num4;
            this.prop5 = num5;
            this.prop6 = num6;
            this.prop7 = num1;
            this.prop8 = num2;
            this.prop9 = num3;
        }
    }

    // Класс Text — тестовый объект для сериализации
    class Text
    {
        int i1, i2, i3, i4, i5; // Приватные поля (не будут сериализованы, если нет доступа)
        public int[] mas;       // Массив, будет сериализован, так как public

        // Конструктор по умолчанию: инициализирует поля и массив
        public Text()
        {
            i1 = 1; i2 = 2; i3 = 3; i4 = 4; i5 = 5;
            mas = new int[] { 1, 2 };
        }
        // Статический метод, возвращающий новый экземпляр Text
        public static Text Get() => new();
    }

    // Основной класс программы
    class Program
    {
        /// Точка входа в программу.
        /// Сравнивает производительность пользовательского сериализатора и встроенного JsonSerializer.
        static void Main(string[] args)
        {
            // Создаем экземпляр пользовательского сериализатора с объектом Text
            var customSerializer = new Serializer(Text.Get());

            // Объект для измерения времени выполнения операций
            var stopwatch = new Stopwatch();

            ///СЕРИАЛИЗАЦИЯ

            // Измерение времени пользовательской сериализации
            CustomSerializationTimeTest(ref customSerializer, ref stopwatch);

            // Получаем количество тактов процессора, затраченных на пользовательскую сериализацию
            var SerializingCustomTicksCount = stopwatch.ElapsedTicks;

            // Измерение времени встроенной сериализации (System.Text.Json)
            string builtinSerializatorResult = BuiltinSerializationTimeTest(Text.Get(), stopwatch);

            // Получаем время в тактах для встроенной сериализации
            var SerializingBuiltinTicksCount = stopwatch.ElapsedTicks;

            //ВЫВОД РЕЗУЛЬТАТОВ         

            // Выводим сравнение времени выполнения двух подходов
            PrintTicks(SerializingCustomTicksCount, SerializingBuiltinTicksCount);

            // Выводим результаты сериализации (JSON-строки) для сравнения
            PrintResults(customSerializer.GetData(), builtinSerializatorResult);

            ///ЗАПИСЬ В ФАЙЛ

            // Измеряем время записи сериализованных данных в файл
            CustomSerializationToFileTimeTest(customSerializer, ref stopwatch);

            ///ДЕСЕРИАЛИЗАЦИЯ 

            // Открываем файл с сериализованными данными
            using (var inputFile = new StreamReader(Path.Combine("./", "serialized_data.json")))
            {
                // Читаем весь JSON из файла
                string jsonString = inputFile.ReadToEnd();

                // Выполняем десериализацию с замером времени
                object des_obj = CustomDeserializationTimeTest(customSerializer, stopwatch, jsonString);

                // Создаем новый сериализатор для десериализованного объекта и сериализуем его обратно
                var newSerializer = new Serializer(des_obj).Serialize();

                // Выводим содержимое десериализованного объекта (для проверки корректности)
                Console.WriteLine(newSerializer.GetData());
            }
        }

        private static object CustomDeserializationTimeTest(Serializer customSerializer, Stopwatch stopwatch, string jsonString)
        {
            stopwatch.Restart(); // Перезапускаем таймер

            var des_obj = customSerializer.Deserialize(jsonString); // Десериализация

            stopwatch.Stop(); // Останавливаем таймер
            return des_obj;
        }


        private static void CustomSerializationToFileTimeTest(Serializer serializer, ref Stopwatch stopwatch)
        {
            stopwatch.Restart(); // Перезапускаем таймер

            // Преобразуем данные в JSON и записываем в файл
            serializer.ConvertDataToJsonFile();

            stopwatch.Stop(); // Останавливаем таймер
        }


        private static string BuiltinSerializationTimeTest(object myClass, Stopwatch stopwatch)
        {
            stopwatch.Restart(); // Перезапускаем таймер

            // Сериализуем объект в JSON с помощью встроенного средства
            var secondResult = JsonSerializer.Serialize(myClass);

            stopwatch.Stop(); // Останавливаем таймер
            return secondResult;
        }


        private static void CustomSerializationTimeTest(ref Serializer serializer, ref Stopwatch stopwatch)
        {
            stopwatch.Start(); // Запускаем таймер

            serializer.Serialize(); // Выполняем сериализацию

            stopwatch.Stop(); // Останавливаем таймер
        }


        private static void PrintResults(string firstResult, string secondResult)
        {
            Console.WriteLine($"Results\n\nCustom\n-----------------------------\n{firstResult}\n\nBuiltin\n-----------------------------\n{secondResult}");
        }


        private static void PrintTicks(long SerializingCustomTicksCount, long SerializingBuiltinTicksCount)
        {
            Console.WriteLine($"Elapsed time in ticks\nCustom | Builtin\n{SerializingCustomTicksCount} | {SerializingBuiltinTicksCount}\n");
        }
    }
}