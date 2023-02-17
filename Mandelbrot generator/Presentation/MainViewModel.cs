using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Mandelbrot_generator.Models;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace Mandelbrot_generator.Presentation
{
    public class MainViewModel : ObservableObject
    {
        public IRelayCommand DoWorkCommand { get; private set; }
        public IRelayCommand ResetCommand { get; private set; }
        public IRelayCommand SaveCommand { get; private set; }
        private readonly ILogic logic;
        private WriteableBitmap bitmapDisplay;
        private CancellationTokenSource source = new CancellationTokenSource();
        private Stopwatch recalculate;
        private Stopwatch redraw;
        private string myTitle = "Mandelbrot Pieter";
        private string x = "-.---";
        private string y = "-.---";
        private double scalingFactor = 1;
        private string cornderCoörds = ".....";
        private string calculationTime = "No calculation yet.";
        private string redrawTime = "No informaion given";
        private string resolution = "Resolution has not been set.";
        private string tokenRequests = "no data given.";
        private string colour;
        private double dpiX = 96d;
        private double dpiY = 96d;
        private double iterationPoint = 0;
        private bool working = false;
        private double yOffset = 0;
        private double xOffset = 0;
        private uint maxColumn = 640;
        private uint maxRow = 480;
        private int iterations = 100;
        private uint[,] iterationArray;
        private uint[,] colourArray;
        Point p;

        public string MyTitle
        {
            get => myTitle;
            private set => SetProperty(ref myTitle, value);
        }
        public string X
        {
            get => "X: " + x;
            private set => SetProperty(ref x, value);
        }
        public string Y
        {
            get => "Y: " + y;
            private set => SetProperty(ref y, value);
        }
        public double ScalingFactor
        {
            get => scalingFactor;
            private set
            {
                SetProperty(ref scalingFactor, value);
            }
        }
        public string CornerCoörds
        {
            get => cornderCoörds;
            private set => SetProperty(ref cornderCoörds, value);
        }
        public string CalculationTime
        {
            get => this.calculationTime;
            private set => SetProperty(ref calculationTime, value);
        }
        public string TokenRequests
        {
            get => this.tokenRequests;
            private set => SetProperty(ref tokenRequests, value);
        }
        public string RedrawTime
        {
            get => redrawTime;
            private set => SetProperty(ref redrawTime, value);
        }
        public WriteableBitmap BitmapDisplay
        {
            get => bitmapDisplay;
            set => SetProperty(ref bitmapDisplay, value);
        }
        public string Resolution
        {
            get => resolution;
            set => SetProperty( ref resolution, value);
        }
        public string ComboboxColour
        {
            get => colour;
            set
            {
                colour = value;
                RedrawBitmap();
            }
        }
        public int TextboxIterations
        {
            get => iterations;
            set
            {
                SetProperty(ref iterations, value); 
                DoWorkCommand = new RelayCommand(async () => await RecalculateMandelbrot(), () => !working);
            }
        }
        public double IterationPoint
        {
            get => iterationPoint;
            set => SetProperty(ref iterationPoint, value);
        }
        public MainViewModel(ILogic logic)
        {
            this.logic = logic;
            redraw = new Stopwatch();
            recalculate = new Stopwatch();
            iterationArray = new uint[maxRow, maxColumn];
            ResetCommand = new RelayCommand(ResetMandelbrot);
            SaveCommand = new RelayCommand(SaveMandelbrot);
            CreateBitmap(Convert.ToInt32(maxColumn), Convert.ToInt32(maxRow));
        }
        private void ResetMandelbrot()
        {
            ScalingFactor = 1;
            yOffset = 0;
            xOffset = 0;
            iterations = 100;
            RefreshMandelbrot();
        }
        private void SaveMandelbrot()
        {
            using(FileStream stream = new FileStream("savedImage.png", FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(BitmapDisplay));
                encoder.Save(stream);
            }
        }

        private async void RefreshMandelbrot()
        {
            if (source != null)
                source.Cancel();
            source = new CancellationTokenSource();
            try
            {
                await RecalculateMandelbrot();
                RedrawBitmap();
            } 
            catch (OperationCanceledException ex)
            { 
                CalculationTime = "The operation is canceled!"; 
                RedrawTime = "The operation is canceled!"; 
            }

        }

        private void CreateBitmap(int width, int height)
        {
            var pixelFormat = System.Windows.Media.PixelFormats.Pbgra32;
            BitmapDisplay = new WriteableBitmap(width, height, dpiX, dpiY, pixelFormat, null);
            OnPropertyChanged(nameof(BitmapDisplay));
        }
        private void SetBitmap(uint[,] colors)
        {
            int numberOfRows = colors.GetLength(1);
            int numberOfColumns = colors.GetLength(0);
            Int32Rect rectangle = new Int32Rect(0, 0, numberOfRows, numberOfColumns);
            BitmapDisplay.WritePixels(rectangle, colors, BitmapDisplay.BackBufferStride, 0, 0);
        }
        private async Task RecalculateMandelbrot()
        {
            recalculate.Restart();
            this.iterationArray = await logic.CalulateMandelbrot(source.Token, xOffset, yOffset, maxRow, maxColumn, iterations, this.ScalingFactor);
            recalculate.Stop();
            this.CalculationTime = recalculate.ElapsedMilliseconds.ToString() + " ms";
        }
        private async Task RedrawBitmap()
        {
            this.colourArray = new uint[Convert.ToUInt16(this.maxRow), Convert.ToUInt16(this.maxColumn)];
            redraw.Restart();
            switch (this.colour)
            {
                case "System.Windows.Controls.ComboBoxItem: Banding":
                    SetBitmap(await Banding());
                    break;
                case "System.Windows.Controls.ComboBoxItem: Grayscale":
                    SetBitmap(await Grayscale());
                    break;
                case "System.Windows.Controls.ComboBoxItem: colour":
                    SetBitmap(await Colour());
                    break;
            }
            redraw.Stop();
            RedrawTime = $"{redraw.ElapsedMilliseconds} ms";
        }
        public async Task<uint[,]> Banding()
        {
            Color white = Color.FromRgb(255, 255, 255);
            Color black = Color.FromRgb(0, 0, 0);
            await Task.Run(() =>
            {
                Parallel.For(0, this.maxRow, i =>
                {
                    for (int j = 0; j < this.maxColumn; j++)
                    {
                        source.Token.ThrowIfCancellationRequested();
                        if ((iterationArray[i, j] % 2) == 0)
                            colourArray[i, j] = (uint)(((white.A << 24) | (white.R << 16) | (white.G << 8) | white.B) & 0xffffffffL);
                        else
                            colourArray[i, j] = (uint)(((black.A << 24) | (black.R << 16) | (black.G << 8) | black.B) & 0xffffffffL);
                    }
                });
            });
            return colourArray;
        }
        public async Task<uint[,]> Grayscale()
        {
            double scale1 = (255.0 / iterations);
            Color gray;
            await Task.Run(() =>
            {
                Parallel.For(0, this.maxRow, i =>
                {
                    for (int j = 0; j < this.maxColumn; j++)
                    {
                        source.Token.ThrowIfCancellationRequested();
                        byte converted = (byte)(iterationArray[i, j] * scale1);
                        Color grey = Color.FromRgb(converted, converted, converted);
                        colourArray[i, j] = (uint)(((grey.A << 24) | (grey.R << 16) | (grey.G << 8) | grey.B) & 0xffffffffL);
                    }
                });
            });
            return colourArray;
        }
        public async Task<uint[,]> Colour()
        {
            double scale2 = (255.0 / iterations);
            await Task.Run(() =>
            {
                Parallel.For(0, this.maxRow, i =>
                {
                    for (int j = 0; j < this.maxColumn; j++)
                    {
                        source.Token.ThrowIfCancellationRequested();
                        colourArray[i, j] = iterationArray[i, j] * 845138000;
                    }
                });
            });
            return colourArray;
        }
        public void MouseLeftButtonDown(Point p)
        {
           this.p = p;
        }
        public async Task MouseLeftButtonUp(Point q)
        {
            double dx = p.X - q.X;
            double dy = p.Y - q.Y;
            xOffset += dx / maxRow / scalingFactor;
            yOffset -= dy / maxColumn / scalingFactor;
            RefreshMandelbrot();
        }
        public void MouseMoved(Point point)
        {
            try
            {
                IterationPoint = iterationArray[(int)point.X, (int)point.Y];
                this.X = "" + (int)point.X;
                this.Y = "" + (int)point.Y;
            }
            catch (Exception) { }
        }
        public async void MouseWheelMoved(double delta, Point q)
        {
            if (delta > 0)
            {
                double dx = bitmapDisplay.Width/2 - q.X;
                double dy = bitmapDisplay.Height/2 - q.Y;
                xOffset -= dx / maxRow / scalingFactor;
                yOffset += dy / maxColumn / scalingFactor;

                ScalingFactor *= 1.1;
                RefreshMandelbrot();
            }
            else if (delta < 0)
            {
                ScalingFactor /= 1.1;
                if (!(scalingFactor < 1))
                {
                    RefreshMandelbrot();
                }
                else scalingFactor = 1;
            }                
        }
        public void ChangeResolution(double height, double width)
        {
            maxRow = (uint)height;
            maxColumn = (uint)width;
            CreateBitmap((int)width, (int)height);
            RefreshMandelbrot();
            this.Resolution = "" + (int)width + " x " + (int)height;
        }
    }
}
