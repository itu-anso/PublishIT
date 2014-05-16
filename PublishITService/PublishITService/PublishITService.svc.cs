using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using PublishITService.DTOs;
using PublishITService.Parsers;

namespace PublishITService
{
    /// <summary>
    /// Service class to upload and download different kinds of media and 
    /// getting and registering users
    /// </summary>
    public class PublishITService : IPublishITService
    {
        private readonly IPublishITEntities _publishITEntities;

        public PublishITService()
        {
            
        }

        public PublishITService(IPublishITEntities publishITEntities = null)
        {
            _publishITEntities = publishITEntities;
        }

        /// <summary>
        /// Gets information on a user given the user's id
        /// </summary>
        /// <param name="id"> The integer id to compare with when searching in the database </param>
        /// <returns> Returns a UserDTO with all information on a user,
        /// or one with username "No user found" and status "Not a user"</returns>
		public UserDTO GetUserById(int id) {
			using (var entities = _publishITEntities ?? new RentIt09Entities()) {
				var foundUser = (from u in entities.user
								 where u.user_id == id
								 select new UserDTO {
									 name = u.name,
                                     username = u.user_name,
									 birthday = u.birthday,
									 status = u.status,
									 email = u.email,
									 user_id = u.user_id,
                                     password = u.password
								 }).FirstOrDefault();


				if (foundUser != null) {
					foundUser.roles = new List<RoleDTO>();

					var roleList = (from r in entities.role
									from u in r.user
									where u.user_id == foundUser.user_id
									select r).ToList();

					foreach (var r in roleList) {
						var x = new RoleDTO { Id = r.role_id, Title = r.role1 };
						foundUser.roles.Add(x);
					}

					return foundUser;
				}

				return new UserDTO
				{
                    birthday = DateTime.MinValue,
                    email = "no@email.com",
                    name = "",
                    password = "",
				    username = "No user found",
                    status = "Not a user"
				};
			}
		}

        /// <summary>
        /// Gets information on a user by giving a user name string and
        /// a password string
        /// </summary>
        /// <param name="username"> user's user name </param>
        /// <param name="password"> user's password </param>
        /// <returns> Returns a UserDTO with all information on a user </returns>
        public UserDTO SignIn(string username, string password)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from u in entities.user
                    where u.user_name == username && u.password == password
                    select new UserDTO
                    {
                        name = u.name,
                        username = u.user_name,
                        birthday = u.birthday,
                        status = u.status,
                        email = u.email,
                        user_id = u.user_id,
                    }).FirstOrDefault();


