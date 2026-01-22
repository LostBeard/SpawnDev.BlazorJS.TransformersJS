using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class DepthEstimationResult : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public DepthEstimationResult(IJSInProcessObjectReference _ref) : base(_ref) { }

        public DepthEstimationDepth Depth => JSRef!.Get<DepthEstimationDepth>("depth");
        /// <summary>
        /// The raw depth map predicted by the model.
        /// </summary>
        public Tensor PredictedDepth => JSRef!.Get<Tensor>("predicted_depth");
    }
}
