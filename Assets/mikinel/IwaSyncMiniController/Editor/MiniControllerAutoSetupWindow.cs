using System.Linq;
using HoshinoLabs.IwaSync3.Udon;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using IwaSync3 = HoshinoLabs.IwaSync3.IwaSync3;

namespace mikinel.vrc.IwaSyncMiniController.Editor
{
    public class MiniControllerAutoSetupWindow : EditorWindow
    {
        private static MiniControllerAutoSetupWindow miniControllerAutoSetupWindow; //複数タブ表示不可能用

        [FormerlySerializedAs("_targetMiniController")]
        public MiniController targetMiniController;

        private Vector2 _scrollPosition;

        private static Texture2D _customGreenTexture;

        private static Texture2D GetCustomGreenTexture()
        {
            if (_customGreenTexture == null)
            {
                _customGreenTexture = CreateTexture2D(new Color(0.22f, 0.35f, 0f));
            }

            return _customGreenTexture;
        }

        private void OnEnable() => EditorApplication.update += OnUpdate;
        private void OnDisable() => EditorApplication.update -= OnUpdate;

        private void OnUpdate()
        {
            Repaint();
        }

        [MenuItem("MikinelTools/iwaSyncMiniController/AutoSetup")]
        public static void ShowWindow()
        {
            var window = GetWindow<MiniControllerAutoSetupWindow>("AutoSetup");
            window.minSize = new Vector2(400, 300);
        }

        private void OnGUI()
        {
            var defaultColor = GUI.color;

            if (Application.isPlaying)
            {
                GUI.color = Color.red;
                EditorGUILayout.LabelField("AutoSetupはプレイ中には使用できません", EditorStyles.boldLabel);
                return;
            }

            //シーン上のvideoCoreを取得
            var iwaSync3Array = FindObjectsOfType<IwaSync3>().ToList();
            iwaSync3Array.Reverse();

            GUI.color = Color.cyan;
            EditorGUILayout.LabelField("iwaSyncMiniController AutoSetup", EditorStyles.boldLabel);
            GUI.color = defaultColor;

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("MiniController", GUILayout.Width(100));

            if (targetMiniController == null)
                GUI.color = Color.red;

            targetMiniController =
                (MiniController)EditorGUILayout.ObjectField(targetMiniController, typeof(MiniController), true);
            EditorGUILayout.EndHorizontal();

            if (targetMiniController != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button($"Select & Focus", GUILayout.Width(100)))
                {
                    SelectAndFocus(targetMiniController.gameObject);
                }

                EditorGUILayout.EndHorizontal();
            }

            GUI.color = defaultColor;

            EditorGUILayout.Space(10);

            if (targetMiniController == null)
            {
                EditorGUILayout.HelpBox("MiniControllerを指定してください", MessageType.Info);
            }
            else if (iwaSync3Array == null || iwaSync3Array.Count == 0)
            {
                EditorGUILayout.HelpBox("シーン上にiwaSyncが見つかりませんでした", MessageType.Info);
            }
            else
            {
                EditorGUILayout.LabelField("シーン上のiwaSync一覧", EditorStyles.boldLabel);

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                EditorGUILayout.BeginVertical();

                for (var i = 0; i < iwaSync3Array.Count; i++)
                {
                    EditorGUILayout.BeginVertical();
                    DrawElementView(i, iwaSync3Array[i],
                        targetMiniController.core == iwaSync3Array[i].GetComponentInChildren<VideoCore>());
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawElementView(int index, IwaSync3 iwaSync3, bool isSetNow)
        {
            var defaultColor = GUI.color;

            var elementStyle = new GUIStyle(GUI.skin.box);
            elementStyle.normal.background = isSetNow ? GetCustomGreenTexture() : elementStyle.normal.background;

            EditorGUILayout.BeginHorizontal(elementStyle);
            // EditorGUILayout.LabelField($"{index}", GUILayout.Width(20)); //index

            EditorGUILayout.LabelField($"{iwaSync3.gameObject.name} {(isSetNow ? "[現在設定中]" : string.Empty)}");

            if (GUILayout.Button("Select & Focus", GUILayout.Width(100)))
            {
                SelectAndFocus(iwaSync3.gameObject);
            }

            GUI.color = Color.yellow;
            if (GUILayout.Button($"Set"))
            {
                targetMiniController.AutoSetup(iwaSync3);
            }

            GUI.color = defaultColor;

            EditorGUILayout.EndHorizontal();
        }

        private static void SelectAndFocus(GameObject gameObject)
        {
            Selection.activeObject = gameObject;

            if (SceneView.sceneViews.Count < 1)
                return;

            var sceneView = (SceneView)SceneView.sceneViews[0];
            sceneView.LookAt(gameObject.transform.position, sceneView.rotation, 1f);
        }

        private static Texture2D CreateTexture2D(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();

            return texture;
        }
    }
}