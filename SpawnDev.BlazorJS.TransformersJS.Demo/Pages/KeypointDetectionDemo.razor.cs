using Microsoft.AspNetCore.Components;
using SpawnDev.BlazorJS.JSObjects;
using File = SpawnDev.BlazorJS.JSObjects.File;

namespace SpawnDev.BlazorJS.TransformersJS.Demo.Pages
{
    public partial class KeypointDetectionDemo
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
        string SelectedModel = "onnx-community/vitpose-base-simple";
        string? fileObjectUrl = "images/QpXlLNyLDKZUxXjokbUyy.jpeg";
        string? resultObjectUrl = null;
        Dictionary<string, ModelLoadProgress> ModelProgresses = new();
        AutoImageProcessor? autoImageProcessor = null;
        AutoModel? autoModel = null;

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
            if (autoModel == null)
            {
                try
                {
                    Log($"Pipeline loading... ", false);
                    using var OnProgress = new ActionCallback<ModelLoadProgress>(Pipeline_OnProgress);
                    autoModel = await AutoModel.FromPretrained(SelectedModel, new PipelineOptions { Device = webGPUSupported ? "webgpu" : null, OnProgress = OnProgress });
                    JS.Set("_autoModel", autoModel);
                    autoImageProcessor = await AutoImageProcessor.FromPretrained(SelectedModel, new PipelineOptions { Device = webGPUSupported ? "webgpu" : null, OnProgress = OnProgress });
                    JS.Set("_autoImageProcessor", autoImageProcessor);
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

            // prepare inputs with the auto image processor
            var inputs = await autoImageProcessor!.Call(rawImage);

            JS.Log("_inputs", inputs);
            JS.Set("_inputs", inputs);

            // predict heatmaps
            var autoModelResult = await autoModel!.Call(inputs);

            // get heatmaps
            using var heatmaps = autoModelResult.Heatmaps;

            JS.Log("_heatmaps", heatmaps);
            JS.Set("_heatmaps", heatmaps);

            // post-process heatmaps to get keypoints and scores
            var boxes = new (int, int, int, int)[][] { new[] { (0, 0, rgbImage.Width, rgbImage.Height) } };

            var results = autoImageProcessor.PostProcessPoseEstimation(heatmaps, boxes)[0][0];

            // show results
            using var canvas = new OffscreenCanvas(rgbImage.NaturalWidth, rgbImage.NaturalHeight);
            using var ctx = canvas.Get2DContext();

            // draw image to canvas
            ctx.DrawImage(rgbImage, 0, 0);

            // draw edges between key points
            var points = results.Keypoints;
            ctx.LineWidth = 4;
            ctx.StrokeStyle = "blue";
            using var modelConfig = autoModel.Config;
            var edges = modelConfig.Edges;
            foreach (var (i, j) in edges)
            {
                var (x1, y1) = points[i];
                var (x2, y2) = points[j];
                ctx.BeginPath();
                ctx.MoveTo(x1, y1);
                ctx.LineTo(x2, y2);
                ctx.Stroke();
            }

            // Draw circle at each keypoint
            ctx.FillStyle = "red";
            foreach (var (x, y) in points)
            {
                ctx.BeginPath();
                ctx.Arc(x, y, 8, 0, 2 * Math.PI);
                ctx.Fill();
            }

            // create a url we can assign to an img element
            using var blob = await canvas.ConvertToBlob();
            resultObjectUrl = URL.CreateObjectURL(blob);

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
