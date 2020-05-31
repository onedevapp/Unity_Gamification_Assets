using OneDevApp;
using UnityEngine;

public class BulletSpawner : MonoBehaviour {

    public GameObject bulletPrefab;
    public float fireRate;
    public bool usePool = false;

    private float nextFire;

    private void Update()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            if(usePool)
                spawnBullet();
            else
                Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        }
    }

    void spawnBullet()
    {
        //To get a specific item from the poolable list
        //PoolObjectItem item = PoolManagerScript.singleton.itemsToPool.Find(x => x.itemName == "laserRed");
        //PoolObjects _pooledObject = PoolManagerScript.singleton.GetObjectFromPool(item.objectToPool);

        //To a random item from the poolable list
        PoolObjects _pooledObject = PoolManager.Instance.GetObjectFromPool(PoolManager.Instance.itemsToPool[Random.value > 0.5 ? 0 : 1].objectToPool);

        //Enabling item from the pool
        PoolManager.Instance.EnableObjectFromPool(_pooledObject, PoolManager.Instance.parentGameObject.transform.position, PoolManager.Instance.parentGameObject.transform.rotation);
    }
}
