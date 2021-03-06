﻿using System;
using System.Collections.Generic;
using System.Linq;
using PublishITService.DTOs;
using PublishITService.Parsers;

namespace PublishITService.Repositories
{
    /// <summary>
    /// Class for finding, delete, and edit users. Furthermore,
    /// it gives opportunity to find media based on different criteria. 
    /// </summary>
    public class Repository : IRepository
    {
        private readonly IPublishITEntities _publishITEntities;

        public Repository() : this(new RentIt09Entities())
        {}

        public Repository(IPublishITEntities publishITEntities)
        {
            _publishITEntities = publishITEntities;
        }

        /// <summary>
        /// Find a user by his or hers ID.
        /// </summary>
        /// <param name="id">ID of a user.</param>
        /// <returns>User by the found ID.</returns>
        public UserDTO FindUserById(int id)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from u in entities.user
                    where u.user_id == id
                    select new UserDTO
                    {
                        name = u.name,
                        username = u.user_name,
                        birthday = u.birthday,
                        status = u.status,
                        email = u.email,
                        user_id = u.user_id,
                        password = u.password
                    }).FirstOrDefault();

                if (foundUser != null)
                {
                    foundUser.roles = new List<RoleDTO>();

                    var roleList = (from r in entities.role
                        from u in r.user
                        where u.user_id == foundUser.user_id
                        select r).ToList();

                    foreach (var r in roleList)
                    {
                        var x = new RoleDTO {Id = r.role_id, Title = r.role1};
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

        /// <summary>
        /// Find a user by his or hers username.
        /// </summary>
        /// <param name="username">Username of a user.</param>
        /// <returns>A user with the found username.</returns>
        public UserDTO FindUserByUsername(string username)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from u in entities.user
                                 where u.user_name == username
                                 select new UserDTO
                                 {
                                     name = u.name,
                                     username = u.user_name,
                                     birthday = u.birthday,
                                     status = u.status,
                                     email = u.email,
                                     user_id = u.user_id,
                                     password = u.password
                                 }).FirstOrDefault();


                if (foundUser != null)
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

                return new UserDTO
                {
                    username = "No user found",
                    status = "Not a user"
                };
            }
        }


        /// <summary>
        /// Find a user by his or hers username and password. 
        /// </summary>
        /// <param name="username">Username of a user.</param>
        /// <param name="password">Password of a user.</param>
        /// <param name="organizationId"> The organization the user is linked to </param>
        /// <returns>The user returned based on the username and password.</returns>
        public UserDTO FindUserByUsernameAndPassword(string username, string password, int organizationId)

        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
            var foundUser = (from u in entities.user
                    where u.user_name == username && u.password == password && u.organization_id == organizationId
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
        /// Adds a user to the database.
        /// </summary>
        /// <param name="newUser">The user to be added.</param>
        /// <returns>A message indicating whether the adding was an success.</returns>
        public ResponseMessage AddUser(UserDTO newUser)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from u in entities.user
                                 where u.user_name == newUser.username
                                 select new UserDTO
                                 {
                                     name = u.name,
                                     username = u.user_name,
                                     birthday = u.birthday,
                                     status = u.status,
                                     email = u.email,
                                     user_id = u.user_id,
                                     password = u.password
                                 }).FirstOrDefault();

                if (foundUser == null)
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

                    if (userRole != null)
                        userRole.user.Add(new user
                        {
                            user_id = id,
                            name = newUser.name,
                            user_name = newUser.username,
                            password = newUser.password,
                            birthday = newUser.birthday,
                            email = newUser.email,
                            organization_id = newUser.organization_id,
                            salt = "salt",
                            status = "Active"
                        });

                    entities.SaveChanges();

                    var addedUser = (from u in entities.user
                        where u.user_name == newUser.username
                        select u).FirstOrDefault();

                    if (addedUser != null && addedUser.user_name.Equals(newUser.username))
                    {
                        return new ResponseMessage {IsExecuted = true, Message = "User registered"};
                    }
                    return new ResponseMessage {IsExecuted = false, Message = "Registration failed"};
                }
                return new ResponseMessage {IsExecuted = false, Message = "Username already exists"};
            }
        }

        /// <summary>
        /// Softly deletes a user from the database by changing his or hers status from 
        /// "Active" to "Deleted".
        /// </summary>
        /// <param name="id">Id of the user to be deleted.</param>
        /// <returns>A message indicating wheter the deletion was an success.</returns>
        public ResponseMessage DeleteUser(int id)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var userToBeDeleted = (from u in entities.user
                    where u.user_id == id
                    select u).FirstOrDefault();

                if (userToBeDeleted != null && userToBeDeleted.status.Equals("Active"))
                {
                    userToBeDeleted.status = "Deleted";

                    try
                    {
                        entities.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    return new ResponseMessage { IsExecuted = false, Message = "No user found. Deletion failed" };
                }

                var userThatShouldBeDeletedNow = (from u in entities.user
                    where u.user_id == id
                    select u).FirstOrDefault();

                if (userThatShouldBeDeletedNow != null && userThatShouldBeDeletedNow.status.Equals("Deleted"))
                {
                    return new ResponseMessage { IsExecuted = true, Message = "Deletion completed" };
                }
                return new ResponseMessage { IsExecuted = false, Message = "Deletion failed" };
            }
        }

        /// <summary>
        /// Method to edit a user and its information.
        /// </summary>
        /// <param name="inputUser">The user to be edited.</param>
        public void EditUser(UserDTO inputUser)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundUser = (from user in entities.user
                    where user.user_id == inputUser.user_id
                    select user).FirstOrDefault();

                if (foundUser != null)
                {
                    foundUser.user_name = inputUser.username;
                    foundUser.email = inputUser.email;
                    
                    entities.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Stores a media into the database.
        /// </summary>
        /// <param name="request">An object containing info about the media.</param>
        /// <param name="mediaParser">MediaParser responsible for saving a media into the database.</param>
        public void StoreMedia(RemoteFileInfo request, IMediaParser mediaParser)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                mediaParser.StoreMedia(request, entities);
            }
        }

        /// <summary>
        /// Gets a media path.
        /// </summary>
        /// <param name="id">The ID of the media.</param>
        /// <returns>A path for the found media.</returns>
        public string GetMediaPath(int id)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundMedia = (from med in entities.media
                    where med.media_id == id
                    select med).FirstOrDefault();

                if (foundMedia != null && foundMedia.number_of_downloads == null)
                {
                    foundMedia.number_of_downloads = 1;
                }
                else
                {
                    if (foundMedia != null) foundMedia.number_of_downloads++;
                }

                entities.SaveChanges();

                return (from med in entities.media
                        where med.media_id == id
                        select med.location).FirstOrDefault();
            }
        }

        /// <summary>
        /// Find a media by its ID.
        /// </summary>
        /// <param name="id">ID of the media to search for.</param>
        /// <returns>The found media data type.</returns>
        public MediaDTO FindMediaById(int id)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundMedia = (from med in entities.media
                    where med.media_id == id
                    select med).FirstOrDefault();

                var foundDocument = (from doc in entities.document
                                    where doc.media_id == id
                                    select doc).FirstOrDefault();

                var foundVideo = (from vid in entities.video
                                 where vid.media_id == id
                                 select vid).FirstOrDefault();

                if (foundMedia != null)
                {


                    if (foundDocument != null)
                    {
                        return new MediaDTO
                        {
                            media_id = foundMedia.media_id,
                            user_id = foundMedia.user_id,
                            format_id = foundMedia.format_id,
                            title = foundMedia.title,
                            average_rating = foundMedia.average_rating,
                            date = foundMedia.date,
                            description = foundMedia.description,
                            location = foundMedia.location,
                            number_of_downloads = foundMedia.number_of_downloads,
                            status = foundDocument.status
                        };
                    }
                    if (foundVideo != null)
                    {
                        return new MediaDTO
                        {
                            media_id = foundMedia.media_id,
                            user_id = foundMedia.user_id,
                            format_id = foundMedia.format_id,
                            title = foundMedia.title,
                            average_rating = foundMedia.average_rating,
                            date = foundMedia.date,
                            description = foundMedia.description,
                            location = foundMedia.location,
                            number_of_downloads = foundMedia.number_of_downloads,
                            length = foundVideo.length,
                            number_of_rents = foundVideo.number_of_rents,
                            number_of_trailer_views = foundVideo.number_of_trailer_views
                        };
                    }
                }

                return new MediaDTO
                {
                    title = "No media found"
                };
            }
        }

        /// <summary>
        /// Find a media by its title and organization ID.
        /// </summary>
        /// <param name="title">Title of the media.</param>
        /// <param name="organizationId">Organization ID of the media.</param>
        /// <returns>A list of found medias based on title and organization ID.</returns>
        public List<MediaDTO> FindMediaByTitle(string title, int organizationId)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var medias = new List<MediaDTO>();

                var foundMediaDoc = from med in entities.media
                    join u in entities.user on med.user_id equals u.user_id
                    join doc in entities.document on med.media_id equals doc.media_id
                    where med.title.Contains(title) && u.organization_id == organizationId
                    select new {med, doc};

                var foundMediaVid = from med in entities.media
                    join u in entities.user on med.user_id equals u.user_id
                    join vid in entities.video on med.media_id equals vid.media_id
                    where med.title.Contains(title) && u.organization_id == organizationId
                    select new { med, vid };

                if(foundMediaDoc.Count() > 0)
                {
                    foreach (var med in foundMediaDoc)
                    {
                        medias.Add(new MediaDTO
                        {
                            media_id = med.med.media_id,
                            user_id = med.med.user_id,
                            format_id = med.med.format_id,
                            title = med.med.title,
                            average_rating = med.med.average_rating,
                            date = med.med.date,
                            description = med.med.description,
                            location = med.med.location,
                            number_of_downloads = med.med.number_of_downloads,
                            status = med.doc.status
                        });
                    }
                }
                if (foundMediaVid.Any())
                {
                    foreach (var med in foundMediaVid)
                    {
                        medias.Add(new MediaDTO
                        {
                            media_id = med.med.media_id,
                            user_id = med.med.user_id,
                            format_id = med.med.format_id,
                            title = med.med.title,
                            average_rating = med.med.average_rating,
                            date = med.med.date,
                            description = med.med.description,
                            location = med.med.location,
                            number_of_downloads = med.med.number_of_downloads,
                            length = med.vid.length,
                            number_of_rents = med.vid.number_of_rents,
                            number_of_trailer_views = med.vid.number_of_trailer_views
                        });
                    }
                }
                

                return medias;
            }
        }

        /// <summary>
        /// Find movies based on genre and organization ID.
        /// </summary>
        /// <param name="inputGenre">Genre to search for.</param>
        /// <param name="organizationId">Organization ID to search for.</param>
        /// <returns>A list of found movies as mediaDTOs.</returns>
        public List<MediaDTO> FindMoviesByGenre(string inputGenre, int organizationId)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var medias = new List<MediaDTO>();

                // Search for genres. Genre contains a collection of medias that represent the specific genre
                var gen = (from genreName in entities.genre
                    where genreName.genre1.Equals(inputGenre)
                    select genreName).FirstOrDefault();
                
                    // Traversal of the media collection in the specific genre.
                if (gen != null)
                {
                    var foundMediaVid = (from med in gen.media
                                        join u in entities.user on med.user_id equals u.user_id
                                        join vid in entities.video on med.media_id equals vid.media_id
                                        where u.organization_id == organizationId
                                        select new { med, vid });
                    
                        foreach (var med in foundMediaVid)
                        {
                            if (med.med != null && med.vid != null)
                            {
                                medias.Add(new MediaDTO
                                {
                                    media_id = med.med.media_id,
                                    user_id = med.med.user_id,
                                    format_id = med.med.format_id,
                                    title = med.med.title,
                                    average_rating = med.med.average_rating,
                                    date = med.med.date,
                                    description = med.med.description,
                                    location = med.med.location,
                                    number_of_downloads = med.med.number_of_downloads,
                                    length = med.vid.length,
                                    number_of_rents = med.vid.number_of_rents,
                                    number_of_trailer_views = med.vid.number_of_trailer_views
                                });
                            }
                        }
                    }
                

                return medias;
            }

        }

        /// <summary>
        /// Find a rating for a movie given by a user.
        /// </summary>
        /// <param name="movieId">ID for a movie.</param>
        /// <param name="userId">ID for a user.</param>
        /// <returns>Rating object found for a movie.</returns>
        public rating FindRating(int movieId, int userId)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundRating =
                    entities.rating.SingleOrDefault(rate => rate.media_id == movieId && rate.user_id == userId);

                    return foundRating;
            }
        }

        /// <summary>
        /// Posts a rating for a movie.
        /// </summary>
        /// <param name="rating">Rating value to be posted.</param>
        /// <param name="movieId">Movie object for a which a rating belongs.</param>
        /// <param name="userId">User object for which a rating belongs.</param>
        /// <returns>A message indicating whether the rating was a success.</returns>
        public ResponseMessage PostRating(int rating, int movieId, int userId)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var foundRating = entities.rating.SingleOrDefault(rate => rate.media_id == movieId && rate.user_id == userId); 

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

                    return new ResponseMessage { IsExecuted = true, Message = "Rating added" };
                }

                foundRating.rating1 = rating;

                entities.SaveChanges();

                return new ResponseMessage { IsExecuted = true, Message = "Rating changed" };
            }
        }

        /// <summary>
        /// Find a media(s) by a author's ID.
        /// </summary>
        /// <param name="userId">ID to search by.</param>
        /// <returns>A list of medias by a user/author.</returns>
        public List<MediaDTO> FindMediasByAuthorId(int userId)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                List<MediaDTO> medias = new List<MediaDTO>();

                
                    var userMedia = from med in entities.media
                        where med.user_id == userId
                        select med;

                if (userMedia.Count() > 0)
                {
                    var foundMedia = from u in userMedia
                        join med in entities.media on u.media_id equals med.media_id into mMed
                        from med in mMed.DefaultIfEmpty()
                        join doc in entities.document on u.media_id equals doc.media_id into mDocs
                        from doc in mDocs.DefaultIfEmpty()
                        join vid in entities.video on u.media_id equals vid.media_id into mVids
                        from vid in mVids.DefaultIfEmpty()
                        where doc == null ^ vid == null ^ med == null
                        select new {med, doc, vid};


                    foreach (var med in foundMedia)
                    {
                        if (med.med != null)
                        {
                            if (med.vid == null)
                            {
                                medias.Add(new MediaDTO
                                {
                                    media_id = med.med.media_id,
                                    user_id = med.med.user_id,
                                    format_id = med.med.format_id,
                                    title = med.med.title,
                                    average_rating = med.med.average_rating,
                                    date = med.med.date,
                                    description = med.med.description,
                                    location = med.med.location,
                                    number_of_downloads = med.med.number_of_downloads,
                                    status = med.doc.status
                                });
                            }
                            if (med.doc == null)
                            {
                                medias.Add(new MediaDTO
                                {
                                    media_id = med.med.media_id,
                                    user_id = med.med.user_id,
                                    format_id = med.med.format_id,
                                    title = med.med.title,
                                    average_rating = med.med.average_rating,
                                    date = med.med.date,
                                    description = med.med.description,
                                    location = med.med.location,
                                    number_of_downloads = med.med.number_of_downloads,
                                    length = med.vid.length,
                                    number_of_rents = med.vid.number_of_rents,
                                    number_of_trailer_views = med.vid.number_of_trailer_views
                                });
                            }
                        }
                    }
                }

                return medias;
            }
        }

        public void AddAdminAsRole(int userId)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                var userToBeAdmin = (from u in entities.user
                    where u.user_id == userId
                    select new user
                    {
                        user_id = u.user_id,
                        name = u.name,
                        user_name = u.user_name,
                        password = u.password,
                        birthday = u.birthday,
                        email = u.email,
                        organization_id = u.organization_id,
                        salt = "salt",
                        status = "Active"
                    }).FirstOrDefault();

                if (userToBeAdmin != null)
                {
                    var adminRole = (from r in entities.role
                        where r.role_id == 2
                        select r).FirstOrDefault();

                    if (adminRole != null)
                    {
                        adminRole.user.Add(userToBeAdmin);

                        entities.SaveChanges();
                    }
                }
            }
        }

        public List<MediaDTO> FindMediasByAuthorName(string username, int organizationId)
        {
            using (var entities = _publishITEntities ?? new RentIt09Entities())
            {
                List<MediaDTO> medias = new List<MediaDTO>();

                var userMedia = from med in entities.media
                    join u in entities.user on med.user_id equals u.user_id
                    where u.user_name.Contains(username) && u.organization_id == organizationId
                    select med;

                if (userMedia.Count() > 0)
                {
                    var foundMedia = from u in userMedia
                        join med in entities.media on u.media_id equals med.media_id into mMed
                        from med in mMed.DefaultIfEmpty()
                        join doc in entities.document on u.media_id equals doc.media_id into mDocs
                        from doc in mDocs.DefaultIfEmpty()
                        join vid in entities.video on u.media_id equals vid.media_id into mVids
                        from vid in mVids.DefaultIfEmpty()
                        where doc == null ^ vid == null ^ med == null
                        select new {med, doc, vid};


                    foreach (var med in foundMedia)
                    {
                        if (med.med != null)
                        {
                            if (med.vid == null)
                            {
                                medias.Add(new MediaDTO
                                {
                                    media_id = med.med.media_id,
                                    user_id = med.med.user_id,
                                    format_id = med.med.format_id,
                                    title = med.med.title,
                                    average_rating = med.med.average_rating,
                                    date = med.med.date,
                                    description = med.med.description,
                                    location = med.med.location,
                                    number_of_downloads = med.med.number_of_downloads,
                                    status = med.doc.status
                                });
                            }
                            if (med.doc == null)
                            {
                                medias.Add(new MediaDTO
                                {
                                    media_id = med.med.media_id,
                                    user_id = med.med.user_id,
                                    format_id = med.med.format_id,
                                    title = med.med.title,
                                    average_rating = med.med.average_rating,
                                    date = med.med.date,
                                    description = med.med.description,
                                    location = med.med.location,
                                    number_of_downloads = med.med.number_of_downloads,
                                    length = med.vid.length,
                                    number_of_rents = med.vid.number_of_rents,
                                    number_of_trailer_views = med.vid.number_of_trailer_views
                                });
                            }
                        }
                    }
                }

                return medias;
            }
        }

        /// <summary>
        /// Check whether a rent exists.
        /// </summary>
        /// <param name="userId">User to search by.</param>
        /// <param name="movieId">Movie to search by.</param>
        /// <returns>Boolean indicating whether the renting exists.</returns>
        public bool CheckingIfRentExists(int userId, int movieId)
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