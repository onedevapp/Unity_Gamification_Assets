/**
 * Source : http://www.theappguruz.com/blog/how-to-make-a-wheel-of-fortune-in-unity-the-easiest-way
 * Included Dynamic values, Probability of winniing least prize, In UI Canvas  
 **/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class ProportionalWheelItem
{
    public string name; 
    public int value; 
    public Sprite icon; 
    [Range (0,100)]
    public int chance; 
}


public class SpinWheel : MonoBehaviour
{
	public List<ProportionalWheelItem> prizeListItem;
	public List<SpinWheelItem> prizeListGO;
	public List<AnimationCurve> animationCurves;
    public Transform wheelTransform;
	private bool spinning;	
	private float anglePerItem;	
	private int randomTime;
	private int itemNumber;

    public List<ProportionalWheelItem> randomPrize;

    void Start()
    {        
        spinning = false;
        anglePerItem = 360 / prizeListItem.Count;

        var rnd = new System.Random();
        randomPrize = prizeListItem.OrderBy(item => rnd.Next()).ToList();

        for (int i = 0; i < prizeListGO.Count; i++)
        {
            SpinWheelItem wheelItem = prizeListGO[i];
            wheelItem.itemImage.sprite = randomPrize[i].icon;
            wheelItem.itemName.text = randomPrize[i].name;
        }

    }

    void  Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space) && !spinning) {
		
			randomTime = UnityEngine.Random.Range (1, 4);
            itemNumber = SelectItem(randomPrize);

            float maxAngle = 360 * randomTime + (itemNumber * anglePerItem);

            StartCoroutine (SpinTheWheel (5 * randomTime, maxAngle));
		}
	}
	
	IEnumerator SpinTheWheel (float time, float maxAngle)
	{
		spinning = true;
		
		float timer = 0.0f;		
		float startAngle = wheelTransform.eulerAngles.z;		
		maxAngle = maxAngle - startAngle;

        int animationCurveNumber = UnityEngine.Random.Range (0, animationCurves.Count);
		Debug.Log ("Animation Curve No. : " + animationCurveNumber);
		
		while (timer < time)
        {
		    //to calculate rotation
			float angle = maxAngle * animationCurves [animationCurveNumber].Evaluate (timer / time) ;
            wheelTransform.eulerAngles = new Vector3 (0.0f, 0.0f, angle + startAngle);
			timer += Time.deltaTime;
			yield return 0;
		}

        wheelTransform.eulerAngles = new Vector3 (0.0f, 0.0f, maxAngle + startAngle);
		spinning = false;
        
		Debug.Log ("Prize: " + randomPrize[itemNumber].value);//use prize[itemNumnber] as per requirement
	}


    public int SelectItem(List<ProportionalWheelItem> items)
    {
        System.Random rnd = new System.Random();

        // Calculate the summa of all portions.
        int poolSize = 0;
        for (int i = 0; i < items.Count; i++)
        {
            poolSize += items[i].chance;
        }

        // Get a random integer from 0 to PoolSize.
        int randomNumber = rnd.Next(0, poolSize) + 1;

        // Detect the item, which corresponds to current random number.
        int accumulatedProbability = 0;
        for (int i = 0; i < items.Count; i++)
        {
            accumulatedProbability += items[i].chance;
            if (randomNumber <= accumulatedProbability)
                return i;
        }

        return rnd.Next(0, items.Count);    // this code will never come while you use this programm right :)
    }
}
