using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {

        public Camera playerCamera;
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset;
        
        private bool _isCameraStopped;
        public void StopCamera() => _isCameraStopped = true;
    }
}