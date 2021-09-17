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
    [Header("Options de la partie")]
    public static int DESKSIZE = 50;
    public static int WARRIORSATSTART = 1;
    [Range(0, 1)]public static float MAPPROPSDENSITY = 0.5f;

    public static float TIMEBETWEENROUNDS = 1f;
    public static float TIMEBETWEENMOVES = .1f;

    [Header("Affichage")]
    public int ROUND_ACTIONS = 0;
    public int ROUND = 0;

    [Header("Objets à instantier")]
    public List<GameObject> monsters;
    public List<EnvironmentProps> props;

    [Header("Joueur")]
    public GameObject playerPrefab;

    [Header("UI")]
    public Slider playerHealthBar;
    public Text txtGolds;
    public Text txtRound;

    //Controls
    protected FixedJoystick joystick;
    protected JoyButton joybutton;

    private List<GameObject> boxes;
    private List<GameObject> propsInstantiate;

    private List<GameObject> warriors;
    private List<Team> teams;

    private GameObject playerObject;
    private Vector2Int startPos;

    private GameObject flag;

    // Start is called before the first frame update
    void Start()
    {
        InitiateGame();
    }

    void Update()
    {
        if(playerObject && !playerObject.GetComponent<Player>().isDestroyed)
        {
            float speedVar = playerObject.GetComponent<Player>().speed;
            Rigidbody rbPlayer = playerObject.GetComponent<Rigidbody>();
            Vector3 direction = new Vector3(joystick.Horizontal * speedVar + Input.GetAxis("Horizontal") * speedVar,
            rbPlayer.velocity.y,
            joystick.Vertical * speedVar + Input.GetAxis("Vertical") * speedVar); 
            rbPlayer.velocity = direction;

            if(joybutton.isPressed == true)
            {
                ProcessPlayerAttack();
            }

            RefreshGolds();

            var input = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
            if(input != Vector3.zero)
            {
                this.playerObject.transform.forward = input;
            }

            /*if(rbPlayer.velocity != Vector3.zero)
            {
                playerObject.transform.rotation.SetLookRotation(direction);
            }*/
        }
    }

    /// <summary>
    /// Initialisation de la partie
    /// </summary>
    private void InitiateGame()
    {
        boxes = new List<GameObject>();
        warriors = new List<GameObject>();
        propsInstantiate = new List<GameObject>();
        GameObject healthCanvas;

        this.joybutton = FindObjectOfType<JoyButton>();
        this.joystick = FindObjectOfType<FixedJoystick>();

        for (int i = 0; i < DESKSIZE; i++)
        {
            for (int j = 0; j < DESKSIZE; j++)
            {
                GameObject newBox = Instantiate(props.Where(obj => obj.title == "floor").FirstOrDefault().prefabObject);
                Box boxObject = newBox.AddComponent<Box>() as Box;
                BoxCollider boxCollider = newBox.AddComponent<BoxCollider>() as BoxCollider;
                boxObject.CreateBox(i, j);
                boxes.Add(newBox);   
            }
        }

        FlagSpawn();

        playerObject = Instantiate(playerPrefab);
        Player playerComponent = playerObject.AddComponent<Player>() as Player;
        playerComponent.CreatePlayer(startPos.x, startPos.y+2);
        playerComponent.healthBar = playerHealthBar;
        playerComponent.healthBar.maxValue = playerComponent.fullLife;
        playerComponent.healthBar.value = playerComponent.currentLife;


        ProcessSpawnProps();

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

    ///Faire spawn le drapeau
    private void FlagSpawn()
    {
        GameObject healthCanvas;
        startPos = ProcessFlagSpawn();
        flag = Instantiate((GameObject)Resources.Load("Prefabs/Flag", typeof(GameObject)));
        Flag flagObject = flag.AddComponent<Flag>() as Flag;
        flagObject.CreateFlag(startPos.x, startPos.y);
        healthCanvas = Instantiate((GameObject)Resources.Load("Prefabs/Health Bar", typeof(GameObject)), flagObject.transform);
        flagObject.healthBar = healthCanvas.GetComponentInChildren(typeof(Slider)) as Slider;
        flagObject.healthBar.maxValue = flagObject.fullLife;
        flagObject.healthBar.value = flagObject.currentLife;
    }

    /// <summary>
    /// Faire spawn les elements du décor
    /// </summary>
    private void ProcessSpawnProps()
    {
        foreach(var box in boxes)
        {
            if(box.GetComponent<Box>().isFull == false && Random.value < (MAPPROPSDENSITY/10))
            {
                List<EnvironmentProps> propsFinded = props.FindAll(obj => obj.title == "propsEnvironment");
                if(propsFinded.Count > 0)
                {
                    GameObject newBox = Instantiate(propsFinded[Random.Range(0, propsFinded.Count)].prefabObject);
                    Box boxObject = newBox.AddComponent<Box>() as Box;
                    boxObject.CreateBox(box.GetComponent<Box>().posX, box.GetComponent<Box>().posY);
                    propsInstantiate.Add(newBox);   
                    box.GetComponent<Box>().isFull = true;
                }
            }
        }
    }

    /// <summary>
    /// Jouer une vague
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayRound()
    {
        while (flag.GetComponent<Flag>().currentLife > 0)
        {
            ROUND += 1;
            Debug.Log("Round - " + ROUND);
            txtRound.text = ""+ROUND;
            while (warriors.Count > 0)
            {
                ROUND_ACTIONS += 1;
                foreach (var warrior in warriors.ToList())
                {
                    if(!warrior.GetComponent<Warrior>().isDestroyed)
                    {
                        ProcessWarriorMoveTo(warrior.GetComponent<Warrior>());
                        ProcessWarriorAttack(warrior.GetComponent<Warrior>());
                    }
                    else
                    {
                        warriors.Remove(warrior);
                        Destroy(warrior);
                    }
                    yield return new WaitForSeconds(TIMEBETWEENMOVES);
                }
                if(flag.GetComponent<Flag>().currentLife <= 0)
                    break;
                yield return new WaitForSeconds(TIMEBETWEENROUNDS);
            }
        }
    }

    public void RefreshGolds()
    {
        txtGolds.text = ""+playerObject.GetComponent<Player>().golds;
    }

    /// <summary>
    /// Le joueur attaque devant lui
    /// </summary>
    private void ProcessPlayerAttack()
    {
        this.playerObject.GetComponent<Player>().AttackAnim();
    }

    /// <summary>
    /// Faire attaquer le monstre s'il peut
    /// </summary>
    /// <param name="warrior"></param>
    private void ProcessWarriorAttack(Warrior warrior)
    {
        if(Vector3.Distance(warrior.gameObject.transform.position, warrior.target.gameObject.transform.position) < warrior.ATTACKRANGE)
        {
            warrior.target.TakeDamage(warrior.damage, warrior.gameObject);
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
