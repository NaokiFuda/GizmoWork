using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class WaterRippleController : UdonSharpBehaviour
{
    [SerializeField] private Material waterMaterial;

    // 時間の滑らかさの設定
    [SerializeField][Range(0.1f, 5.0f)] private float smoothTimeScale = 1.0f;
    // モバイル向けの時間スケール係数
    [SerializeField][Range(0.1f, 1.0f)] private float mobileTimeScaleFactor = 0.5f;

    // 事前に生成したノイズテクスチャを使用するオプション
    [SerializeField] private Texture2D preGeneratedNoiseTexture;
    // モバイル向けの低解像度テクスチャ（オプション）
    [SerializeField] private Texture2D mobileLowResNoiseTexture;

    // テクスチャサイズ設定
    [SerializeField] private int textureSize = 256;
    [SerializeField] private int mobileTextureSize = 64; // モバイル向けに小さいサイズ
    [SerializeField] private bool generateRuntimeTexture = false;

    // 波のオフセットランダム性
    [SerializeField][Range(0.0f, 1.0f)] private float offsetRandomness = 0.5f;
    [SerializeField][Range(0.0f, 1.0f)] private float mobileOffsetRandomness = 0.2f; // モバイル向け低減

    // モバイル最適化パラメータ
    [SerializeField][Range(0.0f, 1.0f)] private float mobileRippleStrength = 0.5f; // PCの値に対する倍率
    [SerializeField][Range(0.0f, 1.0f)] private float mobileWaveHeight = 0.5f; // PCの値に対する倍率
    [SerializeField][Range(0.0f, 1.0f)] private float mobileDistortionStrength = 0.4f; // PCの値に対する倍率

    // モバイルの更新頻度制御
    [SerializeField][Range(1, 4)] private int mobileUpdateInterval = 1; // 1=毎フレーム、2=2フレームに1回

    // フォールバック処理を使用するフラグ - 特別な理由がない限りfalseにする
    [SerializeField] private bool useFallbackOnMobile = false;

    // 元のパラメータ値を保存
    private float originalRippleStrength;
    private float originalWaveHeight;
    private float originalDistortionStrength;
    private float originalRippleScale;
    private float originalTimeScale;

    private Texture2D noiseTexture;
    private bool textureApplied = false;
    private bool isMobileUser = false;

    // 滑らかな時間更新のための変数
    private float animationTime = 0.0f;
    private float lastDeltaTime = 0.0f;

    void Start()
    {
        // モバイルユーザーかどうかを判定
        isMobileUser = !Networking.LocalPlayer.IsUserInVR();

        // 元のパラメータ値を保存
        if (waterMaterial != null)
        {
            originalRippleStrength = waterMaterial.GetFloat("_RippleStrength");
            originalWaveHeight = waterMaterial.GetFloat("_WaveHeight");
            originalDistortionStrength = waterMaterial.GetFloat("_DistortionStrength");
            originalRippleScale = waterMaterial.GetFloat("_RippleScale");
            originalTimeScale = smoothTimeScale;
        }

        // モバイル向け最適化設定を適用
        if (isMobileUser)
        {
            ApplyMobileOptimizations();
        }

        ApplyNoiseTexture();
        InitializeWaveOffsets();
    }

    private void ApplyMobileOptimizations()
    {
        if (waterMaterial != null)
        {
            // モバイル向けにシェーダーパラメータを軽量化（ただし極端な設定は避ける）
            waterMaterial.SetFloat("_RippleStrength", originalRippleStrength * mobileRippleStrength);
            waterMaterial.SetFloat("_WaveHeight", originalWaveHeight * mobileWaveHeight);
            waterMaterial.SetFloat("_DistortionStrength", originalDistortionStrength * mobileDistortionStrength);

            // 波の複雑さを少しだけ下げる（極端に下げない）
            waterMaterial.SetFloat("_RippleScale", originalRippleScale * 0.85f);

            // フォールバック処理の使用は最終手段（明滅の原因になる可能性あり）
            waterMaterial.SetFloat("_UseFallback", useFallbackOnMobile ? 1.0f : 0.0f);

            // 時間スケールを調整（極端に下げない）
            smoothTimeScale = originalTimeScale * mobileTimeScaleFactor;
        }
    }

    void OnEnable()
    {
        // 有効化されたときに再度テクスチャを適用
        if (textureApplied && noiseTexture != null)
        {
            ApplyExistingTexture();
        }
    }

    private void ApplyNoiseTexture()
    {
        if (isMobileUser && mobileLowResNoiseTexture != null)
        {
            // モバイル向け低解像度テクスチャを使用
            noiseTexture = mobileLowResNoiseTexture;
            waterMaterial.SetTexture("_NoiseMap", noiseTexture);
            textureApplied = true;
        }
        else if (preGeneratedNoiseTexture != null)
        {
            // 通常の事前生成テクスチャを使用
            noiseTexture = preGeneratedNoiseTexture;
            waterMaterial.SetTexture("_NoiseMap", noiseTexture);
            textureApplied = true;
        }
        else if (generateRuntimeTexture)
        {
            // ランタイムでテクスチャを生成（モバイルならサイズを小さく）
            int finalSize = isMobileUser ? mobileTextureSize : textureSize;
            noiseTexture = new Texture2D(finalSize, finalSize, TextureFormat.RGBA32, false);
            noiseTexture.name = "WaterNoiseTexture_Runtime";
            noiseTexture.wrapMode = TextureWrapMode.Repeat;
            noiseTexture.filterMode = FilterMode.Bilinear;
            GenerateNoiseTexture(isMobileUser);
            waterMaterial.SetTexture("_NoiseMap", noiseTexture);
            textureApplied = true;
        }
    }

    private void ApplyExistingTexture()
    {
        if (noiseTexture != null && waterMaterial != null)
        {
            waterMaterial.SetTexture("_NoiseMap", noiseTexture);
        }
    }

    private void InitializeWaveOffsets()
    {
        // モバイルの場合はランダム性を少しだけ減らす
        float randomFactor = isMobileUser ? mobileOffsetRandomness : offsetRandomness;

        // 初期オフセットをランダムに設定して同一感を減らす
        Vector4 initialOffsets = new Vector4(
            Random.Range(0f, 1f) * randomFactor,
            Random.Range(0f, 1f) * randomFactor,
            Random.Range(0f, 1f) * randomFactor,
            Random.Range(0f, 1f) * randomFactor
        );
        waterMaterial.SetVector("_WaveOffsets", initialOffsets);
    }

    void Update()
    {
        // モバイルの場合は更新頻度を下げる（ただし時間は毎フレーム更新）
        bool shouldUpdateRender = true;
        if (isMobileUser && mobileUpdateInterval > 1)
        {
            shouldUpdateRender = (Time.frameCount % mobileUpdateInterval == 0);
        }

        // 時間は常に更新（スキップフレームでも時間だけは更新）
        lastDeltaTime = Time.deltaTime;
        animationTime += lastDeltaTime * smoothTimeScale;
        float waveSpeed = waterMaterial.GetFloat("_WaveSpeed");
        float time = animationTime * waveSpeed;

        // シェーダーに時間値を常に渡す（これは毎フレーム行う）
        waterMaterial.SetFloat("_TimeValue", time);

        // レンダリング更新（オフセット値の更新）は間引く場合がある
        if (shouldUpdateRender)
        {
            // テクスチャが存在しないか、マテリアルに設定されていない場合は再度適用
            if ((noiseTexture != null) && (waterMaterial.GetTexture("_NoiseMap") != noiseTexture))
            {
                ApplyExistingTexture();
            }

            // 波のオフセット値を計算（モバイルなら少しだけ単純化）
            Vector4 waveOffsets = CalculateWaveOffsets(time, isMobileUser);
            waterMaterial.SetVector("_WaveOffsets", waveOffsets);
        }
    }

    private Vector4 CalculateWaveOffsets(float time, bool simplify)
    {
        if (simplify)
        {
            // モバイル向けに少しだけ単純化したオフセット計算
            // 元の値に近い周波数を維持して波の特性を保つ
            return new Vector4(
                Mathf.Sin(time * 0.22f) * 0.5f,
                Mathf.Cos(time * 0.35f) * 0.5f,
                Mathf.Sin(time * 0.16f + 0.7f) * 0.5f,
                Mathf.Cos(time * 0.27f + 0.3f) * 0.5f
            );
        }
        else
        {
            // 通常の高品質なオフセット計算
            return new Vector4(
                Mathf.Sin(time * 0.23f) * 0.5f,
                Mathf.Cos(time * 0.37f) * 0.5f,
                Mathf.Sin(time * 0.17f + 0.7f) * 0.5f,
                Mathf.Cos(time * 0.29f + 0.3f) * 0.5f
            );
        }
    }

    private void GenerateNoiseTexture(bool simplify)
    {
        int size = noiseTexture.width; // 既に設定されたサイズを使用

        if (simplify)
        {
            // モバイル向けに少しだけシンプルなノイズ生成
            // 元のものとあまり違いすぎないように調整
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float u = (float)x / size;
                    float v = (float)y / size;

                    // 元の周波数に近いノイズ生成
                    float noise1 = Mathf.PerlinNoise(u * 4.8f, v * 4.8f);
                    float noise2 = Mathf.PerlinNoise(u * 10f, v * 10f) * 0.5f;
                    float noise3 = Mathf.PerlinNoise(u * 6.5f + 3.5f, v * 6.5f + 2.2f) * 0.3f;

                    // ディテールは省くが、基本的な特性は維持
                    Color col = new Color(noise1, noise2, noise3, 1.0f);
                    noiseTexture.SetPixel(x, y, col);
                }
            }
        }
        else
        {
            // 通常の高品質なノイズテクスチャの生成
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float u = (float)x / size;
                    float v = (float)y / size;

                    // より自然な複合ノイズを使用
                    float noise1 = Mathf.PerlinNoise(u * 5f, v * 5f);
                    float noise2 = Mathf.PerlinNoise(u * 13f, v * 13f) * 0.5f;
                    float noise3 = Mathf.PerlinNoise(u * 7f + 3.7f, v * 7f + 2.3f) * 0.3f;

                    // さらに細かいディテールを追加
                    float detail = Mathf.PerlinNoise(u * 25f, v * 25f) * 0.1f;
                    noise1 = Mathf.Lerp(noise1, noise1 * detail, 0.3f);

                    Color col = new Color(noise1, noise2, noise3, 1.0f);
                    noiseTexture.SetPixel(x, y, col);
                }
            }
        }

        noiseTexture.Apply();
    }
}