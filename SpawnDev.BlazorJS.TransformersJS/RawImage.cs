using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// https://huggingface.co/docs/transformers.js/en/api/utils/image<br/>
    /// https://github.com/huggingface/transformers.js/blob/6f43f244e04522545d3d939589c761fdaff057d4/src/utils/image.js#L77<br/>
    /// </summary>
    public class RawImage : JSObject
    {
        #region static properties
        /// <summary>
        /// 
        /// </summary>
        public static int Length => JS.Get<int>("Transformers.RawImage.length");
        #endregion
        #region static methods
        /// <summary>
        /// Create a RawImage from a URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<RawImage> FromURL(string url) => JS.CallAsync<RawImage>("Transformers.RawImage.fromURL", url);
        /// <summary>
        /// Create a RawImage from a Blob
        /// </summary>
        /// <param name="blob"></param>
        /// <returns></returns>
        public static Task<RawImage> FromBlob(Blob blob) => JS.CallAsync<RawImage>("Transformers.RawImage.fromBlob", blob);
        /// <summary>
        /// Create a RawImage from a Canvas
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public static RawImage FromCanvas(HTMLCanvasElement canvas) => JS.Call<RawImage>("Transformers.RawImage.fromCanvas", canvas);
        /// <summary>
        /// Create a RawImage from a Canvas
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public static RawImage FromCanvas(OffscreenCanvas canvas) => JS.Call<RawImage>("Transformers.RawImage.fromCanvas", canvas);
        /// <summary>
        /// Create a RawImage from a Tensor
        /// </summary>
        /// <param name="tensor"></param>
        /// <returns></returns>
        public static RawImage FromTensor(Tensor tensor) => JS.Call<RawImage>("Transformers.RawImage.fromTensor", tensor);
        /// <summary>
        /// Create a RawImage from an HTMLImageElement
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static RawImage FromImage(HTMLImageElement image)
        {
            using var canvas = new OffscreenCanvas(image.NaturalWidth, image.NaturalHeight);
            using var ctx = canvas.Get2DContext();
            ctx.DrawImage(image);
            // Use RawImage.fromCanvas() to create a RawImage 
            var rawImage = RawImage.FromCanvas(canvas);
            return rawImage;
        }
        /// <summary>
        /// Create a RawImage from a URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<RawImage> Read(string url) => JS.CallAsync<RawImage>("Transformers.RawImage.read", url);
        /// <summary>
        /// Create a RawImage from another RawImage
        /// </summary>
        /// <param name="rawImage"></param>
        /// <returns></returns>
        public static Task<RawImage> Read(RawImage rawImage) => JS.CallAsync<RawImage>("Transformers.RawImage.read", rawImage);
        #endregion

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public RawImage(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Create a new instance of RawImage
        /// </summary>
        /// <param name="data">The pixel data.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="channels">The number of channels.</param>
        public RawImage(Uint8Array data, int width, int height, int channels) : base(JS.New("Transformers.RawImage", data, width, height, channels)) { }
        /// <summary>
        /// Create a new instance of RawImage
        /// </summary>
        /// <param name="data">The pixel data.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="channels">The number of channels.</param>
        public RawImage(Uint8ClampedArray data, int width, int height, int channels) : base(JS.New("Transformers.RawImage", data, width, height, channels)) { }
        /// <summary>
        /// Create a new instance of RawImage
        /// </summary>
        /// <param name="data">The pixel data.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="channels">The number of channels.</param>
        public RawImage(byte[] data, int width, int height, int channels) : base(JS.New("Transformers.RawImage", data, width, height, channels)) { }
        /// <summary>
        /// Returns the size of the image (width, height).
        /// </summary>
        public (int width, int height) Size => JSRef!.Get<(int width, int height)>("size");
        /// <summary>
        /// Returns the image as a tensor
        /// </summary>
        /// <param name="channelFormat"></param>
        /// <returns></returns>
        public Tensor ToTensor(string? channelFormat = null) => channelFormat == null ? JSRef!.Call<Tensor>("toTensor") : JSRef!.Call<Tensor>("toTensor", channelFormat);
        /// <summary>
        /// Returns the image as a canvas
        /// </summary>
        /// <returns></returns>
        public OffscreenCanvas ToCanvas() => JSRef!.Call<OffscreenCanvas>("toCanvas");
        /// <summary>
        /// Save the image to the given path.
        /// </summary>
        /// <param name="path">The path to save the image to.</param>
        public Task Save(string path) => JSRef!.CallVoidAsync("save", path);
    }
}
