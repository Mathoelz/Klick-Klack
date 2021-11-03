using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Stats gamestats;
    [SerializeField] private GameObject[] levelinfo;
    [SerializeField] private AudioMixer volume;
    [SerializeField] private Slider slider;
    [SerializeField] private Slider health;
    [SerializeField] private TextMeshProUGUI lifes;
    private void Start()
    {
        SetLevelStats();
        slider.value = PlayerPrefs.GetFloat("GameVolume", 0.75f);
        health.value = PlayerPrefs.GetFloat("Lifes", 5);
        gamestats.setLifes(Mathf.RoundToInt(health.value));
        SetVolume();
    }

    public void PlayGame()
    {
        gamestats.setMarathonMode(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    public void LoadLevel(int level)
    {
        switch(level)
        {
            case 1:
                SceneManager.LoadScene("Level 1");
                break;
            case 2:
                SceneManager.LoadScene("Level 2");
                break;
            case 3:
                SceneManager.LoadScene("Level 3");
                break;
            case 4:
                SceneManager.LoadScene("Level 4");
                break;
            case 5:
                SceneManager.LoadScene("Level 5");
                break;
            case 6:
                SceneManager.LoadScene("Level 6");
                break;
            default:
                break;
        }
    }

    private void SetLevelStats()
    {
        for(int i = 0; i < levelinfo.Length; ++i)
        {
            Score current = gamestats.doesScoreExist(levelinfo[i].name);
            if(current != null)
            {
                levelinfo[i].GetComponent<TextMeshProUGUI>().text = current.getLevel() + "\nScore: " + current.getScore() + " Time: " + current.getTime().ToString("F2");
            }
            else
            {
                levelinfo[i].GetComponent<TextMeshProUGUI>().text = levelinfo[i].name + "\nScore: 0 Time: No Record"; 
            }
        }
    }

    public void SetVolume()
    {
        volume.SetFloat("MainVolume", Mathf.Log10(slider.value) * 20);
        PlayerPrefs.SetFloat("GameVolume", slider.value);
    }

    public void SetLifes()
    {
        lifes.text = health.value.ToString();
        gamestats.setLifes(Mathf.RoundToInt(health.value));
        PlayerPrefs.SetFloat("Lifes", health.value);
    }

    public void ResetStats()
    {
        gamestats.DeleteScores();
        SetLevelStats();
    }
}
