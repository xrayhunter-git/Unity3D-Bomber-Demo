using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xrayhunter.WWIIBuilding
{
    [System.Serializable]
    public class Debre
    {
        public GameObject prefab;
        public float deletionDelay = 60.0f;
        public Vector3 offsets;
    }

    [System.Serializable]
    public class StructureInfo
    {
        public GameObject prefab;
        public Vector3 offset;
        public float waitDestructionAnimation;
        public Vector3 destruction_offset;
        public bool deleteWhenReachedOffset;
        public float destruction_speed = 5.0f;
        public List<Debre> debreEffects_damaged;
        public List<Debre> debreEffects_destruction;
        public int delaySwitch = 0;
        public int health;
        public AudioClip[] damagedClips;
        public AudioClip[] destroyedClips;
    }

    [RequireComponent(typeof(AudioSource))]
    public class DamageableStructure : MonoBehaviour
    {
        public List<StructureInfo> structureStages;

        // Temporary until gets fixed...
        public StructureInfo healthyStage;
        public StructureInfo unhealthyStage;

        public bool destroyOnDeath = false;

        public float maxHealth = 100;

        public float health;

        private float lastHealth;

        private GameObject displayObject;

        private Vector3 lastHitpoint;
        
        private StructureInfo lastPrefab;

        private GameObject destroyedObject;

        private AudioSource sourceMain;
        private AudioSource sourceEffects;

        // Use this for initialization
        void Start ()
        {
            health = maxHealth;
            if (GetComponents<AudioSource>().Length > 0)
                sourceMain = GetComponents<AudioSource>()[0];
            else
                sourceMain = this.gameObject.AddComponent<AudioSource>();

            if (GetComponents<AudioSource>().Length > 1)
                sourceEffects = GetComponents<AudioSource>()[1];
            else
                sourceEffects = this.gameObject.AddComponent<AudioSource>();
        }
	
	    // Update is called once per frame
	    void Update ()
        {
		    if (health != lastHealth) // Stop the spam.
            {
                /*StructureInfo newDisplayObject = GrabNextStructure();
                
                Destroy(displayObject.gameObject);
                displayObject = Instantiate(newDisplayObject.prefab, this.transform);
                */

                StructureInfo info = healthyStage;

                if (health <= 50)
                {
                    info = unhealthyStage;
                }

                if (info != lastPrefab)
                {
                    sourceMain.clip = GetRandomClip(info.destroyedClips);
                    if(sourceMain.clip != null && !sourceMain.isPlaying)
                        sourceMain.Play();

                    foreach (Debre debre in info.debreEffects_destruction)
                    {
                        GameObject obj = Instantiate(debre.prefab, this.transform.position + debre.offsets, debre.prefab.transform.rotation);
                        obj.transform.parent = this.transform;
                        Destroy(obj, debre.deletionDelay);
                    }
                    StructureInfo previousInfo = lastPrefab;
                    lastPrefab = info;

                    if (displayObject != null)
                    {
                        if (lastPrefab.delaySwitch == 0)
                        {
                            Destroy(displayObject);
                        }
                        else
                        {
                            destroyedObject = displayObject;
                            StartCoroutine(DoDestroyAnimation(previousInfo.waitDestructionAnimation, destroyedObject, previousInfo));
                            Destroy(displayObject, lastPrefab.delaySwitch);
                        }
                    }

                    if (info != null && info.prefab != null)
                    {
                        displayObject = Instantiate(info.prefab, this.transform.position + info.offset, info.prefab.transform.rotation);
                        displayObject.name = "Display Object";
                        displayObject.transform.parent = this.transform;
                    }
                }
                
                if (lastHitpoint != (new Vector3()))
                {
                    sourceEffects.clip = GetRandomClip(info.damagedClips);
                    if (sourceEffects.clip != null && !sourceEffects.isPlaying)
                        sourceEffects.Play();

                    foreach (Debre debre in info.debreEffects_damaged)
                    {
                        GameObject obj = Instantiate(debre.prefab, this.lastHitpoint, debre.prefab.transform.rotation);

                        Destroy(obj, debre.deletionDelay);
                    }

                    lastHitpoint = new Vector3();
                }

                lastHealth = health;
            }

	    }
        
        public void Damage(float damage, Vector3 hitpoint = new Vector3())
        {
            health -= damage;
            lastHitpoint = hitpoint;
        }

        private AudioClip GetRandomClip(AudioClip[] clips)
        {
            if (clips.Length == 0) return null;
            int rand = Random.Range(0, clips.Length);
            return clips[rand];
        }

        public IEnumerator DoDestroyAnimation(float beforeStartTime, GameObject lastObject, StructureInfo lastInformation)
        {
            if (beforeStartTime != 0)
                yield return new WaitForSeconds(beforeStartTime);

            if (lastObject != null && lastInformation != null)
            {
                while (lastObject.transform.position.y > lastInformation.destruction_offset.y)
                {
                    Vector3 pos = lastObject.transform.position;
                    pos.y -= Time.deltaTime * lastInformation.destruction_speed;
                    lastObject.transform.position = pos;

                    yield return new WaitForEndOfFrame();
                }
            }

            if (lastInformation.deleteWhenReachedOffset && lastObject != null)
                DestroyImmediate(lastObject);
        }

        private StructureInfo GrabNextStructure()
        {
            StructureInfo result = structureStages[0];

            structureStages.Sort((x, y) => -x.health.CompareTo(y.health));

            for(int i = 0; )
            {
                if(info.health >= health)
                {
                    return info;
                }
            }

            return result;
        }
    }

}