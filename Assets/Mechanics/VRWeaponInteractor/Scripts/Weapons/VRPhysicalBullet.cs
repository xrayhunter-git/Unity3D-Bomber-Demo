//========= Copyright 2018, Sam Tague, All rights reserved. ===================
//
// Moves an object until it hits a target, at which time it calls the Damage(float) method on all scripts on the hit object
// Can also home in on targets and "detonate" when in close proximity.
//
//=============================================================================
#if VRInteraction
using UnityEngine;
using System.Collections;
using VRInteraction;

namespace VRWeaponInteractor
{
	public class VRPhysicalBullet : MonoBehaviour
    {
		public Transform bulletRoot;
        public float speed = 100f;
        public float maxLifeTime = 3;
		public float timeToResetAfterHit = 1f;

		private bool _active;
		private Coroutine _maxLifeCo;
		private float _currentSpeed = -1f;

		private string _damageMethodName = "Damage";
		private LayerMask _layerMask;
		private int _damage = 16;
		private float _bulletForce = 100;
		private GameObject _bulletDecalPrefab;
		private TrailRenderer _trail;
		private LineRenderer _line;
		private Renderer[] _childRenderers;

		public string damageMethodName
		{
			set { _damageMethodName = value; }
		}
		public LayerMask layerMask
		{
			set { _layerMask = value; }
		}
		public int damage
		{
			set { _damage = value; }
		}
		public float bulletForce
		{
			set { _bulletForce = value; }
		}
		public GameObject bulletDecalPrefab
		{
			set { _bulletDecalPrefab = value; }
		}
		public LineRenderer getLine
		{
			get 
			{
				if (_line == null) _line = bulletRoot.GetComponentInChildren<LineRenderer>();
				return _line; 
			}
		}
		public TrailRenderer getTrail
		{
			get
			{
				if (_trail == null) _trail = bulletRoot.GetComponentInChildren<TrailRenderer>();
				return _trail; 
			}
		}
		public Renderer[] getChildRenderers
		{
			get
			{
				if (_childRenderers == null)
					_childRenderers = bulletRoot.GetComponentsInChildren<Renderer>();
				return _childRenderers;
			}
		}

        void OnEnable()
        {
			if (bulletRoot == null) bulletRoot = transform;
			StopAllCoroutines();
			_active = true;
			_currentSpeed = speed;
			ToggleRenderers(true);
			if (getLine != null)
			{
				getLine.SetPositions(new Vector3[0]);
				getLine.enabled = true;
			}
			if (getTrail != null)
			{
				getTrail.Clear();
				getTrail.enabled = true;
			}
			_maxLifeCo = StartCoroutine(MaxLife());
        }

		void Update()
		{
			if (!_active) return;

			RaycastHit hit;
			bool hitSomething = false;
			hitSomething = Physics.Raycast(bulletRoot.position, bulletRoot.forward, out hit, _currentSpeed * Time.deltaTime, _layerMask.value);
			
			if (hitSomething)
			{
				if (getLine != null) getLine.SetPositions(new Vector3[] {bulletRoot.position, hit.point});
				bulletRoot.position = hit.point;
				StartCoroutine(ApplyDamage(hit));
			} else
			{
				Vector3 oldPosition = bulletRoot.position;
				bulletRoot.Translate(Vector3.forward * Time.deltaTime * _currentSpeed);
				if (getLine != null) getLine.SetPositions(new Vector3[] {oldPosition, bulletRoot.position});
			}
		}

		IEnumerator ApplyDamage(RaycastHit hit)
        {
			hit.collider.SendMessageUpwards(_damageMethodName, _damage, SendMessageOptions.DontRequireReceiver);

			VRGunHandler.ApplyDecal(_bulletDecalPrefab, hit);

			ToggleRenderers(false);
			if (getLine != null) getLine.enabled = false;

			_active = false;

			if (_bulletForce > 0.001f)
			{
	            yield return null;
	            if (hit.rigidbody != null) hit.rigidbody.AddForceAtPosition(bulletRoot.forward * _bulletForce, hit.point, ForceMode.Impulse);
			}

			PoolingManager.instance.DestroyPoolObject(bulletRoot.gameObject, timeToResetAfterHit);

			if (_maxLifeCo != null) StopCoroutine(_maxLifeCo);

			yield return null;
        }

		IEnumerator MaxLife()
		{
			yield return new WaitForSeconds(maxLifeTime);
			PoolingManager.instance.DestroyPoolObject(bulletRoot.gameObject);
		}

		void ToggleRenderers(bool show)
		{
			foreach(Renderer renderer in getChildRenderers)
			{
				if (renderer.GetComponent<DontHide>() != null) continue;
				renderer.enabled = show;
			}
		}
    }
}
#endif