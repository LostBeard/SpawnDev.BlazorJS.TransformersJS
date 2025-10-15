using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class ImageProcessorConfiguration : JSObject
    {
        /// <inheritdoc/>
        public ImageProcessorConfiguration(IJSInProcessObjectReference _ref) : base(_ref) { }
        public bool DoAffineTransform { get => JSRef!.Get<bool>("do_affine_transform"); set => JSRef!.Set("do_affine_transform", value); }
        public bool DoNormalize { get => JSRef!.Get<bool>("do_normalize"); set => JSRef!.Set("do_normalize", value); }
        public bool DoRescale { get => JSRef!.Get<bool>("do_rescale"); set => JSRef!.Set("do_rescale", value); }
        public float[] ImageMean
        {
            get => JSRef!.Get<float[]>("image_mean"); 
            set => JSRef!.Set("image_mean", value);
        }
        public string ImageProcessorType
        {
            get => JSRef!.Get<string>("image_processor_type"); 
            set => JSRef!.Set("image_processor_type", value);
        }
        public float[] ImageStd
        {
            get => JSRef!.Get<float[]>("image_std"); 
            set => JSRef!.Set("image_std", value);
        }
        public float NormalizeFactor
        {
            get => JSRef!.Get<float>("normalize_factor"); 
            set => JSRef!.Set("normalize_factor", value);
        }
        public double RescaleFactor
        {
            get => JSRef!.Get<double>("rescale_factor"); 
            set => JSRef!.Set("rescale_factor", value);
        }
        public Size Size
        {
            get => JSRef!.Get<Size>("size");
            set => JSRef!.Set("size", value);
        }
    }
}
