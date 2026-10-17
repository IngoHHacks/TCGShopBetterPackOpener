namespace TCGShopBetterPackOpener;

public class MachineRef(InteractableAutoPackOpener machine) : IEquatable<MachineRef>
{
    internal InteractableAutoPackOpener Machine => machine;

    public Vector3 Position => Machine?.transform.position ?? Vector3.zero;

    internal string PositionString => Position.FormattedString();
        
    public MachineQueue Queue => StoredPackData.Instance.Machines[this];

    public bool Equals(MachineRef other) => other != null && Machine == other.Machine;
    public override bool Equals(object obj) => obj is MachineRef other && Equals(other);

    public override int GetHashCode()
    {
        return Machine?.GetHashCode() ?? 0;
    }
}