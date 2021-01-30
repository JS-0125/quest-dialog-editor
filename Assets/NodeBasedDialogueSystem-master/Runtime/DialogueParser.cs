using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Subtegral.DialogueSystem.DataContainers;

namespace Subtegral.DialogueSystem.Runtime
{
    public class DialogueParser : MonoBehaviour
    {
        [SerializeField] private DialogueContainer dialogue;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button choicePrefab;
        [SerializeField] private Button nextPrefab;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private Transform nextButtonTransform;


        private void Start()
        {
            var narrativeData = dialogue.NodeLinks.First(); //Entrypoint node
            ProceedToNarrative(narrativeData.TargetNodeGUID);
        }

        private void ProceedToNarrative(string narrativeDataGUID)
        {
            var text = dialogue.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).DialogueText;
            var choices = dialogue.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID);
            dialogueText.text = ProcessProperties(text);
            var buttons = buttonContainer.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                Destroy(buttons[i].gameObject);
            }

            // 대화가 끝날 때
            if (choices.Count() == 0)
            {
                var choice = choices.ToList();
                var button = Instantiate(nextPrefab, nextButtonTransform);
                button.onClick.AddListener(() => EndDialogue());
                return;
            }

            // chioce가 1개 == 다음 버튼
            if (choices.Count() == 1)
            {
                var choice = choices.ToList();
                var button = Instantiate(nextPrefab, nextButtonTransform);
                button.GetComponentInChildren<Text>().text = ProcessProperties(choice[0].PortName);
                button.onClick.AddListener(() => ProceedToNarrative(choice[0].TargetNodeGUID));
                return;
            }

            // chioce가 2개 이상 == 선택지가 있을 때
            foreach (var choice in choices)
            {
                var button = Instantiate(choicePrefab, buttonContainer);
                button.GetComponentInChildren<Text>().text = ProcessProperties(choice.PortName);
                button.onClick.AddListener(() => ProceedToNarrative(choice.TargetNodeGUID));
            }
        }

        private string ProcessProperties(string text)
        {
            foreach (var exposedProperty in dialogue.ExposedProperties)
            {
                text = text.Replace($"[{exposedProperty.PropertyName}]", exposedProperty.PropertyValue);
            }
            return text;
        }

        private void EndDialogue()
        {
            dialogueText.transform.parent.gameObject.SetActive(false);
        }
    }
}