using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D otherColldider)
    {
        if(otherColldider.transform.GetComponent<PlayerStats>())
        {   if(transform.GetComponent<Weapon>() != null)
            {
                transform.GetComponent<Weapon>().GetPickedUp();
            }

            if(transform.GetComponent<Coin>() != null)
            {
                transform.GetComponent<Coin>().GetPickedUp();
            }
            // if(transform.GetComponent<QuestObject>())
            // {
            //     //add to player quest
            // }
        }   
    }


}
