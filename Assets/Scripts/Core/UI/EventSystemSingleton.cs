using UnityEngine;
using UnityEngine.EventSystems;

namespace HFantasy.Script.Core.UI
{
    [RequireComponent(typeof(EventSystem))]
    public class EventSystemSingleton : MonoBehaviour
    {
        private void Awake()
        {
            var eventSystems = FindObjectsOfType<EventSystem>();
            if (eventSystems.Length > 1)
            {
                for (int i = 1; i < eventSystems.Length; i++)
                {
                    if (eventSystems[i].gameObject != gameObject)
                    {
                        Destroy(eventSystems[i].gameObject);
                    }
                }
            }
        }
    }
}