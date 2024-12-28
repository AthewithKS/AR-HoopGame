using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    int score;
    public int throws;
    public TMP_Text scoreText;


    private void OnEnable()
    {
        BallControl.onBallThrown += BallThrown;
    }
    private void OnDisable()
    {
        BallControl.onBallThrown -= BallThrown;
    }

    private void BallThrown()
    {
        throws++;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hoop"))
        {
            score++;
            scoreText.text = "Score: " + score;
            AudioManager.PlayAudio(global::audio.ScorerSound);
        }
    }
}
