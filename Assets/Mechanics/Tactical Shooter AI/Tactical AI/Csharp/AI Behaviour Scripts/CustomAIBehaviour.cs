using UnityEngine;
using System.Collections;

/*
 * Script that contains several behaviour classes
 * */


//Base behaviour class
namespace TacticalAI
{
    public class CustomAIBehaviour : MonoBehaviour
    {

        [HideInInspector]
        public TacticalAI.BaseScript baseScript = null;
        [HideInInspector]
        public TacticalAI.GunScript gunScript;
        [HideInInspector]
        public TacticalAI.SoundScript soundScript;
        [HideInInspector]
        public TacticalAI.RotateToAimGunScript rotateToAimGunScript;
        [HideInInspector]
        public TacticalAI.AnimationScript animationScript;
        [HideInInspector]
        public TacticalAI.CoverFinderScript coverFinderScript;
        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public NavmeshInterface navI;
        [HideInInspector]
        public LayerMask layerMask;

        public Vector3 targetVector;
        public BehaviourLevel behaveLevel;

        public enum BehaviourLevel
        {
            None = 0, Idle = 1, Combat = 2,
        }

        public void Start()
        {
            ApplyBehaviour();
        }


        public virtual void Initiate()
        {
            if (gameObject.GetComponent<TacticalAI.BaseScript>())
            {
                baseScript = gameObject.GetComponent<TacticalAI.BaseScript>();

                gunScript = baseScript.gunScript;
                soundScript = baseScript.audioScript;
                rotateToAimGunScript = baseScript.headLookScript;
                animationScript = baseScript.animationScript;
                coverFinderScript = baseScript.coverFinderScript;
                myTransform = baseScript.GetTranform();

                layerMask = TacticalAI.ControllerScript.currentController.GetLayerMask();

                navI = baseScript.GetAgent();
            }
        }

        public virtual void AICycle()
        {

        }

        public virtual void EachFrame()
        {
        }

        public void KillBehaviour()
        {
            OnEndBehaviour();
            Destroy(this);
        }

        public virtual void OnEndBehaviour()
        {

        }

        public void ApplyBehaviour()
        {
            if (behaveLevel == BehaviourLevel.Idle)
            {
                Initiate();
                if (baseScript)
                    baseScript.SetIdleBehaviour(this);
            }
            else if (behaveLevel == BehaviourLevel.Combat)
            {
                Initiate();
                if (baseScript)
                    baseScript.SetCombatBehaviour(this);
            }
        }

    }
}


//Run right to the target position.
namespace TacticalAI
{
    public class ChargeTarget : TacticalAI.CustomAIBehaviour
    {


        float minDistToTargetIfNotInCover = 0;

        public override void Initiate()
        {
            base.Initiate();
            minDistToTargetIfNotInCover = baseScript.minDistToTargetIfNotInCover * baseScript.minDistToTargetIfNotInCover;

        }

        public override void AICycle()
        {
            //Run to the key transform if we have one
            if (baseScript.keyTransform)
            {
                /*if (Vector3.SqrMagnitude(myTransform.position - baseScript.keyTransform.position) > minDistToTargetIfNotInCover
                && !Physics.Linecast(baseScript.GetEyePos(), baseScript.keyTransform.position, layerMask))
                {
                    targetVector = baseScript.keyTransform.position;
                }
                else
                {
                    targetVector = myTransform.position;
                }*/
                targetVector = baseScript.keyTransform.position;
            }
            //Otherwise, run at the target we are firing on
            else if (baseScript.targetTransform)
            {
                if (Vector3.SqrMagnitude(myTransform.position - baseScript.targetTransform.position) > minDistToTargetIfNotInCover
                || Physics.Linecast(baseScript.GetEyePos(), baseScript.targetTransform.position, layerMask))
                {
                    targetVector = baseScript.targetTransform.position;
                }
                else
                {
                    targetVector = myTransform.position;
                }
                //targetVector = baseScript.targetTransform.position;
            }
        }
    }
}

