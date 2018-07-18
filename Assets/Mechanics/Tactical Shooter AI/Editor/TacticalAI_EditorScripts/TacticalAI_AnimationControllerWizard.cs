using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.Animations;

public class TacticalAI_AnimationControllerWizard : EditorWindow 
{
	private string animationControllerName = "Tactical AI Animation Controller";

	AnimationClip idleAnimation;
	AnimationClip aimingAnimation;
				
	AnimationClip runForwardsAnimation;
	AnimationClip runBackwardsAnimation;
	AnimationClip runLeftAnimation;
	AnimationClip runRightAnimation;
			
	bool showOptionalAnimations = false;

	AnimationClip walkForwardsAnimation;
	AnimationClip walkBackwardsAnimation;
	AnimationClip walkLeftAnimation;
	AnimationClip walkRightAnimation;

    AnimationClip walkForwardRightsAnimation;
    AnimationClip walkForwardLeftAnimation;
    AnimationClip walkBackwardsRightAnimation;  
    AnimationClip walkBackwardsLeftAnimation;

    AnimationClip runForwardRightsAnimation;
    AnimationClip runForwardLeftAnimation;
    AnimationClip runBackwardsRightAnimation;
    AnimationClip runBackwardsLeftAnimation;


    bool fullBodyMeleeAnimation = false;		
	AnimationClip meleeAnimation;
	
	AnimationClip reloadAnimation;
	AnimationClip fireAnimation;

    AnimationClip dodgeLeftAnimation;
    AnimationClip dodgeRightAnimation;

    AnimationClip idleUpperAnim;
	AnimationClip walkUpperAnim;
	AnimationClip runUpperAnim;
	
	AnimationClip sprintingAnimation;
	
	AnimationClip crouchingAnimation;

    AnimationClip leapAnimation;
    AnimationClip vaultAnimation;
    AnimationClip staggerAnimation;

    public TacticalAI.AnimationNameCombo dynamicObjects;
			
	[MenuItem ("Assets/Create/Tactical AI Animation Controller")]		
    public static void  ShowWindow () 
	    {
	        EditorWindow.GetWindow(typeof(TacticalAI_AnimationControllerWizard));      
	    }	
	    
	private Vector2 scrollPos;
	
	
	void OnGUI () 
		{
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);		
			GUILayout.Label ("[R] means that this is a requried field", EditorStyles.boldLabel);  
			     	
        	EditorGUILayout.Space();
        	GUILayout.Label ("Animation Settings", EditorStyles.boldLabel);	
        			animationControllerName = EditorGUILayout.TextField("Controller Name", animationControllerName); 
        			EditorGUILayout.Space();
        			
        			idleAnimation = (AnimationClip)EditorGUILayout.ObjectField("[R] Standing", idleAnimation, typeof(AnimationClip), true);
        			aimingAnimation = (AnimationClip)EditorGUILayout.ObjectField("[R] Aim (Upper Body)", aimingAnimation, typeof(AnimationClip), true);       			
        			
        			runForwardsAnimation = (AnimationClip)EditorGUILayout.ObjectField("[R] Run Forwards", runForwardsAnimation, typeof(AnimationClip), true);
        			runBackwardsAnimation = (AnimationClip)EditorGUILayout.ObjectField("[R] Run Backwards", runBackwardsAnimation, typeof(AnimationClip), true);
        			runLeftAnimation = (AnimationClip)EditorGUILayout.ObjectField("[R] Run Left", runLeftAnimation, typeof(AnimationClip), true);
        			runRightAnimation = (AnimationClip)EditorGUILayout.ObjectField("[R] Run Right", runRightAnimation, typeof(AnimationClip), true);
        			crouchingAnimation = (AnimationClip)EditorGUILayout.ObjectField("Crouching", crouchingAnimation, typeof(AnimationClip), true); 
        			
