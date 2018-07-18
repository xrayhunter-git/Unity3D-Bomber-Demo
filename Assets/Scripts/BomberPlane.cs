using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace WWIIBomber
{
    [System.Serializable]
    public class BombData
    {
        public GameObject prefab;
        public GameObject explosion_prefab;
        public AudioClip audio;

        [Range(0, 100)]
        public int chanceOfSpawn;
        
        public float minDamage = 50.0f;
        public float maxDamage = 100.0f;

        public float radiusOfDamage = 20.0f;
    }

    [RequireComponent(typeof(GameObject))]
    public class BomberPlane : MonoBehaviour
    {
        public int paddingTime = 5;

        public float speedMin = 5.0f;
        public float speedMax = 10.0f;
        public float rotationSpeedMin = 0.01f;
        public float rotationSpeedMax = 0.1f;

        public bool AIBomber = true;
        public bool AIBombBayEnabled = true;

        public List<Transform> dropPayloads;

        public List<BombData> bombs;

        public bool unlimitedAmmo = true;

        public float bombYOffset = 5.0f;

        public float bombSpacing = 2.0f;

        public Vector3[] waypoints;

        public bool destroyAtFinalWaypoint = false;

        public float bombReleaseDelay = 0.5f;

        private float bombReleaseCounter = 0.0f;

        private int waypointCounter = 0;

        private int bombDropCounter = 0;

        private float speed = 5.0f;

        void Start()
        {
            if (dropPayloads == null && dropPayloads.Count == 0)
            {
                GameObject dropPayloadObj = new GameObject();
                dropPayloadObj.transform.parent = this.transform;

                dropPayloads.Add(dropPayloadObj.transform);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (AIBombBayEnabled)
            {
                if (bombs.Count != 0)
                {
                    bombReleaseCounter -= Time.deltaTime;

                    foreach (Transform dropPayload in dropPayloads)
                    {
                        if (Vector3.Distance(this.transform.position, dropPayload.position) <= paddingTime + 10 + bombs.Count)
                        {
                            // Start dropping payload.
                            if (bombReleaseCounter <= 0)
                            {
                                BombData bombData = GetNextBomb(); // Gets a random next bomb.
                                GameObject bomb = Instantiate(bombData.prefab, this.transform.position + new Vector3(bombSpacing * (bombDropCounter % 2 == 0 ? -1 : 1), -this.bombYOffset, Random.Range(0, 2)), Quaternion.Euler(90.0f, this.transform.rotation.y, this.transform.rotation.z));

                                DroppedBomb bombInfo = bomb.GetComponent<DroppedBomb>();
                                bombInfo.explosion_prefab = bombData.explosion_prefab;
                                bombInfo.clip = bombData.audio;
                                bombInfo.minDamage = bombData.minDamage;
                                bombInfo.maxDamage = bombData.maxDamage;
                                bombInfo.radiusOfDamage = bombData.radiusOfDamage;

                                bombReleaseCounter = bombReleaseDelay;

                                bombDropCounter++;
                                if (bombDropCounter > 5)
                                    bombDropCounter = 0;

                                speed = Mathf.Lerp(speed, speedMin, Time.deltaTime);

                                Destroy(bomb, 60); // Despawn after 60 seconds to clear memory.
                            }
                        }
                    }
                }
            }

            if (AIBomber)
            {
                if (waypoints != null && waypoints.Length > 0)
                {
                    if (waypointCounter < waypoints.Length && waypoints[waypointCounter] != null)
                    {
                        if (bombReleaseCounter <= -paddingTime)
                        {
                            speed = Mathf.Lerp(speed, speedMax, Time.deltaTime);

                            float distance = Vector3.Distance(this.transform.position, waypoints[waypointCounter]);

                            float rotationSpeed = Mathf.Clamp(distance * rotationSpeedMin, rotationSpeedMin, rotationSpeedMax);

                            Quaternion rotation = Quaternion.LookRotation(waypoints[waypointCounter] - transform.position);

                            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
                        }

                        speed = Mathf.Clamp(speed, speedMin, speedMax);

                        transform.Translate(Vector3.forward * Time.deltaTime * speed);

                        if (Vector3.Distance(this.transform.position, waypoints[waypointCounter]) <= 10)
                        {
                            waypointCounter++;
                        }
                    }
                    else
                    {
                        waypointCounter = 0;
                        if (destroyAtFinalWaypoint)
                            DestroyImmediate(this.gameObject);
                    }
                }
            }
        }

        private BombData GetNextBomb()
        {
            int chance = Random.Range(0, 100);
            BombData bomb = bombs[0];

            // Selects the higher chance bombs first.
            for (int i = 0; i < bombs.Count; i++)
            {
                if (bombs[i].chanceOfSpawn < chance)
                {
                    bomb = bombs[i];
                    break;
                }
            }

            if (!unlimitedAmmo)
            {
                bombs.Remove(bomb);
            }

            return bomb;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;

            int bombAmount = 0;
            if (bombs != null)
                bombAmount = bombs.Count;

            if (dropPayloads != null)
            {
                foreach (Transform dropPayload in dropPayloads)
                    Gizmos.DrawWireSphere(dropPayload.transform.position, paddingTime + 10 + bombAmount);
            }

            Gizmos.color = Color.yellow;

            foreach (Vector3 waypoint in waypoints)
            {
                Gizmos.DrawWireCube(waypoint, new Vector3(1, 1, 1));
            }

            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(this.transform.position + new Vector3(0, -bombYOffset, 0), new Vector3(bombSpacing * 2, 0, 2));
        }
    }
}