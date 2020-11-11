using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text highScoreText;

    public void Start()
    {
        //ハイスコアを表示
        highScoreText.text = "High Score :" + PlayerPrefs.GetInt("HighScore");
    }

    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("Main");
    }
    public void EndGame()
    {
        //UnityEditorで止めたい場合
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
