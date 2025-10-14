namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// DocumentQuestionAnsweringPipeline return type
    /// https://huggingface.co/docs/transformers.js/api/pipelines#module_pipelines..DocumentQuestionAnsweringPipelineType
    /// </summary>
    public class DocumentQuestionAnsweringOutput
    {
        /// <summary>
        /// The generated text.
        /// </summary>
        public string Answer { get; set; } = "";
    }
}