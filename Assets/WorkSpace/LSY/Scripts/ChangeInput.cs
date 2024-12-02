using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeInput : MonoBehaviour
{
    [Header("Login Panel")]
    [SerializeField] private TMP_InputField _emailInput;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] Button confirmButton;
    //[SerializeField] private GameObject _signupPanel;
    //[SerializeField] private GameObject _verifyPanel;
    //[SerializeField] private GameObject _nicknamePanel;
    //[SerializeField] private DataManager _dataManager;

    //[Header("Sign UP Panel")]
    //[SerializeField] private TMP_InputField _emailSignupInput;
    //[SerializeField] private TMP_InputField _passwordSignupInput;
    //[SerializeField] private TMP_InputField _passwordConfirmInput;

    //[Header("Nickname Panel")]
    //[SerializeField] private TMP_InputField _nicknameInput;
    //[SerializeField] private GameObject _lobbyPanel;

    //[Header("Lobby Panel")]
    //[SerializeField] private GameObject _createRoomPanel;
    //private Dictionary<string, RoomEntry> _roomDic = new Dictionary<string, RoomEntry>();
    //[SerializeField] private RoomEntry _roomPrefab;
    //[SerializeField] private RectTransform _roomTransform;

    //[Header("Create Room Panel")]
    //[SerializeField] private TMP_InputField _roomNameInput;
    //[SerializeField] private TMP_InputField _maxPlayerInput;

    EventSystem system;
    public Selectable firstInput;

    private void Start()
    {
        system = EventSystem.current;

        firstInput = _emailInput;
        firstInput.Select();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
            if (next != null)
            {
                next.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
            {
                next.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            confirmButton.onClick.Invoke();
            Debug.Log("Button pressed!");
        }
    }
}
