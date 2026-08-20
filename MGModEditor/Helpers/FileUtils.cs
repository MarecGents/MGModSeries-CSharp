namespace MGEditor.Helpers;

public class FileUtils
{

    public List<string> GetFiles(string path, bool recursive = false, string searchPattern = "*")
    {
        var files = new List<string>(Directory.GetFiles(path, searchPattern));

        if (recursive)
        {
            files.AddRange(Directory.GetDirectories(path).SelectMany(d => GetFiles(d, recursive, searchPattern)));
        }

        return files;
    }

    public string[] GetDirectories(string path)
    {
        return Directory.GetDirectories(path);
    }

    public string GetFileExtension(string path)
    {
        return Path.GetExtension(path).Replace(".", "");
    }

    public string GetFileNameAndExtension(string path)
    {
        return Path.GetFileName(path);
    }

    public string StripExtension(string path, bool keepPath = false)
    {
        if (keepPath)
        {
            return path.StartsWith(".") ? path.Split('.')[1] : path.Split('.').First();
        }

        return Path.GetFileNameWithoutExtension(path);
    }

    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public DirectoryInfo CreateDirectory(string path)
    {
        return Directory.CreateDirectory(path);
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public string ReadFile(string path)
    {
        return File.ReadAllText(path);
    }

    /// <summary>安全读取：文件不存在或读取失败返回 null（供配置回退场景使用）。</summary>
    public string? TryReadFile(string path)
    {
        try
        {
            return FileExists(path) ? File.ReadAllText(path) : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<string> ReadFileAsync(string path)
    {
        return await File.ReadAllTextAsync(path);
    }

    public async Task<byte[]> ReadFileAsBytesAsync(string path)
    {
        return await File.ReadAllBytesAsync(path);
    }

    public bool WriteFile(string filePath, string fileContent)
    {
        if (!DirectoryExists(Path.GetDirectoryName(filePath)))
        {
            CreateDirectory(Path.GetDirectoryName(filePath));
        }

        // 原子写：先写 .tmp 再 File.Replace/Move，避免覆盖中途进程退出/中断留下半写文件
        // （修复：编辑器保存 config.json 非原子写导致服务端二次启动读损坏文件）
        var tmpPath = filePath + ".tmp";
        try
        {
            File.WriteAllText(tmpPath, fileContent);
            if (FileExists(filePath))
            {
                File.Replace(tmpPath, filePath, null);
            }
            else
            {
                File.Move(tmpPath, filePath);
            }
            return true;
        }
        catch
        {
            // 失败：清理残留 .tmp，返回 false 供调用方提示
            try { if (FileExists(tmpPath)) File.Delete(tmpPath); } catch { }
            return false;
        }
    }

    public void WriteFile(string filePath, byte[] fileContent)
    {
        if (!FileExists(filePath))
        {
            CreateFile(filePath);
        }

        File.WriteAllBytes(filePath, fileContent);
    }

    public async Task WriteFileAsync(string filePath, string fileContent)
    {
        if (!DirectoryExists(Path.GetDirectoryName(filePath)))
        {
            CreateDirectory(Path.GetDirectoryName(filePath));
        }

        if (!FileExists(filePath))
        {
            CreateFile(filePath);
        }

        await File.WriteAllTextAsync(filePath, fileContent);
    }

    public async Task WriteFileAsync(string filePath, byte[] fileContent)
    {
        if (!FileExists(filePath))
        {
            CreateFile(filePath);
        }

        await File.WriteAllBytesAsync(filePath, fileContent);
    }

    private void CreateFile(string filePath)
    {
        var stream = File.Create(filePath);
        stream.Close();
    }

    public bool DeleteFile(string filePath)
    {
        if (!FileExists(filePath))
        {
            return false;
        }

        File.Delete(filePath);
        return true;
    }

    /// <summary>
    ///     Copy a file from one path to another
    /// </summary>
    /// <param name="copyFromPath">Source file to copy from</param>
    /// <param name="destinationFilePath"></param>
    /// <param name="overwrite">Should destination file be overwritten</param>
    public bool CopyFile(string copyFromPath, string destinationFilePath, bool overwrite = false)
    {
        // Check it exists first
        if (!FileExists(copyFromPath))
        {
            return false;
        }

        // Ensure dir exists
        Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath));

        // Copy the file
        File.Copy(copyFromPath, destinationFilePath, overwrite);
        return true;
    }

    /// <summary>
    ///     Delete a directory, must be empty unless 'deleteContent' is set to 'true'
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="deleteContent"></param>
    public void DeleteDirectory(string directory, bool deleteContent = false)
    {
        Directory.Delete(directory, deleteContent);
    }

}