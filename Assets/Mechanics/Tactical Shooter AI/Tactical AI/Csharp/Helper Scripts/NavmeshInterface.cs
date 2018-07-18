using UnityEngine;
using System.Collections;
using System.Diagnostics;



/*interface INavmeshInterface
{	    
    void SetDestination();	
    bool ReachedDestination(Vector3 v);
    bool PathPartial();
    bool PathPending();
    Vector3[] GetNavmeshVertices();	
	
    void SetSpeed(float f);
    float GetSpeed();  
    void SetAcceleration(float f);
    float GetAcceleration();  
    void SetStoppingDistance(float f);
    float GetStoppingDistance();   
    float GetRemainingDistance();  
}*/


//public class NavmeshInterface : IEquatable<INavmeshInterface> 
namespace TacticalAI{
public class NavmeshInterface : MonoBehaviour
{
	    UnityEngine.AI.NavMeshAgent agent;
        Vector3 lastPos;
        Vector3 returnVel;
        Transform myTransform;
	
	    public virtual void Initialize(GameObject gameObject)
	    {
            myTransform = gameObject.GetComponent<AnimationScript>().myAIBodyTransform;
            myTransform = transform;
            lastPos = myTransform.position;

            if (gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>() != null){
			    agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
		    }
		    else
		    {
			    UnityEngine.Debug.Log("No Agent Found!");
		    }
	    }

        void Update()
        {
            if (Time.timeScale > 0.0f)
            {
                returnVel = (myTransform.position - lastPos) / Time.deltaTime;
                lastPos = myTransform.position;
            }
        }
	
        public virtual void SetDestination(Vector3 v){
                if(agent.enabled)
                    agent.SetDestination(v);
                //Debug.DrawLine(transform.position, v);
                //Debug.Break();
        }
    
        public virtual bool ReachedDestination(){
                return (agent.enabled && agent.remainingDistance != Mathf.Infinity /*&& agent.pathStatus == NavMeshPathStatus.PathComplete*/ && agent.remainingDistance <= 0 && !agent.pathPending);
        }
    
        public virtual bool PathPartial(){
    	    return (agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial);
        }
    
        public virtual Vector3 GetDesiredVelocity()
        {
            //return agent.desiredVelocity;
            //return valToReturn;
            return returnVel;
        }
    
        public virtual bool PathPending(){
    	    return agent.pathPending;
        }
    
        public virtual bool HasPath(){
    	    return agent.hasPath;
        }
    
        public virtual Vector3[] GetNavmeshVertices(){
    	    return UnityEngine.AI.NavMesh.CalculateTriangulation().vertices;
        }
        public virtual void SetSpeed(float f){
    	    agent.speed = f;
        }
    
        public virtual float GetSpeed(){
    	    return agent.speed;
        }
    
        public virtual void SetAcceleration(float f){
    	    agent.acceleration = f;
        }
        public virtual float GetAcceleration(){
    	    return agent.acceleration;
        }
        public virtual void SetStoppingDistance(float f){
    	    agent.stoppingDistance = f;
        }
    
        public virtual float GetStoppingDistance(){
    	    return agent.stoppingDistance;
        }
    
 
        public virtual float GetRemainingDistance(){
            if (agent.enabled)
                return agent.remainingDistance;
            return 0;
        }

        //Are we on a navmesh link? (for parkour)
        public virtual bool OnNavmeshLink()
        {
            return agent.isOnOffMeshLink;
        }

        //Instantly move the object to the end of the link 
        public virtual void CompleteOffMeshLink()
        {
                agent.CompleteOffMeshLink();
        }

        //Turn off the agent component (for stagger) 
        public virtual void DisableAgent()
        {
                agent.enabled = false;
        }

        //Turn on the agent (after stagger)
        public virtual void EnableAgent()
        {
                agent.enabled = true;
        }


        //Check if we have a parkour in the plotted path.  (If so, don't dodge because otherwise the agent breaks.)
        public virtual bool HaveOffMeshLinkInPath()
        {
                UnityEngine.AI.OffMeshLinkData linkData = agent.nextOffMeshLinkData;
                //print(linkData.endPos);
                //print(linkData.activated);
                //print(linkData.offMeshLink);
                //print(linkData.valid);
                return linkData.valid;
        }

    }
}
