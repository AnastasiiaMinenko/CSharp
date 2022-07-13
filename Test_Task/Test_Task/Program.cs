using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace TestTask
{
    public struct RequiredFileInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime DateTime { get; set; }
    }
    class Parser
    {
        private const string XMLNodeRoot = "Root";
        private const string XMLNodeDirection = "dir";
        private const string XMLNodeTemplate = "template";
        private const string XMLNodeFoundedFiles = "FoundedFiles";
        private const string XMLNodeCountOfFiles = "count";
        private const string XMLNodeLastModifyDate = "LastModifyDate";
        private const string XMLNodePath = "Path";
        private const string XMLNodeName = "Name";
        private const string XMLNodeFile = "File";
        private const string XMLNodeNumberOfFile = "nr";

        private const string XMLFileSavePath = "D:\\ParsedData.xml";

        static void Main(string[] args)
        {
            Console.WriteLine("Enter the path to the folder: ");
            string folderPath = Console.ReadLine();

            Console.WriteLine("Enter the template: ");
            string template = Console.ReadLine();

            List<string> suitFilesPathes = GetFilesByTemplate(folderPath, template);

            List<RequiredFileInfo> filesInfo = GetFilesInfo(suitFilesPathes);

            xmlMaker(filesInfo, folderPath, template);
        }
        private static List<string> GetFilesByTemplate(string folderPath, string template)
        {
            List<string> files = new List<string>();
            GetFilesByTemplateRecursive(files, folderPath, template);

            return files;
        }
        private static void GetFilesByTemplateRecursive(List<string> files, string folderPath, string template)
        {
            try 
            {
                string[] filePaths = Directory.GetFileSystemEntries(folderPath);

                foreach (string filePath in filePaths)
                {
                    FileAttributes attr = File.GetAttributes(filePath);

                    var nameOfFile = Path.GetFileNameWithoutExtension(filePath);

                    if (attr.HasFlag(FileAttributes.Directory)) //folder search
                    {
                        GetFilesByTemplateRecursive(files, filePath, template);
                    }
                    else 
                    {
                        if (filePath.Contains(template))
                        {
                            files.Add(filePath);
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("Path is not valid please check if this path exists or not");
            }
        }
        private static List<RequiredFileInfo> GetFilesInfo(List<string> files)
        {
            List<RequiredFileInfo> result = new List<RequiredFileInfo>();
            foreach (string filePath in files)
            {
                result.Add(GetFileInfo(filePath));
            }

            return result;
        }
        private static RequiredFileInfo GetFileInfo(string filePath)
        {
            RequiredFileInfo fileInfo = new RequiredFileInfo()
            {
                Name = Path.GetFileNameWithoutExtension(filePath),
                Path = filePath,
                DateTime = File.GetLastWriteTime(filePath)
            };

            return fileInfo;
        }
        private static void xmlMaker(List<RequiredFileInfo> filesInfo, string folderPath, string template)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlNode rootNode = xmlDoc.CreateElement(XMLNodeRoot);
            XmlAttribute rootAttr = xmlDoc.CreateAttribute(XMLNodeDirection);
            rootAttr.InnerText = folderPath;
            rootNode.Attributes.Append(rootAttr);

            XmlAttribute templateAttr = xmlDoc.CreateAttribute(XMLNodeTemplate);
            templateAttr.InnerText = template;
            rootNode.Attributes.Append(templateAttr);
            xmlDoc.AppendChild(rootNode);

            XmlNode countNode = xmlDoc.CreateElement(XMLNodeFoundedFiles);
            XmlAttribute countAttr = xmlDoc.CreateAttribute(XMLNodeCountOfFiles);
            countAttr.InnerText = filesInfo.Count.ToString();
            countNode.Attributes.Append(countAttr);
            rootNode.AppendChild(countNode);

            for (int i = 0; i < filesInfo.Count; i++)
            {
                RequiredFileInfo fileInfo = filesInfo[i];
                XmlNode idNode = xmlDoc.CreateElement(XMLNodeFile);
                XmlAttribute idAttr = xmlDoc.CreateAttribute(XMLNodeNumberOfFile);
                idAttr.Value = (i + 1).ToString("D3");
                idNode.Attributes.Append(idAttr);
                countNode.AppendChild(idNode);

                XmlNode nameNode = xmlDoc.CreateElement(XMLNodeName);
                nameNode.InnerText = fileInfo.Name;
                idNode.AppendChild(nameNode);

                XmlNode pathNode = xmlDoc.CreateElement(XMLNodePath);
                pathNode.InnerText = fileInfo.Path;
                idNode.AppendChild(pathNode);

                XmlNode dataNode = xmlDoc.CreateElement(XMLNodeLastModifyDate);
                dataNode.InnerText = fileInfo.DateTime.ToString();
                idNode.AppendChild(dataNode);
            }
            xmlDoc.Save(XMLFileSavePath);
        }
    }
}
