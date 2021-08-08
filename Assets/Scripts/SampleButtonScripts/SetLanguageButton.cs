using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.SampleButtonScripts
{
    [RequireComponent(typeof(Button))]
    public class SetLanguageButton : MonoBehaviour
    {
        [SerializeField] private string locale = "en-GB";

        private Button button;

        [Inject] private ITranslationManager translationManager;

        private void Start()
        {
            button = GetComponent<Button>();

            button.onClick.AddListener(SetLanguage);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(SetLanguage);
        }

        private void SetLanguage()
        {
            translationManager.LoadLanguage(locale);
        }
    }
}