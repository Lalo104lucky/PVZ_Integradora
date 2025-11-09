using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;

    public GameObject zombie;

    public ZombieTypeProb[] zombieTypes;

    private List<ZombieTypes> probList = new List<ZombieTypes>();

    public int zombieMax;

    public int zombiesSpawned;

    public float zombieDelay = 5;

    public Slider progressBar;

    private void Start()
    {
        InvokeRepeating("SpawnZombie", 10, zombieDelay);

        foreach(ZombieTypeProb zom in zombieTypes)
        {
            for (int i = 0; i < zom.probability; i++)
            {
                probList.Add(zom.type);
            }
        }

        progressBar.maxValue = zombieMax;
    }

    private void Update()
    {
        progressBar.value = zombiesSpawned;
    }

    void SpawnZombie() {
        if (zombiesSpawned >= zombieMax)
            return;
        zombiesSpawned++;
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