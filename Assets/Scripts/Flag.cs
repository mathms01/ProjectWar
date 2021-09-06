using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : Entity
{
    public Flag(int posX, int posY)
    {
        this.posX = posX;
        this.posY = posY;
        this.fullLife = 1000;
        this.currentLife = fullLife;
        this.elementGameObject = (GameObject)Resources.Load("Prefabs/Flag", typeof(GameObject));
        this.elementGameObject.transform.position = new Vector3(posX, 5f, posY);
        Instantiate(this.elementGameObject);
    }
}
