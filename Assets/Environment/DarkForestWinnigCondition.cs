using UnityEngine;

public class DarkForestWinnigCondition : MonoBehaviour
{
    [SerializeField] private LevelScript lvlScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lvlScript.FinishAvailable)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().FinishLevel();
        }
    }
}
