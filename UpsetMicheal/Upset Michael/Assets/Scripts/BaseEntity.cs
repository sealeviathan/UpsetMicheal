using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntity : MonoBehaviour
{
    int health = 100;
    int maxHealth = 100;
    int gas = 10;
    int maxGas = 10;
    public int Health 
    {
        get {return health;}
    }
    public int Gas
    {
        get {return gas;}
    }

    public void Damage(int damage)
    {
        health -= damage;
    }
    public void Heal(int amount)
    {
        health += amount;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
    }
    public void GasUp(int amount)
    {
        gas += amount;
        if(gas > maxGas)
        {
            gas = maxGas;
        }
    }
    public void ExpendGas(int amount)
    {
        gas -= amount;
    }
    public void Kill()
    {
        Destroy(gameObject);
    }
}
