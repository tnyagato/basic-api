using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading;
using System.Web.Http.Filters;
using basic_api.Models;
using TinBasic;

namespace basic_api.Controllers
{
    public class GroupsController : ApiController
    {

        //Update Groups 
        [BasicAuthentication]
        [HttpPost]
        public WebApi.WebResponse UpdateGroup([FromBody] Group value, string action)
        {

            WebApi.WebResponse r = new WebApi.WebResponse();

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
        public WebApi.WebResponse GetGroup(string by, string val)
        {

            WebApi.WebResponse r = new WebApi.WebResponse();

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