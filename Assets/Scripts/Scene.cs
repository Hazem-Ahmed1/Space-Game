using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

    }
    // Start is called before the first frame update
    public void ToGame()
    {
        SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    public void Exit()
    {
        Application.Quit();
    }
}
