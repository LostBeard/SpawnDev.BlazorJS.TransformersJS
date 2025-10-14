using Microsoft.AspNetCore.Components;
using SpawnDev.BlazorJS.JSObjects;
using File = SpawnDev.BlazorJS.JSObjects.File;

namespace SpawnDev.BlazorJS.TransformersJS.Demo.Pages
{
    public partial class DocumentQuestionAnsweringDemo
    {
        [Inject]
        BlazorJSRuntime JS { get; set; } = default!;

        bool beenInit = false;
        bool busy = true;
        string logMessage = "";
        ElementReference fileInputRef;
        HTMLInputElement? fileInput;
        Transformers? Transformers = null;
        File? File = null;
        string SelectedModel = "Xenova/donut-base-finetuned-docvqa";
        string? fileObjectUrl = "images/invoice.png";
        string? resultObjectUrl = null;
        Dictionary<string, ModelLoadProgress> ModelProgresses = new();
        DocumentQuestionAnsweringPipeline? documentQuestionAnsweringPipeline = null;
        DocumentQuestionAnsweringOutput[] Answers = new DocumentQuestionAnsweringOutput[0];
        string Question = "What is the invoice number?";

        protected override void OnAfterRender(bool firstRender)
        {
            if (!beenInit && fileInput == null)
            {
                beenInit = true;
                fileInput = new HTMLInputElement(fileInputRef);
                fileInput.OnChange += FileInput_OnChange;
                busy = false;
                StateHasChanged();
            }
        }
        void Pipeline_OnProgress(ModelLoadProgress obj)
        {
            if (obj.File != null)
            {
                if (ModelProgresses.TryGetValue(obj.File, out var progress))
                {
                    progress.Status = obj.Status;
                    if (obj.Progress != null) progress.Progress = obj.Progress;
                    if (obj.Total != null) progress.Total = obj.Total;
                    if (obj.Loaded != null) progress.Loaded = obj.Loaded;
                }
                else
                {
                    ModelProgresses[obj.File] = obj;
                }
            }
            StateHasChanged();
        }
        async Task RunIt()
        {
            if (busy) return;
            var webGPUSupported = !JS.IsUndefined("navigator.gpu?.requestAdapter");
            busy = true;
            StateHasChanged();
            // init Transformers.js if not already initialized
            if (Transformers == null)
            {
                Log($"Initializing... ", false);
                Transformers = await Transformers.Init();
                Log($"Done");
            }
            // create pipeline if not already created
            if (documentQuestionAnsweringPipeline == null)
            {
                try
                {
                    Log($"Pipeline loading... ", false);
                    using var OnProgress = new ActionCallback<ModelLoadProgress>(Pipeline_OnProgress);
                    documentQuestionAnsweringPipeline = await Transformers.DocumentQuestionAnsweringPipeline(SelectedModel, new PipelineOptions { OnProgress = OnProgress });
                    ModelProgresses.Clear();
                    Log($"Done");
                    ModelProgresses.Clear();
                }
                catch
                {
                    Log($"Error");
                }
            }
            // process file
            if (!string.IsNullOrEmpty(fileObjectUrl))
            {
                try
                {
                    await ProcessFile();
                }
                catch { }
            }
            busy = false;
            StateHasChanged();
        }
        async void FileInput_OnChange(Event ev)
        {
            if (!string.IsNullOrEmpty(fileObjectUrl))
            {
                URL.RevokeObjectURL(fileObjectUrl);
                fileObjectUrl = null;
            }
            if (!string.IsNullOrEmpty(resultObjectUrl))
            {
                URL.RevokeObjectURL(resultObjectUrl);
                resultObjectUrl = null;
            }
            File?.Dispose();
            using var Files = fileInput!.Files;
            File = Files!.FirstOrDefault();
            if (File == null)
            {
                return;
            }
            busy = true;
            StateHasChanged();
            fileObjectUrl = await FileReader.ReadAsDataURLAsync(File);
            busy = false;
            StateHasChanged();
        }
        void Log(string msg, bool newLine = true)
        {
            if (newLine)
            {
                logMessage += $"{msg}<br/>";
            }
            else
            {
                logMessage += $"{msg}";
            }
            StateHasChanged();
        }
        async Task ProcessSelectedFile()
        {
            if (!string.IsNullOrEmpty(resultObjectUrl))
            {
                URL.RevokeObjectURL(resultObjectUrl);
                resultObjectUrl = null;
            }
            if (fileObjectUrl == null) return;
            StateHasChanged();
            var rgbImage = await HTMLImageElement.CreateFromImageAsync(fileObjectUrl);

            // convert input image to RawImage
            using var rawImage = RawImage.FromImage(rgbImage);

            Log($"Question: {Question}");

            // call the pipeline
            Answers = await documentQuestionAnsweringPipeline!.Call(rawImage, Question);

            foreach (var answer in Answers)
            {
                Log($"Answer: {answer.Answer}");
            }

            StateHasChanged();
        }
        async Task ProcessFile()
        {
            busy = true;
            StateHasChanged();
            try
            {
                Log("Running pipeline... ", false);
                await ProcessSelectedFile();
                Log("Done");
            }
            catch (Exception ex)
            {
                Log($"Error");
            }
            busy = false;
            StateHasChanged();
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            if (fileInput != null)
            {
                fileInput.OnChange -= FileInput_OnChange;
                fileInput.Dispose();
            }
            if (!string.IsNullOrEmpty(fileObjectUrl))
            {
                URL.RevokeObjectURL(fileObjectUrl);
                fileObjectUrl = null;
            }
            if (!string.IsNullOrEmpty(resultObjectUrl))
            {
                URL.RevokeObjectURL(resultObjectUrl);
                resultObjectUrl = null;
            }
            beenInit = false;
        }
    }
}
