using System;
using System.Drawing;
using System.IO;
using FileLibrary.Exceptions;
using ImageMagick;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;

namespace FileLibrary.FileInter
{
    public class ImageFile
    {
        private byte[] _bytes = null;
        private byte[] _originalBytes = null;
        private string _ext = String.Empty;
        private ISupportedImageFormat _format;

        public void Save(string path, string fileName)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                File.WriteAllBytes(path + fileName + _ext, _bytes.Length > _originalBytes.Length ? _originalBytes : _bytes);
            }
            catch (Exception)
            {
                throw new CustomException("File yaradıla bilmədi.");
            }

            try
            {
                var file = new FileInfo(path + fileName);
                var optimizer = new ImageOptimizer();
                optimizer.LosslessCompress(file);
                file.Refresh();
            }
            catch (Exception)
            {
                //ignore
            }
            
        }

        public void Remove(string path)
        {
            try
            {
                if (File.Exists(path))
                    return;
                File.Delete(path);
            }
            catch (Exception)
            {
                throw new CustomException("File silinə bilmədi.");
            }
        }

        public ImageFile Load(byte[] bytes, string ext)
        {
            this._bytes = bytes;
            this._originalBytes = bytes;
            this._ext = ext;
            if (ext.Equals(".png"))
            {
                this._format = new PngFormat();
            }
            else if (ext.ToLower().Equals(".jpg") || ext.ToLower().Equals(".jpeg"))
            {
                this._format = new JpegFormat();
            }
            else if (ext.Equals(".gif"))
            {
                this._format = new GifFormat();
            }
            else if (ext.Equals(".tiff"))
            {
                this._format = new TiffFormat();
            }
            return this;
        }

        public ImageFile AddWatermark(byte[] bytes)
        {
            ImageLayer imageLayer = new ImageLayer();
            imageLayer.Image = new Bitmap(new MemoryStream(bytes));
            imageLayer.Opacity = 55;

            using (MemoryStream inStream = new MemoryStream(_bytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream)
                            .Format(_format)
                            .Overlay(imageLayer)
                            .Save(outStream);
                    }

                    this._bytes = outStream.ToArray();
                    this._originalBytes = outStream.ToArray();
                    // Do something with the stream.
                }
            }

            return this;
        }

        public ImageFile Resize(int width)
        {
            Size size = new Size();
            Image image = new Bitmap(new MemoryStream(_bytes));

            if (image.Width >= width)
            {
                size.Width = width;
                size.Height = 0;
            }
            else
            {
                size.Width = image.Width;
                size.Height = image.Height;
            }

            using (MemoryStream inStream = new MemoryStream(_bytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream)
                            .Resize(size)
                            .Format(_format)
                            .Save(outStream);
                    }
                    this._bytes = outStream.ToArray();
                    // Do something with the stream.
                }
            }

            return this;
        }

        public ImageFile Optimize(int quality)
        {
            if (_bytes.Length > 61440)
            {
                _format.Quality = quality;
            }

            using (MemoryStream inStream = new MemoryStream(_bytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream)
                            .Format(_format)
                            .Save(outStream);
                    }
                    this._bytes = outStream.ToArray();
                    // Do something with the stream.
                }
            }

            return this;
        }
        public ImageFile ChangeBackground(Color color)
        {
            using (MemoryStream inStream = new MemoryStream(_bytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream).BackgroundColor(color)
                            .Format(_format)
                            .Save(outStream);
                    }
                    this._bytes = outStream.ToArray();
                    // Do something with the stream.
                }
            }

            return this;
        }
    }
}