using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Add the TextMesh Pro namespace to access the various functions.

public class CountdownController : MonoBehaviour {
    public int countdownTime;

    [SerializeField] TextMeshProUGUI countdownTimer; 
    [SerializeField] TextMeshProUGUI CountDownToStart;
    [SerializeField] private float mainTimer;
    [SerializeField] private GameObject winbar;

    private float timer;
    private bool canCount = true;
    private bool doOnce = false;

    public static bool GameIsPaused = false;

    private void Start () {
        countdownTimer.gameObject.SetActive (false);
        timer = mainTimer;
        StartCoroutine (CountdownToStart ());
    }

    IEnumerator CountdownToStart () {
        while (countdownTime > 0) {
            CountDownToStart.text = countdownTime.ToString ();

            yield return new WaitForSeconds (1f);

            countdownTime--;
        }

        CountDownToStart.text = "GO!";

        GameObject.Find("Player").GetComponent<PlayerController>().enabled = true;
        GameObject.Find("Player2").GetComponent<PlayerController>().enabled = true;
        GameObject.Find("Player").GetComponent<CableComponent>().enabled = true;

        yield return new WaitForSeconds (1f);

        CountDownToStart.gameObject.SetActive (false);
        countdownTimer.gameObject.SetActive (true);

    }

    void Update () {
        if(countdownTimer.gameObject.activeSelf && winbar.activeSelf)
        {
            if (timer >= 0.0f && canCount)
            {
                timer -= Time.deltaTime;
                countdownTimer.text = timer.ToString("F");
            }
            else if (timer <= 0.0f && !doOnce)
            {
                canCount = false;
                doOnce = true;
                countdownTimer.text = "0.00";
                timer = 0.0f;
            }
        }
    }

    public float getTimer()
    {
        return this.timer;
    }

    public float getStartTime()
    {
        return this.mainTimer;
    }
}