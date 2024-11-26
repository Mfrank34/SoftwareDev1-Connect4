using UnityEngine;

public class GameManger : MonoBehaviour
{
  public void GameStart()
  {
    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
  }

  public void GameQuit()
  {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#endif 
    Application.Quit();

  }
}
