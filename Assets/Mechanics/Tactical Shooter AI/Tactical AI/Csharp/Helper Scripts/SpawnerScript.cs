using UnityEngine;
using System.Collections;

namespace TacticalAI
{
    public class SpawnerScript : MonoBehaviour
    {
        public Wave[] waves;
        public Transform[] spawnPoints;
        public int currentWave = 0;
        public int enemiesLeft = 0;

        public bool spawnerActive = true;
        float timeTilNextWave = 0;
        public float timeBeforeFirstWave = 1;
        public float timeBetweenWaves = 3;

        public float spawnPointDiameter = 10.0f;

        void Start()
        {
            timeTilNextWave = timeBeforeFirstWave;
        }

        void Update()
        {
            if (spawnerActive && timeTilNextWave < 0 && enemiesLeft <= 0 && currentWave < waves.Length)
            {
                StartWave();
            }

            timeTilNextWave -= Time.deltaTime;
        }

        void StartWave()
        {
            for(int i = 0; i < waves[currentWave].agents.Length; i++)
            {
                Transform spawnPointNow = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Vector3 positionNow = spawnPointNow.position;
                positionNow.x += (Random.value - 0.5f) * spawnPointDiameter;
                positionNow.z += (Random.value - 0.5f) * spawnPointDiameter;

                GameObject prefab = (GameObject)GameObject.Instantiate(waves[currentWave].agents[i], positionNow, spawnPointNow.rotation);
                prefab.SendMessage("SetWaveSpawner", this);
                enemiesLeft++;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                Gizmos.DrawWireSphere(spawnPoints[i].position, spawnPointDiameter/2.0f);
            }
        }

        void EndWave()
        {
            currentWave++;
            timeTilNextWave = timeBetweenWaves;
        }

        public void AgentDied()
        {
            enemiesLeft--;
            if(enemiesLeft <= 0)
            {
                EndWave();
            }
        }
    }
}

namespace TacticalAI
{
    [System.Serializable]
    public class Wave
    {
        public GameObject[] agents;
    }
}