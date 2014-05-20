using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using PublishITService.DTOs;

namespace PublishITService
{
    [ServiceContract]
    public interface IPublishITService
    {
        // Operation contracts for users
        [OperationContract]
        UserDTO GetUserById(int id);

        [OperationContract]
        UserDTO SignIn(string username, string password, int organization);

        [OperationContract]
        UserDTO GetUserByUserName(string username);

        [OperationContract]
        ResponseMessage RegisterUser(UserDTO user);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ResponseMessage DeleteUser(int id);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ResponseMessage EditUser(UserDTO user);


        // Operation contracts for media

        [OperationContract]
		void UploadMedia(RemoteFileInfo request);

        [OperationContract]
		byte[] DownloadMedia(int id);

	    [OperationContract]
	    string Test();

        [OperationContract]
		string StreamMovie(int userId, int movieId);

        [OperationContract]
        List<MediaDTO> SearchMedia(string title, int organizationId);

        [OperationContract]
        List<MediaDTO> GetMoviesByGenre(string inputGenre, int organizationId);

        [OperationContract]
        List<MediaDTO> GetMediaByAuthorId(int userId);

        [OperationContract]
        List<MediaDTO> GetMediaByAuthorName(string username, int organizationId);

        [OperationContract]
        MediaDTO GetMedia(int id);

        [OperationContract]
        void AddAdminRole(int userId);


        // Operation contracts for rating
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        int GetRating(int movieId, int userId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ResponseMessage PostRating(int rating, int movieId, int userId);
    }

	[MessageContract]
	public class RemoteFileInfo : IDisposable
	{
		[MessageHeader(MustUnderstand = true)] 
		public string FileName;

		[MessageHeader(MustUnderstand = true)]
		public long Length;

		[MessageHeader(MustUnderstand = true)]
		public int UserId;

		[MessageHeader(MustUnderstand = true)]
		public string Title;

		[MessageHeader(MustUnderstand = true)]
		public int GenreId;

		[MessageHeader(MustUnderstand = true)]
		public string Status;

		[MessageBodyMember(Order = 1)] 
		public System.IO.Stream FileStream;

		public void Dispose()
		{
			if (FileStream != null) {
				FileStream.Close();
				FileStream = null;
			}
		}
	}

	[DataContract]
	public class FileContent
	{
		[DataMember]
		public byte[] Content { get; set; }
	}

	[DataContract]
	public class MediaInfo
	{
		[DataMember]
		public string FileName { get; set; }

		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public int UserId { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string Date { get; set; }

		[DataMember]
		public string Status { get; set; }

		[DataMember]
		public string Length { get; set; }

		[DataMember]
		public int GenreId { get; set; }

	}
}