//Move to a key transform
namespace TacticalAI
{
    public class MoveToTransform : TacticalAI.CustomAIBehaviour
    {
        public override void Initiate()
        {
            base.Initiate();
        }

        public override void AICycle()
        {
            if (baseScript.keyTransform)
                targetVector = baseScript.keyTransform.position;
        }
    }
}

//Upon being warned of a grenade, move in a random direction in an attempt to escape.
namespace TacticalAI
{
    public class RunAwayFromGrenade : TacticalAI.CustomAIBehaviour
    {
        //Completely random number.
        float timeToGiveUp = 3.0f;

        public override void Initiate()
        {
            base.Initiate();
            GetPositionToMoveTo();
        }

        public override void AICycle()
        {
            if (timeToGiveUp < 0 || baseScript.transformToRunFrom == null || (!navI.PathPending() && navI.GetRemainingDistance() < 1))
            {
                KillBehaviour();
            }
            timeToGiveUp -= Time.deltaTime;
        }

        void GetPositionToMoveTo()
        {
            //Find a random direction to run in.
            //We don't want to move directly away from the genade because otherwise we may end up backing right into a corner.
            //Raycasts to check for corners are too unreliable because of variations in terrain height.
            Vector3 tempVec = baseScript.transformToRunFrom.position;
            tempVec.x += (Random.value - 0.5f) * baseScript.distToRunFromGrenades;
            tempVec.z = (Random.value - 0.5f) * baseScript.distToRunFromGrenades;
            UnityEngine.AI.NavMeshHit hit;
            UnityEngine.AI.NavMesh.SamplePosition(tempVec, out hit, baseScript.distToRunFromGrenades, -1);
        }
    }
}

//Quickly move to the side
namespace TacticalAI
{
    public class Dodge : TacticalAI.CustomAIBehaviour
    {

        public override void Initiate()
        {
            base.Initiate();
            myOrigStoppingDist = navI.GetStoppingDistance();
            origAcceleration = navI.GetAcceleration();
        }

        float myOrigStoppingDist;
        float origAcceleration;

        IEnumerator SetDodge()
        {
            //Prepare the navagent to rapidly change directions.
            navI.SetSpeed(baseScript.dodgingSpeed);
            navI.SetStoppingDistance(0);
            navI.SetAcceleration(10000);
            baseScript.isDodging = true;

            rotateToAimGunScript.Deactivate();
            animationScript.StopRot();

            //Don't move until we have plotted a path.
            while (!navI.HasPath())
            {
                yield return null;
            }

            if (!navI.HaveOffMeshLinkInPath())
            {
                //Debug.Log("YO");
                animationScript.PlayDodgingAnimation(dodgeRight);
                //animationScript.currentlyRotating = false;

                //Debug.Log("Dodging Time");
                yield return new WaitForSeconds(baseScript.dodgingTime);
            }

            //return the navagent to it's original state.
            rotateToAimGunScript.Activate();
            animationScript.StartRot();
            //animationScript.currentlyRotating = true;
            navI.SetAcceleration(origAcceleration);
            navI.SetStoppingDistance(myOrigStoppingDist);
            navI.SetSpeed(baseScript.runSpeed);
            baseScript.isDodging = false;
            //yield return new WaitForSeconds(0.2f);

            KillBehaviour();
        }
        bool dodgeRight;



