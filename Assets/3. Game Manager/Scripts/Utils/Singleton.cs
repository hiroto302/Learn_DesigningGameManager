using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance
    {
        get { return instance; }
    }

    public static bool IsInitialized
    {
        get { return instance != null; }    // 既に存在してるかチェックできる : 存在したら true , 存在してないなら false を返す
    }

    protected virtual void Awake()
    {
        // 既に存在している時
        if (instance != null)
        {
            Debug.LogError("[Singleton] Trying to instantiate a second instance of a singleton class ");
        }
        // まだ存在していない時
        else
        {
            instance = (T) this;
        }
    }

    // シングルトンクラスがDestoryされた時、必ず参照を無くすこと。
    // Destory して Hierarch 上から消えたとしても、シーン内に残っているの他のクラスから参照されいる時、は不使用状態にならないため注意すること
    // シングルトンクラスの重複、参照型(static, delegateも同様に)はメモリーリークの原因となりうるので対策することを意識する
    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            // //nullを代入することで参照は消える
            instance = null;
        }
    }
}
