// Decompiled with JetBrains decompiler
// Type: FIleLibrary.FileTypeCheck
// Assembly: FIleLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C1B0BF4B-3310-4B5E-8240-61D0D1A2A7B9
// Assembly location: C:\Users\Dell\Desktop\WebP-wrapper\crmhalalbackend\bin\FIleLibrary.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;

namespace FileLibrary
{
    public class FileTypeCheck
    {
        private readonly Dictionary<string, string> _allowedFileType = new Dictionary<string, string>();
        private static FileTypeCheck _instance;

        private void LoadFileTypes()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            this._allowedFileType.Add("jpeg", "FF D8 FF");
            this._allowedFileType.Add("jpg", "FF D8 FF");
            this._allowedFileType.Add("png", "89 50 4E 47 0D 0A 1A 0A|49 45 4E 44 AE 42 60 82");
            this._allowedFileType.Add("gif", "47 49 46 38|00 3B");
            this._allowedFileType.Add("xls", "D0 CF 11 E0 A1 B1 1A E1");
            this._allowedFileType.Add("xlsx", "50 4B 03 04 14 00 06 00");
            this._allowedFileType.Add("doc", "D0 CF 11 E0 A1 B1 1A E1");
            this._allowedFileType.Add("docx", "50 4B 03 04 14 00 06 00");
            this._allowedFileType.Add("pdf", "25 50 44 46");
        }

        private FileTypeCheck() => this.LoadFileTypes();

        public static bool IsFileValidation(string extension, byte[] byteArr)
        {
            FileTypeCheck fileTypeCheck = FileTypeCheck.Instance();
            string input = fileTypeCheck._allowedFileType.ContainsKey(extension) ? fileTypeCheck._allowedFileType[extension] : string.Empty;
            string[] strArray1 = input.Split('|');
            int num1 = input.IndexOf('x');
            if (strArray1.Length > 1)
            {
                string str1 = strArray1[0];
                string str2 = strArray1[1];
                string[] strArray2 = str1.Split(' ');
                string[] strArray3 = str2.Split(' ');
                for (int index = 0; index < strArray2.Length; ++index)
                {
                    if (Convert.ToInt32(strArray2[index], 16) != (int)byteArr[index])
                        return false;
                }
                int length = strArray3.Length;
                for (int index = byteArr.Length - 1; index >= byteArr.Length - strArray3.Length + 1; --index)
                {
                    if (Convert.ToInt32(strArray3[--length], 16) != (int)byteArr[index])
                    {
                        byte num2 = byteArr[index];
                        return false;
                    }
                }
            }
            else if (num1 != -1)
            {
                int num2 = num1;
                int num3 = input.LastIndexOf('x');
                string[] strArray2 = input.Substring(0, num2 - 1).Split(' ');
                string[] strArray3 = input.Substring(num3 + 2, input.Length - num3 - 2).Split(' ');
                int count = Regex.Matches(input, "xx").Count;
                for (int index = 0; index < strArray2.Length; ++index)
                {
                    if (Convert.ToInt32(strArray2[index], 16) != (int)byteArr[index])
                        return false;
                }
                int num4 = 0;
                for (int index = strArray2.Length + count; index < strArray2.Length + count + strArray3.Length; ++index)
                {
                    if (Convert.ToInt32(strArray3[num4++], 16) != (int)byteArr[index])
                        return false;
                }
            }
            else
            {
                if (input.Length <= 0)
                    return false;
                string[] strArray2 = input.Split(' ');
                for (int index = 0; index < strArray2.Length; ++index)
                {
                    if (Convert.ToInt32(strArray2[index], 16) != (int)byteArr[index])
                        return false;
                }
            }
            return true;
        }

        public static bool ImageFileTypeValidation(byte[] byteArr, string ext)
        {
            ISupportedImageFormat format = new BitmapFormat();

            if (ext.Equals(".png"))
            {
                format = new PngFormat();
            }
            else if (ext.ToLower().Equals(".jpg") || ext.ToLower().Equals(".jpeg"))
            {
                format = new JpegFormat();
            }
            else if (ext.Equals(".gif"))
            {
                format = new GifFormat();
            }
            else if (ext.Equals(".tiff"))
            {
                format = new TiffFormat();
            }

            try
            {
                using (MemoryStream inStream = new MemoryStream(byteArr))
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream)
                            .Format(format);
                    }
                    // Do something with the stream.
                }

                return true;
            }
            catch (Exception e)
            {
                //ignore
            }

            return false;
        }

        private static FileTypeCheck Instance() => FileTypeCheck._instance ?? new FileTypeCheck();
    }
}
