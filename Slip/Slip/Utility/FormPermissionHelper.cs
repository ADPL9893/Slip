using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Utility
{
    public class FormPermissionHelper
    {
        public static bool CheckFormPermission(string action, string controller)
        {



            bool isAllowed = false;
            //if (SessionFacade.UserSession == null)
            //{
            //    return false;
            //}

            //if (SessionFacade.UserSession.IsAdmin == true)
            //{
            //    isAllowed = true;
            //}
            //else
            //{
            //    //var permission = SessionFacade.FormPermissions.Where(t => t.FormGroup.ToLower().Equals(controller.ToLower()) && t.FormName.ToLower().Equals(action.ToLower()));
            //    var permission = SessionFacade.FormPermissions.Where(t => t.Controller.ToLower().Equals(controller.ToLower()) && t.Action.ToLower().Equals(action.ToLower()));
            //    if (permission != null && permission.Count() > 0)
            //    {
            //        isAllowed = true;
            //    }
            //}


            return isAllowed;
        }
        public static bool CheckFormPermissionGroup(int GroupID)
        {



            bool isAllowed = false;
            if (SessionFacade.UserSession == null)
            {
                return false;
            }

            if (SessionFacade.UserSession.IsAdmin == true)
            {
                isAllowed = true;
            }
            else
            {
                //var permission = SessionFacade.FormPermissions.Where(t => t.FormGroup.ToLower().Equals(controller.ToLower()) && t.FormName.ToLower().Equals(action.ToLower()));
                var permission = SessionFacade.FormPermissions.Where(t => t.GroupID.Equals(GroupID));
                if (permission != null && permission.Count() > 0)
                {
                    isAllowed = true; //permission.FirstOrDefault().IsActive;
                }
            }


            return isAllowed;
        }
    }
}