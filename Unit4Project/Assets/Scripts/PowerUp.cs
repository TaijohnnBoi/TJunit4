using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType { None, Pushback, Rockets, Smash }

public class PowerUp : MonoBehaviour
{
    public PowerUpType powerUpType;
    public float hangTime; 
    public float smashSpeed; 
    public float explosionForce
    public float explosionRadius; 
    bool smashing = false;
    float floorY;
}
