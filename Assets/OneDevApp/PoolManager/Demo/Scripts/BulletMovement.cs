using OneDevApp;
using UnityEngine;

public class BulletMovement : MonoBehaviour {


    //Main Camera used in the scene required for binding sprite
    private Camera mainCamera;
    private Renderer r;
    void Start()
    {
        mainCamera = Camera.main;
        r = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update () {

        float maxEdge = mainCamera.rect.height;
        Bounds b = r.bounds;
        //Get the position of the screen edge
        Vector3 screenEdge = mainCamera.WorldToViewportPoint(b.min);
        float screenEdgeValue = screenEdge.y;
        if (screenEdgeValue >= maxEdge)
        {
            //GetComponent<PoolObjects>().DisablePoolObject();
            PoolManager.Instance.DisablePoolObjectItem(GetComponent<PoolObjects>());
        }
        else
        {
            this.transform.Translate(Vector2.up);
        }
    }
}
