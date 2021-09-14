using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe d'une entité (élément du jeu pouvant être détruit)
/// </summary>
public class Entity : GameElement
{
    protected int fullLife;
    public int currentLife;

    public int damage;

    public bool isDestroyed = false;

    /// <summary>
    /// Infliger des dégats à l'objet
    /// </summary>
    /// <param name="dmg">puissance de l'attaque</param>
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

    /// <summary>
    /// Destruction de l'entité
    /// </summary>
    public void DestroyCurrentEntity()
    {
        Debug.Log("Entity is destroyed !");
    }
}
