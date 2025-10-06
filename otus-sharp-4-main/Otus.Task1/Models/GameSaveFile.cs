using System;

namespace Otus.Task1.Models
{


    public enum Gender{
        None = 0,
        Male=1,
        Female=2,
    }

    /// <summary>
    /// Предмет
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Название предмета
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// Количество
        /// </summary>
        /// <value></value>
        public int Quantity { get; set; }
    }

    /// <summary>
    /// Информация о пользователе
    /// </summary>
    public class User
    {
        /// <summary>
        /// Уровень пользователя
        /// </summary>
        /// <value></value>
        public int Level { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        /// <value></value>
        public string Name { get; set; }


        /// <summary>
        /// Пол персонажа
        /// </summary>
        /// <value></value>
        public Gender Gender{get;set;}
    }


    /// <summary>
    /// Состояние игры
    /// </summary>
    [Serializable]
    public class GameStatus
    {


        /// <summary>
        /// Текущая локация
        /// </summary>
        /// <value></value>
        public string CurrentLocation { get; set; }

        /// <summary>
        /// Информация о пользователе
        /// </summary>
        /// <value></value>
        public User User { get; set; }

        public Item[] Items { get; set; }

        /// <summary>
        /// Координаты пользователя
        /// </summary>
        /// <value></value>
        public (double, double) Coords { get; set; }
    }

    /// <summary>
    /// Сохраняемый файл
    /// </summary>
    public class SaveFile : GameStatus
    {

        /// <summary>
        /// Дата создания файла
        /// </summary>
        /// <value></value>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Дата сохранения
        /// </summary>
        /// <value></value>
        public DateTime? SaveDate { get; set; }

        /// <summary>
        /// НАзвание файла
        /// </summary>
        /// <value></value>
        public string FileName { get; set; }



    }
}