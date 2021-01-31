using Subtegral.DialogueSystem.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour
{
    public string guid;

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
                this.gameObject.SetActive(false);

                Debug.Log("collect");

                Camera camera = Camera.main;
                camera.GetComponent<QuestParser>().CheckCollected(guid, this.gameObject);

            }
        }
    }
}
