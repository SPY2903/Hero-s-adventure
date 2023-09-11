using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagerSelection : MonoBehaviour
{
    [SerializeField] private Button[] Levels;
    [SerializeField] private string sceneName;
    Matrix matrix = new Matrix(2, 3);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(sceneName); 
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            for(int i = 1; i <= 6; i++)
            {
                if(i == matrix.GetCurrentIndex())
                {
                    SceneManager.LoadScene("Level"+i);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            matrix.MoveLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            matrix.MoveRight();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            matrix.MoveUp();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            matrix.MoveDown();
        }
        foreach (Button btn in Levels)
        {
            if (btn.GetComponent<LevelButton>().GetIndex() == matrix.GetCurrentIndex() )
            {
                btn.GetComponent<LevelButton>().Selected();
            }
            else
            {
                btn.GetComponent<LevelButton>().UnSelect();
            }
        }
    }
    public void SetCurrentSelection (int index)
    {
        matrix.SetCurrentIndex(index);
    }
}
