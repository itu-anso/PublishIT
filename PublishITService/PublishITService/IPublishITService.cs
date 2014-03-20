using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace PublishITService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IPublishITService
    {
        // Operation contracts for users

        [OperationContract]
        string GetUser(string userName, string password);

        [OperationContract]
        bool RegisterUser(string userName, string password, string birthday, int gender);

        [OperationContract]
        bool DeleteUser(string userName, string password);

        [OperationContract]
        bool EditUser(string userName, string password, string birthday, int gender);


        // Operation contracts for media

        [OperationContract]
        bool UploadMedia(File media);

        [OperationContract]
        File DownloadMedia(int id);

        [OperationContract]
        Stream StreamMedia(int id);

        [OperationContract]
        List<string> SearchMedia(string title);

        [OperationContract]
        List<string> GetMoviesByGenre(string genre);

        [OperationContract]
        string GetMedia(int id);


        // Operation contracts for rating
        // int? is nullable
        [OperationContract]
        int? GetRating(int movieId, int userId);

        [OperationContract]
        bool PostRating(int rating, int movieId, int userId);
    }
}
