using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressEnterScript : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private GameObject Selection;
    [SerializeField] private GameObject continueSelection;
    [SerializeField] private AudioSource pressAudio;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            pressAudio.Play();
        }
        if (Input.GetKeyUp(KeyCode.Return))
        {
            gameObject.SetActive(false);
            Selection.SetActive(true);
        }
    }
}
