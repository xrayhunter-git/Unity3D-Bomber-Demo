using UnityEngine;
using System.Collections;

public class CommandCenter : MonoBehaviour
{
    public Transform attackingStartingPos;
    public Transform defendingStartingPos;
    public FlankingRoute[] flankingRoutes;
    public Transform[] suppressingPositions;
    public float visualizationRadius = 0.5f;
    public int[] targetTeams;
    public int[] commandTeams;
    public float checkInterval = 1.0f;
    public float targetAreaRadius = 20.0f;
    public float commandAreaRadius = 50.0f;
    bool showGizmos = true;
    float timeTillNextCheck = 1.0f;
    bool currentlyCommanding = false;
    CaptainScript currentCaptain;


    public bool visualizeInfluenceSpheres = true;

    // Update is called once per frame
    void Update ()
    {
        timeTillNextCheck -= Time.deltaTime;
        if(timeTillNextCheck < 0)
        {
            if(!currentlyCommanding)
            {
                
                TryToStartCommand();
            }
            else
            {
                TryToEndCommand();
            }
            timeTillNextCheck = checkInterval;
        }
    }

    void TryToEndCommand()
    {
         if(!TacticalAI.ControllerScript.currentController.TargetOnTeamsInRadius(targetTeams, targetAreaRadius, defendingStartingPos.position))
            {
                currentlyCommanding = false;
                currentCaptain.EndOrders();
            }
    }

    void TryToStartCommand()
    {
        if(TacticalAI.ControllerScript.currentController.TargetOnTeamsInRadius(targetTeams, targetAreaRadius, defendingStartingPos.position))
        {
            TacticalAI.Target[] potentialComs = TacticalAI.ControllerScript.currentController.GetCurrentAIsWithinRadius(commandTeams, commandAreaRadius, attackingStartingPos.position);
            for(int i = 0; i < potentialComs.Length; i++)
            {
                CaptainScript c = potentialComs[i].targetScript.gameObject.GetComponent<CaptainScript>();
                if (c)
                {
                    if (c.AskForCaptain(this, flankingRoutes, suppressingPositions))
                    {
                        currentlyCommanding = true;
                        currentCaptain = c;
                        return;
                    }
                }
            }
        }
    }


    void OnDrawGizmos()
    {
        if (showGizmos && defendingStartingPos && attackingStartingPos)
        {
            for (int i = 0; i < suppressingPositions.Length; i++)
            {
                if (suppressingPositions[i])
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(suppressingPositions[i].position, visualizationRadius);
                }
            }

            if (visualizeInfluenceSpheres)
            {
                Gizmos.color = Color.grey;
                Gizmos.DrawWireSphere(transform.position, targetAreaRadius);

                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position, commandAreaRadius);
            }
        }
    }
}
