using UnityEngine;
using System.Collections;

/*
 * Once the agent dies and was killed with enough damage to a given hitbox,
 * it replaces materials on a mesh renderer with a transparent material and instantiates an object at a given point.
 * This can be used to create the illusion of a mesh part being destroyed.
 * Note that the body part's colliders will still linger.
 * */

namespace TacticalAI
{
    public class DismembermentScript : MonoBehaviour
    {

        public bool isEnabled = true;
        public TacticalAI.HitBox myHitBox;
        public float damageMinimum = 1;
        //How the renderer's material array should look after the script is triggered
        public Material[] materialSet;
        public GameObject effect;
        public SkinnedMeshRenderer myRenderer;
        public Transform effectSpawn;
        public bool shouldParentEffectToSpawn;

        
        public void OnAIDeath()
        {
            if (isEnabled && myHitBox.damageTakenThisFrame > damageMinimum)
            {
                //Make the necessary parts of the mesh transparent.
                myRenderer.materials = materialSet;
                if (effect)
                {
                    GameObject nE = (GameObject)Instantiate(effect, effectSpawn.position, effectSpawn.rotation);
                    //If you want the effect to follow the ragdoll as it falls.  
                    if (shouldParentEffectToSpawn)
                    {
                        nE.transform.parent = effectSpawn;
                    }
                }
            }
        }
    }
}


