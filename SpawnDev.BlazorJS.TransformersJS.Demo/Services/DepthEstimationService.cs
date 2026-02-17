using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS.Demo.Services
{
    /// <summary>
    /// This class handles the loading and caching of depth estimation pipelines.<br/>
    /// It also has methods for creating and caching 2D+Z images from 2D images<br/>
    /// It provides events indicating the progress of loading models
    /// </summary>
    public class DepthEstimationService
    {
        /// <summary>
        /// The progress percentage from 0 to 100
        /// </summary>
        public float OverallLoadProgress
        {
            get
            {
                var total = (float)ModelProgresses.Values.Sum(p => p.Total ?? 0);
                if (total == 0f) return 0;
                var loaded = (float)ModelProgresses.Values.Sum(p => p.Loaded ?? 0);
                return loaded * 100f / total;
            }
        }
        /// <summary>
        /// True if loading models
        /// </summary>
        public bool Loading { get; private set; }
        /// <summary>
        /// True if loading models
        /// </summary>
        public bool ModelsLoaded => DepthEstimationPipelines.Any();
        /// <summary>
        /// Holds the loading progress for models that are loading
        /// </summary>
        public Dictionary<string, ModelLoadProgress> ModelProgresses { get; } = new();
        /// <summary>
        /// Holds all loaded depth estimation pipelines
        /// </summary>
        public Dictionary<string, DepthEstimationPipeline> DepthEstimationPipelines { get; } = new Dictionary<string, DepthEstimationPipeline>();
        /// <summary>
        /// The default depth estimation model. Used if no model is specified.
        /// </summary>
        public string DefaultDepthEstimationModel { get; set; } = "onnx-community/depth-anything-v2-small";
        /// <summary>
        /// Result from a WebGPU support check
        /// </summary>
        public bool WebGPUSupported { get; private set; }
        /// <summary>
        /// Cache for generated 2DZ images keyed by the image source string
        /// </summary>
        public Dictionary<string, HTMLImageElement> Cached2DZImages { get; } = new Dictionary<string, HTMLImageElement>();
        BlazorJSRuntime JS;
        Transformers? Transformers = null;
        public DepthEstimationService(BlazorJSRuntime js)
        {
            JS = js;
            WebGPUSupported = !JS.IsUndefined("navigator.gpu?.requestAdapter");
        }
        public event Action OnStateChange = default!;
        void StateHasChanged()
        {
            OnStateChange?.Invoke();
        }

        void Pipeline_OnProgress(ModelLoadProgress obj)
        {
            if (!string.IsNullOrEmpty(obj.File))
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
        bool? hasFp16 = null;
        public async Task<bool> HasFp16()
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
        SemaphoreSlim LoadLimiter = new SemaphoreSlim(1);
        public async Task<DepthEstimationPipeline> GetDepthEstimationPipeline(string? model = null, bool useWebGPU = true)
        {
            if (string.IsNullOrEmpty(model))
            {
                model = DefaultDepthEstimationModel;
            }
            if (!WebGPUSupported)
            {
                useWebGPU = false;
            }
            var key = $"{model}+{useWebGPU}";
            if (DepthEstimationPipelines.TryGetValue(key, out var depthEstimationPipeline))
            {
                return depthEstimationPipeline;
            }
            await LoadLimiter.WaitAsync();
            var supportsFp16 = await HasFp16();
            using var OnProgress = new ActionCallback<ModelLoadProgress>(Pipeline_OnProgress);
            try
            {
                Loading = true;
                if (Transformers == null)
                {
                    Transformers = await Transformers.Init();
                }
                // Load Depth Estimation Pipeline
                depthEstimationPipeline = await Transformers.DepthEstimationPipeline(model, new PipelineOptions
                {
                    Device = useWebGPU ? "webgpu" : null,
                    OnProgress = OnProgress,
                    Dtype = supportsFp16 ? "fp16" : "fp32",
                });
                DepthEstimationPipelines[key] = depthEstimationPipeline;
#if DEBUG
                JS.Set("_pipeline", depthEstimationPipeline);
#endif
                return depthEstimationPipeline;
            }
            finally
            {
                Loading = false;
                ModelProgresses.Clear();
                StateHasChanged();
                LoadLimiter.Release();
            }
        }
        public async Task<HTMLImageElement> ImageTo2DZImage(HTMLImageElement image, string? model = null, bool useWebGPU = true)
        {
            var source = image.Src;
            if (!Cached2DZImages.TryGetValue(source, out var imageWithDepth))
            {
                // get the depth estimation pipeline
                var DepthEstimationPipeline = await GetDepthEstimationPipeline(model, useWebGPU);
                // generate the depth map
                using var rawImage = RawImage.FromImage(image);
                using var depthResult = await DepthEstimationPipeline!.Call(rawImage);
                using var depthInfo = depthResult.Depth;
                using var depthMapData = depthInfo.Data;
                var depthWidth = depthInfo.Width;
                var depthHeight = depthInfo.Height;
                // create 2D+Z image object url
                var imageWithDepthObjectUrl = await Create2DZObjectUrl(image, depthMapData, depthWidth, depthHeight);
                imageWithDepth = await HTMLImageElement.CreateFromImageAsync(imageWithDepthObjectUrl);
                Cached2DZImages[source] = imageWithDepth;
            }
            return imageWithDepth;
        }
        public async Task<HTMLImageElement> ImageTo2DZImage(string source, string? model = null, bool useWebGPU = true)
        {
            if (!Cached2DZImages.TryGetValue(source, out var imageWithDepth))
            {
                // get image
                using var image = await HTMLImageElement.CreateFromImageAsync(source);
                // get the depth estimation pipeline
                var DepthEstimationPipeline = await GetDepthEstimationPipeline(model, useWebGPU);
                // generate the depth map
                using var depthResult = await DepthEstimationPipeline!.Call(source);
                using var depthInfo = depthResult.Depth;
                using var depthMapData = depthInfo.Data;
                var depthWidth = depthInfo.Width;
                var depthHeight = depthInfo.Height;
                // create 2D+Z image object url
                var imageWithDepthObjectUrl = await Create2DZObjectUrl(image, depthMapData, depthWidth, depthHeight);
                imageWithDepth = await HTMLImageElement.CreateFromImageAsync(imageWithDepthObjectUrl);
                Cached2DZImages[source] = imageWithDepth;
            }
            return imageWithDepth;
        }
        public async Task<string> Create2DZObjectUrl(HTMLImageElement rgbImage, Uint8Array grayscale1BPPUint8Array, int width, int height, string? type = null, float? quality = null)
        {
            var outWidth = width * 2;
            var outHeight = height;
            var grayscale1BPPBytes = grayscale1BPPUint8Array.ReadBytes();
            var depthmapRGBABytes = Grayscale1BPPToRGBA(grayscale1BPPBytes, width, height);
            using var canvas = new OffscreenCanvas(outWidth, outHeight);
            using var ctx = canvas.Get2DContext();
            // draw rgb image
            ctx.DrawImage(rgbImage);
            // draw depth map
            ctx.PutImageBytes(depthmapRGBABytes, width, height, width, 0);
            using var blob = await canvas.ConvertToBlob(new ConvertToBlobOptions { Type = type, Quality = quality });
            var ret = URL.CreateObjectURL(blob);
            return ret;
        }
        public async Task<Blob> Create2DZBlob(HTMLImageElement rgbImage, Uint8Array grayscale1BPPUint8Array, int width, int height, string? type = null, float? quality = null)
        {
            var outWidth = width * 2;
            var outHeight = height;
            var grayscale1BPPBytes = grayscale1BPPUint8Array.ReadBytes();
            var depthmapRGBABytes = Grayscale1BPPToRGBA(grayscale1BPPBytes, width, height);
            using var canvas = new OffscreenCanvas(outWidth, outHeight);
            using var ctx = canvas.Get2DContext();
            // draw rgb image
            ctx.DrawImage(rgbImage);
            // draw depth map
            ctx.PutImageBytes(depthmapRGBABytes, width, height, width, 0);
            var blob = await canvas.ConvertToBlob(new ConvertToBlobOptions { Type = type, Quality = quality });
            return blob;
        }
        public async Task<Blob> ImageToBlob(HTMLImageElement rgbImage, string? type = null, float? quality = null)
        {
            using var canvas = new OffscreenCanvas(rgbImage.Width, rgbImage.Height);
            using var ctx = canvas.Get2DContext();
            ctx.DrawImage(rgbImage);
            var blob = await canvas.ConvertToBlob(new ConvertToBlobOptions { Type = type, Quality = quality });
            return blob;
        }
        async Task<string> CreateDepthImageObjectUrl(Uint8Array grayscale1BPPUint8Array, int width, int height, string? type = null, float? quality = null)
        {
            var grayscale1BPPBytes = grayscale1BPPUint8Array.ReadBytes();
            var depthmapRGBABytes = Grayscale1BPPToRGBA(grayscale1BPPBytes, width, height);
            using var canvas = new OffscreenCanvas(width, height);
            using var ctx = canvas.Get2DContext();
            ctx.PutImageBytes(depthmapRGBABytes, width, height);
            using var blob = await canvas.ConvertToBlob(new ConvertToBlobOptions { Type = type, Quality = quality });
            var ret = URL.CreateObjectURL(blob);
            return ret;
        }
        byte[] Grayscale1BPPToRGBA(byte[] grayscaleData, int width, int height)
        {
            var ret = new byte[width * height * 4];
            for (var i = 0; i < grayscaleData.Length; i++)
            {
                var grayValue = grayscaleData[i];
                ret[i * 4] = grayValue;     // Red
                ret[i * 4 + 1] = grayValue; // Green
                ret[i * 4 + 2] = grayValue; // Blue
                ret[i * 4 + 3] = 255;       // Alpha
            }
            return ret;
        }
    }
}
