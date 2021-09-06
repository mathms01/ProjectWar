using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static int DESKSIZE = 50;
    public static int WARRIORSATSTART = 20;

    public static float TIMEBETWEENROUNDS = 1f;
    public static float TIMEBETWEENMOVES = .1f;

    public int ROUND = 0;

    public List<Box> boxes;

    public List<Warrior> warriors;
    public List<Team> teams;

    private Flag flag;

    // Start is called before the first frame update
    void Start()
    {
        InitiateGame();
    }

    private void InitiateGame()
    {
        boxes = new List<Box>();
        warriors = new List<Warrior>();

        for (int i = 0; i < DESKSIZE; i++)
        {
            for (int j = 0; j < DESKSIZE; j++)
            {
                boxes.Add(new Box(i, j));   
            }
        }

        Vector2Int pos = ProcessFlagSpawn();
        flag = new Flag(pos.x, pos.y);

        for (int i = 0; i < WARRIORSATSTART; i++)
        {
            Vector2Int posWarriors = ProcessWarriorSpawn();
            warriors.Add(new Warrior(posWarriors.x, posWarriors.y));
        }

        StartCoroutine("PlayRound");
    }

    private IEnumerator PlayRound()
    {
        while (true)
        {
            ROUND += 1;
            Debug.Log("Round - "+ROUND);
            foreach (var warrior in warriors)
            {
                ProcessWarriorMoveToFlag(warrior);
                ProcessWarriorAttack(warrior);
                yield return new WaitForSeconds(TIMEBETWEENMOVES);
            }
            yield return new WaitForSeconds(TIMEBETWEENROUNDS);    
        }
    }

    private void ProcessWarriorAttack(Warrior warrior)
    {
        if(Functions.TestRange(warrior.target.posX, warrior.posX-1,warrior.posX+1) && Functions.TestRange(warrior.target.posY, warrior.posY-1,warrior.posY+1))
        {
            warrior.target.TakeDamage(warrior.damage);
        }
    }

    private void ProcessWarriorMove(Warrior warrior)
    {
        List<Box> boxesFinded = boxes.FindAll(x => x.isFull == false && Functions.TestRange(x.posX, warrior.posX-1,warrior.posX+1) && Functions.TestRange(x.posY, warrior.posY-1,warrior.posY+1));
        Box box = boxesFinded[Random.Range (0, boxesFinded.Count)];
        box.isFull = true;

        boxes.Find(x => warrior.posX == x.posX && warrior.posY == x.posY).isFull = false;

        warrior.Move(box);
    }

    private void ProcessWarriorMoveToFlag(Warrior warrior)
    {
        warrior.target = flag;
        List<Box> boxesFinded = boxes.FindAll(x => x.isFull == false && Functions.TestRange(x.posX, warrior.posX-1,warrior.posX+1) && Functions.TestRange(x.posY, warrior.posY-1,warrior.posY+1));

        Box box = boxesFinded[0];
        float distance = 1000f;
        foreach (var boxVar in boxesFinded)
        {
            float distanceTmp = Vector2Int.Distance(new Vector2Int(boxVar.posX, boxVar.posY), new Vector2Int(flag.posX, flag.posY));
            if(distanceTmp < distance)
            {
                distance = distanceTmp;
                box = boxVar;
            }
        }
        box.isFull = true;

        boxes.Find(x => warrior.posX == x.posX && warrior.posY == x.posY).isFull = false;

        warrior.Move(box);
    }

    private Vector2Int ProcessWarriorSpawn()
    {
        Vector2Int pos = new Vector2Int();

        Box box = boxes.Find(x => x.isFull == false);
        pos = new Vector2Int(box.posX, box.posY);

        box.isFull = true;

        return pos;
    }

    private Vector2Int ProcessFlagSpawn()
    {
        Vector2Int pos = new Vector2Int();

        List<Box> boxFinded = boxes.FindAll(x => x.isFull == false);
        Box boxToSpawn = boxFinded[Random.Range (0, boxFinded.Count)];
        pos = new Vector2Int(boxToSpawn.posX, boxToSpawn.posY);

        boxToSpawn.isFull = true;

        return pos;
    }
}
