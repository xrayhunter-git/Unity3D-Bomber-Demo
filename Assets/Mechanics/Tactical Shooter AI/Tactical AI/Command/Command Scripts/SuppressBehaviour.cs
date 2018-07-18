using UnityEngine;
using System.Collections;

//Copy and paste this for your custom behaviours.
//Check the CustomAIBehaviour script to see the parent class.

namespace TacticalAI
{
    public class SuppressBehaviour : TacticalAI.CustomAIBehaviour
    {
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
            
        }

        public override void EachFrame()
        {

        }

        public override void OnEndBehaviour()
        {

        }

        Transform suppressPos;

        public void SetTransformPos(Transform s)
        {
            suppressPos = s;
            targetVector = suppressPos.position;
        }
    }
}