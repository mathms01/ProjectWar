using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe Warrior -> Monstres du jeu
/// </summary>
public class Warrior : Entity
{
    public int DETECTENEMYDISTANCE = 15;
    public int ATTACKRANGE = 3;

    public Entity target;

    // Visual settings:
    float speed = 30f;
    float rotSpeed = 30f;

    /// <summary>
    /// Initialisation du Warrior
    /// </summary>
    /// <param name="posX">position X</param>
    /// <param name="posY">position Y</param>
    public void CreateWarrior(int posX, int posY)
    {
        this.posX = posX;
        this.posY = posY;
        this.fullLife = 10;
        this.currentLife = fullLife;
        this.damage = 20;
        this.elementGameObject = this.gameObject;
        this.elementGameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        this.elementGameObject.transform.position = new Vector3(posX, 1f, posY);
        //RandomChangeColor();
    }

    /// <summary>
    /// Déplacement du Warrior
    /// </summary>
    /// <param name="boxToGo">box de target</param>
    public void Move(Box boxToGo){
        this.startPosX = this.posX;
        this.startPosY = this.posY;
        this.posX = boxToGo.posX;
        this.posY = boxToGo.posY;
        StartCoroutine("MoveAnim");
    }

    /// <summary>
    /// Animation de déplacement et déplacement
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveAnim(){
         while( this.elementGameObject.transform.position != new Vector3(posX, 1f, posY))
         {
            this.elementGameObject.transform.position = Vector3.Lerp(this.elementGameObject.transform.position, new Vector3(posX, 1f, posY), Time.deltaTime * speed);
            Vector3 rotateDirectionVector = new Vector3(posX, 1f, posY) - this.elementGameObject.transform.position;
            Quaternion rotateDirection = (rotateDirectionVector != Vector3.zero) ?  Quaternion.LookRotation(new Vector3(posX, 1f, posY) - this.elementGameObject.transform.position) : Quaternion.identity;
            this.elementGameObject.transform.rotation = Quaternion.Lerp(this.elementGameObject.transform.rotation, rotateDirection, Time.deltaTime * rotSpeed);
            yield return new WaitForSeconds(1f);
         }
        yield return new WaitForSeconds(10f);
    }

    /// <summary>
    /// Modifie la target du Warrior
    /// </summary>
    /// <param name="newTarget"></param>
    public void ChangeTarget(Entity newTarget)
    {
        this.target = newTarget;
    }
}
