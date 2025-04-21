#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

// WaterRippleControllerと同じフォルダに配置する
[CustomEditor(typeof(WaterRippleController))]
public class NoiseTextureGenerator : Editor
{
    private int textureSize = 256;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Noise Texture Generator", EditorStyles.boldLabel);

        textureSize = EditorGUILayout.IntSlider("Texture Size", textureSize, 64, 1024);

        if (GUILayout.Button("Generate Noise Texture"))
        {
            GenerateAndSaveNoiseTexture();
        }
    }

    private void GenerateAndSaveNoiseTexture()
    {
        // テクスチャを生成
        Texture2D newTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float u = (float)x / textureSize;
                float v = (float)y / textureSize;

                float noise1 = Mathf.PerlinNoise(u * 5f, v * 5f);
                float noise2 = Mathf.PerlinNoise(u * 13f, v * 13f) * 0.5f;
                float noise3 = Mathf.PerlinNoise(u * 7f + 3.7f, v * 7f + 2.3f) * 0.3f;

                Color col = new Color(noise1, noise2, noise3, 1.0f);
                newTexture.SetPixel(x, y, col);
            }
        }
        newTexture.Apply();

        // アセットとして保存
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Noise Texture",
            "WaterNoiseTexture",
            "png",
            "Please enter a name for the noise texture."
        );

        if (path.Length > 0)
        {
            byte[] bytes = newTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();

            // テクスチャインポート設定を調整
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.isReadable = true;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.filterMode = FilterMode.Bilinear;
                importer.wrapMode = TextureWrapMode.Repeat;
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }

            // 生成したテクスチャを自動設定
            Texture2D savedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (savedTexture != null)
            {
                WaterRippleController controller = (WaterRippleController)target;
                SerializedObject so = new SerializedObject(controller);
                SerializedProperty texProp = so.FindProperty("preGeneratedNoiseTexture");
                texProp.objectReferenceValue = savedTexture;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(controller);
            }
        }

        GameObject.DestroyImmediate(newTexture);
    }
}
#endif