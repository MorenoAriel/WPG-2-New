using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip buttonClickSound;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    //Ini UI menu simple banget tapi bisa di scale kok, jadi kalau mau buat menu yang lebih bagus bisa di kembangkan lagi, tapi untuk sekarang ini cukup untuk prototype aja ya
    public void Play_Game()
    {
        PlayButtonClickSound();
        SceneManager.LoadScene("MainScenery");
//ganti nama scene dengan nama scene yang akan di load pertama kali (namanya harus sama ya(huruf kapital dan yang lainya))
        Debug.Log("Play Game");
    }

    public void Exit()
    {
        PlayButtonClickSound();
        Debug.Log("Exit");
        Application.Quit();//ini sudah langsung keluar kalau muncul debug berarti udah keluar mungkin di play unity tidak terlihat tapi di build sudah keluar kok jadi tenang aja
    }

    public void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    
}
