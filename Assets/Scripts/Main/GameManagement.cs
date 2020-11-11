using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagement : MonoBehaviour
{
    //スコア
    public Text scoreText;
    private int score;

    // Start is called before the first frame update
    void Start()
    {
        //scoreを初期化する
        score = 0;
    }
    void Update()
    {
        if (PlayerPrefs.GetInt("HighScore") < score)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
    }
    
    //スコアを追加
    public void AddScore()
    {
        score += 100;
        scoreText.text = "Score: " + score.ToString();
    }
    public void GameOver()
    {
        //Debug.Log("GameOver呼び出された");
        SceneManager.LoadScene("GameSet");
    }
}
