using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OneDevApp
{
    [System.Serializable]
    public class PoolObjectItem
    {
        //Name for the Poolable item
        public string itemName;
        //Poolable item Gameobject 
        public PoolObjects objectToPool;
    }

    public class PoolManager : MonoInstance<PoolManager>
    {
        #region Public variables

        //this is the list that contains all the objects PREFABS that we want to pool
        [Tooltip("Assign all PoolObjectItem to prepool items before actual usage")]
        public List<PoolObjectItem> itemsToPool;

        [Tooltip("The parent game object for all poolable items")]
        public GameObject parentGameObject;
        #endregion


        #region Private variables
        //Dictionary containig a key for each PoolObject and the list of PoolObjects that might be used.
        private Dictionary<int, List<PoolObjects>> poolDictionary = new Dictionary<int, List<PoolObjects>>();
        #endregion


        private void Start()
        {

            if (itemsToPool.Count == 0)
            {
                Debug.LogError("Please make sure that you set the list of poolable objects.");
            }

            foreach (PoolObjectItem item in itemsToPool)
            {
                if (item == null)
                {
                    Debug.LogWarning("PoolObjectItem object: " + item.ToString() + " is null.");
                }
                else
                {
                    AddObjectToPoolDictionary(item.objectToPool, 1);
                }
            }
        }

        /// <summary>
        /// Adds a new pool object to our dictionary.
        /// This is called from GameplayManager or from Destroyable objects that have debris.
        /// </summary>
        /// <param name="newPoolObject">The new to pool object.</param>
        public void AddObjectToPoolDictionary(PoolObjects newPoolObject, int size)
        {
            //Lets make sure that we don't have any pool object with this id.
            int poolKey = newPoolObject.GetInstanceID();

            if (poolDictionary.ContainsKey(poolKey) == false)
            {
                //If we don't have the id of this pool object in our pool manager 
                //Then just create a new entry for our dictionary
                //We will Instantiate one object and add it to the list
                //You can extend it and spawn like 10 or more objects
                List<PoolObjects> newPoolObjectsList = new List<PoolObjects>();
                for (int i = 0; i < size; i++)
                {
                    InstantiateNewPoolObject(newPoolObject, newPoolObjectsList);
                }
                poolDictionary.Add(poolKey, newPoolObjectsList);
            }
            else
            {
                Debug.LogWarning("You've tried to add a new PoolObject to our dictionary, but it already contains the object.");
            }
        }

        /// <summary>
        /// Get the selected pool object from an existing pool and enables it.
        /// </summary>
        /// <param name="key">The pool key.</param>
        /// <returns></returns>
        public GameObject GetNewFromPool(string key)
        {
            return EnableObjectFromPool(GetObjectFromPool(key));
        }

        /// <summary>
        /// Get the selected pool object from an existing pool.
        /// </summary>
        /// <param name="key">The pool key.</param>
        /// <returns></returns>
        public PoolObjects GetObjectFromPool(string key)
        {
            PoolObjectItem item = itemsToPool.Where(x => x.itemName == key).FirstOrDefault();
            return GetObjectFromPool(item);
        }

        /// <summary>
        /// Get the selected pool object from an existing pool.
        /// </summary>
        /// <param name="poolObject">The pool object.</param>
        /// <returns></returns>
        public PoolObjects GetObjectFromPool(PoolObjectItem item)
        {
            //Get the pool key for this object.
            int poolKey = item.objectToPool.GetInstanceID();

            if (poolDictionary.ContainsKey(poolKey))
            {
                //Get the list
                List<PoolObjects> poolObjectsList = poolDictionary[poolKey];

                //If the object exists return the first available one.
                //If we don't find any abailable object we will instantiate a new one.
                //This is why it is very important to disable the PoolObjects
                for (int i = 0; i < poolObjectsList.Count; i++)
                {
                    if (poolObjectsList[i].active == false)
                        return poolObjectsList[i];
                }

                //Now, spawn a new one, if we don't find anyone
                InstantiateNewPoolObject(item.objectToPool, poolObjectsList);

                //return the last object added to the list
                return poolObjectsList[poolObjectsList.Count - 1];
            }
            else
            {
                Debug.LogError("You are trying to get a PoolObject that doesn't exist in our poolDictionary. Please make sure that you add the pool object before trying to get them.");
                return null;
            }
        }

        /// <summary>
        /// Instantiates a new PoolObject and adds it to the correct list.
        /// </summary>
        /// <param name="poolObjectToSpawn">Object to instnatiate.</param>
        /// <param name="poolListToAdd">List to add.</param>
        private void InstantiateNewPoolObject(PoolObjects poolObjectToSpawn, List<PoolObjects> poolListToAdd)
        {
            //New poolObject
            PoolObjects newPoolObject = (PoolObjects)Instantiate(poolObjectToSpawn, Vector3.zero, Quaternion.identity);
            //First, we set it to be disabled
            newPoolObject.DisablePoolObject();
            newPoolObject.transform.parent = parentGameObject.transform;
            //Add to the pool list
            poolListToAdd.Add(newPoolObject);
        }

        /// <summary>
        /// Disables all pool object selected.
        /// This is called, for example, when the player chooses to continue the game.
        /// </summary>
        /// <param name="poolObject">The pool object to disable</param>
        public void DisablePoolObjectList(PoolObjects poolObject)
        {
            //Get the pool key of this object
            int poolKey = poolObject.GetInstanceID();

            //If it exists, then for each PoolObject in the list, disable it
            if (poolDictionary.ContainsKey(poolKey) == true)
            {
                for (int i = 0; i < poolDictionary[poolKey].Count; i++)
                {
                    poolDictionary[poolKey][i].DisablePoolObject();
                }
            }
            else
            {
                Debug.LogWarning("You've tried to disable all pool objects but you didn't add the current pool object to the dictionary. Pool object: " + poolObject.name);
            }
        }

        /// <summary>
        /// Disables pool object selected.
        /// </summary>
        /// <param name="poolObject">The pool object to disable</param>
        public void DisablePoolObjectItem(PoolObjects poolObject)
        {
            poolObject.DisablePoolObject();
        }

        #region Helper Methods to Enable Object From Pool
        /// <summary>
        /// Enable pool object
        /// </summary>
        /// <param name="poolObject">The pool object to disable</param>
        public GameObject EnableObjectFromPool(PoolObjects poolObject)
        {
            GameObject newObject = poolObject.gameObject;
            poolObject.ActivatePoolObject();
            return newObject;
        }

        /// <summary>
        /// Enable pool object
        /// </summary>
        /// <param name="poolObject">The pool object to be enabled</param>
        /// <param name="itemPosition">The pool object position</param>
        /// <param name="itemRotation">The pool object rotation</param>
        public GameObject EnableObjectFromPool(PoolObjects poolObject, Vector2 itemPosition, Quaternion itemRotation)
        {
            GameObject newObject = poolObject.gameObject;
            newObject.transform.position = itemPosition;
            newObject.transform.rotation = itemRotation;
            poolObject.ActivatePoolObject();
            return newObject;
        }

        /// <summary>
        /// Enable pool object
        /// </summary>
        /// <param name="poolObject">The pool object to be enabled</param>
        /// <param name="itemPosition">The pool object position</param>
        /// <param name="itemRotation">The pool object rotation</param>
        /// <param name="myParent">The pool object to be child of gameobject</param>
        public GameObject EnableObjectFromPool(PoolObjects poolObject, Vector2 itemPosition, Quaternion itemRotation, GameObject myParent)
        {
            GameObject newObject = poolObject.gameObject;
            newObject.transform.position = itemPosition;
            newObject.transform.rotation = itemRotation;
            newObject.transform.parent = myParent.transform;
            poolObject.ActivatePoolObject();
            return newObject;
        }

        /// <summary>
        /// Enable pool object
        /// </summary>
        /// <param name="poolObject">The pool object to be enabled</param>
        /// <param name="myParent">The pool object to be child of gameobject</param>
        public GameObject EnableObjectFromPool(PoolObjects poolObject, GameObject myParent)
        {
            GameObject newObject = poolObject.gameObject;
            newObject.transform.position = myParent.transform.position;
            newObject.transform.rotation = myParent.transform.rotation;
            newObject.transform.parent = myParent.transform;
            poolObject.ActivatePoolObject();
            return newObject;
        }
        #endregion
    }

}