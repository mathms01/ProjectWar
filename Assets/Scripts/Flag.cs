using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : Entity
{
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