                if (foundUser != null && foundUser.status.Equals("Active"))
                {
                    foundUser.roles = new List<RoleDTO>();

                    var roleList = (from r in entities.role
                        from u in r.user
                        where u.user_id == foundUser.user_id
                        select r).ToList();

                    foreach (var r in roleList)
                    {
                        var x = new RoleDTO { Id = r.role_id, Title = r.role1 };
                        foundUser.roles.Add(x);
                    }

                    return foundUser;
                }
                return new UserDTO {name = "Sign in failed"};
            }
        }

        /// <summary>
        /// Gets information on a user by given the user name
        /// </summary>
        /// <param name="username"> User's user name </param>
        /// <returns> Returns a UserDTO with all information on a user,
        /// or one with username "No user found" and status "Not a user"</returns>
        public UserDTO GetUserByUserName(string username) {
			using (var entities = _publishITEntities ?? new RentIt09Entities()) {
				var foundUser = (from u in entities.user
								 where u.user_name == username
								 select new UserDTO {
									 name = u.name,
                                     username = u.user_name,
									 birthday = u.birthday,
									 status = u.status,
									 email = u.email,
									 user_id = u.user_id,
                                     password = u.password
								 }).FirstOrDefault();


				if (foundUser != null) {
					foundUser.roles = new List<RoleDTO>();

					var roleList = (from r in entities.role
									from u in r.user
									where u.user_id == foundUser.user_id
									select r).ToList();

					foreach (var r in roleList) {
						var x = new RoleDTO { Id = r.role_id, Title = r.role1 };
						foundUser.roles.Add(x);
					}

					return foundUser;
				}

				return new UserDTO
				{
                                        birthday = DateTime.MinValue,
                    email = "no@email.com",
                    name = "",
                    password = "",
				    username = "No user found",
				    status = "Not a user"
				};
			}
		}

        /// <summary>
        /// Adds a user to the database
        /// </summary>
        /// <param name="inputUser"> UserDTO containing information on the user to be added </param>
        /// <returns> A response message with a boolean value saying if the registration was a success and a message explaining why/why not </returns>
	    public ResponseMessage RegisterUser(UserDTO inputUser)
	    {
		    using (var entities = _publishITEntities ?? new RentIt09Entities())
		    {
			    if (GetUserByUserName(inputUser.username).username.Equals("No user found"))
			    {
				    int id;
				    if (!entities.user.Any())
				    {
					    id = 1;
				    }
				    else
				    {
					    id = entities.user.Max(u => u.user_id) + 1;
				    }
				    var userRole = (from r in entities.role
					    where r.role_id == 1
					    select r).FirstOrDefault();

			        var newUser = new user
			        {
			            user_id = id,
			            name = inputUser.name,
			            user_name = inputUser.username,
			            password = inputUser.password,
			            birthday = inputUser.birthday,
			            email = inputUser.email,
			            organization_id = inputUser.organization_id,
			            salt = "salt",
			            status = "Active"
			        };

                    userRole.user.Add(newUser);

				    entities.SaveChanges();

				    if (GetUserByUserName(inputUser.username).username.Equals(inputUser.username))
				    {
					    return new ResponseMessage {IsExecuted = true, Message = "User registered"};
				    }

				    return new ResponseMessage {IsExecuted = false, Message = "Registration failed"};
			    }
			    return new ResponseMessage {IsExecuted = false, Message = "Username already exists"};
		    }
	    }

        /// <summary>
        /// Soft deletes a user by changing its status to "Deleted"
        /// </summary>
        /// <param name="id"> The integer id to compare with when searching in the database </param>
        /// <returns> A response message with a boolean value saying if the deletion was a success and a message explaining why/why not </returns>
	    public ResponseMessage DeleteUser(int id)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from u in entities.user
                    where u.user_id == id
                    select u).FirstOrDefault();

                if (foundUser != null) foundUser.status = "Deleted";

                try
                {
                    entities.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                if (GetUserById(id).status.Equals("Deleted"))
                {
                    return new ResponseMessage {IsExecuted = true, Message = "Deletion completed"};
                }
                return new ResponseMessage {IsExecuted = false, Message = "Deletion failed"};
            }
        }

        /// <summary>
        /// Changes the information on a user to be the same information as given in the input
        /// </summary>
        /// <param name="inputUser"> UserDTO with the user information </param>
        /// <returns> A response message with a boolean value saying if the editing was a success and a message explaining why/why not </returns>
        public ResponseMessage EditUser(UserDTO inputUser)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from user in entities.user
                                where user.user_id == inputUser.user_id
                                select user).FirstOrDefault();

                if (foundUser != null)
                {
                    foundUser.name = inputUser.name;
                    foundUser.user_name = inputUser.username;
                    foundUser.password = inputUser.password;
                    foundUser.birthday = inputUser.birthday;
                    foundUser.email = inputUser.email;
                    foundUser.organization_id = inputUser.organization_id;
                }

                try
                    {
                        entities.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw new Exception();
                    }

                    var gottenUser = GetUserById(inputUser.user_id);

                if (gottenUser.name.Equals(inputUser.name) && gottenUser.password.Equals(inputUser.password) && gottenUser.birthday.Equals(inputUser.birthday) && gottenUser.email.Equals(inputUser.email))
                {
                    return new ResponseMessage {IsExecuted = true, Message = "User edited"};
                }
                return new ResponseMessage {IsExecuted = false, Message = "Editing failed"};
            }
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
		    }
            else if (Path.GetExtension(request.FileName) == ".pdf")
		    {
		        mediaParser = new DocumentParser();
		    }


			byte[] buffer = new byte[10000];
			int bytesRead, totalBytesRead = 0;

			do
			{
				bytesRead = request.FileStream.Read(buffer, 0, buffer.Length);
				totalBytesRead += bytesRead;
			} while (bytesRead > 0);
			
			byte[] fileStream = new byte[totalBytesRead];
			request.FileStream.Read(fileStream, 0, fileStream.Length);

			using (var entities = _publishITEntities ?? new RentIt09Entities())
			{
			    if (mediaParser != null)
			    {
			        mediaParser.StoreMedia(fileStream, request, entities);
			    }
			}
		}

        /// <summary>
        /// Downloads a media given the media's integer id
        /// </summary>
        /// <param name="id"> The id used to search in the database </param>
        /// <returns> Returns a FileStream of the found media </returns>
		public FileStream DownloadMedia(int id)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                // Get the path for the requested file
                var path = (from med in entities.media
                            where med.media_id == id
                            select med.location).FirstOrDefault();

                // Create a new FileStream with the path, how to open the file and access rights.
                FileStream stream = new FileStream(@path, FileMode.Open, FileAccess.Read);

                return stream;
            }
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
				using (var entities = _publishITEntities ?? new RentIt09Entities())
				{
                    var mediaPath = (from m in entities.media
						            where m.media_id == movieId
						            select m.location).FirstOrDefault();

                    // Set mediastreamed  with a video xml tag witch provide a screen with the requested video
                    mediaStreamed = "<video width='320' heigth='240' controls>" +
                                        "<source src='" + mediaPath + "' type='video/mp4'>" +
                                        "<source='movie.ogg' type='video/ogg'>" +
                                    "</video>";
				}
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
        public List<media> SearchMedia(string title)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var medias = new List<media>();

                var foundMedia = from m in entities.media
                                 where m.title.Contains(title)
                                 select m;

                foreach (var med in foundMedia)
                {
                    medias.Add(new media
                    {
                        media_id = med.media_id,
                        user_id = med.user_id,
                        format_id = med.format_id,
                        title = med.title,
                        average_rating = med.average_rating,
                        date = med.date,
                        description = med.description,
                        location = med.location,
                        number_of_downloads = med.number_of_downloads
                    });
                }

                return medias;
            }
        }

        /// <summary>
        /// Gets medias of a certain genre
        /// </summary>
        /// <param name="inputGenre"> The genre string used to search in the database </param>
        /// <returns> Returns a list of media objects containing the information of the media </returns>
        public List<media> GetMediaByGenre(string inputGenre)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var medias = new List<media>();

                // Search for genres. Genre contains a collection of medias that represent the specific genre
                var foundgenre = from genreName in entities.genre
                                 where genreName.genre1.Contains(inputGenre)
                                 select genreName;


                foreach (var gen in foundgenre)
                {
                    // Traversal of the media collection in the specific genre.
                    foreach (var med in gen.media)
                    {
                        medias.Add(new media
                        {
                            media_id = med.media_id,
                            user_id = med.user_id,
                            format_id = med.format_id,
                            title = med.title,
                            average_rating = med.average_rating,
                            date = med.date,
                            description = med.description,
                            location = med.location,
                            number_of_downloads = med.number_of_downloads
                        });
                    }
                }

                return medias;
            }
        }

        /// <summary>
        /// Gets a certain media by given the media's integer id
        /// </summary>
        /// <param name="id"> The id used to search in the database </param>
        /// <returns> Returns a media object with all its information </returns>
        public media GetMedia(int id)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundMedia = (from med in entities.media
                                    where med.media_id == id
                                    select med).FirstOrDefault();

                if (foundMedia != null)
                {
                    var theMedia = new media
                    {
                        media_id = foundMedia.media_id,
                        user_id = foundMedia.user_id,
                        format_id = foundMedia.format_id,
                        title = foundMedia.title,
                        average_rating = foundMedia.average_rating,
                        date = foundMedia.date,
                        description = foundMedia.description,
                        location = foundMedia.location,
                        number_of_downloads = foundMedia.number_of_downloads
                    };
                    return theMedia;
                }
                return new media
                {
                    title = "No media found"
                };
            }
        }

        /// <summary>
        /// Gets a rating on a certain movie by a certain user by given the two integer id's
        /// </summary>
        /// <param name="movieId"> The movie's id used to search in the database </param>
        /// <param name="userId"> The user's id used to search in the database </param>
        /// <returns> Returns the rating as an integer </returns>
        public int GetRating(int movieId, int userId)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundRating =
                    entities.rating.SingleOrDefault(rate => rate.media_id == movieId && rate.user_id == userId);

                if (foundRating != null)
                {
                    return foundRating.rating1;
                }
                return -1;
            }
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
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundRating =
                    entities.rating.SingleOrDefault(rate => rate.media_id == movieId && rate.user_id == userId);

                if (foundRating == null)
                {
                    int id;

                    if (!entities.rating.Any())
                    {
                        id = 1;
                    }
                    else
                    {
                        id = entities.rating.Max(r => r.rating_id) + 1;
                    }


                    entities.rating.Add(new rating
                    {
                        rating_id = id,
                        rating1 = rating,
                        user_id = userId,
                        media_id = movieId
                    });

                    entities.SaveChanges();

                    return new ResponseMessage{IsExecuted = true, Message = "Rating added"};
                }

                    foundRating.rating_id = rating;

                        entities.SaveChanges();

                        return new ResponseMessage {IsExecuted = true, Message = "Rating changed"};
            }
        }

        /// <summary>
        /// Checks if a user has the authority to rent a movie
        /// </summary>
        /// <param name="userId"> The id of the user wanting to rent the movie </param>
        /// <param name="movieId"> The movie to be rented </param>
        /// <returns> Returns a boolean saying if the movie can be rented by the user </returns>
		private bool RentExist(int userId, int movieId)
		{
			var date = DateTime.Now;
			using (var entities = _publishITEntities ?? new RentIt09Entities())
			{
				var q = from r in entities.rent
					where (r.media_id == movieId && r.user_id == userId && (r.start_date <= date && r.end_date >= date))
					select r;

				if (q.Any())
				{
					return true;
				}
			}
			return false;
		}
	}
}
