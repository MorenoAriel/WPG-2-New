using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{

    //Ini UI menu simple banget tapi bisa di scale kok, jadi kalau mau buat menu yang lebih bagus bisa di kembangkan lagi, tapi untuk sekarang ini cukup untuk prototype aja ya
    public void Play_Game()
    {
        SceneManager.LoadScene("MainScenery");
//ganti nama scene dengan nama scene yang akan di load pertama kali (namanya harus sama ya(huruf kapital dan yang lainya))
        Debug.Log("Play Game");
    }
    public void Exit()
    {
        Debug.Log("Exit");
        Application.Quit();//ini sudah langsung keluar kalau muncul debug berarti udah keluar mungkin di play unity tidak terlihat tapi di build sudah keluar kok jadi tenang aja
    }
    
}
