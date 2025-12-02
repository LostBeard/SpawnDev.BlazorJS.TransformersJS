using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS.ONNX
{
    public class RuntimeTensor : JSObject
    {
        /// <inheritdoc/>
        public RuntimeTensor(IJSInProcessObjectReference _ref) : base(_ref) { }
    }
}
