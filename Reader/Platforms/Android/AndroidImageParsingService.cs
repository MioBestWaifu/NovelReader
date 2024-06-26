﻿using Java.Util;
using Mio.Reader.Services;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Util;
using Base64 = Android.Util.Base64;
using System.Diagnostics;

namespace Mio.Reader.Platforms.Android
{
    public class AndroidImageParsingService : ImageParsingService
    {
        public override async Task<string> ParseImageEntryToBase64(ZipArchiveEntry entry)
        {
            if (entry != null)
            {
                try
                {
                    using (var stream = entry.Open())
                    {
                        // Convert the ZipArchiveEntry stream to a byte array
                        byte[] imageBytes;
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            imageBytes = memoryStream.ToArray();
                        }

                        // Decode the byte array to a Bitmap
                        var bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);

                        // Determine the image format
                        var imageFormat = entry.FullName.Split('.')[^1] switch
                        {
                            "jpg" => Bitmap.CompressFormat.Jpeg,

                            "jpeg" => Bitmap.CompressFormat.Jpeg,
                            "png" => Bitmap.CompressFormat.Png,
                            "gif" => Bitmap.CompressFormat.Webp,
                            _ => Bitmap.CompressFormat.Jpeg
                        };

                        // Convert the Bitmap to a byte array
                        using (var byteArrayOutputStream = new MemoryStream())
                        {
                            bitmap.Compress(imageFormat, 100, byteArrayOutputStream);
                            byte[] bitmapData = byteArrayOutputStream.ToArray();

                            // Encode the byte array to a Base64 string
                            string base64String = Base64.EncodeToString(bitmapData, Base64Flags.Default);
                            return base64String;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            return "";
        }

        public override async Task<string> ParseImageEntryToBase64(ZipArchiveEntry entry, int newWidth, int newHeight)
        {
            if (entry != null)
            {
                try
                {
                    using (var stream = entry.Open())
                    {
                        // Convert the ZipArchiveEntry stream to a byte array
                        byte[] imageBytes;
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            imageBytes = memoryStream.ToArray();
                        }

                        // Decode the byte array to a Bitmap
                        var bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);

                        // Determine the image format
                        var imageFormat = entry.FullName.Split('.')[^1] switch
                        {
                            "jpg" => Bitmap.CompressFormat.Jpeg,

                            "jpeg" => Bitmap.CompressFormat.Jpeg,
                            "png" => Bitmap.CompressFormat.Png,
                            "gif" => Bitmap.CompressFormat.Webp,
                            _ => Bitmap.CompressFormat.Jpeg
                        };

                        // Convert the Bitmap to a byte array
                        using (var byteArrayOutputStream = new MemoryStream())
                        {
                            bitmap.Compress(imageFormat, 100, byteArrayOutputStream);
                            byte[] bitmapData = byteArrayOutputStream.ToArray();

                            // Encode the byte array to a Base64 string
                            string base64String = Base64.EncodeToString(bitmapData, Base64Flags.Default);
                            return base64String;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            return "";
        }
    }
}
