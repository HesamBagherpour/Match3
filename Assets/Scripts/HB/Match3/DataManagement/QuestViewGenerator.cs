using System.Collections.Generic;
using HB.Match3.View.Quest;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HB.Match3.DataManagement
{
    public class QuestViewGenerator
    {
        public Dictionary<string, GameObject> InitQuestView(GameObject itemPrefab, RectTransform parent, List<QuestViewData> questViewDatas)
        {
            var outPut = new Dictionary<string, GameObject>();
            foreach (var questItem in questViewDatas)
            {
                GameObject go = Object.Instantiate(itemPrefab, parent);
                go.GetComponent<Image>().sprite = questItem.QuestSprite;
                go.name = questItem.QuestName;
                go.GetComponent<UiQuestElement>().Init(questItem.count);
                outPut.Add(questItem.QuestName, go);
            }
            return outPut;
        }

        public Dictionary<string, GameObject> InitQuestView(GameObject uiRemainingItemPrefab, RectTransform objectives, List<QuestData> resultRemainingQuests, List<QuestViewData> questViewDatas)
        {
            var outPut = new Dictionary<string, GameObject>();
            foreach (var questData in resultRemainingQuests)
            {

                GameObject go = Object.Instantiate(uiRemainingItemPrefab, objectives);

                go.name = questData.QuestName;
                go.GetComponentInChildren<TextMeshPro>().text = questData.count.ToString();

                foreach (var questViewData in questViewDatas)
                {
                    if (questViewData.QuestName == questData.QuestName)
                    {
                        go.GetComponent<Image>().sprite = questViewData.QuestSprite;
                    }
                }

                outPut.Add(questData.QuestName, go);

            }

            return outPut;
        }
    }
}