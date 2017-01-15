using UnityEngine;
using System.Collections;

namespace EliotScripts.ObjectPool
{
    public class ObjectPool : MonoBehaviour
    {
        private ArrayList objectPoolList;

        // Use this for initialization
        void Start()
        {
            objectPoolList = new ArrayList();
        }

        // Add to the object pool
        public void AddToPool(GameObject go)
        {
            objectPoolList.Add(go);
            go.SetActive(false);
        }

        // Search for object by tag and pull from the pool. If object isn't found, return null.
        public GameObject PullFromPool(string tag)
        {
            if (objectPoolList.Count == 0)
            {
                return null;
            } 
            foreach (GameObject item in objectPoolList)
            {
                if (item.CompareTag(tag))
                {
                    item.SetActive(true);
                    objectPoolList.Remove(item);
                    return item;
                }
            }
            return null;
        }

    }
}

