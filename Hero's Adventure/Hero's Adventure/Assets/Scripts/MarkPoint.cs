using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class MarkPoint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI warringText;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineVirtualCamera CVcamera;
    [SerializeField] private float timeFocus = 2f;
    [SerializeField] private GameObject bossHealthBar;
    [SerializeField] private GameObject tele;
    private Animator warringAnim;
    private Animator CVcameraAnim;
    private int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        warringAnim = warringText.GetComponent<Animator>();
        CVcameraAnim = CVcamera.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(boss.GetComponent<BossScript>().GetCurrentHealth() == 0)
        {
            tele.SetActive(true);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            count++;
            if(count == 1)
            {
                StartCoroutine(FocusBoss());
            }
        }
    }
    IEnumerator FocusBoss()
    {
        warringText.gameObject.SetActive(true);
        warringAnim.SetBool("appear", true);
        CVcamera.Follow = null;
        CVcameraAnim.SetBool("move", true);
        player.GetComponent<PlayerController>().SetCanMove(false);
        yield return new WaitForSeconds(timeFocus);
        warringAnim.gameObject.SetActive(false);
        bossHealthBar.SetActive(true);
        CVcamera.Follow = player.transform;
        CVcameraAnim.SetBool("move", false);
        boss.GetComponent<BossScript>().setCanMove(true);
        player.GetComponent<PlayerController>().SetCanMove(true);

    }
}
