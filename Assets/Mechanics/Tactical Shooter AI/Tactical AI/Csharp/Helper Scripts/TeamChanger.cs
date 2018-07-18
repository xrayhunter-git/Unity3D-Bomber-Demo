using UnityEngine;
using System.Collections;


namespace TacticalAI
{
    public class TeamChanger : MonoBehaviour
    {

        //Sample Script which waits a certain amount of time and then changes the target's team. 
        public TargetScript targetScriptToChange;
        public float timeToWait = 5.0f;
        public int newTeam = 10;

        void Update()
        {
            timeToWait -= Time.deltaTime;

            if (timeToWait < 0)
                {
                    targetScriptToChange.SetNewTeam(newTeam);
                    this.enabled = false;
                }
        }
    }
}
