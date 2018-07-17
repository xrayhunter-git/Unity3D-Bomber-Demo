using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class BombData
{
    public GameObject prefab;
    public GameObject explosion_prefab;
    public AudioClip audio;

    [Range(0, 100)]
    public int chanceOfSpawn;
}

[RequireComponent(typeof(GameObject))]
public class BomberPlane : MonoBehaviour
{
    public int paddingTime = 5;

    public float speed = 5.0f;

    public Transform dropPayload;

    public BombData[] bombs;

    public bool losesBombs = true;

    public Vector3[] waypoints;
    
    public bool destroyAtFinalWaypoint = false;

    public float bombReleaseDelay = 0.5f;

    private float bombReleaseCounter = 0.0f;

    private int waypointCounter = 0;
    
    void Start()
    {
        if (dropPayload == null)
        {
            GameObject dropPayloadObj = new GameObject();
            dropPayloadObj.transform.parent = this.transform;

            dropPayload = dropPayloadObj.transform;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            if (waypointCounter < waypoints.Length && waypoints[waypointCounter] != null)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, waypoints[waypointCounter], Time.deltaTime * speed);
                if (Vector3.Distance(this.transform.position, waypoints[waypointCounter]) <= 5)
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

        if (bombs.Length == 0) return;

        bombReleaseCounter -= Time.deltaTime;
        if (Vector3.Distance(this.transform.position, dropPayload.position) <= paddingTime + 10 + bombs.Length)
        {
            // Start dropping payload.
            if (bombReleaseCounter <= 0)
            {
                BombData bombData = GetNextBomb(); // Gets a random next bomb.
                GameObject bomb = Instantiate(bombData.prefab, this.transform.position + new Vector3(Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10)), Quaternion.Euler(90.0f, this.transform.rotation.y, this.transform.rotation.z));

                DroppedBomb bombInfo = bomb.GetComponent<DroppedBomb>();
                bombInfo.explosion_prefab = bombData.explosion_prefab;
                bombInfo.clip = bombData.audio;

                bombReleaseCounter = bombReleaseDelay;

                Destroy(bomb, 60); // Despawn after 60 seconds to clear memory.
            }

        }
	}

    private BombData GetNextBomb()
    {
        int chance = Random.Range(0, 100);
        BombData bomb = bombs[0];

        // Selects the higher chance bombs first.
        for (int i = 0; i < bombs.Length; i++)
        {
            if (bombs[i].chanceOfSpawn < chance)
            {
                bomb = bombs[i];
                break;
            }
        }

        if (losesBombs)
        {
            // moves the bombs array around.
            BombData[] temp = new BombData[bombs.Length - 1];

            int increment = 0;
            for (int i = 0; i < bombs.Length; i++)
            {
                if (bombs[i] != bomb)
                {
                    temp[i] = bombs[increment];
                    increment++;
                }
            }

            bombs = temp;
        }

        return bomb;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        int bombAmount = 0;
        if (bombs != null)
            bombAmount = bombs.Length;

        if (dropPayload != null)
            Gizmos.DrawWireSphere(dropPayload.transform.position, paddingTime + 10 + bombAmount);

        foreach(Vector3 waypoint in waypoints)
        {
            Gizmos.DrawWireCube(waypoint, new Vector3(1, 1, 1));
        }
    }
}
