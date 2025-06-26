using System.Reflection;
using UdonSharp;
using UnityEditor;
using UnityEngine;

namespace mikinel.vrc.IwaSyncMiniController.Editor
{
    [CustomEditor(typeof(MiniController))]
    public class MiniControllerEditor : UnityEditor.Editor
    {
        private MiniController _target;
        
        private SerializedProperty _modeProperty;
        private SerializedProperty _coreProperty;
        private SerializedProperty _muteOnProperty;
        private SerializedProperty _muteOffProperty;
        private SerializedProperty _volumeSliderProperty;
        private SerializedProperty _reloadProperty;
        
        public override void OnInspectorGUI()
        {
            FindProperties();
        
            serializedObject.Update();
            
            _modeProperty.intValue = EditorGUILayout.Popup("Mode", _modeProperty.intValue, new[]
            {
                "Default",
                "Resync Only"
            });

            switch (_modeProperty.intValue)
            {
                case MiniController.MODE_DEFAULT:
                    DrawGUI_Mode_Default();
                    break;
                case MiniController.MODE_RESYNC_ONLY:
                    DrawGUI_Mode_ResyncOnly();
                    break;
            }

            GUILayout.Space(10);
            if (GUILayout.Button("AutoSetup"))
            {
                var iwaSync3Array = FindObjectsOfType<HoshinoLabs.IwaSync3.IwaSync3>();

                if (iwaSync3Array.Length < 1 || iwaSync3Array[0] == null)
                {
                    var errorMsg = "iwaSyncがシーン上に見つかりませんでした";
                    EditorUtility.DisplayDialog("Error", errorMsg, "OK");
                    Debug.LogError(errorMsg);
                    return;
                }

                if (iwaSync3Array.Length == 1)
                {
                    //iwaSyncが1つだけ見つかった場合
                    var miniController = target as MiniController;
                    miniController.AutoSetup(iwaSync3Array[0]);
                }
                else
                {
                    //iwaSyncが複数見つかった場合
                    var msg = "iwaSyncが複数見つかりました\nセットアップウィンドウを使用してください";
                    EditorUtility.DisplayDialog("Info", msg, "OK");
                    Debug.Log(msg);
                    
                    //セットアップウィンドウを開く
                    EditorApplication.ExecuteMenuItem("MikinelTools/iwaSyncMiniController/AutoSetup");
                    var miniControllerAutoSetupWindow = EditorWindow.GetWindow<MiniControllerAutoSetupWindow>("AutoSetup");
                    miniControllerAutoSetupWindow.targetMiniController = target as MiniController;
                }
            }
            
            if (serializedObject.ApplyModifiedProperties())
                ApplyModifiedProperties();
        }
        
        private void DrawGUI_Mode_Default()
        {
            EditorGUILayout.PropertyField(_coreProperty);
            EditorGUILayout.PropertyField(_muteOnProperty);
            EditorGUILayout.PropertyField(_muteOffProperty);
            EditorGUILayout.PropertyField(_volumeSliderProperty);
            EditorGUILayout.PropertyField(_reloadProperty);
        }
        
        private void DrawGUI_Mode_ResyncOnly()
        {
            EditorGUILayout.PropertyField(_coreProperty);
            EditorGUILayout.PropertyField(_reloadProperty);
        }

        private void FindProperties()
        {
            _target = target as MiniController;
            
            _modeProperty = serializedObject.FindProperty("mode");
            _coreProperty = serializedObject.FindProperty("core");
            _muteOnProperty = serializedObject.FindProperty("muteOn");
            _muteOffProperty = serializedObject.FindProperty("muteOff");
            _volumeSliderProperty = serializedObject.FindProperty("volumeSlider");
            _reloadProperty = serializedObject.FindProperty("reloadButton");
        }
        
        private void ApplyModifiedProperties()
        {
            FindProperties();
            
            SetVariable(_target, "mode", _modeProperty.intValue);
            SetVariable(_target, "core", _coreProperty.objectReferenceValue);
            SetVariable(_target, "muteOn", _muteOnProperty.objectReferenceValue);
            SetVariable(_target, "muteOff", _muteOffProperty.objectReferenceValue);
            SetVariable(_target, "volumeSlider", _volumeSliderProperty.objectReferenceValue);
            SetVariable(_target, "reloadButton", _reloadProperty.objectReferenceValue);
        }
        
        private void SetVariable(UdonSharpBehaviour self, string symbolName, object value)
        {
            var field = self.GetType().GetField(symbolName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(self, value);
        }
    }
}