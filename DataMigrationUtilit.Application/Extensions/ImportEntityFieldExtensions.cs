using System;

namespace DataMigrationUtilit.Application.Extensions
{
    public static class ImportEntityFieldExtensions
    {
        public static string FixFormat(this string field)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                throw new ArgumentNullException("значение отсутствует");
            }

            Span<char> span = field.ToCharArray();

            var indexes = GetStartAndEndIndexFormatting(ref span);

            int start = indexes.Item1;
            int end = indexes.Item2;

            if (start > end)
            {
                return string.Empty;
            }

            // Создаем новый массив для результата
            char[] resultArray = new char[end - start + 1];
            int resultIndex = 0;
            bool isPreviousSpace = false;

            // Заполняем новый массив, заменяя несколько пробелов на один
            for (int i = start; i <= end; i++)
            {
                if (char.IsWhiteSpace(span[i]))
                {
                    if (!isPreviousSpace)
                    {
                        resultArray[resultIndex++] = ' ';
                        isPreviousSpace = true;
                    }
                }
                else
                {
                    if(resultIndex == 0)
                    {
                        resultArray[resultIndex++] = char.ToUpper(span[i]);
                    } else
                    {
                        resultArray[resultIndex++] = char.ToLower(span[i]);
                    }

                    isPreviousSpace = false;
                }
            }

            var result = new string(resultArray, 0, resultIndex);

            return result;
        }

        public static string FixFullNameFormat(this string field)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                throw new ArgumentNullException("значение отсутствует");
            }

            Span<char> span = field.ToCharArray();

            var indexes = GetStartAndEndIndexFormatting(ref span);

            int start = indexes.Item1;
            int end = indexes.Item2;

            if (start > end)
            {
                return string.Empty;
            }

            // Создаем новый массив для результата
            char[] resultArray = new char[end - start + 1];
            int resultIndex = 0;
            bool isPreviousSpace = false;

            // Заполняем новый массив, заменяя несколько пробелов на один
            for (int i = start; i <= end; i++)
            {
                if (char.IsWhiteSpace(span[i]))
                {
                    if (!isPreviousSpace)
                    {
                        resultArray[resultIndex++] = ' ';
                        isPreviousSpace = true;
                    }
                }
                else
                {
                    if (resultIndex == 0 || isPreviousSpace)
                    {
                        resultArray[resultIndex++] = char.ToUpper(span[i]);
                    }
                    else
                    {
                        resultArray[resultIndex++] = char.ToLower(span[i]);
                    }

                    isPreviousSpace = false;
                }
            }

            var result = new string(resultArray, 0, resultIndex);

            return result;
        }

        private static (int, int) GetStartAndEndIndexFormatting(ref Span<char> span)
        {
            int start = 0;
            int end = span.Length - 1;
            // Найти первый не пробельный символ
            while (start <= end && char.IsWhiteSpace(span[start]))
            {
                start++;
            }

            // Найти последний не пробельный символ
            while (end >= start && char.IsWhiteSpace(span[end]))
            {
                end--;
            }

            return (start, end);
        }
    }
}
