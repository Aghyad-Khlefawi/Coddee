using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Coddee.Xamarin.Controls
{
    public class SvgViewer : Frame
    {
        private readonly SKCanvasView _canvasView = new SKCanvasView();

        public SvgViewer()
        {
            Padding = new Thickness(0);
            HasShadow = false;
            BackgroundColor = Color.Transparent;
            Content = _canvasView;
            _canvasView.PaintSurface += CanvasViewOnPaintSurface;
        }

        public static readonly BindableProperty SvgDataProperty = BindableProperty.Create(
            nameof(SvgData),
            typeof(string),
            typeof(SvgViewer),
            default(string),
            propertyChanged: RedrawCanvas);

        public string SvgData
        {
            get => (string)GetValue(SvgDataProperty);
            set => SetValue(SvgDataProperty, value);
        }
        public static readonly BindableProperty FillColorProperty = BindableProperty.Create(
            nameof(FillColor),
            typeof(Color),
            typeof(SvgViewer),
            default(Color),
            propertyChanged: RedrawCanvas);

        public Color FillColor
        {
            get => (Color)GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }
        
        private static void RedrawCanvas(BindableObject bindable, object oldvalue, object newvalue)
        {
            var svgViewer = bindable as SvgViewer;
            svgViewer?._canvasView.InvalidateSurface();
        }

        private void CanvasViewOnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            var info = args.Info;
            var surface = args.Surface;
            var canvas = surface.Canvas;
            
            var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = FillColor.ToSKColor()
            };
            canvas.Clear();
            var path = SKPath.ParseSvgPathData(SvgData);
            if (path==null)
                return;
            path.GetTightBounds(out var bounds);

            canvas.Translate(info.Width / 2, info.Height / 2);

            canvas.Scale(info.Width / (bounds.Width + paint.StrokeWidth),
                info.Height / (bounds.Height + paint.StrokeWidth));

            canvas.Translate(-bounds.MidX, -bounds.MidY);

            canvas.DrawPath(path, paint);
        }
    }
}