        bool AquireDodgingTarget()
        {
            Vector3 dodgePos = myTransform.position;


            //Choose whether to dodge left or right.
            if (Random.value < 0.5f)
            {
                //Get a target position.
                dodgePos += animationScript.myAIBodyTransform.right * baseScript.dodgingSpeed * baseScript.dodgingTime;
                dodgePos.y = myTransform.position.y;
                dodgeRight = true;
            }
            else
            {
                //Get a target position.
                dodgePos += -animationScript.myAIBodyTransform.right * baseScript.dodgingSpeed * baseScript.dodgingTime;
                dodgePos.y = myTransform.position.y;
                dodgeRight = false;
            }

            //Make sure there are no walls in the way.
            RaycastHit hit;
            if (!Physics.Linecast(myTransform.position + baseScript.dodgeClearHeightCheckPos, dodgePos + baseScript.dodgeClearHeightCheckPos, out hit, layerMask.value))
            {
                //Debug.Break();
                Debug.DrawLine(myTransform.position + baseScript.dodgeClearHeightCheckPos, hit.point, Color.green);
                targetVector = dodgePos;
                return true;
            }
            else
            {
                Debug.DrawLine(myTransform.position + baseScript.dodgeClearHeightCheckPos, dodgePos, Color.red);
                //Debug.Break();
            }

            return false;
        }

        public override void OnEndBehaviour()
        {
            rotateToAimGunScript.Activate();
            animationScript.StartRot();
            navI.SetAcceleration(origAcceleration);
            navI.SetStoppingDistance(myOrigStoppingDist);
            navI.SetSpeed(baseScript.runSpeed);
            baseScript.isDodging = false;
            Destroy(this);
        }


        public override void AICycle()
        {
            // if we're not dodging, dodge,
            if (!baseScript.isDodging)
            {
                if (AquireDodgingTarget())
                {
                    StartCoroutine(SetDodge());
                }
                else
                {
                    //Debug.Break();
                    KillBehaviour();
                }
            }
        }
    }
}

/*
 * Effectivly a chase behaviour, but the position of the target is only updated once the agent reaches it's first destination
 * */
namespace TacticalAI
{
    public class Search : TacticalAI.CustomAIBehaviour
    {

        float radiusToCallOffSearch;

        public override void Initiate()
        {
            base.Initiate();
            radiusToCallOffSearch = baseScript.radiusToCallOffSearch;
        }

        public override void AICycle()
        {
            //Every time we get close to our goal position, set a new position based on our target's current position.
            if (baseScript.targetTransform && navI.GetRemainingDistance() < radiusToCallOffSearch)
            {
                targetVector = baseScript.targetTransform.position;
            }
            else if (!baseScript.targetTransform)
            {
                targetVector = myTransform.position;
            }
        }
    }
}

/*
 * If the agent is not engaged in combat, move to the position a sound was heard
 * */
namespace TacticalAI
{
    public class InvestigateSound : TacticalAI.CustomAIBehaviour
    {

        float radiusToCallOffSearch;

        public virtual void Inititae()
        {
            base.Initiate();
            radiusToCallOffSearch = baseScript.radiusToCallOffSearch;
        }

        public override void AICycle()
        {
            //End the behaviour if we get close enough to the source, or we engage a target.
            if (navI.GetRemainingDistance() < radiusToCallOffSearch || baseScript.IsEnaging())
            {
                KillBehaviour();
            }
            else
            {
                targetVector = baseScript.lastHeardNoisePos;
            }
        }

        public override void OnEndBehaviour()
        {
            Destroy(this);
        }
    }
}

/*
 * Move along a path defined by the user
 * */
namespace TacticalAI
{
    public class Patrol : TacticalAI.CustomAIBehaviour
    {

        bool haveAPatrolTarget = false;
        int currentPatrolIndex = 0;
        float patrolNodeDistSquared;

        public virtual void Inititae()
        {
            base.Initiate();
        }

        public override void AICycle()
        {
            if (baseScript.patrolNodes.Length >= 0)
            {
                //if we don't have a current goal, find one.
                if (!haveAPatrolTarget)
                {
                    SetPatrolNodeDistSquared();
                    targetVector = baseScript.patrolNodes[currentPatrolIndex].position;
                    haveAPatrolTarget = true;

                    //Move the current patrol node index up and loop it around to the beginning if necessary 
                    currentPatrolIndex++;
                    if (currentPatrolIndex >= baseScript.patrolNodes.Length)
                    {
                        currentPatrolIndex = 0;
                    }
                }
                //if we have one, check if we're to close.  If so, cancel the current goal.
                else if (Vector3.SqrMagnitude(targetVector - myTransform.position) < patrolNodeDistSquared)
                {
                    haveAPatrolTarget = false;
                }
            }
            else
            {
                Debug.LogError("No patrol nodes set!  Please set the array in the inspector, via script, or change the AI's non-engaging behavior");
            }
        }

