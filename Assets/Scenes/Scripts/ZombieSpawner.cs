using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;

    public ZombieTypeProb[] zombieTypes;

    private List<ZombieTypes> probList = new List<ZombieTypes>();

    public int zombieMax;

    public int zombiesSpawned;

    public int zombiesAlive;

    public float zombieDelay = 5;

    public Slider progressBar;

    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        zombiesAlive = 0;
        InvokeRepeating("SpawnZombie", 15, zombieDelay);

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
        zombiesAlive++;

        ZombieTypes selectedType = probList[Random.Range(0, probList.Count)];
        int r = Random.Range(0, spawnPoints.Length);
        GameObject myZombie = Instantiate(selectedType.zombiePrefab, spawnPoints[r].position, Quaternion.identity);
        Zombie zombieScript = myZombie.GetComponent<Zombie>();
        zombieScript.type = selectedType;

        //if (zombiesSpawned >= zombieMax)
        //    zombieScript.lastZombie = true;
    }

    public void OnZombieKilled()
    {
        zombiesAlive--;

        if(zombiesAlive <= 0 && zombiesSpawned >= zombieMax)
        {
            gameManager.Win();
        }
    }
}

[System.Serializable]
public class ZombieTypeProb
{
    public ZombieTypes type;

    public int probability;
}