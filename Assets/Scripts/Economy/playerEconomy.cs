using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerEconomy : MonoBehaviour
{
    public static playerEconomy econ;
    private int coins;

    [SerializeField] private TMPro.TMP_Text coinCollection;

    public void Awake()
    {
        econ = this;
    }

    public void addCoin()
    {
        coins++;
        coinCollection.text = coins.ToString();
    }
}
