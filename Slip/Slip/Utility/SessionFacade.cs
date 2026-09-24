using Slip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Slip.Utility
{
    public class SessionFacade
    {
        #region ::Private Constant::
        private const string UserSessionIn = "UserLogin";
        private const string FormPermissionsList = "FormPermissionsList";
        private const string _FormPermissionsList = "FormPermissions";
        private const string _BarcodeGenerateNew = "BarcodeGenerateNew";
        private const string _ReceiveIssuePrint = "ReceiveIssuePrint";
        private const string _FormPermissionsGroup = "FormPermissionsGroup";
        private const string _QRGenerateNew = "QRGenerateNew";
        #endregion

        #region::Public Property::
        //public static SEC_User UserSession
        //{
        //    get
        //    {
        //        SEC_User userauth = (SEC_User)HttpContext.Current.Session[UserSessionIn];
        //        return userauth;
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[UserSessionIn] = value;
        //    }
        //}

        public static SEC_User UserSession
        {
            get
            {
                if (HttpContext.Current?.Session == null)
                    return null;

                return HttpContext.Current.Session[UserSessionIn] as SEC_User;
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                {
                    HttpContext.Current.Session[UserSessionIn] = value;
                }
            }
        }

        #endregion
        public static List<CheckPermission_User> FormPermissions
        {
            get
            {
                return (List<CheckPermission_User>)HttpContext.Current.Session[_FormPermissionsList];
            }
            set
            {
                HttpContext.Current.Session[_FormPermissionsList] = value;
            }
        }
        //public static List<BarcodeGenerateListOne> BarcodeGenerateList
        //{
        //    get
        //    {
        //        return (List<BarcodeGenerateListOne>)HttpContext.Current.Session[_BarcodeGenerateNew];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[_BarcodeGenerateNew] = value;
        //    }
        //}
        public static List<CheckPermission_User> FormPermissionsGroup
        {
            get
            {
                return (List<CheckPermission_User>)HttpContext.Current.Session[_FormPermissionsGroup];
            }
            set
            {
                HttpContext.Current.Session[_FormPermissionsGroup] = value;
            }
        }
        //public static List<HPHT_Barcode> HPHTBarcodeList
        //{
        //    get
        //    {
        //        return (List<HPHT_Barcode>)HttpContext.Current.Session[_BarcodeGenerateNew];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[_BarcodeGenerateNew] = value;
        //    }
        //}
        //public static List<ReceiveIssuePrint> ReceiveIssuePrint
        //{
        //    get
        //    {
        //        return (List<ReceiveIssuePrint>)HttpContext.Current.Session[_ReceiveIssuePrint];
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[_ReceiveIssuePrint] = value;
        //    }
        //}
        public static List<QRList> QRGenerateList
        {
            get
            {
                return (List<QRList>)HttpContext.Current.Session[_QRGenerateNew];
            }
            set
            {
                HttpContext.Current.Session[_QRGenerateNew] = value;
            }
        }
    }
}