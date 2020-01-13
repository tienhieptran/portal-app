using BI_Project.Models.EntityModels;
using BI_Project.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BI_SUN.Services.SetDefaultPage
{
    public class SetDefaultPageService : DBBaseService
    {
        public SetDefaultPageService(DBConnection dBConnection) : base(dBConnection)
        {

        }

        public List<EntityUserMenuModel> GetListDefaultPage(int userId)
        {
            List<EntityUserMenuModel> _listAllDefaultPage = new List<EntityUserMenuModel>();
            DBConnection.OpenDBConnect();
            try
            {

                string sqlSelectListDefaultPage = "SELECT  UserId, a.[MenuId],b.Path,b.Name,[IsDefaultPage] FROM [dbo].[UserMenu] a inner join [dbo].[Menu] b ON a.MenuId = b.MenuId and b.Path is not null and UserId = @UserId";

                DBConnection.command.CommandText = sqlSelectListDefaultPage;
                DBConnection.command.Parameters.AddWithValue("@UserId", userId);
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityUserMenuModel entityUserMenu = new EntityUserMenuModel();
                            entityUserMenu.UserId = reader.GetInt32(reader.GetOrdinal("UserId"));
                            entityUserMenu.MenuId = reader.GetInt32(reader.GetOrdinal("MenuId"));
                            entityUserMenu.Path = reader.GetString(reader.GetOrdinal("Path")).ToString().Trim();
                            entityUserMenu.Name = reader.GetString(reader.GetOrdinal("Name")).ToString().Trim();
                            entityUserMenu.IsDefaultPage = reader.IsDBNull(reader.GetOrdinal("IsDefaultPage")) ? false : reader.GetBoolean(reader.GetOrdinal("IsDefaultPage"));

                            _listAllDefaultPage.Add(entityUserMenu);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string Error = ex.ToString();
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }

            return _listAllDefaultPage;
        }

        // Update IsDefaultPage = true
        public void UpdatePageDefault(int userId, int menuId)
        {
            DBConnection.OpenDBConnect();
            try
            {
                bool isDefault = true;
                string sqlUpdateIsDefaultPage = "Update UserMenu set IsDefaultPage = 'false' Where UserId =@UserId";
                DBConnection.command.CommandText = sqlUpdateIsDefaultPage;
                DBConnection.command.Parameters.AddWithValue("@UserId", userId);
                DBConnection.command.ExecuteNonQuery();
                DBConnection.command.Parameters.Clear();

                string sqlUpdatePage = "Update  UserMenu set IsDefaultPage= @IsDefaultPage where MenuId = @MenuId and UserId = @UserId";
                DBConnection.command.CommandText = sqlUpdatePage;
                DBConnection.command.Parameters.AddWithValue("@UserId", userId);
                DBConnection.command.Parameters.AddWithValue("@MenuId", menuId);


                DBConnection.command.Parameters.AddWithValue("@IsDefaultPage", isDefault);

                DBConnection.command.ExecuteNonQuery();
                DBConnection.command.Parameters.Clear();
            }

            catch (Exception ex)
            {
                string Error = ex.ToString();
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }
        }

    }

}



