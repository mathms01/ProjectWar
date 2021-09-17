using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Classe d'une entité (élément du jeu pouvant être détruit)
/// </summary>
public class Entity : GameElement
{
    public int fullLife;
    public int currentLife;
    public Slider healthBar;

    public int damage;
    public int goldValue = 0;

    public bool isDestroyed = false;

    /// <summary>
    /// Infliger des dégats à l'objet
    /// </summary>
    /// <param name="dmg">puissance de l'attaque</param>
    /// <param>gold à retourner</param>
    public int TakeDamage(int dmg, GameObject source)
    {
        if((currentLife - dmg) > 0)
        {
            currentLife -= dmg;
            if(healthBar != null)
                healthBar.value = currentLife;
        }
        else
        {
            currentLife = 0;
            if(healthBar != null)
                healthBar.value = currentLife;

            if(isDestroyed == false)
            {
                DestroyCurrentEntity(source);
                isDestroyed = true;
                return goldValue;
            }
        }
        return 0;
    }

    /// <summary>
    /// Destruction de l'entité
    /// </summary>
    public void DestroyCurrentEntity(GameObject source)
    {
        
    }
}
