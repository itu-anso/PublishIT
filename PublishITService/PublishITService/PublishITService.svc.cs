using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using PublishITService.Resources;

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
								 select new UserDTO() {
									 name = u.name,
                                     username = u.user_name,
									 birthday = u.birthday,
									 status = u.status,
									 email = u.email,
									 user_id = u.user_id,
								 }).FirstOrDefault();


				if (foundUser != null && foundUser.status.Equals("Active")) {
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

				return new UserDTO() { name = "No user found" };
			}
		}

        public UserDTO SignIn(string username, string password)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from u in entities.user
                    where u.user_name == username && u.password == password
                    select new UserDTO()
                    {
                        name = u.name,
                        username = u.user_name,
                        birthday = u.birthday,
                        status = u.status,
                        email = u.email,
                        user_id = u.user_id,
                    }).First();


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
                return new UserDTO() {name = "Sign in failed"};
            }
        }

        public UserDTO GetUserByName(string username) {
			using (var entities = _publishITEntities ?? new RentIt09Entities()) {
				var foundUser = (from u in entities.user
								 where u.name == username
								 select new UserDTO() {
									 name = u.name,
                                     username = u.user_name,
									 birthday = u.birthday,
									 status = u.status,
									 email = u.email,
									 user_id = u.user_id,
								 }).FirstOrDefault();


				if (foundUser != null && foundUser.status.Equals("Active")) {
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

				return new UserDTO() { name = "No user found" };
			}
		}

	    public ResponseMessage RegisterUser(UserDTO inputUser)
	    {
		    using (var entities = _publishITEntities ?? new RentIt09Entities())
		    {
			    if (GetUserByName(inputUser.name).name.Equals("No user found"))
			    {
				    var salt = "salt";
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
					    role = new Collection<role>() {new role() {role_id = userRole.role_id, role1 = userRole.role1}}
				    });

				    entities.SaveChanges();

				    if (GetUserByName(inputUser.name).name.Equals(inputUser.name))
				    {
					    return new ResponseMessage() {IsExecuted = true, Message = "User registered"};
				    }

				    return new ResponseMessage() {IsExecuted = false, Message = "Registration failed"};
			    }
			    return new ResponseMessage() {IsExecuted = false, Message = "Username already exists"};
		    }
	    }

	    public ResponseMessage DeleteUser(UserDTO inputUser)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from u in entities.user
                    where u.user_id == inputUser.user_id
                    select u).FirstOrDefault();

                entities.user.Remove(foundUser);

                try
                {
                    entities.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                if (GetUserById(inputUser.user_id).name.Equals("No user found"))
                {
                    return new ResponseMessage() {IsExecuted = true, Message = "Deletion completed"};
                }
                else
                {
                    return new ResponseMessage() {IsExecuted = false, Message = "Deletion failed"};
                }
                
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
                    return new ResponseMessage() {IsExecuted = true, Message = "User edited"};
                }
                else
                {
                    return new ResponseMessage() {IsExecuted = false, Message = "Editing failed"};
                }
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

		public Stream DownloadMedia(int id)
		{
			throw new NotImplementedException();
		}

		public string StreamMedia(int userId, int movieId)
		{
			string mediaStreamed = "";

			if (RentExist(userId, movieId))
			{
				var media = "";
				using (var entities = _publishITEntities ?? new RentIt09Entities())
				{
					media = (from m in entities.media
						where m.media_id == userId
						select m.location).SingleOrDefault();
				}
				mediaStreamed = "" +
					"<video width='320' heigth='240' controls>" +
						"<source src='" + media + "' type='video/mp4'>" +
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

        //public List<media> SearchMedia(string title)
        //{
        //    using (var entities = _publishITEntities ?? new RentIt09Entities())
        //    {
        //        List<media> mediaTitles = new List<media>();

        //        var foundTitle = from mediaTitle in entities.media
        //                         where mediaTitle.title == title
        //                         select mediaTitle;

        //        foreach (media mediaTitle in foundTitle)
        //            {
        //                mediaTitles.Add(mediaTitle);
        //            }

        //        return mediaTitles;
        //    }
        //}

        public List<string> GetMoviesByGenre(string genre)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                List<string> moviesByGenre = new List<string>();

                var movieByGenre = from mov in entities.video
                                   join med in entities.media on mov.media_id equals med.media_id
                                   select med.title;

                foreach (string mov in movieByGenre)
                {
                    moviesByGenre.Add(mov);
                }

                return moviesByGenre;
            }
        }

        //public media GetMedia(int id)
        //{
        //    using (var entities = _publishITEntities ?? new RentIt09Entities())
        //    {
        //        media foundMovies = (from med in entities.media
        //                            where med.media_id == id
        //                            select med).First();

        //        return foundMovies;
        //    }
        //}

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
			DateTime date = new DateTime();
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
