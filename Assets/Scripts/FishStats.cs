using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fish", menuName = "Fish")]
public class FishStats : ScriptableObject
{
    public float minSpeed;
    public float maxSpeed;

    public float fishSize;

    public float givesMoney;
}
