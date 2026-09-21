using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneSwitch : MonoBehaviour
{
    private bool isPaused;
    public GameObject pauseObject;
    int difficulty;
    string[] diffNames = {"Easy", "Medium", "Hard"};
    string[] csvDiffs = {"EASY", "MEDI", "HARD"};
    public TMP_Text difficultyText;
    public Animator TitleAnimator;
    public Slider volumeSlider;
    void Start()
    {
        UpdateText();
        difficulty = PlayerPrefs.GetInt("Difficulty", 1);
        if(difficultyText != null)
            difficultyText.text = diffNames[difficulty];
        LoadSettings();
    }
    public void UpdateLanguage(int languageNum)
    {
        if (LocalizationManager.Instance == null)
            return;

        LocalizationManager.Instance.SetLanguage(languageNum);
        SceneManager.LoadScene("Title");
        //loadScene("Title");
    }
    public void UpdateText()
    {
        if (LocalizationManager.Instance == null)
            return;
        for(int i = 0; i < diffNames.Length; i++)
        {
            diffNames[i] = LocalizationManager.Instance.GetText(csvDiffs[i]);
        }
    }
    public void loadScene(string sceneName){
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
        SceneManager.LoadSceneAsync(sceneName);
    }
    public void quit(){
        Application.Quit();
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    public void pause(){
        isPaused = !isPaused;

        if (isPaused)
        {
            pauseObject.SetActive(true);
            Time.timeScale = 0.0f; // Pause the game
            // Optional: Pause audio
            AudioListener.pause = true;
        }
        else
        {
            pauseObject.SetActive(false);
            Time.timeScale = 1.0f; // Resume normal time
            // Optional: Resume audio
            AudioListener.pause = false;
        }
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            pause();
    }
    public void changeDifficulty()
    {
        difficulty += 1;
        difficulty %= 3;
        PlayerPrefs.SetInt("Difficulty", difficulty);
        difficultyText.text = diffNames[difficulty];
    }
    public void startAnim(){
        TitleAnimator.Play("TitleCutscene");
    }
        public void SetVolume()
    {
        float volume = volumeSlider.value;
        // Set global Unity audio volume
        AudioListener.volume = volume;

        // Save the setting
        PlayerPrefs.SetFloat("Vol", volume);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        // Default to full volume if no setting has been saved
        float volume = PlayerPrefs.GetFloat("Vol", 1f);

        AudioListener.volume = volume;

        // Update the slider to match the saved volume
        if (volumeSlider != null)
            volumeSlider.value = volume;
    }


}
