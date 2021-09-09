using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : GameElement
{
    protected int fullLife;
    public int currentLife;

    public int damage;

    public bool isDestroyed = false;

    public void TakeDamage(int dmg)
    {
        if((currentLife - dmg) > 0)
        {
            Debug.Log("Taking Damage : "+currentLife);
            currentLife -= dmg;
        }
        else
        {
            currentLife = 0;
            if(isDestroyed == false)
            {
                DestroyCurrentEntity();
            }
            isDestroyed = true;
        }
    }

    public void DestroyCurrentEntity()
    {
        Debug.Log("Entity is destroyed !");
    }
}
