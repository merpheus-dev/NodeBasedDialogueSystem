using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueParser : MonoBehaviour
{
    [SerializeField] private NarrativeDataWrapper dialogue;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button choicePrefab;
    [SerializeField] private Transform buttonContainer;
    
    private void Start()
    {
        var narrativeData = dialogue.NarrativeData.First(); //Entrypoint node
        ProceedToNarrative(narrativeData.TargetNodeGUID);
    }

    private void ProceedToNarrative(string narrativeDataGUID)
    {
        var text = dialogue.NarrativeTextData.Find(x => x.NodeGUID == narrativeDataGUID).DialogueText;
        var choices = dialogue.NarrativeData.Where(x => x.BaseNodeGUID == narrativeDataGUID);
        dialogueText.text = text;
        var buttons = buttonContainer.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            Destroy(buttons[i].gameObject);
        }
        foreach (var choice in choices)
        {
            var button = Instantiate(choicePrefab, buttonContainer);
            button.GetComponentInChildren<Text>().text = choice.PortName;
            button.onClick.AddListener(()=>ProceedToNarrative(choice.TargetNodeGUID));
        }
    }
}