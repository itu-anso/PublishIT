using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace PublishITService
{
    
    [ServiceContract]
    public interface IPublishITService
    {
        // Operation contracts for users

        [OperationContract]
        [WebInvoke (Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        UserDTO GetUserById(UserDTO user);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        UserDTO GetUserByName(UserDTO user);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
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
