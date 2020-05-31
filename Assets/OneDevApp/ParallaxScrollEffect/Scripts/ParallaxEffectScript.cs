using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OneDevApp
{
    public enum BoundToCameraType
    {
        [Tooltip("Dont bind the sprite to the screen ")]
        DontBind,
        [Tooltip("Bind the sprite to the screen edge")]
        BindToScreen,
        [Tooltip("Random bind the sprite to the screen edge")]
        RandomBindToSreen,
    }
    public class ParallaxEffectScript : MonoBehaviour
    {
        #region Public variables
        [Tooltip("The speed at which the parallax moves, which will likely be opposite from the speed at which your character moves.")]
        public float playerSpeed = 0.0f;

        [Tooltip("The speed at which this object moves in relation to the speed of the parallax.")]
        [Range(0.0f, 1.0f)]
        public float SpeedRatio;

        [Tooltip("Is vertical or horizontal movement of the parallax.")]
        public bool IsHorizontalScroll = true;

        [Tooltip("Is movement of the parallax is omni-directional or bi-direction.")]
        public bool IsBidirectionalMovement = true;

        [Tooltip("Is the sprites need to clone, best used along with wrap sprite")]
        public bool enableCloneObject = false;

        [Tooltip("Is Sprites need to warp each other in the parallax.")]
        public bool WrapSprite = true;

        [Tooltip("The overlap in world units for wrapping elements. This can help fix rare one pixel gaps.")]
        public float WrapOverlap = 0.0f;

        [Tooltip("Does the sprites need to bound to the camera in parallax.")]
        public BoundToCameraType BoundToCamera = BoundToCameraType.DontBind;
        #endregion


        #region Private variables
        //Used to sort each renderer based on position
        internal List<Renderer> GameObjectRenderers = new List<Renderer>();
        //Main Camera used in the scene required for binding sprite
        private Camera mainCamera;

        //Dummy Player speed
        private Hint _playerHint;
        #endregion

        void Start()
        {
            mainCamera = Camera.main;
            _playerHint = FindObjectOfType<Hint>();

            #region Cloning gameobject 
            //Is clone object is enabled, we need to clone each and every sprite.
            if (enableCloneObject)
            {
                //Get all child game object which contains renderer and only active objects
                GameObject[] childs = new GameObject[transform.childCount];
                //Looping through the each child
                for (int i = 0; i < childs.Length; i++)
                {
                    //Get the child object
                    GameObject child = transform.GetChild(i).gameObject;
                    //Get the renderer object of that child
                    Renderer r = child.GetComponent<Renderer>();

                    // Add only the visible children or gameobject with renderer components only
                    if (!child.activeSelf || r == null)
                        continue;

                    childs[i] = child; // Add the game object to the array.

                    //Check whether the renderer needs to bind to the camera
                    if (BoundToCamera != BoundToCameraType.DontBind)
                    {
                        //Get the screen bound size
                        float vertExtent = mainCamera.orthographicSize;
                        float horzExtent = vertExtent * Screen.width / Screen.height;
                        float leftBound = (float)(horzExtent - r.bounds.size.x / 2.0f);
                        float rightBound = (float)(r.bounds.size.x / 2.0f - horzExtent);
                        float bottomBound = (float)(vertExtent - r.bounds.size.y / 2.0f);
                        float topBound = (float)(r.bounds.size.y / 2.0f - vertExtent);

                        //get the child position
                        var pos = child.transform.position;

                        //If the movement is horizontal then child need to position at left cornor  
                        if (IsHorizontalScroll)
                        {
                            //Gets the minimum position to bound
                            pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
                            //If random bind then get the random value of the position
                            if (BoundToCamera == BoundToCameraType.RandomBindToSreen)
                                pos.x = Random.Range(0, pos.x / 2);
                            //transforming the child position to the calculated position
                            child.transform.position = new Vector3(pos.x, child.transform.position.y, child.transform.position.z);
                        }
                        else   //if the movement is vertical then the child needs to position at bottom cornor
                        {
                            pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
                            //If random bind then get the random value of the position
                            if (BoundToCamera == BoundToCameraType.RandomBindToSreen)
                                pos.y = Random.Range(0, pos.y / 2);
                            //transforming the child position to the calculated position
                            child.transform.position = new Vector3(child.transform.position.x, pos.y, child.transform.position.z);
                        }

                    }
                }

                for (int i = 0; i < childs.Length; i++)
                {
                    if (childs[i] == null) continue;

                    Transform child = childs[i].gameObject.transform;
                    Renderer r = child.GetComponent<Renderer>();
                    //Create a clone for filling rest of the screen
                    GameObject objectCopy = GameObject.Instantiate(child.gameObject);
                    //Set clone parent and position
                    objectCopy.transform.SetParent(transform);

                    //Get the position of the child object
                    Vector3 rSize = (r.bounds.max - r.bounds.min);
                    if (IsHorizontalScroll)
                    {
                        //Transforming the child clone object next to the child object
                        objectCopy.transform.position = new Vector3(child.transform.position.x + rSize.x, child.transform.position.y, child.transform.position.z);
                    }
                    else
                    {
                        //Transforming the child clone object above to the child object
                        objectCopy.transform.position = new Vector3(child.transform.position.x, child.transform.position.y + rSize.y, child.transform.position.z);
                    }
                }
            }
            #endregion

            #region Sorting gameobject based on position 
            // Get all the children of the layer with a renderer
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                Renderer r = child.GetComponent<Renderer>();

                // Add only the visible children
                if (child.gameObject.activeSelf && r != null)
                {
                    GameObjectRenderers.Add(r);
                }
            }

            // Sort by position.
            // Note: Get the children from left to right.
            // We would need to add a few conditions to handle
            // all the possible scrolling directions.
            GameObjectRenderers = GameObjectRenderers.OrderBy(
              t => t.transform.position.x
            ).ToList();
            #endregion
        }

        /// <summary>
        /// Set the position of an object such that the bottom left of the object ends up being x and y
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="r">Renderer</param>
        /// <param name="x">x bottom left</param>
        /// <param name="y">y bottom left</param>
        public void ResetPosition(GameObject obj, Renderer r, float x, float y)
        {
            Vector3 pos = new Vector3(x, y, obj.transform.position.z);
            obj.transform.position = pos;

            float xOffset = r.bounds.min.x - obj.transform.position.x;
            if (xOffset != 0)
            {
                pos.x -= xOffset;
                obj.transform.position = pos;
            }
            float yOffset = r.bounds.min.y - obj.transform.position.y;
            if (yOffset != 0)
            {
                pos.y -= yOffset;
                obj.transform.position = pos;
            }

        }

        void Update()
        {
            #region Movement calculation

            playerSpeed = _playerHint.playerSpeed;

            //If bidirectional movement, we can use player speed as it is else need to get the absoulte value of the player speed.
            float t = Time.deltaTime * (IsBidirectionalMovement ? playerSpeed : -Mathf.Abs(playerSpeed));

            for (int i = 0; i < transform.childCount; i++)
            {
                //Get the child object
                GameObject child = transform.GetChild(i).gameObject;
                // move everything first based on speed ratio assigned to it.
                if (IsHorizontalScroll)
                {
                    transform.GetChild(i).Translate(t * SpeedRatio, 0.0f, 0.0f);
                }
                else
                {
                    transform.GetChild(i).Translate(0.0f, t * SpeedRatio, 0.0f);
                }
            }

            /*if (IsHorizontalScroll)
            {
                transform.Translate(t * SpeedRatio, 0.0f, 0.0f);
            }
            else
            {
                transform.Translate(0.0f, t * SpeedRatio, 0.0f);
            }*/
            #endregion


            #region Renderering calculation
            //Get the first and last renderer objects from the sorted list
            Renderer firstChild = GameObjectRenderers.FirstOrDefault();
            Renderer lastChild = GameObjectRenderers.LastOrDefault();
            //Get the current renderer object based on direction. i.e if t is greater than zero then last object will be visible to the screen else first object will be visible
            Renderer r = (t > 0 ? lastChild : firstChild);
            GameObject obj = r.gameObject;

            //Get the min and max value of the screen.
            float minEdge, maxEdge;
            if (IsHorizontalScroll)
            {
                minEdge = mainCamera.rect.x;
                maxEdge = mainCamera.rect.width;
            }
            else
            {
                minEdge = mainCamera.rect.y;
                maxEdge = mainCamera.rect.height;
            }

            Bounds b = r.bounds;
            //Get the position of the screen edge
            Vector3 screenEdge = (t > 0 ? mainCamera.WorldToViewportPoint(b.min) : mainCamera.WorldToViewportPoint(b.max));
            float screenEdgeValue = (IsHorizontalScroll ? screenEdge.x : screenEdge.y);

            //Moving Left (Scrolling Right)
            if (t > 0 && screenEdgeValue >= maxEdge)
            {
                if (IsHorizontalScroll)
                {
                    // move to the back of the line at far left
                    float newX = (firstChild.bounds.min.x - r.bounds.size.x) + WrapOverlap;
                    //If not wrap sprite then the next position will be after the screen bounds
                    if (!WrapSprite)
                    {
                        float horzExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
                        newX -= horzExtent;
                    }
                    //Reset the renderer to the new position
                    ResetPosition(obj, r, newX, r.bounds.min.y);
                }
                else
                {
                    // move to the back of the line at far bottom
                    float newY = (firstChild.bounds.min.y - r.bounds.size.y) + WrapOverlap;
                    //If not wrap sprite then the next position will be after the screen bounds
                    if (!WrapSprite)
                    {
                        float horzExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
                        newY -= horzExtent;
                    }
                    //Reset the renderer to the new position
                    ResetPosition(obj, r, r.bounds.min.x, newY);
                }
                //To maintain the sorting order of the object
                GameObjectRenderers.Remove(r);
                GameObjectRenderers.Insert(0, r);
            }
            else if (t < 0 && screenEdgeValue <= minEdge)//Moving Right (Sprite Scrolling Left)
            {
                if (IsHorizontalScroll)
                {
                    // move to the front of the line at far right
                    float newX = (lastChild.bounds.max.x) - WrapOverlap;
                    //If not wrap sprite then the next position will be after the screen bounds
                    if (!WrapSprite)
                    {
                        float horzExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
                        newX += horzExtent;
                    }
                    //Reset the renderer to the new position
                    ResetPosition(obj, r, newX, r.bounds.min.y);
                }
                else
                {
                    // move to the front of the line at far top
                    float newY = (lastChild.bounds.max.y) - WrapOverlap;
                    //If not wrap sprite then the next position will be after the screen bounds
                    if (!WrapSprite)
                    {
                        float horzExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
                        newY += horzExtent;
                    }
                    //Reset the renderer to the new position
                    ResetPosition(obj, r, r.bounds.min.x, newY);
                }
                //To maintain the sorting order of the object
                GameObjectRenderers.Remove(r);
                GameObjectRenderers.Add(r);
            }
            #endregion
        }
    }
}