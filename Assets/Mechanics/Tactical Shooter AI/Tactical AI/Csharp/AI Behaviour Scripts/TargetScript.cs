using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


/*
 * Used by agents to select their targets and determine what targets they can see.
 * Can also be added to non-agent objects to mark them as targets that agents should fire upon.
 * */

namespace TacticalAI
{
    public class TargetScript : MonoBehaviour
    {

        public Transform targetObjectTransform;

        public Transform myLOSTarget;
        public float targetPriority = 1;
        public TacticalAI.BaseScript myAIBaseScript;
        public GameObject healthScriptHolder;

        private int myUniqueID;

        //Target location stuff
        public int myTeamID;
        public int[] alliedTeamsIDs;
        public int[] enemyTeamsIDs;


        //TargetStuff
        private List<TacticalAI.Target> listOfCurrentlyNoticedTargets = new List<TacticalAI.Target>();

        //Every x second, if we're not in cover, we'll check for LoS between all known targets.  If we can't find any, the agent will return to it's idle behavior
        public int timeBeforeTargetExpiration = 15;

        private List<int> targetIDs = new List<int>();

        //private Transform[] allyTransforms;
        private TacticalAI.Target[] enemyTargets = null;

        public float timeBetweenTargetChecksIfEngaging = 7;
        public float timeBetweenTargetChecksIfNotEngaging = 12;

        //public bool willEverLoseAwareness = false;

        public float timeBetweenLOSChecks = 0.5f;

        //public bool shouldUseLOSTargets = true;

        private bool engaging = false;

        public TacticalAI.Target currentEnemyTarget;

        public float shoutDist = 50;

        public float timeBetweenReactingToSounds = 15;
        private bool shouldReactToNewSound = true;

        public float maxLineOfSightChecksPerFrame = 3;

        public Transform eyeTransform;

        private float effectiveFOV;
        public float myFieldOfView = 130;
        public bool debugFieldOfView;

        [HideInInspector]
        public LayerMask layerMask;

        public bool canAcceptDynamicObjectRequests = false;


		public float maxDistToNoticeTarget = 9999f;

        // Use this for initialization
        void Awake()
        {
            if (!healthScriptHolder)
                healthScriptHolder = gameObject;

            layerMask = TacticalAI.ControllerScript.currentController.GetLayerMask();

            if (!targetObjectTransform)
                targetObjectTransform = transform;

            if (!myLOSTarget)
                myLOSTarget = targetObjectTransform;


            //Add ourselves to the list of targets
            if (TacticalAI.ControllerScript.currentController)
                myUniqueID = TacticalAI.ControllerScript.currentController.AddTarget(myTeamID, targetObjectTransform, this);
            else
                UnityEngine.Debug.LogWarning("No AI Controller Found!");


            if (!eyeTransform)
                eyeTransform = targetObjectTransform;


            effectiveFOV = myFieldOfView / 2;
            maxDistToNoticeTarget = maxDistToNoticeTarget*maxDistToNoticeTarget;

            if (myAIBaseScript)
            {
                myAIBaseScript.SetTargetObj(this);
            }
        }

        public void SetNewTeam(int newTeam)
        {
            TacticalAI.ControllerScript.currentController.ChangeAgentsTeam(myUniqueID, newTeam);
            myTeamID = newTeam;
        }

        void Start()
        {
            //Start some loops
            if (myAIBaseScript)
            {
                StartCoroutine(LoSLoop());
                StartCoroutine(TargetSelectionLoop());
                //StartCoroutine(CountDownToTargetExperation());
            }
        }

        IEnumerator LoSLoop()
        {
            //Check to see if we can see enemy targets every x seconds
            yield return new WaitForSeconds(Random.value);

            while (myAIBaseScript.isCurrentlyActive())
            {
                CheckForLOSAwareness(false);
                yield return new WaitForSeconds(timeBetweenLOSChecks);
            }
        }

        IEnumerator TargetSelectionLoop()
        {
            //Pick which target we will fire at and take cover from.
            //Update our target more frequently if we are engaging
            yield return new WaitForSeconds(Random.value);
            while (myAIBaseScript.isCurrentlyActive())
            {
                if (engaging)
                    yield return new WaitForSeconds(timeBetweenTargetChecksIfEngaging);
                else
                    yield return new WaitForSeconds(timeBetweenTargetChecksIfNotEngaging);

                ChooseTarget();
            }
        }

        void OnDestroy()
        {
            RemoveThisTargetFromPLay();
        }

