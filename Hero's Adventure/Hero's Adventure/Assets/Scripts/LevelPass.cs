using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPass : MonoBehaviour
{
    [SerializeField] private float delayTime = 1f;
    [SerializeField] GameObject changeScene;
    [SerializeField] private string sceneName;
    [SerializeField] private AudioSource teleSound;
    private PlayerController playerController;
    private bool isTrigger = false;
    private bool canFade = false;
    private Animator anim;
    private int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.Find("Warrior").GetComponent<PlayerController>();
        anim = changeScene.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTrigger)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                playerController.SetCanMove(false);
                canFade = true;
            }
            if (canFade)
            {
                playerController.Fade();
                count++;
                if(count == 1)
                {
                    teleSound.Play();
                }
                if(playerController.GetFadeValue() == 0)
                {
                    anim.SetTrigger("Play");
                    StartCoroutine(LoadScene());
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTrigger = true;
            //SceneManager.LoadScene();
        }
    }
    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(sceneName);
    }
}
