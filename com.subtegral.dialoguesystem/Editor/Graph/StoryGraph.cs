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
using System.IO;

namespace Subtegral.DialogueSystem.Editor
{
    public class StoryGraph : EditorWindow
    {
        private string _filePath = null;

        private StoryGraphView _graphView;
        private DialogueContainer _dialogueContainer;

        private void Update()
        {
            if (Selection.activeObject == null || Selection.activeObject.GetType() != typeof(DialogueContainer))
            {
                return;
            }
            var selectedGraph = Selection.activeObject as DialogueContainer;
            var path = AssetDatabase.GetAssetPath(selectedGraph.GetInstanceID());
            if (path != null && path != _filePath)
            {
                var saveUtility = GraphSaveUtility.GetInstance(_graphView);
                saveUtility.LoadNarrative(path);
                _filePath = path;
                UpdateTitle();
            }
        }

        [MenuItem("Graph/Narrative Graph")]
        public static void CreateGraphViewWindow()
        {
            var window = GetWindow<StoryGraph>();
            window.UpdateTitle();
        }

        private void ConstructGraphView()
        {
            _graphView = new StoryGraphView
            {
                name = _filePath != null ? Path.GetFileName(_filePath) : "Unsaved New Narrative"
            };
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            UpdateTitle();

            toolbar.Add(new Button(() => Save()) { text = "Save Data" });

            toolbar.Add(new Button(() => CreateNew()) { text = "New narrative" });
            toolbar.Add(new Button(() => _graphView.CreateNewDialogueNode("Dialogue Node")) { text = "New Node", });
            rootVisualElement.Add(toolbar);
        }

        private void UpdateTitle()
        {
            this.titleContent = new GUIContent(_filePath != null ? Path.GetFileName(_filePath) : "Unsaved New Narrative");
        }

        private void CreateNew()
        {
            var loadFilePath = EditorUtility.SaveFilePanelInProject("Create Narrative", "New narrative", "asset", "");

            var saveUtility = GraphSaveUtility.GetInstance(_graphView);
            _graphView = new StoryGraphView
            {
                name = "Narrative Graph"
            };
            _filePath = loadFilePath;
            UpdateTitle();
        }
        private void SaveAs()
        {
            var saveUtility = GraphSaveUtility.GetInstance(_graphView);
            // TODO: change file path only if save succeeded.
            _filePath = EditorUtility.SaveFilePanelInProject("New narrative", "New narrative", "asset", "Save As...");
            saveUtility.SaveNodes(_filePath);
            UpdateTitle();
        }
        private void Save()
        {
            if (_filePath == null)
            {
                SaveAs();
            }
            else
            {
                var saveUtility = GraphSaveUtility.GetInstance(_graphView);
                saveUtility.SaveNodes(_filePath);
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
            var miniMap = new MiniMap { anchored = true };
            miniMap.SetPosition(new Rect(10, 30, 200, 140));
            _graphView.Add(miniMap);
        }


        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }

    }
}