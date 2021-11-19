
namespace EliWorker
{
    /// <summary>
    /// ELI format error of the file being scanned
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 06.11.2021,
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// All rights reserved
    /// </summary>
    public enum CheckErrorFormatELI
    {

        /// <summary>
        /// no errors, installed by default
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Error! Pixel format mismatch
        /// </summary>
        ERROR_PIXEL_FORMAT_MISMATCH = 1,

        /// <summary>
        /// Error! The size of the title does not match or some of its fields are empty
        /// </summary>
        ERROR_HEADER_SIZE_DOES_NOT_MATCH = 2,

        /// <summary>
        /// Error! Signature does not match
        /// </summary>
        ERROR_SIGNATURE_DOES_NOT_MATCH = 3,

        /// <summary>
        /// Other errors (for example, inconsistency in the size of the processed images)
        /// </summary>
        ERROR_OTHER = 4
    }


    public class Common
    {
    }
}
