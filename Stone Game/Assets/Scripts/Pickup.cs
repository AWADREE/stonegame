using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    Transform playerHand;

    // Start is called before the first frame update
    void Start()
    {
        playerHand = FindObjectOfType<PlayerHand>().transform;
    }


    private void OnCollisionEnter2D(Collision2D otherColldider)
    {
        if(otherColldider.transform.GetComponent<PlayerStats>())
        {
            GetPickedUp();
        }   
    }

    void GetPickedUp()
    {
        if(transform.GetComponent<Weapon>())
        {   
            //setting playerhand as the parent of this object
            gameObject.transform.SetParent(playerHand);
            //unequip all weapons
            Component[] weapons;
            weapons = transform.parent.GetComponentsInChildren<Weapon>();
            foreach (Weapon weaponTemp in weapons)
            {
                weaponTemp.GetUnequiped();
            }
            //equip this weapon
            transform.GetComponent<Weapon>().GetEquiped();

            //resset pos and rotation
            gameObject.transform.localPosition = new Vector3(0f,0f,0f);
            // gameObject.transform.localRotation = Quaternion.identity;
            Destroy(gameObject.GetComponent<Rigidbody2D>());
            Destroy(gameObject.GetComponent<Collider2D>());

            //Link newWeaponStats to player
            FindObjectOfType<PlayerCombat>().GetStats();
        }
        else if(GetComponent<Coin>())
        {
            //Add increase player currency
            FindObjectOfType<PlayerStats>().IncreaseCurrencyBy(GetComponent<Coin>().GetCoinValue());
            GetComponent<Coin>().DestroyCoin();
        }
        // else if(GetComponent<QuestObject>())

        // {
        //     //add to player quest
        // }
        

    }

}
