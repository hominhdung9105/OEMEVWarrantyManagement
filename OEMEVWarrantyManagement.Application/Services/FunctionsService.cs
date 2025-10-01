using System.Data;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Share.Enum;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class FunctionsService : IFunctionsService
    {
        public List<RoleScreenPermission> GetFunctions(string roleId)
        {
            var permissions = _permissions[roleId]
            .Select(p => new RoleScreenPermission
            {
                Screen = p.Screen,
                Actions = p.Actions
            })
            .ToList();

            return permissions;
        }

        private static readonly Dictionary<string, List<(ScreenEnum Screen, ActionEnum Actions)>> _permissions
            = new()
        {
            {
                RoleIdEnum.ScStaff.GetRoleId(), new List<(ScreenEnum, ActionEnum)>
                {
                    (ScreenEnum.Dashboard, ActionEnum.View),
                    (ScreenEnum.Warranty, ActionEnum.Create | ActionEnum.View | ActionEnum.Update),
                    (ScreenEnum.Campaign, ActionEnum.View | ActionEnum.Schedule),
                    (ScreenEnum.SpareParts, ActionEnum.Create | ActionEnum.View), // Request, Track
                    (ScreenEnum.Reports, ActionEnum.View)
                }
            },
            {
                RoleIdEnum.Technician.GetRoleId(), new List<(ScreenEnum, ActionEnum)>
                {
                    (ScreenEnum.Dashboard, ActionEnum.View),
                    (ScreenEnum.Warranty, ActionEnum.View | ActionEnum.Update), // update kết quả kỹ thuật
                    (ScreenEnum.Campaign, ActionEnum.View | ActionEnum.Update), // update trạng thái task
                    (ScreenEnum.SpareParts, ActionEnum.View) // xem phụ tùng được giao
                }
            },
            {
                RoleIdEnum.EvmStaff.GetRoleId(), new List<(ScreenEnum, ActionEnum)>
                {
                    (ScreenEnum.Dashboard, ActionEnum.View),
                    (ScreenEnum.Warranty, ActionEnum.View | ActionEnum.Approve),
                    (ScreenEnum.Campaign, ActionEnum.Create | ActionEnum.View | ActionEnum.Update | ActionEnum.Assign),
                    (ScreenEnum.SpareParts, ActionEnum.View | ActionEnum.Create | ActionEnum.Update | ActionEnum.Approve),
                    (ScreenEnum.Reports, ActionEnum.View)
                }
            },
            {
                RoleIdEnum.Admin.GetRoleId(), new List<(ScreenEnum, ActionEnum)>
                {
                    (ScreenEnum.Dashboard, ActionEnum.View),
                    (ScreenEnum.UserManagement, ActionEnum.Create | ActionEnum.View | ActionEnum.Update | ActionEnum.Delete | ActionEnum.Assign),
                    (ScreenEnum.Reports, ActionEnum.View)
                }
            }
        };
    }
}
