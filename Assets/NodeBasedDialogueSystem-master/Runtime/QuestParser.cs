using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Subtegral.DialogueSystem.DataContainers;

public class QuestSuccessCheck
{
    public QuestNodeData quest;
    public bool arrived = false;
    public bool collected = false;
    public bool talked = false;
    public bool inTime = false;
}

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

        List<QuestSuccessCheck> AcceptedQuests = new List<QuestSuccessCheck>();

        private void Start()
        {
            var narrativeData = quest.NodeLinks.First(); //Entrypoint node
            ProceedToNarrative(narrativeData.TargetNodeGUID);
        }

        private void ProceedToNarrative(string narrativeDataGUID)
        {
            var currentQuest = quest.QuestNodeData.Find(x => x.NodeGUID == narrativeDataGUID);

            var text = quest.QuestNodeData.Find(x => x.NodeGUID == narrativeDataGUID).QuestText;
            var choices = quest.NodeLinks.Where(x => x.BaseNodeGUID == narrativeDataGUID);
            questText.text = text;

            var button = Instantiate(acceptPrefab, acceptTransform);
            button.onClick.AddListener(() => ProcessAccept(currentQuest));

            //var buttons = buttonContainer.GetComponentsInChildren<Button>();
            //for (int i = 0; i < buttons.Length; i++)
            //{
            //    Destroy(buttons[i].gameObject);
            //}

            // ��ȭ�� ���� ��
            //if (choices.Count() == 0)
            //{
            //    var choice = choices.ToList();
            //    var button = Instantiate(nextPrefab, nextButtonTransform);
            //    button.onClick.AddListener(() => EndDialogue());
            //    return;
            //}

            //// chioce�� 1�� == ���� ��ư
            //if (choices.Count() == 1)
            //{
            //    var choice = choices.ToList();
            //    var button = Instantiate(nextPrefab, nextButtonTransform);
            //    button.GetComponentInChildren<Text>().text = ProcessProperties(choice[0].PortName);
            //    button.onClick.AddListener(() => ProceedToNarrative(choice[0].TargetNodeGUID));
            //    return;
            //}

            //// chioce�� 2�� �̻� == �������� ���� ��
            //foreach (var choice in choices)
            //{
            //    var button = Instantiate(choicePrefab, buttonContainer);
            //    button.GetComponentInChildren<Text>().text = ProcessProperties(choice.PortName);
            //    button.onClick.AddListener(() => ProceedToNarrative(choice.TargetNodeGUID));
            //}
        }

        private void ProcessAccept(QuestNodeData currentQuest)
        {
            var questText = Instantiate(textPrefab, questContainer);
            questText.text = currentQuest.QuestText;

            if ((currentQuest.successConditionEnum & successCondition.ARRIVED) == successCondition.ARRIVED)
                currentQuest.successCondition.destination.SetActive(true);

            if ((currentQuest.successConditionEnum & successCondition.COLLECT) == successCondition.COLLECT)
                currentQuest.successCondition.collection.SetActive(true);

            //if ((currentQuest.successConditionEnum & successCondition.TALK) == successCondition.TALK)
            //    currentQuest.successCondition.destination.SetActive(true);

            //if ((currentQuest.successConditionEnum & successCondition.TIMELIMIT) == successCondition.TIMELIMIT)
            //    currentQuest.successCondition.destination.SetActive(true);

            QuestSuccessCheck tmpQSC = new QuestSuccessCheck() { quest = currentQuest };
            AcceptedQuests.Add(tmpQSC);

            //foreach (var exposedProperty in quest.ExposedProperties)
            //{
            //    text = text.Replace($"[{exposedProperty.PropertyName}]", exposedProperty.PropertyValue);
            //}
            //return text;
        }

        public void CheckArrived(string guid)
        {
            AcceptedQuests.Find(x => x.quest.NodeGUID == guid).arrived = true;
        }

        public void CheckCollected(string guid)
        {
            if (AcceptedQuests.Find(x => x.quest.NodeGUID == guid) != null)
                AcceptedQuests.Find(x => x.quest.NodeGUID == guid).collected = true;
        }

        public void CheckTalked(string guid)
        {
            AcceptedQuests.Find(x => x.quest.NodeGUID == guid).talked = true;
        }


        public void CheckTimeLimit(string guid)
        {
            AcceptedQuests.Find(x => x.quest.NodeGUID == guid).inTime = true;
        }

        private void CheckQuestSuccess()
        {

        }
        private void PrintQuest()
        {
            for(int i = 0; i < AcceptedQuests.Count(); ++i)
            {
                //switch (CurrentQuests[i].successConditionEnum)
                //{
                //    case successCondition.ARRIVED:
                //        break;
                //    case successCondition.COLLECT:
                //        break;
                //    case successCondition.TALK:
                //        break;
                //    case successCondition.TIMELIMIT:
                //        break;
                //}
            }        
        }
        private void EndDialogue()
        {
            questText.transform.parent.gameObject.SetActive(false);
        }
    }
}