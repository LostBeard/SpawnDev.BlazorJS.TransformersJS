using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// https://github.com/huggingface/transformers.js/blob/1538e3a1544a93ef323e41c4e3baef6332f4e557/src/utils/audio.js#L781
    /// </summary>
    public class RawAudio : JSObject
    {
        /// <inheritdoc/>
        public RawAudio(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Create a new `RawAudio` object.
        /// </summary>
        /// <param name="audio">Audio data</param>
        /// <param name="samplingRate">Sampling rate of the audio data</param>
        public RawAudio(Float32Array audio, double samplingRate) : base(JS.New("Transformers.RawAudio", audio, samplingRate)) { }
        /// <summary>
        /// Audio data
        /// </summary>
        public Float32Array Audio => JSRef!.Get<Float32Array>("audio");
        /// <summary>
        /// Sampling rate of the audio data
        /// </summary>
        public double SamplingRate => JSRef!.Get<double>("sampling_rate");
        /// <summary>
        /// Convert the audio to a wav file buffer.
        /// </summary>
        /// <returns>The WAV file.</returns>
        public ArrayBuffer ToWav() => JSRef!.Call<ArrayBuffer>("toWav");
        /// <summary>
        /// Convert the audio to a WAV Blob.
        /// </summary>
        /// <returns></returns>
        public Blob ToBlob() => JSRef!.Call<Blob>("toBlob");
        /// <summary>
        /// Save the audio to a wav file.
        /// </summary>
        /// <param name="path">path</param>
        /// <returns></returns>
        public Task Save(string path) => JSRef!.CallVoidAsync("save", path);
    }
}