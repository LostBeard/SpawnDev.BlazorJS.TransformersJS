using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using SpawnDev.BlazorJS;
using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.Toolbox;
using SpawnDev.EBML.WebM;
using System.Diagnostics;
using Timer = System.Timers.Timer;

namespace SpawnDev.BlazorJS.TransformersJS.Demo.Components
{
    public partial class AudioRecorder : IAsyncDisposable
    {
        [Inject] BlazorJSRuntime JS { get; set; } = default!;
        [Inject] DialogService DialogService { get; set; } = default!;
        [Inject] MediaDevicesService MediaDevicesService { get; set; } = default!;
        [Parameter] public EventCallback<MediaStreamRecording?> OnRecordingChanged { get; set; }
        TimeSpan RecordingDurationTimeSpan => mediaStreamRecording == null ? RecordingDuration.Elapsed : TimeSpan.FromMilliseconds(mediaStreamRecording.Duration);
        Stopwatch RecordingDuration = new Stopwatch();
        bool _beenInit = false;
        MediaStream? MediaStream = null;
        MediaRecorder? mediaRecorder = null;
        List<Blob> recordedChunks = new List<Blob>();
        MediaStreamRecording? mediaStreamRecording = null;
        bool isRecordingPaused => mediaRecorder != null && mediaRecorder.State == "paused";
        bool isRecording => mediaRecorder != null && mediaRecorder.State != "inactive";
        bool canRecord => MediaStream != null && !MediaStream.IsWrapperDisposed;
        bool isRecordDisabled => (isRecording && !isRecordingPaused) || !canRecord || mediaStreamRecording != null;
        bool isPauseDisabled => !isRecording || isRecordingPaused;
        bool isDiscardDisabled => isRecording || mediaStreamRecording == null;
        bool isStopDisabled => !isRecording;
        bool isSelectSourceDisable => isRecording;
        bool DiscardRecording = false;
        double RecordingTimeSlice = 0;
        bool selecting = false;
        bool IsDisposing = false;
        bool IsDisposed = false;
        // audio context
        Timer? audioContextTimer = null;
        AudioContext? audioContext = null;
        MediaStreamAudioSourceNode? mediaStreamAudioSourceNode = null;
        AnalyserNode? analyserNode = null;
        Float32Array? pcmData = null;
        double MicInputVolume = 0;
        double MaxMicInputVolume = 0;
        double elapsed = 0;
        Stopwatch Stopwatch = new Stopwatch();
        ElementReference canvasElRef;
        HTMLCanvasElement? canvasEl = null;
        CanvasRenderingContext2D? ctx = null;
        List<double> amplitudes = new List<double>();
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_beenInit)
            {
                _beenInit = true;
                canvasEl = new HTMLCanvasElement(canvasElRef);
                ctx = canvasEl.Get2DContext();
                MediaDevicesService.OnDeviceInfosChanged += MediaDevicesService_OnDeviceInfosChanged;
            }
        }
        private void DataFlushTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            mediaRecorder?.RequestData();
        }
        private void MediaDevicesService_OnDeviceInfosChanged()
        {
            StateHasChanged();
        }
        void StartRecording()
        {
            if (mediaRecorder == null) return;
            //isRecording = true;
            DiscardRecording = false;
            try
            {
                if (isRecordingPaused)
                {
                    //isRecordingPaused = false;
                    mediaRecorder.Resume();
                }
                else
                {
                    if (RecordingTimeSlice > 0)
                    {
                        mediaRecorder.Start(RecordingTimeSlice);
                    }
                    else
                    {
                        mediaRecorder.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"StartRecording failed: {ex.Message}");
                //isRecording = false;
            }
        }
        void StopRecording()
        {
            if (mediaRecorder == null || !isRecording) return;
            //isRecording = false;
            //isRecordingPaused = false;
            mediaRecorder.Stop();
        }
        void PauseRecording()
        {
            if (mediaRecorder == null || !isRecording) return;
            //isRecordingPaused = true;
            mediaRecorder.Pause();
        }
        void DeleteRecording()
        {
            Console.WriteLine($"DeleteRecording");
            mediaStreamRecording?.Dispose();
            mediaStreamRecording = null;
            amplitudes = new List<double>();
            StateHasChanged();
            _ = OnRecordingChanged.InvokeAsync(mediaStreamRecording!);
        }
        void CloseMediaStream()
        {
            StopRecording();
            if (MediaStream != null)
            {
                MediaStream.RemoveAllTracks();
                MediaStream.Dispose();
                MediaStream = null;
                if (!IsDisposing) StateHasChanged();
            }
            CloseMediaRecorder();
            CloseAudioContext();
        }
        string? GetAudioRecordingCodec()
        {
            return MediaDevicesService.MediaRecorderAudioFormats.FirstOrDefault();
        }
        void CloseMediaRecorder()
        {
            if (mediaRecorder != null)
            {
                mediaRecorder.OnDataAvailable -= MediaRecorder_OnDataAvailable;
                mediaRecorder.OnError -= MediaRecorder_OnError;
                mediaRecorder.OnPause -= MediaRecorder_OnPause;
                mediaRecorder.OnResume -= MediaRecorder_OnResume;
                mediaRecorder.OnStart -= MediaRecorder_OnStart;
                mediaRecorder.OnStop -= MediaRecorder_OnStop;
                mediaRecorder.Dispose();
                mediaRecorder = null;
            }
        }
        public async Task<bool> SelectMediaSource()
        {
            var ret = false;
            MediaStream? selected = null;
            if (selecting) return ret;
            selecting = true;
            try
            {
                selected = await MediaDevicesService.MediaDevices!.GetUserMedia(false, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetUserMedia error: {ex.Message}");
                selecting = false;
                return ret;
            }
            JS.Set("_selected", selected);
            JS.Log("_selected", selected);
            if (selected != null)
            {
                CloseMediaStream();
                MediaStream = selected;
                InitAudioContext(selected);
                var audioVideoRecordingCodec = GetAudioRecordingCodec();
                Console.WriteLine($"MediaRecorder requesting: {audioVideoRecordingCodec}");
                mediaRecorder = new MediaRecorder(MediaStream!, new MediaRecorderOptions { MimeType = audioVideoRecordingCodec });
                mediaRecorder.OnDataAvailable += MediaRecorder_OnDataAvailable;
                mediaRecorder.OnError += MediaRecorder_OnError;
                mediaRecorder.OnPause += MediaRecorder_OnPause;
                mediaRecorder.OnResume += MediaRecorder_OnResume;
                mediaRecorder.OnStart += MediaRecorder_OnStart;
                mediaRecorder.OnStop += MediaRecorder_OnStop;
                Console.WriteLine($"MediaRecorder using: {mediaRecorder.MimeType}");
                StateHasChanged();
                ret = true;
            }
            selecting = false;
            return ret;
        }
        void MediaRecorder_OnDataAvailable(BlobEvent blobEvent)
        {
            JS.Log($"MediaRecorder_OnDataAvailable: {mediaRecorder?.State ?? ""}");
            var blob = blobEvent.Data;
            Console.WriteLine($"Blob.Size: {blob.Size}");
            if (blob.Size == 0)
            {
                blob.Dispose();
            }
            else
            {
                recordedChunks.Add(blob);
            }
        }
        void MediaRecorder_OnStart()
        {
            JS.Log($"MediaRecorder_OnStart: {mediaRecorder?.State ?? ""}");
            //isRecording = true;
            //isRecordingPaused = false;
            RecordingDuration.Restart();
            amplitudes = new List<double>();
            StateHasChanged();
        }
        void MediaRecorder_OnResume()
        {
            JS.Log($"MediaRecorder_OnResume: {mediaRecorder?.State ?? ""}");
            //isRecordingPaused = false;
            RecordingDuration.Start();
            StateHasChanged();
        }
        void MediaRecorder_OnPause()
        {
            JS.Log($"MediaRecorder_OnPause: {mediaRecorder?.State ?? ""}");
            //isRecordingPaused = true;
            RecordingDuration.Stop();
            StateHasChanged();
        }
        async void MediaRecorder_OnStop()
        {
            try
            {
                JS.Log($"MediaRecorder_OnStop: {mediaRecorder?.State ?? ""}");
                //isRecordingPaused = false;
                //isRecording = false;
                RecordingDuration.Stop();
                if (DiscardRecording || IsDisposing)
                {
                    DiscardRecording = false;
                    recordedChunks.ToArray().DisposeAll();
                    recordedChunks.Clear();
                    JS.Log($"MediaRecorder_OnStop: discarding");
                    return;
                }
                if (recordedChunks.Count == 0)
                {
                    JS.Log($"MediaRecorder_OnStop: no data");
                    return;
                }
                var blobs = recordedChunks.ToArray();
                recordedChunks.Clear();
                var totalSize = blobs.Sum(o => o.Size);
                Console.WriteLine($"blobs count: {blobs.Length} Size: {totalSize}");
                var blob = new Blob(blobs, new BlobOptions { Type = mediaRecorder!.MimeType });
                using var arrayBuffer = await blob.ArrayBuffer();
                var sw = Stopwatch.StartNew();
                Console.WriteLine($"Preparing WebM Media");
                using var stream = new ArrayBufferStream(arrayBuffer);
                var webm = new WebMDocumentReader(stream);
                var finalBlob = blob;
                var isMissingDuration = webm.Duration == null;
#if DEBUG
                Console.WriteLine($"1 Media duration: {webm.Duration ?? -1} Size: {webm.Length} Video Size: {webm.VideoPixelWidth}x{webm.VideoPixelHeight} Elapsed: {sw.Elapsed.TotalMilliseconds}");
#endif
                if (webm.FixDuration())
                {
                    using var fixedStream = new ArrayBufferStream(webm.Length);
                    webm.CopyTo(fixedStream);
                    finalBlob = new Blob(new[] { fixedStream.Source! }, new BlobOptions { Type = blob.Type });
                    blob.Dispose();
                }
#if DEBUG
                Console.WriteLine($"2 Media duration: {webm.Duration} Size: {webm.Length} Video Size: {webm.VideoPixelWidth}x{webm.VideoPixelHeight} Elapsed: {sw.Elapsed.TotalMilliseconds}");
#endif
                var recording = new MediaStreamRecording(finalBlob);
                recording.Duration = webm.Duration ?? 0;
                mediaStreamRecording = recording;
#if DEBUG
                JS.Log("lastRecordingURL", sw.Elapsed.TotalMilliseconds, recording.Data!.Size, recording.URL);
#endif
                await OnRecordingChanged.InvokeAsync(mediaStreamRecording);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                JS.Log($"MediaRecorder_OnStop: {ex.Message}");
                var nmt = true;
            }
            finally
            {
                RecordingDuration.Reset();
                StateHasChanged();
            }
        }
        void MediaRecorder_OnError(MediaRecorderErrorEvent errorEvent)
        {
            JS.Log("MediaRecorder_OnError", errorEvent);
            StateHasChanged();
        }
        public async ValueTask DisposeAsync()
        {
            Console.WriteLine($"{GetType().Name}.DisposeAsync");
            if (IsDisposed || IsDisposing)
            {
                return;
            }
            IsDisposing = true;
            if (_beenInit)
            {
                _beenInit = false;
                MediaDevicesService.OnDeviceInfosChanged -= MediaDevicesService_OnDeviceInfosChanged;
                CloseMediaStream();
                canvasEl?.Dispose();
                canvasEl = null;
            }
            IsDisposed = true;
        }
        //async Task CloseDialog(MediaStreamRecording? recording)
        //{
        //    DialogService.Close(recording);
        //}
        // https://jameshfisher.com/2021/01/18/measuring-audio-volume-in-javascript/
        void InitAudioContext(MediaStream stream)
        {
            CloseAudioContext();
            audioContext = new AudioContext();
            mediaStreamAudioSourceNode = audioContext.CreateMediaStreamSource(stream);
            analyserNode = audioContext.CreateAnalyser();
            mediaStreamAudioSourceNode.Connect(analyserNode);
            pcmData = new Float32Array((long)analyserNode.FFTSize);
            audioContextTimer = new Timer();
            audioContextTimer.Elapsed += AudioContextTimer_Elapsed;
            //audioContextTimer.AutoReset = false;
            audioContextTimer.Interval = 50;
            audioContextTimer?.Start();
        }
        private void AudioContextTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            AnalyseFrame();
            //audioContextTimer?.Start();
        }
        void CloseAudioContext()
        {
            if (audioContextTimer != null)
            {
                audioContextTimer.Elapsed -= AudioContextTimer_Elapsed;
                audioContextTimer.Dispose();
                audioContextTimer = null;
            }
            if (pcmData != null)
            {
                pcmData.Dispose();
                pcmData = null;
            }
            if (analyserNode != null)
            {
                analyserNode.Dispose();
                analyserNode = null;
            }
            if (mediaStreamAudioSourceNode != null)
            {
                mediaStreamAudioSourceNode.Dispose();
                mediaStreamAudioSourceNode = null;
            }
            if (audioContext != null)
            {
                audioContext.Dispose();
                audioContext = null;
            }
            MicInputVolume = 0;
            MaxMicInputVolume = 0;
            amplitudes.Clear();
        }
        void AnalyseFrame()
        {
            if (analyserNode == null || pcmData == null) return;
            Stopwatch.Restart();
            if (!isRecordingPaused && mediaStreamRecording == null)
            {
                analyserNode.GetFloatTimeDomainData(pcmData);
                var pcmDataNet = pcmData.ToArray();
                MicInputVolume = pcmDataNet.Average(); // Math.Sqrt(pcmDataNet.SquareAvgFastDouble());
                if (MaxMicInputVolume < MicInputVolume) MaxMicInputVolume = MicInputVolume;
                amplitudes.Add(MicInputVolume);
            }
            DrawAmplitudes();
            elapsed = Math.Round(Stopwatch.Elapsed.TotalMilliseconds);
        }
        void DrawAmplitudes()
        {
            var ch = canvasEl!.Height;
            var cw = canvasEl.Width;
            var count = amplitudes.Count;
            var maxAmplitude = MaxMicInputVolume;
            // clear
            ctx!.ClearRect(0, 0, cw, ch);
            // center bar
            ctx.FillStyle = "white";
            ctx.FillRect(0, (ch / 2) - 1, cw, 2);
            // waveform
            if (mediaStreamRecording != null)
            {
                ctx.FillStyle = "blue";
            }
            else if (isRecording)
            {
                ctx.FillStyle = "green";
            }
            else
            {
                ctx.FillStyle = "white";
            }
            if (maxAmplitude > 0)
            {
                var width = 2;
                var x = cw - width;
                for (var i = count - 1; i >= 0 && x >= 0; i--)
                {
                    var nornalizedAmplitude = amplitudes[i] / maxAmplitude;
                    var height = (int)(nornalizedAmplitude * ch);
                    if (height >= 1)
                    {
                        var y = (1 + ch - height) / 2;
                        ctx.FillRect(x, y, width, height);
                    }
                    x -= width;
                }
            }
            ctx.Font = "bold 16px verdana, sans-serif";
            var txt = RecordingDurationTimeSpan.ToString().Split('.')[0];
            ctx.FillText(txt, 2, 18);
#if DEBUG && false
            ctx.FillText(elapsed.ToString(), 2, 36);
#endif
        }
    }
}
