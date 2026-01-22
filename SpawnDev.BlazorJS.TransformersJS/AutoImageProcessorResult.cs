using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class AutoImageProcessorResult : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public AutoImageProcessorResult(IJSInProcessObjectReference _ref) : base(_ref) { }
        public List<(int height, int width)> OriginalSizes => JSRef!.Get<List<(int height, int width)>>("original_sizes");
        public Tensor<Float32Array> PixelValues => JSRef!.Get<Tensor<Float32Array>>("pixel_values");
        public List<(int height, int width)> ReshapedInputSizes => JSRef!.Get<List<(int height, int width)>>("reshaped_input_sizes");
    }
}