        void SetPatrolNodeDistSquared()
        {
            patrolNodeDistSquared = baseScript.closeEnoughToPatrolNodeDist * baseScript.closeEnoughToPatrolNodeDist;
        }

    }
}

/*
 * Randomly move around
 * */
namespace TacticalAI
{
    public class Wander : TacticalAI.CustomAIBehaviour
    {

        bool haveCurrentWanderPoint = false;

        public virtual void Inititae()
        {
            base.Initiate();
        }

        public override void EachFrame()
        {
            Debug.DrawLine(myTransform.position, targetVector);
        }

        public override void AICycle()
        {
            if (!haveCurrentWanderPoint)
            {
                //If we have no key transform, randomly choose a goal location within a given radius of agent's current position.
                if (!baseScript.keyTransform)
                    targetVector = FindDestinationWithinRadius(myTransform.position);
                else
                    //If we do have a key transform, randomly choose a goal location within a given radius of our key transform.
                    targetVector = FindDestinationWithinRadius(baseScript.keyTransform.position);

                haveCurrentWanderPoint = true;
            }
            else if (!navI.PathPending() && navI.GetRemainingDistance() < baseScript.GetDistToChooseNewWanderPoint())
            {
                haveCurrentWanderPoint = false;
            }

        }

        public Vector3 FindDestinationWithinRadius(Vector3 originPos)
        {
            //Actually returns destination within a square.
            return new Vector3(originPos.x + (Random.value - 0.5f) * baseScript.GetWanderDiameter(), originPos.y, originPos.z + (Random.value - 0.5f) * baseScript.GetWanderDiameter());
        }
    }
}

/*
 * A complex behaviour that involves the use of cover.  
 * If no cover can be found, the agent will charge directly towards their target
 * */
namespace TacticalAI
{
    public class Cover : TacticalAI.CustomAIBehaviour
    {

        //bool movingTowardsCrouchPos = false;	
        float maxTimeInCover = 10f;
        float minTimeInCover = 5f;
        bool foundDynamicCover = false;
        float minDistToTargetIfNotInCover = 5f;
        float maxTimeTilNoCoverCharge = 3;
        float timeTilNoCoverCharge = 0;

        public override void Initiate()
        {
            base.Initiate();
            maxTimeInCover = baseScript.maxTimeInCover;
            minTimeInCover = baseScript.minTimeInCover;
            minDistToTargetIfNotInCover = baseScript.minDistToTargetIfNotInCover * baseScript.minDistToTargetIfNotInCover;
            timeTilNoCoverCharge = maxTimeTilNoCoverCharge;
        }


        public override void OnEndBehaviour()
        {
            LeaveCover();
        }

        void Update()
        {
            if (!(baseScript.currentCoverNodeScript || foundDynamicCover))
            {
                timeTilNoCoverCharge -= Time.deltaTime;
            }
        }

