using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Entity
{
    public Entity target;

    public Warrior(int posX, int posY)
    {
        this.posX = posX;
        this.posY = posY;
        this.fullLife = 10;
        this.currentLife = fullLife;
        this.damage = 100;
        this.elementGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        this.elementGameObject.transform.localScale = new Vector3(1f, 1.5f, 1f);
        this.elementGameObject.transform.position = new Vector3(posX, 2f, posY);
        RandomChangeColor();
    }

    public void Move(Box boxToGo){
        this.posX = boxToGo.posX;
        this.posY = boxToGo.posY;
        StartCoroutine("MoveAnim");
    }

    IEnumerator MoveAnim(){
        this.elementGameObject.transform.position = Vector3.Lerp(this.elementGameObject.transform.position, new Vector3(posX, 2f, posY), Time.deltaTime * 10f);
        yield return new WaitForSeconds(.1f);
    }

    public void ChangeTarget(Entity newTarget)
    {
        this.target = newTarget;
    }
}
