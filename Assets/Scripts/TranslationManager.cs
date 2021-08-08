using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using GetText;
using Zenject;

namespace Assets.Scripts
{
    public class TranslationManager : ITranslationManager
    {
        private ICatalog catalog;
        private readonly SignalBus signalBus;

        public TranslationManager(SignalBus signalBus)
        {
            this.signalBus = signalBus;
            LoadLanguage(CultureInfo.CurrentUICulture.Name);
        }

        public void LoadLanguage(string locale)
        {
            try
            {
                using var fileStream = new FileStream($"Assets/Resources/Translation/{locale}/assets.mo", FileMode.Open);
                SetCatalog(new Catalog(fileStream));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Unable to load language for locale {locale}, using default empty catalog, got exception {e.Message}");
                SetCatalog(new Catalog());
            }
        }

        public string Translate(string text, params object[] args)
        { 
            return catalog.GetString(text, args);
        }
            

        public string TranslatePlural(string text, string plural, long n, params object[] args)
        {
            return catalog.GetPluralString(text, plural, n, args);
        }

        public string TranslateParticular(string context, string text, params object[] args)
        {
            return catalog.GetParticularString(context, text, args);
        }

        public string TranslateParticularPlural(string context, string text, string plural, long n, params object[] args)
        {
            return catalog.GetParticularPluralString(context, text, plural, n, args);
        }

        private void SetCatalog(ICatalog catalog)
        {
            this.catalog = catalog;
            signalBus.Fire<LanguageChangedSignal>();
        }
    }
}