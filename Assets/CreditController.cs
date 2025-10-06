using UnityEngine;

public class CreditController : MonoBehaviour
{
    public void Menu()
    {
        SceneController.Instance.StartSceneSwitch(SceneType.MainMenu);
    }
}
