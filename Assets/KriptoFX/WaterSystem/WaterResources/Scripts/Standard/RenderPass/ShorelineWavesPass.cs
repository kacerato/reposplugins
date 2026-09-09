using UnityEngine;
using UnityEngine.Rendering;

namespace KWS
{
    public class ShorelineWavesPass : WaterPass
    {
        ShorelineWavesPassCore _pass;

        public void Initialize()
        {
            IsInitialized           =  true;
            _pass                   =  new ShorelineWavesPassCore(WaterInstance);
            _pass.OnSetRenderTarget += OnSetRenderTarget;

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

        private void OnSetRenderTarget(CommandBuffer cmd, Camera cam, RenderTexture rt1, RenderTexture rt2)
        {
            KWS_SPR_CoreUtils.SetRenderTarget(cmd, rt1, rt2, rt1.depthBuffer, ClearFlag.Color, Color.black);
        }

        public override void Release()
        {
            if (_pass != null)
            {
                _pass.OnSetRenderTarget -= OnSetRenderTarget;
                _pass.Release();
            }

            IsInitialized = false;
        }
    }
}