using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Car.Weapon
{
    public class Bullet : MonoBehaviour, IThrowable
    {
        public CarController target;
        public float rad = 1;
        public float height = 1;
        private event Action<Bullet> _killAction; 
        private void Awake()
        {
            target = GameObject.FindObjectOfType<CarController>();
        }

        public void Init(Action<Bullet> killAction)
        {
            _killAction = killAction;
        }

        public void OnThrow()
        {
            transform.Translate(this.transform.forward * Time.deltaTime * 100f);
        }

        public void OnCollide()
        {
            target.TakeDamage();
        }

        private void Update()
        {
            if (Contains(target.transform.position))
            {
                OnCollide();
                _killAction?.Invoke(this);
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.matrix = Handles.matrix = transform.localToWorldMatrix;
            Gizmos.color = Handles.color = Contains(target.transform.position) ? Color.red : Color.white;

            Vector3 top = new Vector3(0, height, 0);

            Handles.DrawWireDisc(Vector3.zero, Vector3.up, rad);
            Handles.DrawWireDisc(top, Vector3.up, rad);

        }

        public bool Contains(Vector3 pos)
        {
            Vector3 dirToTargetWorld = (pos - transform.position);
            Vector3 vecToTarget = transform.InverseTransformVector(dirToTargetWorld);

            //height check
            if (vecToTarget.y < 0 || vecToTarget.y > height)
            {
                return false;
            }

            Vector3 flatDirToTarget = vecToTarget;
            flatDirToTarget.y = 0;
            float flatDistance = flatDirToTarget.magnitude;

            // check if inside or outside
            if (flatDistance > rad)
            {
                return false;
            }

            return true;
        }
    }
}