        			showOptionalAnimations = EditorGUILayout.ToggleLeft("Show Optional Animations", showOptionalAnimations);
        			if(showOptionalAnimations)
        				{
							meleeAnimation = (AnimationClip)EditorGUILayout.ObjectField("Melee", meleeAnimation, typeof(AnimationClip), true);
        					fullBodyMeleeAnimation = EditorGUILayout.Toggle("Full Body Melee Animation", fullBodyMeleeAnimation);
        					
        					sprintingAnimation =  (AnimationClip)EditorGUILayout.ObjectField("Sprinting", sprintingAnimation, typeof(AnimationClip), true);
							
							reloadAnimation = (AnimationClip)EditorGUILayout.ObjectField("Reload", reloadAnimation, typeof(AnimationClip), true);
							fireAnimation = (AnimationClip)EditorGUILayout.ObjectField("Fire", fireAnimation, typeof(AnimationClip), true);	
							        				
		        			walkForwardsAnimation = (AnimationClip)EditorGUILayout.ObjectField("Walk Forwards", walkForwardsAnimation, typeof(AnimationClip), true);
		        			walkBackwardsAnimation = (AnimationClip)EditorGUILayout.ObjectField("Walk Backwards", walkBackwardsAnimation, typeof(AnimationClip), true);
		        			walkLeftAnimation = (AnimationClip)EditorGUILayout.ObjectField("Walk Left", walkLeftAnimation, typeof(AnimationClip), true);
		        			walkRightAnimation = (AnimationClip)EditorGUILayout.ObjectField("Walk Right", walkRightAnimation, typeof(AnimationClip), true);
                            EditorGUILayout.Space();

                            walkForwardRightsAnimation = (AnimationClip)EditorGUILayout.ObjectField("Walk Forward-Right", walkForwardRightsAnimation, typeof(AnimationClip), true);
                            walkForwardLeftAnimation = (AnimationClip)EditorGUILayout.ObjectField("Walk Forward-Left", walkForwardLeftAnimation, typeof(AnimationClip), true);
                            walkBackwardsRightAnimation = (AnimationClip)EditorGUILayout.ObjectField("Walk Backward-Right", walkBackwardsRightAnimation, typeof(AnimationClip), true);
                            walkBackwardsLeftAnimation = (AnimationClip)EditorGUILayout.ObjectField("Walk Backward-Left", walkBackwardsLeftAnimation, typeof(AnimationClip), true);
                            EditorGUILayout.Space();


                            runForwardRightsAnimation = (AnimationClip)EditorGUILayout.ObjectField("Run Forward-Right", runForwardRightsAnimation, typeof(AnimationClip), true);
                            runForwardLeftAnimation = (AnimationClip)EditorGUILayout.ObjectField("Run Forward-Left", runForwardLeftAnimation, typeof(AnimationClip), true);
                            runBackwardsRightAnimation = (AnimationClip)EditorGUILayout.ObjectField("Run Backward-Right", runBackwardsRightAnimation, typeof(AnimationClip), true);
                            runBackwardsLeftAnimation = (AnimationClip)EditorGUILayout.ObjectField("Run Backward-Left", runBackwardsLeftAnimation, typeof(AnimationClip), true);
                            EditorGUILayout.Space();
							
							idleUpperAnim = (AnimationClip)EditorGUILayout.ObjectField("Idle (Upper Body)", idleUpperAnim, typeof(AnimationClip), true);
							walkUpperAnim = (AnimationClip)EditorGUILayout.ObjectField("Walk Idle (Upper body)", walkUpperAnim, typeof(AnimationClip), true);
							runUpperAnim = (AnimationClip)EditorGUILayout.ObjectField("Run Idle (Upper body)", runUpperAnim, typeof(AnimationClip), true);
                            EditorGUILayout.Space();

                            dodgeLeftAnimation = (AnimationClip)EditorGUILayout.ObjectField("Dodge Left", dodgeLeftAnimation, typeof(AnimationClip), true);
                            dodgeRightAnimation = (AnimationClip)EditorGUILayout.ObjectField("Dodge Right", dodgeRightAnimation, typeof(AnimationClip), true);                          
                            EditorGUILayout.Space();

                            leapAnimation = (AnimationClip)EditorGUILayout.ObjectField("Leap", leapAnimation, typeof(AnimationClip), true);
                            vaultAnimation = (AnimationClip)EditorGUILayout.ObjectField("Vault", vaultAnimation, typeof(AnimationClip), true);
                            staggerAnimation = (AnimationClip)EditorGUILayout.ObjectField("Stagger", staggerAnimation, typeof(AnimationClip), true);
        }

        		
        	EditorGUILayout.Space();
        	
        	bool hasAllAnimations = (idleAnimation && aimingAnimation && runForwardsAnimation && runBackwardsAnimation && runLeftAnimation && runRightAnimation);
        		
