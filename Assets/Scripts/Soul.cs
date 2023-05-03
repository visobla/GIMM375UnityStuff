using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickup;
    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();

        if (playerInventory != null)
        {
            audioSource.pitch = (Random.Range(0.6f, 1.3f));
            audioSource.PlayOneShot(pickup);
            playerInventory.SoulsCollected();
            gameObject.SetActive(false);
        }
    }
}
