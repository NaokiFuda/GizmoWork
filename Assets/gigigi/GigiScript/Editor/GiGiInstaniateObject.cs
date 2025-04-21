using UnityEngine;
using UnityEditor;

public class GiGiInstaniateObject : EditorWindow
{
    public GameObject _cardPrefab; 
    public Sprite[] _cardSet;
    public GameObject _cardFolder;
    public int _duplicateCount = 0;
    public float _setCardSize = 0.05f;
    public Vector3 _texSize;
    public Vector3 _cardSize;
    public Vector3 _offset;
    public Vector3 _defaultRotation;

    [MenuItem("Tools/Spawn Cards")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GiGiInstaniateObject));
    }

    void OnGUI()
    {
        ScriptableObject target = this;

        SerializedObject so = new SerializedObject(target);

        SerializedProperty stringsProperty = so.FindProperty("_cardSet");

        EditorGUILayout.PropertyField(stringsProperty, true);

        so.ApplyModifiedProperties();


        GUILayout.Label("Card Spawner", EditorStyles.boldLabel);

        _cardPrefab = EditorGUILayout.ObjectField("Card Prefab", _cardPrefab, typeof(GameObject), false) as GameObject;

        _cardFolder = (GameObject)EditorGUILayout.ObjectField("Card Folder", _cardFolder, typeof(GameObject), true) ;
        _duplicateCount = EditorGUILayout.IntField("Deplicated Card Count", _duplicateCount);
        _setCardSize = EditorGUILayout.FloatField("Set Card Size", _setCardSize);
        _texSize = EditorGUILayout.Vector3Field("Texture Size", _texSize);
        _cardSize = EditorGUILayout.Vector3Field("Texture Size", _cardSize);
        _offset = EditorGUILayout.Vector3Field("Spawn Offset", _offset);
        _defaultRotation = EditorGUILayout.Vector3Field("Default Rotation", _defaultRotation);

        if (GUILayout.Button("Spawn Cards"))
        {
            SpawnCards();
        }
    }

    void SpawnCards()
    {
        if (_cardPrefab == null)
        {
            Debug.LogError("Card Prefab is not assigned.");
            return;
        }
        if (_cardSize == Vector3.zero)
        {
            _cardSize = new Vector3(_setCardSize, 1.0f, _setCardSize * 0.9f);
        }
        if (_texSize == Vector3.zero)
        {
            _texSize = new Vector3(_cardSet[0].bounds.size.x * 1.002f, _cardSet[0].bounds.size.y * 0.998f, 0.001f);
        }

        for (int i = 1; i < _cardSet.Length; i++)
        {
            int j = 0;
            while ( j <= _duplicateCount)
            {
                GameObject card = PrefabUtility.InstantiatePrefab(_cardPrefab) as GameObject;
                if (card != null)
                {
                    card.transform.position = _cardFolder.transform.position + _offset * (i - 1);
                    card.transform.SetParent(_cardFolder.transform);
                    card.name = "Card" + i +"_"+ j;
                    if (_defaultRotation.x > 1.0f)
                    {
                        card.transform.rotation = new Quaternion(_defaultRotation.x - (int)_defaultRotation.x, _defaultRotation.y, _defaultRotation.z, 1 - _defaultRotation.x - (int)_defaultRotation.x);
                    }
                    if (_defaultRotation.x < 0.0f)
                    {
                        card.transform.rotation = new Quaternion(_defaultRotation.x - (int)_defaultRotation.x, _defaultRotation.y, _defaultRotation.z, 1 + _defaultRotation.x - (int)_defaultRotation.x);
                    }
                    else
                    {
                        card.transform.rotation = new Quaternion(_defaultRotation.x, _defaultRotation.y, _defaultRotation.z, 1 - _defaultRotation.x);
                    }
                    card.transform.GetChild(2).transform.localScale = _texSize;
                    card.GetComponent<BoxCollider>().size = new Vector3(_texSize.x, 0.05f, _texSize.y);
                    card.transform.localScale = _cardSize;
                    card.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = _cardSet[0];
                    card.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = _cardSet[i];
                    card.SetActive(false);
                }
                j++;
            }
        }
    }
}