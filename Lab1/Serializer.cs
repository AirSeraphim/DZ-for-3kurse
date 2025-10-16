using System;
using System.Collections;           // Для интерфейса IEnumerable и других коллекций
using System.Collections.Generic;   // Для Dictionary, List и т.д.
using System.IO;                    // Для работы с файлами (StreamWriter)
using System.Reflection;            // Для рефлексии: получение полей, свойств, конструкторов
using System.Text;                  // Для StringBuilder
using System.Text.RegularExpressions; // Для парсинга JSON через регулярные выражения
using Microsoft.VisualBasic;

namespace  lab1
{ 
    /// Класс для сериализации объектов в упрощённый формат JSON.
    /// Поддерживает публичные поля, свойства, массивы и простые типы. 
    public class Serializer
    { 
        /// Создаёт новый экземпляр сериализатора для указанного объекта.
        public Serializer(object obj)
        {
            this.Obj = obj;
        }

        // Объект, который будет сериализован
        private object Obj { get; set; }

        // Словарь для хранения сериализованных данных (ключ — имя поля/свойства, значение — строковое представление)
        private Dictionary<string, string?>? SerializedDict { get; set; }
 
        /// Получает словарь с данными объекта: имена полей и свойств и их значения в виде строк.
        /// Использует рефлексию для доступа к публичным членам объекта.
        private Dictionary<string, string?> GetDataDict()
        {
            var dict = new Dictionary<string, string?>();

            // Получаем тип объекта для анализа его структуры
            Type type = this.Obj.GetType();

            // Получаем все публичные поля и свойства
            var fields = type.GetFields();
            var properties = type.GetProperties();

            // Добавляем поля в словарь
            foreach (var field in fields)
            {
                dict[field.Name] = GetValueToString(field, this.Obj);
            }

            // Добавляем свойства в словарь
            foreach (var property in properties)
            {
                dict[property.Name] = GetValueToString(property, this.Obj);
            }

            return dict;
        }

        /// Преобразует значение свойства в строку.
        /// Если значение является перечислением (например, массивом), вызывает специальный метод.
        private static string? GetValueToString(PropertyInfo fieldOrProperty, object obj)
        {
            object? value = fieldOrProperty.GetValue(obj);

            if (obj == null || value == null)
            {
                return null;
            }
            else
            {
                // Если значение — коллекция, обрабатываем как массив
                return IsEnumerable(value) ? GetEnumerableValuesString(value) : value.ToString();
            }
        }

        /// Преобразует элементы перечисляемого объекта (например, массива) в строку.
        /// 
        private static string? GetEnumerableValuesString(object obj)
        {
            var enumerable = (int[])obj;

            var sb = new StringBuilder();

            for (int i = 0; i < enumerable.Length; i++)
            {
                sb.Append($"{enumerable[i]},");
            }

            return sb.ToString(); // Результат заканчивается запятой
        }

        /// Проверяет, является ли объект перечисляемым (массивом, списком и т.п.)
        private static bool IsEnumerable(object obj)
        {
            return obj is IEnumerable || obj is IList || obj is Collection;
        }

        /// Преобразует значение поля в строку.
        /// Аналогично методу для свойств, но принимает FieldInfo.
        private static string? GetValueToString(FieldInfo fieldOrProperty, object obj)
        {
            object? value = fieldOrProperty.GetValue(obj);

            if (obj == null || value == null)
            {
                return null;
            }
            else
            {
                return IsEnumerable(value) ? GetEnumerableValuesString(value) : value.ToString();
            }
        }

        /// Возвращает сериализованные данные в виде строки JSON-подобного формата.
        /// Формат: {"ключ1":"значение1","ключ2":"значение2"}
        public string GetData()
        {
            var sb = new StringBuilder();
            sb.Append('{'); // Открывающая скобка JSON

            if (this.SerializedDict != null)
            {
                foreach (var pair in this.SerializedDict)
                {
                    sb.Append($"\"{pair.Key}\":\"{pair.Value}\",");
                }
            }

            sb.Append('}'); // Закрывающая скобка

            var result = sb.ToString();
            return result;
        }

        /// Выполняет сериализацию объекта: собирает данные и сохраняет их во внутренний словарь.
        public Serializer Serialize()
        {
            this.SerializedDict = this.GetDataDict();
            return this;
        }

