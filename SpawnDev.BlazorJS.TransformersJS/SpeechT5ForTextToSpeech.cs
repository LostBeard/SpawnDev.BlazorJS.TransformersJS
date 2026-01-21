using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.TransformersJS.ONNX;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class SpeechT5ForTextToSpeech : Pipeline
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public SpeechT5ForTextToSpeech(IJSInProcessObjectReference _ref) : base(_ref) { }
        public static Task<SpeechT5ForTextToSpeech> FromPretrained(string modelId, FromPretrainedOptions? options = null) => JS.CallAsync<SpeechT5ForTextToSpeech>("Transformers.SpeechT5ForTextToSpeech.from_pretrained", modelId, options);
        public static Task<SpeechT5ForTextToSpeech> FromPretrained(string modelId, PipelineOptions? options = null) => JS.CallAsync<SpeechT5ForTextToSpeech>("Transformers.SpeechT5ForTextToSpeech.from_pretrained", modelId, options);
        public Task<GenerateSpeechResult> GenerateSpeech(TensorProxy<BigInt64Array> inputIds, Tensor<Float32Array> speakerEmbeddings, GenerateSpeechOptions options) => JSRef!.CallAsync<GenerateSpeechResult>("generate_speech", inputIds, speakerEmbeddings, options);
    }
}
