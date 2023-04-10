using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelScript : MonoBehaviour
{
    private List<string> collectibles;
    private List<string> checkpointPassed;
    private string prevPassedCheckpoint;
    private string currentPassedCheckpoint;
    private GameMaster checkpointSystem;
    private Vector3 prevCheckpoint;
    private Vector3 currentCheckpoint;
    private Vector3 currentRotation;
    private int levelCollectibleNb;
    private int levelCollectibleGot;
    private bool gotNewCheckpoint = false;
    private bool finishAvailable = false;

    public List<string> Collectibles { get => collectibles; set => collectibles = value; }
    public Vector3 CurrentCheckpoint { get => currentCheckpoint; set => currentCheckpoint = value; }
    public Vector3 CurrentRotation { get => currentRotation; set => currentRotation = value; }
    public int LevelCollectibleNb { get => levelCollectibleNb; set => levelCollectibleNb = value; }
    public int LevelCollectibleGot { get => levelCollectibleGot; set => levelCollectibleGot = value; }
    public Vector3 PrevCheckpoint { get => prevCheckpoint; set => prevCheckpoint = value; }
    public bool GotNewCheckpoint { get => gotNewCheckpoint; set => gotNewCheckpoint = value; }
    public bool FinishAvailable { get => finishAvailable; set => finishAvailable = value; }
    public string PrevPassedCheckpoint { get => prevPassedCheckpoint; set => prevPassedCheckpoint = value; }
    public string CurrentPassedCheckpoint { get => currentPassedCheckpoint; set => currentPassedCheckpoint = value; }
    public List<string> CheckpointPassed { get => checkpointPassed; set => checkpointPassed = value; }

    // Start is called before the first frame update
    void Start()
    {
        collectibles = new List<string>();
        checkpointPassed = new List<string>();
        checkpointSystem = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        checkpointSystem.CurrentScene = SceneManager.GetActiveScene().buildIndex;
        levelCollectibleNb = GameObject.FindGameObjectsWithTag("Collectable").Length;
        prevCheckpoint = checkpointSystem.CurrentCheckpoint;
        currentCheckpoint = prevCheckpoint;
        checkpointSystem.LevelCollectibleNb = levelCollectibleNb;
    }

    // Update is called once per frame
    void Update()
    {
        if (prevCheckpoint != currentCheckpoint)
        {
            for (int i = 0; i < Collectibles.Count; ++i)
                if (!checkpointSystem.Collectibles.Contains(Collectibles[i]))
                    checkpointSystem.Collectibles.Add(Collectibles[i]);

            for (int i = 0; i < CheckpointPassed.Count; ++i)
                if (!checkpointSystem.CheckpointPassed.Contains(CheckpointPassed[i]))
                    checkpointSystem.CheckpointPassed.Add(CheckpointPassed[i]);

            checkpointSystem.CurrentCheckpoint = CurrentCheckpoint;
            checkpointSystem.CurrentCheckpointPassed = currentPassedCheckpoint;
            checkpointSystem.CurrentRotation = CurrentRotation;
            checkpointSystem.LevelCollectibleGot += LevelCollectibleGot;

            prevCheckpoint = currentCheckpoint;
            collectibles.Clear();
            levelCollectibleGot = 0;

            if(PrevPassedCheckpoint != null)
                GameObject.Find(PrevPassedCheckpoint).GetComponent<BoxCollider>().enabled = true;
            prevPassedCheckpoint = currentPassedCheckpoint;
        }
            

        if(checkpointSystem.LevelCollectibleGot + levelCollectibleGot == checkpointSystem.LevelCollectibleNb)
        {
            FinishAvailable = true;
        }
        else
            FinishAvailable = false;
    }
}
