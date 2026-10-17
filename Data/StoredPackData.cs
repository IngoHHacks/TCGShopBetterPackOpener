namespace TCGShopBetterPackOpener;

public class StoredPackData
{
    public static StoredPackData Instance { get; private set; } = new();
    
    public readonly Dictionary<MachineRef, MachineQueue> Machines = new();
    
    public MachineRef GetOrAddMachine(InteractableAutoPackOpener machine)
    {
        var machineRef = new MachineRef(machine);
        if (!Machines.ContainsKey(machineRef))
        {
            Machines[machineRef] = new MachineQueue();
        }
        return machineRef;
    }

    public MachineRef GetMachineAtPos(Vector3 position)
    {
        var exact = Machines.FirstOrDefault(m => m.Key.Position == position).Key;
        if (exact != null)
        {
            return exact;
        }
        var closest = Machines.OrderBy(m => Vector3.Distance(m.Key.Position, position)).FirstOrDefault().Key;
        if (closest != null && Vector3.Distance(closest.Position, position) < 0.1f)
        {
            return closest;
        }
        return null;
    }

}