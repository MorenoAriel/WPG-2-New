using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPanelUI : MonoBehaviour
{
    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";
    public string bonusGameSceneName = "BonusGame";

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void RestartBonusGame()
    {
        PlayClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene(bonusGameSceneName);
    }

    public void BackToMenu()
    {
        PlayClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void PlayClick()
    {
        if (audioSource != null && buttonClickSound != null)
            audioSource.PlayOneShot(buttonClickSound);
    }
}