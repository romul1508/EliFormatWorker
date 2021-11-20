# EliFormatWorker
High speed image processing
The EliWorker program is a console program. 
EliWorker takes the names of two files (ELI images) of the same size as command line arguments,
processes the images, and saves the result to an ELI image output file.
As an example of image processing, each pixel 
of the first image is divided by its corresponding pixel of the second image.
Image format - 16 bits per pixel, unsigned integer (65536 grayscale). 
The program uses all available processor cores for multithreading.
The program was created in the C # programming language. 
It uses Microsoft ASP .NET Core version 5.0 to perform the functionality. 
The program functions in the line of Microsoft Windows (x64) operating systems, as well as in LINUX.
Microsoft Visual Studio 2019 was used to develop the program.

You can follow the link https://github.com/romul1508/ImageToRGB16ELI to download the code 
for the ImageToRGB16ELI application, which allows you to convert an image in jpg format to a ELI format 
and save it in a binary file on disk.

Description of the ELI file format:

At the beginning of the file there is a header that describes the image parameters:

offset   Length          Name               Description
 0           4            signature          Signature, "ELI\0"
 4           4            header_length      Header length in bytes
 8           4            data_offset        Offset to the image from the beginning of the file in bytes
 12(0x0C)    4            reserved           Reserved, must be 0
 16(0x10)    4            image_width        Image width pixels
 20(0x14)    4            image_height       Image height in pixels
 24(0x18)    4            bit_count          Bits per pixel
 28(0x1C)    4            line_length        Length of one image line in bytes
 32(0x20)    480 + 512xN  Reserved

The offset to the start of the image data must be a multiple of 512 bytes.
