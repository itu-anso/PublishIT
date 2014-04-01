using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace PublishITService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IPublishITService
    {
        // Operation contracts for users

        [OperationContract]
        [WebInvoke (Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        UserDTO GetUser(UserDTO user);

        [OperationContract]
        ResponseMessage RegisterUser(UserDTO user);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ResponseMessage DeleteUser(UserDTO user);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ResponseMessage EditUser(UserDTO user);


        // Operation contracts for media

        //[OperationContract]
        //bool UploadMedia(File media);

        //[OperationContract]
        //File DownloadMedia(int id);

        [OperationContract]
        string StreamMedia(int id);

        //[OperationContract]
        //List<string> SearchMedia(string title);

        [OperationContract]
        List<string> GetMoviesByGenre(string genre);

        //[OperationContract]
        //string GetMedia(int id);


        // Operation contracts for rating
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        int GetRating(int movieId, int userId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        bool PostRating(int rating, int movieId, int userId);
    }
}
