using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    // Visual settings:
    float speed = 30f;
    float rotSpeed = 30f;

    //Animateur
    Animator animPlayer;

    public void CreatePlayer(int posX, int posY)
    {
        this.posX = posX;
        this.posY = posY;
        this.fullLife = 1000;
        this.currentLife = fullLife;
        this.damage = 100;
        this.elementGameObject = this.gameObject;
        this.animPlayer = this.elementGameObject.GetComponent<Animator>();
        this.elementGameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        this.elementGameObject.transform.position = new Vector3(posX, 1.5f, posY);
        //RandomChangeColor();
    }

    void Update() {
        UpdateAnim();
    }

    private void UpdateAnim()
    {
        float currentSpeed = this.elementGameObject.GetComponent<Rigidbody>().velocity.magnitude;
        this.animPlayer.SetFloat("speed", currentSpeed);
    }

    public void Move(Box boxToGo)
    {
        this.startPosX = this.posX;
        this.startPosY = this.posY;
        this.posX = boxToGo.posX;
        this.posY = boxToGo.posY;
        StartCoroutine("MoveAnim");
    }
}