        //public override void EachFrame()
        public override void AICycle()
        {
            if (coverFinderScript)
            {
                //Choose which part of the cpover node we should move to based on whether we are suppressed and firing.
                if (baseScript.useAdvancedCover  || (!gunScript.IsFiring() || !baseScript.shouldFireFromCover))
                {
                    targetVector = baseScript.currentCoverNodePos;
                }
                else
                {
                    targetVector = baseScript.currentCoverNodeFiringPos;
                }


                if (baseScript.currentCoverNodeScript || foundDynamicCover)
                {
                    //If we can't reach our cover, find a different piece of cover.
                    if (navI.PathPartial())
                    {
                        LeaveCover();
                    }
                    //Start the countdown to leave cover once we reach it.
                    if (!baseScript.inCover && navI.ReachedDestination())
                    {
                        baseScript.inCover = true;
                        StartCoroutine(SetTimeToLeaveCover(Random.Range(minTimeInCover, maxTimeInCover)));
                    }
                }
                else
                {
                    //If we don't have cover, find cover.
                    TacticalAI.CoverData coverData = coverFinderScript.FindCover(baseScript.targetTransform, baseScript.keyTransform);

                    if (coverData.foundCover)
                    {
                        SetCover(coverData.hidingPosition, coverData.firingPosition, coverData.isDynamicCover, coverData.coverNodeScript);
                        //Play vocalization
                        if (soundScript)
                            soundScript.PlayCoverAudio();
                    }
                    //If we can't find cover, charge at our target.
                    else if (baseScript.targetTransform && timeTilNoCoverCharge < 0)
                    {
                        NoCoverFindDest();
                    }
                }
            }
            //If we don't have cover, charge at our target.
            else if (baseScript.targetTransform && timeTilNoCoverCharge < 0)
            {
                NoCoverFindDest();
            }
        }

        void NoCoverFindDest()
        {
            if (Vector3.SqrMagnitude(myTransform.position - baseScript.targetTransform.position) > minDistToTargetIfNotInCover
                || Physics.Linecast(baseScript.GetEyePos(), baseScript.targetTransform.position, layerMask))
            {
                targetVector = baseScript.targetTransform.position;
            }
            else
            {
                targetVector = myTransform.position;
            }
        }



        IEnumerator SetTimeToLeaveCover(float timeToLeave)
        {
            //Count down to leave cover
            while (timeToLeave > 0 && (baseScript.currentCoverNodeScript || foundDynamicCover))
            {
                if (baseScript.inCover)
                    timeToLeave--;
                else
                    timeToLeave -= 0.25f;


                if (baseScript.targetTransform)
                {
                    //Makes the agent leave cover if it is no longer safe.  Uses the cover node's built in methods to check.
                    if (!foundDynamicCover && !baseScript.currentCoverNodeScript.CheckForSafety(baseScript.targetTransform.position))
                    {
                        LeaveCover();
                    }
                    //Makes the agent leave cover if it is no longer safe. 
                    else if (foundDynamicCover && !Physics.Linecast(baseScript.currentCoverNodePos, baseScript.targetTransform.position, layerMask.value))
                    {
                        LeaveCover();
                    }
                }
                yield return new WaitForSeconds(1);
            }
            if (baseScript.currentCoverNodeScript || foundDynamicCover)
            {
                LeaveCover();
            }
        }

        //Called when the agent wants to leave cover.  Sets variables to values appropriate for an agent that is not in cover.
        public void LeaveCover()
        {
            if (baseScript.currentCoverNodeScript)
            {
                baseScript.currentCoverNodeScript.setOccupied(false);
                baseScript.currentCoverNodeScript = null;
            }
            else if (foundDynamicCover)
            {
                TacticalAI.ControllerScript.currentController.RemoveACoverSpot(baseScript.currentCoverNodeFiringPos);
            }

            baseScript.inCover = false;
            baseScript.SetOrigStoppingDistance();

            foundDynamicCover = false;

            if (!baseScript.shouldFireFromCover)
            {
                coverFinderScript.ResetLastCoverPos();
            }

            if(baseScript.useAdvancedCover)
            {
                animationScript.EndAdvancedCover();
            }
        }


        //Used to set variables once cover is found.
        void SetCover(Vector3 newCoverPos, Vector3 newCoverFiringSpot, bool isDynamicCover, TacticalAI.CoverNodeScript newCoverNodeScript)
        {
            timeTilNoCoverCharge = maxTimeTilNoCoverCharge;

            baseScript.currentCoverNodePos = newCoverPos;
            baseScript.currentCoverNodeFiringPos = newCoverFiringSpot;

            navI.SetStoppingDistance(0);

            if (isDynamicCover)
            {
                foundDynamicCover = true;
                TacticalAI.ControllerScript.currentController.AddACoverSpot(baseScript.currentCoverNodeFiringPos);
            }
            else
            {
                baseScript.currentCoverNodeScript = newCoverNodeScript;
                baseScript.currentCoverNodeScript.setOccupied(true);
                if (baseScript.useAdvancedCover)
                {                  
                    animationScript.StartAdvancedCover(baseScript.currentCoverNodeScript.advancedCoverDirection, baseScript.currentCoverNodeScript.faceDir);
                }
            }
        }
    }
}