        public void RemoveThisTargetFromPLay()
        {
            if (TacticalAI.ControllerScript.currentController != null && isPlaying)
                TacticalAI.ControllerScript.currentController.RemoveTargetFromTargetList(myUniqueID);
        }

        //Stop errors when you quit the game early
        bool isPlaying = true;
        void OnApplicationQuit()
        {
            isPlaying = false;
        }


        //Update the local lists of allies and enemies.
        public void UpdateEnemyAndAllyLists(TacticalAI.Target[] a, TacticalAI.Target[] e)
        {
            if (myAIBaseScript)
            {
                //allyTransforms = a;
                enemyTargets = e;

                //If we don't have any targets left, exit the engaging state
                if (enemyTargets.Length == 0)
                {
                    myAIBaseScript.EndEngage();
                    engaging = false;
                }

                TacticalAI.Target[] lastTargets = listOfCurrentlyNoticedTargets.ToArray();
                listOfCurrentlyNoticedTargets = new List<TacticalAI.Target>();
                Vector3[] lastlastKnownTargetPositions = lastKnownTargetPositions.ToArray();
                lastKnownTargetPositions = new List<Vector3>();

                //Put all targets that still exist in new list
                for (int i = 0; i < lastTargets.Length; i++)
                {
                    for (int x = 0; x < enemyTargets.Length; x++)
                    {
                        if (lastTargets[i].uniqueIdentifier == enemyTargets[x].uniqueIdentifier)
                        {
                            listOfCurrentlyNoticedTargets.Add(enemyTargets[x]);
                            lastKnownTargetPositions.Add(lastlastKnownTargetPositions[i]);
                            break;
                        }
                    }
                }

                //Check to see if we can see any targets.  If engaging, we aren't limited by what direction we are looking in.
                if (engaging)
                {
                    CheckForLOSAwareness(true);
                }
                else
                {
                    CheckForLOSAwareness(false);
                }

                ChooseTarget();
            }
        }

        //If a target is noticed, it is prioritized
        void NoticeATarget(TacticalAI.Target newTarget)
        {
            int IDToAdd = newTarget.uniqueIdentifier;

            //Make sure we haven't seen this target already
            for (int i = 0; i < targetIDs.Count; i++)
            {
                if (targetIDs[i] == IDToAdd)
                {
                    return;
                }
            }

            lastKnownTargetPositions.Add(newTarget.transform.position);
            listOfCurrentlyNoticedTargets.Add(newTarget);
            targetIDs.Add(IDToAdd);

            ChooseTarget();

            //If we aren't already engaging in combat, start engaging.
            if (!engaging)
            {
                myAIBaseScript.StartEngage();
                engaging = true;
            }
        }

        //Old target loss method
        /*IEnumerator CountDownToTargetExperation()
        {
            bool foundATargetWeAreStillEngaging = false;

            //In some situations you may never want the agent to lose track of targets
            while (myAIBaseScript.enabled && willEverLoseAwareness)
            {
                foundATargetWeAreStillEngaging = false;
                //Because cover is supposed to block line of sight, we don't count it against an agent if they can't see their target while in cover
                if (listOfCurrentlyNoticedTargets.Count > 0 && !myAIBaseScript.inCover)
                {
                    for (int i = 0; i < listOfCurrentlyNoticedTargets.Count; i++)
                    {
                        if (!Physics.Linecast(eyeTransform.position, listOfCurrentlyNoticedTargets[i].transform.position, layerMask))
                        {
                            foundATargetWeAreStillEngaging = true;
                            i = listOfCurrentlyNoticedTargets.Count;
                        }

                        //For performance, we'll only check one target per frame
                        yield return null;
                    }
                    if (!foundATargetWeAreStillEngaging)
                    {
                        myAIBaseScript.EndEngage();
                        engaging = false;
                        listOfCurrentlyNoticedTargets = new List<TacticalAI.Target>();
                        targetIDs = new List<int>();
                        ChooseTarget();
                    }
                }

                //TODO:  Check after we leave cover, instead of waiting for the next cycle.

                yield return new WaitForSeconds(timeBeforeTargetExpiration);
            }
        }*/

        //REMEMBER TO COMMENT THIS
        //Summery, in case I forget: The agent will lose track of targets that move far enough away from their last known position
        private List<Vector3> lastKnownTargetPositions = new List<Vector3>();
        public float distToLoseAwareness = 35f;

