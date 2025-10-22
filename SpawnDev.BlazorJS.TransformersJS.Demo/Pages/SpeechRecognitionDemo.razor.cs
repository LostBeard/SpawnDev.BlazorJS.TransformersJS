using Microsoft.AspNetCore.Components;
using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.TransformersJS.Demo.Components;
using File = SpawnDev.BlazorJS.JSObjects.File;

namespace SpawnDev.BlazorJS.TransformersJS.Demo.Pages
{
    public partial class SpeechRecognitionDemo : IDisposable
    {
        [Inject]
        BlazorJSRuntime JS { get; set; } = default!;

        ElementReference fileInputRef;
        HTMLInputElement? fileInput;
        File? File = null;
        string? fileObjectUrl = null;
        bool beenInit = false;
        bool busy = false;
        string logMessage = "";
        Transformers? Transformers = null;
        AutomaticSpeechRecognitionPipeline? SelectedPipeline = null;
        AutomaticSpeechRecognitionResult? PipelineResult = null;

        static List<string> Models = new List<string>
        {
            "onnx-community/parakeet-ctc-0.6b-ONNX"
        };

        string SelectedModel = Models.First();

        // 2025-10-21 fails if enabled
        bool UseWebGPU = false;
        string ModelKey = "";
        Dictionary<string, AutomaticSpeechRecognitionPipeline> Pipelines = new Dictionary<string, AutomaticSpeechRecognitionPipeline>();

        Dictionary<string, ModelLoadProgress> ModelProgresses = new();

        void OnRecordingChanged(MediaStreamRecording mediaStreamRecording)
        {
            if (!string.IsNullOrEmpty(fileObjectUrl))
            {
                URL.RevokeObjectURL(fileObjectUrl);
                fileObjectUrl = null;
            }
            File?.Dispose();
            File = null;
            fileObjectUrl = mediaStreamRecording?.URL;
            StateHasChanged();
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

        ActionCallback<ModelLoadProgress>? OnProgress = null;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!beenInit && fileInput == null)
            {
                beenInit = true;
                fileInput = new HTMLInputElement(fileInputRef);
                fileInput.OnChange += FileInput_OnChange;
                busy = false;
                StateHasChanged();
            }
            if (firstRender)
            {
                OnProgress ??= new ActionCallback<ModelLoadProgress>(Pipeline_OnProgress);
                if (Transformers == null)
                {
                    busy = true;
                    Log($"Transformers initializing... ", false);
                    Transformers = await Transformers.Init();
                    Log($"Done");
                    busy = false;
                    StateHasChanged();
                }
            }
        }

        async void FileInput_OnChange(Event ev)
        {
            if (!string.IsNullOrEmpty(fileObjectUrl))
            {
                URL.RevokeObjectURL(fileObjectUrl);
                fileObjectUrl = null;
            }
            PipelineResult?.Dispose();
            PipelineResult = null;
            File?.Dispose();
            using var Files = fileInput!.Files;
            File = Files?.FirstOrDefault();
            if (File != null)
            {
                fileObjectUrl = URL.CreateObjectURL(File);
            }
            StateHasChanged();
        }

        async Task RunIt()
        {
            if (busy || string.IsNullOrEmpty(fileObjectUrl)) return;

            var key = UseWebGPU ? $"{SelectedModel}+webgpu" : SelectedModel;
            SelectedPipeline = null;
            busy = true;
            StateHasChanged();
            ModelKey = key;

            if (Transformers == null)
            {
                Log($"Initializing... ", false);
                Transformers = await Transformers.Init();
                Log($"Done");
            }

            if (!Pipelines.TryGetValue(key, out var pipeline))
            {
                try
                {
                    Log($"Pipeline loading... ", false);
                    pipeline = await Transformers.AutomaticSpeechRecognitionPipeline(SelectedModel, new PipelineOptions
                    {
                        Device = UseWebGPU ? "webgpu" : null,
                        OnProgress = OnProgress
                    });
                    Pipelines[key] = pipeline;
                    Log($"Done");
                }
                catch (Exception ex)
                {
                    Log($"Error loading model: {ex.Message}");
                    busy = false;
                    StateHasChanged();
                    return;
                }
                finally
                {
                    ModelProgresses.Clear();
                }
            }

            SelectedPipeline = pipeline;
            if (SelectedPipeline != null)
            {
                try
                {
                    await ProcessFile();
                }
                catch (Exception ex)
                {
                    Log($"Error processing file: {ex.Message}");
                }
            }
            busy = false;
            StateHasChanged();
        }

        async Task ProcessFile()
        {
            if (SelectedPipeline == null || string.IsNullOrEmpty(fileObjectUrl)) return;

            Log($"Processing...", false);

            try
            {
                // Process using the pipeline
                PipelineResult?.Dispose();
                PipelineResult = null;
                PipelineResult = await SelectedPipeline.Call(fileObjectUrl!);

                Log($"Pipeline call completed.");
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Log($"Error during processing: {ex.Message}");
            }
        }

        void Log(string message, bool newLine = true)
        {
            if (newLine)
            {
                logMessage += message + "<br/>";
            }
            else
            {
                logMessage = message;
            }
            StateHasChanged();
        }

        public void Dispose()
        {
            if (!string.IsNullOrEmpty(fileObjectUrl))
            {
                URL.RevokeObjectURL(fileObjectUrl);
                fileObjectUrl = null;
            }
            PipelineResult?.Dispose();
            File?.Dispose();
            if (fileInput != null)
            {
                fileInput.OnChange -= FileInput_OnChange;
                fileInput.Dispose();
                fileInput = null;
            }
            foreach (var pipeline in Pipelines.Values)
            {
                pipeline?.DisposeJS();
            }
        }
    }
}