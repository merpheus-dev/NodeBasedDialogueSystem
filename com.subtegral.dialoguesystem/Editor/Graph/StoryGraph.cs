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
        private string _fileName;
        private string _filePath;

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
        
        private void RegenerateToolbar()
        {
            // remove the old toolbar
            rootVisualElement.Remove(rootVisualElement.Q<Toolbar>());
            // generate a new toolbar
            GenerateToolbar();
        }
        
        private void GenerateToolbar()
        {
            var toolbar           = new Toolbar();
            toolbar.Add(new Button(() => RequestDataOperation(0)) {text  = "New"});
            toolbar.Add(new Button(() => RequestDataOperation(1)) {text  = "Save"});
            toolbar.Add(new Button(() => RequestDataOperation(2)) {text = "Load"});
            if (_fileName != string.Empty) {
                var fileNameTextField = new Label($"File Name: {_fileName}");
                toolbar.Add(fileNameTextField);
            }
            rootVisualElement.Add(toolbar);
        }

        private void RequestDataOperation(byte option)
        {
            var saveUtility = GraphSaveUtility.GetInstance(_graphView);
            switch (option) {
                case 0: 
                {
                    _fileName = string.Empty;
                    _filePath = string.Empty;
                    rootVisualElement.Remove(_graphView);
                    ConstructGraphView();
                    RegenerateToolbar();
                    GenerateMiniMap();
                    GenerateBlackBoard();
                    break;
                }
                case 1: 
                {
                    if (_filePath != string.Empty) {
                        saveUtility.SaveGraph(_filePath);
                    } else saveUtility.SaveGraph(out _filePath);
                    
                    Debug.Log($"Saved Narrative at: {_filePath}");
                    _fileName = _filePath.Split('/').Last();
                    _fileName = _fileName[..^6];
                    RegenerateToolbar();
                    break;
                }
                case 2:
                {
                    saveUtility.LoadNarrative(out _filePath, out _fileName);
                    RegenerateToolbar();
                    break;
                }
            }
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            GenerateMiniMap();
            GenerateBlackBoard();
        }

        private void GenerateMiniMap()
        {
            var miniMap = new MiniMap {anchored = true};
            var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
            miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
            _graphView.Add(miniMap);
        }

        private void GenerateBlackBoard()
        {
            var blackboard = new Blackboard(_graphView);
            blackboard.Add(new BlackboardSection {title = "Exposed Variables"});
            blackboard.addItemRequested = _ =>
            {
                _graphView.AddPropertyToBlackBoard(ExposedProperty.CreateInstance(), false);
            };
            blackboard.editTextRequested = (_, element, newValue) =>
            {
                var oldPropertyName = ((BlackboardField) element).text;
                if (_graphView.ExposedProperties.Any(x => x.PropertyName == newValue))
                {
                    EditorUtility.DisplayDialog("Error", "This property name already exists, please chose another one.",
                        "OK");
                    return;
                }

                var targetIndex = _graphView.ExposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
                _graphView.ExposedProperties[targetIndex].PropertyName = newValue;
                ((BlackboardField) element).text = newValue;
            };
            blackboard.SetPosition(new Rect(10,30,200,300));
            _graphView.Add(blackboard);
            _graphView.Blackboard = blackboard;
        }

        private void OnDisable() => rootVisualElement.Remove(_graphView);
    }
}
