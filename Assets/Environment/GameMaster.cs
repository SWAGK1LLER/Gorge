using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class GameMaster : MonoBehaviour
{
    //private enum GameStates { MainMenu, InGame, Pause };
    //private GameStates prevState = GameStates.InGame;
    //private GameStates currentState = GameStates.MainMenu;
    [SerializeField] private LevelChanger lvlChanger;
    private List<string> collectibles;
    private List<string> checkpointPassed;
    private string currentCheckpointPassed;
    private static GameMaster instance;
    //private static InputManager inputs;
    //private static event Action<InputActionMap> actionList;
    private int currentScene;
    private Vector3 currentCheckpoint;
    private Vector3 currentRotation;
    private int levelCollectibleNb;
    public int levelCollectibleGot;
    private LevelScript levelUpdate;
    private PlayerInput inputs;
    private string currentControlScheme;
    private bool changeLightReady = true;

    public Vector3 CurrentCheckpoint { get => currentCheckpoint; set => currentCheckpoint = value; }
    public Vector3 CurrentRotation { get => currentRotation; set => currentRotation = value; }
    public int CurrentScene { get => currentScene; set => currentScene = value; }
    public List<string> Collectibles { get => collectibles; set => collectibles = value; }
    public int LevelCollectibleGot { get => levelCollectibleGot; set => levelCollectibleGot = value; }
    public List<string> CheckpointPassed { get => checkpointPassed; set => checkpointPassed = value; }
    public int LevelCollectibleNb { get => levelCollectibleNb; set => levelCollectibleNb = value; }
    public LevelScript LevelUpdate { get => levelUpdate; set => levelUpdate = value; }
    public string CurrentCheckpointPassed { get => currentCheckpointPassed; set => currentCheckpointPassed = value; }

    private void Awake()
    {
        collectibles = new List<string>();
        checkpointPassed = new List<string>();
        currentCheckpoint = GameObject.FindGameObjectWithTag("Player").transform.position;
        currentRotation = GameObject.FindGameObjectWithTag("Player").transform.eulerAngles;
        inputs = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        if(InputSystem.GetDevice<Gamepad>() != null)
            inputs.SwitchCurrentControlScheme(InputSystem.GetDevice<Gamepad>());
        currentControlScheme = inputs.currentControlScheme;
        if (instance == null)
        {
            instance = this;
            SceneManager.activeSceneChanged += ChangedActiveScene;
            InputSystem.onDeviceChange +=
            (device, change) =>
            {
                switch (change)
                {
                    case InputDeviceChange.Added:
                        print("New Device.");
                        inputs.SwitchCurrentControlScheme(device);
                        currentControlScheme = inputs.currentControlScheme;
                        break;
                    case InputDeviceChange.Disconnected:
                        print("Device got unplugged.");
                        InputDevice[] devices = { InputSystem.GetDevice<Keyboard>(), InputSystem.GetDevice<Mouse>() };
                        inputs.SwitchCurrentControlScheme(inputs.defaultControlScheme, devices);
                        currentControlScheme = inputs.currentControlScheme;
                        break;
                    case InputDeviceChange.Reconnected:
                        print("Plugged back in.");
                        inputs.SwitchCurrentControlScheme(device);
                        currentControlScheme = inputs.currentControlScheme;
                        break;
                    case InputDeviceChange.Removed:
                        print("Device Removed.");
                        InputDevice[] Defaultdevices = { InputSystem.GetDevice<Keyboard>(), InputSystem.GetDevice<Mouse>() };
                        inputs.SwitchCurrentControlScheme(inputs.defaultControlScheme, Defaultdevices);
                        currentControlScheme = inputs.currentControlScheme;
                        break;
                }
            };
            DontDestroyOnLoad(instance);
        }
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.inputs != null)
        {
            if (this.inputs.enabled != false && currentControlScheme != inputs.currentControlScheme)
            {
                if(currentControlScheme == "Keyboard&Mouse")
                    inputs.SwitchCurrentControlScheme(currentControlScheme, InputSystem.GetDevice<Keyboard>());
                else if(currentControlScheme == "Gamepad")
                    inputs.SwitchCurrentControlScheme(currentControlScheme, InputSystem.GetDevice<Gamepad>());
            }
        }
        for (int i = 0; i < collectibles.Count; ++i)
        {
            Destroy(GameObject.Find(collectibles[i]));
        }

        for (int i = 0; i < checkpointPassed.Count; ++i)
        { 
            Destroy(GameObject.Find(checkpointPassed[i]));
        }

        if (CurrentCheckpointPassed != null)
        {
            GameObject.Find(CurrentCheckpointPassed).GetComponent<BoxCollider>().enabled = false;
            CurrentCheckpointPassed = null;
        }

        if(SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (Input.GetKey(KeyCode.M) && Input.GetKey(KeyCode.T) && Input.GetKey(KeyCode.H))
            {
                lvlChanger.FadeInToNextLevel(3);
                currentCheckpoint = Vector3.zero;
                Collectibles.Clear();
                CheckpointPassed.Clear();
                LevelCollectibleGot = 0;
                LevelCollectibleNb = 0;
            }

            if (Input.GetKey(KeyCode.L) && Input.GetKey(KeyCode.U) && Input.GetKey(KeyCode.X) && changeLightReady)
            {
                changeLightReady = false;
                GameObject.FindGameObjectWithTag("Light").GetComponent<Light>().enabled = !GameObject.FindGameObjectWithTag("Light").GetComponent<Light>().enabled;
                StartCoroutine(timer());
            }

            if(Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.U) && Input.GetKey(KeyCode.F) && SceneManager.GetActiveScene().buildIndex == 1)
            {
                lvlChanger.FadeInToNextLevel(2);
                currentCheckpoint = Vector3.zero;
                Collectibles.Clear();
                CheckpointPassed.Clear();
                LevelCollectibleGot = 0;
                LevelCollectibleNb = 0;
            }
        }

        if (SceneManager.GetActiveScene().isLoaded && !lvlChanger.ChangingScene)
            lvlChanger.FadeOutInNextLevel();
    }

    private IEnumerator timer()
    {
        yield return new WaitForSeconds(1);
        changeLightReady = true;
    }

    private void ChangedActiveScene(Scene current, Scene next)
    {
        string currentName = current.name;

        if (currentName == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            inputs = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        }
    }

    public void PlayerIsDead(int sceneIndexToLoad)
    {
        if (GameObject.FindGameObjectWithTag("LM") != null)
        {
            LevelUpdate = GameObject.FindGameObjectWithTag("LM").GetComponent<LevelScript>();
        }

        if (LevelUpdate != null)
        {
            if (LevelUpdate.GotNewCheckpoint)
            {
                for (int i = 0; i < LevelUpdate.Collectibles.Count; ++i)
                    if (!this.collectibles.Contains(LevelUpdate.Collectibles[i]))
                        this.collectibles.Add(LevelUpdate.Collectibles[i]);

                for (int i = 0; i < LevelUpdate.CheckpointPassed.Count; ++i)
                    if (!this.collectibles.Contains(LevelUpdate.CheckpointPassed[i]))
                        this.checkpointPassed.Add(LevelUpdate.CheckpointPassed[i]);

                this.currentCheckpoint = LevelUpdate.CurrentCheckpoint;
                this.currentRotation = LevelUpdate.CurrentRotation;
                this.levelCollectibleGot += LevelUpdate.LevelCollectibleGot;
            }

            SceneManager.LoadScene(sceneIndexToLoad, LoadSceneMode.Single);
        }
    }

    public void RespawnFromDeath()
    {
        lvlChanger.FadeInToNextLevel(currentScene);
    }
}
