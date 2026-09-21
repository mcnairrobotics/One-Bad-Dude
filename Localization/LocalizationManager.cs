using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    public enum Language
    {
        English,
        Spanish,
        German,
        French,
        Italian,
        Russian,
        Japanese,
        Portuguese,
        Chinese,
        Korean
    }

    [SerializeField] private TextAsset localizationCSV;

    public Language currentLanguage = Language.English;

    private Dictionary<string, Dictionary<Language, string>> translations =
        new Dictionary<string, Dictionary<Language, string>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadCSV();
        LoadLanguage();
    }

    private void LoadCSV()
    {
        translations.Clear();

        List<string[]> rows = ParseCSV(localizationCSV.text);

        if (rows.Count == 0)
        {
            Debug.LogError("Localization CSV is empty!");
            return;
        }

        // First row is the header
        string[] header = rows[0];

        for (int i = 1; i < rows.Count; i++)
        {
            string[] columns = rows[i];

            if (columns.Length < 11)
            {
                Debug.LogWarning(
                    $"Localization row {i} has only {columns.Length} columns."
                );

                continue;
            }

            string key = columns[0].Trim();

            if (string.IsNullOrEmpty(key))
                continue;

            Dictionary<Language, string> languageTexts =
                new Dictionary<Language, string>();

            languageTexts[Language.English] =
                CleanText(columns[1]);

            languageTexts[Language.Spanish] =
                CleanText(columns[2]);

            languageTexts[Language.German] =
                CleanText(columns[3]);

            languageTexts[Language.French] =
                CleanText(columns[4]);

            languageTexts[Language.Italian] =
                CleanText(columns[5]);

            languageTexts[Language.Russian] =
                CleanText(columns[6]);

            languageTexts[Language.Japanese] =
                CleanText(columns[7]);

            languageTexts[Language.Portuguese] =
                CleanText(columns[8]);

            languageTexts[Language.Chinese] =
                CleanText(columns[9]);

            languageTexts[Language.Korean] =
                CleanText(columns[10]);

            translations[key] = languageTexts;
        }

        Debug.Log(
            $"Loaded {translations.Count} localization entries."
        );
    }

    private string CleanText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        // Convert literal \n from the CSV into actual newlines
        text = text.Replace("\\n", "\n");

        return text.Trim();
    }

    public string GetText(string key)
    {
        if (!translations.ContainsKey(key))
        {
            Debug.LogWarning(
                $"Missing localization key: {key}"
            );

            return "[" + key + "]";
        }

        if (!translations[key].ContainsKey(currentLanguage))
        {
            Debug.LogWarning(
                $"Missing language for key: {key}"
            );

            return "[" + key + "]";
        }

        string text = translations[key][currentLanguage];

        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning(
                $"Empty translation for {key} " +
                $"in {currentLanguage}"
            );

            return "[" + key + "]";
        }

        return text;
    }
    public void SetLanguage(int langNum)
    {
        currentLanguage = (Language)langNum;
        SetLanguage(currentLanguage);
    }
    public void SetLanguage(Language language)
    {
        currentLanguage = language;

        PlayerPrefs.SetInt(
            "Language",
            (int)language
        );

        PlayerPrefs.Save();

        /*LocalizedText[] texts =
            FindObjectsByType<LocalizedText>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (LocalizedText text in texts)
        {
            text.UpdateText();
        }*/
    }

    private void LoadLanguage()
    {
        if (PlayerPrefs.HasKey("Language"))
        {
            currentLanguage =
                (Language)PlayerPrefs.GetInt("Language");
        }
        else
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Spanish:
                    currentLanguage = Language.Spanish;
                    break;

                case SystemLanguage.German:
                    currentLanguage = Language.German;
                    break;

                case SystemLanguage.French:
                    currentLanguage = Language.French;
                    break;

                case SystemLanguage.Italian:
                    currentLanguage = Language.Italian;
                    break;

                case SystemLanguage.Russian:
                    currentLanguage = Language.Russian;
                    break;

                case SystemLanguage.Japanese:
                    currentLanguage = Language.Japanese;
                    break;

                case SystemLanguage.Portuguese:
                    currentLanguage = Language.Portuguese;
                    break;

                case SystemLanguage.Chinese:
                    currentLanguage = Language.Chinese;
                    break;

                case SystemLanguage.Korean:
                    currentLanguage = Language.Korean;
                    break;

                default:
                    currentLanguage = Language.English;
                    break;
            }
        }
    }

    // =====================================================
    // CSV PARSER
    // =====================================================

    private List<string[]> ParseCSV(string csv)
    {
        List<string[]> rows =
            new List<string[]>();

        List<string> currentRow =
            new List<string>();

        StringBuilder currentField =
            new StringBuilder();

        bool insideQuotes = false;

        for (int i = 0; i < csv.Length; i++)
        {
            char c = csv[i];

            // ---------------------------------------------
            // QUOTE
            // ---------------------------------------------

            if (c == '"')
            {
                // Double quote inside a quoted field
                if (insideQuotes &&
                    i + 1 < csv.Length &&
                    csv[i + 1] == '"')
                {
                    currentField.Append('"');
                    i++;
                }
                else
                {
                    insideQuotes = !insideQuotes;
                }

                continue;
            }

            // ---------------------------------------------
            // COMMA
            // ---------------------------------------------

            if (c == ',' && !insideQuotes)
            {
                currentRow.Add(
                    currentField.ToString()
                );

                currentField.Clear();

                continue;
            }

            // ---------------------------------------------
            // NEW LINE
            // ---------------------------------------------

            if ((c == '\n' || c == '\r') &&
                !insideQuotes)
            {
                // Handle Windows \r\n
                if (c == '\r' &&
                    i + 1 < csv.Length &&
                    csv[i + 1] == '\n')
                {
                    i++;
                }

                currentRow.Add(
                    currentField.ToString()
                );

                currentField.Clear();

                // Ignore completely empty rows
                if (currentRow.Count > 1 ||
                    !string.IsNullOrWhiteSpace(currentRow[0]))
                {
                    rows.Add(
                        currentRow.ToArray()
                    );
                }

                currentRow.Clear();

                continue;
            }

            // ---------------------------------------------
            // NORMAL CHARACTER
            // ---------------------------------------------

            currentField.Append(c);
        }

        // Add final field/row
        if (currentField.Length > 0 ||
            currentRow.Count > 0)
        {
            currentRow.Add(
                currentField.ToString()
            );

            rows.Add(
                currentRow.ToArray()
            );
        }

        return rows;
    }
}