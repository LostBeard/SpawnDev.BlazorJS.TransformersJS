using Microsoft.AspNetCore.Components;
using SpawnDev.BlazorJS.JSObjects;
using System.Text.Json.Serialization;
using File = SpawnDev.BlazorJS.JSObjects.File;


namespace SpawnDev.BlazorJS.TransformersJS.Demo.Pages
{
    public partial class TextToImage
    {
        [Inject]
        BlazorJSRuntime JS { get; set; } = default!;

        public string TextBoxValue { get; set; } = "";

        bool beenInit = false;
        bool busy = false;
        bool UseWebGPU = true;
        string logMessage = "";
        string outputFileName = "generated.png";
        Transformers? Transformers = null;
        TextToImagePipeline? pipeline;
        string? generatedImageObjectUrl = null;

        Dictionary<string, ModelLoadProgress> ModelProgresses = new();
        void Pipeline_OnProgress(ModelLoadProgress obj)
        {
            var key = $"";
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
            if (obj.Status == "done")
            {
                ModelProgresses.Remove(obj.File);
            }
            StateHasChanged();
        }
        ActionCallback<ModelLoadProgress> OnProgress => new ActionCallback<ModelLoadProgress>(Pipeline_OnProgress);
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!beenInit && !busy && Transformers == null)
            {
                busy = true;
                Log($"Transformers initializing... ", false);
                Transformers = await Transformers.Init();
                Log($"Done");
                busy = false;
                StateHasChanged();
            }
        }
        async Task Run()
        {
            if (Transformers == null || busy) return;
            busy = true;

            if (pipeline == null)
            {
                beenInit = true;
                try
                {
                    Log($"Pipeline loading... ", false);

                    pipeline = await Transformers.TextToImagePipeline("Zhare-AI/sd-1-5-webgpu", new PipelineOptions
                    {
                        Device = UseWebGPU ? "webgpu" : null,
                        OnProgress = OnProgress
                    });

                    Log($"Done");
                }
                catch (Exception ex)
                {
                    Log($"Error: {ex.Message}");
                }
            }

            var result = await pipeline!.Call(TextBoxValue, new TextToImageOptions
            {
                Height = 512,
                Width = 512,
                NumInferenceSteps = 50,
                GuidanceScale = 7.5,
                NumImagesPerPrompt = 1
            });

            JS.Log("_result", result);
            JS.Set("_result", result);

            generatedImageObjectUrl = result.Images.FirstOrDefault();

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

        public void Dispose()
        {
            if (!string.IsNullOrEmpty(generatedImageObjectUrl))
            {
                URL.RevokeObjectURL(generatedImageObjectUrl);
                generatedImageObjectUrl = null;
            }
            beenInit = false;
        }
    }
}
