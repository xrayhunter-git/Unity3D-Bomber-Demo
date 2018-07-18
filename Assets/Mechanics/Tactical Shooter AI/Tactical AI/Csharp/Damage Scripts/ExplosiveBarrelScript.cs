using UnityEngine;
using System.Collections;

/*
 * When the object this script is attached to is damaged enough, it is destroyed 
 * and another object (usually an explosion) is created in it's place.
 * */

namespace TacticalAI
{
    public class ExplosiveBarrelScript : MonoBehaviour
    {
        public float health = 50;
        public GameObject explosion;
        private bool exploded = false;

        //Two damage methods for Paragon AI.
        void Damage(float damage)
        {
            health -= damage;
            if (health < 0 && !exploded)
                Detonate();
        }

        void SingleHitBoxDamage(float damage)
        {
            health -= damage;
            if (health < 0 && !exploded)
                Detonate();
        }

        void Detonate()
        {
            //Need to keep track if it's exploded so that the explosion we create doesn't loop around and make the barrel try and "explode" again.
            //If we didn't have this, we'd end up in a perpetual loop or explosions creating more explosions.
            exploded = true;

            //Create the explosion effect.
            if (explosion)
                Instantiate(explosion, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}