using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System;

public class DustInteraction : UdonSharpBehaviour
{
    [Header("レイヤー設定")]
    [SerializeField] private Renderer dustLayerRenderer;
    [SerializeField] private float dustRemovalSpeed = 0.1f;
    [SerializeField] private float dustRestoreSpeed = 0.01f;
    [SerializeField] private bool enableDustRestore = false;

    [Header("通知設定")]
    [SerializeField] private ActivateRenderer notificationTarget;
    UdonBehaviour[] _targetScripts;
    [SerializeField] private string notificationEventName = "OnDustRemoved";
    //[SerializeField] private string notificationClassName = "ActivateRenderer";
    [SerializeField] private float dustThreshold = 0.3f;

    [Header("デバッグ")]
    [SerializeField] private bool showDebug = false;

    // シェーダープロパティ名
    private string dustAmountProperty = "_DustAmount";

    // 状態管理
    private float currentDustAmount = 1.0f;
    private bool notificationSent = false;

    private void Start()
    {
        // 初期値を設定
        if (dustLayerRenderer != null)
        {
            currentDustAmount = 1.0f;
            dustLayerRenderer.material.SetFloat(dustAmountProperty, currentDustAmount);
            if (notificationTarget != null) _targetScripts = notificationTarget.GetComponents<UdonBehaviour>();
        }
        else
        {
            Debug.Log("DustInteraction: 埃レイヤーレンダラーが設定されていません");
        }
    }
    public void SwitchDustRestore()
    {
        enableDustRestore = !enableDustRestore;
    }

    private void Update()
    {
        // 埃を自然に戻す（オプション機能）
        if (enableDustRestore && currentDustAmount < 1.0f)
        {
            currentDustAmount = Mathf.Min(1.0f, currentDustAmount + dustRestoreSpeed * Time.deltaTime);
            dustLayerRenderer.material.SetFloat(dustAmountProperty, currentDustAmount);

            // 埃が一定量戻ったら通知フラグをリセット
            if (currentDustAmount > dustThreshold && notificationSent)
            {
                notificationSent = false;
                ResetDust();
                if (showDebug) Debug.Log("DustInteraction: 通知フラグをリセットしました");
            }
        }
    }

    // コライダーとの接触で埃を払う処理
    public override void OnPlayerTriggerStay(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            RemoveDust(Time.deltaTime);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (Networking.GetOwner(other.gameObject) == Networking.LocalPlayer && other.gameObject.layer == 26)
        {
            RemoveDust(Time.deltaTime);
        }
    }

    // 手動で埃を払う公開メソッド
    public void RemoveDust(float amount)
    {
        if (dustLayerRenderer != null)
        {
            // 埃の量を減らす
            currentDustAmount = Mathf.Max(0.0f, currentDustAmount - dustRemovalSpeed * amount);
            dustLayerRenderer.material.SetFloat(dustAmountProperty, currentDustAmount);

            // 閾値を下回ったら通知を送信
            if (currentDustAmount <= dustThreshold && !notificationSent)
            {
                SendNotification();
                notificationSent = true;
            }
        }
    }

    
    // 通知を送信
    public void SendNotification()
    {
        if (notificationTarget != null)
        {
            /*
            Type targetType = Type.GetType(notificationClassName + ", Assembly-CSharp");
            
            foreach (UdonBehaviour target in _targetScripts)
            {
                if(target.GetComponent(targetType) )
                    target.SendCustomEvent(notificationEventName);
            }
            */
            notificationTarget.SendCustomEvent(notificationEventName);
            if (showDebug) Debug.Log("DustInteraction: 通知を送信しました");
        }
    }

    // 埃の量をリセット
    public void ResetDust()
    {
        currentDustAmount = 1.0f;
        if (dustLayerRenderer != null)
        {
            dustLayerRenderer.material.SetFloat(dustAmountProperty, currentDustAmount);
        }
        SendNotification();
        notificationSent = false;
    }
}