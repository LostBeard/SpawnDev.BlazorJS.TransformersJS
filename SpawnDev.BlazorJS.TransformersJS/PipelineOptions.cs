using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class PipelineOptions
    {
        /// <summary>
        /// The device to run the pipeline on.<br/>
        /// Possible options:<br/>
        /// "webgpu"<br/>
        /// "webgl"<br/>
        /// "webnn"<br/>
        /// "wasm"
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("device")]
        public string? Device { get; set; }

        /// <summary>
        /// Callback function to be called during model loading.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("progress_callback")]
        public ActionCallback<ModelLoadProgress>? OnProgress { get; set; }

        /// <summary>
        /// The data type to use for the model.<br/>
        /// Possible values: "fp32", "fp16", "q8", "q4"
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("dtype")]
        public Union<FromPretrainedSubOptions, string>? Dtype { get; set; }
    }
}
