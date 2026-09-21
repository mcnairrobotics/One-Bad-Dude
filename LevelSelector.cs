using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [System.Serializable]
    public class Level
    {
        public string levelName;
        public string sceneName;
        public Sprite image;

        // PlayerPrefs key used to determine if this level is unlocked
        public string unlockKey;
    }

    public Level[] levels;

    public Image levelImage;
    public LocalizedText levelText;

    private int currentLevel = 0;

    private void Start()
    {
        // Find the first unlocked level
        for (int i = 0; i < levels.Length; i++)
        {
            if (IsUnlocked(i))
            {
                currentLevel = i;
                break;
            }
        }

        UpdateLevel();
    }

    public void NextLevel()
    {
        int next = currentLevel + 1;

        if (next < levels.Length)//IsUnlocked(next)
        {
            currentLevel = next;
            UpdateLevel();
        }
    }

    public void PreviousLevel()
    {
        int previous = currentLevel - 1;

        if (previous >= 0)
        {
            currentLevel = previous;
            UpdateLevel();
        }
    }

    private void UpdateLevel()
    {
        Level level = levels[currentLevel];

        levelImage.sprite = level.image;
        levelText.updateKey(level.levelName);
    }

    public void LoadCurrentLevel()
    {
        //if (IsUnlocked(currentLevel))
        //{
            SceneManager.LoadScene(levels[currentLevel].sceneName);
        //}
    }

    private bool IsUnlocked(int index)
    {
        // First level is always unlocked
        if (index == 0)
            return true;

        // PlayerPrefs value of 1 = unlocked
        return PlayerPrefs.GetInt(levels[index].unlockKey, 0) == 1;
    }

    // Call this when the player beats a level
    public void UnlockLevel(int index)
    {
        if (index >= 0 && index < levels.Length)
        {
            PlayerPrefs.SetInt(levels[index].unlockKey, 1);
            PlayerPrefs.Save();
        }
    }
}