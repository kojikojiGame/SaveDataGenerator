using UnityEngine;

/// <summary>
/// MasterDataに継承して扱うInterface
/// </summary>
/// <remarks>個数を提供することで、MasterDataの要素数をTreeを合わせる</remarks>
public interface IMasterData
{
    /// <summary>
    /// データの要素数を取得
    /// </summary>
    /// <returns>要素数を返却</returns>
    public int GetDataCount();
}