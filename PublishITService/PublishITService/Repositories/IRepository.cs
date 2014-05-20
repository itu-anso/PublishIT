using System.Collections.Generic;
using System.IO;
using System.Linq;
using PublishITService.DTOs;
using PublishITService.Parsers;

namespace PublishITService.Repositories
{
    /// <summary>
    /// Interface for repository class.
    /// </summary>
    public interface IRepository
    {
        UserDTO FindUserById(int id);

        UserDTO FindUserByUsername(string username);

        UserDTO FindUserByUsernameAndPassword(string username, string password);

        ResponseMessage AddUser(UserDTO newUser);

        ResponseMessage DeleteUser(int id);

        void EditUser(UserDTO inputUser);

        void StoreMedia(RemoteFileInfo mediaInfo, IMediaParser mediaParser);

        string GetMediaPath(int id);

        MediaDTO FindMediaById(int id);

        List<MediaDTO> FindMediaByTitle(string title, int organizationId);

        List<MediaDTO> FindMoviesByGenre(string inputGenre, int organizationId);

        rating FindRating(int movieId, int userId);

        ResponseMessage PostRating(int rating, int movieId, int userId);

        bool CheckingIfRentExists(int userId, int movieId);

        List<MediaDTO> FindMediasByAuthorId(int userId);
    }
}
