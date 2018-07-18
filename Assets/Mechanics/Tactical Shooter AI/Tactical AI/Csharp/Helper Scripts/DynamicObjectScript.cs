using UnityEngine;
using System.Collections;

/*
 * Used to demonstrate the dynamic object system
 * This script will find agents of a given team, and attempt to make them use the dynamic object
 * */

namespace TacticalAI
{
    public class DynamicObjectScript : MonoBehaviour
    {

        public int[] teamsToAlert;
        public float lookForTeamsRadius = 20;
        public float timeBetweenChecks = 1;

        public Transform dynamicObjectTransform;
        public string dynamicObjectAnimationClipKey = "DynamicObject";
        public string dynamicObjectMethod = "UseDynamicObject";

        public bool requireEngaging = true;

        public bool currentlyEnabled = true;

        public bool showRadius = true;

        [Range(0.0f, 1.0f)]
        public float oddsToLookEachCycle = 0.1f;

        public float timeToWait = 1.0f;

        void Start()
        {
            if (dynamicObjectTransform)
            {
                StartCoroutine(Cycle());
            }
            else
            {
                Debug.LogWarning("No Dynamic Object Transform found!  Please assign it in the inspector!");
            }
        }

        IEnumerator Cycle()
        {
            yield return new WaitForSeconds(timeBetweenChecks);

            while (this.enabled)
            {
                //Don't look every cycle.  This helps add some randombess
                if (currentlyEnabled && Random.value < oddsToLookEachCycle)
                {
                    //Find the agents on a given team.  All we need from them is their target scripts
                    TacticalAI.Target[] currentListOfTargets = TacticalAI.ControllerScript.currentController.GetCurrentAIsWithinRadius(teamsToAlert, lookForTeamsRadius, dynamicObjectTransform.position);
                    for (int i = 0; i < currentListOfTargets.Length; i++)
                    {
                        //Query the agent to see whether ot not it will use the object.
                        if (currentListOfTargets[i].targetScript.UseDynamicObject(dynamicObjectTransform, dynamicObjectAnimationClipKey, dynamicObjectMethod, requireEngaging, timeToWait))
                        {
                            //If a suitable agent is found, don't look any more.
                            DisableDynamicObject();
                            break;
                        }
                    }
                }

                //Only check every so often to improve performace
                yield return new WaitForSeconds(timeBetweenChecks);
            }
        }

        void EnableDynamicObject()
        {
            currentlyEnabled = true;
        }

        void DisableDynamicObject()
        {
            currentlyEnabled = false;
        }

        void OnDrawGizmos()
        {
            if (showRadius && dynamicObjectTransform)
            {
                Gizmos.color = Color.grey;
                Gizmos.DrawWireSphere(dynamicObjectTransform.position, lookForTeamsRadius);
            }
        }
    }
}
