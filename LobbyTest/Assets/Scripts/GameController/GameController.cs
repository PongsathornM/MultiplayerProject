using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private int score;

    [SerializeField]private Text scoreText;
    
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScore(int addScore)
    {
        score += addScore;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score : " + score;
    }
}
