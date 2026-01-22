using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class AutoModelResult : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public AutoModelResult(IJSInProcessObjectReference _ref) : base(_ref) { }

        public Tensor<Float32Array> Heatmaps => JSRef!.Get<Tensor<Float32Array>>("heatmaps");
    }
}
