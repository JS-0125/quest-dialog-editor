using Subtegral.DialogueSystem.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Collection : MonoBehaviour
{
    private GameObject collectionUi;
    public string guid;
    private Vector3 position;
    private void Start()
    {
        var Canvas = GameObject.Find("Canvas (2)");
        collectionUi = Canvas.transform.Find("CollectionUi").gameObject;
        this.gameObject.SetActive(false);
        position = this.transform.position;
        position.y += 2;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        { 
            collectionUi.transform.position = position;
            collectionUi.transform.rotation = Camera.main.transform.rotation;
            collectionUi.gameObject.SetActive(true);
            if (Input.GetButtonDown("LeftClick"))
            {
                this.gameObject.SetActive(false);
                collectionUi.gameObject.SetActive(false);

                Debug.Log("collect");

                Camera camera = Camera.main;
                camera.GetComponent<QuestParser>().CheckCollected(guid, this.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        collectionUi.gameObject.SetActive(false);
    }
}
