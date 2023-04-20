using UnityEngine;
using Utility;

namespace _Project.Scripts.Managers
{
    public class Services : Singleton<Services>
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private GameFlowManager gameFlowManager;

        public UIManager UIManager => uiManager;
        public GameFlowManager GameFlowManager => gameFlowManager;
    }
}