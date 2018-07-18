using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xrayhunter.WWIIAlarms
{
    [RequireComponent(typeof(AudioSource))]
    public class AirAlarm : MonoBehaviour
    {

        public float RadarDetectionRadius = 500.0f;

        public float ScanTimer = 5.0f;

        public AudioClip alarm;

        private float ScanCounter = 0.0f;

        private AudioSource source;

        private bool needingToPlay = false;

        // Use this for initialization
        void Start()
        {
            source = GetComponent<AudioSource>();
            source.spatialBlend = 1;
            source.loop = true;

            if (alarm != null)
                source.clip = alarm;
        }

        // Update is called once per frame
        void Update()
        {
            ScanCounter -= Time.deltaTime;
            if (ScanCounter <= 0)
            {
                needingToPlay = false;
                Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, RadarDetectionRadius);

                foreach (Collider collider in hitColliders)
                {
                    if (collider.GetComponent<WWIIBomber.BomberPlane>() != null)
                    {
                        needingToPlay = true;
                    }
                }
                if (needingToPlay)
                {
                    if (source.clip != null && !source.isPlaying)
                        source.Play();

                    source.volume = 1.0f;
                }
                else
                    StartCoroutine(FadeOut(0.01f));


                ScanCounter = ScanTimer;
            }

        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(this.transform.position, RadarDetectionRadius);
        }

        private IEnumerator FadeOut(float speed)
        {
            while (source.volume > 0.0f)
            {
                source.volume -= speed;
                yield return new WaitForSeconds(0.1f);
            }

            if (source.volume == 0.0f)
                source.Stop();
        }
    }

}
