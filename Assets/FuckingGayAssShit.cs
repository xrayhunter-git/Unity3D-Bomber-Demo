using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuckingGayAssShit : MonoBehaviour {
    public GameObject prefab_explosion;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            // Explode prefab
            if (prefab_explosion == null) return;
            GameObject explosion = Instantiate(prefab_explosion, this.transform);
            Destroy(this.GetComponent<Renderer>());
        }
    }
}
