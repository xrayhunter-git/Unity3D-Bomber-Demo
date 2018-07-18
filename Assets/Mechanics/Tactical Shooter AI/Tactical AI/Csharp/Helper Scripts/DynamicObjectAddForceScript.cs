using UnityEngine;
using System.Collections;

/*
 * Cample script to demonstrate the dynamic object system
 * Used to allow an agent to knoeck something over, or throw it into the air
 * */

namespace TacticalAI
{
    public class DynamicObjectAddForceScript : MonoBehaviour
    {

        public Rigidbody rigidBodyToAddForceTo;
        public float forceToAdd = 100;

        public bool shouldResetKinematic = false;
        public float timeUntilIsKinematicAgain = 2;

        public Vector3 relativeVectorToAddForceIn = Vector3.forward;
        public bool showVector;

        public TacticalAI.CoverNodeScript coverNodeToActivate;

        void Awake()
        {
            //If the agent is knocking something over to make cover, we don't want the cover node to be used until the cover is created.
            if (coverNodeToActivate)
                coverNodeToActivate.transform.parent = null;
        }

        public IEnumerator UseDynamicObject()
        {
            if (rigidBodyToAddForceTo)
            {
                //Add force to the rigid Body
                rigidBodyToAddForceTo.isKinematic = false;
                Vector3 VectorToAddForceIn = GetVectorToAddForceIn() * forceToAdd;
                rigidBodyToAddForceTo.AddForce(VectorToAddForceIn, ForceMode.Impulse);

                //Activate the cover node	
                if (coverNodeToActivate)
                    coverNodeToActivate.ActivateNode(1.0f);

                if (shouldResetKinematic)
                {
                    //Make sure whatever it is doesn't get knocked around afterwards
                    yield return new WaitForSeconds(timeUntilIsKinematicAgain);
                    rigidBodyToAddForceTo.isKinematic = true;
                }
            }
        }

        Vector3 GetVectorToAddForceIn()
        {
            return (transform.forward * relativeVectorToAddForceIn.z +
                    transform.up * relativeVectorToAddForceIn.y +
                    transform.right * relativeVectorToAddForceIn.x);
        }

        void OnDrawGizmos()
        {
            if (showVector)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(rigidBodyToAddForceTo.position, GetVectorToAddForceIn());
            }
        }
    }
}
