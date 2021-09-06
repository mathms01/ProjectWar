using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameElement : MonoBehaviour
{
        //position de la case
    public int posX;
    public int posY;

    //gameobject de l'element
    public GameObject elementGameObject;

    public void BoxChangeColor()
    {
        Color color;
        if ((this.posX + this.posY)%2 == 0)
            color = new Color(0.35f,0.35f,0.35f,1); 
        else
            color = new Color(0.5f,0.5f,0.5f,1); 
        
        elementGameObject.GetComponent<MeshRenderer>().material.color = color;
    }

    public void RandomChangeColor()
    {
        Color color;
        color = new Color(Random.Range(0f, 1f),Random.Range(0f, 1f),Random.Range(0f, 1f),1); 
        
        elementGameObject.GetComponent<MeshRenderer>().material.color = color;
    }
}