        	if(hasAllAnimations)
        		GUI.enabled = true;
        	else
        		{
        			string strToWrite = "";

        			if(!idleAnimation)	
        				strToWrite = "'Standing' Animation field cannot be empty!";	
        			else if(!aimingAnimation)			
        				strToWrite = "'Aim [Upper Body]' Animation field cannot be empty!";	
        			else if(!runBackwardsAnimation)			
        				strToWrite = "'Run Forwards' Animation field cannot be empty!";	
        			else if(!runLeftAnimation)			
        				strToWrite = "'Run Backwards' Animation field cannot be empty!";	
        			else if(!runLeftAnimation)			
        				strToWrite = "'Run Left' Animation field cannot be empty!";	
        			else if(!runRightAnimation)			
        				strToWrite = "'Run Right' Animation field cannot be empty!";	        							        							        							        							
        				        			
        			EditorGUILayout.HelpBox(strToWrite, MessageType.Warning);
        			GUI.enabled = false;
        		}	
        	 	
        	 	
        	 EditorGUILayout.Space();
        	 	
			if(GUILayout.Button("Create New Animation Controller"))
	        {
	            this.CreateANewController();
	            this.Close();
	        }
	        		
        	EditorGUILayout.EndScrollView();   	
		}
		
		void CreateANewController()
		{
			var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath ("Assets/"+animationControllerName+".controller");
		
			controller.AddParameter("Forwards", AnimatorControllerParameterType.Float);	
			controller.AddParameter("Horizontal", AnimatorControllerParameterType.Float);		
			controller.AddParameter("Crouching", AnimatorControllerParameterType.Bool);		
			controller.AddParameter("Speed", AnimatorControllerParameterType.Float);			
			controller.AddParameter("Engaging", AnimatorControllerParameterType.Bool);	
			
			controller.AddParameter("Melee", AnimatorControllerParameterType.Trigger);	
			controller.AddParameter("Reloading", AnimatorControllerParameterType.Trigger);	

			controller.AddParameter("Fire", AnimatorControllerParameterType.Trigger);	
			controller.AddParameter("Sprinting", AnimatorControllerParameterType.Bool);	

            controller.AddParameter("DodgeRight", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("DodgeLeft", AnimatorControllerParameterType.Trigger);

            controller.AddParameter("Leap", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Vault", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Stagger", AnimatorControllerParameterType.Trigger);

        //LAYER 1///////////////////////////////////////////
        //Add Outer States
        var baseStateMachine = controller.layers[0].stateMachine;
					
			
			//Set Move Tree
			//new BlendTree();	
			BlendTree moveTree = new BlendTree();
			AnimatorState moveState = controller.CreateBlendTreeInController("Move", out moveTree, 0);								
			
			//Add Transitions					
			AnimatorState baseCrouching = null;
			if(crouchingAnimation){
				baseCrouching = baseStateMachine.AddState("Crouch");
				baseCrouching.motion = crouchingAnimation;
				
				//Move
				var moveToCrouchTranistion = moveState.AddTransition(baseCrouching, false);
				moveToCrouchTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "Crouching");
				
				var crouchToMoveTranistion = baseCrouching.AddTransition(moveState, false);
				crouchToMoveTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0.25f, "Crouching");
			
			}
			
			if(fullBodyMeleeAnimation && meleeAnimation){
					var baseMelee = baseStateMachine.AddState("Melee");
					baseMelee.motion = meleeAnimation;	
				
				//Melee
				if(baseCrouching != null){
					var crouchToMeleeTranistion = baseCrouching.AddTransition(baseMelee, false);
					crouchToMeleeTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "Melee");		
				}	
				
				//Move
				var moveToMeleeTranistion = moveState.AddTransition(baseMelee, false);
				moveToMeleeTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "Melee");
						
				baseMelee.AddTransition(moveState, true);						
			}	
			
			//		
			if(sprintingAnimation)
			{
				var baseSprint = baseStateMachine.AddState("Sprint");
				baseSprint.motion = sprintingAnimation;	
				
				//Move
				var moveToSprintTranistion = moveState.AddTransition(baseSprint, false);
				moveToSprintTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "Sprinting");
				
				var sprintToMoveTranistion = baseSprint.AddTransition(moveState, false);	
				sprintToMoveTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0.25f, "Sprinting");													
			}	
			//Set Parameters
            moveTree.blendType = BlendTreeType.FreeformDirectional2D;
            moveTree.blendParameter = "Horizontal";
            moveTree.blendParameterY = "Forwards";
			
			//Add Animations
			float walkThreshold = 0.5f;
			float runThreshold = 1.0f;

            moveTree.AddChild(idleAnimation, new Vector2(0.0f, 0.0f));

            moveTree.AddChild(runForwardsAnimation, new Vector2(0.0f, runThreshold));
            if (walkForwardsAnimation)
            {
                moveTree.AddChild(walkForwardsAnimation, new Vector2(0.0f, walkThreshold));
            }

            moveTree.AddChild(runBackwardsAnimation, new Vector2(0.0f, -runThreshold));
            if (walkBackwardsAnimation)
            {
                moveTree.AddChild(walkBackwardsAnimation, new Vector2(0.0f, -walkThreshold));
            }

            moveTree.AddChild(runRightAnimation, new Vector2(runThreshold, 0.0f));
            if (walkRightAnimation)
            {
                moveTree.AddChild(walkRightAnimation, new Vector2(walkThreshold, 0.0f));
            }

            moveTree.AddChild(runLeftAnimation, new Vector2(-runThreshold, 0.0f));
            if (walkLeftAnimation)
            {
                moveTree.AddChild(walkLeftAnimation, new Vector2(-walkThreshold, 0.0f));
            }

            if (walkForwardRightsAnimation) { moveTree.AddChild(walkForwardRightsAnimation, new Vector2(walkThreshold, walkThreshold)); }
            if (walkForwardLeftAnimation) { moveTree.AddChild(walkForwardLeftAnimation, new Vector2(-walkThreshold, walkThreshold)); }
            if (walkBackwardsRightAnimation) { moveTree.AddChild(walkBackwardsRightAnimation, new Vector2(walkThreshold, -walkThreshold)); }
            if (walkBackwardsLeftAnimation) { moveTree.AddChild(walkBackwardsLeftAnimation, new Vector2(-walkThreshold, -walkThreshold)); }

            if (runForwardRightsAnimation) { moveTree.AddChild(runForwardRightsAnimation, new Vector2(runThreshold, runThreshold)); }
            if (runForwardLeftAnimation) { moveTree.AddChild(runForwardLeftAnimation, new Vector2(-runThreshold, runThreshold)); }
            if (runBackwardsRightAnimation) { moveTree.AddChild(runBackwardsRightAnimation, new Vector2(runThreshold, -runThreshold)); }
            if (runBackwardsLeftAnimation) { moveTree.AddChild(runBackwardsLeftAnimation, new Vector2(-runThreshold, -runThreshold)); }

        if (dodgeLeftAnimation)
            {
                AnimatorState leftDB = baseStateMachine.AddState("DodgeLeft");
                leftDB.motion = dodgeLeftAnimation;

                var anyToLeftDTranistion = baseStateMachine.AddAnyStateTransition(leftDB);
                anyToLeftDTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "DodgeLeft");

                leftDB.AddTransition(moveState, true);
            }

            if (dodgeRightAnimation)
            {
                AnimatorState rightDB = baseStateMachine.AddState("DodgeRight");
                rightDB.motion = dodgeRightAnimation;

                var anyToRightDTranistion = baseStateMachine.AddAnyStateTransition(rightDB);
                anyToRightDTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "DodgeRight");

                rightDB.AddTransition(moveState, true);
            }

        if (leapAnimation)
        {
            AnimatorState leapB = baseStateMachine.AddState("Leap");
            leapB.motion = leapAnimation;

            var anyToLeapBTranistion = baseStateMachine.AddAnyStateTransition(leapB);
            anyToLeapBTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "Leap");

            leapB.AddTransition(moveState, true);
        }

        if (vaultAnimation)
        {
            AnimatorState vaultB = baseStateMachine.AddState("Vault");
            vaultB.motion = vaultAnimation;

            var anyToVaultBTranistion = baseStateMachine.AddAnyStateTransition(vaultB);
            anyToVaultBTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "Vault");

            vaultB.AddTransition(moveState, true);
        }

        if (staggerAnimation)
        {
            AnimatorState staggerB = baseStateMachine.AddState("Stagger");
            staggerB.motion = staggerAnimation;

            var anyToStaggerBTranistion = baseStateMachine.AddAnyStateTransition(staggerB);
            anyToStaggerBTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "Stagger");

            staggerB.AddTransition(moveState, true);
        }

        ////////////////////////////////////////////////////	
        //Layer 2
        ////////////////////////////////////////////////////

        controller.AddLayer("UpperBody");
			var upperStateMachine = controller.layers[1].stateMachine;
			
						
			var upperAim = upperStateMachine.AddState("Aim");	
			upperAim.motion = aimingAnimation;					
			
			BlendTree idleUpperTree = new BlendTree();
			AnimatorState upperIdle = controller.CreateBlendTreeInController("Idle", out idleUpperTree, 1);	
			idleUpperTree.blendParameter = "Speed";
			
			idleUpperTree.AddChild(idleUpperAnim,0.0f);
			if(walkUpperAnim)			
				idleUpperTree.AddChild(walkUpperAnim,0.5f);
			if(runUpperAnim)
				idleUpperTree.AddChild(runUpperAnim,1.0f);			

			
			//Idle Transitions	
			var idleToAim = upperIdle.AddTransition(upperAim, false);
			idleToAim.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "Engaging");	
			
			var aimToidle = upperAim.AddTransition(upperIdle, false);						
			aimToidle.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0.25f, "Engaging");

            if (dodgeLeftAnimation)
            {
                AnimatorState leftD = upperStateMachine.AddState("DodgeLeft");
                leftD.motion = dodgeLeftAnimation;

                var anyToLeftDTranistion = upperStateMachine.AddAnyStateTransition(leftD);
                anyToLeftDTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "DodgeLeft");

                leftD.AddTransition(upperAim, true);
            }

            if (dodgeRightAnimation)
            {
                AnimatorState rightD = upperStateMachine.AddState("DodgeRight");
                rightD.motion = dodgeRightAnimation;

                var anyToRightDTranistion = upperStateMachine.AddAnyStateTransition(rightD);
                anyToRightDTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "DodgeRight");

                rightD.AddTransition(upperAim, true);
            }
            
            if (leapAnimation)
            {
                AnimatorState leapU = upperStateMachine.AddState("Leap");
                leapU.motion = leapAnimation;

                var anyToLeapUTranistion = upperStateMachine.AddAnyStateTransition(leapU);
                anyToLeapUTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "Leap");

                leapU.AddTransition(upperAim, true);
            }

            if (vaultAnimation)
            {
                AnimatorState vaultU = upperStateMachine.AddState("Vault");
                vaultU.motion = vaultAnimation;

                var anyToVaultUTranistion = upperStateMachine.AddAnyStateTransition(vaultU);
                anyToVaultUTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "Vault");

                vaultU.AddTransition(upperAim, true);
            }

            if (staggerAnimation)
            {
                AnimatorState staggerU = upperStateMachine.AddState("Stagger");
                staggerU.motion = staggerAnimation;

                var anyToStaggerUTranistion = upperStateMachine.AddAnyStateTransition(staggerU);
                anyToStaggerUTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "Stagger");

                staggerU.AddTransition(upperAim, true);
            }
            
        //Melee 
        if (meleeAnimation)
				{
					AnimatorState upperMelee = upperStateMachine.AddState("Melee");
					upperMelee.motion = meleeAnimation;
					
					var aimToMeleeTranistion = upperAim.AddTransition(upperMelee, false);
					aimToMeleeTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "Melee");	
					
					upperMelee.AddTransition(upperAim, true);								
				}
			if(sprintingAnimation)
				{
					var upperSprint = upperStateMachine.AddState("Sprint");
					upperSprint.motion = sprintingAnimation;	
					
					//Move
					var aimToSprintTranistion = upperAim.AddTransition(upperSprint, false);
					aimToSprintTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "Sprinting");
					
					var sprintToAimTranistion = upperSprint.AddTransition(upperAim, false);	
					sprintToAimTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0.25f, "Sprinting");													
				}		
			
			//Reload 	
			if(reloadAnimation)
				{
					AnimatorState reloadAnim = upperStateMachine.AddState("Reload");
					reloadAnim.motion = reloadAnimation;
					
					var aimToReloadTranistion = upperAim.AddTransition(reloadAnim, false);
					aimToReloadTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "Reloading");	
					
					reloadAnim.AddTransition(upperAim, true);								
				}
			//Fire 	
			if(fireAnimation)
				{
					AnimatorState fireAnim = upperStateMachine.AddState("Fire");
					fireAnim.motion = fireAnimation;
					
					var aimToFireTranistion = upperAim.AddTransition(fireAnim, false);
					aimToFireTranistion.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.25f, "Fire");	
					
					fireAnim.AddTransition(upperAim, true);								
				}														
		}
}

namespace TacticalAI{
public class AnimationNameCombo
{
	public string stateName = "Dynamic_Object";
	public AnimationClip animation;
						
	private AnimationNameCombo()
		{
			stateName = "Dynamic_Object";
		}
}
}


