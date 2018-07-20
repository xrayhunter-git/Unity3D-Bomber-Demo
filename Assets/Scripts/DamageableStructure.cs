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
        public int delayDeletion = 0;
        public int health;
        public AudioClip[] damagedClips;
        public AudioClip[] destroyedClips;
    }

    [RequireComponent(typeof(AudioSource))]
    public class DamageableStructure : MonoBehaviour
    {
        public List<StructureInfo> structureStages;

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
            foreach (Transform trans in GetComponentsInChildren<Transform>())
            {
                if (trans != null && trans.gameObject != this.gameObject)
                    DestroyImmediate(trans.gameObject);
            }

            UpdateStructure(true);

            health = maxHealth;
            if (GetComponents<AudioSource>().Length > 0)
                sourceMain = GetComponents<AudioSource>()[0];
            else
                sourceMain = this.gameObject.AddComponent<AudioSource>();

            if (GetComponents<AudioSource>().Length > 1)
                sourceEffects = GetComponents<AudioSource>()[1];
            else
                sourceEffects = this.gameObject.AddComponent<AudioSource>();

            sourceMain.spatialBlend = 1;
            sourceEffects.spatialBlend = 1;
        }
	
	    // Update is called once per frame
	    void Update ()
        {
            if (health != lastHealth) // Stop the spam.
            {
                UpdateStructure();

                lastHealth = health;
            }

	    }

        
        public void Damage(float damage, Vector3 hitpoint = new Vector3())
        {
            health -= damage;
            lastHitpoint = hitpoint;
        }

        private void UpdateStructure(bool start = false)
        {

            StructureInfo info = GrabNextStructure();

            if (info != null)
            {
                if (info != lastPrefab)
                {
                    if (!start)
                    {

                        foreach (Debre debre in info.debreEffects_destruction)
                        {
                            GameObject obj = Instantiate(debre.prefab, this.transform.position + debre.offsets, Quaternion.AngleAxis(0, Vector3.up));
                            obj.transform.parent = this.transform;
                            Destroy(obj, debre.deletionDelay);
                        }
                    }

                    StructureInfo previousInfo = lastPrefab;
                    lastPrefab = info;

                    if (displayObject != null)
                    {
                        if (previousInfo.delayDeletion == 0)
                        {
                            Destroy(displayObject);
                        }
                        else
                        {

                            destroyedObject = displayObject;
                            StartCoroutine(DoDestroyAnimation(previousInfo.waitDestructionAnimation, destroyedObject, previousInfo));
                            Destroy(destroyedObject, previousInfo.delayDeletion);
                        }
                    }

                    if (info != null && info.prefab != null)
                    {
                        displayObject = Instantiate(info.prefab, this.transform.position + info.offset, transform.rotation * info.prefab.transform.rotation);
                        displayObject.name = "Display Object";
                        displayObject.transform.parent = this.transform;
                    }
                }

                if (lastHitpoint != (new Vector3()))
                {
                    AudioClip clip = GetRandomClip(info.damagedClips);
                    if (clip != null)
                    {
                        sourceEffects.clip = clip;
                        sourceEffects.transform.position = lastHitpoint;
                        if (sourceEffects.clip != null && !sourceEffects.isPlaying)
                            sourceEffects.Play();
                    }

                    foreach (Debre debre in info.debreEffects_damaged)
                    {
                        GameObject obj = Instantiate(debre.prefab, this.lastHitpoint, Quaternion.AngleAxis(0, Vector3.up));

                        Destroy(obj, debre.deletionDelay);
                    }

                    lastHitpoint = new Vector3();
                }
            }
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

            AudioClip clip = GetRandomClip(lastInformation.destroyedClips);
            if (clip != null)
            {
                if (lastInformation.debreEffects_destruction.Count > 0)
                    sourceMain.transform.localPosition = lastInformation.debreEffects_destruction[0].offsets;

                sourceMain.clip = clip;
                sourceMain.pitch = lastInformation.destruction_speed - 1;
                if (sourceMain.clip != null && !sourceMain.isPlaying)
                    sourceMain.Play();
            }

            if (lastObject != null && lastInformation != null)
            {
                while (lastObject.transform.position.y > lastInformation.destruction_offset.y)
                {
                    if (lastObject == null && lastInformation == null) break;

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
            if (structureStages == null || structureStages.Count == 0) return null;

            StructureInfo result = structureStages[0];
            
            for(int i = structureStages.Count - 1; i > 0; i--)
            {
                if(structureStages[i].health >= health)
                {
                    result = structureStages[i];
                    break;
                }
            }

            return result;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;

            foreach(StructureInfo info in structureStages)
            {
                foreach(Debre debre in info.debreEffects_destruction)
                {
                    Gizmos.DrawCube(debre.offsets + this.transform.position, new Vector3(1, 1, 1));
                }
            }

        }
    }

}