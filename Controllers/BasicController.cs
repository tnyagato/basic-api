
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Cors;
using System.Web.Http.Filters;
using TinCore;
using TinBasic;
using System.Configuration;
using WebResponse = TinBasic.WebResponse;

namespace basic_api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BasicController : ApiController
    {

        public static Basic basic = new Basic(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        //Get User 
        [BasicAuthentication]
        [HttpGet]
        public WebResponse GetUser(string by, string val)
        {

            WebResponse r = new WebResponse();

            r.message = "success";

            if (by == "id")
            {
                r.dt = basic.GetUserInfo(val);
            }
            else if (by == "userID")
            {
                r.dt = basic.GetProfileByUser(val);
            }
            else if (by == "all")
            {
                r.dt = basic.GetUserInfo();
            }

            return r;

        }

        //Login
        [HttpPost]
        public WebResponse Login([FromBody] User value)
        {


            WebResponse thisResponse = new WebResponse();


            string resp = basic.LoginProcess(value.userID, value.password);

            if (resp.StartsWith("success"))
            {
                thisResponse.status = "success";
                thisResponse.message = "success";


                DataTable dtUser = new DataTable("dtUser");
                dtUser = basic.GetUserInfo(value.userID);
                dtUser.TableName = "dtUser";
                thisResponse.ds.Tables.Add(dtUser);


                DataTable dtProfile = new DataTable("dtProfile");
                dtProfile = basic.GetProfileByUser(value.userID);
                dtProfile.TableName = "dtProfile";
                thisResponse.ds.Tables.Add(dtProfile);


                DataTable dtRole = new DataTable("dtRole");
                dtRole = basic.GetRoleByID(Core.GetRowData(dtProfile, "roleID"));
                dtRole.TableName = "dtRole";
                thisResponse.ds.Tables.Add(dtRole);

            }
            else
            {
                thisResponse.status = "failed";
                thisResponse.message = resp;
            }



            return thisResponse;
        }

        //Change Password
        [BasicAuthentication]
        [HttpPost]
        public WebResponse ChangePassword([FromBody] ChangePassword value)
        {
            WebResponse thisResponse = new WebResponse();

            string resp = basic.ChangePassword(value.userID, value.currentPassword, value.newPassword, value.confirmPassword);

            if (resp == "success")
            {

                thisResponse.status = "success";
                thisResponse.message = resp;
            }
            else
            {
                thisResponse.status = "failed";
                thisResponse.message = resp;

            }


            return thisResponse;
        }

        //Change Password Admin
        [BasicAuthentication]
        [HttpPost]
        public WebResponse ChangePasswordAdmin([FromBody] ChangePassword value)
        {
            WebResponse thisResponse = new WebResponse();

            string resp = basic.ChangePassword(value.userID, value.currentPassword, value.newPassword, value.confirmPassword, false);

            if (resp == "success")
            {

                thisResponse.status = "success";
                thisResponse.message = resp;
            }
            else
            {
                thisResponse.status = "failed";
                thisResponse.message = resp;

            }


            return thisResponse;
        }

        //Self Reset

        [HttpPost]
        public WebResponse SelfReset([FromBody] User value)
        {
            WebResponse thisResponse = new WebResponse();


            DataTable dtProfile = basic.GetProfileByUser(value.userID);

            //Check if user exists
            if (dtProfile.Rows.Count != 1)
            {
                //dont tell the attacker this is not a valid user - pretend it exists
                thisResponse.status = "failed";
                thisResponse.message = "Reset failed";
                return thisResponse;
            }

            //Check Email
            if (Core.GetRowData(dtProfile, "email").Contains("@") == false)
            {
                thisResponse.status = "failed";
                thisResponse.message = "No email configured";
                return thisResponse;
            }

            //generate password
            value.password = System.Web.Security.Membership.GeneratePassword(8, 2);



            string resp = basic.SetNewPassword(value.userID, value.password);

            if (resp == "success")
            {

                basic.SetChangePassword(value.userID, 1);

                //Send email
                //Core core = new Core();


                //new Thread(delegate ()
                //{
                //    core.SendEmail_Orbit(Core.GetRowData(dtProfile, "email"), "Password reset", $"Hi {Core.GetRowData(dtProfile, "firstName")},  <br/>Your new password is {value.password} ");
                //}).Start();

                thisResponse.status = "success";
                thisResponse.message = "success";
            }
            else
            {
                thisResponse.status = "failed";
                thisResponse.message = resp;
            }


            return thisResponse;
        }

        //Update User 
        [BasicAuthentication]
        [HttpPost]
        public WebResponse UpdateUser([FromBody] User value, string action)
        {

            WebResponse r = new WebResponse();

            string resp = "";

            if (action == "add")
            {
                resp = basic.AddProcess(value);
            }
            else if (action == "edit")
            {
                resp = basic.UpdateUser(value);
            }
            else if (action == "delete")
            {
                resp = resp = basic.DeleteProcess(value.userID);
            }

            else if (action == "")
            {
            }

            if (resp == "success")
            {
                r.status = "success";
                r.message = "success";
            }
            else
            {
                r.status = "failed";
                r.message = resp;
            }

            return r;

        }



        //Update Account 
        [BasicAuthentication]
        [HttpPost]
        public WebResponse UpdateAccount([FromBody] Account value, string action)
        {

            WebResponse r = new WebResponse();

            string resp = "";

            if (action == "add")
            {
                resp = basic.AddProcess(value.user);

                if (resp == "success")
                {

                    resp = basic.AddProfile(value.profile);

                    if (resp == "success")
                    {
                        r.status = "success";
                        r.message = "success";
                    }
                    else
                    {
                        r.status = "failed";
                        r.message = resp;
                    }

                }
                else
                {
                    r.status = "failed";
                    r.message = resp;
                }




            }
            else if (action == "edit")
            {
                //resp = basic.EditProfile(value);
            }
            else if (action == "delete")
            {
                //resp = basic.DeleteProfile(value);
            }
            else if (action == "")
            {
            }

            if (resp == "success")
            {
                r.status = "success";
                r.message = "success";
            }
            else
            {
                r.status = "failed";
                r.message = resp;
            }

            return r;

        }

        //Get Account 
        [BasicAuthentication]
        [HttpGet]
        public WebResponse GetAccount(string by, string val)
        {

            WebResponse r = new WebResponse();

            r.message = "success";

            if (by == "id")
            {
                r.dt = basic.GetProfileByID(val);
            }
            else if (by == "all")
            {
                r.dt = basic.GetProfiles();
            }

            return r;

        }


        //Update Profiles
        [BasicAuthentication]
        [HttpPost]
        public WebResponse UpdateProfile([FromBody] Profile value, string action)
        {

            WebResponse r = new WebResponse();

            string resp = "";

            if (action == "add")
            {
                resp = basic.AddProfile(value);
            }
            else if (action == "edit")
            {
                resp = basic.EditProfile(value);
            }
            else if (action == "delete")
            {
                resp = basic.DeleteProfile(value);
            }
            else if (action == "")
            {
            }

            if (resp == "success")
            {
                r.status = "success";
                r.message = "success";
            }
            else
            {
                r.status = "failed";
                r.message = resp;
            }

            return r;

        }

        //Get Profiles 
        [BasicAuthentication]
        [HttpGet]
        public WebResponse GetProfile(string by, string val)
        {

            WebResponse r = new WebResponse();

            r.message = "success";

            if (by == "id")
            {
                r.dt = basic.GetProfileByID(val);
            }
            else if (by == "userID")
            {
                r.dt = basic.GetProfileByUser(val);
            }
            else if (by == "all")
            {
                DataTable dtProfiles = basic.GetProfileByUser(val);
                dtProfiles.TableName = "dtProfiles";
                r.ds.Tables.Add(dtProfiles);

                DataTable dtRoles = basic.GetRoles();
                dtRoles.TableName = "dtRoles";
                r.ds.Tables.Add(dtRoles);
            }
            else if (by == "set")
            {
                DataTable dtProfiles = basic.GetProfileByUser(val);
                dtProfiles.TableName = "dtProfiles";
                r.ds.Tables.Add(dtProfiles);

                DataTable dtRoles = basic.GetRoles();
                dtRoles.TableName = "dtRoles";
                r.ds.Tables.Add(dtRoles);

                DataTable dtApprovalRoles = basic.GetApprovalRoles();
                dtApprovalRoles.TableName = "dtApprovalRoles";
                r.ds.Tables.Add(dtApprovalRoles);

                DataTable dtEmployees = basic.GetEmployees();
                dtEmployees.TableName = "dtEmployees";
                r.ds.Tables.Add(dtEmployees);
            }
            else if (by == "setapp")
            {
                DataTable dtProfiles = basic.GetProfileByUser(val);
                dtProfiles.TableName = "dtProfiles";
                r.ds.Tables.Add(dtProfiles);

                DataTable dtRoles = basic.GetRoles();
                dtRoles.TableName = "dtRoles";
                r.ds.Tables.Add(dtRoles);

                DataTable dtApprovalRoles = basic.GetApprovalRoles();
                dtApprovalRoles.TableName = "dtApprovalRoles";
                r.ds.Tables.Add(dtApprovalRoles);


            }
            else if (by == "setemp")
            {
                DataTable dtProfiles = basic.GetProfileByUser(val);
                dtProfiles.TableName = "dtProfiles";
                r.ds.Tables.Add(dtProfiles);

                DataTable dtRoles = basic.GetRoles();
                dtRoles.TableName = "dtRoles";
                r.ds.Tables.Add(dtRoles);


                DataTable dtEmployees = basic.GetEmployees();
                dtEmployees.TableName = "dtEmployees";
                r.ds.Tables.Add(dtEmployees);
            }

            return r;

        }



        //Update Roles 
        [BasicAuthentication]
        [HttpPost]
        public WebResponse UpdateRole([FromBody] Role value, string action)
        {

            WebResponse r = new WebResponse();

            string resp = "";

            if (action == "add")
            {
                resp = basic.AddRole(value);
            }
            else if (action == "edit")
            {
                resp = basic.EditRole(value);
            }
            else if (action == "delete")
            {
                resp = basic.DeleteRole(value);
            }
            else if (action == "")
            {
            }

            if (resp == "success")
            {
                r.status = "success";
                r.message = "success";
            }
            else
            {
                r.status = "failed";
                r.message = resp;
            }

            return r;

        }

        //Get Roles 
        [BasicAuthentication]
        [HttpGet]
        public WebResponse GetRole(string by, string val)
        {

            WebResponse r = new WebResponse();

            r.message = "success";

            if (by == "id")
            {
                r.dt = basic.GetRoleByID(val);
            }
            else if (by == "all")
            {
                r.dt = basic.GetRoles();
            }

            return r;

        }



        //Get Logs
        [BasicAuthentication]
        [HttpGet]
        public WebResponse GetLog(string by, string val)
        {

            WebResponse r = new WebResponse();

            r.message = "success";

            if (by == "id")
            {
                //r.dt = basic.GetRoleByID(val);
            }
            else if (by == "all")
            {
                r.dt = basic.GetLogs();
            }

            return r;

        }



        //Update Settings
        [BasicAuthentication]
        [HttpPost]
        public WebResponse UpdateSetting([FromBody] Setting value, string action)
        {

            WebResponse r = new WebResponse();

            string resp = "";

            if (action == "add")
            {
                resp = basic.AddSetting(value.sName, value.sValue);
            }
            else if (action == "edit")
            {
                resp = basic.UpdateSetting(value.sName, value.sValue, true);
            }
            else if (action == "delete")
            {
                resp = basic.DeleteSetting(value.sName);
            }
            else if (action == "")
            {
            }

            if (resp == "success")
            {
                r.status = "success";
                r.message = "success";
            }
            else
            {
                r.status = "failed";
                r.message = resp;
            }

            return r;

        }

        //Get Settings 
        [BasicAuthentication]
        [HttpGet]
        public WebResponse GetSetting(string by, string val)
        {

            WebResponse r = new WebResponse();

            r.message = "success";

            if (by == "name")
            {
                r.message = basic.GetSetting(val);
            }
            else if (by == "id")
            {
                r.dt = basic.GetSettingByID(val);
            }
            else if (by == "all")
            {
                r.dt = basic.GetSettings();
            }

            return r;


        }







        //Update ApprovalGroups 
        [BasicAuthentication]
        [HttpPost]
        public WebResponse UpdateApprovalGroup([FromBody] ApprovalGroup value, string action)
        {

            WebResponse r = new WebResponse();

            string resp = "";

            if (action == "add")
            {
                resp = basic.AddApprovalGroup(value);
            }
            else if (action == "edit")
            {
                resp = basic.EditApprovalGroup(value);
            }
            else if (action == "delete")
            {
                resp = basic.DeleteApprovalGroup(value);
            }
            else if (action == "")
            {
            }

            if (resp == "success")
            {
                r.status = "success";
                r.message = "success";
            }
            else
            {
                r.status = "failed";
                r.message = resp;
            }

            return r;

        }

        //Get ApprovalGroups 
        [BasicAuthentication]
        [HttpGet]
        public WebResponse GetApprovalGroup(string by, string val)
        {

            WebResponse r = new WebResponse();

            r.message = "success";

            if (by == "id")
            {
                r.dt = basic.GetApprovalGroupByID(val);
            }
            else if (by == "all")
            {
                r.dt = basic.GetApprovalGroups();
            }
            else if (by == "set")
            {
                DataTable dtGroups = basic.GetApprovalGroups();
                dtGroups.TableName = "dtGroups";
                r.ds.Tables.Add(dtGroups);

                DataTable dtTypes = basic.GetApprovalTypes();
                dtTypes.TableName = "dtTypes";
                r.ds.Tables.Add(dtTypes);

                DataTable dtRoles = basic.GetApprovalRoles();
                dtRoles.TableName = "dtRoles";
                r.ds.Tables.Add(dtRoles);
            }

            return r;

        }


        //Update ApprovalRoles 
        [BasicAuthentication]
        [HttpPost]
        public WebResponse UpdateApprovalRole([FromBody] ApprovalRole value, string action)
        {

            WebResponse r = new WebResponse();

            string resp = "";

            if (action == "add")
            {
                resp = basic.AddApprovalRole(value);
            }
            else if (action == "edit")
            {
                resp = basic.EditApprovalRole(value);
            }
            else if (action == "delete")
            {
                resp = basic.DeleteApprovalRole(value);
            }
            else if (action == "")
            {
            }

            if (resp == "success")
            {
                r.status = "success";
                r.message = "success";
            }
            else
            {
                r.status = "failed";
                r.message = resp;
            }

            return r;

        }

        //Get ApprovalRoles 
        [BasicAuthentication]
        [HttpGet]
        public WebResponse GetApprovalRole(string by, string val)
        {

            WebResponse r = new WebResponse();

            r.message = "success";

            if (by == "id")
            {
                r.dt = basic.GetApprovalRoleByID(val);
            }
            else if (by == "all")
            {
                r.dt = basic.GetApprovalRoles();
            }

            return r;

        }


    }


    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization != null)
            {
                var authToken = actionContext.Request.Headers.Authorization.Parameter;

                // decoding authToken we get decode value in 'Username:Password' format
                var decodeauthToken = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authToken));


                // spliting decodeauthToken using ':' 
                var arrUserNameandPassword = decodeauthToken.Split(':');

                // at 0th postion of array we get username and at 1st we get password
                if (IsAuthorizedUser(arrUserNameandPassword[0], arrUserNameandPassword[1]))
                {
                    // setting current principle
                    Thread.CurrentPrincipal = new GenericPrincipal(
                    new GenericIdentity(arrUserNameandPassword[0]), null);
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

            }
            else
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }

        public static bool IsAuthorizedUser(string Username, string Password)
        {
            return BasicController.basic.isValidToken(Username, Password);
        }

    }

}
