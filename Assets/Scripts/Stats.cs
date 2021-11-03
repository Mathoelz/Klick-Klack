using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Stats : MonoBehaviour
{
    BinaryFormatter bf = new BinaryFormatter();
    private List<Score> highscores = new List<Score>();
    private List<Score> marathon = new List<Score>();
    private bool marathonmode = false;
    public int lifes = 5;
    private void Awake()
    {
        this.Load();
        DontDestroyOnLoad(gameObject);
    }

    public void Save()
    {
        Score current;
        foreach (Score score in this.marathon) 
        {
            current = this.doesScoreExist(score.getLevel());
            if(current != null)
            {
                current.updateScore(score.getTime(), score.getScore());
            }
            else
            {
                this.highscores.Add(score);
            }
        }
        FileStream file = File.Create(Application.persistentDataPath + "/gamestats.save");
        bf.Serialize(file, highscores);
        file.Close();
    }

    public void Load()
    {
        if(File.Exists(Application.persistentDataPath + "/gamestats.save"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/gamestats.save", FileMode.Open);
            this.highscores = (List<Score>)(bf.Deserialize(file));
            file.Close();
        }
    }

    public void AddScore(string levelname, float time, int points)
    {
        Score score = new Score(levelname, time, points);
        this.marathon.Add(score);
    }

    public Score doesScoreExist(string level)
    {
        foreach(Score score in this.highscores)
        {
            if(level == score.getLevel())
            {
                return score;
            }
        }
        return null;
    }
    
    public void DeleteScores()
    {
        if(File.Exists(Application.persistentDataPath + "/gamestats.save"))
        {
            File.Delete(Application.persistentDataPath + "/gamestats.save");
        }
        this.highscores.Clear();
    }

    #region getters and setters
    public bool getMarathonMode()
    {
        return this.marathonmode;
    }

    public void setMarathonMode(bool mode)
    {
        this.marathonmode = mode;
    }

    public int getSumScore()
    {
        int sum = 0;
        foreach(Score score in this.marathon)
        {
            sum += score.getScore();
        }
        return sum;
    }

    public float getSumTime()
    {
        float sum = 0f;
        foreach(Score score in this.marathon)
        {
            sum += score.getTime();
        }
        return sum;
    }

    public void setLifes(int lifes)
    {
        this.lifes = lifes;
    }

    public int getLifes()
    {
        return this.lifes;
    }

    public int levelCount()
    {
        return this.highscores.Count;
    }
    #endregion
}
