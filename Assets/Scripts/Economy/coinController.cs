using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSupportLibrary;
public class coinController : MonoBehaviour
{
    Vector3 startPos;
    private void Awake()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(TaroController.controller.transform.position, transform.position) <= 1.5f)
        {
            transform.position = Vector3.Lerp(transform.position, TaroController.controller.transform.position, Time.deltaTime * .8f);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, startPos, Time.deltaTime * .8f);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            CollectCoin();
        }
    }

    public void CollectCoin()
    {
        playerEconomy.econ.addCoin();
        destroyCoin();
    }

    public void destroyCoin() 
    {
        Destroy(gameObject);
    }
}
