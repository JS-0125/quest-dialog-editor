using Subtegral.DialogueSystem.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour
{
    public string guid;
    public int amount;
    private int count;

    private void Start()
    {
        this.gameObject.SetActive(false);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (Input.GetButtonDown("LeftClick"))
            {
                ++count;
                this.gameObject.SetActive(false);

                Debug.Log("collect");
                if (count == amount)
                {
                    Debug.Log("collect success");
                    Camera camera = Camera.main;
                    camera.GetComponent<QuestParser>().CheckCollected(guid);
                }
            }     
        }
    }
}
