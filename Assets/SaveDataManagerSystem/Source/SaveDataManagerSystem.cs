using UnityEngine;

namespace SaveDataManagerSystem;

/// <summary>
/// セーブデータの管理を行うクラス
/// </summary>
/// <remarks>
/// 本クラスでは、static　のクラスとして、ユーザーがソースコード中から参照できる機能を提供します。
/// また、動的に扱いい場合は別途サービスクラスを作成して提供します。
/// </remarks>
public static class SaveDataUtility
{
    // TODO : セーブデータのフォーマットに置き換える
    public static bool Data = false;

}
