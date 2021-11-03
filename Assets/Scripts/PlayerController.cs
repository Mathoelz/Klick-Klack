using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;

public class PlayerController : MonoBehaviour {

    public TextMeshProUGUI points;
    public float Mass = 1000;
    public float Turnspeed = 10;
    public GameObject Sequence;

    [SerializeField] private Transform camPivot;
    [SerializeField] private TextMeshProUGUI lifes;
    [SerializeField] private AudioMixer volume;
    private CountdownController timer;
    private int count = 0;
    private Vector3 movementForce;
    private Rigidbody rb;
    private Stats gamestats;

    public Vector3 MovementForce
    {
        get { return movementForce;  }
    }
    void Start () {
        volume.SetFloat("MainVolume", Mathf.Log10(PlayerPrefs.GetFloat("GameVolume")) * 20);
        gamestats = GameObject.FindGameObjectWithTag("Gamestats").GetComponent<Stats>();
        timer = GameObject.Find("Interface").GetComponent<CountdownController>();
        rb = GetComponent<Rigidbody>();
        camPivot.rotation = transform.rotation;
        setLifes();
        SetScoreText();
        if (gamestats.getMarathonMode() == false)
        {
            lifes.gameObject.SetActive(false);
        }
        if (camPivot == null)
        {
            if (rb.CompareTag("Player"))
            {
                camPivot = GameObject.Find("CamPivot1").GetComponent<Transform>();
            }
            else
            {
                camPivot = GameObject.Find("CamPivot2").GetComponent<Transform>();
            }
        }
        
    }

    void FixedUpdate () {

        if (!this.Sequence.activeSelf)
        {
            camPivot.position = transform.position;
            float moveHorizontal;
            float moveVertical;
            if (rb.CompareTag("Player"))
            {
                moveHorizontal = Input.GetAxis("Horizontal");
                moveVertical = Input.GetAxis("Vertical");

            }
            else
            {
                moveHorizontal = Input.GetAxis("Horizontal2");
                moveVertical = Input.GetAxis("Vertical2");

            }
            var forward = camPivot.transform.forward;
            var right = camPivot.transform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 movement = forward * moveVertical + right * 0.3f * moveHorizontal;
            camPivot.Rotate(new Vector3(0, moveHorizontal, 0) * Turnspeed * Time.fixedDeltaTime);

            movementForce = movement * Mass;
            rb.AddForce(movementForce);
        }

        if(this.timer.getTimer() <= 0)
        {
            this.timeOut();
        }
    }

    void OnTriggerEnter (Collider other) {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.GetComponentInParent<AudioSource>().PlayOneShot(other.GetComponentInParent<AudioSource>().clip);
            other.gameObject.SetActive(false);
            count += 10;
            SetScoreText();
        }
        else if (other.CompareTag("Bumper"))
        {

        }
        else
        {
            this.startSequence(other);
        }
    }

    void SetScoreText () {
        if (gamestats.getMarathonMode())
        {
            points.text = "Score: " + (gamestats.getSumScore() + count).ToString();
        }
        else {
            points.text = "Score: " + count.ToString();
        }
    }

    void startSequence(Collider collider)
    {
        TextMeshProUGUI[] userinfo = this.Sequence.GetComponentsInChildren<TextMeshProUGUI>();

        if (collider.CompareTag("Win"))
        {
            GameObject.FindGameObjectWithTag("Gameover").SetActive(false);
            collider.GetComponentInParent<AudioSource>().PlayOneShot(collider.GetComponentInParent<AudioSource>().clip);
            collider.gameObject.SetActive(false);
            SetSequenceText("Win");
            this.saveStats(timer.getStartTime() - this.timer.getTimer(), count);
        }
        else if (collider.CompareTag("Gameover"))
        {
            GameObject.FindGameObjectWithTag("Win").SetActive(false);
            collider.GetComponent<AudioSource>().PlayOneShot(collider.GetComponent<AudioSource>().clip);
            SetSequenceText("Lose");
        }
        this.Sequence.SetActive(true);
    }

    void saveStats(float time, int score)
    {
        gamestats.GetComponent<Stats>().AddScore(SceneManager.GetActiveScene().name, time, score);
    }

    void timeOut()
    {
        TextMeshProUGUI message = this.Sequence.transform.Find("UserInformation").GetComponent<TextMeshProUGUI>();
        message.text = "Time over!";
        this.Sequence.SetActive(true);
    }

    void setLifes()
    {
        lifes.text = "Lifes: " + gamestats.getLifes().ToString();
    }

    private void SetSequenceText(string text) 
    {
        TextMeshProUGUI[] userinfo = this.Sequence.GetComponentsInChildren<TextMeshProUGUI>();

        foreach(TextMeshProUGUI info in userinfo)
        {
            if(info.name == "Score")
            {
                if(text == "Win")
                {
                    count += Mathf.RoundToInt(timer.getTimer() * 100);
                    SetScoreText();
                }
                else
                {

                }
            }
            else
            {
                if(text == "Win")
                {
                    info.fontSize = 40;
                    info.text = "Level Complete \n Level Clear Bonus: \n" + timer.getTimer().ToString("F2") + " x 100 = " + Mathf.RoundToInt(timer.getTimer() * 100);
                }
                else
                {
                    if (gamestats.getLifes() == 1)
                    {
                        info.text = "GAME OVER!";
                    }
                    else
                    {
                        info.text = "Fallout!";
                    }
                }
            }
        }
    }
}