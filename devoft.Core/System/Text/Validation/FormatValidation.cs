using System.Linq;
using System.Text.RegularExpressions;

namespace System
{
    public static class FormatValidation
    {
        public static bool IsMatching(this Regex r, string text)
            => r.Matches(text).Cast<Match>().Any(x => x.Value == text);

        public static bool IsValidEmail(string text)
            => EmailParser.IsMatching(text);

        private static Regex _emailParser;
        private static Regex EmailParser
            => _emailParser ?? (_emailParser = new Regex(
                                                    //@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                                                    @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))+(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                                                    RegexOptions.Compiled));

        public static bool IsPascal(string text)
            => PascalParser.IsMatching(text);

        private static Regex _pascalParser;
        private static Regex PascalParser
            => _pascalParser ?? (_pascalParser = new Regex(
                                                    @"([A-Z]{1})([a-z]+)",
                                                    RegexOptions.Compiled));

        private static Regex _personNameParser;
        public static bool IsPersonName(string name)
            => PersonNameParser.IsMatching(name);

        //TODO: Dar soporte a apellidos del estilo Garcia-Linares o D'Strampes. 
        //TODO: Se debe de permitir los valores de Mª, Mª., permitir ªº

        //Se permite Garcia-linares o D'strampes
        private static Regex PersonNameParser
            => _personNameParser ?? (_personNameParser = new Regex(@"([A-Z]{1}|)([a-zá-úÁ-Úä-üÄ-Üà-ùÀ-Ù(\-)?(\')?]*)+((\s[a-zA-Zá-úÁ-Úä-üÄ-Üà-ùÀ-Ù((\-)?(\')?]+)*)", RegexOptions.Compiled));

        private static Regex _urlParser;

        public static bool IsValidUrl(string text) => UrlParser.IsMatching(text);

        private static Regex UrlParser
            => _urlParser ?? (_urlParser = new Regex(
                                                    //@"((http([s]?)://)*(www\.){1})+(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+(/(([a-zA-Z0-9_\.\-])+))*",
                                                    @"(((http([s]?)://)?))((www\.)?)(((([a-zA-Z0-9_\-])+\.))+)([a-zA-Z0-9_\-])+([a-zA-Z0-9]{1,4})+(/(([a-zA-Z0-9_\.\-\?])+))*",
                                                    RegexOptions.Compiled));
    }
}
