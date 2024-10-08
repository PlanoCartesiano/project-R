using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine.UI;
using UnityEngine.Diagnostics;

public enum GameState
{
    PAUSE,
    RUN,
    INVENTORY
}

public class GameDataController : MonoBehaviour
{

    public string transitionedFromScene;
    public static GameDataController Instance { get; private set; }

    private Inventory inventory;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public GameState currentState;
    private fadeEffect fadeEffect;
    public int test;
    public int Essence;
    public TextMeshProUGUI essenceTxt;

    [Header("Raffa Player Information")]
    public int maximumHealth;
    [HideInInspector] public Vector2 startPosition;

    [Header("Panels")]
    public GameObject PausePanel;
    public GameObject InventoryPanel;

    [Header("First Element of Each Panel")]
    public Button firstButtonPausePanel;
    public Button firstButtonInventoryPanel;

    [Header("Items Database")]
    public string[] itemName;
    public Sprite[] itemIcon;
    public Sprite[] itemImage;
    public string[] itemDescription;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        inventory = FindFirstObjectByType(typeof(Inventory)) as Inventory;
        startPosition = playerScript.Instance.transform.position;
        fadeEffect = FindFirstObjectByType(typeof(fadeEffect)) as fadeEffect;
        PausePanel.SetActive(false);
        InventoryPanel.SetActive(false);
        fadeEffect.fadeOut();
    }

    void Update()
    {
        essenceTxt.text = Essence.ToString();

        if (Input.GetButtonDown("Cancel") && currentState != GameState.INVENTORY)
        {
            PauseGame();
        }

        if (Input.GetButtonDown("Fire3") && currentState == GameState.INVENTORY)
        {
            closePanel();
        }
    }

    void PauseGame()
    {
        bool pauseState = PausePanel.activeSelf;
        pauseState = !pauseState;

        PausePanel.SetActive(pauseState);

        switch (pauseState)
        {
            case true:
                Time.timeScale = 0;
                changeState(GameState.PAUSE);
                firstButtonPausePanel.Select();
                break;

            case false:
                Time.timeScale = 1;
                changeState(GameState.RUN);
                break;
        }
    }

    public void changeState(GameState newState)
    {
        currentState = newState;
    }

    public void buttonInventoryDown()
    {
        PausePanel.SetActive(false);
        InventoryPanel.SetActive(true);
        firstButtonInventoryPanel.Select();
        inventory.LoadInventory();
        changeState(GameState.INVENTORY);
    }

    public void closePanel()
    {
        InventoryPanel.SetActive(false);
        PausePanel.SetActive(true);
        inventory.ClearLoadedItems();
        firstButtonPausePanel.Select();
        changeState(GameState.PAUSE);
    }
}
