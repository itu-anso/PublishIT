using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace PublishITService.Resources {
	public class DocumentParser : IMediaParser {

		public IPublishITEntities _publishITEntities { get; set; }

		public void StoreMedia(byte[] mediaStream, RemoteFileInfo request, IPublishITEntities entities)
		{
			string path = @"rentit09\resources\media\document\" + request.UserId + @"\" + request.FileName;
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

			if (File.Exists(path))
			{
				try {
					media media = new media
					{
						title = request.Title,
						format_id = 1,
						location = path,
						user_id = request.UserId
					};
					entities.media.Add(media);
					entities.SaveChanges();

					document document = new document
					{
						media_id = media.media_id,
						status = request.Status
					};
					entities.document.Add(document);
					entities.SaveChanges();
					
				}
				catch (Exception)
				{
					throw;
				}
			}
		}
	}
}