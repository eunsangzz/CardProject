using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void MainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void MainSecne()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void StartBtn()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void TutoBtn()
    {
        SceneManager.LoadScene("Tuto");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
