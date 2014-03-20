using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace PublishITService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class PublishITService : IPublishITService
    {

        public string GetUser(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public bool RegisterUser(string userName, string password, string birthday, int gender)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public bool EditUser(string userName, string password, string birthday, int gender)
        {
            throw new NotImplementedException();
        }

        public bool UploadMedia(File media)
        {
            throw new NotImplementedException();
        }

        public File DownloadMedia(int id)
        {
            throw new NotImplementedException();
        }

        public Stream StreamMedia(int id)
        {
            throw new NotImplementedException();
        }

        public List<string> SearchMedia(string title)
        {
            throw new NotImplementedException();
        }

        public List<string> GetMoviesByGenre(string genre)
        {
            throw new NotImplementedException();
        }

        public string GetMedia(int id)
        {
            throw new NotImplementedException();
        }

        public int? GetRating(int movieId, int userId)
        {
            throw new NotImplementedException();
        }

        public bool PostRating(int rating, int movieId, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
