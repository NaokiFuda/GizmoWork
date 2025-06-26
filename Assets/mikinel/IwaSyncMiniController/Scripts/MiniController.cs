using HoshinoLabs.IwaSync3.Udon;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace mikinel.vrc.IwaSyncMiniController
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class MiniController : UdonSharpBehaviour
    {
        public int mode = MODE_DEFAULT;
        
        public VideoCore core;

        [Space] 
        public GameObject muteOn;
        public GameObject muteOff;
        public Slider volumeSlider;
        public Button reloadButton;

        private bool _isDragging = false;

        public const int MODE_DEFAULT = 0;
        public const int MODE_RESYNC_ONLY = 1;

        private void Update()
        {
            var muted = core.muted;
            var unMuted = !muted;
            var isPlaying = core.isPlaying;
            var isError = core.isError;
            var flag = isPlaying || (isError && core.errorRetry == 0);

            if (muteOn != null)
            {
                if (muteOn.activeSelf != unMuted)
                    muteOn.SetActive(unMuted);                
            }

            if (muteOff != null)
            {
                if (muteOff.activeSelf != muted)
                    muteOff.SetActive(muted);    
            }

            if (volumeSlider != null)
            {
                volumeSlider.minValue = core.minVolume;
                volumeSlider.maxValue = core.maxVolume;

                if (!_isDragging)
                    volumeSlider.SetValueWithoutNotify(core.volume);   
            }

            if (reloadButton != null)
            {
                if (reloadButton.interactable != flag)
                    reloadButton.interactable = flag;
            }
        }

        public void BeginDrag() => _isDragging = true;
        public void EndDrag() => _isDragging = false;

        public void OnUpdateVolume() => core.volume = volumeSlider.value;
    }
}