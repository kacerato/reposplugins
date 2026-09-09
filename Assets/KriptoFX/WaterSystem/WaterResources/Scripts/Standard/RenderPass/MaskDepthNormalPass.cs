using UnityEngine;
using UnityEngine.Rendering;

namespace KWS
{
    public class MaskDepthNormalPass : WaterPass
    {
        MaskDepthNormalPassCore _pass;

        public void Initialize()
        {
            IsInitialized                   =  true;
            _pass                           =  new MaskDepthNormalPassCore(WaterInstance);
            _pass.OnInitializedRenderTarget += OnInitializedRenderTarget;

            _camEvent = CameraEvent.BeforeForwardAlpha;
            if (_cmd == null) _cmd = new CommandBuffer() {name = _pass.PassName};
        }

        public override void Execute(Camera cam)
        {
            if (!IsInitialized) Initialize();
            _cmd.Clear();
            _pass.Execute(cam, _cmd);
            cam.AddCommandBuffer(_camEvent, _cmd);
        }

        private void OnInitializedRenderTarget(CommandBuffer cmd, KWS_RTHandle rt1, KWS_RTHandle rt2, KWS_RTHandle rt3)
        {
            KWS_SPR_CoreUtils.SetRenderTarget(cmd, rt1, rt2, rt3, ClearFlag.All, Color.black);
        }


        public override void Release()
        {
            if (_pass != null)
            {
                _pass.OnInitializedRenderTarget -= OnInitializedRenderTarget;
                _pass.Release();
            }

            IsInitialized = false;
        }
    }
}