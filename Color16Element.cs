using System;


namespace EliWorker
{
    /// <summary>
    /// The class is designed for computational image processing
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 07.11.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// All rights reserved
    /// </summary>
    public class Color16Element
    {
        private ushort inputColor1 = 0;            // RGB 565 for first input image
        private ushort inputColor2 = 0;            // RGB 565 for second input image
        private ushort outputColor = 0;            // RGB 565 for output image

        protected int index_x = 0;                 // the current index to the width in 							     // pixels
        protected int index_y = 0;                 // the current height index in pixels

        protected readonly ushort red_mask = 0xF800;
        protected readonly ushort green_mask = 0x7E0;
        protected readonly ushort blue_mask = 0x1F;

        public Color16Element(ushort inputColor1, ushort inputColor2, int index_x, int index_y)
        {
            string Tag = "class Color16Element, constructor";

            this.index_x = index_x;
            // Console.WriteLine("{0}: Debug: index_x = {index_x} .", Tag);

            this.index_y = index_y;
            // Console.WriteLine("{0}: Debug: index_y = {index_y} .", Tag);

            //-------------------------------------

            this.inputColor1 = inputColor1;
            // Console.WriteLine("{0}: Debug: inputColor1 = {inputColor1} .", Tag);

            this.inputColor2 = inputColor2;
            // Console.WriteLine("{0}: Debug: inputColor2 = {inputColor2} .", Tag);

            //--------------------------------------

            byte inpR1 = (byte)((inputColor1 & red_mask) >> 11);
            // Console.WriteLine("{0}: Debug: inpR1 = {inpR1} .", Tag);

            byte inpG1 = (byte)((inputColor1 & green_mask) >> 5);
            // Console.WriteLine("{0}: Debug: inpG1 = {inpG1} .", Tag);

            byte inpB1 = (byte)(inputColor1 & blue_mask);
            // Console.WriteLine("{0}: Debug: inpB1 = {inpB1} .", Tag);

            //-----------------------------------------
            //-----------------------------------------

            byte inpR2 = (byte)((inputColor2 & red_mask) >> 11);
            // Console.WriteLine("{0}: Debug: inpR2 = {inpR2} .", Tag);

            byte inpG2 = (byte)((inputColor2 & green_mask) >> 5);
            // Console.WriteLine("{0}: Debug: inpG2 = {inpG2} .", Tag);

            byte inpB2 = (byte)(inputColor2 & blue_mask);
            // Console.WriteLine("{0}: Debug: inpB2 = {inpB2} .", Tag);

            //------------------------------------------
            //------------------------------------------
            byte outR = 0;
            try
            {
                outR = (byte)(inpR1 / inpR2);
            }
            catch (Exception e)
            {
                outR = 31;

            }
            finally
            {
                // Console.WriteLine(" {0}: Debug: outR = {outR} .", Tag);
            }
            //-------------------------------------------

            byte outG = 0;
            try
            {
                outG = (byte)(inpG1 / inpG2);
            }
            catch (Exception e)
            {
                outR = 63;
            }
            finally
            {
                // Console.WriteLine(" {0}: Debug: outG = {outG} .", Tag);
            }

            byte outB = 0;
            try
            {
                outB = (byte)(inpB1 / inpB2);
            }
            catch (Exception e)
            {
                outB = 31;
            }
            finally
            {
                // Console.WriteLine("{0}: Debug: outB = {outB} .", Tag);
            }

            //---------------------------------------
            outputColor = (ushort)((outR << 11) | (outG << 5) | outB);
        }

        /// <summary>
        /// RGB 565 for first input image
        /// </summary>
        public ushort InputColor1
        {
            get { return inputColor1; }
        }

        /// <summary>
        /// RGB 565 for second input image
        /// </summary>
        public ushort InputColor2
        {
            get { return inputColor2; }
        }

        /// <summary>
        /// RGB 565 for output image
        /// </summary>
        public ushort OutputColor
        {
            get { return outputColor; }
        }

        /// <summary>
        /// the current index to the width in pixels
        /// </summary>
        public int Index_X
        {
            get { return index_x; }
        }

        /// <summary>
        /// the current height index in pixels
        /// </summary>
        public int Index_Y
        {
            get { return index_y; }
        }
    }
}
