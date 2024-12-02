using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField _emailInput;

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
            Button next = system.currentSelectedGameObject.GetComponent<Button>();
            if (next != null)
            {
                if (next != null)
                {
                    next.onClick.Invoke();
                }
            }
        }
    }
}
