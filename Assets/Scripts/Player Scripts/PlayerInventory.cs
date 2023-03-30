using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    public TMP_Text _soulInfo;
    public int AmountOfCollected { get; private set; }

    public void SoulsCollected()
    {
        AmountOfCollected++;
        _soulInfo.text = AmountOfCollected.ToString();
    }
}
