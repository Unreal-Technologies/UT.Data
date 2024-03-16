using System.Runtime.CompilerServices;

namespace UT.Data
{
    public abstract class Strings
    {
        #region Members
        private static Dictionary<string, string>? text;
        private static Languages? current;
        #endregion //Members

        #region Enums
        public enum Languages
        {
            En, Nl
        }
        #endregion //Enums

        #region Properties
        public static string String_InputError { get { return Strings.GetValue("S!InputError"); } }
        public static string String_RequiredField { get { return Strings.GetValue("S!RequiredField"); } }
        public static Languages? LanguageOverride { get; set; }
        public static Languages Language { 
            get; 
            set; 
        }
        public static Languages? Current { get { return Strings.current; } protected set { Strings.current=value; } }
        #endregion //Properties

        #region Public Methods
        public static string GetKey(string value)
        {
            Strings.Load();
            if (Strings.text == null)
            {
                return value;
            }

            string? key = text.Where(x => x.Value == value).Select(x => x.Key).FirstOrDefault();
            if(key == null)
            {
                return value;
            }
            return key;
        }

        public static string GetValue(string key)
        {
            Strings.Load();
            if (Strings.text == null)
            {
                return key;
            }

            if (text.TryGetValue(key, out string? value))
            {
                return value;
            }

            return key;
        }
        #endregion //Public Methods

        #region Private Methods
        private static void Load()
        {
            Languages language = Strings.Language;
            if (Strings.LanguageOverride != null)
            {
                language = Strings.LanguageOverride.Value;
            }

            if ((Strings.current == null || Strings.text == null))
            {
                Strings.Load(language);
            }
        }

        private static void Load(Languages language)
        {
            string? content = Resources.ResourceManager.GetString(language.ToString());
            if(content == null)
            {
                return;
            }

            Strings.text = [];
            foreach(string line in content.Split("\r\n"))
            {
                string[] components = line.Split(":");
                string left = string.Join(":", components.Take(1));
                string right = string.Join(":", components.Skip(1));

                Strings.text.Add(left, right);
            }
            Strings.current = language;
        }
        #endregion //Private Methods
    }
}
