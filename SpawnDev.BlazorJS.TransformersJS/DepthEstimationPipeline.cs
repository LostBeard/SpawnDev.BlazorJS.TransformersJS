using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// Depth estimation pipeline using any AutoModelForDepthEstimation. This pipeline predicts the depth of an image.<br/>
    /// https://huggingface.co/docs/transformers.js/api/pipelines#pipelinesdepthestimationpipeline<br/>
    /// https://github.com/huggingface/transformers.js/blob/6f43f244e04522545d3d939589c761fdaff057d4/src/pipelines.js#L2908<br/>
    /// </summary>
    public class DepthEstimationPipeline : Pipeline
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public DepthEstimationPipeline(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Run the DepthEstimationPipeline with the specified source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Task<DepthEstimationResult> Call(string source) => _Call<DepthEstimationResult>(source);

        /// <summary>
        /// Run the DepthEstimationPipeline with the specified source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Task<DepthEstimationResult> Call(RawImage source) => _Call<DepthEstimationResult>(source);
    }
}
