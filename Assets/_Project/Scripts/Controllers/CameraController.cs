using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {

        [SerializeField] private Camera _playerCamera;
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset;


        

        private void Follow()
        {
            if (_target == null || _isCameraStopped) return;
            _playerCamera.transform.position = new Vector3(Mathf.Lerp(_playerCamera.transform.position.x,
            _target.transform.position.x, Constants.cameraPosXLerpSpeed * Time.deltaTime), _target.transform.position.y + _offset.y, _target.transform.position.z + _offset.z);
        }

        private bool _isCameraStopped;
        public void StopCamera() => _isCameraStopped = true;
    }
}