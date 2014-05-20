using System.Collections.Generic;
using System.IO;
using PublishITService.DTOs;
using PublishITService.Parsers;
using PublishITService.Repositories;

namespace PublishITService
{
    /// <summary>
    /// Service class to upload and download different kinds of media and 
    /// getting and registering users
    /// </summary>
    public class PublishITService : IPublishITService
    {
        private readonly IRepository _repository;

        public PublishITService()
        {
            _repository = new Repository();
        }

        public PublishITService(IRepository repository = null)
        {
            _repository = repository;
        }

        /// <summary>
        /// Gets information on a user given the user's id
        /// </summary>
        /// <param name="id"> The integer id to compare with when searching in the database </param>
        /// <returns> Returns a UserDTO with all information on a user </returns>
		public UserDTO GetUserById(int id) 
        {
			    return _repository.FindUserById(id);
		}

        /// <summary>
        /// Gets information on a user by giving a user name string and
        /// a password string
        /// </summary>
        /// <param name="username"> user's user name </param>
        /// <param name="password"> user's password </param>
        /// <returns> Returns a UserDTO with all information on a user </returns>
        public UserDTO SignIn(string username, string password, int organizationId)
        {
                return _repository.FindUserByUsernameAndPassword(username, password, organizationId);
        }

        /// <summary>
        /// Gets information on a user by given the user name
        /// </summary>
        /// <param name="username"> User's user name </param>
        /// <returns> Returns a UserDTO with all information on a user </returns>
        public UserDTO GetUserByUserName(string username)
        {
            return _repository.FindUserByUsername(username);
        }

        /// <summary>
        /// Adds a user to the database
        /// </summary>
        /// <param name="inputUser"> UserDTO containing information on the user to be added </param>
        /// <returns> A response message with a boolean value saying if the registration was a success and a message explaining why/why not </returns>
	    public ResponseMessage RegisterUser(UserDTO inputUser)
	    {
            if (inputUser.name != null && 
                inputUser.username != null && 
                inputUser.password != null && 
                inputUser.email != null &&
                inputUser.organization_id != 0 &&
                inputUser.birthday != null)
            {
                    return _repository.AddUser(inputUser);
            }
            return new ResponseMessage {IsExecuted = false, Message = "For registration to be performed Name, Username, Password, Email, Birthday, and Organization id has to be added"};
	    }

        /// <summary>
        /// Softly deletes a user by changing its status to "Deleted"
        /// </summary>
        /// <param name="id"> The integer id to compare with when searching in the database </param>
        /// <returns> A response message with a boolean value saying if the deletion was a success and a message explaining why/why not </returns>
	    public ResponseMessage DeleteUser(int id)
        {
                return _repository.DeleteUser(id);
        }

        /// <summary>
        /// Changes the information on a user to be the same information as given in the input
        /// </summary>
        /// <param name="inputUser"> UserDTO with the user information </param>
        /// <returns> A response message with a boolean value saying if the editing was a success and a message explaining why/why not </returns>
        public ResponseMessage EditUser(UserDTO inputUser)
        {
            _repository.EditUser(inputUser);

                    var gottenUser = GetUserById(inputUser.user_id);

                if (gottenUser.name.Equals(inputUser.name) && gottenUser.password.Equals(inputUser.password) && gottenUser.birthday.Equals(inputUser.birthday) && gottenUser.email.Equals(inputUser.email))
                {
                    return new ResponseMessage {IsExecuted = true, Message = "User edited"};
                }
                return new ResponseMessage {IsExecuted = false, Message = "Editing failed"};
            
        }

        /// <summary>
        /// Uploads a media of some format to the database
        /// </summary>
        /// <param name="request"> A RemoteFileInfo object with the requested upload </param>
        /// <returns> A response message with a boolean value saying if the upload was a success and a message explaining why/why not </returns>
		public void UploadMedia(RemoteFileInfo request)
		{
			IMediaParser mediaParser = null;

			if (Path.GetExtension(request.FileName) == ".mp4")
			{
				mediaParser = new VideoParser();
				_repository.StoreMedia(request.FileStream, request, mediaParser);
			}
			else if (Path.GetExtension(request.FileName) == ".pdf")
			{
				mediaParser = new DocumentParser();
				_repository.StoreMedia(request.FileStream, request, mediaParser);
			}
		}

        /// <summary>
        /// Downloads a media given the media's integer id
        /// </summary>
        /// <param name="id"> The id used to search in the database </param>
        /// <returns> Returns a FileStream of the found media </returns>

