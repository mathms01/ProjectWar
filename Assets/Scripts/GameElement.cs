﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Element du jeu
/// </summary>
public class GameElement : MonoBehaviour
{
        //position de la case
    public int posX;
    public int posY;

    public int startPosX;
    public int startPosY;

    //gameobject de l'element
    public GameObject elementGameObject;

    /// <summary>
    /// Changer la couleur de l'élément en damier
    /// </summary>
    public void BoxChangeColor()
    {
        Color color;
        if ((this.posX + this.posY)%2 == 0)
            color = new Color(0.35f,0.35f,0.35f,1); 
        else
            color = new Color(0.5f,0.5f,0.5f,1); 
        
        elementGameObject.GetComponent<MeshRenderer>().material.color = color;
    }

    /// <summary>
    /// Changer la couleur de l'élément de manière random
    /// </summary>
    public void RandomChangeColor()
    {
        Color color;
        color = new Color(Random.Range(0f, 1f),Random.Range(0f, 1f),Random.Range(0f, 1f),1); 
        
        elementGameObject.GetComponent<MeshRenderer>().material.color = color;
    }
}
