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
        public List<StructureInfo> structureStages;

        public bool destroyOnDeath = false;

        private float health;

        private float lastHealth;

        private GameObject displayObject;

	    // Use this for initialization
	    void Start ()
        {
            if (structureStages != null && structureStages.Count > 0) // Should be the healthiest.
                displayObject = Instantiate(GrabNextStructure(), this.transform);

            if (displayObject != null)
                displayObject.name = "Display Object";
	    }
	
	    // Update is called once per frame
	    void Update ()
        {
		    if (health != lastHealth) // Stop the spam.
            {
                GameObject newDisplayObject = GrabNextStructure();

                if (newDisplayObject != displayObject)
                {
                    Destroy(displayObject.gameObject);
                    displayObject = Instantiate(newDisplayObject, this.transform);
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

        private GameObject GrabNextStructure()
        {
            GameObject result = structureStages[0].prefab;

            for (int i = 0; i < structureStages.Count; i++)
            {
                if (structureStages[i].health < health)
                {
                    return structureStages[i].prefab;
                }
            }

            return result;
        }
    }

}