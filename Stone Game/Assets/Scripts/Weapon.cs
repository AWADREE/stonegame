using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    //referances
    Text expText;
    Text levelText;
    Slider expSlider;
    
    //basic stats
    [SerializeField] int weaponId;
    [SerializeField] string weaponName;
    [SerializeField] bool melee =true;
    [SerializeField] float baseDmg =50f;
    [SerializeField] float baseRng = 50f;
    [SerializeField] float baseSpeed =5f;
    [SerializeField] float windUpTime= 2f;
    [SerializeField] float abillityWindUpTime= 2f;
    [SerializeField] float abillityRecoveryTime= 2f;
    [SerializeField] int abillityBaseDamage= 100;
    [SerializeField] float abillityBaseRange;
    [SerializeField] int abillityCost;
    [SerializeField] bool oneHanded =true;
    [SerializeField] bool twoHanded =true;
    [SerializeField] bool sheath =true;
    [SerializeField] float level =1f;

    //other
    [SerializeField] bool equiped; //serialized for debugging
    bool uIConnected = false;
    [SerializeField] float exp =0f;         //current exp in this lvl
    [SerializeField] float expNeeded =1000f; //exp needed to level up to next level
    [SerializeField] float expNeededIncreasePerLevel = 1.25f;
    [SerializeField] float remaingExp;      //for debuging exp waitinf to be used
    [SerializeField] float nextLevelExp;    //for debuging
    Transform playerHand;
    SpriteRenderer[] renderers;

    PlayerStats playerStats;

    private void Awake() 
    {
        renderers = GetComponentInChildren<SpriteRenderer>().GetComponentsInChildren<SpriteRenderer>();
        playerStats = FindObjectOfType<PlayerStats>();
        playerHand = FindObjectOfType<PlayerHand>().transform;
        // if(GetComponentInParent<PlayerHand>() != null)
        // {
        //     equiped = true;
        // }
        //if the index of this object is the same as the index of the selected weapon then this weapon is equiped
        if(playerStats.GetEquipedWeaponId() == weaponId)
        {
            equiped = true;
        }
        else
        {
            equiped = false;
        }
    }

    private void Update() 
    {
        if(equiped)
        {
            ConnectUI();
            //updating UI
            expSlider.value = calculateExp();
            UpdateProfileText();

            if(exp < 0)
            {
                exp =0;
            }
        }
    }

    void UpdateProfileText()
    {
        expText.text = ((expSlider.value*100).ToString("F2")+"%");
        levelText.text = level.ToString();
    }

    float calculateExp()
    {
        return exp/ expNeeded;
    }

    void levelup ()
    {
        level++;
        baseDmg +=20f;
        exp = 0f;
        expNeeded *= expNeededIncreasePerLevel;
        sendWeaponStats();
    }

    public void GetExp(float expReward)
    {
        if(expNeeded - exp > expReward)
        {
            exp +=expReward;
            nextLevelExp =0f;
        }
        else if(expNeeded - exp <= expReward)
        {

            nextLevelExp = expReward - (expNeeded- exp);
            levelup();
            GetRestOfExp(nextLevelExp);
            //get exp
        }
    }


    void GetRestOfExp(float expReward)
    {
        if(expReward < expNeeded)
        {
            exp+=expReward;
            nextLevelExp =0f;
        }
        else if(expReward >= expNeeded)
        {
            nextLevelExp = expReward - (expNeeded-exp);
            levelup();
            GetRestOfExp(nextLevelExp);
            // nextLevelExp = expNeeded*2f;
        }
    }

    void ConnectUI()
    {

        if (uIConnected)
        {
            return;
        }
        else
        { 
            expSlider = GameObject.Find("EXP Slider").GetComponent<Slider>();
            expText = GameObject.Find("EXPText").GetComponent<Text>();
            levelText = GameObject.Find("LevelText").GetComponent<Text>();

            uIConnected = true;
        }

    }

    public void GetPickedUp()
    {
        //setting playerhand as the parent of this object
        gameObject.transform.SetParent(playerHand);
        //unequip all weapons
        Component[] weapons;
        weapons = transform.parent.GetComponentsInChildren<Weapon>();
        foreach (Weapon weaponTemp in weapons)
        {
            if(weaponTemp.IsEquiped())
            {
                weaponTemp.GetUnequiped();
            }
        }
        //equip this weapon
        GetEquiped();

        //resset pos and rotation
        gameObject.transform.localPosition = new Vector3(0f,0f,0f);
        // gameObject.transform.localRotation = Quaternion.identity;
        gameObject.transform.localEulerAngles = new Vector3(0f,0f,-309f);
        Destroy(gameObject.GetComponent<Rigidbody2D>());
        Destroy(gameObject.GetComponent<Collider2D>());

        //Link newWeaponStats to player
        sendWeaponStats();
    }

    void sendWeaponStats()
    {
        playerStats.SetBaseDamage(baseDmg); 
        playerStats.SetRange(baseRng); 
        playerStats.SetatkSpeed(baseSpeed); 
        playerStats.SetWindUpTime(windUpTime); 
        playerStats.SetWeaponId(weaponId); 
        playerStats.SetAbillityCost(abillityCost); 
        playerStats.SetAbillityWindUpTime(abillityWindUpTime); 
        playerStats.SetAbillityRecoveryTime(abillityRecoveryTime); 
        playerStats.SetAbillityBaseDamage(abillityBaseDamage); 
        playerStats.SetAbillityBaseRange(abillityBaseRange); 
    }
    public bool IsEquiped()
    {
        return equiped;
    }

    public void GetEquiped()
    {
        equiped = true;
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.enabled = true;
        }
        sendWeaponStats();
    }

    public void GetUnequiped()
    {
        equiped = false;
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.enabled = false;
        }
    }

    // public float GetSpeed()
    // {
    //     return baseSpeed;
    // }

    // public float GetRng()
    // {
    //     return baseRng;
    // }

    // public float GetDamage()
    // {
    //     //return calculated damge, its just the baseDmg for now
    //     return baseDmg;
    // }

    public bool IsMelee()
    {
        return melee;
    }
    
    public bool IsOneHanded()
    {
        return oneHanded;
    }
    
    public bool IsTwoHanded()
    {
        return twoHanded;
    }
    
    public bool HaveSheath()
    {
        return sheath;
    }

}
