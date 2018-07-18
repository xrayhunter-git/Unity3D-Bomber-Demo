using UnityEngine;
using System.Collections;

/*
 * This script manages the animations and rotation of the agent.
 * */

namespace TacticalAI
{
    public class AnimationScript : MonoBehaviour
    {
        //Stuff
        public TacticalAI.BaseScript myBaseScript;
        public Transform myAIBodyTransform;
        public TacticalAI.GunScript gunScript;
        public Animator animator;
        Transform myTransform;
        public TacticalAI.RotateToAimGunScript rotateGunScript;

        //Speed
        float currentVelocityRatio = 0;
        float animationDampTime = 0.1f;
        NavmeshInterface navi;

        //Offset
        //This is required because we de-parent the body from the navmesh agent for 
        //rotation purposes.  We need to make it keep up.
        public Vector3 bodyOffset;

        //Cover
        public float minDistToCrouch = 1;

        //Speeds
        public float maxMovementSpeed = -1f;
        public float animatorSpeed = 1f;
        public float meleeAnimationSpeed = 1f;

        //Animation Hashes
        private int currentAngleHash;
        private int engagingHash;
        private int crouchingHash;
        private int reloadingHash;
        private int meleeHash;
        private int fireHash;
        private int forwardsMoveHash;
        private int sidewaysMoveHash;
        private int sprintingHash;
        private int dodgeRightHash;
        private int dodgeLeftHash;
        //private int leapHash;
        //private int vaultHash;
        private int staggerHash;
        private int grenadeHash;

        private int coverHash;
        private int centerHash;
        private int rightHash;
        private int leftHash;

        //Dynamic objects
        public float maxAngleDeviation = 10;
        Quaternion currRotRequired;
        public bool useCustomRotation = false;

        Vector3 directionToFace;

        //Rotation
        float myAngle;
        [Range(0.0f, 90.0f)]
        public float minAngleToRotateBase = 65;
        Quaternion newRotation;
        public float turnSpeed = 4.0f;

        public float meleeAnimationLength = 3;

        private bool sprinting = false;


        // Use this for initialization
        void Awake()
        {
            SetHashes();
        }

        void Start()
        {
            //Set offset of mesh	
            if (myAIBodyTransform)
            {
                bodyOffset = myAIBodyTransform.localPosition;
                bodyOffset.x *= transform.localScale.x;
                bodyOffset.y *= transform.localScale.y;
                bodyOffset.z *= transform.localScale.z;
                myAIBodyTransform.parent = null;
            }
            else
            {
                Debug.LogWarning("No transform set for 'myAIBodyTransform'.  Please assign a transform in the inspector!");
                this.enabled = false;
            }

            //Inititate Hashes and stuff	
            navi = myBaseScript.GetAgent();
            minDistToCrouch = minDistToCrouch * minDistToCrouch;
            myTransform = transform;

            lerpSpeed = myBaseScript.runSpeed;


            //Check to make sure we have all of our scripts assigned	
            if (!myBaseScript)
            {
                Debug.LogWarning("No Base Script found!  Please add one in the inspector!");
                this.enabled = false;
            }
            else if (maxMovementSpeed < 0)
            {
                maxMovementSpeed = myBaseScript.runSpeed;
            }

            if (!animator)
            {
                Debug.LogWarning("No animator component found!  Please add one in the inspector!");
                this.enabled = false;
            }
            else
            {
                animator.speed = animatorSpeed;
            }
        }



        // Update is called once per frame
        void LateUpdate()
        {
            //Set body to it's current position
            if (!onLink && !myBaseScript.isStaggered && !enteredCover)
            {
                myAIBodyTransform.position = myTransform.position + bodyOffset;
            }
            else if (isVLerping)
            {
                LerpPos();
            }
            
            AnimateAI();
            //This has to be in late update or we get nasty non-normalized quaternions.
            RotateAI();
        }


