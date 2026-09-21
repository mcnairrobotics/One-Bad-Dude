using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string localizationKey;

    private TMP_Text text;
    private float originalFontSize = 0;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        originalFontSize = text.fontSize;
    }

    void Start()
    {
        UpdateText();
    }
    private void OnEnable()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        if (LocalizationManager.Instance == null)
            return;

        text.text =
            LocalizationManager.Instance.GetText(localizationKey);
        if (LocalizationManager.Instance.currentLanguage != 0)
        {
            text.fontSize = originalFontSize / 1.3f;
        }
    }
    public void updateKey(string newKey)
    {
        localizationKey = newKey;
        UpdateText();
    }
    public void clearText()
    {
        text.text = "";
    }
}