using UnityEngine;

namespace _Project.Scripts.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Panel : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();
        }

        public virtual void OpenPopup()
        {
            _canvasGroup.alpha = 1F;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

        }

        public virtual void ClosePopup()
        {
            _canvasGroup.alpha = 0F;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}