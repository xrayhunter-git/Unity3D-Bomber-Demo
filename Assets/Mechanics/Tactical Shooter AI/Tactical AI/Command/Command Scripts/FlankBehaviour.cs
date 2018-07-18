using UnityEngine;
using System.Collections;

//Copy and paste this for your custom behaviours.
//Check the CustomAIBehaviour script to see the parent class.

namespace TacticalAI
{
    public class FlankBehaviour : TacticalAI.CustomAIBehaviour
    {
        bool haveANodeTarget = false;
        int currentPatrolIndex = 0;
        float minNodeDistSquared;

        void Awake()
        {
            Initiate();
        }

        public override void Initiate()
        {
            base.Initiate();
        }

        public override void AICycle()
        {
            if (flankPositions.Length >= 0)
            {
                //if we don't have a current goal, find one.
                if (!haveANodeTarget)
                {
                    SetMinNodeDistSquared();
                    targetVector = flankPositions[currentPatrolIndex].position;
                    haveANodeTarget = true;

                    //Move the current patrol node index up and loop it around to the beginning if necessary 
                    currentPatrolIndex++;
                }
                //if we have one, check if we're to close.  If so, cancel the current goal.
                else if (Vector3.SqrMagnitude(targetVector - myTransform.position) < minNodeDistSquared)
                {
                    haveANodeTarget = false;
                    if (currentPatrolIndex >= flankPositions.Length)
                    {
                        KillBehaviour();
                    }
                }
            }
            else
            {
                Debug.LogError("No Flank nodes set!  Please set the array in the inspector");
            }
        }

        void SetMinNodeDistSquared()
        {
            minNodeDistSquared = baseScript.closeEnoughToPatrolNodeDist * baseScript.closeEnoughToPatrolNodeDist;
        }

        Transform[] flankPositions;

        public void SetNodes(Transform[] s)
        {
            flankPositions = s;
        }
    }
}