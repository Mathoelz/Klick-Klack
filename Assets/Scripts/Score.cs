using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Score
{
    private string level;
    private float time = 0;
    private int score = 0;

    public Score(string level, float time, int score)
    {
        this.setLevel(level);
        this.setTime(time);
        this.setScore(score);
    }

    #region Getters and setters
    public void setTime(float time)
    {
        this.time = time;
    }

    public void setScore(int score)
    {
        this.score = score;
    }

    public float getTime()
    {
        return this.time;
    }

    public int getScore()
    {
        return this.score;
    }

    public void setLevel(string level)
    {
        this.level = level;
    }

    public string getLevel()
    {
        return this.level;
    }
    #endregion

    public void updateScore(float time, int score)
    {
        if(time <= this.time)
        {
            this.time = time;
        }
        if(score >= this.score)
        {
            this.score = score;
        }
    }
}
