using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WWIIBuilding
{
    [System.Serializable]
    public class Debre
    {
        public GameObject prefab;
        public List<GameObject> debreEffects;
        public int health;
    }

    [System.Serializable]
    public class StructureInfo
    {
        public GameObject prefab;
        public List<GameObject> debreEffects;
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

	    // Use this for initialization
	    void Start ()
        {
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

                if (displayObject != null && info != null && info.prefab != null)
                {
                    Destroy(displayObject.gameObject);
                    displayObject = Instantiate(info.prefab, this.transform);
                }
                
                if (displayObject != null)
                    displayObject.name = "Display Object";

                lastHealth = health;
            }

	    }

        public void Damage(float damage)
        {
            health -= damage;
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