using UnityEngine;
using System.Collections;

public class FormationFollower : MonoBehaviour {

    public bool canAcceptCommands = true;
    public bool canFlank = true;
    public bool canSuppress = true;
    TacticalAI.BaseScript baseScript;

    TacticalAI.FlankBehaviour flankBehave = null;
    TacticalAI.SuppressBehaviour suppressBehave = null;

    // Use this for initialization
    void Start () {
        baseScript = gameObject.GetComponent<TacticalAI.BaseScript>();
    }
	
	// Update is called once per frame
	public void Flank (Transform[] t)
    {
        flankBehave = (TacticalAI.FlankBehaviour)transform.gameObject.AddComponent(typeof(TacticalAI.FlankBehaviour));
        baseScript.SetTacticalBehaviour(flankBehave);
        flankBehave.SetNodes(t);
    }

    public void Suppress (Transform t)
    {
        suppressBehave = (TacticalAI.SuppressBehaviour)transform.gameObject.AddComponent(typeof(TacticalAI.SuppressBehaviour));
        baseScript.SetTacticalBehaviour(suppressBehave);
        suppressBehave.SetTransformPos(t);
    }

    public void EndOrders()
    {
        if(flankBehave)
        {
            flankBehave.KillBehaviour();
        }
        if (suppressBehave)
        {
            suppressBehave.KillBehaviour();
        }
    }
}
