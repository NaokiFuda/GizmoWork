
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FitSightAndFade : UdonSharpBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] Camera mainCamera;
    [SerializeField] SpriteRenderer image;
    [SerializeField] PlayerRayManager playerRayManager;
    [SerializeField] float hitRadius = 1f;
    [SerializeField] float maxDistance = 1f;
     bool _effectWorked;
    public bool GetEffectWorked() {  return _effectWorked; }

    bool _effectFinished;
    public bool GetEffectFinished() { return _effectFinished; }
    public void Activate()
    {
        RaycastHit hit;
        Ray ray = playerRayManager.GetPlayerRay();
        Debug.Log(ray);

        var playerHead = playerRayManager.GetPlayerHead();
        var targetDistance = maxDistance;
        Physics.SphereCast(ray, hitRadius, out hit, maxDistance);

        if (hit.collider == null) targetDistance = hit.distance;

        transform.position = playerHead.position + ray.direction * targetDistance;
        transform.rotation = Quaternion.LookRotation(playerHead.position - transform.position);
        transform.localScale = Vector3.one * targetDistance;
        HitAction();
    }
    public void HitAction()
    {
        _alpha = 1f;
        image.color = new Color(1, 1, 1, 1);
        audioSource.Play();
        //Debug.Log(audioSource.isPlaying);
        //_isFading = true; ; return;
        _effectWorked = true;
        if (IsObjectInView(gameObject, mainCamera)) _isFading = true; 
        else AdjastObjInView();
    }
    public void AdjastObjInView()
    {
        if (!IsObjectInView(gameObject, mainCamera))
        {
            Ray ray = playerRayManager.GetPlayerRay();
            var playerHead = playerRayManager.GetPlayerHead();

            if (Vector3.SqrMagnitude(playerHead.position - transform.position) <= Vector3.SqrMagnitude(ray.direction * 0.5f))
            {
                transform.position = ray.direction * 0.5f + playerHead.position;
                transform.rotation = Quaternion.LookRotation(playerHead.position - transform.position);
                transform.localScale = Vector3.one * 0.5f;
                _isFading = true;
                return;
            }

            transform.position += ray.direction * -0.05f;
            transform.rotation = Quaternion.LookRotation(playerHead.position - transform.position);
            transform.localScale = Vector3.one * Vector3.Distance(playerHead.position , transform.position);

            SendCustomEventDelayedSeconds(nameof(AdjastObjInView), 0.01f);
        }
        else _isFading = true; 
    }
    public bool IsObjectInView(GameObject obj, Camera camera)
    {
        Renderer renderer = obj.GetComponent<SpriteRenderer>();
        if (renderer == null) return false;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

    [SerializeField] float fadeSpeed = 0.1f;
    [SerializeField] float fadeRate = 0.1f;
    float _alpha = 1f;
    bool _isFading;
    float _fadeTimer;
    private void Update()
    {
        if (_isFading)
        {
            _fadeTimer += Time.deltaTime;

            if (_fadeTimer >= fadeSpeed)
            {
                _fadeTimer = 0f;
                _alpha -= fadeRate;

                if (_alpha <= 0)
                {
                    _alpha = 0;
                    _isFading = false;
                    _effectFinished = true;
                }

                image.color = new Color(1, 1, 1, _alpha);
            }
        }
    }
}
