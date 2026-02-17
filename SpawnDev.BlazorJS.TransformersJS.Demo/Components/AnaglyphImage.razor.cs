using Microsoft.AspNetCore.Components;
using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.MultiView;
using SpawnDev.BlazorJS.TransformersJS.Demo.Services;

namespace SpawnDev.BlazorJS.TransformersJS.Demo.Components
{
    public partial class AnaglyphImage
    {
        ElementReference canvasElRef;
        HTMLCanvasElement? canvas = null;

        [Parameter]
        public EventCallback<bool> ProgressChanged { get; set; }

        [Inject]
        DepthEstimationService DepthEstimationService { get; set; } = default!;

        [Inject]
        BlazorJSRuntime JS { get; set; } = default!;

        [Parameter]
        public string Style { get; set; } = "";

        [Parameter]
        public string Source { get; set; } = "";

        [Parameter]
        public int AnaglyphProfile { get; set; }

        [Parameter]
        public float Focus3D { get; set; } = 0.5f;

        [Parameter]
        public float Level3D { get; set; } = 1.0f;

        string OutputKey => $"{Source}#{Focus3D}#{Level3D}#{AnaglyphProfile}";
        string _OutputKeyCurrent = "";

        public string GeneratedSource => string.IsNullOrEmpty(_GeneratedSource) ? Source : _GeneratedSource;

        RenderAnaglyph? anaglyphRenderer { get; set; }

        string _GeneratedSource = "";
        public bool Processing { get; set; }
        public bool ProcessingFailed { get; set; }

        public async Task<bool> DownloadImage(string filename, float? quality = null)
        {
            if (anaglyphRenderer == null) return false;
            var ext = filename.Split(".").Last().ToLowerInvariant();
            string? mimeType = null;
            switch (ext)
            {
                case "jpg":
                case "jpeg":
                    mimeType = "image/jpeg";
                    break;
                default:
                    mimeType = $"image/{ext}";
                    break;
            }
            var objectUrl = await anaglyphRenderer.ToObjectUrl(mimeType, quality);
            if (string.IsNullOrEmpty(objectUrl)) return false;
            DownloadFile(objectUrl, filename);
            URL.RevokeObjectURL(objectUrl);
            return true;
        }
        void DownloadFile(string url, string filename)
        {
            using var document = JS.GetDocument();
            using var a = document!.CreateElement<HTMLAnchorElement>("a");
            a.Href = url;
            a.Download = filename;
            document.Body!.AppendChild(a);
            a.Click();
            document.Body.RemoveChild(a);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!Processing && _OutputKeyCurrent != OutputKey)
            {
                await Update();
            }
        }
        async Task Update()
        {
            var outputKey = OutputKey;
            if (_OutputKeyCurrent == outputKey || Processing) return;
            try
            {
                canvas ??= new HTMLCanvasElement(canvasElRef);
                anaglyphRenderer ??= new RenderAnaglyph(canvas);
                ProcessingFailed = false;
                Processing = true;
                await ProgressChanged.InvokeAsync(true);
                using var image = await HTMLImageElement.CreateFromImageAsync(Source);
                var depthEstimationPipeline = await DepthEstimationService.GetDepthEstimationPipeline();
                using var rawImage = await RawImage.FromURL(Source);
                using var depthResult = await depthEstimationPipeline.Call(rawImage);
                using var depth = depthResult.Depth;
                anaglyphRenderer.Level3D = Level3D;
                anaglyphRenderer.Focus3D = Focus3D;
                anaglyphRenderer.ProfileIndex = AnaglyphProfile;
                anaglyphRenderer.SetInput(image);
                anaglyphRenderer.SetDepth(depth.Width, depth.Height, depth.Data);
                anaglyphRenderer.Render();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
            finally
            {
                Processing = false;
                _OutputKeyCurrent = outputKey;
                await ProgressChanged.InvokeAsync(false);
                if (_OutputKeyCurrent != OutputKey)
                {
                    _ = Update();
                }
            }
        }
    }
}
