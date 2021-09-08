using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : GameElement
{
    //Gameplay
    public bool isFull;

    public void CreateBox(int posX, int posY)
    {
        this.posX = posX;
        this.posY = posY;
        this.elementGameObject = this.gameObject;
        this.elementGameObject.transform.position = new Vector3(posX, 1f, posY);
        BoxChangeColor();
        isFull = false;
    }
}
