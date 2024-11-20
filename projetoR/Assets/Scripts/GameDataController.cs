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
    private itensInformations itemInfo;

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
    //public GameObject PausePanel;
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
        itemInfo = FindFirstObjectByType(typeof(itensInformations)) as itensInformations;
        startPosition = playerScript.Instance.transform.position;
        fadeEffect = FindFirstObjectByType(typeof(fadeEffect)) as fadeEffect;
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
            PauseGame();
        }
    }

    void PauseGame()
    {
        bool inventoryState = InventoryPanel.activeSelf;
        inventoryState = !inventoryState;

        InventoryPanel.SetActive(inventoryState);

        switch (inventoryState)
        {
            case true:
                Time.timeScale = 0;
                changeState(GameState.PAUSE);
                firstButtonInventoryPanel.Select();
                inventory.LoadInventory();
                changeState(GameState.INVENTORY);
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
}
