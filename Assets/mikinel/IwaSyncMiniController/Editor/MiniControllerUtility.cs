using HoshinoLabs.IwaSync3.Udon;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VRC.Udon;

namespace mikinel.vrc.IwaSyncMiniController.Editor
{
    public static class MiniControllerUtility
    {
        public static void AutoSetup(this MiniController miniController, HoshinoLabs.IwaSync3.IwaSync3 iwaSync3)
        {
            //UNDO
            Undo.RecordObject(miniController, "IwaSync MiniController AutoSetup");
            
            var videoCore = iwaSync3.GetComponentInChildren<VideoCore>();

            if (videoCore == null)
            {
                var errorMsg = "iwaSyncの子オブジェクトにvideoCoreが見つかりませんでした";
                EditorUtility.DisplayDialog("Error", errorMsg, "OK");
                Debug.LogError(errorMsg);
                return;
            }
            
            miniController.core = videoCore;
            
            var videoController = iwaSync3.GetComponentInChildren<VideoController>();
            var videoControllerUdon = videoController.GetComponent<UdonBehaviour>();

            if (videoController == null || videoControllerUdon == null)
            {
                ErrorDialog("iwaSyncの子オブジェクトにvideoControllerが見つかりませんでした");
                return;
            }

            if (miniController.muteOn != null)
            {
                var muteOnButton = miniController.muteOn.GetComponentInChildren<Button>();
                if (muteOnButton == null)
                {
                    ErrorDialog("muteOnの子オブジェクトにButtonが見つかりませんでした");
                    return;
                }
                
                muteOnButton.onClick.RemoveAllListeners();
                muteOnButton.onClick = new Button.ButtonClickedEvent();
                UnityEditor.Events.UnityEventTools.AddStringPersistentListener(
                    muteOnButton.onClick, videoControllerUdon.SendCustomEvent, nameof(videoController.MuteOn));
                EditorUtility.SetDirty(muteOnButton);
            }

            if (miniController.muteOff != null)
            {
                var muteOffButton = miniController.muteOff.GetComponentInChildren<Button>();
                if (muteOffButton == null)
                {
                    ErrorDialog("muteOffの子オブジェクトにButtonが見つかりませんでした");
                    return;
                }
                
                muteOffButton.onClick.RemoveAllListeners();
                muteOffButton.onClick = new Button.ButtonClickedEvent();
                UnityEditor.Events.UnityEventTools.AddStringPersistentListener(
                    muteOffButton.onClick, videoControllerUdon.SendCustomEvent, nameof(videoController.MuteOff));
                EditorUtility.SetDirty(muteOffButton);
            }

            if (miniController.reloadButton != null)
            {
                var reloadButton = miniController.reloadButton.GetComponentInChildren<Button>();
                if (reloadButton == null)
                {
                    ErrorDialog("reloadの子オブジェクトにButtonが見つかりませんでした");
                    return;
                }
                
                reloadButton.onClick.RemoveAllListeners();
                reloadButton.onClick = new Button.ButtonClickedEvent();
                UnityEditor.Events.UnityEventTools.AddStringPersistentListener(
                    reloadButton.onClick, videoControllerUdon.SendCustomEvent, nameof(videoController.Reload));
                EditorUtility.SetDirty(reloadButton);
            }

            //set dirty
            EditorUtility.SetDirty(miniController);

            //show done dialog
            EditorUtility.DisplayDialog("完了", "自動セットアップが完了しました", "OK");
        }

        public static void ErrorDialog(string message)
        {
            EditorUtility.DisplayDialog("Error", message, "OK");
            Debug.LogError(message);
        }
    }
}