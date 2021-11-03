 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 
 public class Bumper : MonoBehaviour
 {
     [SerializeField] private int bumperForce = 800;
 
     public void OnTriggerEnter (Collider collider)
     {
         if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "Player2") {
             collider.gameObject.GetComponent<Rigidbody>().AddExplosionForce(bumperForce, transform.position, 1);
         }
     }
 }