        //Animations
        void AnimateAI()
        {
            //Correctly blend strafing and forwards/backwards movement
            if (!onLink && !myBaseScript.isStaggered)
            {
                float dampTimeNow = 0.01f;
                if(!myBaseScript.inCover)
                {
                    dampTimeNow = animationDampTime;
                } 

                animator.SetFloat(forwardsMoveHash, Vector3.Dot(myAIBodyTransform.forward, navi.GetDesiredVelocity()) / maxMovementSpeed, dampTimeNow, Time.deltaTime);
                animator.SetFloat(sidewaysMoveHash, Vector3.Dot(myAIBodyTransform.right, navi.GetDesiredVelocity()) / maxMovementSpeed, dampTimeNow, Time.deltaTime);
            }


            Cover();
        }

        bool useAdvancedCover = false;
        int coverFaceDirection;
        Vector3 coverStandDirection;
        public float coverTransitionTime = 0.5f;

        void Cover()
        {
            if(!useAdvancedCover)
            {
                //Check to see if we should crouch, and if so, crouch.  We only crouch if we are in cover and not firing or being suppressed.
                if (myBaseScript.inCover && (!gunScript || !gunScript.IsFiring() || !myBaseScript.shouldFireFromCover) && Vector3.SqrMagnitude(myTransform.position - myBaseScript.GetCurrentCoverNodePos()) < minDistToCrouch && currentVelocityRatio < 0.3)
                {
                    animator.SetBool(crouchingHash, true);
                }
                else
                {
                    animator.SetBool(crouchingHash, false);
                }
            }
            else
            {
                if (myBaseScript.inCover)
                {
                    if (!enteredCover && navi.ReachedDestination())
                    {
                        if (!gunScript || !gunScript.IsFiring() || !myBaseScript.shouldFireFromCover)
                        {
                            animator.SetBool(coverHash, true);
                            enteredCover = true;
                        }
                        useCustomRotation = true;
                        directionToFace = coverStandDirection;
                    }
                    if (enteredCover)
                    {
                        if (!gunScript || !gunScript.IsFiring() || !myBaseScript.shouldFireFromCover)
                        {
                            rotateGunScript.stopForCover = true;
                            animator.SetBool(centerHash, false);
                            animator.SetBool(leftHash, false);
                            animator.SetBool(rightHash, false);
                            startedFireCycle = false;
                        }
                        else if (gunScript.IsFiring())
                        {
                            if(!startedFireCycle)
                            {
                                timeToAimRotate = coverTransitionTime;
                                startedFireCycle = true;
                            }
                            timeToAimRotate -= Time.deltaTime;
                            if (timeToAimRotate < 0)
                            {
                                rotateGunScript.stopForCover = false;
                            }
                            if (coverFaceDirection == 0)
                            {
                                animator.SetBool(centerHash, true);
                            }
                            if (coverFaceDirection == 1)
                            {
                                animator.SetBool(rightHash, true);
                            }
                            if (coverFaceDirection == -1)
                            {
                                animator.SetBool(leftHash, true);
                            }
                        }
                    }
                }
                else
                {
                    animator.SetBool(coverHash, false);
                    rotateGunScript.stopForCover = false;
                }
            }
        }

        float timeToAimRotate;
        bool enteredCover = false;
        bool startedFireCycle = false;

        public void StartAdvancedCover(Vector3 standDir, int faceDir)
        {
            useAdvancedCover = true;
            coverStandDirection = standDir;
            coverFaceDirection = faceDir;
            enteredCover = false;
            startedFireCycle = false;
        }

        public void EndAdvancedCover()
        {
            useAdvancedCover = false;
            useCustomRotation = false;
            enteredCover = false;
            //animator.SetBool(centerHash, false);
            //animator.SetBool(leftHash, false);
            //animator.SetBool(rightHash, false);
        }


