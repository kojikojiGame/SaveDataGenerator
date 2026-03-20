
namespace SaveDataManagerSystem.Model;

/// <summary>
/// 保存されたデータのロードクラス
/// </summary>
public interface SDMS_ISaveDataLoader
{
    /// <summary>
    /// 保存先ファイルパス
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// プロパティ<see cref="FilePath"/>で 指定したファイルパスからロードを行う
    /// </summary>
    /// <param name="saveData"> 取得した生データを返却 </param>
    /// <returns></returns>
    public bool Load(ref object saveData);

    /// <summary>
    /// 引数から指定したファイルパスからロードを行う
    /// </summary>
    /// <param name="filePath"> ロードを行うファイルパス </param>
    /// <param name="saveData"> 取得した生データを返却 </param>
    /// <returns></returns>
    public bool LoadFromFilePath(string filePath, ref object saveData);

}
