using System;
using System.Collections.Generic;

namespace Cifar10.WPF.Model;

public sealed class UpscaleConvNet
{
    private readonly List<ILayer> _layers = new();

    private UpscaleConvNet()
    {
    }

    public static UpscaleConvNet CreateDefault(int pSizeX, int pSizeY)
    {
        var net = new UpscaleConvNet();
        net._layers.Add(new InputLayer(pSizeX, pSizeY, 3));
        net._layers.Add(new DeAvgPoolLayer(2));

        // 6x Conv-ReLU 64, 3x3, padding=1, stride=1
        for (int i = 0; i < 6; i++)
            net._layers.Add(new ConvReLULayer(64, 3, padding: 1, stride: 1));

        // 1x Conv-ReLU 64, 1x1
        net._layers.Add(new ConvReLULayer(64, 1, padding: 0, stride: 1));
        // final Conv to 3 channels
        net._layers.Add(new ConvReLULayer(3, 1, padding: 0, stride: 1, relu: false));

        net.InitializeIdentity();
        return net;
    }

    private void InitializeIdentity()
    {
        Tensor? last = null;
        foreach (var layer in _layers)
        {
            last = layer.Initialize(last);
        }
    }

    public (float[] r, float[] g, float[] b) Upscale(float[] r16, float[] g16, float[] b16)
    {
        var input = new Tensor(r16, g16, b16, 16, 16);
        Tensor output = input;
        foreach (var layer in _layers)
            output = layer.Forward(output);

        var r = new float[output.Width * output.Height];
        var g = new float[r.Length];
        var b = new float[r.Length];
        output.CopyToChannels(r, g, b);
        return (r, g, b);
    }

    #region Layers and Tensor
    private interface ILayer
    {
        Tensor Initialize(Tensor? previous);
        Tensor Forward(Tensor input);
    }

    private sealed class Tensor
    {
        public int Width { get; }
        public int Height { get; }
        public int Channels { get; }
        private readonly float[] _data; // layout: [c][y][x]

        public Tensor(int width, int height, int channels)
        {
            Width = width;
            Height = height;
            Channels = channels;
            _data = new float[channels * width * height];
        }

        public Tensor(float[] r, float[] g, float[] b, int width, int height)
        {
            Width = width;
            Height = height;
            Channels = 3;
            _data = new float[Channels * width * height];
            CopyFromChannels(r, g, b);
        }

        public float Get(int x, int y, int c)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return 0f;
            return _data[(c * Height + y) * Width + x];
        }

        public void Set(int x, int y, int c, float value)
        {
            _data[(c * Height + y) * Width + x] = value;
        }

        public void CopyFromChannels(float[] r, float[] g, float[] b)
        {
            int len = Width * Height;
            for (int i = 0; i < len; i++)
            {
                _data[i] = r[i];
                _data[len + i] = g[i];
                _data[len * 2 + i] = b[i];
            }
        }

        public void CopyToChannels(float[] r, float[] g, float[] b)
        {
            int len = Width * Height;
            for (int i = 0; i < len; i++)
            {
                r[i] = _data[i];
                g[i] = _data[len + i];
                b[i] = _data[len * 2 + i];
            }
        }
    }

    private sealed class InputLayer : ILayer
    {
        private readonly int _w;
        private readonly int _h;
        private readonly int _c;

        public InputLayer(int w, int h, int c)
        {
            _w = w; _h = h; _c = c;
        }

        public Tensor Initialize(Tensor? previous) => new(_w, _h, _c);

        public Tensor Forward(Tensor input) => input; // passthrough
    }

    private sealed class DeAvgPoolLayer : ILayer
    {
        private readonly int _factor;

        public DeAvgPoolLayer(int factor)
        {
            _factor = Math.Max(1, factor);
        }

        public Tensor Initialize(Tensor? previous)
        {
            if (previous == null) throw new InvalidOperationException("Previous layer required.");
            return new Tensor(previous.Width * _factor, previous.Height * _factor, previous.Channels);
        }

        public Tensor Forward(Tensor input)
        {
            var output = new Tensor(input.Width * _factor, input.Height * _factor, input.Channels);
            for (int c = 0; c < input.Channels; c++)
            {
                for (int y = 0; y < output.Height; y++)
                {
                    for (int x = 0; x < output.Width; x++)
                    {
                        float v = input.Get(x / _factor, y / _factor, c);
                        output.Set(x, y, c, v);
                    }
                }
            }
            return output;
        }
    }

    private sealed class ConvReLULayer : ILayer
    {
        private readonly int _filters;
        private readonly int _kernel;
        private readonly int _padding;
        private readonly int _stride;
        private readonly bool _relu;
        private float[][,]? _weights; // [out][ky, kx, in] flattened into [,]
        private float[]? _bias;
        private int _inChannels;

        public ConvReLULayer(int filters, int kernel, int padding, int stride, bool relu = true)
        {
            _filters = filters;
            _kernel = kernel;
            _padding = padding;
            _stride = stride == 0 ? 1 : stride;
            _relu = relu;
        }

        public Tensor Initialize(Tensor? previous)
        {
            if (previous == null) throw new InvalidOperationException("Previous layer required.");
            _inChannels = previous.Channels;
            int outW = (previous.Width + 2 * _padding - _kernel) / _stride + 1;
            int outH = (previous.Height + 2 * _padding - _kernel) / _stride + 1;

            _weights = new float[_filters][,];
            _bias = new float[_filters];
            for (int f = 0; f < _filters; f++)
            {
                _weights[f] = new float[_kernel, _kernel * _inChannels];
            }
            InitializeIdentityWeights();
            return new Tensor(outW, outH, _filters);
        }

        private void InitializeIdentityWeights()
        {
            if (_weights == null) return;
            var rand = new Random(1234);
            for (int f = 0; f < _filters; f++)
            {
                for (int c = 0; c < _inChannels; c++)
                {
                    int center = _kernel / 2;
                    if (f == c && center < _kernel)
                    {
                        _weights[f][center, center * _inChannels + c] = 1f;
                    }
                    else
                    {
                        _weights[f][center, center * _inChannels + c] = (float)(rand.NextDouble() * 0.01);
                    }
                }
                if (_bias != null) _bias[f] = 0f;
            }
        }

        public Tensor Forward(Tensor input)
        {
            if (_weights == null || _bias == null)
                throw new InvalidOperationException("Layer not initialized.");

            int outW = (input.Width + 2 * _padding - _kernel) / _stride + 1;
            int outH = (input.Height + 2 * _padding - _kernel) / _stride + 1;
            var output = new Tensor(outW, outH, _filters);

            for (int f = 0; f < _filters; f++)
            {
                for (int oy = 0; oy < outH; oy++)
                {
                    for (int ox = 0; ox < outW; ox++)
                    {
                        float sum = _bias[f];
                        for (int ky = 0; ky < _kernel; ky++)
                        {
                            int iy = oy * _stride + ky - _padding;
                            for (int kx = 0; kx < _kernel; kx++)
                            {
                                int ix = ox * _stride + kx - _padding;
                                int weightBase = kx * _inChannels;
                                for (int c = 0; c < _inChannels; c++)
                                {
                                    float w = _weights[f][ky, weightBase + c];
                                    float v = input.Get(ix, iy, c);
                                    sum += w * v;
                                }
                            }
                        }
                        if (_relu && sum < 0f) sum = 0f;
                        output.Set(ox, oy, f, sum);
                    }
                }
            }
            return output;
        }
    }
    #endregion
}
