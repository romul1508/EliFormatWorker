using EliWorker.Interfaces;
using System;

/// The EliWorker program is a console program. 
/// EliWorker takes the names of two files (ELI images) of the same size as command line arguments,
/// processes the images, and saves the result to an ELI image output file.
/// As an example of image processing, each pixel 
/// of the first image is divided by its corresponding pixel of the second image.
/// Image format - 16 bits per pixel, unsigned integer (65536 grayscale). 
/// The program uses all available processor cores for multithreading.
/// The program was created in the C # programming language. 
/// It uses Microsoft ASP .NET Core version 5.0 to perform the functionality. 
/// The program functions in the line of Microsoft Windows (x64) operating systems, as well as in LINUX.

///---------------------------
/// Описание формата ELI-файла:
///---------------------------

// At the beginning of the file there is a header that describes the image parameters:

// offset   Length          Name               Description
// 0           4            signature          Signature, "ELI\0"
// 4           4            header_length      Header length in bytes
// 8           4            data_offset        Offset to the image from the beginning of the file in bytes
// 12(0x0C)    4            reserved           Reserved, must be 0
// 16(0x10)    4            image_width        Image width pixels
// 20(0x14)    4            image_height       Image height in pixels
// 24(0x18)    4            bit_count          Bits per pixel
// 28(0x1C)    4            line_length        Length of one image line in bytes
// 32(0x20)    480 + 512xN  Reserved

// The offset to the start of the image data must be a multiple of 512 bytes. 

//---------------------------------------------

namespace EliWorker
{
    /// <summary>
    /// The program allows you to work with images in ELI format
    /// author: Roman Ermakov
    /// e-mail: romul1508@mail.ru
    /// sinc 06.11.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// All rights reserved
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Please enter a name ELI Files as arguments command 			line.");
                Console.WriteLine("Error! Command line arguments not specified or entered 				incorrectly .");
                Console.ReadLine();
                return 1;
            }

            // Source files 
            string fromEliFileName1 = args[0];
            string fromEliFileName2 = args[1];

            // Result file
            string resultEliFile = args[2];

            // designed to check source ELI files and process them appropriately
            IELIFormat iELIFormat1 = new ELIFormat();

            // Create imageWorker
            ImageWorker imageWorker = new ImageWorker(fromEliFileName1, fromEliFileName2, resultEliFile, iELIFormat1);

            // load ELI source files
            // and check ELI headers for format compliance
            int ready_to_upload_image = imageWorker.LoadChecketSourceELI();

            if (ready_to_upload_image != (int)CheckErrorFormatELI.NONE)
                return 1;

            // perform high-speed image processing
            imageWorker.ImageProc();

            // save the resulting image in ELI format
            imageWorker.SaveImage();

            return 0;
        }
    }
}
