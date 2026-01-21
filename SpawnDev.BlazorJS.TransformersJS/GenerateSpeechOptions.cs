using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class GenerateSpeechOptions
    {
        /// <summary>
        /// The vocoder to use for the text-to-speech pipeline.
        /// </summary>
        [Required]
        [JsonPropertyName("vocoder")]
        public SpeechT5HifiGan? Vocoder { get; set; }
    }
}
