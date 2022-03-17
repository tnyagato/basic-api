using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using TinCore;

namespace basic_api.Models
{
    public class GroupService
    {

        public static string con = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        private static Logger logger = new Logger(con);

        //Groups 
        public static string AddGroup(Group group)
        {

            SqlCommand objcommand = new SqlCommand();
            objcommand.CommandText = "INSERT INTO Groups " +
            "(groupName, createdDate, createdBy, updatedDate, updatedBy)" +
            " VALUES " +
            "(@groupName, @createdDate, @createdBy, @updatedDate, @updatedBy)";

            {
                var withBlock = objcommand.Parameters;
                withBlock.AddWithValue("@groupName", group.groupName);
                withBlock.AddWithValue("@createdDate", DateTime.Now);
                withBlock.AddWithValue("@createdBy", Core.GetCurrentUser());
                withBlock.AddWithValue("@updatedDate", DateTime.Now);
                withBlock.AddWithValue("@updatedBy", Core.GetCurrentUser());
            }

            TinDBv2 db = new TinDBv2(con);

            string resp = db.ProcessCommand(objcommand).message;

            if (resp == "success")
            {
                logger.WriteLog("", "", Core.GetCurrentMethod(), $"Added group: {group.groupID}  {group.groupName}");
            }

            return resp;



        }

        public static DataTable GetGroupByID(string id)
        {

            SqlCommand objcommand = new SqlCommand();
            objcommand.CommandText = "SELECT * FROM Groups WHERE groupID = @groupID ";
            objcommand.Parameters.AddWithValue("@groupID", id);

            TinDBv2 db = new TinDBv2(con);
            return db.ProcessCommand(objcommand).dt;

        }

        public static DataTable GetGroups()
        {

            SqlCommand objcommand = new SqlCommand();
            objcommand.CommandText = "SELECT * FROM Groups ";

            TinDBv2 db = new TinDBv2(con);
            return db.ProcessCommand(objcommand).dt;

        }

        public static DataTable GetGroupsNew(string id)
        {

            SqlCommand objcommand = new SqlCommand();
            objcommand.CommandText = "SELECT * FROM groups WHERE groupID not in (SELECT groupID FROM contacts WHERE custid = @custID) ";
            objcommand.Parameters.AddWithValue("@custID", id);

            TinDBv2 db = new TinDBv2(con);
            return db.ProcessCommand(objcommand).dt;

        }

        public static string EditGroup(Group group)
        {

            SqlCommand objcommand = new SqlCommand();
            objcommand.CommandText = "UPDATE Groups SET groupName = @groupName, updatedDate = @updatedDate, updatedBy = @updatedBy WHERE groupID = @groupID ";

            {
                var withBlock = objcommand.Parameters;
                withBlock.AddWithValue("@groupID", group.groupID);
                withBlock.AddWithValue("@groupName", group.groupName);
                withBlock.AddWithValue("@updatedDate", DateTime.Now);
                withBlock.AddWithValue("@updatedBy", Core.GetCurrentUser());
            }

            TinDBv2 db = new TinDBv2(con);


            string resp = db.ProcessCommand(objcommand).message;

            if (resp == "success")
            {
                logger.WriteLog("", "", Core.GetCurrentMethod(), $"Updated group: {group.groupID}  {group.groupName}");
            }

            return resp;



        }

        public static string DeleteGroup(Group group)
        {

            SqlCommand objcommand = new SqlCommand();
            objcommand.CommandText = "DELETE FROM Groups WHERE groupID = @groupID ";
            objcommand.Parameters.AddWithValue("@groupID", group.groupID);

            TinDBv2 db = new TinDBv2(con);
            return db.ProcessCommand(objcommand).message;

            string resp = db.ProcessCommand(objcommand).message;

            if (resp == "success")
            {
                logger.WriteLog("", "", Core.GetCurrentMethod(), $"Deleted group: {group.groupID}  {group.groupName}");
            }

            return resp;

        }
    }

    public class Group
    {
        public Group()
        {
            groupID = 0;
            groupName = "";
            createdDate = Core.NullDateTime;
            createdBy = "";
            updatedDate = Core.NullDateTime;
            updatedBy = "";
        }

        public int groupID { get; set; }
        public string groupName { get; set; }
        public DateTime createdDate { get; set; }
        public string createdBy { get; set; }
        public DateTime updatedDate { get; set; }
        public string updatedBy { get; set; }
    }


}

