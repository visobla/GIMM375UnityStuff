using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulTutorial : MonoBehaviour
{
    public GameObject tutorial;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        { 
                Destroy(tutorial);
        }
    }
}
