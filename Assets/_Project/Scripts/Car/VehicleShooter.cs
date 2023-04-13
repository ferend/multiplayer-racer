using System;
using _Project.Scripts.Car.Weapon;
using UnityEngine;
using UnityEngine.Pool;

namespace _Project.Scripts.Car
{
    public class VehicleShooter : MonoBehaviour
    {
        [SerializeField] private Bullet _carBullet;
        private int _spawnAmount = 10;
        private ObjectPool<Bullet> _pool;
        float shootingTimer = 0;
        public float shootingCooldown = 0.2f;

        private void Start()
        {
            _pool = new ObjectPool<Bullet>(() =>
            {
                return Instantiate(_carBullet);
            }, bullet =>
            {
                bullet.gameObject.SetActive(true);
            }, bullet =>
            {
                bullet.gameObject.SetActive(false);
            }, bullet =>
            {
                Destroy(bullet.gameObject);
            }, false, 10, 20);
        }


        private void Update()
        {
            // If the shooting joystick is being used and the shooting timer is ready...
            if( UltimateJoystick.GetJoystickState( "Shooting" ) && shootingTimer <= 0)
            {
                // Then reset the timer and shoot a bullet.
                shootingTimer = shootingCooldown;
                ShootBullet();
            }
            
            // If the shoot timer is above zero, reduce it.
            if( shootingTimer > 0 )
                shootingTimer -= Time.deltaTime;

        }
        
        private void ShootBullet()
        {
            var currentBullet = _pool.Get();
            currentBullet.OnThrow();
            currentBullet.Init(KillShape);
        }

        private void KillShape(Bullet bullet)
        {
            _pool.Release(bullet);
        }
        
    }
}
