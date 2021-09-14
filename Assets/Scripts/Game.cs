using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe du Jeu
/// </summary>
public class Game : MonoBehaviour
{
    public static int DESKSIZE = 50;
    public static int WARRIORSATSTART = 20;

    public static float TIMEBETWEENROUNDS = 1f;
    public static float TIMEBETWEENMOVES = .1f;

    public int ROUND_ACTIONS = 0;
    public int ROUND = 0;

    //Controls
    protected FixedJoystick joystick;
    protected JoyButton joybutton;

    public List<GameObject> boxes;

    public List<GameObject> warriors;
    public List<Team> teams;

    public List<GameObject> monsters;

    public GameObject playerPrefab;
    private GameObject playerObject;

    private GameObject flag;

    // Start is called before the first frame update
    void Start()
    {
        InitiateGame();
    }

    void Update()
    {
        if(playerPrefab)
        {
            Rigidbody rbPlayer = playerObject.GetComponent<Rigidbody>();
            rbPlayer.velocity = new Vector3(joystick.Horizontal * 10f + Input.GetAxis("Horizontal") * 10f,
            rbPlayer.velocity.y,
            joystick.Vertical * 10f + Input.GetAxis("Vertical") * 10f);
        }
    }

    /// <summary>
    /// Initialisation de la partie
    /// </summary>
    private void InitiateGame()
    {
        boxes = new List<GameObject>();
        warriors = new List<GameObject>();

        this.joybutton = FindObjectOfType<JoyButton>();
        this.joystick = FindObjectOfType<FixedJoystick>();

        for (int i = 0; i < DESKSIZE; i++)
        {
            for (int j = 0; j < DESKSIZE; j++)
            {
                GameObject newBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Box boxObject = newBox.AddComponent<Box>() as Box;
                BoxCollider boxCollider = newBox.AddComponent<BoxCollider>() as BoxCollider;
                boxObject.CreateBox(i, j);
                boxes.Add(newBox);   
            }
        }

        Vector2Int pos = ProcessFlagSpawn();
        flag = Instantiate((GameObject)Resources.Load("Prefabs/Flag", typeof(GameObject)));
        Flag flagObject = flag.AddComponent<Flag>() as Flag;
        flagObject.CreateFlag(pos.x, pos.y);
        Instantiate(flag);

        playerObject = Instantiate(playerPrefab);
        Player playerComponent = playerObject.AddComponent<Player>() as Player;
        playerComponent.CreatePlayer(pos.x, pos.y);

        for (int i = 0; i < WARRIORSATSTART; i++)
        {
            Vector2Int posWarriors = ProcessWarriorSpawn();
            GameObject newWarrior = Instantiate(monsters[Random.Range (0, monsters.Count)]);
            Warrior warrior = newWarrior.AddComponent<Warrior>() as Warrior;
            warrior.CreateWarrior(posWarriors.x, posWarriors.y);
            warriors.Add(newWarrior);
        }

        StartCoroutine("PlayRound");
    }

    /// <summary>
    /// Jouer une vague
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayRound()
    {
        while (true)
        {
            ROUND += 1;
            Debug.Log("Round - " + ROUND);
            while (warriors.Count > 0 || flag.GetComponent<Flag>().currentLife >= 0 || playerObject.GetComponent<Player>().currentLife >= 0)
            {
                ROUND_ACTIONS += 1;
                foreach (var warrior in warriors)
                {
                    ProcessWarriorMoveToFlag(warrior.GetComponent<Warrior>());
                    ProcessWarriorAttack(warrior.GetComponent<Warrior>());
                    yield return new WaitForSeconds(TIMEBETWEENMOVES);
                }
                yield return new WaitForSeconds(TIMEBETWEENROUNDS);
            }
        }
    }

    /// <summary>
    /// Faire attaquer le monstre s'il peut
    /// </summary>
    /// <param name="warrior"></param>
    private void ProcessWarriorAttack(Warrior warrior)
    {
        if(Functions.TestRange(warrior.target.posX, warrior.posX-1,warrior.posX+1) && Functions.TestRange(warrior.target.posY, warrior.posY-1,warrior.posY+1))
        {
            warrior.target.TakeDamage(warrior.damage);
        }
    }

    /// <summary>
    /// Faire bouger le monstre dans la direction voulu
    /// </summary>
    /// <param name="warrior"></param>
    private void ProcessWarriorMove(Warrior warrior)
    {
        List<GameObject> boxesFinded = boxes.FindAll(x => x.GetComponent<Box>().isFull == false && Functions.TestRange(x.GetComponent<Box>().posX, warrior.posX-1,warrior.posX+1) && Functions.TestRange(x.GetComponent<Box>().posY, warrior.posY-1,warrior.posY+1));
        GameObject box = boxesFinded[Random.Range (0, boxesFinded.Count)];
        box.GetComponent<Box>().isFull = true;

        boxes.Find(x => warrior.posX == x.GetComponent<Box>().posX && warrior.posY == x.GetComponent<Box>().posY).GetComponent<Box>().isFull = false;

        warrior.Move(box.GetComponent<Box>());
    }

    /// <summary>
    /// Déplacement du monstre vers le drapeau
    /// </summary>
    /// <param name="warrior"></param>
    private void ProcessWarriorMoveToFlag(Warrior warrior)
    {
        warrior.target = flag.GetComponent<Entity>();
        List<GameObject> boxesFinded = boxes.FindAll(x => x.GetComponent<Box>().isFull == false && Functions.TestRange(x.GetComponent<Box>().posX, warrior.posX-1,warrior.posX+1) && Functions.TestRange(x.GetComponent<Box>().posY, warrior.posY-1,warrior.posY+1));

        GameObject box = boxesFinded[0];
        float distance = 1000f;
        foreach (var boxVar in boxesFinded)
        {
            float distanceTmp = Vector2Int.Distance(new Vector2Int(boxVar.GetComponent<Box>().posX, boxVar.GetComponent<Box>().posY), new Vector2Int(flag.GetComponent<Entity>().posX, flag.GetComponent<Entity>().posY));
            if(distanceTmp < distance)
            {
                distance = distanceTmp;
                box = boxVar;
            }
        }
        box.GetComponent<Box>().isFull = true;

        boxes.Find(x => warrior.posX == x.GetComponent<Box>().posX && warrior.posY == x.GetComponent<Box>().posY).GetComponent<Box>().isFull = false;

        warrior.Move(box.GetComponent<Box>());
    }

    /// <summary>
    /// Apparisition d'un monstre
    /// </summary>
    /// <returns></returns>
    private Vector2Int ProcessWarriorSpawn()
    {
        Vector2Int pos = new Vector2Int();

        Box box = boxes.Find(x => x.GetComponent<Box>().isFull == false).GetComponent<Box>();
        pos = new Vector2Int(box.posX, box.posY);

        box.isFull = true;

        return pos;
    }

    /// <summary>
    /// Apparition du drapeau
    /// </summary>
    /// <returns></returns>
    private Vector2Int ProcessFlagSpawn()
    {
        Vector2Int pos = new Vector2Int();

        List<GameObject> boxFinded = boxes.FindAll(x => x.GetComponent<Box>().isFull == false);
        Box boxToSpawn = boxFinded[Random.Range (0, boxFinded.Count)].GetComponent<Box>();
        pos = new Vector2Int(boxToSpawn.posX, boxToSpawn.posY);

        boxToSpawn.isFull = true;

        return pos;
    }
}
