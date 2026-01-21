using Microsoft.JSInterop;
using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class TextToImagePipeline : Pipeline
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public TextToImagePipeline(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Run the TextToImagePipeline with the specified source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Task<TextToImagePipelineResult> Call(string source, TextToImageOptions options) => _Call<TextToImagePipelineResult>(source, options);
    }
    public class TextToImagePipelineResult : JSObject
    {
        /// <inheritdoc/>
        public TextToImagePipelineResult(IJSInProcessObjectReference _ref) : base(_ref) { }

        public string[] Images => JSRef!.Get<string[]>("images");
    }
    public class TextToImageOptions
    {
        [JsonPropertyName("height")]
        public int Height { get; set; } = 512;
        [JsonPropertyName("width")]
        public int Width { get; set; } = 512;
        [JsonPropertyName("num_ingerence_steps")]
        public int NumInferenceSteps { get; set; } = 50;
        [JsonPropertyName("guidance_scale")]
        public double GuidanceScale { get; set; } = 7.5;
        [JsonPropertyName("num_images_per_prompt")]
        public int NumImagesPerPrompt { get; set; } = 1;
    }
}
