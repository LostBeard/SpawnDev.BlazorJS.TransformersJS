using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// Image to Image pipeline using any `AutoModelForImageToImage`. This pipeline generates an image based on a previous image input.<br/>
    /// https://huggingface.co/docs/transformers.js/api/pipelines#pipelinesimagetoimagepipeline<br/>
    /// </summary>
    public class ImageToImagePipeline : Pipeline
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public ImageToImagePipeline(IJSInProcessObjectReference _ref) : base(_ref) { }

        /// <summary>
        /// Run the ImageToImagePipeline with the specified source image URL
        /// </summary>
        /// <param name="source">The image URL or path to transform</param>
        /// <returns></returns>
        public Task<RawImage> Call(string source) => _Call<RawImage>(source);

        /// <summary>
        /// Run the ImageToImagePipeline with the specified RawImage source
        /// </summary>
        /// <param name="source">The RawImage to transform</param>
        /// <returns></returns>
        public Task<RawImage> Call(RawImage source) => _Call<RawImage>(source);
    }
}