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
    public class QuestParser : MonoBehaviour
    {
        [SerializeField] private QuestContainer quest;
        [SerializeField] private TextMeshProUGUI questText;
        [SerializeField] private TextMeshProUGUI textPrefab;

        [SerializeField] private Button acceptPrefab;
        [SerializeField] private Transform acceptTransform;
        [SerializeField] private Transform questContainer;



        private void Start()
        {
            var narrativeData = quest.NodeLinks.First(); //Entrypoint node
            ProceedToNarrative(narrativeData.TargetNodeGUID);
        }

        private void ProceedToNarrative(string narrativeDataGUID)
        {
            var text = quest.QuestNodeData.Find(x => x.NodeGUID == narrativeDataGUID).QuestText;
            var choices = quest.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID);
            questText.text = text;

            var button = Instantiate(acceptPrefab, acceptTransform);
            button.onClick.AddListener(() => ProcessAccept(text));

            //var buttons = buttonContainer.GetComponentsInChildren<Button>();
            //for (int i = 0; i < buttons.Length; i++)
            //{
            //    Destroy(buttons[i].gameObject);
            //}

            // 대화가 끝날 때
            //if (choices.Count() == 0)
            //{
            //    var choice = choices.ToList();
            //    var button = Instantiate(nextPrefab, nextButtonTransform);
            //    button.onClick.AddListener(() => EndDialogue());
            //    return;
            //}

            //// chioce가 1개 == 다음 버튼
            //if (choices.Count() == 1)
            //{
            //    var choice = choices.ToList();
            //    var button = Instantiate(nextPrefab, nextButtonTransform);
            //    button.GetComponentInChildren<Text>().text = ProcessProperties(choice[0].PortName);
            //    button.onClick.AddListener(() => ProceedToNarrative(choice[0].TargetNodeGUID));
            //    return;
            //}

            //// chioce가 2개 이상 == 선택지가 있을 때
            //foreach (var choice in choices)
            //{
            //    var button = Instantiate(choicePrefab, buttonContainer);
            //    button.GetComponentInChildren<Text>().text = ProcessProperties(choice.PortName);
            //    button.onClick.AddListener(() => ProceedToNarrative(choice.TargetNodeGUID));
            //}
        }

        private void ProcessAccept(string text)
        {
            var questText = Instantiate(textPrefab, questContainer);
            questText.text = text;
            //foreach (var exposedProperty in quest.ExposedProperties)
            //{
            //    text = text.Replace($"[{exposedProperty.PropertyName}]", exposedProperty.PropertyValue);
            //}
            //return text;
        }

        private void EndDialogue()
        {
            questText.transform.parent.gameObject.SetActive(false);
        }
    }
}