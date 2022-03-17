using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Web.Http.Filters;
using WebResponse = TinBasic.WebResponse;
using basic_api.Models;

namespace basic_api.Controllers
{
    public class GroupsController : ApiController
    {

        //Update Groups 
        [BasicAuthentication]
        [HttpPost]
        public WebResponse UpdateGroup([FromBody] Group value, string action)
        {

            WebResponse r = new WebResponse();

            string resp = "";

            if (action == "add")
            {
                resp = GroupService.AddGroup(value);
            }
            else if (action == "edit")
            {
                resp = GroupService.EditGroup(value);
            }
            else if (action == "delete")
            {
                resp = GroupService.DeleteGroup(value);
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

        //Get Groups 
        [BasicAuthentication]
        [HttpGet]
        public WebResponse GetGroup(string by, string val)
        {

            WebResponse r = new WebResponse();

            r.message = "success";

            if (by == "id")
            {
                r.dt = GroupService.GetGroupByID(val);
            }
            else if (by == "all")
            {
                r.dt = GroupService.GetGroups();
            }
            else if (by == "new")
            {
                r.dt = GroupService.GetGroupsNew(val);
            }

            return r;

        }

    }

}