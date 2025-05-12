using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace HFantasy.Script.PostProcessing
{
       [System.Serializable, VolumeComponentMenu("Custom Post-processing/Claw Vignette")]
        public class ClawVignette : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter enable = new BoolParameter(true);
        public ColorParameter clawColor = new ColorParameter(Color.red, true);
        public FloatParameter intensity = new FloatParameter(1.0f, true);
        public FloatParameter size = new FloatParameter(0.8f, true);
        public TextureParameter clawTexture = new TextureParameter(null);
        public Vector2Parameter flowSpeed = new Vector2Parameter(Vector2.zero, true);
        public FloatParameter flowStrength = new FloatParameter(0.1f, true);

        public bool IsActive() => enable.value && intensity.value > 0;
        public bool IsTileCompatible() => false;
    }


}