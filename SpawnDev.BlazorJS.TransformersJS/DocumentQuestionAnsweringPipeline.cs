using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// Document Question Answering pipeline using any AutoModelForDocumentQuestionAnswering. The inputs/outputs are similar to the (extractive) question answering pipeline; however, the pipeline takes an image (and optional OCR’d words/boxes) as input instead of text context.<br/>
    /// https://huggingface.co/docs/transformers.js/api/pipelines#pipelinesdocumentquestionansweringpipeline
    /// https://huggingface.co/docs/transformers.js/api/pipelines#module_pipelines..DocumentQuestionAnsweringPipelineType
    /// </summary>
    public class DocumentQuestionAnsweringPipeline : Pipeline
    {
        /// <inheritdoc/>
        public DocumentQuestionAnsweringPipeline(IJSInProcessObjectReference _ref) : base(_ref) { }

        /// <summary>
        /// Answer a question about a document
        /// </summary>
        /// <param name="source">The image of the document to use.</param>
        /// <param name="question">A question to ask of the document.</param>
        /// <param name="options">Additional keyword arguments to pass along to the generate method of the model.</param>
        /// <returns></returns>
        public async Task<DocumentQuestionAnsweringOutput[]> Call(string source, string question, object? options = null)
            => options == null ? MakeArray<DocumentQuestionAnsweringOutput>(await _Call<JSObject>(source, question)) : MakeArray<DocumentQuestionAnsweringOutput>(await _Call<JSObject>(source, question, options));

        /// <summary>
        /// Answer a question about a document
        /// </summary>
        /// <param name="source">The image of the document to use.</param>
        /// <param name="question">A question to ask of the document.</param>
        /// <param name="options">Additional keyword arguments to pass along to the generate method of the model.</param>
        /// <returns></returns>
        public async Task<DocumentQuestionAnsweringOutput[]> Call(RawImage source, string question, object? options = null)
            => options == null ? MakeArray<DocumentQuestionAnsweringOutput>(await _Call<JSObject>(source, question)) : MakeArray<DocumentQuestionAnsweringOutput>(await _Call<JSObject>(source, question, options));
    }
}