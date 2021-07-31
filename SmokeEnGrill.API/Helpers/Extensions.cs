using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmokeEnGrill.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static void AddPagination(this HttpResponse response,
            int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage,
                totalItems, totalPages);
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination",
                JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }

        public static int CalculateAge(this DateTime theDateTime)
        {
            var age = 0;
            if (theDateTime != null)
            {
                age = DateTime.Today.Year - Convert.ToInt32(theDateTime.Year);
                if (theDateTime.AddYears(age) > DateTime.Today)
                    age--;
            }

            return age;
        }

        //email validation
        public static bool EmailValid(string email)
        {
            string pattern = @"^[a-z][a-z|0-9|]*([_][a-z|0-9]+)*([.][a-z|0-9]+([_][a-z|0-9]+)*)?@[a-z][a-z|0-9|]*\.([a-z][a-z|0-9]*(\.[a-z][a-z|0-9]*)?)$";
            pattern = @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$";
            System.Text.RegularExpressions.Match match = Regex.Match(email.Trim(), pattern, RegexOptions.IgnoreCase);
            if (match.Success)
                return true;
            else
                return false;
        }

        public static bool IsNumeric(this string str)
        {
            if (str == null)
                return false;

            try
            {
                double result;
                if (double.TryParse(str, out result))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        // Checks whether there are duplicates in a list.
        public static bool AreAnyDuplicates<T>(this IEnumerable<T> list)
        {
            var hashset = new HashSet<T>();
            return list.Any(e => !hashset.Add(e));
        }

        // Checks whether or not a date is a valid date.
        public static bool IsDate(this string str)
        {
            try
            {
                DateTime dt = DateTime.Parse(str);

                if (dt != DateTime.MinValue && dt != DateTime.MaxValue)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static string UppercaseWords(this string value)
        {
            char[] array = value.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
            if (char.IsLower(array[0]))
            {
                array[0] = char.ToUpper(array[0]);
            }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
            if (array[i - 1] == ' ')
            {
                if (char.IsLower(array[i]))
                {
                array[i] = char.ToUpper(array[i]);
                }
            }
            }
            return new string(array);
        }

        public static string To5Digits(this string data)
        {
            switch (data.Length)
            {
                case 1:
                    data = "0000" + data;
                    break;
                case 2:
                    data = "000" + data;
                    break;
                case 3:
                    data = "00" + data;
                    break;
                case 4:
                    data = "0" + data;
                    break;
            }
            return data;
        }

        public static string FormatPhoneNumber(this string phone)
        {
            if (phone.Length == 10)
            {
                return String.Format("{0}.{1}.{2}.{3}.{4}", phone.Substring(0, 2), phone.Substring(2, 2),
                    phone.Substring(4, 2), phone.Substring(6, 2), phone.Substring(8));
            }
            return phone;
        }

        public static string RegNum5digits(string refNum)
        {
            string idnum = "";

            if (refNum.Length == 1)
                idnum += "0000" + refNum;
            else if (refNum.Length == 2)
                idnum += "000" + refNum;
            else if (refNum.Length == 3)
                idnum += "00" + refNum;
            else if (refNum.Length == 4)
                idnum += "0" + refNum;
            else
                idnum += refNum;

            return idnum;
        }
    }
}