        void CheckIfWeStillHaveAwareness()
        {
            Transform enemyTransformCheckingNow;
            int i = 0;

            for (i = 0; i < listOfCurrentlyNoticedTargets.Count; i++)
            {               
                enemyTransformCheckingNow = listOfCurrentlyNoticedTargets[i].transform;
                if (eyeTransform && enemyTransformCheckingNow && !Physics.Linecast(eyeTransform.position, enemyTransformCheckingNow.position, layerMask))
                {
                    lastKnownTargetPositions[i] = enemyTransformCheckingNow.position;
                }
                else if (enemyTransformCheckingNow && Vector3.Distance(enemyTransformCheckingNow.position, lastKnownTargetPositions[i]) > distToLoseAwareness)
                {
                    listOfCurrentlyNoticedTargets.RemoveAt(i);
                    lastKnownTargetPositions.RemoveAt(i);
                    i -= 1;
                }
            }
            if (listOfCurrentlyNoticedTargets.Count == 0)
            {
                myAIBaseScript.EndEngage();
                engaging = false;
                listOfCurrentlyNoticedTargets = new List<TacticalAI.Target>();
                lastKnownTargetPositions = new List<Vector3>();
                targetIDs = new List<int>();
            }
        }

        void ChooseTarget()
        {
            if (eyeTransform)
            {
                float currentEnemyScore = 0;
                float enemyScoreCheckingNow = 0;
                Transform enemyTransformCheckingNow = eyeTransform;
                currentEnemyTarget = null;
                bool foundTargetWithLoS = false;
                int i = 0;

                CheckIfWeStillHaveAwareness();

                for (i = 0; i < listOfCurrentlyNoticedTargets.Count; i++)
                {
                    if (listOfCurrentlyNoticedTargets[i].transform)
                    {
                        enemyTransformCheckingNow = listOfCurrentlyNoticedTargets[i].transform;

                        //Only add points if we have LoS
                        if (!Physics.Linecast(eyeTransform.position, enemyTransformCheckingNow.position, layerMask))
                        {
                            //Get initial score based on distance
                            enemyScoreCheckingNow = Vector3.SqrMagnitude(enemyTransformCheckingNow.position - targetObjectTransform.position);
                            //enemyScoreCheckingNow = Vector3.Distance(enemyTransformCheckingNow.position, targetObjectTransform.position);

                            //Divide by priority
                            enemyScoreCheckingNow = enemyScoreCheckingNow / (listOfCurrentlyNoticedTargets[i].targetScript.GetComponent<TacticalAI.TargetScript>().targetPriority);

                            //See if this score is low enough to warrent changing target
                            if (enemyScoreCheckingNow < currentEnemyScore || currentEnemyScore == 0 || !foundTargetWithLoS)
                            {
                                currentEnemyTarget = listOfCurrentlyNoticedTargets[i];
                                currentEnemyScore = enemyScoreCheckingNow;
                                foundTargetWithLoS = true;
                            }
                        }
                        //Settle for targets we can't see, if we have to.
                        else if (!foundTargetWithLoS)
                        {
                            enemyScoreCheckingNow = Vector3.SqrMagnitude(enemyTransformCheckingNow.position - targetObjectTransform.position);
                            if (enemyScoreCheckingNow < currentEnemyScore || currentEnemyScore < 0 || !foundTargetWithLoS)
                            {
                                currentEnemyTarget = listOfCurrentlyNoticedTargets[i];
                                currentEnemyScore = enemyScoreCheckingNow;
                            }
                        }
                    }
                }
                                                    
               	if(currentEnemyTarget != null)
                {	
                    AlertAlliesOfEnemy_Shout();
                }    
                
                //If all of the above fails, pick a random target- even if it's one we haven't seen
                if (currentEnemyTarget == null && enemyTargets.Length > 0)
                {
                    currentEnemyTarget = enemyTargets[Random.Range(0, enemyTargets.Length - 1)];
                }
            
				if(currentEnemyTarget != null)
                {	
                    myAIBaseScript.SetMyTarget(currentEnemyTarget.transform, currentEnemyTarget.targetScript.myLOSTarget);
                }
                if (currentEnemyTarget == null)
                
                {
                    myAIBaseScript.RemoveMyTarget();
                }
            }
        }

        public void AlertAlliesOfEnemy_Shout()
        {
            //Wait for a frame to let everything initialize if an enemy is spooted on the first frame
            if (currentEnemyTarget != null && currentEnemyTarget.transform)
            {
                //This "sound" is only to alert other agents of the enemy's location, NOT to produce a sound audible by the player.
                TacticalAI.ControllerScript.currentController.CreateSound(currentEnemyTarget.transform.position, shoutDist, alliedTeamsIDs);
            }
        }

