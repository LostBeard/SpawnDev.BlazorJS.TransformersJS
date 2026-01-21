using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class TensorToDataUrlOptions
    {
        /// <summary>
        /// Describes the image format represented in RGBA color space.<br/>
        /// ImageFormat: "RGB" | "RGBA" | "BGR" | "RBG"
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("format")]
        public string? Format { get; set; }
        /// <summary>
        /// Describes normalization parameters when preprocessing the image as model input.<br/>
        /// Data element are ranged from 0 to 255.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("norm")]
        public NormalizationParameters? Norm { get; set; }
        /// <summary>
        /// Describes the tensor layout when representing data of one or more image(s).<br/>
        /// ImageTensorLayout: "NHWC" | "NCHW"
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("tensorLayout")]
        public string? TensorLayout { get; set; }
    }
}
