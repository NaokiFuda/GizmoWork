#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UdonSharp;

public class SimpleDustLayerGenerator : EditorWindow
{
    private GameObject targetObject;
    private float distanceOffset = 0.005f;
    private Material dustMaterial;
    private bool generateUV = true;
    private bool useVertexColor = true;

    [MenuItem("VRChat/Dust Effect/Generate Dust Layer")]
    static void Init()
    {
        SimpleDustLayerGenerator window = (SimpleDustLayerGenerator)EditorWindow.GetWindow(typeof(SimpleDustLayerGenerator));
        window.titleContent = new GUIContent("Dust Layer Generator");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Dust Layer Generator", EditorStyles.boldLabel);

        targetObject = (GameObject)EditorGUILayout.ObjectField("Target Mesh", targetObject, typeof(GameObject), true);
        distanceOffset = EditorGUILayout.FloatField("Distance Offset", distanceOffset);
        dustMaterial = (Material)EditorGUILayout.ObjectField("Dust Material", dustMaterial, typeof(Material), false);
        generateUV = EditorGUILayout.Toggle("Generate UVs", generateUV);
        useVertexColor = EditorGUILayout.Toggle("Use Vertex Colors for Edges", useVertexColor);

        if (GUILayout.Button("Generate Dust Layer") && targetObject != null)
        {
            GenerateDustLayer();
        }
    }

    void GenerateDustLayer()
    {
        MeshFilter meshFilter = targetObject.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("Target object doesn't have a valid mesh!");
            return;
        }

        // 元のメッシュを取得
        Mesh originalMesh = meshFilter.sharedMesh;

        // 新しい埃メッシュを作成
        Mesh dustMesh = new Mesh();
        dustMesh.name = originalMesh.name + "_Dust";

        // 頂点とUVをコピー
        Vector3[] vertices = originalMesh.vertices;
        Vector3[] normals = originalMesh.normals;
        Vector2[] uvs = generateUV ? originalMesh.uv : new Vector2[vertices.Length];
        int[] triangles = originalMesh.triangles;

        // 頂点を法線方向にオフセット
        Vector3[] offsetVertices = new Vector3[vertices.Length];
        Color[] vertexColors = new Color[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            offsetVertices[i] = vertices[i] + normals[i] * distanceOffset;

            // 単純なエッジ検出 - 全ての頂点を白で初期化
            vertexColors[i] = Color.white;
        }

        // エッジ検出（シンプルなバージョン）
        if (useVertexColor)
        {
            // エッジとその近傍を検出するためのシンプルな方法
            bool[] isEdge = new bool[vertices.Length];

            // 三角形のエッジを検出
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int a = triangles[i];
                int b = triangles[i + 1];
                int c = triangles[i + 2];

                // 各頂点の法線の差を計算
                float abDot = Vector3.Dot(normals[a], normals[b]);
                float bcDot = Vector3.Dot(normals[b], normals[c]);
                float caDot = Vector3.Dot(normals[c], normals[a]);

                // 法線の差が大きい場合、エッジとして検出
                if (abDot < 0.8f) { isEdge[a] = true; isEdge[b] = true; }
                if (bcDot < 0.8f) { isEdge[b] = true; isEdge[c] = true; }
                if (caDot < 0.8f) { isEdge[c] = true; isEdge[a] = true; }
            }

            // エッジ検出結果を頂点カラーに適用
            for (int i = 0; i < vertices.Length; i++)
            {
                if (isEdge[i])
                {
                    vertexColors[i] = new Color(0.3f, 0.3f, 0.3f, 1.0f);
                }
            }
        }

        // 新しいメッシュにデータを設定
        dustMesh.vertices = offsetVertices;
        dustMesh.triangles = triangles;
        dustMesh.normals = normals;
        if (generateUV) dustMesh.uv = uvs;
        dustMesh.colors = vertexColors;

        // 最適化
        dustMesh.RecalculateBounds();

        // 埃レイヤーのGameObjectを作成
        GameObject dustObject = new GameObject(targetObject.name + "_DustLayer");
        dustObject.transform.SetParent(targetObject.transform);
        dustObject.transform.localPosition = Vector3.zero;
        dustObject.transform.localRotation = Quaternion.identity;
        dustObject.transform.localScale = Vector3.one;

        // メッシュコンポーネントを追加
        MeshFilter dustMeshFilter = dustObject.AddComponent<MeshFilter>();
        dustMeshFilter.sharedMesh = dustMesh;

        MeshRenderer dustRenderer = dustObject.AddComponent<MeshRenderer>();
        if (dustMaterial != null)
        {
            dustRenderer.sharedMaterial = dustMaterial;
        }
        else
        {
            // デフォルトのマテリアルを作成
            Material defaultMaterial = new Material(Shader.Find("Custom/SimpleDustOverlay"));
            defaultMaterial.SetColor("_DustColor", new Color(0.7f, 0.7f, 0.7f, 0.8f));
            dustRenderer.sharedMaterial = defaultMaterial;

            // アセットとしてマテリアルを保存
            if (!AssetDatabase.IsValidFolder("Assets/DustLayers"))
            {
                AssetDatabase.CreateFolder("Assets", "DustLayers");
            }
            AssetDatabase.CreateAsset(defaultMaterial, "Assets/DustLayers/DefaultDustMaterial.mat");
        }

        // コライダーを追加
        BoxCollider dustCollider = dustObject.AddComponent<BoxCollider>();
        dustCollider.isTrigger = true;
        dustCollider.size = dustMesh.bounds.size;
        dustCollider.center = dustMesh.bounds.center;

        // UdonSharpの埃インタラクションコンポーネントを追加
        UdonSharpBehaviour dustInteraction = dustObject.AddComponent<DustInteraction>();

        Selection.activeGameObject = dustObject;
        Debug.Log("埃レイヤーが正常に生成されました！");

        // アセットとしてメッシュを保存
        if (!AssetDatabase.IsValidFolder("Assets/DustLayers"))
        {
            AssetDatabase.CreateFolder("Assets", "DustLayers");
        }

        AssetDatabase.CreateAsset(dustMesh, "Assets/DustLayers/" + dustMesh.name + ".asset");
        AssetDatabase.SaveAssets();
    }
}
#endif