﻿using FileLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileLibrary.FileInter
{
    public class ExcelFile
    {
        private byte[] _bytes = null;
        private byte[] _originalBytes = null;
        private string _ext = String.Empty;

        public void Save(string path, string fileName)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                File.WriteAllBytes(path + fileName + _ext, _bytes);
            }
            catch (Exception)
            {
                throw new CustomException("File yaradıla bilmədi.");
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

        public ExcelFile Load(byte[] bytes, string ext)
        {
            if ( ext == ".xls" || ext == ".xlsx" )
            {
                try
                {
                    this._bytes = bytes;
                    this._originalBytes = bytes;
                    this._ext = ext;

                }
                catch (Exception)
                {

                    throw;
                }

            }
            return this;
        }
    }
}
