using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace Assets.Scripts
{
    [RequireComponent(typeof(TMP_Text))]
    public class TranslateLabelString : MonoBehaviour
    {
        private TMP_Text text;
        private string originalText;

        [SerializeField] private string context;
        [SerializeField] private string pluralText;
        [SerializeField] private long pluralNumber = 1;
        [SerializeField] private int formatParamsAmount = 0;

        private int prevFormatParamsAmount = 0;
        private long prevPluralNumber = 1;
        private object[] formatParams;
        private bool dirty = true;

        [Inject] private ITranslationManager translationManager;
        [Inject] private SignalBus signalBus;

        private void Awake()
        {
            formatParams = new object[formatParamsAmount];
            prevFormatParamsAmount = formatParamsAmount;
        }

        private void Start()
        {
            text = GetComponent<TMP_Text>();
            originalText = text.text;

            signalBus.Subscribe<LanguageChangedSignal>(SetDirty);

            if (!string.IsNullOrWhiteSpace(pluralText) && formatParamsAmount > 0)
            {
                SetFormatParamsArgument(pluralNumber, 0);
            }

            Translate();
        }

        private void SetDirty()
        {
            dirty = true;
        }

        private void Update()
        {
            if (formatParamsAmount != prevFormatParamsAmount)
            {
                // Enable changes to amount of parameters in the editor during runtime
                var oldParams = formatParams;
                formatParams = new object[formatParamsAmount];

                for (var i = 0; i < Math.Min(formatParamsAmount, prevFormatParamsAmount); i++)
                {
                    formatParams[i] = oldParams[i];
                }

                prevFormatParamsAmount = formatParamsAmount;
                dirty = true;
            }

            if (pluralNumber != prevPluralNumber)
            {
                // Enable changing the plural number in the editor during runtime
                prevPluralNumber = pluralNumber;
                dirty = true;
            }

            if (dirty)
            {
                Translate();
            }
        }

        private void OnDestroy()
        {
            signalBus.Unsubscribe<LanguageChangedSignal>(SetDirty);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Get the translation object, containing all the necessary information to construct
        /// a Gettext file.
        /// </summary>
        /// <returns></returns>
        public TranslationObject GetTranslationObject() => new TranslationObject
        {
            PrimaryString = GetComponent<TMP_Text>().text,
            PluralString = pluralText,
            Context = string.IsNullOrWhiteSpace(context) ? null : context,
            FormatString = formatParamsAmount > 0,
        };
#endif

        public void SetPluralNumber(long number, bool setFormatParam = true)
        {
            pluralNumber = number;
            dirty = true;

            if (setFormatParam)
            {
                SetFormatParamsArgument(number, 0);
            }
        }

        public void SetFormatParamsArgument(object argument, int argumentNumber)
        {
            formatParams[argumentNumber] = argument;
            dirty = true;
        }

        private void Translate()
        {
            var hasContext = !string.IsNullOrWhiteSpace(context);
            var hasPlural = !string.IsNullOrWhiteSpace(pluralText);

            text.text =
                hasContext && hasPlural ? translationManager.TranslateParticularPlural(context, originalText, pluralText, pluralNumber, formatParams)
                : hasContext ? translationManager.TranslateParticular(context, originalText, formatParams)
                : hasPlural ? translationManager.TranslatePlural(originalText, pluralText, pluralNumber, formatParams)
                : translationManager.Translate(originalText, formatParams);

            dirty = false;
        }
    }
}
