using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    private float spawnRange = 9.0f;
    public int enemyCount;
    public int waveNumber = 1;
    public GameObject[] powerupPrefabs;

    public GameObject bossPrefab;
    public GameObject[] miniEnemyPrefabs;
    public int bossRound;

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemyWave(waveNumber);
        int randomPowerup = Random.Range(0,powerupPrefabs.Length);
        Instantiate(powerupPrefabs[randomPowerup], GenerateSpawnPosition(), powerupPrefabs[randomPowerup].transform.rotation);
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            int randomEnemy = Random.Range(0, enemyPrefabs.Length);

            Instantiate(enemyPrefabs[randomEnemy], GenerateSpawnPosition(), enemyPrefabs[randomEnemy].transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;
        if (enemyCount == 0)
        {
            waveNumber++;
            SpawnEnemyWave(waveNumber);
            int randomPowerup = Random.Range(0, powerupPrefabs.Length);
            Instantiate(powerupPrefabs[randomPowerup], GenerateSpawnPosition(), powerupPrefabs[randomPowerup].transform.rotation);

            if (enemyCount == 0)
            {
                SpawnBossWave(waveNumber);
            }
            else
            {
                SpawnEnemyWave(waveNumber);
            }
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(spawnRange, spawnRange);
        float spawnPosZ = Random.Range(spawnRange, spawnRange);

        Vector3 RandomPos = new Vector3(spawnPosX, 0, spawnPosZ);

        return RandomPos;
    }

    void SpawnBossWave(int currentRound)
    {
        int miniEnemysToSpawn; 
        //We dont want to divide by 0!
        if (bossRound != 0) 
        {
            miniEnemysToSpawn = currentRound / bossRound; 
        } 
        else
        { 
            miniEnemysToSpawn = 1; } var boss = Instantiate(bossPrefab, GenerateSpawnPosition(), bossPrefab.transform.rotation); 
            boss.GetComponent<Enemy>().miniEnemySpawnCount = miniEnemysToSpawn; 
        }
    }
