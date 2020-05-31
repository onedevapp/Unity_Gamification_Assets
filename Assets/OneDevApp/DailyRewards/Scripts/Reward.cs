using OneDevApp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* The class representation of the Reward
**/
[Serializable]
public class Reward
{
    public string name;
    public int reward;
    public Sprite sprite;
    public DailyRewardState state = DailyRewardState.UNCLAIMED_UNAVAILABLE;
}
