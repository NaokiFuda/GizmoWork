
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.U2D;

public class SerialNumCardSet : UdonSharpBehaviour
{
    [SerializeField] private Texture2D _sourceTexture;
    [SerializeField] private GameObject CardPrefab;
    [SerializeField] private GameObject CardFolder;
    private int i = 0;
    private int j = 0;
    private int k = 0;
    private Sprite[] divideds;
    

    [SerializeField/*, MinValue(1), LabelText("横の分割数")*/] int _columns = 1;
    [SerializeField/*, MinValue(1), LabelText("縦の分割数")*/] int _rows = 1;

    [SerializeField/*, ReadOnly*/] int _width;
    [SerializeField/*, ReadOnly*/] int _height;
    [SerializeField/*, ReadOnly*/] int _imageCount = -1;

    void Start()
    {
        if (_sourceTexture != null)
        {
            _width = _sourceTexture.width / _columns;
            _height = _sourceTexture.height / _rows;
        }
        
        _imageCount = _columns * _rows;
        divideds = new Sprite[_imageCount];
        for (i = 0; i < _imageCount; ++i)
        {
            if (i % _columns == 0 && i != 0) { j = 0; k++; }
            var rect = new Rect(_width * j, _height * k, _width, _height);
            Sprite sprite = Sprite.Create(_sourceTexture, rect, new Vector2(0.5f, 0.5f));
            divideds[i]= sprite;
            j++;
        }
        for (i = 0; i < _imageCount; ++i)
        {
                Instantiate(CardPrefab, CardFolder.transform.position + new Vector3(i * 0.1f, 0, 0), Quaternion.identity, CardFolder.transform);
                CardFolder.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = divideds[i];
            
        }

    }
}
