using UnityEngine;

namespace _Project.Scripts.Car.Weapon
{
    public interface IThrowable
    {
        void OnThrow();
        void OnCollide();
    }
}