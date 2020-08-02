using System;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace PrprojVersionChanger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("请输入Pr项目文件地址：");
            string path = Console.ReadLine().Trim().Trim('"');
            prprojFile = new FileInfo(path);
            Decompress();
            ReadXml();
            Compress();
            Console.WriteLine($"正在删除文件");
            prprojXml.Delete();
            prprojTargetXml.Delete();
            Console.WriteLine("完成");
            Console.ReadKey();
        }

        private static FileInfo prprojFile = null;
        private static FileInfo prprojXml = new FileInfo(Guid.NewGuid().ToString() + ".xml");
        private static FileInfo prprojTargetXml;
        private static string targetVersion;

        private static void ReadXml()
        {
            Console.WriteLine($"正在读取XML");
            XmlDocument doc = new XmlDocument();
            doc.Load(prprojXml.FullName);
            var ele = doc.DocumentElement.ChildNodes[1];

            Console.WriteLine("当前版本为：" + ele.Attributes["Version"].Value);
            Console.Write("请输入目标版本号：");
            targetVersion = Console.ReadLine().Trim();
            ele.Attributes["Version"].Value = targetVersion;
            prprojTargetXml = new FileInfo(prprojFile.Name.Split('.')[0]);
            Console.WriteLine("正在保存");
            doc.Save(prprojTargetXml.FullName);
        }
        public static void Compress()
        {
            Console.WriteLine("正在压缩");
            using FileStream originalFileStream = prprojTargetXml.OpenRead();

            using FileStream compressedFileStream = File.Create($"{prprojFile.FullName.Split('.')[0]}_{targetVersion}.prproj");
            using GZipStream compressionStream = new GZipStream(compressedFileStream,
               CompressionMode.Compress);
            originalFileStream.CopyTo(compressionStream);

        }

        public static void Decompress()
        {
            Console.WriteLine($"正在解压");
            using FileStream originalFileStream = prprojFile.OpenRead();
            string currentFileName = prprojFile.FullName;

            using FileStream decompressedFileStream = File.Create(prprojXml.FullName);
            using GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress);
            decompressionStream.CopyTo(decompressedFileStream);
            Console.WriteLine($"文件解压成功");
        }


    }
}
