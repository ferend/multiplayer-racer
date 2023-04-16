using UnityEngine;

namespace _Project.Scripts.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIScreen : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        protected float openDuration = 0.5F;
        protected float closeDuration = 0.5F;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
    
        public virtual void OpenScreen()
        {
            OnScreenOpen();
            gameObject.SetActive(true);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public virtual void CloseScreen()
        {
            OnScreenClose();
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        protected abstract void OnScreenOpen();
        protected abstract void OnScreenClose();

    }
}