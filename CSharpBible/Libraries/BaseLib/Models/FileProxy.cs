using BaseLib.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLib.Models;

public class FileProxy : IFile
{
    public bool Exists(string sPath)
        => File.Exists(sPath);
    public Stream OpenRead(string sPath)
        => File.OpenRead(sPath);
    public Stream OpenWrite(string sPath)
        => File.OpenWrite(sPath);
    public Stream Create(string sPath)
        => File.Create(sPath);
    public string ReadAllText(string sPath)
        => File.ReadAllText(sPath);
    public string ReadAllText(string sPath, Encoding encoding)
        => File.ReadAllText(sPath, encoding);
    public void WriteAllText(string sPath, string sContents)
        => File.WriteAllText(sPath, sContents);
    public void WriteAllText(string sPath, string sContents, Encoding encoding)
        => File.WriteAllText(sPath, sContents, encoding);
    public byte[] ReadAllBytes(string sPath)
        => File.ReadAllBytes(sPath);
    public void WriteAllBytes(string sPath, byte[] rgBytes)
        => File.WriteAllBytes(sPath, rgBytes);
    public void Delete(string sPath)
        => File.Delete(sPath);
    public void Copy(string sSourceFileName, string sDestFileName, bool xOverwrite)
        => File.Copy(sSourceFileName, sDestFileName, xOverwrite);
    public void Move(string sSourceFileName, string sDestFileName)
        => File.Move(sSourceFileName, sDestFileName);
}
