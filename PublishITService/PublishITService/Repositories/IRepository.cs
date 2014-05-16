using System.Collections.Generic;
using System.IO;
using System.Linq;
using PublishITService.DTOs;
using PublishITService.Parsers;

namespace PublishITService.Repositories
{
    public interface IRepository
    {
        UserDTO FindUserById(int id);

        UserDTO FindUserByUsername(string username);

        UserDTO FindUserByUsernameAndPassword(string username, string password);

        void AddUser(UserDTO newUser);

        void DeleteUser(int id);

        void EditUser(UserDTO inputUser);

        void StoreMedia(byte[] mediaStream, RemoteFileInfo mediaInfo, IMediaParser mediaParser);

        string GetMediaPath(int id);

        media FindMediaById(int id);

        List<media> FindMediaByTitle(string title);

        List<media> FindMoviesByGenre(string inputGenre);

        IQueryable<genre> FindRelatedGenres(string inputGenre);

        rating FindRating(int movieId, int userId);

        ResponseMessage PostRating(int rating, int movieId, int userId);

        bool CheckingIfRentExists(int userId, int movieId);
    }
}
