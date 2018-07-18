using UnityEngine;
using System.Collections;

public class FlankingRoute : MonoBehaviour {

    public Transform[] route = new Transform[0];
    public bool showPath = true;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	public Transform[] GetRoute()
    {
        return route;
    }

    void OnDrawGizmos()
    {
        if (showPath && route.Length > 1)
        {
            for (int i = 1; i < route.Length; i++)
            {
                if (route[i])
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(route[i].position, route[i - 1].position);
                }
                else
                {
                    break;
                }
            }
        }
    }
}
