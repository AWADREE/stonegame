using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    // Start is called before the first frame update

    Enemy myEnemy;
    [SerializeField] Enemy enemyPrefab;
    [SerializeField] float respawnTime;
    float timSinceEnemyDied;
    bool myEnemyIsDead;
    void Start()
    {
        myEnemy = GetComponentInChildren<Enemy>();
        gameObject.name = myEnemy.name+"Spawn Point";
    }

    // Update is called once per frame
    void Update()
    {
        if(myEnemyIsDead)
        {
            timSinceEnemyDied+= Time.deltaTime;
        }

        if(timSinceEnemyDied>= respawnTime)
        {
            //Respawn enemy
            Instantiate(enemyPrefab,transform.position,Quaternion.identity).transform.SetParent(gameObject.transform);
            myEnemyIsDead = false;
            timSinceEnemyDied= 0;
            myEnemy = GetComponentInChildren<Enemy>();
        }
    }

    public void EnemyIsDead()
    {
        myEnemyIsDead = true;
    }
}
