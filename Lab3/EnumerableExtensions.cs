using System;
using System.Collections.Generic; // Подключаем для использования интерфейса IEnumerable<T>

namespace domashka3
{
     
    /// Статический класс, содержащий методы расширения для коллекций.
     
    public static class EnumerableExtensions
    {
         
        /// Метод расширения, который возвращает элемент из коллекции с наибольшим числовым значением,
        /// определённым с помощью делегата преобразования.
        /// Если коллекция пустая или содержит только null-элементы, возвращается null.
        /// Работает только с ссылочными типами (где T : class).
 
        public static T? GetMax<T>(this IEnumerable<T> collection, Func<T, float> convertToNumber) where T : class
        {
            // Проверяем, передана ли коллекция. Если нет — выбрасываем исключение
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            // Проверяем, передана ли функция преобразования. Если нет — ошибка
            if (convertToNumber == null)
                throw new ArgumentNullException(nameof(convertToNumber));

            // Переменная для хранения элемента с максимальным значением (может быть null)
            T? maxItem = null;

            // Переменная для хранения текущего максимального числового значения
            float maxValue = float.MinValue; // Начинаем с самого маленького возможного float

            // Перебираем все элементы в коллекции
            foreach (var item in collection)
            {
                // Пропускаем элемент, если он равен null (чтобы избежать NullReferenceException)
                if (item == null) continue;

                // Преобразуем текущий элемент в число с помощью переданной функции
                float value = convertToNumber(item);

                // Если это первый элемент или его значение больше предыдущего максимума
                if (maxItem == null || value > maxValue)
                {
                    // Обновляем максимальный элемент и соответствующее ему значение
                    maxItem = item;
                    maxValue = value;
                }
            }

            // Возвращаем найденный элемент с максимальным значением (или null, если ни один не подошёл)
            return maxItem;
        }
    }
}