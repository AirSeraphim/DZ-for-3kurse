using System;
using System.IO;
using System.Text.Json.Serialization; // Для управления сериализацией в JSON 
using System.Xml.Serialization;       // Для управления сериализацией в XML 

namespace domaska2.Models
{
     
    /// Пустой класс-заглушка. В данном случае не используется.
    /// Обычно пространство имён уже выполняет роль контейнера для моделей.
     
    public class Models
    {
        // Этот класс пуст — вероятно, оставлен по ошибке или для группировки.
        // На функциональность не влияет.
    }

     
    /// Перечисление пола пользователя.
    /// Используется в классе User для хранения информации о гендере.
     
    public enum Gender
    {
        None = 0,   // Пол не указан
        Male = 1,   // Мужской
        Female = 2  // Женский
    }

     
    /// Представляет предмет в инвентаре игрока (например, меч, зелье и т.п.).
    /// Поддерживает сериализацию через BinaryFormatter, XML и JSON.
     
    [Serializable] // Помечает класс как доступный для бинарной сериализации
    public class Item
    {
         
        /// Название предмета (например, "Меч", "Зелье силы").
        /// Сериализуется как есть в XML и JSON.
         
        public string Name { get; set; }

        // Приватное поле для хранения количества
        private int quantity;

         
        /// Количество данного предмета.
        /// Не может быть отрицательным — при попытке установить значение < 0 выбрасывается исключение.
        /// Защищает целостность данных.
         
        public int Quantity
        {
            get => quantity;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Quantity cannot be less than 0.");
                quantity = value;
            }
        }
    }

     
    /// Информация о пользователе (игроке): имя, уровень, пол.
    /// Поддерживает разные форматы сериализации с кастомизацией имён и поведения.
     
    [Serializable]           // Требуется для BinaryFormatter
    [XmlType("u")]          // При сериализации в XML класс будет представлен как <u> вместо <User>
    public class User
    {
         
        /// Уровень игрока. В XML сохраняется как атрибут: <u level="99">.
        /// В JSON — как обычное поле.
         
        [XmlAttribute("level")] // Указывает, что Level должен быть атрибутом, а не элементом
        public int Level { get; set; }

         
        /// Имя пользователя. Сериализуется как элемент/поле в обоих форматах.
         
        public string Name { get; set; }

         
        /// Пол пользователя (Gender). Это свойство НЕ сериализуется напрямую,
        /// потому что XML и JSON не поддерживают enum по умолчанию так, как нам нужно.
        /// Вместо этого используется строковое представление через GenderString.
         
        [XmlIgnore] // Полностью игнорируется при XML-сериализации
        public Gender Gender { get; set; }

         
        /// Вспомогательное свойство, используемое только для сериализации.
        /// Преобразует Gender в короткую строку ("m" для Male, "f" для Female).
        /// При десериализации обратно устанавливает значение Gender.
         
        [XmlElement("gender")]     // В XML будет элемент <gender>
        [JsonPropertyName("gender")] // В JSON будет ключ "gender"
        public string GenderString
        {
            get => Gender switch
            {
                Gender.Male => "m",
                Gender.Female => "f",
                _ => throw new InvalidOperationException("Gender must be Male or Female.")
            };
            set => Gender = value?.ToLower() switch
            {
                "m" => Gender.Male,
                "f" => Gender.Female,
                _ => Gender.None
            };
        }
    }

     
    /// Описывает текущее состояние игры: где игрок, кто он, какие предметы имеет.
    /// Базовый класс для SaveFile.
     
    [Serializable] // Разрешает бинарную сериализацию
    public class GameStatus
    {
         
        /// Текущая локация игрока (например, "Тронный зал", "Лес").
        /// Сериализуется как есть.
         
        public string CurrentLocation { get; set; }

         
        /// Данные об игроке.
        /// В XML будет сериализован как <u> (благодаря [XmlType("u")] в User).
        /// В JSON будет поле с именем "u".
         
        [XmlElement("u")]       // В XML: <u>...</u>
        [JsonPropertyName("u")]  // В JSON: "u": { ... }
        public User User { get; set; }

         
        /// Массив предметов в инвентаре.
        /// Будет сериализован как массив в JSON и как коллекция элементов в XML.
         
        public Item[] Items { get; set; }

         
        /// Координаты игрока в виде кортежа (X, Y).
         
        [JsonIgnore]
        [XmlIgnore]
        public (double, double) Coords { get; set; }
    }

     
    /// Полная информация о сохранённой игре.
    /// Расширяет GameStatus, добавляя метаданные о файле сохранения.
     
    [Serializable] // Обязательно для BinaryFormatter
    public class SaveFile : GameStatus // Наследуется от GameStatus — получает все его поля
    {
         
        /// Дата и время создания сохранения.
        /// Сериализуется как строка ISO 8601 в JSON, как DateTime в других форматах.
         
        public DateTime CreatedDate { get; set; }

         
        /// Дата и время последнего сохранения.
        /// Тип nullable (DateTime?) — может быть null, если дата не установлена.
        /// Полезно для временных или черновых сохранений.
         
        public DateTime? SaveDate { get; set; }

         
        /// Имя файла сохранения (например, "save_001.dat").
        /// Хранится как обычная строка.
         
        public string FileName { get; set; }
    }
}