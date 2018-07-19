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
        public Vector3 destruction_offset;
        public float destruction_speed = 5.0f;
        public List<Debre> debreEffects_damaged;
        public List<Debre> debreEffects_destruction;
        public int delaySwitch = 0;
        public int health;
    }

    public class DamageableStructure : MonoBehaviour
    {
        //public List<StructureInfo> structureStages;

        // Temporary until gets fixed...
        public StructureInfo healthyStage;
        public StructureInfo unhealthyStage;

        public bool destroyOnDeath = false;

        public float maxHealth = 100;

        public float health;

        private float lastHealth;

        private GameObject displayObject;

        private Vector3 lastHitpoint;

        private GameObject destroyedObject;
        private StructureInfo lastPrefab;

	    // Use this for initialization
	    void Start ()
        {
            lastPrefab = healthyStage;
            health = maxHealth;
            //if (structureStages != null && structureStages.Count > 0) // Should be the healthiest.
            //displayObject = Instantiate(GrabNextStructure().prefab, this.transform);
            if (healthyStage != null && healthyStage.prefab != null)
                displayObject = Instantiate(healthyStage.prefab, this.transform);

            if (displayObject != null)
                displayObject.name = "Display Object";
	    }
	
	    // Update is called once per frame
	    void Update ()
        {
            if(destroyedObject != null && lastPrefab != null)
            {
                Debug.Log(destroyedObject.transform.position);
                Debug.Log(lastPrefab.destruction_offset);
                destroyedObject.transform.position = Vector3.Lerp(destroyedObject.transform.position, lastPrefab.destruction_offset, Time.deltaTime * lastPrefab.destruction_speed);
            }

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
                    foreach (Debre debre in info.debreEffects_destruction)
                    {
                        GameObject obj = Instantiate(debre.prefab, this.transform.position + debre.offsets, debre.prefab.transform.rotation);
                        obj.transform.parent = this.transform;
                        Destroy(obj, debre.deletionDelay);
                    }
                    destroyedObject = displayObject;
                    lastPrefab = info;
                }

                if (displayObject != null && info != null && info.prefab != null)
                {
                    if (lastPrefab.delaySwitch == 0)
                    {
                        Destroy(displayObject.gameObject);
                    }
                    else
                    {
                        Destroy(displayObject.gameObject, lastPrefab.delaySwitch);
                    }
                    displayObject = Instantiate(info.prefab, this.transform.position + info.offset, info.prefab.transform.rotation);
                }
                
                if (displayObject != null)
                {
                    displayObject.name = "Display Object";
                    displayObject.transform.parent = this.transform;
                }

                if (lastHitpoint != null)
                {
                    foreach (Debre debre in info.debreEffects_damaged)
                    {
                        GameObject obj = Instantiate(debre.prefab, this.lastHitpoint, debre.prefab.transform.rotation);

                        Destroy(obj, debre.deletionDelay);
                    }
                }

                lastHealth = health;
            }

	    }

        public void Damage(float damage, Vector3 hitpoint = new Vector3())
        {
            health -= damage;
            lastHitpoint = hitpoint;
        }

        /*private StructureInfo GrabNextStructure()
        {
            StructureInfo result = structureStages[0];

            structureStages.Sort((x, y) => -x.health.CompareTo(y.health));

            foreach(StructureInfo info in structureStages)
            {
                if(info.health >= health)
                {
                    return info;
                }
            }

            return result;
        }
        */
    }

}