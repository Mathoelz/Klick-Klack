using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            this.GetComponent<Animator>();
            GameObject.Find("Wall").GetComponent<Animator>().SetBool("ButtonPushed",true);
        }
    }
}