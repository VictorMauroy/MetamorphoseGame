using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType{
    Slime,
    Golem,
    Ondine,
    SacredFirefly,
    MiniWizard,
    Human
}
public class CharacterProperties : MonoBehaviour
{
    public float walkSpeed;
    public float runSpeedAdd;
    public float life;
    public float jumpForce;

    [Header("Enter the type of your Monster")]
    public EntityType _monsterType;
}
