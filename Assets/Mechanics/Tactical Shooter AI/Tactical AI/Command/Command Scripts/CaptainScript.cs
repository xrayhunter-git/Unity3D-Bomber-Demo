using UnityEngine;
using System.Collections;

public class CaptainScript : MonoBehaviour {

    public FormationFollower[] squadMembers;
    TacticalAI.BaseScript baseScript;
    CommandCenter currentCommandCenter;
    //bool canFlank = false;
    //bool canSuppress = true;
    public bool shouldCancelOrdersOnDeath = false;

	// Use this for initialization
	void Start () {
        baseScript = gameObject.GetComponent<TacticalAI.BaseScript>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool AskForCaptain(CommandCenter c, FlankingRoute[] flankingRoutes, Transform[] suppressingPositions)
    {
        if (baseScript.CanStartCommand())
        {
            DistributeOrders(flankingRoutes, suppressingPositions);
            return true;
        }
        return false;
    }

    public void DistributeOrders (FlankingRoute[] flankingRoutes, Transform[] suppressingPositions)
    {
        int currentFlankers = 0;
        int currentSuppressors = 0;

        for(int i = 0; i < squadMembers.Length; i++)
        {
            if(squadMembers[i])
            {
                if (currentFlankers > currentSuppressors && currentSuppressors < suppressingPositions.Length && squadMembers[i].canSuppress && suppressingPositions.Length > 0)
                {
                    squadMembers[i].Suppress(suppressingPositions[currentSuppressors]);
                    currentSuppressors++;
                }
                else if (squadMembers[i].canFlank && flankingRoutes.Length > 0)
                {
                    squadMembers[i].Flank(flankingRoutes[Random.Range(0, flankingRoutes.Length)].GetRoute());
                    currentFlankers++;
                }
            }
        }
    }

    public void EndOrders()
    {
        for (int i = 0; i < squadMembers.Length; i++)
        {
            if (squadMembers[i])
            {
                squadMembers[i].EndOrders();
            }
        }
    }

    public void OnAIDeath()
    {
        if(shouldCancelOrdersOnDeath)
        {
            EndOrders();
        }
    }
}
