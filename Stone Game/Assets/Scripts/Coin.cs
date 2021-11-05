using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] int value;
   

    public int GetCoinValue()
    {
        return value;
    }
    
    public void DestroyCoin()
    {
        Destroy(gameObject);
    }


}
