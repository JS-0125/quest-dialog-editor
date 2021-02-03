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
    public bool inTime = true;
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

        [SerializeField] private TextMeshProUGUI timer;

        [SerializeField] private Transform collectObjectContainer;
        [SerializeField] private Transform destinationContainer;
        [SerializeField] private PlayerAction playerAction;


        private List<QuestSuccessCheck> AcceptedQuests = new List<QuestSuccessCheck>();
        private List<NodeLinkData> canAcceptQuest = new List<NodeLinkData>();
        private DialogueParser dialogueParser;

        private void Start()
        {
            dialogueParser = this.transform.GetComponent<DialogueParser>();
            Debug.Log(quest.NodeLinks.First().PortName);
            canAcceptQuest.Add(quest.NodeLinks.First());

            var notCollection = collectObjectContainer.GetComponentsInChildren<Transform>();
            for (int i = 0; i < notCollection.Count(); ++i)
                notCollection[i].parent = null;

            var collections = GameObject.FindObjectsOfType<Collection>(true);
            for (int i = 0; i < collections.Count(); ++i)
                collections[i].transform.parent = collectObjectContainer;


            var notDestination = destinationContainer.GetComponentsInChildren<Transform>();
            for (int i = 0; i < notDestination.Count(); ++i)
                notDestination[i].parent = null;

            var destinations = GameObject.FindObjectsOfType<Destination>(true);
            for (int i = 0; i < destinations.Count(); ++i)
                destinations[i].transform.parent = destinationContainer;
        }

        public bool IsQuestGiver(GameObject obj)
        {
            Debug.Log("IsQuestGiver");

            for(int i =0;i< canAcceptQuest.Count(); ++i)
            {
                Debug.Log("check canAcceptQuest");

                var checkQuest = quest.QuestNodeData.Find(x => x.NodeGUID == canAcceptQuest[i].TargetNodeGUID);

                if (checkQuest == null)
                    continue;

                if (checkQuest.QeustGiver == obj.name)
                {
                    Debug.Log("find quest");

                    dialogueParser.dialogue = checkQuest.questDialogue;
                    dialogueParser.StartTalk(canAcceptQuest[i].TargetNodeGUID);
                    return true;
                }
            }
            return false;
        }

        public void ProceedToNarrative(string narrativeDataGUID)
        {
            var buttons = acceptTransform.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                Destroy(buttons[i].gameObject);
            }

            questText.gameObject.SetActive(true);
            acceptTransform.gameObject.SetActive(true);

            var currentQuest = quest.QuestNodeData.Find(x => x.NodeGUID == narrativeDataGUID);

            var text = quest.QuestNodeData.Find(x => x.NodeGUID == narrativeDataGUID).QuestText;

            questText.text = text;

            var button = Instantiate(acceptPrefab, acceptTransform);
            button.onClick.AddListener(() => ProcessAccept(currentQuest));
        }


        private void ProcessAccept(QuestNodeData currentQuest)
        {
            playerAction.State = PlayerState.Idle;
            canAcceptQuest.Remove(quest.NodeLinks.Find(x => x.TargetNodeGUID == currentQuest.NodeGUID));

            var questContainerText = Instantiate(textPrefab, questContainer);
            questContainerText.text = currentQuest.QuestText;

            QuestSuccessCheck tmpQSC = new QuestSuccessCheck() { quest = currentQuest };
            AcceptedQuests.Add(tmpQSC);
            

            if ((currentQuest.successConditionEnum & successCondition.ARRIVED) == successCondition.ARRIVED)
                destinationContainer.Find(currentQuest.successCondition.destination).gameObject.SetActive(true);

            if ((currentQuest.successConditionEnum & successCondition.COLLECT) == successCondition.COLLECT)
            {
                for (int i = 0; i < currentQuest.successCondition.collection.Count(); ++i)
                    collectObjectContainer.Find(currentQuest.successCondition.collection[i]).gameObject.SetActive(true);
                UpdateCollectQuestText();
            }

            if ((currentQuest.successConditionEnum & successCondition.TIMELIMIT) == successCondition.TIMELIMIT)
                StartCoroutine(Timer(currentQuest.successCondition.limitSec, currentQuest.NodeGUID));
            
            questText.gameObject.SetActive(false);
            acceptTransform.gameObject.SetActive(false);
        }

        public void CheckArrived(string guid)
        {
            var currentQuest = AcceptedQuests.Find(x => x.quest.NodeGUID == guid);

            currentQuest.arrived = true;
            if (CheckQuestSuccess(currentQuest))
                Successed(currentQuest);

            Debug.Log("arrived success");
        }

        public void CheckCollected(string guid, GameObject collection)
        {
            var currentQuest = AcceptedQuests.Find(x => x.quest.NodeGUID == guid);

            UpdateCollectQuestText();
            Debug.Log(collectObjectContainer.GetComponentsInChildren<Transform>().GetLength(0));

            if (collectObjectContainer.GetComponentsInChildren<Transform>().GetLength(0) == 1) 
            {
                currentQuest.collected = true;
                if (CheckQuestSuccess(currentQuest))
                    Successed(currentQuest);

                Debug.Log("collect success");
            }
        }

        public bool CheckTalkPartner(GameObject partner)
        {
            QuestSuccessCheck talkPartner;
            try
            {
                talkPartner = AcceptedQuests.Find(x => x.quest.successCondition.obj.name == partner.name);
            }
            catch (NullReferenceException e)
            {
                return false;
            }

            
            if (talkPartner != null)
            {
                var currentQuest = AcceptedQuests.Find(x => x.quest.successCondition.obj == partner);

                var dialogueParser = this.gameObject.GetComponent<DialogueParser>();
                dialogueParser.dialogue = currentQuest.quest.successCondition.dialogue;
                dialogueParser.StartTalk("not quest give");

                currentQuest.talked = true;
                if (CheckQuestSuccess(currentQuest))
                    Successed(currentQuest);

                Debug.Log("talk success");
                return true;
            }
            return false;
        }

        private IEnumerator Timer(float time, string guid)
        {
            Debug.Log("timer start");
            timer.gameObject.SetActive(true);

            while (time > 0)
            {
                if (AcceptedQuests.Find(x => x.quest.NodeGUID == guid) == null)
                    break;
                time -= 0.1f;
                timer.text = time.ToString();
                yield return new WaitForSeconds(0.1f);
            }

            if (AcceptedQuests.Find(x => x.quest.NodeGUID == guid) != null)
                AcceptedQuests.Find(x => x.quest.NodeGUID == guid).inTime = false;

            timer.gameObject.SetActive(false);
        }

        private bool CheckQuestSuccess(QuestSuccessCheck check)
        {
            switch (check.quest.successConditionEnum)
            {
                case successCondition.ARRIVED:
                    if (check.arrived)
                        return true;
                    break;
                case successCondition.COLLECT:
                    if (check.collected)
                        return true;
                    break;
                case successCondition.TALK:
                    if (check.talked)
                        return true;
                    break;
                case successCondition.TIMELIMIT:
                    if (check.inTime)
                        return true;
                    break;
                case successCondition.ARRIVED | successCondition.COLLECT:
                    if (check.arrived && check.collected)
                        return true;
                    break;
                case successCondition.ARRIVED | successCondition.TALK:
                    if (check.arrived && check.talked)
                        return true;
                    break;
                case successCondition.ARRIVED | successCondition.TIMELIMIT:
                    if (check.arrived && check.inTime)
                        return true;
                    break;
                case successCondition.COLLECT | successCondition.TALK:
                    if (check.collected && check.talked)
                        return true;
                    break;
                case successCondition.COLLECT | successCondition.TIMELIMIT:
                    if (check.collected && check.inTime)
                        return true;
                    break;
                case successCondition.TALK | successCondition.TIMELIMIT:
                    if (check.talked && check.inTime)
                        return true;
                    break;
                case successCondition.ARRIVED | successCondition.COLLECT | successCondition.TALK:
                    if (check.arrived && check.collected && check.talked)
                        return true;
                    break;
                case successCondition.ARRIVED | successCondition.COLLECT | successCondition.TIMELIMIT:
                    if (check.arrived && check.collected && check.inTime)
                        return true;
                    break;
                case successCondition.ARRIVED | successCondition.TALK | successCondition.TIMELIMIT:
                    if (check.arrived && check.talked && check.inTime)
                        return true;
                    break;
                case successCondition.COLLECT | successCondition.TALK | successCondition.TIMELIMIT:
                    if (check.collected && check.talked && check.inTime)
                        return true;
                    break;
                case successCondition.All:
                    if (check.arrived && check.collected && check.talked && check.inTime)
                        return true;
                    break;
                case successCondition.None:
                    return true;
            }
            return false;
        }

        private void Successed(QuestSuccessCheck successedQuest)
        {
            foreach (var quest in quest.NodeLinks.Where(x => x.BaseNodeGUID == successedQuest.quest.NodeGUID))
                canAcceptQuest.Add(quest);

            Destroy(questContainer.GetChild(AcceptedQuests.IndexOf(successedQuest)).gameObject);
            AcceptedQuests.Remove(successedQuest);
            Debug.Log("Äù½ºÆ® ¿Ï·á!");
        }

        private void UpdateCollectQuestText()
        {
            for(int i = 0; i < AcceptedQuests.Count(); ++i)
            {
                var quest = AcceptedQuests[i].quest;
                string tmpText = quest.QuestText;

                if ((quest.successConditionEnum & successCondition.COLLECT) == successCondition.COLLECT)
                {
                    int count = 0;
                    int collectNumber = quest.successCondition.collection.Count();
                    for (int j = 0; j < collectObjectContainer.childCount; ++j)
                        if (collectObjectContainer.GetChild(j).gameObject.activeSelf == true)
                            ++count;

                    tmpText += " (" + (collectNumber - count).ToString() + "/" + collectNumber.ToString() + ")";
                }

                var questText = questContainer.GetChild(i);
                questText.GetComponent<TextMeshProUGUI>().text = tmpText;
            }    
        }
    }
}