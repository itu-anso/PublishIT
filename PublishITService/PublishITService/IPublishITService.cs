using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;

namespace PublishITService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IPublishITService
    {
        // Operation contracts for users

        [OperationContract]
        UserDTO GetUser(UserDTO user);

        [OperationContract]
        ResponseMessage RegisterUser(UserDTO user);

        [OperationContract]
        ResponseMessage DeleteUser(UserDTO user);

        [OperationContract]
        ResponseMessage EditUser(UserDTO user);


        // Operation contracts for media

        [OperationContract]
        bool UploadMedia(File media);

        [OperationContract]
        File DownloadMedia(int id);

        [OperationContract]
        string StreamMedia(int id);

        [OperationContract]
        List<string> SearchMedia(string title);

        [OperationContract]
        List<string> GetMoviesByGenre(string genre);

        [OperationContract]
        string GetMedia(int id);


        // Operation contracts for rating
        // int? is nullable
        [OperationContract]
        int GetRating(int movieId, int userId);

        [OperationContract]
        bool PostRating(int rating, int movieId, int userId);
    }
}
