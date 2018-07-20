using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace xrayhunter.WWIIBomber
{
    [System.Serializable]
    public class BombData
    {
        public GameObject prefab;
        public GameObject explosion_prefab;
        public AudioClip audio;

        [Range(0, 100)]
        public int chanceOfSpawn;

        public float effectDeletionDelay = 5.0f;
        
        public float minDamage = 50.0f;
        public float maxDamage = 100.0f;

        public float radiusOfDamage = 20.0f;
    }

    [RequireComponent(typeof(GameObject), typeof(LineRenderer))]
    public class BomberPlane : MonoBehaviour
    {
        public int paddingTime = 5;

        public float speedMin = 5.0f;
        public float speedMax = 10.0f;
        public float enginePower = 0.05f;
        public float brakingPower = 0.02f;
        public float rotationSpeedMin = 0.01f;
        public float rotationSpeedMax = 0.1f;

        public bool AIBomber = true;
        public bool AIBombBayEnabled = true;

        public float AIMoveOnTimeout = 60;

        public List<Transform> dropPayloads;

        public List<BombData> bombs;

        public bool unlimitedAmmo = true;

        public Vector3 bombBayOffset = new Vector3();

        public float bombSpacing = 2.0f;

        public Vector3[] waypoints;

        public bool destroyAtFinalWaypoint = false;

        public float bombReleaseDelay = 0.5f;

        private float bombReleaseCounter = 0.0f;

        private int waypointCounter = 0;
        private float lastWaypointTime = 0;

        private int bombDropCounter = 0;

        private float speed = 5.0f;

        private float rotationSpeed;


        void Start()
        {
            lastWaypointTime = AIMoveOnTimeout;
            bombReleaseCounter = -paddingTime;
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

                        if (Vector3.Distance(this.transform.position, dropPayload.position) <= speed + bombs.Count + (paddingTime * 4))
                        {
                            speed = Mathf.Lerp(speed, brakingPower + (Mathf.Abs(Vector3.Distance(this.transform.position, dropPayload.position) * 0.05f)), Time.deltaTime);
                            rotationSpeed = Mathf.Lerp(rotationSpeed, brakingPower + ((Mathf.Abs(Vector3.Distance(this.transform.position, dropPayload.position) + speed) * 0.05f)), Time.deltaTime);
                        }
                        else
                        {
                            speed += enginePower;
                        }

                        speed = Mathf.Clamp(speed, speedMin, speedMax);
                        rotationSpeed = Mathf.Clamp(rotationSpeed, rotationSpeedMin, rotationSpeedMax);

                        if (Vector3.Distance(this.transform.position, dropPayload.position) <= paddingTime + bombs.Count + speed)
                        {
                            // Start dropping payload.
                            if (bombReleaseCounter <= 0)
                            {
                                BombData bombData = GetNextBomb(); // Gets a random next bomb.

                                if (bombData.prefab != null)
                                {
                                    GameObject bomb = Instantiate(bombData.prefab, this.transform.position + new Vector3(bombSpacing * (bombDropCounter % 2 == 0 ? -1 : 1), 0, Random.Range(0, 2)) + bombBayOffset, Quaternion.Euler(this.transform.position.x, 45, this.transform.rotation.z));

                                    DroppedBomb bombInfo = bomb.GetComponent<DroppedBomb>();
                                    bombInfo.explosion_prefab = bombData.explosion_prefab;
                                    bombInfo.clip = bombData.audio;
                                    bombInfo.minDamage = bombData.minDamage;
                                    bombInfo.maxDamage = bombData.maxDamage;
                                    bombInfo.radiusOfDamage = bombData.radiusOfDamage;
                                    bombInfo.effectDeletionDelay = bombData.effectDeletionDelay;

                                    bombReleaseCounter = bombReleaseDelay;

                                    bombDropCounter++;
                                    if (bombDropCounter > 5)
                                        bombDropCounter = 0;

                                    Destroy(bomb, 60); // Despawn after 60 seconds to clear memory.
                                }

                            }
                        }
                    }
                }
            }

            if (AIBomber)
            {
                if (waypoints != null && waypoints.Length > 0)
                {
                    if (lastWaypointTime <= 0)
                    {
                        lastWaypointTime = AIMoveOnTimeout;
                        waypointCounter++;
                    }

                    if (waypointCounter < waypoints.Length && waypoints[waypointCounter] != null)
                    {
                        if (bombReleaseCounter <= -paddingTime)
                        {

                            if (Vector3.Distance(this.transform.position, waypoints[waypointCounter]) > (paddingTime * 2) + speed)
                            {
                                this.lastWaypointTime -= Time.deltaTime;
                            }

                            speed = Mathf.Lerp(speed, speedMax, Time.deltaTime);

                            float distance = Vector3.Distance(this.transform.position, waypoints[waypointCounter]);

                            rotationSpeed = Mathf.Clamp(distance * rotationSpeedMin, rotationSpeedMin, rotationSpeedMax);

                            Quaternion rotation = Quaternion.LookRotation(waypoints[waypointCounter] - transform.position);

                            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
                        }

                        transform.Translate(Vector3.forward * Time.deltaTime * speed);


                        if (Vector3.Distance(this.transform.position, waypoints[waypointCounter]) <= paddingTime + speed)
                        {
                            lastWaypointTime = AIMoveOnTimeout;
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
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(dropPayload.transform.position, paddingTime + bombAmount + speed);
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(dropPayload.transform.position, (paddingTime*4) + bombAmount + speed);
                }
            }

            Gizmos.color = Color.yellow;

            foreach (Vector3 waypoint in waypoints)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(waypoint, new Vector3(1, 1, 1));
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(waypoint, (paddingTime * 2) + bombAmount + speed);
            }

            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(this.transform.position + bombBayOffset, new Vector3(bombSpacing * 2, 0, 2));
            
        }
    }
}