        public void StartSprinting()
        {
            if (!sprinting)
            {
                sprinting = true;

                //Make sure the animation in question exists.
                //If the trigger is not found, no animation is played, but no error is thrown.
                for (int i = 0; i < animator.parameters.Length; i++)
                {
                    if (animator.parameters[i].name == "Sprinting")
                    {
                        animator.SetBool(sprintingHash, true);
                    }
                }
            }
        }

        public void StopSprinting()
        {
            if (sprinting)
            {
                sprinting = false;

                //Make sure the animation in question exists.
                //If the trigger is not found, no animation is played, but no error is thrown.
                for (int i = 0; i < animator.parameters.Length; i++)
                {
                    if (animator.parameters[i].name == "Sprinting")
                    {
                        animator.SetBool(sprintingHash, false);
                    }
                }
            }
        }


        public bool isSprinting()
        {
            return sprinting;
        }

        public void PlayReloadAnimation()
        {
            animator.SetTrigger(reloadingHash);
        }

        public void PlayFiringAnimation()
        {
            animator.SetTrigger(fireHash);
        }

        public void PlayDodgingAnimation(bool dodgeRight)
        {
            if (dodgeRight)
            {
                for (int i = 0; i < animator.parameters.Length; i++)
                {
                    if (animator.parameters[i].name == "DodgeRight")
                    {
                        animator.SetTrigger(dodgeRightHash);
                    }
                }

            }
            else
            {
                for (int i = 0; i < animator.parameters.Length; i++)
                {
                    if (animator.parameters[i].name == "DodgeLeft")
                    {
                        animator.SetTrigger(dodgeLeftHash);
                    }
                }

            }
        }

        public void PlayStaggerAnimation()
        {
            for (int i = 0; i < animator.parameters.Length; i++)
            {
                if (animator.parameters[i].name == "Stagger")
                {
                 //   Debug.Break();
                    animator.SetTrigger(staggerHash);
                    animator.SetFloat(forwardsMoveHash, 0, 0, Time.deltaTime);
                    animator.SetFloat(sidewaysMoveHash, 0, 0, Time.deltaTime);
                }
            }
        }

        public void PlayDeathAnimation()
        {
            for (int i = 0; i < animator.parameters.Length; i++)
            {
                if (animator.parameters[i].name == "Death")
                {
                    //   Debug.Break();
                    animator.SetTrigger(staggerHash);
                    animator.SetFloat(forwardsMoveHash, 0, 0, Time.deltaTime);
                    animator.SetFloat(sidewaysMoveHash, 0, 0, Time.deltaTime);
                }
            }
        }

        public void PlayGrenadeAnimation()
        {
            for (int i = 0; i < animator.parameters.Length; i++)
            {
                if (animator.parameters[i].name == "Grenade")
                {
                    //   Debug.Break();
                    animator.SetTrigger(grenadeHash);
                    animator.SetFloat(forwardsMoveHash, 0, 0, Time.deltaTime);
                    animator.SetFloat(sidewaysMoveHash, 0, 0, Time.deltaTime);
                }
            }
        }

        public void StartRot()
        {
            currentlyRotating = true;
        }

        public void StopRot()
        {
            currentlyRotating = false;
        }

        public IEnumerator StartMelee()
        {
            //Rotate to face the target
            directionToFace = -(myAIBodyTransform.position - myBaseScript.targetTransform.position);
            useCustomRotation = true;
            directionToFace.y = 0;

            //Make sure we're rotating		
            while (isPlaying && myAIBodyTransform && myBaseScript.targetTransform && Vector3.Angle(directionToFace, myAIBodyTransform.forward) > maxAngleDeviation)
            {
                directionToFace = -(myAIBodyTransform.position - myBaseScript.targetTransform.position);
                directionToFace.y = 0;

                //Debug stuff
                Debug.DrawRay(myTransform.position, myTransform.forward * 100, Color.magenta);
                Debug.DrawRay(myTransform.position, directionToFace * 100, Color.blue);
                yield return null;
            }

            //Play teh animation
            if (isPlaying && myAIBodyTransform)
            {
                animator.SetTrigger(meleeHash);
                yield return new WaitForSeconds(meleeAnimationLength);
            }
            useCustomRotation = false;
            myBaseScript.StopMelee();
        }


