using UnityEngine;
using System.Collections;

/*
 * Adjusts the priority of a TargetScript based on agents of a given team being in close proximity.
 * Used to make agents fire on a target when other agents are near.
 * */

namespace TacticalAI{
public class AdjustPriorityScript : MonoBehaviour {
	
	public float cycleTime = 2;
	private float curCycleTime;
	public int[] teamNumbersToLookFor;
	
	public float radiusToIncreasePriority = 5;
	private float radiusToIncreasePrioritySqr;
	
	public int amountToIncreasePerTargetFound = 2;
	
	private TacticalAI.TargetScript targetScript;
	
	public bool showRadius;
	public bool alwaysShow;
	private int newPriority = -1;
	
	void Awake()
		{
			targetScript = gameObject.GetComponent<TacticalAI.TargetScript>();
			
            //If no teams are provided, then look for agents that are on the same team as the target we are modifying.
			if(teamNumbersToLookFor.Length == 0)
				{
					teamNumbersToLookFor = new int[1];
					teamNumbersToLookFor[0] = targetScript.myTeamID;
				}
			
	        //Square the radius so that we don't have to use costly SqrRoot
			SetRadiusToIncrease(radiusToIncreasePriority);
		}
	
	void Update () 
		{
            //Only change the priotity every few seconds to improve performance.
			curCycleTime -= Time.deltaTime;
			
			if(curCycleTime < 0)
				{
					UpdatePriority();
					curCycleTime = cycleTime;
				}
		}
	
	void UpdatePriority()
		{
            //make the target "invisible"
			targetScript.targetPriority = -1;
		
            //Get all the targets on the specified teams
			Transform[] targetsToCycle = TacticalAI.ControllerScript.currentController.GetCurrentAIsWithIDs(teamNumbersToLookFor);
			Vector3 myPos = transform.position;
			
			newPriority = -1;
			
            //Increase priority for every agent we find that is close enough
			for(int i = 0; i < targetsToCycle.Length; i++)
				{
					if(Vector3.SqrMagnitude(targetsToCycle[i].position - myPos) < radiusToIncreasePrioritySqr)
						{
							newPriority += amountToIncreasePerTargetFound;
						}
				}

            //Update the target priotity
            targetScript.targetPriority = newPriority;
		
            //If our priority is > 0, update the lists so that other agents can see it.
			if(newPriority > 0)
				{
					TacticalAI.ControllerScript.currentController.UpdateAllEnemiesEnemyLists();
				}
		}
		
	void SetRadiusToIncrease(float x)
		{
			radiusToIncreasePrioritySqr = x*x;
		}
	
	
	void OnDrawGizmos()
		{
			if(showRadius)
				{
					if(newPriority > 0 || alwaysShow)
						{
							Gizmos.color = Color.green;				
							Gizmos.DrawWireSphere (transform.position, radiusToIncreasePriority);
						}
				}
		}
}
}
