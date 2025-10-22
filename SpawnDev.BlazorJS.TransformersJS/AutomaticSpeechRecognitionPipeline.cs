using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// Pipeline that aims at extracting spoken text contained within some audio.<br/>
    /// https://huggingface.co/docs/transformers.js/api/pipelines#module_pipelines.AutomaticSpeechRecognitionPipeline
    /// https://github.com/huggingface/transformers.js/blob/6f43f244e04522545d3d939589c761fdaff057d4/src/pipelines.js#L1719
    /// </summary>
    public class AutomaticSpeechRecognitionPipeline : Pipeline
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public AutomaticSpeechRecognitionPipeline(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Run the AutomaticSpeechRecognitionPipeline with the specified source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Task<AutomaticSpeechRecognitionResult> Call(string source) => _Call<AutomaticSpeechRecognitionResult>(source);
        /// <summary>
        /// Run the AutomaticSpeechRecognitionPipeline with the specified source
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Task<AutomaticSpeechRecognitionResult> Call(RawAudio source) => _Call<AutomaticSpeechRecognitionResult>(source);
    }

    public class AutomaticSpeechRecognitionResult : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public AutomaticSpeechRecognitionResult(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// The recognized text
        /// </summary>
        public string Text => JSRef!.Get<string>("text");
    }

    public class TextAudioPipelineConstructorArgs
    {

    }
}
