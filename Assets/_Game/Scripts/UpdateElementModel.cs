using System;
using UniRx;

[Serializable]
public class UpdateElementModel : EyeModelBase
{
    public ReactiveProperty<bool> IsApplied;
}