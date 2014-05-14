using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using PublishITService.DTOs;
using PublishITService.Parsers;

namespace PublishITService
{
    public class PublishITService : IPublishITService
    {
        private readonly IPublishITEntities _publishITEntities;

		public PublishITService(){}

        public PublishITService(IPublishITEntities publishITEntities = null)
        {
            _publishITEntities = publishITEntities;
        }

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
				    name = "No user found",
                    status = "Not a user"
				};
			}
		}

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
				    username = "No user found",
				    status = "Not a user"
				};
			}
		}

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

				    entities.user.Add(new user
				    {
					    user_id = id,
					    name = inputUser.name,
					    user_name = inputUser.username,
					    password = inputUser.password,
					    birthday = inputUser.birthday,
					    email = inputUser.email,
					    organization_id = inputUser.organization_id,
					    salt = "salt",
					    status = "Active",
					    role = new Collection<role> {new role {role_id = userRole.role_id, role1 = userRole.role1}}
				    });

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

		public void UploadMedia(RemoteFileInfo request)
		{
			string extension = Path.GetExtension(request.FileName);

			IMediaParser parser = (extension == ".mp4") ? (IMediaParser)new VideoParser() : (IMediaParser) new DocumentParser();

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
				parser.StoreMedia(fileStream, request, entities);
			}
		}

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

        public List<media> SearchMedia(string title)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var medias = new List<media>();

                var foundMedia = from m in entities.media
                    where m.title == title
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

        public List<media> GetMoviesByGenre(string indputGenre)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var medias = new List<media>();

                // Search for genres. Genre contains a collection of medias that represent the specific genre
                var foundgenre = from genreName in entities.genre
                                 where genreName.genre1.Contains(indputGenre)
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

        public bool PostRating(int rating, int movieId, int userId)
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
                }
                else
                {
                    foundRating.rating_id = rating;
                    try
                    {
                        entities.SaveChanges();
                    }
                    catch (Exception)
                    {
                        throw new Exception();
                    }

                }

                return GetRating(movieId, userId) == rating;
            }
        }

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
