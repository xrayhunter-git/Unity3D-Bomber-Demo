using UnityEngine;
using System.Collections;



/*
 * This script lets users manually mark cover positions.  
 * */


namespace TacticalAI
{
    public class CoverNodeScript : MonoBehaviour
    {

        public Vector3 SightNodeOffSet = new Vector3(0, 1, 0);

        private Vector3 myPosition;
        private Vector3 sightNodePosition;
        public float nodeRadiusVisualization = 0.1f;

        public bool alwaysDisplay = true;

        public LayerMask layerMask;

        public bool isActive = true;

        private bool occupied = false;
        public bool isAdvancedCover = true;

        public float coverNodeGroup;

        void Start()
        {
            SetPositions();
            AutoSetCoverDirection();
            FindFaceDir();
        }


        void SetPositions()
        {
            myPosition = transform.position;

            sightNodePosition = transform.position;

            sightNodePosition += (transform.forward * SightNodeOffSet.x);
            sightNodePosition += (transform.up * SightNodeOffSet.y);
            sightNodePosition += (transform.right * SightNodeOffSet.z);
        }

        public float angleForAdvancedCover = 100;

        public bool ValidCoverCheck(Vector3 targetPos)
        {
            //Check to see if this cover node is safe
            if (isActive)
            {
                if (Physics.Linecast(myPosition, targetPos, layerMask))
                {
                    //Check to see if this cover node has LOS to target from firingPos
                    if (!Physics.Linecast(sightNodePosition, targetPos, layerMask))
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        public bool CheckForSafety(Vector3 targetPos)
        {
            //Debug.DrawLine(myPosition, targetPos, Color.green);
            //Debug.Break();
            if((!isAdvancedCover || Vector3.Angle(advancedCoverDirection, sightNodePosition - targetPos) > angleForAdvancedCover/2) && Physics.Linecast(myPosition, targetPos, layerMask))
            {
                return true;
            }
            return false;        
        }



        void OnDrawGizmosSelected()
        {
            if (!alwaysDisplay)
            {
                SetPositions();

                if (occupied)
                    Gizmos.color = Color.yellow;
                else if (isActive)
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.red;

                Gizmos.DrawSphere(myPosition, nodeRadiusVisualization);
                Gizmos.DrawWireSphere(sightNodePosition, nodeRadiusVisualization * 2);

                if (isAdvancedCover)
                {
                    Gizmos.DrawRay(sightNodePosition, advancedCoverDirection * 1);
                    Vector3 tarVec = Quaternion.AngleAxis(angleForAdvancedCover / 2, Vector3.up) * advancedCoverDirection;
                    Gizmos.DrawRay(sightNodePosition, tarVec * 1);
                    tarVec = Quaternion.AngleAxis(-angleForAdvancedCover / 2, Vector3.up) * advancedCoverDirection;
                    Gizmos.DrawRay(sightNodePosition, tarVec * 1);
                }
            }
        }

        void OnDrawGizmos()
        {
            if (alwaysDisplay)
            {
                SetPositions();

                if (occupied)
                    Gizmos.color = Color.yellow;
                else if (isActive)
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.red;

                Gizmos.DrawSphere(myPosition, nodeRadiusVisualization);
                Gizmos.DrawWireSphere(sightNodePosition, nodeRadiusVisualization * 2);

                if (isAdvancedCover)
                {
                    Gizmos.DrawRay(sightNodePosition, advancedCoverDirection * 1);
                    Vector3 tarVec = Quaternion.AngleAxis(angleForAdvancedCover / 2, Vector3.up) * advancedCoverDirection;
                    Gizmos.DrawRay(sightNodePosition, tarVec * 1);
                    tarVec = Quaternion.AngleAxis(-angleForAdvancedCover / 2, Vector3.up) * advancedCoverDirection;
                    Gizmos.DrawRay(sightNodePosition, tarVec * 1);
                }
            }
        }

        public Vector3 GetSightNodePosition()
        {
            return sightNodePosition;
        }

        public Vector3 GetPosition()
        {
            return myPosition;
        }

        public void ActivateNode(float t)
        {
            StartCoroutine(EnableThisNode(t));
        }

        IEnumerator EnableThisNode(float t)
        {
            yield return new WaitForSeconds(t);
            isActive = true;
        }

        public void DeActivateNode()
        {
            isActive = false;
        }

        public bool isOccupied()
        {
            return occupied;
        }

        public void setOccupied(bool b)
        {
            occupied = b;
        }

        public Vector3 advancedCoverDirection =  Vector3.forward;
        public bool shouldAutoChooseDirection = true;
        float closestDistSoFar;
        [HideInInspector]
        public int faceDir = 0;

        public void FindFaceDir()
        {
            Vector3 adMyPos =  sightNodePosition - myPosition;
            adMyPos.y = 0;
            float crossP =  Vector3.Dot(adMyPos , Quaternion.AngleAxis(90, Vector3.up) * advancedCoverDirection);
            faceDir = Mathf.RoundToInt(crossP);
            //Debug.Log(faceDir);     
        }

        public void AutoSetCoverDirection()
        {
            if (shouldAutoChooseDirection)
            {
                Vector3 dir = Vector3.forward;
                myPosition = transform.position;
                closestDistSoFar = 999999f;

                dir = CompareDists(dir, transform.right);
                dir = CompareDists(dir, -transform.right);
                dir = CompareDists(dir, -transform.forward);
                dir = CompareDists(dir, transform.forward);
                //Debug.Log(closestDistSoFar);

                advancedCoverDirection = dir;
            }
        }

        public Vector3 CompareDists(Vector3 oldDir, Vector3 dirNow)
        {
            RaycastHit hit;
            if (Physics.Raycast(myPosition, dirNow, out hit, layerMask))
            {                 
                    Debug.DrawLine(myPosition, hit.point, Color.red);
                    float distNow = Vector3.SqrMagnitude(hit.point - myPosition);
                    if (distNow < closestDistSoFar)
                        {
                            closestDistSoFar = distNow;
                            return dirNow;
                        }
            }
            return oldDir;
        }

    }
}
