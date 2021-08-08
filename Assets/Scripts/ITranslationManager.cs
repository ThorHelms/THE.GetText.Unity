namespace Assets.Scripts
{
    public interface ITranslationManager
    {
        void LoadLanguage(string locale);
        string Translate(string text, params object[] args);
        string TranslatePlural(string text, string plural, long n, params object[] args);
        string TranslateParticular(string context, string text, params object[] args);
        string TranslateParticularPlural(string context, string text, string plural, long n, params object[] args);
    }
}