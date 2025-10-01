namespace OEMEVWarrantyManagement.Share.Enum
{
    public class RoleScreenPermission
    {
        public ScreenEnum Screen { get; set; }
        public ActionEnum Actions { get; set; }
    }

    public enum ScreenEnum
    {
        Dashboard,
        Warranty,
        Campaign,
        SpareParts,
        Reports,
        UserManagement
    }

    [Flags]
    public enum ActionEnum
    {
        None = 0,
        View = 1 << 0,
        Create = 1 << 1,
        Update = 1 << 2,
        Delete = 1 << 3,
        Approve = 1 << 4,
        Assign = 1 << 5,
        Schedule = 1 << 6
    }
}
