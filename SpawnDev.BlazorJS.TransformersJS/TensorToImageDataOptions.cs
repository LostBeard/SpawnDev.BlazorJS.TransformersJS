using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// Tensor options for ToImageData()
    /// </summary>
    public class TensorToImageDataOptions
    {
        /// <summary>
        /// The image format.<br/>
        /// Default "RGB"
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("format")]
        public string? Format { get; set; }
        /// <summary>
        /// The tensor layout.<br/>
        /// Default "NCHW"
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("tensorLayout")]
        public string? TensorLayout { get; set; }
    }
}
