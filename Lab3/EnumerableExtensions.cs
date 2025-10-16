using System;
using System.Collections.Generic; // ���������� ��� ������������� ���������� IEnumerable<T>

namespace domashka3
{
     
    /// ����������� �����, ���������� ������ ���������� ��� ���������.
     
    public static class EnumerableExtensions
    {
         
        /// ����� ����������, ������� ���������� ������� �� ��������� � ���������� �������� ���������,
        /// ����������� � ������� �������� ��������������.
        /// ���� ��������� ������ ��� �������� ������ null-��������, ������������ null.
        /// �������� ������ � ���������� ������ (��� T : class).
 
        public static T? GetMax<T>(this IEnumerable<T> collection, Func<T, float> convertToNumber) where T : class
        {
            // ���������, �������� �� ���������. ���� ��� � ����������� ����������
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            // ���������, �������� �� ������� ��������������. ���� ��� � ������
            if (convertToNumber == null)
                throw new ArgumentNullException(nameof(convertToNumber));

            // ���������� ��� �������� �������� � ������������ ��������� (����� ���� null)
            T? maxItem = null;

            // ���������� ��� �������� �������� ������������� ��������� ��������
            float maxValue = float.MinValue; // �������� � ������ ���������� ���������� float

            // ���������� ��� �������� � ���������
            foreach (var item in collection)
            {
                // ���������� �������, ���� �� ����� null (����� �������� NullReferenceException)
                if (item == null) continue;

                // ����������� ������� ������� � ����� � ������� ���������� �������
                float value = convertToNumber(item);

                // ���� ��� ������ ������� ��� ��� �������� ������ ����������� ���������
                if (maxItem == null || value > maxValue)
                {
                    // ��������� ������������ ������� � ��������������� ��� ��������
                    maxItem = item;
                    maxValue = value;
                }
            }

            // ���������� ��������� ������� � ������������ ��������� (��� null, ���� �� ���� �� �������)
            return maxItem;
        }
    }
}