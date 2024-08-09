using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneTransition : MonoBehaviour
{
    private fadeEffect fadeEffect;
    [Header("Defina qual a cena de destino")]
    public string destinationScene;
    [SerializeField] Transform startPoint;
    [SerializeField] Vector2 exitDirection;

    private void Start()
    {
        if(destinationScene == GameDataController.Instance.transitionedFromScene)
        {
            playerScript.Instance.transform.position = startPoint.position;
        }

        fadeEffect = FindFirstObjectByType(typeof(fadeEffect)) as fadeEffect;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
                StartCoroutine("changingScene");
                break;
        }
    }

    IEnumerator changingScene()
    {
        fadeEffect.fadeIn();
        yield return new WaitWhile(() => fadeEffect.blackout.color.a < 0.9f);
        GameDataController.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(destinationScene);
    }
}
