using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe du joueur
/// </summary>
public class Player : Entity
{
    // Visual settings:
    public float speed = 7f;
    float rotSpeed = 30f;
    public int golds = 0;

    //Animateur
    Animator animPlayer;

    //Triggers
    private GameObject attackZone;
    private bool isAttacking = false;

    /// <summary>
    /// Initialisation du Joueur
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    public void CreatePlayer(int posX, int posY)
    {
        this.posX = posX;
        this.posY = posY;
        this.fullLife = 1000;
        this.currentLife = fullLife;
        this.damage = 100;
        this.elementGameObject = this.gameObject;
        this.animPlayer = this.elementGameObject.GetComponent<Animator>();
        this.attackZone = this.elementGameObject.gameObject.transform.Find("AttackZone").gameObject;
        this.attackZone.SetActive(false);
        this.elementGameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        this.elementGameObject.transform.position = new Vector3(posX, 1.5f, posY);
        //RandomChangeColor();
    }

    void Update() {
        UpdateAnim();
        Move();
    }

    /// <summary>
    /// MAJ de l'animation du joueur
    /// </summary>
    private void UpdateAnim()
    {
        float currentSpeed = this.elementGameObject.GetComponent<Rigidbody>().velocity.magnitude;
        this.animPlayer.SetFloat("speed", currentSpeed);

        if(this.isDestroyed == true)
            this.animPlayer.SetTrigger("dead");
    }

    public void AttackAnim()
    {
        this.animPlayer.SetTrigger("attack");
        if(isAttacking == false)
            StartCoroutine("ShowAttackEffect");
    }

    /// <summary>
    /// Jouer une vague
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowAttackEffect()
    {
        isAttacking = true;
        this.attackZone.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        this.attackZone.SetActive(false);
        isAttacking = false;
    }

    //Déplacement du joueur
    public void Move()
    {
        this.startPosX = this.posX;
        this.startPosY = this.posY;
        this.posX = (int)this.elementGameObject.transform.position.x;
        this.posY = (int)this.elementGameObject.transform.position.z;
    }

    private void OnTriggerEnter(Collider otherObject)
    {
        if(isAttacking == true && otherObject.gameObject.GetComponent<Warrior>() != null)
        {
            golds += otherObject.gameObject.GetComponent<Warrior>().TakeDamage(this.damage, this.gameObject);
        }
    }
}
