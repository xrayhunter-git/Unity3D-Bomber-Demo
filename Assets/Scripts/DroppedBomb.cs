using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedBomb : MonoBehaviour
{
    public GameObject explosion_prefab;

    public bool destroyOnImpact = true;

    public AudioClip clip;

    private bool exploded = false;
    
    private void OnCollisionEnter(Collision collision) // Collision Trigger...
    {
        if (collision != null) // Safety guard...
        {
            if (explosion_prefab != null) // Safety guard to prefab not loading being loaded...
            {
                if (!exploded)
                {
                    GameObject explosionFX = Instantiate(explosion_prefab, this.transform.position, this.transform.rotation); // Create effect.
                    
                    if (clip != null)
                    {
                        AudioSource source = explosionFX.AddComponent<AudioSource>();
                        source.clip = clip;
                        source.spatialBlend = 1; // 3D!!!
                        source.Play();
                    }

                    exploded = true; // Boom...

                    if (destroyOnImpact)
                        Destroy(this.gameObject);
                    else
                    {
                        if (GetComponent<Rigidbody>() != null) // Stops the object from moving.
                            Destroy(this.GetComponent<Rigidbody>());
                    }

                    Destroy(explosionFX, 60); // Clean up effects that are laying around.
                }
            }
        }
    }
    
}
