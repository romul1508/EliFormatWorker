using EliWorker.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliWorker
{
    /// <summary>
    /// Provides image processing (EliFileName1 elements are subdivided 
    /// into corresponding EliFileName2 elements)
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 06.11.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// All rights reserved
    /// </summary>
    public class ImageWorker : IDisposable
    {
        /// <summary>
        /// Designed to load header data of ELI files and check 
        /// them for compliance with the format
        /// </summary>
        protected IELIFormat fileFormatELI;
        protected FileStream stream_first;
        protected BinaryReader binReader_first;

        /// <summary>
        /// number of processed bytes of the first stream-stream
        /// </summary>
        protected long num_proc_bytes_first_stream = 0;

        /// <summary>
        /// contains the data of the first image
        /// </summary>
        protected ushort[] first_image_array;

        /// <summary>
        /// length of the first stream in bytes
        /// </summary>
        protected long length_first_stream = 0;
        //---------------------------
        // to load the data of the second ELI file and check
        // it for compliance with the format
        protected FileStream stream_second;
        protected BinaryReader binReader_second;

        /// <summary>
        /// number of processed bytes of the second stream-stream
        /// </summary>
        protected long num_proc_bytes_second_stream = 0;

        /// <summary>
        /// contains the data of the second image
        /// </summary>
        protected ushort[] second_image_array;

        /// <summary>
        /// length of the second stream in bytes
        /// </summary>
        protected long length_secоnd_stream = 0;
        //-------------------------------
        protected FileStream stream_result;
        protected BinaryWriter binWritRes;
        //------------------------------
        protected string nameResultFile = "";

        /// <summary>
        /// Calculates the output image,
        /// ensures safety until the image is written to the ELI file
        /// </summary>
        protected List<Color16Element> color16Elems = new List<Color16Element>();

        //----------------------------
        /// <summary>
        /// used to indicate the size of a data chunk in bytes
        /// necessary when processing file information
        /// </summary>
        protected readonly int portion_len = 4;

        public ImageWorker(string fromEliFileName1, string fromEliFileName2,
            string nameResultFile, IELIFormat fileFormatELI)
        {
            const string tag = "Class ImageWorker, Constructor:";

            this.fileFormatELI = fileFormatELI;
            stream_first = new FileStream(fromEliFileName1, FileMode.Open, FileAccess.Read);
            binReader_first = new BinaryReader(stream_first);
            length_first_stream = stream_first.Length;

            stream_second = new FileStream(fromEliFileName2, FileMode.Open, FileAccess.Read);
            binReader_second = new BinaryReader(stream_second);
            length_secоnd_stream = stream_second.Length;
            this.nameResultFile = nameResultFile;
        }

        /// <summary>
        /// Loads the data of the source files,
        /// checks for compliance with their ELI format         
        /// </summary>
        /// <returns></returns>
        public int LoadChecketSourceELI()
        {
            int res = (int)CheckErrorFormatELI.NONE;

            const string tag = "Class ImageWorker, Method: LoadChecketSourceELI()";

            // compare the sizes of the original images
            // if they do not match, then no other calculations should be
            // done (according to the conditions of the assignment)
            // do not match - immediately to the exit
            if (length_first_stream != length_secоnd_stream)
            {
                Console.WriteLine("\n {0} Error! Image sizes do not match.", tag);
                Console.ReadLine();
                return (int)CheckErrorFormatELI.ERROR_OTHER;
            }

            //-------------------------------------------
            // If the stream length is less than 1024 (in bytes),
            // then the source ELI files do not correspond to the required version of the 	     
            // ELI format
            if (length_first_stream < fileFormatELI.DataOffset)
            {
                Console.WriteLine("{0} Error! Source ELI files do not correspond to the required version of the ELI format .", tag);
                Console.ReadLine();
                return (int)CheckErrorFormatELI.ERROR_OTHER;
            }

            //--------------------------------------
            // we are not working in asynchronous mode here!
            // order computed operations matter !!
            res = CheckFirstEliHider();

            if (res != (int)CheckErrorFormatELI.NONE)
            {
                Console.WriteLine("\n {0} Error! ELI format does not match in method: CheckFirstEliHider .", tag);

                return res;
            }
            //----------------------------------
            res = CheckSecondEliHider();

            if (res != (int)CheckErrorFormatELI.NONE)
            {
                Console.WriteLine("\n {0} Error! ELI format does not match in method: 			CheckSecondEliHider .", tag);
                return res;
            }

            // get ushot arrays with images from streams
            byte[] first_full_array = new byte[stream_first.Length];
            stream_first.Read(first_full_array, 0, first_full_array.Length);

            first_image_array = new ushort[fileFormatELI.SizeX * fileFormatELI.SizeY];
            int count = fileFormatELI.SizeX * fileFormatELI.SizeY * 2;
            System.Buffer.BlockCopy(first_full_array, fileFormatELI.DataOffset, first_image_array, 0, count);
            //----------------------------
            // for the second image
            byte[] second_full_array = new byte[stream_second.Length];
            stream_second.Read(second_full_array, 0, second_full_array.Length);

            second_image_array = new ushort[fileFormatELI.SizeX * fileFormatELI.SizeY];
            System.Buffer.BlockCopy(second_full_array, fileFormatELI.DataOffset, second_image_array, 0, count);
            return res;
        }

        /// <summary>
        /// frees resources before deleting instance
        /// </summary>
        public void Dispose()
        {
            binReader_first.Close();
            binReader_second.Close();
            binWritRes.Close();

            stream_first.Close();
            stream_second.Close();
            stream_result.Close();
        }


        /// <summary>
        /// Provides header data loading and validation for the first ELI file loaded
        /// </summary>
        /// <returns></returns>
        private int CheckFirstEliHider()
        {
            int res = (int)CheckErrorFormatELI.NONE;
            const string tag = "Class ImageWorker, Method: CheckFirstEliHider()";

            binReader_first.BaseStream.Position = 0;
            byte[] utf8SignStrBytes = new byte[portion_len];

            try
            {
                // Signature verification
                // first 4 bytes of ELI file
                for (int i = 0; i < portion_len; i++)
                {
                    utf8SignStrBytes[i] = binReader_first.ReadByte();
                    num_proc_bytes_first_stream++;
                }

                // convert the received utf-8 bytes of the string into Unicode format
                Encoding utf16 = Encoding.GetEncoding("UTF-16");
                byte[] inBuffFinale = Encoding.Convert(Encoding.GetEncoding("UTF-8"), utf16, utf8SignStrBytes);
                string sign = utf16.GetString(inBuffFinale);

                StringBuilder strRes = new StringBuilder();

                int count = 0;
                int len = 3;
                char simb = '\0';

                while (count < len)
                {
                    simb = (sign.Trim()).ElementAt(count);

                    if (simb != ' ')
                    {
                        strRes.Append(simb);
                        count++;
                    }
                    else
                        break;
                }

                string result_sign = strRes.ToString();

                Console.WriteLine("{tag} Debug! signаture = { result_sign} .", tag);

                if (!result_sign.Equals(fileFormatELI.Sign))
                {
                    res = (int)CheckErrorFormatELI.ERROR_SIGNATURE_DOES_NOT_MATCH;
                    Console.WriteLine("\n {0} Error! res = 	CheckErrorFormatELI.ERROR_SIGNATURE_DOES_NOT_MATCH .", tag);
                    Console.ReadLine();
                    return res;
                }

                Console.WriteLine("{0} Debug! The signature matches the ELI format .", tag);
                //------------------------------------------
                //------------------------------------------
                // Header length in bytes
                int head_len = binReader_first.ReadInt32();
                if (head_len != fileFormatELI.HeadLen)
                {
                    // the length of the header does not match the ELI format
                    res = (int)CheckErrorFormatELI.ERROR_HEADER_SIZE_DOES_NOT_MATCH;
                    Console.WriteLine("\n {0} Error! res = CheckErrorFormatELI.ERROR_HEADER_SIZE_DOES_NOT_MATCH .", tag);
                    Console.ReadLine();
                    return res;
                }
                Console.WriteLine( "{0} Debug! The header size[{ head_len}] matches the ELI format .", tag);
                num_proc_bytes_first_stream += portion_len;
                //------------------------
                // Offset to the image from the beginning of the file in bytes
                int data_offset = binReader_first.ReadInt32();
                if (data_offset != fileFormatELI.DataOffset)
                {
                    // Offset to image does not match ELI format
                    res = (int)CheckErrorFormatELI.ERROR_HEADER_SIZE_DOES_NOT_MATCH;
                    Console.WriteLine("\n {0} Error! Offset to image [{1}] does not match ELI format .", tag, data_offset);
                    Console.ReadLine();
                    return res;
                }
                Console.WriteLine("{0} Debug! The data ofset size [{data_offset}] matches the ELI format .", tag);
                num_proc_bytes_first_stream += portion_len;
                //---------------------------------------------
                int reserved = binReader_first.ReadInt32();  // reserved
                //-------------- ------------------------------
                // width and height of the image in pixels (512 bytes in this job)
                int sizeX = binReader_first.ReadInt32();
                if (sizeX != fileFormatELI.SizeX)
                {
                    // does not match the width of the image in pixels 
                    res = (int)CheckErrorFormatELI.ERROR_OTHER;
                    Console.WriteLine("\n {0} Error! Does not match the width of the image in pixels [{1}] .", tag, sizeX);
                    Console.ReadLine();
                    return res;
                }
                Console.WriteLine("{tag} Debug! The width of the image in pixels [{sizeX}]matches the ELI format .", tag);
                num_proc_bytes_first_stream += portion_len;
                //---------------------------------------------
                // image height in pixels (512 bytes in this job)
                int sizeY = binReader_first.ReadInt32();
                if (sizeY != fileFormatELI.SizeY)
                {
                    // does not match the height of the image in pixels 
                    res = (int)CheckErrorFormatELI.ERROR_OTHER;
                    Console.WriteLine("\n {0} Error! Does not match the height of the image in pixels [{1}] .", tag, sizeY);
                    Console.ReadLine();
                    return res;
                }
                Console.WriteLine("\n {0} Debug! The height of the image in pixels [{sizeY}]  matches the ELI format .", tag);
                num_proc_bytes_first_stream += portion_len;
                //-------------------------------------------
                // Bits per pixel (color depth)
                int bit_count = binReader_first.ReadInt32();
                if (bit_count != fileFormatELI.BitCount)
                {
                    // color depth of the image does not match 
                    res = (int)CheckErrorFormatELI.ERROR_OTHER;
                    Console.WriteLine("\n {0} Error! Does not match the bit count of the 				image [{1}] .", tag, bit_count);
                    Console.ReadLine();
                    return res;
                }
                Console.WriteLine("{0} Debug! The bit_count of the image [{bit_count}]  matches the ELI format .", tag);
                num_proc_bytes_first_stream += portion_len;
                //-------------------------------------------
                // Length of one image line in bytes
                int line_length = binReader_first.ReadInt32();
                if (line_length != fileFormatELI.LineLength)
                    Console.WriteLine("{0} Attention! An inaccuracy in the line_length parameter was found in the ELI file [{line_length}] .", tag);
                else
                    Console.WriteLine("{0} Debug! The line_length of the image [{line_length}] matches the ELI format .", tag);
                num_proc_bytes_first_stream += portion_len;
            }
            catch (Exception e)
            {
                res = (int)CheckErrorFormatELI.ERROR_OTHER;
                Console.WriteLine("\n {0} Error! {1} .", tag, e.Message);
                Console.ReadLine();
            }

            return res;
        }

        /// <summary>
        /// Provides the loading of header data and their validation 
        /// for the second loaded ELI file, the header is written to the output file
        /// </summary>
        /// <returns></returns>
        private int CheckSecondEliHider()
        {
            int res = (int)CheckErrorFormatELI.NONE;
            const string tag = "Class ImageWorker, Method: CheckSecondEliHider()";
            binReader_first.BaseStream.Position = 0;
            byte[] utf8SignStrBytes = new byte[portion_len];

            try
            {
                // Signature verification
                // первые 4 байта ELI- файла
                for (int i = 0; i < portion_len; i++)
                {
                    utf8SignStrBytes[i] = binReader_second.ReadByte();
                    num_proc_bytes_second_stream++;
                }

                // convert the received utf-8 bytes of the string into Unicode format
                Encoding utf16 = Encoding.GetEncoding("UTF-16");
                byte[] inBuffFinale = Encoding.Convert(Encoding.GetEncoding("UTF-8"), utf16, utf8SignStrBytes);
                string sign = utf16.GetString(inBuffFinale);

                Console.WriteLine($"\n {0} Debug! signаture = {sign} .", tag);

                StringBuilder strRes = new StringBuilder();
                int count = 0;
                int len = 3;
                char simb = '\0';

                while (count < len)
                {
                    simb = (sign.Trim()).ElementAt(count);

                    if (simb != ' ')
                    {
                        strRes.Append(simb);
                        count++;
                    }
                    else
                        break;
                }

                string result_sign = strRes.ToString();

                //Console.WriteLine("{0} Debug! signаture = {result_sign} .", tag);

                if (!result_sign.Equals(fileFormatELI.Sign))
                {
                    res = (int)CheckErrorFormatELI.ERROR_SIGNATURE_DOES_NOT_MATCH;
                    Console.WriteLine("{0} Error! res = CheckErrorFormatELI.ERROR_SIGNATURE_DOES_NOT_MATCH .", tag);
                    Console.ReadLine();
                    return res;
                }

                Console.WriteLine("{0} Debug! The signature matches the ELI format .", tag);
                //------------------------------------------
                //------------------------------------------
                // Header length in bytes
                int head_len = binReader_second.ReadInt32();
                if (head_len != fileFormatELI.HeadLen)
                {
                    // the length of the header does not match the ELI format
                    res = (int)CheckErrorFormatELI.ERROR_HEADER_SIZE_DOES_NOT_MATCH;
                    Console.WriteLine("{0} Error! res = CheckErrorFormatELI.ERROR_HEADER_SIZE_DOES_NOT_MATCH .", tag);
                    Console.ReadLine();
                    return res;
                }
                Console.WriteLine("{0} Debug! The header size [{head_len}] matches the ELI format .", tag);
                num_proc_bytes_second_stream += portion_len;
                //------------------------
                // Offset to the image from the beginning of the file in bytes
                int data_offset = binReader_second.ReadInt32();
                if (data_offset != fileFormatELI.DataOffset)
                {
                    // Offset to image does not match ELI format
                    res = (int)CheckErrorFormatELI.ERROR_HEADER_SIZE_DOES_NOT_MATCH;
                    Console.WriteLine("{0} Error! Offset to image [{1}] does not match ELI format .", tag, data_offset);
                    Console.ReadLine();
                    return res;
                }
                Console.WriteLine($"\n {tag} Debug! The data ofset size [{data_offset}] matches the ELI format .");
                num_proc_bytes_second_stream += portion_len;
                //---------------------------------------------
                int reserved = binReader_second.ReadInt32();  // reserved
                //--------------------------------------------
                // width and height of the image in pixels (512 bytes in this job)
                int sizeX = binReader_second.ReadInt32();
                if (sizeX != fileFormatELI.SizeX)
                {
                    // does not match the width of the image in pixels 
                    res = (int)CheckErrorFormatELI.ERROR_OTHER;

                    Console.WriteLine("\n {0} Error! Does not match the width of the image in pixels [{1}] .", tag, sizeX);
                    Console.ReadLine();
                    return res;
                }
                Console.WriteLine("{0} Debug! The width of the image in pixels [{sizeX}] matches the ELI format .", tag);
                num_proc_bytes_second_stream += portion_len;
                //---------------------------------------------
                // image height in pixels (512 bytes in this job)
                int sizeY = binReader_second.ReadInt32();
                if (sizeY != fileFormatELI.SizeY)
                {
                    // does not match the height of the image in pixels 
                    res = (int)CheckErrorFormatELI.ERROR_OTHER;
                    Console.WriteLine("{0} Error! Does not match the height of the image in pixels [{1}] .", tag, sizeY);
                    Console.ReadLine();
                    return res;
                }
                Console.WriteLine("{tag} Debug! The height of the image in pixels [{sizeY}]  matches the ELI format .", tag);
                num_proc_bytes_second_stream += portion_len;
                //-------------------------------------------
                // Bits per pixel (color depth)
                int bit_count = binReader_second.ReadInt32();
                if (bit_count != fileFormatELI.BitCount)
                {
                    // color depth of the image does not match 
                    res = (int)CheckErrorFormatELI.ERROR_OTHER;
                    Console.WriteLine("\n {0} Error! Does not match the bit count of the 				image [{1}] .", tag, bit_count);
                    Console.ReadLine();
                    return res;
                }
                Console.WriteLine("{0} Debug! The bit_count of the image [{bit_count}] matches the ELI format .", tag);
                num_proc_bytes_second_stream += portion_len;
                //-------------------------------------------
                // Length of one image line in bytes
                int line_length = binReader_second.ReadInt32();
                if (line_length != fileFormatELI.LineLength)
                    Console.WriteLine("{0} Attention! An inaccuracy in the line_length parameter was found in the ELI file [{line_length}] .", tag);
                else
                    Console.WriteLine("{0} Debug! The line_length of the image [{line_length}] matches the ELI format .", tag);
                num_proc_bytes_second_stream += portion_len;
            }
            catch (Exception e)
            {
                res = (int)CheckErrorFormatELI.ERROR_OTHER;
                Console.WriteLine("\n {0} Error! {1} .", tag, e.Message);
                Console.ReadLine();
            }

            return res;
        }

        /// <summary>
        /// Performs the required high-speed image processing
        /// </summary>
        public void ImageProc()
        {
            string Tag = "Class ImageWorker, ImageProc Method: ";
            color16Elems.Clear();

            // let's go to the beginning of the images
            binReader_first.BaseStream.Position = 512;
            binReader_second.BaseStream.Position = 512;

            // perform parallel high-speed image processing
            // using all processor cores
            Console.WriteLine("{0} Debug: Fast image processing starts…", Tag);

            try
            {
                Parallel.For(0, fileFormatELI.SizeY, LineProc);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Error: {e.Message}…", Tag, e.Message);
            }

            Console.WriteLine("{0} Debug: high-speed image processing completed!", Tag);
        }

        private void LineProc(int y)
        {
            string Tag = "Class ImageWorker, LineProc Method: ";

            ushort inputColor1 = 255;
            ushort inputColor2 = 255;

            int index = -1;

            for (int x = 0; x < fileFormatELI.SizeX; ++x)
            {
                try
                {
                    index = fileFormatELI.SizeX * y + x;
                    inputColor1 = first_image_array[index];
                    inputColor2 = second_image_array[index];
                }
                catch (Exception e)
                {
                    Console.WriteLine("\n {0} Error: {1}…", Tag, e.Message);
                    Console.WriteLine("\n {0} Error: x = {x}, y = {y} .", Tag);
                }
                finally
                {
                    try
                    {
                        if ((inputColor1 != 255) && (inputColor2 != 255))
                        {
                            Color16Element color16 = new Color16Element(inputColor1, inputColor2, x, y);
                            color16Elems.Add(color16);

                            // Console.WriteLine("{Tag} inputColor1 = {inputColor1}, x = 				  //	{x}, y = {y} .", Tag);
                            // Console.WriteLine("{Tag} inputColor2 = {inputColor2}, x = 				  //	{x}, y = {y}  .", Tag);
                        }
                        else
                        {
                            Console.WriteLine("{0} Error2: we create the color ourselves…", Tag);

                            // write in light colors
                            byte r = 31;
                            byte g = 63;
                            byte b = 31;

                            inputColor1 = (ushort)((r << 11) | (g << 5) | b);
                            inputColor2 = (ushort)((r << 11) | (g << 5) | b);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("{0} Error2: {e.Message}…", Tag);
                    }
                }
            }
        }

        /// <summary>
        /// saves the processed image in ELI format
        /// </summary>
        public void SaveImage()
        {
            string Tag = "Class ImageWorker, SaveImage method: ";

            // counter of bytes written to the output file
            int num_proc_bytes_out_stream = 0;

            // We save the result in ELI format
            File.Delete(nameResultFile);
            //----------------------
            stream_result = new FileStream(nameResultFile, FileMode.Create, FileAccess.Write);

            // Convert a unicode sign string to UTF-8 format       
            UnicodeEncoding streamEncoding = new UnicodeEncoding();

            byte[] signStrBuffer = streamEncoding.GetBytes(fileFormatELI.Sign);

            Encoding utf8 = Encoding.GetEncoding("UTF-8");

            byte[] utf8SignStrBytes = Encoding.Convert(
                        Encoding.GetEncoding("UTF-16"), utf8, signStrBuffer);

            byte empty = 0;

            // Writing binary data
            binWritRes = new BinaryWriter(stream_result);

            binWritRes.Write(utf8SignStrBytes);
            binWritRes.Write(empty);
            num_proc_bytes_out_stream = portion_len;

            binWritRes.Write(fileFormatELI.HeadLen);
            num_proc_bytes_out_stream += portion_len;

            binWritRes.Write(fileFormatELI.DataOffset);
            num_proc_bytes_out_stream += portion_len;

            binWritRes.Write(fileFormatELI.Reserved);
            num_proc_bytes_out_stream += portion_len;

            binWritRes.Write(fileFormatELI.SizeX);
            num_proc_bytes_out_stream += portion_len;

            binWritRes.Write(fileFormatELI.SizeY);
            num_proc_bytes_out_stream += portion_len;

            binWritRes.Write(fileFormatELI.BitCount);
            num_proc_bytes_out_stream += portion_len;

            binWritRes.Write(fileFormatELI.LineLength);
            num_proc_bytes_out_stream += portion_len;

            byte[] reserv_null_arraj = new byte[478];

            binWritRes.Write(reserv_null_arraj);
            num_proc_bytes_out_stream += 478;

            ushort outputColor = 0;
            //-----------------------------
            ushort inputColor1 = 255;
            ushort inputColor2 = 255;

            ushort red_mask = 0xF800;
            ushort green_mask = 0x7E0;
            ushort blue_mask = 0x1F;

            int r = 31;
            int g = 63;
            int b = 31;

            foreach (Color16Element color16 in color16Elems)
            {
                if (color16 != null)
                    outputColor = color16.OutputColor;
                else
                    outputColor = (ushort)((r << 11) | (g << 5) | b);

                binWritRes.Write(outputColor);
                num_proc_bytes_out_stream += 2;
            }

            //--------------------------
            // align to sizeу
            long temp = fileFormatELI.SizeX * fileFormatELI.SizeY;
            Console.WriteLine("{0} long temp = {temp}…", Tag);

            // int num_proc_bytes_out_stream
            long count = temp * 2 + fileFormatELI.DataOffset;
            Console.WriteLine("{0} long temp = {count}…", Tag);

            int def = (int)(count) - num_proc_bytes_out_stream;
            Console.WriteLine("{0} long temp = {count}…", Tag);

            if (def > 0)
            {
                def = def / 2;

                for (int i = 0; i < def; i++)
                {
                    outputColor = (ushort)((r << 11) | (g << 5) | b);
                    binWritRes.Write(outputColor);
                }
            }

            //---------------------------
            // We close everything
            binWritRes.Close();
            stream_result.Close();
        }
    }
}


