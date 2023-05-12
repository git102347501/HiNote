namespace HelloNote.Core.Contracts.Services;

public interface IFileService
{
    /// <summary>
    /// 读取文件
    /// </summary>
    /// <param name="folderPath"></param>
    /// <param name="fileName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T Read<T>(string folderPath, string fileName);

    /// <summary>
    /// 保存文件
    /// </summary>
    /// <param name="folderPath"></param>
    /// <param name="fileName"></param>
    /// <param name="content"></param>
    /// <typeparam name="T"></typeparam>
    void Save<T>(string folderPath, string fileName, T content);

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="folderPath"></param>
    /// <param name="fileName"></param>
    void Delete(string folderPath, string fileName);
}