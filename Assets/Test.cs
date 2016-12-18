using System;
using System.IO;
using LZ4;
using UnityEngine;
public class Test : MonoBehaviour
{
    public TextAsset textAsset;

    private void Start()
    {
        //TestZip();
        //TestLZ4();
    }

    //private void TestZip()
    //{
    //    for (int i = 0; i < 100; i++)
    //    {
    //        MemoryStream memoryStream = new MemoryStream();
    //        byte[] textBytes = textAsset.bytes;
    //        memoryStream.Write(textAsset.bytes, 0, textBytes.Length);
    //        memoryStream.Position = 0;
    //        ZipFile zip = ZipFile.Read(memoryStream);
    //        foreach (string entryFileName in zip.EntryFileNames)
    //        {
    //            ZipEntry entry = zip[entryFileName];
    //            MemoryStream extractMemoryStream = new MemoryStream();
    //            entry.Extract(extractMemoryStream);
    //        }
    //    }

    //}
    private void TestLZ4()
    {
        for (int i = 0; i < 100; i++)
        {
            MemoryStream memoryStream = new MemoryStream();
            byte[] textBytes = textAsset.bytes;
            memoryStream.Write(textAsset.bytes, 0, textBytes.Length);
            memoryStream.Position = 0;
            LZ4File lz4File = LZ4File.Read(memoryStream);
            foreach (string entryFileName in lz4File.EntryFileNames)
            {
                LZ4Entry entry = lz4File[entryFileName];
                MemoryStream extractMemoryStream = new MemoryStream();
                entry.Extract(extractMemoryStream);
            }
        }
    }
}

