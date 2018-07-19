using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xrayhunter.WWIIBomber
{
    public class DroppedBomb : MonoBehaviour
    {
        public GameObject explosion_prefab;

        public float minDamage;
        public float maxDamage;

        public float radiusOfDamage;

        public float effectDeletionDelay;

        public bool destroyOnImpact = true;

        public AudioClip clip;

        private bool exploded = false;

        private GameObject explosionFX;

        private void OnCollisionEnter(Collision collision) // Collision Trigger...
        {
            if (collision != null) // Safety guard...
            {
                if (explosion_prefab != null) // Safety guard to prefab not loading being loaded...
                {
                    if (!exploded)
                    {
                        explosionFX = Instantiate(explosion_prefab, this.transform.position, this.transform.rotation); // Create effect.

                        if (clip != null)
                        {
                            AudioSource source = explosionFX.AddComponent<AudioSource>();
                            source.clip = clip;
                            source.spatialBlend = 1;
                            source.Play();
                        }

                        exploded = true; // Boom...

                        // Damage stuff

                        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, radiusOfDamage);

                        foreach(Collider collider in hitColliders)
                        {
                            if (collider.GetComponent<WWIIBuilding.DamageableStructure>() != null)
                            {
                                WWIIBuilding.DamageableStructure health = collider.GetComponent<WWIIBuilding.DamageableStructure>();

                                health.Damage(Random.Range(minDamage, maxDamage) * (Vector3.Distance(this.transform.position, collider.transform.position) / 50));
                            }

                            // Uncomment this for your project to add the health removal to npcs.
                            /*
                            if (collider.GetComponent<TacticalAI.HealthScript>() != null)
                            {
                                TacticalAI.HealthScript health = collider.GetComponent<TacticalAI.HealthScript>();
                                health.Damage(Random.Range(minDamage, maxDamage) * (Vector3.Distance(this.transform.position, collider.transform.position) / 50));
                            }
                            */
                        }

                        if (destroyOnImpact)
                            Destroy(this.gameObject);
                        else
                        {
                            if (GetComponent<Rigidbody>() != null) // Stops the object from moving.
                                Destroy(this.GetComponent<Rigidbody>());
                        }

                        Destroy(explosionFX, effectDeletionDelay); // Clean up effects that are laying around.
                    }
                }
            }
        }

    }
}