using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    // Visual settings:
    float speed = 30f;
    float rotSpeed = 30f;

    public void CreatePlayer(int posX, int posY)
    {
        this.posX = posX;
        this.posY = posY;
        this.fullLife = 1000;
        this.currentLife = fullLife;
        this.damage = 100;
        this.elementGameObject = this.gameObject;
        this.elementGameObject.transform.localScale = new Vector3(1f, 1.5f, 1f);
        this.elementGameObject.transform.position = new Vector3(posX, 2f, posY);
        //RandomChangeColor();
    }

    public void Move(Box boxToGo)
    {
        this.startPosX = this.posX;
        this.startPosY = this.posY;
        this.posX = boxToGo.posX;
        this.posY = boxToGo.posY;
        StartCoroutine("MoveAnim");
    }

    IEnumerator MoveAnim()
    {
        while (this.elementGameObject.transform.position != new Vector3(posX, 2f, posY))
        {
            this.elementGameObject.transform.position = Vector3.Lerp(this.elementGameObject.transform.position, new Vector3(posX, 2f, posY), Time.deltaTime * speed);
            Quaternion rotateDirection = Quaternion.LookRotation(new Vector3(posX, 2f, posY) - this.elementGameObject.transform.position);
            this.elementGameObject.transform.rotation = Quaternion.Lerp(this.elementGameObject.transform.rotation, rotateDirection, Time.deltaTime * rotSpeed);
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(10f);
    }
}