namespace TacticalAI
{
    public class Skirmish : TacticalAI.CustomAIBehaviour
    {
        public float minDistFromTarget = 7f;
        public float maxDistFromTarget = 20f;
        bool haveADestTarget = false;
        int framesUntilCanReachObject = 0;
        public bool canCrossBehindTarget = true;
        public float maxTimeToWaitAtEachPoint = 3f;
        float timeLeftAtThisPoint;

        public override void Initiate()
        {
            base.Initiate();
            minDistFromTarget = baseScript.minSkirmishDistFromTarget;
            maxDistFromTarget = baseScript.maxSkirmishDistFromTarget;
            canCrossBehindTarget = baseScript.canCrossBehindTarget;
            maxTimeToWaitAtEachPoint = baseScript.maxTimeToWaitAtEachSkirmishPoint;
        }

        public override void AICycle()
        {
            //Debugging Stuff to see where they are going
            if (haveADestTarget)
            {
                Debug.DrawLine(myTransform.position, targetVector, Color.yellow, 0.2f);
            }

            if (!haveADestTarget && timeLeftAtThisPoint <= 0)
            {
                targetVector = GetNewDestTarget(baseScript.targetTransform);
            }
            //Check if we have arrived at the new targetvector
            else if (haveADestTarget && framesUntilCanReachObject < 0 && navI.ReachedDestination() && !baseScript.usingDynamicObject)
            {
                haveADestTarget = false;
                //wait
                timeLeftAtThisPoint = maxTimeToWaitAtEachPoint * Random.value;
            }
            framesUntilCanReachObject--;
            timeLeftAtThisPoint -= baseScript.cycleTime;
        }

        Vector3 GetNewDestTarget(Transform targ)
        {
            //Get random vector
            Vector3 directionVector = new Vector3(Random.value - 0.5f, 0, Random.value - 0.5f);

            //Make the agent choose a position between the agent and the target, if desired.  This will get us a new "circle strafing" effect.
            if (!canCrossBehindTarget && Vector3.Dot(directionVector, targ.position - myTransform.position) > 0)
            {
                directionVector *= -1;
            }
            directionVector = directionVector.normalized;

            //Get a spot within the combat range
            Vector3 returnPos = targ.position + (directionVector * (Random.Range(minDistFromTarget, maxDistFromTarget)));

            if (!returnPos.Equals(Vector3.zero))
            {
                RaycastHit hit;
                //Get closer than the default range if we have to in order to fit, otherwise our agent will not pursue into close quarters
                if (Physics.Linecast(targ.position, returnPos + new Vector3(0, baseScript.dodgingClearHeight, 0), out hit, layerMask.value))
                {
                    framesUntilCanReachObject = 5;
                    haveADestTarget = true;
                    return hit.point;
                }
                else
                {
                    framesUntilCanReachObject = 5;
                    haveADestTarget = true;
                    return returnPos;
                }
            }
            else
                return targ.position;
        }

        public override void OnEndBehaviour()
        {
            haveADestTarget = false;
        }
    }
}

/*
 * This behaviour causes the agent to move to a given position and face in a given direction
 * Once there, the agent will attempt to play an animation and call a method on a given object
 * The behaviour then ends
 * */
namespace TacticalAI
{
    //You can change the class name from BehaviorChildTemplate to anything else.  
    public class DynamicObject : CustomAIBehaviour
    {

        Transform movementTargetTransform;
        string methodToCall;
        string dynamicObjectAnimation;
        float timeToWait;

