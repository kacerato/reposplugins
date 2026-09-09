using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace KWS
{
    public static partial class KWS_CoreUtils
    {
        static bool CanRenderWaterForCurrentCamera_PlatformSpecific(Camera cam)
        {
            return true;
        }

        public static Vector2Int GetCameraRTHandleViewPortSize(Camera cam)
        {
            int width;
            int height;

            if (XRSettings.enabled)
            {
                width  = XRSettings.eyeTextureWidth;
                height = XRSettings.eyeTextureHeight;
            }
            else
            {
                width  = cam.pixelWidth;
                height = cam.pixelHeight;
            }

            return new Vector2Int(width, height);
          
        }

        public static bool CanRenderSinglePassStereo(Camera cam)
        {
            return XRSettings.enabled && 
                   (XRSettings.stereoRenderingMode == XRSettings.StereoRenderingMode.SinglePassInstanced && cam.cameraType != CameraType.SceneView);
        }

        public static bool IsSinglePassStereoEnabled()
        {
            return XRSettings.enabled && XRSettings.stereoRenderingMode == XRSettings.StereoRenderingMode.SinglePassInstanced;
        }

        public static void UniversalCameraRendering(WaterSystem waterInstance, Camera camera)
        {
            camera.Render();
        }

        public static void SetPlatformSpecificPlanarReflectionParams(Camera reflCamera)
        {
            
        }

        public static void UpdatePlatformSpecificPlanarReflectionParams(Camera reflCamera, WaterSystem waterInstance)
        {
            if (waterInstance.Settings.UseScreenSpaceReflection && waterInstance.Settings.UseAnisotropicReflections)
            {
                reflCamera.clearFlags      = CameraClearFlags.Color;
                reflCamera.backgroundColor = Color.black;
            }
            else
            {
                reflCamera.clearFlags      = CameraClearFlags.Skybox;
            }
        }

        public static void SetPlatformSpecificCubemapReflectionParams(Camera reflCamera)
        {
           
        }

        public static void SetComputeShadersDefaultPlatformSpecificValues(this CommandBuffer cmd, ComputeShader cs, int kernel)
        {
           
        }

    }
}