        //Stop errors from spamming the console when the game is stopped in the editor.
        bool isPlaying = true;
        void OnApplicationQuit()
        {
            isPlaying = false;
        }

        public IEnumerator WaitForAnimationToFinish()
        {
            //Wait for transition to finish
            while (animator.IsInTransition(1))
            {
                yield return null;
            }
            //Wait for animation to finish
            while (!animator.IsInTransition(1))
            {
                yield return null;
            }
            //wat for second for second transition to finish
            while (animator.IsInTransition(1))
            {
                yield return null;
            }
        }

        //Dynamic Objects
        public IEnumerator DynamicObjectAnimation(string transitionName, Vector3 dir, DynamicObject dynamicObjectScript, float timeToWait)
        {
            directionToFace = dir;
            useCustomRotation = true;

            directionToFace.y = 0;

            //make sure we're rotating to face the proper direction		
            while (Vector3.Angle(directionToFace, myAIBodyTransform.forward) > maxAngleDeviation)
            {
                Debug.DrawRay(myTransform.position, myTransform.forward * 100, Color.magenta);
                Debug.DrawRay(myTransform.position, directionToFace * 100, Color.blue);
                yield return null;
            }
            currentlyRotating = false;

            //Stop before triggering action to make things smoother
            yield return new WaitForSeconds(0.25f);

            //Play the animation and affect the object	
            bool shouldReactivate = false;
            /*if (rotateGunScript.isEnabled)
            {
                rotateGunScript.Deactivate();
                shouldReactivate = true;
            }*/

            dynamicObjectScript.AffectDynamicObject();

            //Make sure the animation in question exists.
            //If the trigger is not found, no animation is played, but no error is thrown.
            for (int i = 0; i < animator.parameters.Length; i++)
            {
                if (animator.parameters[i].name == transitionName)
                {
                    animator.SetTrigger(Animator.StringToHash(transitionName));
                    //Wait until the animation finishes
                    //Wait for transition to finish
                    //yield return StartCoroutine(WaitForAnimationToFinish());
                    yield return new WaitForSeconds(timeToWait);
                    break;
                }
            }

            //Star aiming our weapon again.
            if (shouldReactivate)
            {
                rotateGunScript.Activate();
            }

            //Finish up the dynamic object sequence
            currentlyRotating = true;
            dynamicObjectScript.EndDynamicObjectUsage();
            useCustomRotation = false;
        }


        public bool currentlyRotating = true;