        //Set up some default stuff
        public void StartDynamicObject(Transform newMovementObjectTransform, string newAnimationToUse, string newMethodToCall, bool requireEngaging, float timeToWaitF)
        {
            movementTargetTransform = newMovementObjectTransform;
            dynamicObjectAnimation = newAnimationToUse;
            navI.SetStoppingDistance(0);
            timeToWait = timeToWaitF;


            baseScript.usingDynamicObject = false;

            methodToCall = newMethodToCall;
        }

        //Start things in motion when we arrive at the object
        void UseDynamicObject()
        {
            navI.SetSpeed(0);
            rotateToAimGunScript.Deactivate();
            if (gunScript)
            {
                gunScript.SetCanCurrentlyFire(false);
            }
            StartCoroutine(animationScript.DynamicObjectAnimation(dynamicObjectAnimation, movementTargetTransform.forward, this, timeToWait));
            baseScript.usingDynamicObject = true;
        }

        //Actually do the thing
        public void AffectDynamicObject()
        {
            movementTargetTransform.gameObject.SendMessage(methodToCall);
        }

        private int framesUntilCanReachObject = 5;

        //Go back to normal
        public void EndDynamicObjectUsage()
        {
            if (baseScript.usingDynamicObject)
            {
                baseScript.SetProperSpeed();
                baseScript.SetOrigStoppingDistance();
                baseScript.usingDynamicObject = false;
                if (gunScript)
                {
                    gunScript.SetCanCurrentlyFire(true);
                }
                movementTargetTransform = null;
            }
            KillBehaviour();
        }

        void MoveToDynamicObject()
        {
            if (movementTargetTransform)
            {
                Debug.DrawLine(myTransform.position, movementTargetTransform.position, Color.green, baseScript.cycleTime);
                //Once we're sure we've reached the object, use it.
                if (framesUntilCanReachObject < 0 && navI.ReachedDestination() && !baseScript.usingDynamicObject)
                {
                    UseDynamicObject();
                }
                else if (!baseScript.usingDynamicObject)
                {
                    targetVector = movementTargetTransform.position;
                }
                framesUntilCanReachObject--;
                //Debug.Break();
            }
            else
                KillBehaviour();

        }

        //Your behavior code goes here
        public override void AICycle()
        {
            MoveToDynamicObject();
        }

        public override void EachFrame()
        {

        }
    }
}

namespace TacticalAI
{
    public class Parkour : TacticalAI.CustomAIBehaviour
    {
        //bool haveCurrentWanderPoint = false;
        TacticalNavLink linkObj;
        bool origRotateToAimGun;
        bool started = false;

        public override void Initiate()
        {
            base.Initiate();
            origRotateToAimGun = rotateToAimGunScript.isEnabled;
            rotateToAimGunScript.Deactivate();
            if (TacticalAI.ControllerScript.currentController.GetClosestParkourLink(transform.position, ref linkObj))
            {
                started = true;
                animationScript.Parkour(linkObj.destTransform.position - linkObj.position, linkObj.animString, linkObj.position);
            }

        }

        public override void EachFrame()
        {
            if (!animationScript.onLink)
            {
                KillBehaviour();
            }
        }

        public override void OnEndBehaviour()
        {
            if (started)
            {
                navI.CompleteOffMeshLink();
                myTransform.position = animationScript.myAIBodyTransform.position;
                myTransform.rotation = animationScript.myAIBodyTransform.rotation;
            }
            baseScript.inParkour = false;
            if (origRotateToAimGun)
            {
                rotateToAimGunScript.Activate();
            }
        }
    }
}

namespace TacticalAI
{
    public class ThrowGrenade : TacticalAI.CustomAIBehaviour
    {

        GameObject currentGrenade;
        public override void Initiate()
        {
            base.Initiate();
            StartCoroutine(TossGrenade());
        }

