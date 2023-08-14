using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerEconomy : MonoBehaviour
{
    public static playerEconomy econ;
    private int coins;

    [SerializeField] private Text coinCollection;

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
