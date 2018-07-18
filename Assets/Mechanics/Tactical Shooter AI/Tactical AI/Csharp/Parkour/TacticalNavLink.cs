using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TacticalAI
{
    public class TacticalNavLink : MonoBehaviour
    {
        public AnimType animtype;
        public Vector3 position;
        [HideInInspector]
        public Transform destTransform;
        [HideInInspector]
        public string animString;
        public bool visualize = true;
        public float visualizationRadius = 0.5f;

        public enum AnimType
        {
            Leap = 1, Vault = 2,
        }

        void Awake()
        {
            position = transform.position;
            destTransform = gameObject.GetComponent<UnityEngine.AI.OffMeshLink>().endTransform;
            TacticalAI.ControllerScript.currentController.AddParkourLink(this);
            switch (animtype)
            {
                case AnimType.Leap: animString = "Leap"; break;
                case AnimType.Vault: animString = "Vault"; break;
            }
        }

        public AnimType GetAnimationType()
        {
            return animtype;
        }

        void OnDrawGizmos()
        {
            if (visualize)
            {
                Gizmos.color = Color.blue;

                Gizmos.DrawSphere(transform.position, visualizationRadius);
                if (gameObject.GetComponent<UnityEngine.AI.OffMeshLink>().endTransform)
                {
                    Transform g = gameObject.GetComponent<UnityEngine.AI.OffMeshLink>().endTransform;
                    Gizmos.DrawSphere(g.position, visualizationRadius/2);
                    Gizmos.DrawLine(g.position, transform.position);
                }
            }
        }

    }
}