        /// Определяет тип значения из строки JSON.
        /// Пытается распознать число, дробь, символ или строку.
        /// Убирает кавычки вокруг строк.
        private static object GetTypeOfJsonValue(string value)
        {
            // Удаляем кавычки, если значение в кавычках
            if (value[0] == '\"' && value[^1] == '\"')
            {
                value = value[1..^1]; // Убираем первую и последнюю кавычки
            }

            // Пробуем преобразовать в целое число
            if (int.TryParse(value, out int result_int))
            {
                return result_int;
            }
            // Пробуем float (но игнорируем результат — ошибка логики!)
            else if (float.TryParse(value, out float result_flt))
            {
                return result_flt;
            }
            // Пробуем double
            else if (double.TryParse(value, out double result_dbl))
            {
                return result_dbl;
            }
            // Пробуем char
            else if (char.TryParse(value, out char result_chr))
            {
                return result_chr;
            }

            // По умолчанию возвращаем как строку
            return value;
        }

        /// Десериализует JSON-подобную строку обратно в объект.
        /// Сначала пытается найти конструктор с подходящими параметрами.
        /// Если не находит — использует конструктор без параметров и устанавливает поля/свойства.
        public object Deserialize(string jsonString)
        {
            MatchCollection matches = RegexParseJsonString(jsonString);
            Type obj_type = this.Obj.GetType();

            int pair_count = matches.Count;

            // Подготовка типов, имён и значений для конструктора
            Type[] constructor_types = new Type[pair_count];
            string[] constructor_attr_names = new string[pair_count];
            object[] constructor_values = new object[pair_count];

            int names_count = 0;
            int vals_count = 0;

            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;

                foreach (Group group in groups)
                {
                    if (group.Success)
                    {
                        switch (group.Name)
                        {
                            case "pair": // Игнорируем группу всей пары
                                break;
                            case "key": // Имя поля
                                constructor_attr_names[names_count++] = group.Value;
                                break;
                            case "val": // Значение
                                var tmpValue = GetTypeOfJsonValue(group.Value);
                                constructor_types[vals_count] = tmpValue.GetType();
                                constructor_values[vals_count] = tmpValue;
                                vals_count++;
                                break;
                            default:
                                continue;
                        }
                    }
                }
            }

            // Попытка найти конструктор с такими параметрами
            ConstructorInfo? constructor = obj_type.GetConstructor(constructor_types);

            if (constructor != null)
            {
                object? deserialized_object = constructor.Invoke(constructor_values);
                return deserialized_object ?? throw new InvalidOperationException("Конструктор вернул null.");
            }
            else
            {
                // Если нет подходящего конструктора — пробуем создать через конструктор по умолчанию
                ConstructorInfo? base_constructor = obj_type.GetConstructor(Array.Empty<Type>());

                if (base_constructor != null)
                {
                    object? deserialized_object = base_constructor.Invoke(null);

                    // Устанавливаем поля и свойства
                    for (int i = 0; i < constructor_attr_names.Length; i++)
                    {
                        var field = obj_type.GetField(constructor_attr_names[i], BindingFlags.Public | BindingFlags.Instance);
                        if (field != null && field.FieldType == constructor_types[i])
                        {
                            field.SetValue(deserialized_object, constructor_values[i]);
                        }

                        var property = obj_type.GetProperty(constructor_attr_names[i], BindingFlags.Public | BindingFlags.Instance);
                        if (property != null && property.PropertyType == constructor_types[i])
                        {
                            property.SetValue(deserialized_object, constructor_values[i]);
                        }
                    }

                    return deserialized_object;
                }
                else
                {
                    throw new ConstructorNotFoundException();
                }
            } //ОСтавил, но вроде можно убрать
            throw new ConstructorNotFoundException();
        }

        /// Парсит строку, похожую на JSON, с помощью регулярного выражения.
        /// Поддерживает простые пары ключ-значение, где ключ и значение — слова.
        private static MatchCollection RegexParseJsonString(string jsonString)
        {
            // Регулярное выражение для поиска пар: "ключ":"значение"
            const string kPattern = @"(?<pair>(?<key>[""]?\w+[""]?(?=:)):(?<val>(?<=:)[""]?\w+[""]?))+";
            var matches = Regex.Matches(jsonString, kPattern);
            return matches;
        }
 
        /// Сохраняет сериализованные данные в файл в формате JSON.

        public bool ConvertDataToJsonFile()
        {
            var dictString = this.GetData();

            if (dictString != null)
            {
                using (var outputFile = new StreamWriter(Path.Combine("./", "serialized_data.json")))
                {
                    outputFile.WriteLine(dictString);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}