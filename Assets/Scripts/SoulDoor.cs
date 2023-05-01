using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulDoor : MonoBehaviour
{
    public PlayerInventory _playerInvetory;
    public int soulsNeeded;
    public GameObject door;

    void onTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("In Zone");
            if(_playerInvetory.AmountOfCollected >= soulsNeeded)
            {
                Destroy(door);
            }
        }
    }
}
