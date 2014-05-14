using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace PublishITService.Resources {
	public class DocumentParser : IMediaParser {

		private IPublishITEntities PublishItEntities { get; set; }

		public void StoreMedia(byte[] mediaStream, RemoteFileInfo request, IPublishITEntities entities)
		{
			this.PublishItEntities = entities;
			string path = @"\RentItServices\RentIt09\resources\media\document\" + request.UserId + @"\" + request.FileName;
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			try
			{
				using (var _FileStream =  new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write))
				{
					_FileStream.Write(mediaStream, 0, mediaStream.Length);
				}
			}
			catch (Exception)
			{
				
				throw;
			}
			saveMedia(path, request);
		}

		private void saveMedia(string path, RemoteFileInfo request)
		{
			if (File.Exists(path)) {
				try {
					media media = new media {
						title = request.Title,
						format_id = 1,
						location = path,
						user_id = request.UserId
						
					};
					PublishItEntities.media.Add(media);
					PublishItEntities.SaveChanges();

					document document = new document {
						media_id = media.media_id,
						status = request.Status
					};
					PublishItEntities.document.Add(document);
					PublishItEntities.SaveChanges();

				} catch (Exception) {
					throw;
				}
			}
		}
	}
}