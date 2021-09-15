using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// Classe du Jeu
/// </summary>
public class Game : MonoBehaviour
{
    public static int DESKSIZE = 50;
    public static int WARRIORSATSTART = 1;

    public static float TIMEBETWEENROUNDS = 1f;
    public static float TIMEBETWEENMOVES = .1f;

    public int ROUND_ACTIONS = 0;
    public int ROUND = 0;

    //Controls
    protected FixedJoystick joystick;
    protected JoyButton joybutton;

    private List<GameObject> boxes;

    private List<GameObject> warriors;
    private List<Team> teams;

    public List<GameObject> monsters;
    public List<EnvironmentProps> props;

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
        if(playerObject)
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
        GameObject healthCanvas;

        this.joybutton = FindObjectOfType<JoyButton>();
        this.joystick = FindObjectOfType<FixedJoystick>();

        for (int i = 0; i < DESKSIZE; i++)
        {
            for (int j = 0; j < DESKSIZE; j++)
            {
                GameObject newBox = Instantiate(props.Where(obj => obj.title == "floor").SingleOrDefault().prefabObject);
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
        healthCanvas = Instantiate((GameObject)Resources.Load("Prefabs/Health Bar", typeof(GameObject)), flagObject.transform);
        flagObject.healthBar = healthCanvas.GetComponentInChildren(typeof(Slider)) as Slider;
        flagObject.healthBar.maxValue = flagObject.fullLife;
        flagObject.healthBar.value = flagObject.currentLife;

        playerObject = Instantiate(playerPrefab);
        Player playerComponent = playerObject.AddComponent<Player>() as Player;
        playerComponent.CreatePlayer(pos.x, pos.y);

        for (int i = 0; i < WARRIORSATSTART; i++)
        {
            Vector2Int posWarriors = ProcessWarriorSpawn();
            GameObject newWarrior = Instantiate(monsters[Random.Range (0, monsters.Count)]);
            Warrior warrior = newWarrior.AddComponent<Warrior>() as Warrior;
            warrior.CreateWarrior(posWarriors.x, posWarriors.y);
            healthCanvas = Instantiate((GameObject)Resources.Load("Prefabs/Health Bar", typeof(GameObject)), warrior.transform);
            warrior.healthBar = healthCanvas.GetComponentInChildren(typeof(Slider)) as Slider;
            warrior.healthBar.maxValue = warrior.fullLife;
            warrior.healthBar.value = warrior.currentLife;
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
            while (warriors.Count > 0 || flag.GetComponent<Flag>().currentLife > 0 || playerObject.GetComponent<Player>().currentLife > 0)
            {
                ROUND_ACTIONS += 1;
                foreach (var warrior in warriors)
                {
                    ProcessWarriorMoveTo(warrior.GetComponent<Warrior>());
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
        if(Vector3.Distance(warrior.gameObject.transform.position, warrior.target.gameObject.transform.position) < warrior.ATTACKRANGE)
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
    /// Détection d'un joueur proche
    /// </summary>
    /// <param name="warrior"></param>
    private Entity DetectPlayer(Warrior warrior)
    {
        if(Vector3.Distance(warrior.gameObject.transform.position, playerObject.transform.position) < warrior.DETECTENEMYDISTANCE)
        {
            return playerObject.GetComponent<Player>();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Déplacement du monstre vers le drapeau
    /// </summary>
    /// <param name="warrior"></param>
    private void ProcessWarriorMoveTo(Warrior warrior)
    {
        var playerDetected = DetectPlayer(warrior);
        if(playerDetected != null)
        {
            warrior.ChangeTarget(playerDetected);
        }
        else
        {
            warrior.ChangeTarget(flag.GetComponent<Entity>());
        }
        
        List<GameObject> boxesFinded = boxes.FindAll(x => x.GetComponent<Box>().isFull == false && Functions.TestRange(x.GetComponent<Box>().posX, warrior.posX-1,warrior.posX+1) && Functions.TestRange(x.GetComponent<Box>().posY, warrior.posY-1,warrior.posY+1));

        GameObject box = boxesFinded[0];
        float distance = 1000f;
        foreach (var boxVar in boxesFinded)
        {
            float distanceTmp = Vector2Int.Distance(new Vector2Int(boxVar.GetComponent<Box>().posX, boxVar.GetComponent<Box>().posY), new Vector2Int(warrior.target.GetComponent<Entity>().posX, warrior.target.GetComponent<Entity>().posY));
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
