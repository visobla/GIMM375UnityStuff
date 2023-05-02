using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPlayer : MonoBehaviour
{
    public GameObject Player;
    public GameObject RespawnPoint;

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            Player.transform.position = RespawnPoint.transform.position;
        }
    }
}
