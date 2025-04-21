using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GrassInstanceManager : UdonSharpBehaviour
{
    [Header("草のメッシュ設定")]
    public Mesh[] grassMeshVariants;  // 複数の草メッシュバリエーション
    public Material grassMaterial;    // インスタンシング対応マテリアル

    [Header("生成設定")]
    public Transform terrainTransform; // 草を生やす地形
    public int grassCount = 1000;      // 草の総数
    public float spreadRadius = 10f;   // 広がり範囲

    [Header("インタラクション")]
    public float playerRadius = 2f;    // プレイヤーの影響範囲

    // 内部変数
    private Matrix4x4[][] matricesArray;  // メッシュごとの行列配列
    private int[] matricesCount;          // 各メッシュタイプの実際の数
    private MaterialPropertyBlock propertyBlock;
    private int maxInstances = 1023;      // GPUインスタンシングの制限

    void Start()
    {
        if (grassMeshVariants == null || grassMeshVariants.Length == 0)
        {
            Debug.LogError("草メッシュが設定されていません");
            return;
        }

        // プロパティブロックの初期化
        propertyBlock = new MaterialPropertyBlock();

        // 行列配列の初期化
        matricesArray = new Matrix4x4[grassMeshVariants.Length][];
        matricesCount = new int[grassMeshVariants.Length];

        for (int i = 0; i < grassMeshVariants.Length; i++)
        {
            // 各メッシュタイプごとに最大数の配列を事前に確保
            matricesArray[i] = new Matrix4x4[grassCount];
            matricesCount[i] = 0;
        }

        // 草インスタンスの生成
        GenerateGrassInstances();
    }

    void GenerateGrassInstances()
    {
        // ハッシュシードの設定（一度決めたら変えない）
        float seed = Random.Range(0, 1000f);
        propertyBlock.SetFloat("_VariationSeed", seed);

        // カウントをリセット
        for (int i = 0; i < matricesCount.Length; i++)
        {
            matricesCount[i] = 0;
        }

        for (int i = 0; i < grassCount; i++)
        {
            // ランダム位置の生成
            Vector2 randomCircle = Random.insideUnitCircle * spreadRadius;
            Vector3 position = terrainTransform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            // 地形の高さに合わせる
            RaycastHit hit;
            if (Physics.Raycast(position + Vector3.up * 10, Vector3.down, out hit, 20f))
            {
                position.y = hit.point.y;
            }

            // ランダムな回転
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);

            // ランダムなスケール
            float scale = Random.Range(0.8f, 1.2f);
            Vector3 scaleVec = new Vector3(scale, scale, scale);

            // バリエーションの選択（メッシュタイプ）
            int meshIndex = Random.Range(0, grassMeshVariants.Length);

            // 行列の作成
            Matrix4x4 matrix = Matrix4x4.identity;
            matrix.SetTRS(position, rotation, scaleVec);

            // 対応するメッシュの配列に追加
            if (matricesCount[meshIndex] < grassCount)
            {
                matricesArray[meshIndex][matricesCount[meshIndex]] = matrix;
                matricesCount[meshIndex]++;
            }
        }
    }

    // Update関数の描画部分を修正
    void Update()
    {
            // プレイヤー位置の更新
            propertyBlock.SetVector("_PlayerPosition", Networking.LocalPlayer.GetPosition());
            propertyBlock.SetFloat("_PlayerRadius", playerRadius);
        

        // インスタンスプロパティを明示的に設定（シェーダーの透明問題に対応）
        propertyBlock.SetFloat("_VariationSeed", 42.0f); // バリエーションシード値

        // メッシュタイプごとに描画を行う
        for (int meshIndex = 0; meshIndex < grassMeshVariants.Length; meshIndex++)
        {
            // 以下をお試しください - 簡略化された描画方法
            for (int i = 0; i < matricesCount[meshIndex]; i += maxInstances)
            {
                int count = Mathf.Min(maxInstances, matricesCount[meshIndex] - i);
                if (count <= 0) continue;

                Matrix4x4[] currentBatch = new Matrix4x4[count];
                System.Array.Copy(matricesArray[meshIndex], i, currentBatch, 0, count);

                // より具体的なパラメータでDrawMeshInstancedを呼び出す
                VRCGraphics.DrawMeshInstanced(
                    grassMeshVariants[meshIndex],
                    0, // submeshIndex
                    grassMaterial,
                    currentBatch,
                    count,
                    propertyBlock,
                    UnityEngine.Rendering.ShadowCastingMode.On, // シャドウキャスト有効
                    true, // 影を受ける
                    0, // レイヤー設定（デフォルト）
                    null // カメラを指定しない（すべてのカメラ）
                );
            }
        }
    }

    // 草の再生成（エディタ用およびパブリックメソッド）
    public void RegenerateGrass()
    {
        GenerateGrassInstances();
    }

}