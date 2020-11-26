using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MP_SpawnEnemies : NetworkBehaviour
{
    [SerializeField] private GameObject enemyPrefab;  // [] enemyPrefabs
    [SerializeField] private Transform[] enemyPositions;
    [SerializeField] private Transform[] edges;

    private GameObject enemy_clone;
    private MP_Enemy mp_Enemy;


    public override void OnStartServer()
    {
        for (int index = 0; index < enemyPositions.Length; index++)
        {
            enemy_clone = (GameObject)Instantiate(enemyPrefab, enemyPositions[index].position, Quaternion.identity);
            mp_Enemy = enemy_clone.GetComponent<MP_Enemy>();
            mp_Enemy.index = index;
            mp_Enemy.edge_right = edges[index * 2];
            mp_Enemy.edge_left = edges[index * 2 + 1];
            NetworkServer.Spawn(enemy_clone);
        }
    }

    // Update is called once per frame
    //void Update() { }
}