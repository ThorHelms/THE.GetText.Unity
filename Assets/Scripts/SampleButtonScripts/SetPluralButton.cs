using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.SampleButtonScripts
{
    [RequireComponent(typeof(Button))]
    public class SetPluralButton : MonoBehaviour
    {
        [SerializeField] private int pluralValue = 0;
        [SerializeField] private TranslateLabelString[] labels;

        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();

            button.onClick.AddListener(SetPluralValue);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(SetPluralValue);
        }

        private void SetPluralValue()
        {
            foreach (var label in labels)
            {
                label.SetPluralNumber(pluralValue);
            }
        }
    }
}