        public void HearSound(Vector3 soundPos)
        {
            if (myAIBaseScript && shouldReactToNewSound && !myAIBaseScript.IsEnaging())
            {
                //   StackTrace st = new StackTrace();
                //    UnityEngine.Debug.Log(st.GetFrame(1).GetMethod().Name);
                //    UnityEngine.Debug.Break();

             //   UnityEngine.Debug.Log("Lolol");
               CheckForLOSAwareness(true);
               myAIBaseScript.StartCoroutine("HearSound", soundPos);
               myAIBaseScript.SetAlertSpeed();
               StartCoroutine(SetTimeUntilNextSound());
            }
        }

        //Don't react to sounds in too quick succession, or we may end up with an agent who is paralyzed by all the sounds, unable to move in the direction of any one.
        IEnumerator SetTimeUntilNextSound()
        {
            shouldReactToNewSound = false;
            yield return new WaitForSeconds(timeBetweenReactingToSounds);
            shouldReactToNewSound = true;
        }

        //Check to see if the agent can see the target
        public void CheckForLOSAwareness(bool shouldCheck360Degrees)
        {
            if (enemyTargets != null)
            {
                for (int i = 0; i < enemyTargets.Length; i++)
                {
                    //Debug
                    if (debugFieldOfView)
                        {
                            UnityEngine.Debug.DrawRay(eyeTransform.position, eyeTransform.forward * 20, Color.green, timeBetweenLOSChecks);
                            Vector3 tarVec = Quaternion.AngleAxis(effectiveFOV, Vector3.up) * eyeTransform.forward;
                            UnityEngine.Debug.DrawRay(eyeTransform.position, tarVec * 20, Color.green, timeBetweenLOSChecks);
                            tarVec = Quaternion.AngleAxis(-effectiveFOV, Vector3.up) * eyeTransform.forward;
                            UnityEngine.Debug.DrawRay(eyeTransform.position, tarVec * 20, Color.green, timeBetweenLOSChecks);
                        }

                    //Check for line of sight	
					//Sometimes we may not want to restrict the agent's senses to their field of view.	
                    //Stupid checks to make sure we still have the transforms because Unity can't pass a function telling us that a scene is about to be loaded
                    if (eyeTransform && enemyTargets[i].transform && (shouldCheck360Degrees || Vector3.Angle(eyeTransform.forward, enemyTargets[i].transform.position - eyeTransform.position) < effectiveFOV) && Vector3.SqrMagnitude(eyeTransform.position - enemyTargets[i].transform.position) < maxDistToNoticeTarget)
                    {
                    	//(Vector3.Angle(eyeTransform.forward, enemyTargets[i].transform.position - eyeTransform.position));
                    	//print(shouldCheck360Degrees);
                        if ( !Physics.Linecast(eyeTransform.position, enemyTargets[i].transform.position, layerMask))
                        {
                            NoticeATarget(enemyTargets[i]);
                        }
                    }
                }
            }
        }

        //Used to make this agent move out to the way of a grenade
        public void WarnOfGrenade(Transform t, float d)
        {
            if (myAIBaseScript && myAIBaseScript.inCover)
                {
                    myAIBaseScript.WarnOfGrenade(t,d);
                }
        }

        //Pass on the dynamic object paramaters to other scripts
        public bool UseDynamicObject(Transform newMovementObjectTransform, string newAnimationToUse, string methodToCall, bool requireEngaging)
        {
            return UseDynamicObject(newMovementObjectTransform, newAnimationToUse, methodToCall, requireEngaging, 1.0f);
        }

        //Pass on the dynamic object paramaters to other scripts
        public bool UseDynamicObject(Transform newMovementObjectTransform, string newAnimationToUse, string methodToCall, bool requireEngaging, float timeToWait)
        {
            if (canAcceptDynamicObjectRequests && myAIBaseScript.SetDynamicObject(newMovementObjectTransform, newAnimationToUse, methodToCall, requireEngaging, timeToWait))
            {
                return true;
            }
            return false;
        }

        //Getters
        public int GetUniqueID()
        {
            return myUniqueID;
        }

        public int[] GetEnemyTeamIDs()
        {
            return enemyTeamsIDs;
        }

        public void ApplyDamage(float h)
        {
            //Uncomment for RFPS
            /*
            if (healthScriptHolder.GetComponent<FPSPlayer>())
            {
                healthScriptHolder.GetComponent<FPSPlayer>().ApplyDamage(h);
            }*/

            healthScriptHolder.SendMessage("Damage", h, SendMessageOptions.DontRequireReceiver);
        }

    }
}