        IEnumerator TossGrenade()
        {
            //Debug.Break();
            targetVector = myTransform.position;
            navI.DisableAgent();

            rotateToAimGunScript.Deactivate();
            animationScript.StopRot();

            animationScript.PlayGrenadeAnimation();

            //Debug.Break();
            currentGrenade = (GameObject)(Instantiate(baseScript.grenadePrefab, baseScript.grenadeSpawn.position, baseScript.grenadeSpawn.rotation));
            currentGrenade.transform.parent = baseScript.grenadeSpawn;
            yield return new WaitForSeconds(baseScript.grenadeDelay);
            currentGrenade.transform.parent = null;
            currentGrenade.SendMessage("SetTarget", baseScript.targetPos, SendMessageOptions.DontRequireReceiver);
            currentGrenade = null;
            yield return new WaitForSeconds(baseScript.remainingAnimDelay);

            

            navI.EnableAgent();
            rotateToAimGunScript.Activate();
            animationScript.StartRot();
            navI.SetSpeed(baseScript.runSpeed);
            baseScript.isStaggered = false;
            myTransform.position = animationScript.myAIBodyTransform.position;
            KillBehaviour();
        }

        public void OnDestroy()
        {
            OnEndBehaviour();
        }

        public override void OnEndBehaviour()
        {
            if (baseScript)
            {
                baseScript.isStaggered = false;
            }
            if (currentGrenade)
            {
                currentGrenade.transform.parent = null;
                currentGrenade.SendMessage("DropGrenade", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}

namespace TacticalAI
{
    public class Stagger : TacticalAI.CustomAIBehaviour
    {

        public override void Initiate()
        {
            base.Initiate();
            if (CheckIfStagger())
            {
                StartCoroutine(StaggerLoop());
            }
            else
            {
                KillBehaviour();
            }
        }

        IEnumerator StaggerLoop()
        {
            //Debug.Break();
            targetVector = myTransform.position;
            navI.DisableAgent();

            rotateToAimGunScript.Deactivate();
            animationScript.StopRot();

            animationScript.PlayStaggerAnimation();

            yield return new WaitForSeconds(baseScript.staggerTime);

            navI.EnableAgent();
            //return the navagent to it's original state.
            rotateToAimGunScript.Activate();
            animationScript.StartRot();
            //animationScript.currentlyRotating = true;
            navI.SetSpeed(baseScript.runSpeed);
            baseScript.isStaggered = false;
            myTransform.position = animationScript.myAIBodyTransform.position;
            KillBehaviour();
        }
        /*
        IEnumerator SetStagger()
        {
            //Debug.Break();
            float myOrigStoppingDist = navI.GetStoppingDistance();
            float origAcceleration = navI.GetAcceleration();
            //Prepare the navagent to rapidly change directions.
            navI.SetSpeed(baseScript.staggerSpeed);
            navI.SetStoppingDistance(0);
            navI.SetAcceleration(10000);

            rotateToAimGunScript.Deactivate();
            animationScript.StopRot();

            //Don't move until we have plotted a path.
            while (!navI.HasPath())
            {
                yield return null;
            }

            animationScript.PlayStaggerAnimation();

            yield return new WaitForSeconds(baseScript.dodgingTime);

            //return the navagent to it's original state.
            rotateToAimGunScript.Activate();
            animationScript.StartRot();
            //animationScript.currentlyRotating = true;
            navI.SetAcceleration(origAcceleration);
            navI.SetStoppingDistance(myOrigStoppingDist);
            navI.SetSpeed(baseScript.runSpeed);
            baseScript.isStaggered = false;

            KillBehaviour();
        }*/

        bool CheckIfStagger()
        {
            Vector3 staggerPos = myTransform.position;

            //Get a target position.
            staggerPos -= animationScript.myAIBodyTransform.forward * baseScript.minDistRequiredToStagger;
            staggerPos.y = myTransform.position.y;


            //Make sure there are no walls in the way.
            RaycastHit hit;
            if (!Physics.Linecast(myTransform.position, staggerPos, out hit, layerMask.value))
            {
                return true;
            }
            return false;
        }

        public override void OnEndBehaviour()
        {
            baseScript.isStaggered = false;
        }
    }
}