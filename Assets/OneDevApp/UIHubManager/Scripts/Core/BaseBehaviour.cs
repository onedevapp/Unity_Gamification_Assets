using UnityEngine;

namespace OneDevApp
{
    public class BaseBehaviour : MonoBehaviour
    {
        protected IGameService GameService { get; private set; }

        protected virtual void Awake() { }

        protected virtual void Start()
        {
            GameService = ServiceLocator.GetService<IGameService>();
        }

        protected virtual void OnDestroy() { }
    }

}