//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text.RegularExpressions;

//namespace CRMHalalBackEnd
//{
//    public class FileTypeCheck
//    {
//        private readonly Dictionary<string, Dictionary<string, string>> _allowedFileType = new Dictionary<string, Dictionary<string, string>>();

//        private static FileTypeCheck _instance;
//        private void LoadFileTypes()
//        {
//            var img = new Dictionary<string, string>();
//            img.Add("jpeg", "FF D8 FF|FF D9");
//            img.Add("jpg", "FF D8 FF|FF D9");
//            img.Add("png", "89 50 4E 47 0D 0A 1A 0A|49 45 4E 44 AE 42 60 82");
//            //img.Add("webp", "52 49 46 46 xx xx xx xx 57 45 42 50");
//            //img.Add("svg", "3C 3F 78 6D 6C|2F 73 76 67 3E");
//            img.Add("gif", "47 49 46 38|00 3B");
//            _allowedFileType.Add("image", img);

//            var excel = new Dictionary<string, string>();
//            excel.Add("xls", "D0 CF 11 E0 A1 B1 1A E1");
//            excel.Add("xlsx", "50 4B 03 04 14 00 06 00");
//            _allowedFileType.Add("excel", excel);

//            var word = new Dictionary<string, string>();
//            word.Add("doc", "D0 CF 11 E0 A1 B1 1A E1");
//            word.Add("docx", "50 4B 03 04 14 00 06 00");
//            _allowedFileType.Add("word", word);

//            var pdf = new Dictionary<string, string>();
//            pdf.Add("pdf", "25 50 44 46");
//            _allowedFileType.Add("pdf", pdf);
//        }


//        private FileTypeCheck()
//        {
//            LoadFileTypes();
//        }

//        public static bool IsFileValidation(string fileType, string extension, byte[] byteArr)
//        {
//            FileTypeCheck ins = Instance();

//            var group = ins._allowedFileType.ContainsKey(fileType)?ins._allowedFileType[fileType]:new Dictionary<string,string>();
//            var fileData = group.ContainsKey(extension)?group[extension]:String.Empty;

//            var splitData = fileData.Split('|');
//            var xData = fileData.IndexOf('x');

//            if (splitData.Length > 1)
//            {
//                var beginData = splitData[0];
//                var endData = splitData[1];
//                var splitBegin = beginData.Split(' ');
//                var splitEnd = endData.Split(' ');
//                for (int i = 0; i < splitBegin.Length; i++)
//                {
//                    if (Convert.ToInt32(splitBegin[i], 16) != byteArr[i])
//                    {
//                        return false;
//                    }
//                }

//                var counter = splitEnd.Length;
//                for (int i = byteArr.Length - 1; i >= byteArr.Length - splitEnd.Length + 1; i--)
//                {
//                    if (Convert.ToInt32(splitEnd[--counter], 16) != byteArr[i])
//                    {
//                        var k = byteArr[i];
//                        return false;
//                    }
//                }
//            }
//            else if (xData != -1)
//            {
//                var beginData = xData;
//                var endData = fileData.LastIndexOf('x');
//                var splitRange1 = fileData.Substring(0, beginData - 1).Split(' ');
//                var splitRange2 = fileData.Substring(endData + 2, fileData.Length - endData - 2).Split(' ');
//                var seper = Regex.Matches(fileData, "xx").Count;
//                for (int i = 0; i < splitRange1.Length; i++)
//                {
//                    if (Convert.ToInt32(splitRange1[i], 16) != byteArr[i])
//                    {
//                        return false;
//                    }
//                }

//                var counter = 0;
//                for (int i = splitRange1.Length + seper; i < splitRange1.Length + seper + splitRange2.Length; i++)
//                {
//                    if (Convert.ToInt32(splitRange2[counter++], 16) != byteArr[i])
//                    {
//                        return false;
//                    }
//                }
//            }

//            else if (fileData.Length>0)
//            {
//                var data = fileData.Split(' ');
//                for (int i = 0; i < data.Length; i++)
//                {
//                    if (Convert.ToInt32(data[i], 16) != byteArr[i])
//                    {
//                        return false;
//                    }
//                }
//            }
//            else
//            {
//                return false;
//            }
//            return true;
//        }

//        private static FileTypeCheck Instance()
//        {
//            return _instance == null ? new FileTypeCheck() : _instance;
//        }
//    }
//}