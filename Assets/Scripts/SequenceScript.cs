using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SequenceScript : MonoBehaviour
{
    private TextMeshProUGUI userinfo;
    private Stats gamestats; 

    private void Start()
    {
        gamestats = GameObject.FindGameObjectWithTag("Gamestats").GetComponent<Stats>();
    }

    public void StartSequence()
    {
        userinfo = this.transform.Find("UserInformation").GetComponent<TextMeshProUGUI>();
        if(userinfo.text == "Fallout!" || userinfo.text == "Time over!")
        {
            if(gamestats.getLifes() >= 1 && gamestats.getMarathonMode())
            {
                gamestats.setLifes(gamestats.getLifes() - 1);
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if(userinfo.text.Contains("Level Complete"))
        {
            if (gamestats.getMarathonMode() && SceneManager.GetActiveScene().buildIndex != SceneManager.sceneCountInBuildSettings - 1)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                this.MarathonDone();
            }
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    } 

    public void MarathonDone()
    {
        if (gamestats.getMarathonMode())
        {
            gamestats.AddScore("Marathon", gamestats.getSumTime(), gamestats.getSumScore());
        }
        gamestats.Save();
        Destroy(gamestats.gameObject);
        SceneManager.LoadScene("MainMenu");
    }

}
