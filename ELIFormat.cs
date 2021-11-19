using EliWorker.Interfaces;


namespace EliWorker
{
    /// <summary>
    /// Defines the header fields of the current version of the ELI format
    /// contains default values
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 06.11.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// All rights reserved
    /// </summary>
    public class ELIFormat : IELIFormat
    {
        private int countFieldHider = 8;        // Number of header fields
        private string sign = "ELI";            // Signature
        private int head_len = 32;              // Header length in bytes
        private int data_offset = 512;          // Offset to the image from the beginning 							  // of the file in bytes
        private int reserved = 0;               // Reserved field
        private int sizeX = 512;                // image width in pixels
        private int sizeY = 512;                // image height in pixels
        private int bit_count = 16;             // Bits per pixel
        private int line_length = 1024;         // Length of one image line in bytes

        /// <summary>
        /// Number of header fields
        /// </summary>
        public int CountFieldHider
        {
            get { return countFieldHider; }
            set { countFieldHider = value; }
        }

        /// <summary>
        /// Signature associated with a specific ELI-Format
        /// </summary>
        public string Sign
        {
            get { return sign; }
            set { sign = value; }
        }

        /// <summary>
        /// Header length in bytes
        /// </summary>
        public int HeadLen
        {
            get { return head_len; }
            set { countFieldHider = value; }
        }

        /// <summary>
        /// Offset to the image from the beginning of the file in bytes
        /// </summary>
        public int DataOffset
        {
            get { return data_offset; }
            set { data_offset = value; }
        }


        /// <summary>
        /// Reserved field
        /// </summary>
        public int Reserved
        {
            get { return reserved; }
            set { reserved = value; }
        }

        /// <summary>
        /// Image width in pixels
        /// </summary>
        public int SizeX
        {
            get { return sizeX; }
            set { sizeX = value; }
        }

        /// <summary>
        /// image height in pixels
        /// </summary>
        public int SizeY
        {
            get { return sizeY; }
            set { sizeY = value; }
        }

        /// <summary>
        /// Bits per pixel
        /// </summary>
        public int BitCount
        {
            get { return bit_count; }
            set { bit_count = value; }
        }

        /// <summary>
        /// Length of one image line in bytes
        /// </summary>
        public int LineLength
        {
            get { return line_length; }
            set { line_length = value; }
        }
    }
}
