using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] int value;

    public void GetPickedUp()
    {
        FindObjectOfType<PlayerStats>().IncreaseCurrencyBy(value);
        Destroy(gameObject);
    }
}