        //Rotating
        void RotateAI()
        {
            if (currentlyRotating)
            {
                //Rotate to look in the given direction, if one is given.
                if (useCustomRotation)
                {
                    newRotation = Quaternion.LookRotation(directionToFace);
                    newRotation.eulerAngles = new Vector3(0.0f, newRotation.eulerAngles.y, 0.0f);
                    myAIBodyTransform.rotation = Quaternion.Slerp(myAIBodyTransform.rotation, newRotation, turnSpeed * Time.deltaTime);
                    animator.SetFloat(forwardsMoveHash, 0, animationDampTime, Time.deltaTime);
                    animator.SetFloat(sidewaysMoveHash, 0, animationDampTime, Time.deltaTime);
                }
                else if ((myBaseScript.IsEnaging() && myBaseScript.targetTransform && !sprinting))
                {
                    //Get angle between vector of movement and the actual direction enemyBody is facing				
                    myAngle = Vector3.Angle(myTransform.forward, myAIBodyTransform.forward);

                    if (Vector3.Angle(-myAIBodyTransform.right, myTransform.forward) > 90)
                    {
                        myAngle = -myAngle;
                    }

                    //Get angle between vector of movement and the direction we want to be facing
                    float angleBetweenFor = Vector3.Angle(myTransform.forward, myBaseScript.targetTransform.position - myAIBodyTransform.position);

                    //The following if statement is to even out clipping and crossfading problems with ~45 degree angle strafing.
                    //If the angle between the direction we are moving in and the vector to the target will commonly result in clipping, 
                    //then we face the legs in the direction of movement, play either the forwards or backwards animations and
                    // rely on the chest movement to aim at the target.

                    //We will also always rotate to fact the target if the speed is too low, 
                    //because while standing still, the vector of movement becomes unreliable.	
                    if (angleBetweenFor > minAngleToRotateBase && angleBetweenFor < 180 - minAngleToRotateBase)
                    {
                        newRotation = Quaternion.LookRotation(myBaseScript.targetTransform.position - myAIBodyTransform.position);
                    }
                    else
                    {
                        //Play correct animation			    				
                        if (angleBetweenFor < 90)
                        {
                            newRotation = Quaternion.LookRotation(myTransform.forward);
                            animator.SetFloat(forwardsMoveHash, Vector3.Magnitude(navi.GetDesiredVelocity()) / maxMovementSpeed, animationDampTime, Time.deltaTime);
                            animator.SetFloat(sidewaysMoveHash, 0, animationDampTime, Time.deltaTime);
                        }
                        else
                        {
                            newRotation = Quaternion.LookRotation(-myTransform.forward);
                            animator.SetFloat(forwardsMoveHash, -Vector3.Magnitude(navi.GetDesiredVelocity()) / maxMovementSpeed, animationDampTime, Time.deltaTime);
                            animator.SetFloat(sidewaysMoveHash, 0, animationDampTime, Time.deltaTime);
                        }
                    }

                    //Make sure we only rotate around the y axis
                    newRotation.eulerAngles = new Vector3(0.0f, newRotation.eulerAngles.y, 0.0f);

                    //Smoothly rotate to face target		
                    if (!ControllerScript.pMode || Quaternion.Angle(myAIBodyTransform.rotation, newRotation) > 10f)
                    {
                        myAIBodyTransform.rotation = Quaternion.Slerp(myAIBodyTransform.rotation, newRotation, Time.deltaTime * turnSpeed);
                    }
                }
                //Look in the direction we are moving.
                else
                {
                    myAngle = 0;

                    newRotation = Quaternion.LookRotation(myTransform.forward);
                    newRotation.eulerAngles = new Vector3(0.0f, newRotation.eulerAngles.y, 0.0f);

                    if (!ControllerScript.pMode || Quaternion.Angle(myAIBodyTransform.rotation, newRotation) > 10f)
                    {
                        myAIBodyTransform.rotation = Quaternion.Slerp(myAIBodyTransform.rotation, newRotation, turnSpeed * Time.deltaTime);
                    }
                }
            }
        }

        //Setters
        void SetHashes()
        {
            crouchingHash = Animator.StringToHash("Crouching");
            engagingHash = Animator.StringToHash("Engaging");
            reloadingHash = Animator.StringToHash("Reloading");
            meleeHash = Animator.StringToHash("Melee");
            fireHash = Animator.StringToHash("Fire");
            sidewaysMoveHash = Animator.StringToHash("Horizontal");
            forwardsMoveHash = Animator.StringToHash("Forwards");
            sprintingHash = Animator.StringToHash("Sprinting");
            dodgeRightHash = Animator.StringToHash("DodgeRight");
            dodgeLeftHash = Animator.StringToHash("DodgeLeft");
            //leapHash = Animator.StringToHash("Leap");
            //vaultHash = Animator.StringToHash("Vault");
            staggerHash = Animator.StringToHash("Stagger");
            grenadeHash = Animator.StringToHash("Grenade");

            coverHash = Animator.StringToHash("Cover");
            centerHash = Animator.StringToHash("CoverCenter");
            rightHash = Animator.StringToHash("CoverRight");
            leftHash = Animator.StringToHash("CoverLeft");
            setHashes = true;
        }


