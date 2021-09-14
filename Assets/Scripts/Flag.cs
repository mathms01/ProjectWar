using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe du drapeau
/// </summary>
public class Flag : Entity
{
    /// <summary>
    /// Initialisation du drapeau
    /// </summary>
    /// <param name="posX">position x</param>
    /// <param name="posY">position y</param>
    public void CreateFlag(int posX, int posY)
    {
        this.posX = posX;
        this.posY = posY;
        this.fullLife = 1000;
        this.currentLife = fullLife;
        this.elementGameObject = this.gameObject;
        this.elementGameObject.transform.position = new Vector3(posX, 3f, posY);
    }
}
