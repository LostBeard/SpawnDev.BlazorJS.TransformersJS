using Microsoft.AspNetCore.Components;
using SpawnDev.BlazorJS.JSObjects;
using File = SpawnDev.BlazorJS.JSObjects.File;

namespace SpawnDev.BlazorJS.TransformersJS.Demo.Pages
{
    public partial class ImageToImageDemo : IDisposable
    {
        [Inject]
        BlazorJSRuntime JS { get; set; } = default!;

        ElementReference fileInputRef;
        HTMLInputElement? fileInput;
        File? File = null;
        bool beenInit = false;
        bool busy = false;
        string logMessage = "";
        string outputFileName = "transformed_image.png";
        Transformers? Transformers = null;
        ImageToImagePipeline? ImageToImagePipeline = null;
        string? fileObjectUrl = null;
        string? transformedObjectUrl = null;
        RawImage? lastTransformedImage = null;

        static List<string> Models = new List<string>
        {
            "Xenova/swin2SR-classical-sr-x2-64",
            "Xenova/swin2SR-realworld-sr-x4-64-bsrgan-psnr",
            "Xenova/real-esrgan-4x",
            "Xenova/4x_APISR_GRL_GAN_generator-onnx"
        };

        string SelectedModel = Models.First();
        bool UseWebGPU = true;
        string ModelKey = "";
        Dictionary<string, ImageToImagePipeline> ImageToImagePipelines = new Dictionary<string, ImageToImagePipeline>();

        Dictionary<string, ModelLoadProgress> ModelProgresses = new();

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
            if (!string.IsNullOrEmpty(transformedObjectUrl))
            {
                URL.RevokeObjectURL(transformedObjectUrl);
                transformedObjectUrl = null;
            }
            File?.Dispose();
            using var Files = fileInput!.Files;
            File = Files?.FirstOrDefault();
            if (File != null)
            {
                fileObjectUrl = URL.CreateObjectURL(File);
                outputFileName = $"transformed_{File.Name}";
            }
            StateHasChanged();
        }

        async Task RunIt()
        {
            if (busy || File == null) return;

            var key = UseWebGPU ? $"{SelectedModel}+webgpu" : SelectedModel;
            ImageToImagePipeline = null;
            busy = true;
            StateHasChanged();
            ModelKey = key;

            if (Transformers == null)
            {
                Log($"Initializing... ", false);
                Transformers = await Transformers.Init();
                Log($"Done");
            }

            if (!ImageToImagePipelines.TryGetValue(key, out var pipeline))
            {
                try
                {
                    Log($"Image-to-Image Pipeline loading... ", false);
                    pipeline = await Transformers.ImageToImagePipeline(SelectedModel, new PipelineOptions
                    {
                        Device = UseWebGPU ? "webgpu" : null,
                        OnProgress = OnProgress
                    });
                    ImageToImagePipelines[key] = pipeline;
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

            ImageToImagePipeline = pipeline;
            if (ImageToImagePipeline != null && File != null)
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
            if (ImageToImagePipeline == null || File == null) return;

            if (!string.IsNullOrEmpty(transformedObjectUrl))
            {
                URL.RevokeObjectURL(transformedObjectUrl);
                transformedObjectUrl = null;
            }

            Log($"Processing image...", false);

            try
            {
                // Process the image using the pipeline
                lastTransformedImage?.Dispose();
                lastTransformedImage = await ImageToImagePipeline.Call(fileObjectUrl!);

                // Convert RawImage to object URL using canvas
                transformedObjectUrl = await ConvertRawImageToObjectUrl(lastTransformedImage);

                Log($"Image transformation completed!");
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Log($"Error during image transformation: {ex.Message}");
            }
        }

        async Task<string> ConvertRawImageToObjectUrl(RawImage rawImage)
        {
            using var blob = await rawImage.JSRef!.CallAsync<Blob>("toBlob");
            return URL.CreateObjectURL(blob);
        }

        async Task SaveTransformedImageDirect()
        {
            if (lastTransformedImage != null)
            {
                try
                {
                    // Use the new RawImage.Save() method to save directly
                    await lastTransformedImage.Save(outputFileName);
                    Log($"Image saved as {outputFileName}");
                }
                catch (Exception ex)
                {
                    Log($"Error saving image: {ex.Message}");
                }
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
            if (!string.IsNullOrEmpty(transformedObjectUrl))
            {
                URL.RevokeObjectURL(transformedObjectUrl);
                transformedObjectUrl = null;
            }
            lastTransformedImage?.Dispose();
            File?.Dispose();
            if (fileInput != null)
            {
                fileInput.OnChange -= FileInput_OnChange;
                fileInput.Dispose();
                fileInput = null;
            }
            foreach (var pipeline in ImageToImagePipelines.Values)
            {
                pipeline?.DisposeJS();
            }
        }
    }
}