        bool setHashes = false;

        //Called when the agent enters direct combat
        //The weapon is raised
        public void SetEngaging()
        {
            //yield return null;
            if (!setHashes)
                SetHashes();
            animator.SetBool(engagingHash, true);
        }

        //Called when the agent loses track of the target, or the target is eliminated.
        //The weapon is lowered
        public void SetDisengage()
        {
            //yield return null;
            if (animator)
            {
                if (!setHashes)
                    SetHashes();
                animator.SetBool(engagingHash, false);
            }
        }

        public void LerpPos()
        {
            if (lerpAmt < 1.0f)
            {
                lerpAmt += lerpSpeed / Vector3.Distance(lerpTargPos, startPos) * Time.deltaTime;
                myAIBodyTransform.position = Vector3.Lerp(startPos, lerpTargPos, lerpAmt);
                transform.position = myAIBodyTransform.position;
            }
            else
            {
                isVLerping = false;
                lerpAmt = 0;

                animator.SetFloat(forwardsMoveHash, 0, animationDampTime, Time.deltaTime);
                animator.SetFloat(sidewaysMoveHash, 0, animationDampTime, Time.deltaTime);
            }
        }

        public bool onLink = false;
        public float leapAnimationLength = 1.5f;
        public float vaultAnimationLength = 1.5f;
        public float leapMaxAngle = 15.0f;
        public float vaultMaxAngle = 10.0f;

        public Vector3 lerpTargPos;
        float lerpSpeed = 5f;
        float lerpAmt = 0.0f;
        Vector3 startPos;
        public bool isVLerping;

        public void Parkour(Vector3 dir, string transitionName, Vector3 lerPos)
        {
            onLink = true;
            lerpAmt = 0.0f;
            rotateGunScript.Deactivate();
            float animTime = 3.0f;
            startPos = myTransform.position;
            lerpTargPos = lerPos;
            lerpTargPos.y = myTransform.position.y;
            float ang = 15.0f;


            switch (transitionName)
            {
                case "Vault": animTime = vaultAnimationLength; ang = vaultMaxAngle; break;
                case "Leap": animTime = leapAnimationLength; ang = leapMaxAngle; break;
            }

            StartCoroutine(ParkourAnimate(transitionName, dir, animTime, ang));
        }


        //Dynamic Objects
        public IEnumerator ParkourAnimate(string transitionName, Vector3 dir, float animTime, float maxAng)
        {


            lerpAmt = 0;
            isVLerping = true;


            yield return new WaitForSeconds(0.1f);

            directionToFace = dir;
            directionToFace.y = 0;
            useCustomRotation = true;

            //make sure we're rotating to face the proper direction		
            while (Vector3.Angle(directionToFace, myAIBodyTransform.forward) > maxAng || isVLerping || gunScript.IsFiring() || gunScript.IsReloading())
            {
                Debug.DrawRay(myTransform.position, myAIBodyTransform.forward * 100, Color.yellow);
                Debug.DrawRay(myTransform.position, directionToFace * 100, Color.cyan);
                yield return null;
            }
            currentlyRotating = false;

            //yield return new WaitForSeconds(0.05f);

            //Make sure the animation in question exists.
            //If the trigger is not found, no animation is played, but no error is thrown.
            for (int i = 0; i < animator.parameters.Length; i++)
            {
                if (animator.parameters[i].name == transitionName)
                {
                    animator.SetTrigger(Animator.StringToHash(transitionName));
                    yield return new WaitForSeconds(animTime);
                    break;
                }
            }

            //Finish up the dynamic object sequence
            currentlyRotating = true;
            useCustomRotation = false;
            onLink = false;           
        }
    }
}
