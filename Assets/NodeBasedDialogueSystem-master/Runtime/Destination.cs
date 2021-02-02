using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subtegral.DialogueSystem.Runtime
{
    public class Destination : MonoBehaviour
    {
        public string guid;

        private void Start()
        {
            this.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                this.gameObject.SetActive(false);
                Camera camera = Camera.main;
                camera.GetComponent<QuestParser>().CheckArrived(guid);
            }
        }
    }
}


