using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(TextMeshProUGUI))]
public class CountdownAni : MonoBehaviour
{

    TextMeshProUGUI text;
    Animator ani;
    string[] countNum = { "0", "1", "2", "3", "4", "5" };

    private void Awake()
    {
        ani = GetComponent<Animator>();
        text = GetComponent<TextMeshProUGUI>();
    }

    public void PlayCountdown(int count)
    {

        if (count <= 0 || count > countNum.Length - 1)
        {
            Debug.Log("ī��Ʈ�ٿ� ���� �̿��� �� ����");
            return;
        }

        text.text = countNum[count];
        ani.Play("Countdown");
    }
}
