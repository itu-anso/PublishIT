using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace PublishITService
{
    [ServiceContract]
    public interface IPublishITService
    {
        // Operation contracts for users
        [OperationContract]
        UserDTO GetUserById(int id);

        [OperationContract]
        UserDTO SignIn(string username, string password);

        [OperationContract]
        UserDTO GetUserByUserName(string username);

        [OperationContract]
        ResponseMessage RegisterUser(UserDTO user);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ResponseMessage DeleteUser(UserDTO user);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        ResponseMessage EditUser(UserDTO user);


        // Operation contracts for media

        //[OperationContract]
		//[WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        //bool UploadMedia(MediaInfo mediaInfo, Stream media);
		[OperationContract]
		void UploadMedia(RemoteFileInfo request);

        [OperationContract]
        FileStream DownloadMedia(int id);

        [OperationContract]
		string StreamMedia(int userId, int movieId);

        [OperationContract]
        List<media> SearchMedia(string title);

        [OperationContract]
        List<media> GetMoviesByGenre(string genre);

        [OperationContract]
        media GetMedia(int id);


        // Operation contracts for rating
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        int GetRating(int movieId, int userId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        bool PostRating(int rating, int movieId, int userId);
    }

	[MessageContract]
	public class RemoteFileInfo : IDisposable
	{
		[MessageHeader(MustUnderstand = true)] 
		public string FileName;

		[MessageHeader(MustUnderstand = true)]
		public int Length;

		[MessageHeader(MustUnderstand = true)]
		public int UserId;

		[MessageHeader(MustUnderstand = true)]
		public string Title;

		[MessageHeader(MustUnderstand = true)]
		public int GenreId;

		[MessageHeader(MustUnderstand = true)]
		public string Status;

		[MessageBodyMember(Order = 1)] 
		public Stream FileStream;

		public void Dispose()
		{
			if (FileStream != null) {
				FileStream.Close();
				FileStream = null;
			}
		}
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
