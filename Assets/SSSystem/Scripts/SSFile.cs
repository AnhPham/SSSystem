using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SSFile
{
    private static List<FileInfo> DirSearch(DirectoryInfo d, string searchFor)
    {
        List<FileInfo> founditems = d.GetFiles(searchFor).ToList();
        DirectoryInfo[] dis = d.GetDirectories();
        foreach (DirectoryInfo di in dis)
            founditems.AddRange(DirSearch(di, searchFor));

        return (founditems);
    }

    private static FileInfo SearchTemplateFile(string fileName)
    {
        string path = Application.dataPath;
        DirectoryInfo dir = new DirectoryInfo (path);
        List<FileInfo> lst = DirSearch (dir, fileName);

        if (lst.Count >= 1)
            return lst [0];

        return null;
    }

    public static string GetPathTemplateFile(string fileName)
    {
        FileInfo f = SearchTemplateFile(fileName);

        if (f == null)
            return null;

        string path = f.FullName;
        int index = path.IndexOf ("Assets");
        path = path.Substring (index);

        return path;
    }
}