		public byte[] DownloadMedia(int id)
		{
			// Get the path for the requested file
			var path = _repository.GetMediaPath(id);
	        byte[] bytes;
			// Create a new FileStream with the path, how to open the file and access rights.
			// FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);

			bytes = File.ReadAllBytes(path);
	        
			return bytes;
		}

	    public string Test()
	    {
			return "test";
	    }

        /// <summary>
        /// Streams a movie by providing an HTML source code containing the path to the movie and 
        /// a screen to show the movie on
        /// </summary>
        /// <param name="userId"> The user's id used to check if he can rent the movie wanted </param>
        /// <param name="movieId"> The movie's id used to search in the database </param>
        /// <returns> Returns a string with the source code </returns>
		public string StreamMovie(int userId, int movieId)
		{
			string mediaStreamed;

            if (RentExist(userId, movieId))
            {

                var mediaPath = _repository.GetMediaPath(movieId);

                // Set mediastreamed  with a video xml tag witch provide a screen with the requested video
                mediaStreamed = "<video width='320' heigth='240' controls>" +
                                "<source src='" + mediaPath + "' type='video/mp4'>" +
                                "<source='movie.ogg' type='video/ogg'>" +
                                "</video>";
            }
            else
            {
                mediaStreamed = "" +
                                "<div>" +
                                "<span>Sorry.. It appears you did not rent this title. </span>" +
                                "</div>";
            }
            return mediaStreamed;
		}

        /// <summary>
        /// Gets a list of medias with a title containing the input string
        /// </summary>
        /// <param name="title"> The title string used to search in the database </param>
        /// <returns> Returns a list of media objects containing the information of the media </returns>
        public List<MediaDTO> SearchMedia(string title, int organizationId)
        {
            return _repository.FindMediaByTitle(title, organizationId);
        }

        /// <summary>
        /// Gets movies of a certain genre
        /// </summary>
        /// <param name="inputGenre"> The genre string used to search in the database </param>
        /// <returns> Returns a list of media objects containing the information of the media </returns>
        public List<MediaDTO> GetMoviesByGenre(string inputGenre, int organizationId)

        {
            return _repository.FindMoviesByGenre(inputGenre, organizationId);
        }


        public List<MediaDTO> GetMediaByAuthorId(int userId)
        {
            return _repository.FindMediasByAuthorId(userId);
        }

        public List<MediaDTO> GetMediaByAuthorName(string username, int organizationId)
        {
            return _repository.FindMediasByAuthorName(username, organizationId);
        }

        /// <summary>
        /// Gets a certain media by given the media's integer id
        /// </summary>
        /// <param name="id"> The id used to search in the database </param>
        /// <returns> Returns a media object with all its information </returns>
        public MediaDTO GetMedia(int id)
        {
            return _repository.FindMediaById(id);
        }

        public void AddAdminRole(int userId)
        {
            _repository.AddAdminAsRole(userId);
        }

        /// <summary>
        /// Gets a rating on a certain movie by a certain user by given the two integer id's
        /// </summary>
        /// <param name="movieId"> The movie's id used to search in the database </param>
        /// <param name="userId"> The user's id used to search in the database </param>
        /// <returns> Returns the rating as an integer if found or -1 if not found </returns>
        public int GetRating(int movieId, int userId)
        {
            var foundRating = _repository.FindRating(movieId, userId);

            if (foundRating != null)
            {
                return foundRating.rating1;
            }
            return -1;
        }

        /// <summary>
        /// Adds or changes a certain user's integer rating to a certain movie
        /// </summary>
        /// <param name="rating"> The rating of the movie </param>
        /// <param name="movieId"> The id of the movie to be rated </param>
        /// <param name="userId"> The id of the user giving the rating </param>
        /// <returns> A response message with a boolean value saying if the rating was a success and a message explaining why/why not </returns>
        public ResponseMessage PostRating(int rating, int movieId, int userId)
        {
            return _repository.PostRating(rating, movieId, userId);
        }

        /// <summary>
        /// Checks if a user has the authority to rent a movie
        /// </summary>
        /// <param name="userId"> The id of the user wanting to rent the movie </param>
        /// <param name="movieId"> The movie to be rented </param>
        /// <returns> Returns a boolean saying if the movie can be rented by the user </returns>
		private bool RentExist(int userId, int movieId)
        {
            return _repository.CheckingIfRentExists(userId, movieId);
        }
	}
}
