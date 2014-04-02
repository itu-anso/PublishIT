using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PublishITService.Resources;

namespace PublishITService
{
    public class PublishITService : IPublishITService
    {
        public IPublishITEntities _publishITEntities { get; set; }

		public PublishITService(){}

        public PublishITService(IPublishITEntities publishITEntities = null)
        {
            _publishITEntities = publishITEntities;
        }

        public UserDTO GetUser(UserDTO inputUser)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from user in entities.user
                                where user.name == inputUser.name
                                select new UserDTO()
                                {
                                    name = user.name,
                                    birthday = user.birthday,
                                    status = user.status,
                                    email = user.email,
                                    user_id = user.user_id,
                                    //roles = ;
                                }).FirstOrDefault();
                if (foundUser != null )//&& foundUser.status == )
                {
                    return foundUser;
                }

                return new UserDTO() {name = "No user found"};
            }
        }

        public bool RegisterUser(UserDTO inputUser)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                int id;

                //indsæt eventuelt et GetUser kald og se om den findes i forvejen (og overvej om returtypen skal være andet end bool)
                if (!entities.user.Any())
                {
                    id = 1;
                }
                else
                {
                    id = entities.user.Max(u => u.user_id) + 1;
                }
                entities.user.Add(new user
                {
                    user_id = id,
                    name = inputUser.name,
                    password = inputUser.password,
                    birthday = inputUser.birthday,
                    email = inputUser.email,
                    organization_id = inputUser.organization_id,
                    //role_id = ,
                    //salt = ,
                    //status = ,

                });

                entities.SaveChanges();
            }
            return GetUser(inputUser).name.Equals(inputUser.name);
        }

        public bool DeleteUser(UserDTO inputUser)
        {
			using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from user in entities.user
                    where user.name == inputUser.name
                    select user).FirstOrDefault();

                entities.user.Remove(foundUser);

                try
                {
                    entities.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Provide for exceptions.
                }

                return GetUser(inputUser).name.Equals("No user found");
            }
        }

        public bool EditUser(UserDTO inputUser)
        {
	        using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from user in entities.user
                                where user.name == inputUser.name
                                select user).FirstOrDefault();

                if (foundUser != null)
                {
                    foundUser.name = inputUser.name;
                    foundUser.password = inputUser.password;
                    foundUser.birthday = inputUser.birthday;
                    foundUser.email = inputUser.email;
                    foundUser.organization_id = inputUser.organization_id;
                    foundUser.salt = inputUser.salt;
                    foundUser.status = inputUser.status;
                    try
                    {
                        entities.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        throw;
                    }

                    return GetUser(inputUser).Equals("");
                }
            }
	        return false;
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

        public List<string> SearchMedia(string title)
        {
            List<string> dummyList = new List<string>();
            dummyList.Add("The movie with the title: " + title + " is found. This is just dummy data for testing");
            return dummyList;
        }

        public List<string> GetMoviesByGenre(string genre)
        {
            List<string> dummyList = new List<string>();
            dummyList.Add("A movie with the genre: " + genre + " is found. This is just dummy data for testing");
            return dummyList;
        }

        public string GetMedia(int id)
        {
            string movie = "this is a test movie with id: " + id;
            return movie;
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
                    //?? er det sådan man editer?
                    foundRating.rating_id = rating;
                    try
                    {
                        entities.SaveChanges();
                    }
                    catch (Exception)
                    {
                        throw;
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
