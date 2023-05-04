using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulDoor : MonoBehaviour
{
    public PlayerInventory _playerInvetory;
    public int soulsNeeded;
    public GameObject door;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip squeak;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("In Zone");
            if(_playerInvetory.AmountOfCollected >= soulsNeeded)
            {
                Destroy(door);
                if (door != null)
                {
                    audioSource.PlayOneShot(squeak);
                }
            }
        }
    }
}
