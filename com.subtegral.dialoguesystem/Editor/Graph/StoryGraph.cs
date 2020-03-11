using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Subtegral.DialogueSystem.DataContainers;

namespace Subtegral.DialogueSystem.Editor
{
    public class StoryGraph : EditorWindow
    {
        private string _fileName = "New Narrative";

        private StoryGraphView _graphView;
        private DialogueContainer _dialogueContainer;

        [MenuItem("Graph/Narrative Graph")]
        public static void CreateGraphViewWindow()
        {
            var window = GetWindow<StoryGraph>();
            window.titleContent = new GUIContent("Narrative Graph");
        }

        private void ConstructGraphView()
        {
            _graphView = new StoryGraphView(this)
            {
                name = "Narrative Graph",
            };
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            var fileNameTextField = new TextField("File Name:");
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            toolbar.Add(fileNameTextField);

            toolbar.Add(new Button(() => RequestDataOperation(true)) {text = "Save Data"});

            toolbar.Add(new Button(() => RequestDataOperation(false)) {text = "Load Data"});
           // toolbar.Add(new Button(() => _graphView.CreateNewDialogueNode("Dialogue Node")) {text = "New Node",});
            rootVisualElement.Add(toolbar);
        }

        private void RequestDataOperation(bool save)
        {
            if (!string.IsNullOrEmpty(_fileName))
            {
                var saveUtility = GraphSaveUtility.GetInstance(_graphView);
                if (save)
                    saveUtility.SaveNodes(_fileName);
                else
                    saveUtility.LoadNarrative(_fileName);
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid File name", "Please Enter a valid filename", "OK");
            }
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            GenerateMiniMap();
        }

        private void GenerateMiniMap()
        {
            var miniMap = new MiniMap {anchored = true};
            miniMap.SetPosition(new Rect(10, 30, 200, 140));
            _graphView.Add(miniMap);
        }


        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }
       
    }
}