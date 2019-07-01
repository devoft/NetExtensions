using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace devoft.System
{
    public static class StringExtensions
    {
        /// <summary>
        /// Devuelve la subcadena de <b>str</b> anterior al caracter <b>ch</b>
        /// </summary>
        /// <param name="str">Cadena de origen</param>
        /// <param name="ch">caracter a buscar</param>
        /// <returns>Subcadena de <b>str</b> hasta el caracter <b>ch</b></returns>
        public static string SubstringBefore(this string str, char ch)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < str.Length && str[i] != ch; i++)
                builder.Append(str[i]);
            return builder.ToString();
        }

        /// <summary>
        /// Similar a <see cref="string.Substring(int)"/> pero aplicable a <b>null</b> y si <b>count</b> sobrepasa
        /// el tamaño de la cadena se devuelve la propia cadena.
        /// </summary>
        /// <param name="str">cadena de origen</param>
        /// <param name="count">cantidad de caractéres máximo de la subcadena resultante</param>
        /// <returns>Subcadena de resultante</returns>
        public static string TruncateTo(this string str, int count)
            => !str.IsEmpty() ? str.Substring(0, Math.Min(count, str.Length)) : string.Empty;

        /// <summary>
        /// Indica si <b>str</b> es <b>null</b>, cadena vacía, o espacios en blanco
        /// </summary>
        /// <param name="str">cadena de la que se quiere indagar si es vacía</param>
        /// <returns><b>true</b> si vacía, <b>false</b> en otro caso</returns>
        public static bool IsEmpty(this string str)
            => string.IsNullOrWhiteSpace(str);

        /// <summary>
        /// Versión del método <see cref="string.Trim()"/> pero que se puede aplicar a null, en cuyo caso
        /// se obtiene <see cref="string.Empty"/>
        /// </summary>
        /// <param name="str">cadena a reducir o <b>null</b></param>
        /// <returns>La cadena <b>str</b> reducida o <see cref="string.Empty"/> cuando <b>str</b> es <b>null</b></returns>
        public static string SafeTrim(this string str)
        {
            return str?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Indica si la cadena <b>value</b> está contenida en <b>str</b>, según el criterio de comparación de cadena indicado
        /// por <b>comparison</b>
        /// </summary>
        /// <param name="str">cadena contenedora</param>
        /// <param name="value">cadena contenida</param>
        /// <param name="comparison">criterio de comparación de cadenas</param>
        /// <returns><b>true</b> si value está contenida en <b>str</b>, <b>false</b> en caso contrario</returns>
        public static bool Contains(this string str, string value, StringComparison comparison = StringComparison.CurrentCulture)
            => str.IndexOf(value, comparison) >= 0;


        /// <summary>
        /// Rellena la cadena <paramref name="str"/> por delante con el caracter <paramref name="fillChar"/> hasta completar
        /// la cantidad de caracteres <paramref name="length"/>
        /// </summary>
        /// <param name="str">cadena a completar</param>
        /// <param name="fillChar">caracter a usar para completar</param>
        /// <param name="length">cantidad de caracteres a cumplimentar</param>
        /// <returns>cadena completada</returns>
        /// <example>
        /// <code>
        /// var x = "123A".FillStart('0', 8); // x == "0000123A"
        /// </code>
        /// </example>
        public static string FillStart(this string str, char fillChar, int length)
        {
            var builder = new StringBuilder();
            str = (str + "").Trim();
            for (var i = str.Length; i < length; i++)
                builder.Append(fillChar);
            builder.Append(str);
            return builder.ToString();
        }

        /// <summary>
        /// Rellena la cadena <paramref name="str"/> por detrás con el caracter <paramref name="fillChar"/> hasta completar
        /// la cantidad de caracteres <paramref name="length"/>
        /// </summary>
        /// <param name="str">cadena a completar</param>
        /// <param name="fillChar">caracter a usar para completar</param>
        /// <param name="length">cantidad de caracteres a cumplimentar</param>
        /// <returns>cadena completada</returns>
        /// <example>
        /// <code>
        /// var x = "123A".FillEnd('0', 8); // x == "123A0000"
        /// </code>
        /// </example>
        public static string FillEnd(this string str, char fillChar, int length)
        {
            var builder = new StringBuilder((str + "").Trim());
            for (var i = str.Length; i < length; i++)
                builder.Append(fillChar);
            return builder.ToString();
        }

        public static string ToAlphaNumeric(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("str");
            var result = new StringBuilder(str.Length);
            var started = false;
            foreach (var ch in str)
            {
                if (!started)
                {
                    if (char.IsLetter(ch) || char.IsWhiteSpace(ch))
                    {
                        result.Append(ch);
                        started = true;
                    }
                }
                else if (char.IsLetter(ch) || char.IsNumber(ch))
                    result.Append(ch);
            }
            return result.ToString();
        }

        private static string[] eWords = { "de", "del", "al", "lo", "los", "la", "las", "SOA", "y", "S.L.", "SL", "S.A.", "SA", "AS", "A/S", "Inc", "S.A.C.I.", "A.Ş", "CA", "C.A." };

        /// <summary>
        /// Conforma una nueva cadena a partir de los caracteres de str que sean letras y espacios
        /// 787MARIO DARio432423./==    ==>   Mario Dario
        /// 2321DEL vaLLe3333?>!!      ==>   del Valle  
        /// </summary>
        /// <param name="str">Cadena de la cual solo se toman los caracteres que sean letras</param>
        /// <param name="uppercase">True si se quiere que el resultado final empiece con letra mayúscula</param>
        /// <returns></returns>
        public static string ToAlphaOnly(this string str, bool uppercase)
        {
            var especialsWords = new HashSet<string>(eWords);
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("str");
            var result = new StringBuilder(str.Length);
            string[] words = str.Split(new Char[] { ' ', ',', '.', ':', '\t' });
            foreach (var ch in str.Where(ch => char.IsLetter(ch) || char.IsWhiteSpace(ch) || ch == '-' || ch == '.' || ch == '\u0027'))
            {
                result.Append(ch);
            }
            // Palabra a palabra del string y lo lleva a Camel si no esta en el enumerativo
            var aux = result.ToString();
            var temp = new StringBuilder(str.Length);
            string[] split = aux.Split(new Char[] { ' ', ',', ':', '\t' });

            foreach (string s in split)
            {
                if (!especialsWords.Contains(s) && s != "")
                {
                    temp.Append(char.ToUpper(s[0]));
                    aux = s.Substring(1);
                    foreach (var ch in aux)
                    {
                        temp.Append(char.ToLower(ch));
                    }
                    temp.Append(' ');
                    //if (s.Trim() != "")
                    //    Console.WriteLine(s);
                }
                else
                {
                    if (s != "")
                        temp.Append(s);
                    temp.Append(' ');
                }
            }
            return temp.ToString().TrimEnd();

        }

        /// <summary>
        /// Conforma una nueva cadena a partir de los caracteres de str que sean numeros
        /// </summary>
        /// <param name="str">Cadena de la cual solo se toman los caracteres que sean numeros</param>
        /// <returns></returns>
        public static string ToNumber(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("str");
            var result = new StringBuilder(str.Length);
            foreach (var ch in str)
            {
                if (char.IsDigit(ch))
                    result.Append(ch);
            }
            return result.ToString();
        }

        /// <summary>
        /// Conforma una nueva cadena donde la primera letra de cada palabra es mayuscula
        /// </summary>
        /// <param name="str">Cadena de la cual se toman los caracteres que sean letras, números etc (no caracteres especiales)</param>
        /// <param name="uppercase">True si se quiere que el resultado final empiece con letra mayúscula</param>
        /// <returns></returns>
        public static string ToPascal(this string str, bool uppercase)
        {
            var especialsWords = new HashSet<string>(eWords);
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("str");
            var result = new StringBuilder(str.Length);

            foreach (var ch in str.Where(ch => char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch) || ch == '-' || ch == '\u0027'))
            {
                result.Append(ch);
            }
            // Palabra a palabra del string y lo lleva a Pascal si no esta en el enumerativo
            var aux = result.ToString();
            var temp = new StringBuilder(str.Length);
            var split = aux.Split(' ', ',', '.', ':', '\t');

            foreach (var s in split)
            {
                if (!especialsWords.Contains(s) && s != "")
                {
                    temp.Append(char.ToUpper(s[0]));
                    aux = s.Substring(1);
                    foreach (var ch in aux)
                    {
                        temp.Append(char.ToLower(ch));
                    }
                    temp.Append(' ');
                    //if (s.Trim() != "")
                    //    Console.WriteLine(s);
                }
                else
                {
                    if (s != "")
                        temp.Append(s);
                    temp.Append(' ');
                }
            }
            return temp.ToString().TrimEnd();
        }

        /// <summary>
        /// Conforma una nueva cadena a partir de los caracteres de str, llevando las letras a mayuscula
        /// </summary>
        /// <param name="str">Cadena de la cual solo se transforman los caracteres que sean letras</param>
        /// <returns></returns>
        public static string ToUpperA(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("str");
            var result = new StringBuilder(str.Length);
            foreach (var ch in str)
                result.Append(char.IsLetter(ch) ? char.ToUpper(ch) : ch);
            return result.ToString();
        }

        /// <summary>
        /// Retorna las palabras que conforman un oración
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<string> Words(this string str)
        {
            str = str.Trim();
            var words = new List<string>();
            while (str != "")
            {
                var space = str.IndexOf(" ");
                var dest = "";
                dest = (space > 0 ? str.Substring(0, space) : str);
                words.Add(dest);
                var rest = str.Substring(space + 1, str.Length - space - 1).Trim();
                str = str != rest ? rest : "";
            }
            return words;
        }

        static readonly Random rand = new Random();
        /// <summary>
        /// Devuelve un string donde cada caracter es un random de la "a" a la "z"
        /// </summary>
        /// <param name="lenght">Tamaño del string resultante</param>
        /// <returns></returns>
        public static string GetRandomStr(int lenght)
        {
            var sb = new StringBuilder(lenght);
            for (var i = 0; i < lenght; i++)
                sb.Append((char)(65 + rand.Next(26)));
            return sb.ToString();
        }

        /// <summary>
        /// Devuelve la palabra donde este la ocurrencia de subtext
        /// </summary>
        /// <param name="text">Cadena donde se debe localizar</param>
        /// <param name="subtext">Cadena a localizar</param>
        /// <returns></returns>
        public static List<string> FindWord(string text, string subtext)
        {
            var str = new List<string>();
            while (text != "")
            {
                var space = text.IndexOf(" ");
                var temp = space > 0 ? text.Substring(0, space) : text;
                if (temp.Contains(subtext))
                {
                    str.Add(temp);
                }
                var rest = text.Substring(space + 1, text.Length - space - 1).TrimStart();
                text = text != rest ? rest : "";
            }
            return str;
        }

        public static IEnumerable<string> ExtractAlphaNumericWords(this string str)
        {
            var result = new HashSet<string>();
            var word = default(StringBuilder);
            foreach (var ch in str)
            {
                if (char.IsLetterOrDigit(ch))
                {
                    if (word == null)
                        word = new StringBuilder();
                    word.Append(ch);
                }
                else if (word != null)
                {
                    var actualWord = word.ToString();
                    result.Add(actualWord);
                    word = null;
                    yield return actualWord;
                }
            }
            if (word != null)
                yield return word.ToString();
        }


    }
}
