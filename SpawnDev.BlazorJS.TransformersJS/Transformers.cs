using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// Transformers.js exports
    /// </summary>
    public class Transformers : JSObject
    {
        /// <summary>
        /// The global variable name the import transformers.js module will be saved as
        /// </summary>
        public const string GlobalModuleName = nameof(Transformers);
        /// <summary>
        /// Transformers.js bundled with this library<br/>
        /// Downloaded from:<br/>
        /// https://cdn.jsdelivr.net/npm/@huggingface/transformers@3.8.1
        /// </summary>
        public static string LatestBundledVersionSrc { get; } = $"./_content/SpawnDev.BlazorJS.TransformersJS/transformers-3.8.1.js";
        /// <summary>
        /// Transformers.js CDN URL<br/>
        /// https://cdn.jsdelivr.net/npm/@huggingface/transformers<br/>
        /// To get a specific version use the @ tag:<br/>
        /// https://cdn.jsdelivr.net/npm/@huggingface/transformers@3.7.6
        /// </summary>
        public static string LatestCDNVersionSrc { get; } = $"https://cdn.jsdelivr.net/npm/@huggingface/transformers";
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Transformers(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Returns true if the library has been successfully initialized
        /// </summary>
        public static bool IsInit => !JS.IsUndefined(GlobalModuleName);
        /// <summary>
        /// This method will import the transformers.js module using a dynamic import call
        /// </summary>
        /// <param name="srcUrl"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<Transformers> Init(string? srcUrl = null)
        {
            srcUrl = srcUrl ?? LatestBundledVersionSrc;
            var transformers = await JS.Import<Transformers>(GlobalModuleName, srcUrl);
            if (transformers == null) throw new Exception("Transformers could not be initialized.");
            return transformers;
        }
        static bool? hasFp16 = null;
        /// <summary>
        /// Returns true if the WebGPU adapter supports the shader-f16 feature, which is required for half-precision floating point (fp16) operations in shaders.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> HasWebGpuFp16()
        {
            if (hasFp16 == null)
            {
                try
                {
                    using var navigator = JS.Get<Navigator>("navigator");
                    using var gpu = navigator.Gpu;
                    if (gpu != null)
                    {
                        using var adapter = await gpu.RequestAdapter();
                        using var features = adapter.Features;
                        hasFp16 = features.Has("shader-f16");
                    }
                }
                catch { }
            }
            if (hasFp16 == null) hasFp16 = false;
            return hasFp16.Value;
        }
        //The task defining which pipeline will be returned.Currently accepted tasks are:
        //"audio-classification": will return a AudioClassificationPipeline.
        //"automatic-speech-recognition": will return a AutomaticSpeechRecognitionPipeline.
        //"depth-estimation": will return a DepthEstimationPipeline.
        //"document-question-answering": will return a DocumentQuestionAnsweringPipeline.
        //"feature-extraction": will return a FeatureExtractionPipeline.
        //"fill-mask": will return a FillMaskPipeline.
        //"image-classification": will return a ImageClassificationPipeline.
        //"image-segmentation": will return a ImageSegmentationPipeline.
        //"image-to-text": will return a ImageToTextPipeline.
        //"object-detection": will return a ObjectDetectionPipeline.
        //"question-answering": will return a QuestionAnsweringPipeline.
        //"summarization": will return a SummarizationPipeline.
        //"text2text-generation": will return a Text2TextGenerationPipeline.
        //"text-classification" (alias "sentiment-analysis" available): will return a TextClassificationPipeline.
        //"text-generation": will return a TextGenerationPipeline.
        //"token-classification" (alias "ner" available): will return a TokenClassificationPipeline.
        //"translation": will return a TranslationPipeline.
        //"translation_xx_to_yy": will return a TranslationPipeline.
        //"zero-shot-classification": will return a ZeroShotClassificationPipeline.
        //"zero-shot-audio-classification": will return a ZeroShotAudioClassificationPipeline.
        //"zero-shot-image-classification": will return a ZeroShotImageClassificationPipeline.
        //"zero-shot-object-detection": will return a ZeroShotObjectDetectionPipeline.

        /// <summary>
        /// Creates a new pipeline
        /// </summary>
        /// <typeparam name="TPipeline"></typeparam>
        /// <param name="task"></param>
        /// <param name="model"></param>
        /// <param name="pipelineOptions"></param>
        /// <returns></returns>
        public Task<TPipeline> Pipeline<TPipeline>(string task, string? model = null, PipelineOptions? pipelineOptions = null) where TPipeline : Pipeline 
            => pipelineOptions == null ? JS.CallAsync<TPipeline>($"{GlobalModuleName}.pipeline", task, model) : JSRef!.CallAsync<TPipeline>("pipeline", task, model, pipelineOptions);

        /// <summary>
        /// Create a new depth estimation pipeline
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pipelineOptions"></param>
        /// <returns></returns>
        public Task<DepthEstimationPipeline> DepthEstimationPipeline(string? model = null, PipelineOptions? pipelineOptions = null) 
            => Pipeline<DepthEstimationPipeline>("depth-estimation", model, pipelineOptions);

        /// <summary>
        /// Create a new image-to-image pipeline
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pipelineOptions"></param>
        /// <returns></returns>
        public Task<ImageToImagePipeline> ImageToImagePipeline(string? model = null, PipelineOptions? pipelineOptions = null)
            => Pipeline<ImageToImagePipeline>("image-to-image", model, pipelineOptions);

        /// <summary>
        /// Create a new document question answering pipeline
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pipelineOptions"></param>
        /// <returns></returns>
        public Task<DocumentQuestionAnsweringPipeline> DocumentQuestionAnsweringPipeline(string? model = null, PipelineOptions? pipelineOptions = null)
            => Pipeline<DocumentQuestionAnsweringPipeline>("document-question-answering", model, pipelineOptions);

        /// <summary>
        /// Create a new automatic speech recognition pipeline
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pipelineOptions"></param>
        /// <returns></returns>
        public Task<AutomaticSpeechRecognitionPipeline> AutomaticSpeechRecognitionPipeline(string? model = null, PipelineOptions? pipelineOptions = null)
            => Pipeline<AutomaticSpeechRecognitionPipeline>("automatic-speech-recognition", model, pipelineOptions);

        /// <summary>
        /// Create a new text to image pipeline
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pipelineOptions"></param>
        /// <returns></returns>
        public Task<TextToImagePipeline> TextToImagePipeline(string? model = null, PipelineOptions? pipelineOptions = null)
            => Pipeline<TextToImagePipeline>("text-to-image", model, pipelineOptions);

        /// <summary>
        /// Returns the version of the transformers.js library
        /// </summary>
        public static string? Version => JS.Get<string>($"{GlobalModuleName}.env.version");
    }
}
