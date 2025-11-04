using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;

    public GameObject zombie;

    public ZombieTypeProb[] zombieTypes;

    private List<ZombieTypes> probList = new List<ZombieTypes>();

    private void Start()
    {
        InvokeRepeating("SpawnZombie", 2, 5);

        foreach(ZombieTypeProb zom in zombieTypes)
        {
            for (int i = 0; i < zom.probability; i++)
            {
                probList.Add(zom.type);
            }
        }
    }

    void SpawnZombie() {
        int r = Random.Range(0, spawnPoints.Length);
        GameObject myZombie = Instantiate(zombie, spawnPoints[r].position, Quaternion.identity);
        myZombie.GetComponent<Zombie>().type = probList[Random.Range(0, probList.Count)];
    }
}

[System.Serializable]
public class ZombieTypeProb
{
    public ZombieTypes type;

    public int probability;
}