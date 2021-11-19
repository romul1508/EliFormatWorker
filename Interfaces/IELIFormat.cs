
namespace EliWorker.Interfaces
{
    /// <summary>
    /// Provides work with ELI format
    /// author: Roman Ermakov
    /// e-mail: romul1508@gmail.com
    /// sinc 06.11.2021
    /// version: 1.0.1
    /// Copyright 2021 Roman Ermakov
    /// All rights reserved.
    /// </summary>
    public interface IELIFormat
    {
        /// <summary>
        /// Number of header fields
        /// </summary>
        public int CountFieldHider { get; set; }


        /// <summary>
        /// Signature associated with a specific ELI-Format
        /// </summary>
        public string Sign { get; set; }
        
        /// <summary>
        /// Длина заголовка в байтах
        /// </summary>
        public int HeadLen { get; set; }
        
        /// <summary>
        /// Смещение до изображения от начала файла в байтах
        /// </summary>
        public int DataOffset { get; set; }
        
        /// <summary>
        /// Зарезервированное поле
        /// </summary>
        public int Reserved { get; set; }
        

        /// <summary>
        /// Ширина изображения в пикселах
        /// </summary>
        public int SizeX { get; set; }
        

        /// <summary>
        /// высота изображения в пикселах
        /// </summary>
        public int SizeY { get; set; }        

        /// <summary>
        /// Количество бит на пиксел
        /// </summary>
        public int BitCount { get; set; }
        

        /// <summary>
        /// Длина одной строки изображения в байтах
        /// </summary>
        public int LineLength { get; set; }       

    }
}
