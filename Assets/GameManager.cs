using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private bool isInCutScene = false;
    public bool IsInCutScene { get { return isInCutScene; } set { isInCutScene = value; } }
    private bool isInBattle = false;
    public bool IsInBattle { get { return isInBattle; } set { isInBattle = value; } }
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetIsInCutScene(bool value)
    {
        isInCutScene = value;
    }
    public void SetIsInBattle(bool value)
    {
        isInBattle = value;
    }
}
