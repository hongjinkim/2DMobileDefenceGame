
using System;

[Serializable]
public abstract class BaseSaveData<T> 
    : IDirtyFlag, ISaveObject where T : BaseSaveData<T>
{
    protected bool DirtyFlag { get; set; } = false;

    public event Action OnDirty;
    
    // �ڵ����� ��û�ϴ� �Լ�
    public void Save()
    {
        SetDirty();
        ClientSaveManager.Save();
    }


    public string ToJSON(IJSONSerializer serializer) => serializer.Serialize((T)this);

    public bool IsDirty() => DirtyFlag;

    public void Refresh() => DirtyFlag = false;

    protected void SetDirty()
    {
        DirtyFlag = true;
        OnDirty?.Invoke();
    }

}
