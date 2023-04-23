namespace Utility
{
    using UnityEngine;

    /// <summary>
    /// A Static instance is similar to singleton, but instead of destroying ant new instances, it overrides the current instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }
        protected virtual void Awake() => Instance = this as T;

        protected virtual void OnApplicationQuit()
        {
            Instance = null;
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// This transforms the static instance to into basic singleton. This will destroy any new versions created.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            if(Instance !=null) Destroy(gameObject);
            base.Awake();
        }
    }

    /// <summary>
    /// Persistent version of the singleton. This wil survive through scene loads.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}