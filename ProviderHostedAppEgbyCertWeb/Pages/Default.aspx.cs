using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProviderHostedAppEgbyCertWeb
{
    public partial class Default : System.Web.UI.Page
    {
        string siteName;
        string currentUser;
        List<string> listOfUsers = new List<string>();
        List<string> listOfLists = new List<string>();

        protected void Page_PreInit(object sender, EventArgs e)
        {
            Uri redirectUrl;
            switch (SharePointContextProvider.CheckRedirectionStatus(Context, out redirectUrl))
            {
                case RedirectionStatus.Ok:
                    return;
                case RedirectionStatus.ShouldRedirect:
                    Response.Redirect(redirectUrl.AbsoluteUri, endResponse: true);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    Response.Write("An error occurred while processing your request.");
                    Response.End();
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // The following code gets the client context and Title property by using TokenHelper.
            // To access other properties, the app may need to request permissions on the host web.
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                clientContext.Load(clientContext.Web, web => web.Title);
                clientContext.ExecuteQuery();
                Response.Write(clientContext.Web.Title);


                //Load the properties for the web object.
                clientContext.Load(clientContext.Web, web => web.Title);
                clientContext.ExecuteQuery();

                //Get the site name.
                siteName = clientContext.Web.Title;

                //Get the current user.
                clientContext.Load(clientContext.Web.CurrentUser);
                clientContext.ExecuteQuery();
                currentUser = clientContext.Web.CurrentUser.LoginName;

                //Load the lists from the Web object.
                ListCollection lists = clientContext.Web.Lists;
                clientContext.Load<ListCollection>(lists);
                clientContext.ExecuteQuery();

                //Load the current users from the Web object.
                UserCollection users = clientContext.Web.SiteUsers;
                clientContext.Load<UserCollection>(users);
                clientContext.ExecuteQuery();

                foreach (User siteUser in users)
                {
                    listOfUsers.Add(siteUser.LoginName);
                }


                foreach (List list in lists)
                {
                    listOfLists.Add(list.Title);
                }

                WebTitleLabel.Text = siteName;
                CurrentUserLabel.Text = currentUser;
                UserList.DataSource = listOfUsers;
                UserList.DataBind();
                ListList.DataSource = listOfLists;
                ListList.DataBind();
            }
        }

    }
}