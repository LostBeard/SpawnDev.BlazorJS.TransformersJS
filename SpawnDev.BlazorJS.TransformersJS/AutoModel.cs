using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class AutoModel : Pipeline
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public AutoModel(IJSInProcessObjectReference _ref) : base(_ref) { }
        public static Task<AutoModel> FromPretrained(string modelId, FromPretrainedOptions? options = null) => JS.CallAsync<AutoModel>("Transformers.AutoModel.from_pretrained", modelId, options);
        public static Task<AutoModel> FromPretrained(string modelId, PipelineOptions? options = null) => JS.CallAsync<AutoModel>("Transformers.AutoModel.from_pretrained", modelId, options);
        public Task<AutoModelResult> Call(AutoImageProcessorResult source) => _Call<AutoModelResult>(source);
        public Task<T> Call<T>(AutoImageProcessorResult source) => _Call<T>(source);

        public AutoModelConfig Config => JSRef!.Get<AutoModelConfig>("config");
    }
}
